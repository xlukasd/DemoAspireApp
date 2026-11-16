using DemoAspireApp.ApiService.Domain.Model;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.AddRedisClient("cache", options => options.DisableHealthChecks = true);

builder.AddSqlServerDbContext<MyDbContext>(connectionName: "MyDemoDatabase", options => options.CommandTimeout = 1);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

string[] summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

app.MapGet("/", () => "API service is running. Navigate to /weatherforecast to see sample data.");

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapGet("/tasks", async (MyDbContext dbContext, IConnectionMultiplexer connectionMux) =>
{
    List<DemoAspireApp.ApiService.Domain.Model.Task> result;

    try
    {
        var allTasks = await dbContext.Tasks.ToListAsync();

        var cache = connectionMux.GetDatabase();
        await cache.StringSetAsync("tasks", JsonSerializer.Serialize(allTasks));

        result = allTasks;
    }
    catch (SqlException sqlException)
    {
        var cache = connectionMux.GetDatabase();

        var tryGetTasksFromCache = await cache.StringGetAsync("tasks");

        if (tryGetTasksFromCache.HasValue)
        {
            string valueAsString = tryGetTasksFromCache.ToString();

            List<DemoAspireApp.ApiService.Domain.Model.Task>? deserializedCachedValue = JsonSerializer.Deserialize<List<DemoAspireApp.ApiService.Domain.Model.Task>>(valueAsString);

            if (deserializedCachedValue is not null)
            {
                result = deserializedCachedValue;

                WriteToStandardOutput($"SQL exception occurred, but tasks were retrieved from cache. Returning cached tasks. SQL exception message: {sqlException.Message}");
            }
            else
            {
                WriteToStandardOutput($"SQL exception occurred, and tasks could not be retrieved from cache. SQL exception message: {sqlException.Message}");
                throw;
            }
        }
        else
        {
            WriteToStandardOutput($"SQL exception occurred, and tasks could not be retrieved from cache. SQL exception message: {sqlException.Message}");
            throw;
        }
    }

    return result;
})
.WithName("GetTasks");

static void WriteToStandardOutput(string message)
{
    Console.WriteLine(message);
}

app.MapDefaultEndpoints();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
