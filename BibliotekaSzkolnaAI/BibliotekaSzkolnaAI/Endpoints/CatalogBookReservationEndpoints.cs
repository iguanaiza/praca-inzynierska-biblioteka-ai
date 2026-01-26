namespace BibliotekaSzkolnaAI.Server.Endpoints
{
    public static class CatalogBookReservationEndpoints
    {
        public static void MapBookReservationEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/bookreservation");

            // GET /api/bookreservation/cart
            group.MapGet("/cart", async (IHttpClientFactory clientFactory, HttpContext httpContext) =>
            {
                var apiClient = clientFactory.CreateClient("Api");
                var response = await apiClient.GetAsync("api/bookreservation/cart");

                if (!response.IsSuccessStatusCode)
                    return Results.Problem("API Error", statusCode: (int)response.StatusCode);

                return Results.Stream(await response.Content.ReadAsStreamAsync(), response.Content.Headers.ContentType?.ToString());
            });

            // GET /api/bookreservation/count
            group.MapGet("/count", async (IHttpClientFactory clientFactory, HttpContext httpContext) =>
            {
                var apiClient = clientFactory.CreateClient("Api");
                var response = await apiClient.GetAsync("api/bookreservation/count");

                if (!response.IsSuccessStatusCode)
                    return Results.Problem("API Error", statusCode: (int)response.StatusCode);

                return Results.Stream(await response.Content.ReadAsStreamAsync(), response.Content.Headers.ContentType?.ToString());
            });

            // POST /api/bookreservation/add/{bookCopyId}
            group.MapPost("/add/{bookCopyId:int}", async (int bookCopyId, IHttpClientFactory clientFactory) =>
            {
                var apiClient = clientFactory.CreateClient("Api");

                var response = await apiClient.PostAsync($"api/bookreservation/add/{bookCopyId}", null);

                if (!response.IsSuccessStatusCode)
                    return Results.Problem("API Error", statusCode: (int)response.StatusCode);

                return Results.Ok();
            });


            // POST /api/bookreservation/finalize
            group.MapPost("/finalize", async (IHttpClientFactory clientFactory) =>
            {
                var apiClient = clientFactory.CreateClient("Api");
                var response = await apiClient.PostAsync("api/bookreservation/finalize", null);

                if (!response.IsSuccessStatusCode)
                    return Results.Problem("API Error", statusCode: (int)response.StatusCode);

                return Results.Ok();
            });

            // DELETE /api/bookreservation/remove/{cartItemId}
            group.MapDelete("/remove/{cartItemId:int}", async (int cartItemId, IHttpClientFactory clientFactory) =>
            {
                var apiClient = clientFactory.CreateClient("Api");

                var response = await apiClient.DeleteAsync($"api/bookreservation/remove/{cartItemId}");

                if (!response.IsSuccessStatusCode)
                    return Results.Problem("API Error", statusCode: (int)response.StatusCode);

                return Results.NoContent();
            });

            // DELETE /api/bookreservation/clear
            group.MapDelete("/clear", async (IHttpClientFactory clientFactory) =>
            {
                var apiClient = clientFactory.CreateClient("Api");
                var response = await apiClient.DeleteAsync("api/bookreservation/clear");

                if (!response.IsSuccessStatusCode)
                    return Results.Problem("API Error", statusCode: (int)response.StatusCode);

                return Results.NoContent();
            });

        }
    }
}
