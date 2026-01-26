using BibliotekaSzkolnaAI.API.Data;
using BibliotekaSzkolnaAI.API.Mapping;
using BibliotekaSzkolnaAI.API.Repositories;
using BibliotekaSzkolnaAI.API.Repositories.Interfaces;
using BibliotekaSzkolnaAI.API.Services.Auth;
using BibliotekaSzkolnaAI.API.Services.Bot;
using BibliotekaSzkolnaAI.API.Services.Catalog;
using BibliotekaSzkolnaAI.API.Services.Catalog.Interfaces;
using BibliotekaSzkolnaAI.API.Services.Management;
using BibliotekaSzkolnaAI.API.Services.Management.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

#region Services
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("https://localhost:7205")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("LocalDb")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 4;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, 
        ValidateAudience = true,  
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true, 
        ValidIssuer = builder.Configuration["Jwt:Issuer"],    
        ValidAudience = builder.Configuration["Jwt:Audience"], 
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])) 
    };
});

builder.Services.AddAuthorization();
builder.Services.AddTransient<IPasswordHasher<ApplicationUser>, PasswordHasher<ApplicationUser>>();
builder.Services.AddScoped<TokenService>();

builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile).Assembly);

builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c => {
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Wpisz 'Bearer' [spacja] i swój token.\r\n\r\nPrzyk³ad: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI...\""
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
       {
           new OpenApiSecurityScheme {
               Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
           },
           new string[] {}
       }
   });
});
#endregion

#region Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IBookReservationRepository, BookReservationRepository>();
builder.Services.AddScoped<IFavoritesRepository, FavoritesRepository>();
builder.Services.AddScoped<ILoansRepository, LoansRepository>();
builder.Services.AddScoped<ICopyRepository, CopyRepository>();
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
#endregion

#region Repository Services
builder.Services.AddScoped<IBookCatalogService, BookCatalogService>();
builder.Services.AddScoped<IBookManagementService, BookManagementService>();
builder.Services.AddScoped<IBookReservationService, BookReservationService>();
builder.Services.AddScoped<IFavoritesService, FavoritesService>();
builder.Services.AddScoped<IBookLoanPublicService, BookLoanPublicService>();
builder.Services.AddScoped<IBookLoanManagementService, BookLoanManagementService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICopyManagementService, CopyManagementService>();
builder.Services.AddScoped<IFileService, LocalFileService>();
builder.Services.AddScoped<IBinService, BinService>();
builder.Services.AddSingleton<IFoundryAgentProvider, FoundryAgentProvider>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
#endregion

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await RoleInitializer.InitializeAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "B³¹d inicjacji bazy.");
    }
}

app.Run();
