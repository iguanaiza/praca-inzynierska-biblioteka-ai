using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Books.Management;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BibliotekaSzkolnaAI.Server.Endpoints
{
    public static class ManagementUserEndpoints
    {
        public static void MapManagementUserEndpoints(this IEndpointRouteBuilder app)
        {
            // GET /api/management/users
            var group = app.MapGroup("/api/management/users")
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin, Librarian" });

            // GET /api/management/users/list
            group.MapGet("/list", async (IHttpClientFactory clientFactory, HttpContext httpContext) =>
            {
                var apiClient = clientFactory.CreateClient("Api");
                var queryString = httpContext.Request.QueryString;

                var response = await apiClient.GetAsync($"api/management/users/list{queryString}");

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Problem("Błąd API podczas pobierania listy użytkowników", statusCode: (int)response.StatusCode);
                }

                return Results.Stream(await response.Content.ReadAsStreamAsync(), response.Content.Headers.ContentType?.ToString());
            });

            // GET /api/management/users/details/{id}
            group.MapGet("/details/{id}", async (string id, IHttpClientFactory clientFactory) =>
            {
                var apiClient = clientFactory.CreateClient("Api");
                var response = await apiClient.GetAsync($"api/management/users/details/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Problem("Nie można pobrać danych użytkownika", statusCode: (int)response.StatusCode);
                }

                return Results.Stream(await response.Content.ReadAsStreamAsync(), response.Content.Headers.ContentType?.ToString());
            });

            // POST /api/management/users/create
            group.MapPost("/create", async ([FromBody] UserCreateDto dto, IHttpClientFactory clientFactory) =>
            {
                var apiClient = clientFactory.CreateClient("Api");
                var response = await apiClient.PostAsJsonAsync("api/management/users/create", dto);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Problem(await response.Content.ReadAsStringAsync(), statusCode: (int)response.StatusCode);
                }

                return Results.Ok();
            });

            // PUT /api/management/users/edit/{id}
            group.MapPut("/edit/{id}", async (string id, [FromBody] UserEditDto dto, IHttpClientFactory clientFactory) =>
            {
                var apiClient = clientFactory.CreateClient("Api");
                var response = await apiClient.PutAsJsonAsync($"api/management/users/edit/{id}", dto);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Problem(await response.Content.ReadAsStringAsync(), statusCode: (int)response.StatusCode);
                }

                return Results.NoContent();
            });

            // PUT /api/management/users/bin/{id}
            group.MapPut("/bin/{id}", async (string id, IHttpClientFactory clientFactory) =>
            {
                var apiClient = clientFactory.CreateClient("Api");
                var response = await apiClient.PutAsync($"api/management/users/bin/{id}", null);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Problem(await response.Content.ReadAsStringAsync(), statusCode: (int)response.StatusCode);
                }

                return Results.NoContent();
            });

            // DELETE /api/management/users/delete/{id}
            group.MapDelete("/delete/{id}", async (string id, IHttpClientFactory clientFactory) =>
            {
                var apiClient = clientFactory.CreateClient("Api");
                var response = await apiClient.DeleteAsync($"api/management/users/delete/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Problem(await response.Content.ReadAsStringAsync(), statusCode: (int)response.StatusCode);
                }

                return Results.NoContent();
            });
        }
    }
}
