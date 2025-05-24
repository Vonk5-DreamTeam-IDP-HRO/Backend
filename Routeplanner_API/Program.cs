using Routeplanner_API;
using Microsoft.EntityFrameworkCore;
using Routeplanner_API.Database_Queries;
using Routeplanner_API.Extensions;
using Routeplanner_API.JWT;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Routeplanner_API.Models;
using Microsoft.Extensions.DependencyInjection;
using Routeplanner_API.Helpers;

//dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer deze toevoegen werkt niet 

DotNetEnv.Env.Load(); // Load environment variables from .env file for local development

var builder = WebApplication.CreateBuilder(args);

// Register DbContext with the validated connection string.
// This ensures that the connection string is valid before the application starts.
var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder
    .AddConfiguration(builder.Configuration.GetSection("Logging"))
    .AddConsole()
    .AddDebug());
var logger = loggerFactory.CreateLogger<Program>(); // Create a logger instance

var connectionString = builder.Configuration.GetValidatedConnectionString(logger); // Pass the logger
builder.Services.AddDbContext<RouteplannerDbContext>(options =>
    options.UseNpgsql(connectionString));

// Register Repositories
builder.Services.AddScoped<ILocationDbQueries, LocationDbQueries>();
builder.Services.AddScoped<IRouteDbQueries, RouteDbQueries>();
builder.Services.AddScoped<IUserDbQueries, UserDbQueries>();

// Register Unit of Works
builder.Services.AddScoped<Routeplanner_API.UoWs.LocationUoW>();
builder.Services.AddScoped<Routeplanner_API.UoWs.RouteUoW>();
builder.Services.AddScoped<Routeplanner_API.UoWs.UserUoW>();

// Register Helpers
builder.Services.AddScoped<IUserHelper, UserHelper>();

// Add AutoMapper and discover profiles in the current assembly
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddControllers();

// JWT
var jwtSettings = builder.Configuration.GetValidatedJwtSettings(logger);

builder.Services.AddIdentity<User, UserPermission>()
    .AddEntityFrameworkStores<RouteplannerDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret!))
    };
});

builder.Services.AddEndpointsApiExplorer();

// Update Swagger configuration with valid OpenAPI version
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();