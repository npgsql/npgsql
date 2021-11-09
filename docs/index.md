---
layout: doc
title: Documentation
---

[![stable](https://img.shields.io/nuget/v/Npgsql.svg?label=stable)](https://www.nuget.org/packages/Npgsql/)
[![unstable](https://img.shields.io/myget/npgsql-unstable/v/npgsql.svg?label=unstable)](https://www.myget.org/feed/npgsql-unstable/package/nuget/Npgsql)
[![next patch](https://img.shields.io/myget/npgsql/v/npgsql.svg?label=next%20patch)](https://www.myget.org/feed/npgsql/package/nuget/Npgsql)
[![build](https://img.shields.io/github/workflow/status/npgsql/npgsql/Build)](https://github.com/npgsql/npgsql/actions)
[![gitter](https://img.shields.io/badge/gitter-join%20chat-brightgreen.svg)](https://gitter.im/npgsql/npgsql)

## Getting Started

The best way to use Npgsql is to install its [nuget package](https://www.nuget.org/packages/Npgsql/).

Npgsql aims to be fully ADO.NET-compatible, its API should feel almost identical to other .NET database drivers.

Here's a basic code snippet to get you started.

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

You can find more info about the ADO.NET API in the [MSDN docs](https://msdn.microsoft.com/en-us/library/h43ks021(v=vs.110).aspx) or in many tutorials on the Internet.
