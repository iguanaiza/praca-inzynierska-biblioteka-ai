namespace BibliotekaSzkolnaAI.Server.Endpoints
{
    public static class CatalogLoansEndpoints
    {
        public static void MapCatalogLoansEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/loans").RequireAuthorization();

            // GET: /api/loans/my-loans
            group.MapGet("/my-loans", async (IHttpClientFactory clientFactory, HttpContext httpContext) =>
            {
                var apiClient = clientFactory.CreateClient("Api");

                var queryString = httpContext.Request.QueryString.ToString();

                var response = await apiClient.GetAsync($"api/loans/my-loans{queryString}");

                if (!response.IsSuccessStatusCode)
                    return Results.Problem("Nie można pobrać listy wypożyczeń", statusCode: (int)response.StatusCode);

                return Results.Stream(await response.Content.ReadAsStreamAsync(), response.Content.Headers.ContentType?.ToString());
            });

            // POST: /api/loans/prolong/{id}
            group.MapPost("/prolong/{loanId:int}", async (int loanId, IHttpClientFactory clientFactory) =>
            {
                var apiClient = clientFactory.CreateClient("Api");
                var response = await apiClient.PostAsync($"api/loans/prolong/{loanId}", null);

                if (!response.IsSuccessStatusCode)
                    return Results.Problem("Nie można przedłużyć wypożyczenia", statusCode: (int)response.StatusCode);

                return Results.Ok();
            });

            // POST: /api/loans/return-request/{id}
            group.MapPost("/return/{loanId:int}", async (int loanId, IHttpClientFactory clientFactory) =>
            {
                var apiClient = clientFactory.CreateClient("Api");

                var response = await apiClient.PostAsync($"api/loans/return-request/{loanId}", null);

                if (!response.IsSuccessStatusCode)
                    return Results.Problem("Nie można zgłosić zwrotu", statusCode: (int)response.StatusCode);

                return Results.Ok();
            });
        }
    }
}