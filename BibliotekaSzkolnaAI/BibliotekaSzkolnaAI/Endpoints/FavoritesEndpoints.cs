namespace BibliotekaSzkolnaAI.Server.Endpoints
{
    public static class FavoritesEndpoints
    {
        public static void MapFavoritesEndpoints(this IEndpointRouteBuilder app)
        {
            // GET /api/favorites
            app.MapGet("/api/favorites", async (IHttpClientFactory clientFactory, HttpContext httpContext) =>
            {
                var apiClient = clientFactory.CreateClient("Api");
                var queryString = httpContext.Request.QueryString.ToString();
                var response = await apiClient.GetAsync($"api/favorites{queryString}");

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Problem("Nie można pobrać ulubionych z API", statusCode: (int)response.StatusCode);
                }

                var contentStream = await response.Content.ReadAsStreamAsync();
                return Results.Stream(contentStream, response.Content.Headers.ContentType?.ToString());
            });


            // GET /api/favorites/ids
            app.MapGet("/api/favorites/ids", async (IHttpClientFactory clientFactory, HttpContext httpContext) =>
            {
                var apiClient = clientFactory.CreateClient("Api");
                var response = await apiClient.GetAsync($"api/favorites/ids");

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Problem("Nie można pobrać ID ulubionych", statusCode: (int)response.StatusCode);
                }
                var contentStream = await response.Content.ReadAsStreamAsync();
                return Results.Stream(contentStream, response.Content.Headers.ContentType?.ToString());
            });

            // POST /api/favorites/{id}
            app.MapPost("/api/favorites/{bookId:int}", async (int bookId, IHttpClientFactory clientFactory) =>
            {
                var apiClient = clientFactory.CreateClient("Api");
                var response = await apiClient.PostAsync($"api/favorites/{bookId}", null);

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Problem("Nie można dodać ulubionego", statusCode: (int)response.StatusCode);
                }

                return Results.Ok();
            });

            // DELETE /api/favorites/{id}
            app.MapDelete("/api/favorites/{bookId:int}", async (int bookId, IHttpClientFactory clientFactory) =>
            {
                var apiClient = clientFactory.CreateClient("Api");
                var response = await apiClient.DeleteAsync($"api/favorites/{bookId}");

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Problem("Nie można usunąć ulubionego", statusCode: (int)response.StatusCode);
                }

                return Results.NoContent();
            });
        }
    }
}
