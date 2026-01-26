using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Books.Management;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BibliotekaSzkolnaAI.Server.Endpoints
{
    public static class ManagementBookEndpoints
    {
        public static void MapManagementBookEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/management/books")

                .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin, Librarian" });

            // GET /api/management/books/list
            group.MapGet("/list", async (IHttpClientFactory clientFactory, HttpContext httpContext) =>
            {
                var apiClient = clientFactory.CreateClient("Api");
                var queryString = httpContext.Request.QueryString;

                var response = await apiClient.GetAsync($"api/management/books/list{queryString}");

                if (!response.IsSuccessStatusCode)
                    return Results.Problem("Błąd API podczas pobierania listy książek", statusCode: (int)response.StatusCode);

                return Results.Stream(await response.Content.ReadAsStreamAsync(), response.Content.Headers.ContentType?.ToString());
            });

            // GET /api/management/books/details/{id}
            group.MapGet("/details/{id}", async (int id, IHttpClientFactory clientFactory) =>
            {
                var apiClient = clientFactory.CreateClient("Api");

                var response = await apiClient.GetAsync($"api/management/books/details/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Problem("Nie można pobrać danych z API", statusCode: (int)response.StatusCode);
                }

                var contentStream = await response.Content.ReadAsStreamAsync();
                return Results.Stream(contentStream, response.Content.Headers.ContentType?.ToString());
            });

            // GET /api/management/books/lookups
            group.MapGet("/lookups", async (IHttpClientFactory clientFactory) =>
            {
                var apiClient = clientFactory.CreateClient("Api");

                var response = await apiClient.GetAsync("api/management/books/lookups");

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Problem("Nie można pobrać słowników", statusCode: (int)response.StatusCode);
                }

                return Results.Stream(
                    await response.Content.ReadAsStreamAsync(),
                    response.Content.Headers.ContentType?.ToString()
                );
            });

            // GET /api/management/books/lookup/{isbn}
            group.MapGet("/lookup/{isbn}", async (string isbn, IHttpClientFactory clientFactory, HttpContext httpContext) =>
            {
                var apiClient = clientFactory.CreateClient("Api");

                var response = await apiClient.GetAsync($"api/management/books/lookup/{isbn}");

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Problem("Nie można pobrać danych z API", statusCode: (int)response.StatusCode);
                }

                var contentStream = await response.Content.ReadAsStreamAsync();
                return Results.Stream(contentStream, response.Content.Headers.ContentType?.ToString());
            });

            // POST /api/management/books/create
            group.MapPost("/create", async ([FromBody] BookCreateDto dto, IHttpClientFactory clientFactory) =>
            {
                var apiClient = clientFactory.CreateClient("Api");
                var response = await apiClient.PostAsJsonAsync("api/management/books/create", dto);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Problem("Nie udało się utworzyć książki", statusCode: (int)response.StatusCode);
                }

                return Results.Ok();
            });

            // PUT api/management/books/edit/{id}
            group.MapPut("/edit/{id:int}", async (int id, [FromBody] BookEditDto dto, IHttpClientFactory clientFactory) =>
            {
                var apiClient = clientFactory.CreateClient("Api");
                var response = await apiClient.PutAsJsonAsync($"api/management/books/edit/{id}", dto);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Problem("Nie udało się zapisać zmian", statusCode: (int)response.StatusCode);
                }

                return Results.NoContent();
            });

            // PUT api/management/books/bin/{id}
            group.MapPut("/bin/{id:int}", async (int id, IHttpClientFactory clientFactory) =>
            {
                var apiClient = clientFactory.CreateClient("Api");
                var response = await apiClient.PutAsync($"api/management/books/bin/{id}", null);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Problem("Nie udało się przenieść do kosza", statusCode: (int)response.StatusCode);
                }

                return Results.NoContent();
            });

            // DELETE api/management/books/delete/{id}
            group.MapDelete("/delete/{id:int}", async (int id, IHttpClientFactory clientFactory) =>
            {
                var apiClient = clientFactory.CreateClient("Api");
                var response = await apiClient.DeleteAsync($"api/management/books/delete/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Problem("Nie udało się usunąć trwale", statusCode: (int)response.StatusCode);
                }

                return Results.NoContent();
            });
        }
    }
}
