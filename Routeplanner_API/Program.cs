using Routeplanner_API;
using Routeplanner_API.Database_Queries;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<LocationDbQueries>();
builder.Services.AddScoped<Routeplanner_API.UoWs.LocationUoW>();

builder.Services.AddScoped<RouteDbQueries>();
builder.Services.AddScoped<Routeplanner_API.UoWs.RouteUoW>();

builder.Services.AddScoped<UserDbQueries>();
builder.Services.AddScoped<Routeplanner_API.UoWs.UserUoW>();

builder.Services.AddScoped<Routeplanner_API.Mappers.LocationMapper>();
builder.Services.AddScoped<Routeplanner_API.Mappers.RouteMapper>();

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
