namespace BibliotekaSzkolnaAI.Server.Endpoints
{
    public static class CatalogBookEndpoints
    {
        public static void MapCatalogBookEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/catalog");

            #region Catalog Endpoints

            // GET /api/catalog/homepage-books
            group.MapGet("/homepage-books", async (IHttpClientFactory clientFactory) =>
            {
                var apiClient = clientFactory.CreateClient("Api");
                var response = await apiClient.GetAsync("api/catalog/homepage-books");

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Problem("Nie można pobrać danych z API", statusCode: (int)response.StatusCode);
                }

                return Results.Stream(await response.Content.ReadAsStreamAsync(), response.Content.Headers.ContentType?.ToString());
            });

            // GET /api/catalog/filter-options
            group.MapGet("/filter-options", async (IHttpClientFactory clientFactory) =>
            {
                var apiClient = clientFactory.CreateClient("Api");
                var response = await apiClient.GetAsync("api/catalog/filter-options");

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Problem("Nie można pobrać opcji filtra z API", statusCode: (int)response.StatusCode);
                }

                return Results.Stream(await response.Content.ReadAsStreamAsync(), response.Content.Headers.ContentType?.ToString());
            });

            // GET /api/catalog/books
            group.MapGet("/books", async (IHttpClientFactory clientFactory, HttpContext httpContext) =>
            {
                var apiClient = clientFactory.CreateClient("Api");

                var queryString = httpContext.Request.QueryString.ToString();

                var response = await apiClient.GetAsync($"api/catalog/books{queryString}");

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Problem("Nie można pobrać danych z API", statusCode: (int)response.StatusCode);
                }

                return Results.Stream(await response.Content.ReadAsStreamAsync(), response.Content.Headers.ContentType?.ToString());
            });

            // GET /api/catalog/setbooks
            group.MapGet("/setbooks", async (IHttpClientFactory clientFactory) =>
            {
                var apiClient = clientFactory.CreateClient("Api");
                var response = await apiClient.GetAsync("api/catalog/setbooks");

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Problem("Nie można pobrać danych z API", statusCode: (int)response.StatusCode);
                }

                return Results.Stream(await response.Content.ReadAsStreamAsync(), response.Content.Headers.ContentType?.ToString());
            });

            // GET /api/catalog/textbooks
            group.MapGet("/textbooks", async (IHttpClientFactory clientFactory) =>
            {
                var apiClient = clientFactory.CreateClient("Api");
                var response = await apiClient.GetAsync("api/catalog/textbooks");

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Problem("Nie można pobrać danych z API", statusCode: (int)response.StatusCode);
                }

                return Results.Stream(await response.Content.ReadAsStreamAsync(), response.Content.Headers.ContentType?.ToString());
            });

            // GET /api/catalog/details/{id}
            group.MapGet("/details/{id:int}", async (int id, IHttpClientFactory clientFactory) =>
            {
                var apiClient = clientFactory.CreateClient("Api");
                var response = await apiClient.GetAsync($"api/catalog/details/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    return Results.Problem("Nie można pobrać danych z API", statusCode: (int)response.StatusCode);
                }

                return Results.Stream(await response.Content.ReadAsStreamAsync(), response.Content.Headers.ContentType?.ToString());
            });

            #endregion
        }
    }
}
