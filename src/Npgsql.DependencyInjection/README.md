Npgsql is the open source .NET data provider for PostgreSQL. It allows you to connect and interact with PostgreSQL server using .NET.

This package helps set up Npgsql in applications using dependency injection, notably ASP.NET applications. It allows easy configuration of your Npgsql connections and registers the appropriate services in your DI container. 

For example, if using the ASP.NET minimal web API, simply use the following to register Npgsql:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddNpgsqlDataSource("Host=pg_server;Username=test;Password=test;Database=test");
```

This registers a transient [`NpgsqlConnection`](https://www.npgsql.org/doc/api/Npgsql.NpgsqlConnection.html) which can get injected into your controllers:

```csharp
app.MapGet("/", async (NpgsqlConnection connection) =>
{
    await connection.OpenAsync();
    await using var command = new NpgsqlCommand("SELECT number FROM data LIMIT 1", connection);
    return "Hello World: " + await command.ExecuteScalarAsync();
});
```

But wait! If all you want is to execute some simple SQL, just use the singleton [`NpgsqlDataSource`](https://www.npgsql.org/doc/api/Npgsql.NpgsqlDataSource.html) to execute a command directly:

```csharp
app.MapGet("/", async (NpgsqlDataSource dataSource) =>
{
    await using var command = dataSource.CreateCommand("SELECT number FROM data LIMIT 1");
    return "Hello World: " + await command.ExecuteScalarAsync();
});
```

[`NpgsqlDataSource`](https://www.npgsql.org/doc/api/Npgsql.NpgsqlDataSource.html) can also come in handy when you need more than one connection:

```csharp
app.MapGet("/", async (NpgsqlDataSource dataSource) =>
{
    await using var connection1 = await dataSource.OpenConnectionAsync();
    await using var connection2 = await dataSource.OpenConnectionAsync();
    // Use the two connections...
});
```

Finally, the `AddNpgsqlDataSource` method also accepts a lambda parameter allowing you to configure aspects of Npgsql beyond the connection string.

For more information, [see the Npgsql documentation](https://www.npgsql.org/doc/index.html).
