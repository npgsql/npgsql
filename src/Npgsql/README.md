Npgsql is the open source .NET data provider for PostgreSQL. It allows you to connect and interact with PostgreSQL server using .NET.

## Quickstart

Here's a basic code snippet to get you started:

```csharp
var connString = "Host=myserver;Username=mylogin;Password=mypass;Database=mydatabase";

await using var conn = new NpgsqlConnection(connString);
await conn.OpenAsync();

// Insert some data
await using (var cmd = new NpgsqlCommand("INSERT INTO data (some_field) VALUES (@p)", conn))
{
    cmd.Parameters.AddWithValue("p", "Hello world");
    await cmd.ExecuteNonQueryAsync();
}

// Retrieve all rows
await using (var cmd = new NpgsqlCommand("SELECT some_field FROM data", conn))
await using (var reader = await cmd.ExecuteReaderAsync())
{
while (await reader.ReadAsync())
    Console.WriteLine(reader.GetString(0));
}
```

## Key features

* High-performance PostgreSQL driver. Regularly figures in the top contenders on the [TechEmpower Web Framework Benchmarks](https://www.techempower.com/benchmarks/).
* Full support of most PostgreSQL types, including advanced ones such as arrays, enums, ranges, multiranges, composites, JSON, PostGIS and others.
* Highly-efficient bulk import/export API.
* Failover, load balancing and general multi-host support.
* Great integration with Entity Framework Core via [Npgsql.EntityFrameworkCore.PostgreSQL](https://www.nuget.org/packages/Npgsql.EntityFrameworkCore.PostgreSQL). 

For the full documentation, please visit [the Npgsql website](https://www.npgsql.org).

## Related packages

* The Entity Framework Core provider that works with this provider is [Npgsql.EntityFrameworkCore.PostgreSQL](https://www.nuget.org/packages/Npgsql.EntityFrameworkCore.PostgreSQL).
* Spatial plugin to work with PostgreSQL PostGIS: [Npgsql.NetTopologySuite](https://www.nuget.org/packages/Npgsql.NetTopologySuite)
* NodaTime plugin to use better date/time types with PostgreSQL: [Npgsql.NodaTime](https://www.nuget.org/packages/Npgsql.NodaTime)
* OpenTelemetry support can be set up with [Npgsql.OpenTelemetry](https://www.nuget.org/packages/Npgsql.OpenTelemetry)