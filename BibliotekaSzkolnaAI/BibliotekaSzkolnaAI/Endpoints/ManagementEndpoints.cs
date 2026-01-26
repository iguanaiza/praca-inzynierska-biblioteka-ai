using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Bin;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.BookCopies;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Books.Management;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BibliotekaSzkolnaAI.Server.Endpoints
{
    public static class ManagementEndpoints
    {
        public static void MapManagementEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/management")

                .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin, Librarian" });

            #region DASHBOARD
            // GET /api/management/dashboard
            group.MapGet("/dashboard", async (IHttpClientFactory clientFactory) =>
            {
                var apiClient = clientFactory.CreateClient("Api");
                var response = await apiClient.GetAsync("api/management/dashboard");

                if (!response.IsSuccessStatusCode)
                    return Results.Problem("Błąd API podczas pobierania dashboardu", statusCode: (int)response.StatusCode);

                return Results.Stream(await response.Content.ReadAsStreamAsync(), response.Content.Headers.ContentType?.ToString());
            });
            #endregion

            #region LOANS (Wypożyczenia)
            // GET /api/management/loans
            group.MapGet("/loans", async (IHttpClientFactory clientFactory, HttpContext httpContext) =>
            {
                var apiClient = clientFactory.CreateClient("Api");
                var queryString = httpContext.Request.QueryString;

                var response = await apiClient.GetAsync($"api/management/loans{queryString}");

                if (!response.IsSuccessStatusCode)
                    return Results.Problem("Błąd pobierania listy wypożyczeń", statusCode: (int)response.StatusCode);

                return Results.Stream(await response.Content.ReadAsStreamAsync(), response.Content.Headers.ContentType?.ToString());
            });

            // POST /api/management/loans/approve/{id}
            group.MapPost("/loans/approve/{loanId:int}", async (int loanId, IHttpClientFactory clientFactory) =>
            {
                var apiClient = clientFactory.CreateClient("Api");
                var response = await apiClient.PostAsync($"api/management/loans/approve/{loanId}", null);

                if (!response.IsSuccessStatusCode)
                    return Results.Problem("Nie udało się zatwierdzić wypożyczenia", statusCode: (int)response.StatusCode);

                return Results.Ok();
            });

            // POST /api/management/loans/finalize-return/{id}
            group.MapPost("/loans/finalize-return/{loanId:int}", async (int loanId, IHttpClientFactory clientFactory) =>
            {
                var apiClient = clientFactory.CreateClient("Api");
                var response = await apiClient.PostAsync($"api/management/loans/finalize-return/{loanId}", null);

                if (!response.IsSuccessStatusCode)
                    return Results.Problem("Nie udało się sfinalizować zwrotu", statusCode: (int)response.StatusCode);

                return Results.Ok();
            });
            #endregion

            #region BIN (Kosz)

            // GET /api/management/bin
            group.MapGet("/bin", async (IHttpClientFactory clientFactory) =>
            {
                var client = clientFactory.CreateClient("Api");
                var response = await client.GetAsync("api/management/bin");
                if (!response.IsSuccessStatusCode) return Results.Problem("Błąd kosza", statusCode: (int)response.StatusCode);
                return Results.Stream(await response.Content.ReadAsStreamAsync(), response.Content.Headers.ContentType?.ToString());
            });

            // POST /api/management/bin/restore
            group.MapPost("/bin/restore", async ([FromBody] RestoreItemDto dto, IHttpClientFactory clientFactory) =>
            {
                var client = clientFactory.CreateClient("Api");
                var response = await client.PostAsJsonAsync("api/management/bin/restore", dto);
                if (!response.IsSuccessStatusCode)
                    return Results.Problem(await response.Content.ReadAsStringAsync(), statusCode: (int)response.StatusCode);
                return Results.Ok();
            });

            // POST /api/management/bin/hard-delete
            group.MapPost("/bin/hard-delete", async ([FromBody] RestoreItemDto dto, IHttpClientFactory clientFactory) =>
            {
                var client = clientFactory.CreateClient("Api");
                var response = await client.PostAsJsonAsync("api/management/bin/hard-delete", dto);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Problem(await response.Content.ReadAsStringAsync(), statusCode: (int)response.StatusCode);
                }

                return Results.Ok();
            });

            #endregion
        }
    }
}