using BibliotekaSzkolnaAI.Server.Components;
using BibliotekaSzkolnaAI.Server.Endpoints;
using BibliotekaSzkolnaAI.Server.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Syncfusion.Blazor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddHttpContextAccessor();

builder.Services.AddTransient<ApiTokenHandler>();


var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"];

if (string.IsNullOrEmpty(apiBaseUrl))
{
    throw new InvalidOperationException("Nie znaleziono adresu API (ApiSettings:BaseUrl) w appsettings.json");
}

builder.Services.AddHttpClient("Api", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
})
.AddHttpMessageHandler<ApiTokenHandler>();

builder.Services.AddScoped(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    return factory.CreateClient("Api");
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/konto/zaloguj";
        options.LogoutPath = "/konto/wyloguj";
        options.AccessDeniedPath = "/odmowa-dostepu";

        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.HttpOnly = true;
    });

builder.Services.AddAuthorization();
builder.Services.AddScoped<AuthenticationStateProvider, PersistingServerAuthenticationStateProvider>();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSyncfusionBlazor();
builder.Services.AddScoped<GlobalUiService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapStaticAssets();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

#region API Endpoints
app.MapAuthenticationEndpoints();
app.MapChatEndpoints();
app.MapCatalogBookEndpoints();
app.MapFavoritesEndpoints();
app.MapBookReservationEndpoints();
app.MapCatalogLoansEndpoints();
app.MapManagementEndpoints();
app.MapManagementBookEndpoints();
app.MapManagementCopiesEndpoints();
app.MapManagementUserEndpoints();
#endregion

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BibliotekaSzkolnaAI.Client._Imports).Assembly);

app.Run();