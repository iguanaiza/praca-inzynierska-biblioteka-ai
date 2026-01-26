using BibliotekaSzkolnaAI.Shared.DataTransferObjects.BookCopies;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Books.Management;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BibliotekaSzkolnaAI.Server.Endpoints
{
    public static class ManagementCopiesEndpoints
    {
        public static void MapManagementCopiesEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/management/copies")

                .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin, Librarian" });

            // GET /api/management/copies
            group.MapGet("/list", async (IHttpClientFactory clientFactory, HttpContext httpContext) =>
            {
                var apiClient = clientFactory.CreateClient("Api");
                var queryString = httpContext.Request.QueryString;

                var response = await apiClient.GetAsync($"api/management/copies/list{queryString}");

                if (!response.IsSuccessStatusCode)
                    return Results.Problem("Błąd pobierania egzemplarzy", statusCode: (int)response.StatusCode);

                return Results.Stream(await response.Content.ReadAsStreamAsync(), response.Content.Headers.ContentType?.ToString());
            });

            // GET /api/management/copies/details/{id}
            group.MapGet("/details/{id}", async (int id, IHttpClientFactory clientFactory) =>
            {
                var apiClient = clientFactory.CreateClient("Api");


                var response = await apiClient.GetAsync($"api/management/copies/details/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Problem("Nie można pobrać danych z API", statusCode: (int)response.StatusCode);
                }

                var contentStream = await response.Content.ReadAsStreamAsync();
                return Results.Stream(contentStream, response.Content.Headers.ContentType?.ToString());
            });

            // GET /api/management/copies/next-inventory-number
            group.MapGet("/next-inventory-number", async (IHttpClientFactory clientFactory) =>
            {
                var apiClient = clientFactory.CreateClient("Api");
                var response = await apiClient.GetAsync("api/management/copies/next-inventory-number");

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Problem("Błąd pobierania numeru", statusCode: (int)response.StatusCode);
                }

                var content = await response.Content.ReadAsStringAsync();
                return Results.Ok(int.Parse(content));
            });

            // POST /api/management/copies/create
            group.MapPost("/create", async ([FromBody] CopyCreateDto dto, IHttpClientFactory clientFactory) =>
            {
                var apiClient = clientFactory.CreateClient("Api");
                var response = await apiClient.PostAsJsonAsync("api/management/copies/create", dto);

                if (!response.IsSuccessStatusCode)
                    return Results.Problem(await response.Content.ReadAsStringAsync(), statusCode: (int)response.StatusCode);

                return Results.Ok();
            });

            // PUT /api/management/copies/edit/{id}
            group.MapPut("/edit/{id:int}", async (int id, [FromBody] CopyEditDto dto, IHttpClientFactory clientFactory) =>
            {
                var apiClient = clientFactory.CreateClient("Api");
                var response = await apiClient.PutAsJsonAsync($"api/management/copies/edit/{id}", dto);

                if (!response.IsSuccessStatusCode)
                    return Results.Problem(await response.Content.ReadAsStringAsync(), statusCode: (int)response.StatusCode);

                return Results.NoContent();
            });

            // PUT /api/management/copies/bin/{id}
            group.MapPut("/bin/{id:int}", async (int id, IHttpClientFactory clientFactory) =>
            {
                var apiClient = clientFactory.CreateClient("Api");
                var response = await apiClient.PutAsync($"api/management/copies/bin/{id}", null);

                if (!response.IsSuccessStatusCode)
                    return Results.Problem("Błąd usuwania", statusCode: (int)response.StatusCode);

                return Results.NoContent();
            });

            // DELETE api/management/copies/delete/{id}
            group.MapDelete("/delete/{id:int}", async (int id, IHttpClientFactory clientFactory) =>
            {
                var apiClient = clientFactory.CreateClient("Api");
                var response = await apiClient.DeleteAsync($"api/management/copies/delete/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Problem("Nie udało się usunąć trwale", statusCode: (int)response.StatusCode);
                }

                return Results.NoContent();
            });
        }
    }
}

