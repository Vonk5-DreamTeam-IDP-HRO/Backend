using Routeplanner_API;
using Microsoft.EntityFrameworkCore;
using Routeplanner_API.Data;
using Routeplanner_API.Database_Queries;
using Routeplanner_API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Register DbContext with the validated connection string.
// This ensures that the connection string is valid before the application starts.
var connectionString = builder.Configuration.GetValidatedConnectionString();
builder.Services.AddDbContext<RouteplannerDbContext>(options =>
    options.UseNpgsql(connectionString));

// Register Repositories
builder.Services.AddScoped<ILocationDbQueries, LocationDbQueries>();
builder.Services.AddScoped<IRouteDbQueries, RouteDbQueries>();
// TODO: Register other repositories (IRouteRepository, IUserRepository) here

// Register Unit of Works / Services
builder.Services.AddScoped<Routeplanner_API.UoWs.LocationUoW>();
builder.Services.AddScoped<Routeplanner_API.UoWs.RouteUoW>();
builder.Services.AddScoped<Routeplanner_API.UoWs.UserUoW>();


builder.Services.AddScoped<UserDbQueries>();     // Keep for now

// Add AutoMapper and discover profiles in the current assembly
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
