using Routeplanner_API;
using Microsoft.EntityFrameworkCore;
using Routeplanner_API.Data;
using Routeplanner_API.Database_Queries;
using Routeplanner_API.Extensions;
using Microsoft.OpenApi.Models; // Add this line

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

// Register Unit of Works / Services
builder.Services.AddScoped<Routeplanner_API.UoWs.LocationUoW>();
builder.Services.AddScoped<Routeplanner_API.UoWs.RouteUoW>();
builder.Services.AddScoped<Routeplanner_API.UoWs.UserUoW>();

// Add AutoMapper and discover profiles in the current assembly
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Update Swagger configuration with valid OpenAPI version
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Routeplanner API",
        Version = "v1",
        Description = "API for route planning application",
        Contact = new OpenApiContact
        {
            Name = "Your Team Name",
            Email = "contact@example.com"
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Routeplanner API v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
