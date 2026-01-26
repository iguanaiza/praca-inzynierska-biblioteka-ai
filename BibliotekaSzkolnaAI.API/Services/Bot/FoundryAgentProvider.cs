#region Usings
using Azure.Core;
using Azure.Identity;
using BibliotekaSzkolnaAI.API.Repositories.Interfaces;
using BibliotekaSzkolnaAI.Shared.Models;
using BibliotekaSzkolnaAI.Shared.Models.Params;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
#endregion
using Azure.AI.Agents.Persistent;

namespace BibliotekaSzkolnaAI.API.Services.Bot;

public interface IFoundryAgentProvider
{
    bool IsConfigured { get; }
    PersistentAgent? Agent { get; }
    PersistentAgentsClient? Client { get; }
    string? ThreadId { get; }

    Task<ChatResponseDto> GetAnswerAsync(string userMessage, string? threadId);
}

public class FoundryAgentProvider : IFoundryAgentProvider
{
    #region Properties
    public bool IsConfigured { get; private set; }
    public PersistentAgentsClient? Client { get; private set; }
    public string? ThreadId { get; private set; }
    public PersistentAgent? Agent { get; private set; }
    public string? InitError { get; private set; }

    private readonly IServiceScopeFactory _scopeFactory;
    #endregion

    public FoundryAgentProvider(IConfiguration config, IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
        IsConfigured = false;

        var endpoint = config["AzureAI:AzureAIFoundryProjectEndpoint"];
        var apiKey = config["AzureAI:AzureAIFoundryApiKey"];
        var agentId = config["AzureAI:AzureAIFoundryAgentId"];

        if (string.IsNullOrWhiteSpace(endpoint))
        {
            InitError = "Brak konfiguracji endpoint lub agentId w ustawieniach.";
            Console.WriteLine(InitError);
            return; 
        }

        try
        {
            Client = new PersistentAgentsClient(endpoint, new DefaultAzureCredential());

            PersistentAgentThread agentThread = Client.Threads.CreateThread();
            ThreadId = agentThread.Id;

            Agent = Client.Administration.GetAgent(agentId);

            EnsureAgentHasTools().Wait();

            IsConfigured = true;
        }
        catch (Exception ex)
        {
            InitError = ex.Message;
            if (ex.InnerException != null)
            {
                InitError += $" | Inner: {ex.InnerException.Message}";
            }
        }
    }

    private async Task EnsureAgentHasTools()
    {
        try
        {
            var parametersJson = BinaryData.FromObjectAsJson(new
            {
                type = "object",
                properties = new
                {
                    query = new
                    {
                        type = "string",
                        description = "Tytuł książki, autor lub słowo kluczowe"
                    }
                },
                required = new[] { "query" }
            }, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            FunctionToolDefinition searchTool = new FunctionToolDefinition(
                name: "search_books",
                description: "Wyszukuje książki w bazie danych biblioteki. Zwraca dostępność.",
                parameters: parametersJson
            );

            bool hasTool = Agent.Tools.Any(t => t is FunctionToolDefinition ft && ft.Name == "search_books");

            if (!hasTool)
            {
                PersistentAgent updatedAgent = await Client.Administration.UpdateAgentAsync(
                    Agent.Id,
                    tools: new ToolDefinition[] { searchTool }
                );

                Agent = updatedAgent;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ostrzeżenie (Tools): {ex.Message}");
        }
    }

    public async Task<ChatResponseDto> GetAnswerAsync(string userMessage, string? threadId)
    {
        if (!IsConfigured || Client == null || Agent == null)
        {
            return new ChatResponseDto
            {
                IsSuccess = false,
                ErrorMessage = $"BŁĄD KONFIGURACJI: {InitError ?? "Nieznany błąd startu."}"
            };
        }

        string activeThreadId = !string.IsNullOrEmpty(threadId) ? threadId : ThreadId!;

        try
        {
            await Client.Messages.CreateMessageAsync(
                activeThreadId,
                MessageRole.User,
                userMessage);

            ThreadRun run = await Client.Runs.CreateRunAsync(activeThreadId, Agent.Id);

            do
            {
                await Task.Delay(100);
                run = await Client.Runs.GetRunAsync(activeThreadId, run.Id);

                if (run.Status == RunStatus.RequiresAction
                    && run.RequiredAction is SubmitToolOutputsAction submitAction)
                {
                    var toolOutputs = new List<ToolOutput>();

                    foreach (var toolCall in submitAction.ToolCalls)
                    {
                        if (toolCall is RequiredFunctionToolCall functionCall && functionCall.Name == "search_books")
                        {
                            using var doc = JsonDocument.Parse(functionCall.Arguments);
                            string query = "";

                            if (doc.RootElement.TryGetProperty("query", out var qElement))
                            {
                                query = qElement.GetString() ?? "";
                            }

                            string dbResult = await SearchDatabaseAsync(query);

                            toolOutputs.Add(new ToolOutput(toolCall, dbResult));
                        }
                    }

                    if (toolOutputs.Count > 0)
                    {
                        var payload = new
                        {
                            tool_outputs = toolOutputs.Select(t => new
                            {
                                tool_call_id = t.ToolCallId,
                                output = t.Output
                            }).ToArray()
                        };
                        var content = RequestContent.Create(payload);
                        await Client.Runs.SubmitToolOutputsToRunAsync(activeThreadId, run.Id, content);
                        await Task.Delay(200);
                        run = await Client.Runs.GetRunAsync(activeThreadId, run.Id);
                    }
                }
            }
            while (run.Status == RunStatus.Queued || run.Status == RunStatus.InProgress);

            if (run.Status == RunStatus.Completed)
            {
                var messages = Client.Messages.GetMessages(activeThreadId, order: ListSortOrder.Descending);
                foreach (var msg in messages)
                {
                    if (msg.Role == MessageRole.Agent)
                    {
                        foreach (var contentItem in msg.ContentItems)
                        {
                            if (contentItem is MessageTextContent textItem)
                            {
                                string cleanText = Regex.Replace(textItem.Text, @"【.*?】", "").Trim();
                                return new ChatResponseDto { Response = cleanText, ThreadId = activeThreadId, IsSuccess = true };
                            }
                        }
                    }
                }
            }

            string errorDetails = run.LastError != null ? $"{run.LastError.Code}: {run.LastError.Message}" : "Brak szczegółów";
            return new ChatResponseDto { IsSuccess = false, ErrorMessage = $"Status: {run.Status}. Info: {errorDetails}" };
        }
        catch (Exception ex)
        {
            return new ChatResponseDto { IsSuccess = false, ErrorMessage = ex.Message };
        }
    }

    private async Task<string> SearchDatabaseAsync(string query)
    {
        try
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var bookRepo = scope.ServiceProvider.GetRequiredService<IBookRepository>();

                var filterTitle = new BookQueryParams { Title = query };
                var result = await bookRepo.GetBooksAsync(filterTitle, includeHidden: false);

                if (result.TotalCount == 0)
                {
                    var filterAuthor = new BookQueryParams { BookAuthor = query };
                    result = await bookRepo.GetBooksAsync(filterAuthor, includeHidden: false);
                }

                if (result.TotalCount == 0)
                {
                    return "Nie znaleziono książek pasujących do zapytania.";
                }

                Console.WriteLine($"[DB] Znaleziono {result.TotalCount} wyników.");

                var sb = new StringBuilder();
                sb.AppendLine($"ZNALEZIONO {result.TotalCount} POZYCJI (max 5):");

                foreach (var book in result.Items.Take(5))
                {
                    string dostepnosc = book.AvailableCopyCount > 0 ? "Dostępna" : "Wypożyczona";
                    sb.AppendLine("---");
                    sb.AppendLine($"TYTUŁ: '{book.Title}'");
                    sb.AppendLine($"AUTOR: {book.BookAuthor?.Name} {book.BookAuthor?.Surname}");
                    sb.AppendLine($"STAN: {dostepnosc} ({book.AvailableCopyCount} szt).");
                    sb.AppendLine($"ID_DO_LINKU: {book.Id}");
                }
                return sb.ToString();
            }
        }
        catch (Exception ex)
        {
            return $"Błąd techniczny bazy: {ex.Message}";
        }
    }
}