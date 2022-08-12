# Npgsql - the .NET data provider for PostgreSQL

[![stable](https://img.shields.io/nuget/v/Npgsql.svg?label=stable)](https://www.nuget.org/packages/Npgsql/)
[![next patch](https://img.shields.io/myget/npgsql/v/npgsql.svg?label=next%20patch)](https://www.myget.org/feed/npgsql/package/nuget/Npgsql)
[![daily builds (vnext)](https://img.shields.io/myget/npgsql-vnext/v/npgsql.svg?label=vnext)](https://www.myget.org/feed/npgsql-vnext/package/nuget/Npgsql)
[![build](https://github.com/npgsql/npgsql/actions/workflows/build.yml/badge.svg)](https://github.com/npgsql/npgsql/actions/workflows/build.yml)
[![gitter](https://img.shields.io/badge/gitter-join%20chat-brightgreen.svg)](https://gitter.im/npgsql/npgsql)

## What is Npgsql?

Npgsql is the open source .NET data provider for PostgreSQL. It allows you to connect and interact with PostgreSQL server using .NET.

For the full documentation, please visit [the Npgsql website](https://www.npgsql.org). For the Entity Framework Core provider that works with this provider, see [Npgsql.EntityFrameworkCore.PostgreSQL](https://github.com/npgsql/efcore.pg).

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

For the full documentation, please visit the Npgsql website at [https://www.npgsql.org](https://www.npgsql.org).
