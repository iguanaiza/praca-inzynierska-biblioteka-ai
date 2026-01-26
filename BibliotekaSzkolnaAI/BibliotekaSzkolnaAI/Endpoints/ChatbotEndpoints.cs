using BibliotekaSzkolnaAI.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace BibliotekaSzkolnaAI.Server.Endpoints
{
    public static class ChatEndpoints
    {
        public static void MapChatEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/chat");

            // POST /api/chat
            group.MapPost("/", async ([FromBody] ChatRequestDto request, IHttpClientFactory clientFactory) =>
            {
                var apiClient = clientFactory.CreateClient("Api");

                var response = await apiClient.PostAsJsonAsync("api/chat", request);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Problem("Błąd komunikacji z API AI.", statusCode: (int)response.StatusCode);
                }

                return Results.Stream(await response.Content.ReadAsStreamAsync(), "application/json");
            });
        }
    }
}
