using System;
using Npgsql;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateSlimBuilder(args);
builder.Logging.ClearProviders();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

var app = builder.Build();

var sampleTodos = new Todo[] {
    new(1, "Walk the dog"),
    new(2, "Do the dishes", DateOnly.FromDateTime(DateTime.Now)),
    new(3, "Do the laundry", DateOnly.FromDateTime(DateTime.Now.AddDays(1))),
    new(4, "Clean the bathroom"),
    new(5, "Clean the car", DateOnly.FromDateTime(DateTime.Now.AddDays(2)))
};

var todosApi = app.MapGroup("/todos");
todosApi.MapGet("/", () => sampleTodos);

// Keeping because it is in the template but not actually benchmarked.
todosApi.MapGet("/{id}", (int id) =>
    sampleTodos.FirstOrDefault(a => a.Id == id) is { } todo
        ? Results.Ok(todo)
        : Results.NotFound());

// Don' actually run it, just be sure it's rooted.
app.Lifetime.ApplicationStarted.Register(() => Console.WriteLine("Application started. Press Ctrl+C to shut down."));
app.Start();
await app.StopAsync();

var connectionString = Environment.GetEnvironmentVariable("NPGSQL_TEST_DB")
                       ?? "Server=localhost;Username=npgsql_tests;Password=npgsql_tests;Database=npgsql_tests;Timeout=0;Command Timeout=0";

var dataSourceBuilder = new NpgsqlSlimDataSourceBuilder(connectionString);
await using var dataSource = dataSourceBuilder.Build();

await using var conn = dataSource.CreateConnection();
await conn.OpenAsync();
await using var cmd = new NpgsqlCommand("SELECT 'Hello World'", conn);
await using var reader = await cmd.ExecuteReaderAsync();
if (!await reader.ReadAsync())
    throw new Exception("Got nothing from the database");

var value = reader.GetFieldValue<string>(0);
if (value != "Hello World")
    throw new Exception($"Got {value} instead of the expected 'Hello World'");

public record Todo(int Id, string? Title, DateOnly? DueBy = null, bool IsComplete = false);

[JsonSerializable(typeof(Todo[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}
