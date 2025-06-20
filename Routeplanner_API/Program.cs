﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Routeplanner_API.Database_Queries;
using Routeplanner_API.Extensions;
using Routeplanner_API.Helpers;
using Routeplanner_API.JWT;
using Routeplanner_API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

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
var validatedJwtSettings = builder.Configuration.GetValidatedJwtSettings(logger);

logger.LogInformation("JWT Config: Issuer={Issuer}, Audience={Audience}, Secret.Length={SecretLength}",
    validatedJwtSettings.Issuer,
    validatedJwtSettings.Audience,
    validatedJwtSettings.Secret?.Length);

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.Configure<JwtSettings>(options =>
{
    options.Secret = validatedJwtSettings.Secret;
    options.Issuer = validatedJwtSettings.Issuer;
    options.Audience = validatedJwtSettings.Audience;
    options.ExpiryMinutes = validatedJwtSettings.ExpiryMinutes;
});

// builder.Services.AddIdentity<User, UserPermission>()
//     .AddEntityFrameworkStores<RouteplannerDbContext>()
//     .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = validatedJwtSettings.Issuer,
        ValidAudience = validatedJwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(validatedJwtSettings.Secret!))
    };
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError(context.Exception, "JWT Authentication failed: {Message}", context.Exception.Message);
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddEndpointsApiExplorer();

// Update Swagger configuration with valid OpenAPI version
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Routeplanner API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});


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