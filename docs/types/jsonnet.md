# Json.NET Type Plugin

The Json.NET plugin allows applications to automatically make use of [Newtonsoft Json.NET](http://www.newtonsoft.com/json) when reading and writing JSON data. Note that Npgsql includes built-in support for [`System.Text.Json`](https://learn.microsoft.com/dotnet/standard/serialization/system-text-json/overview?pivots=dotnet-6-0), without requiring extra package; this page only covers using Newtonsoft Json.NET.

[PostgreSQL natively supports two JSON types](https://www.postgresql.org/docs/current/static/datatype-json.html): `jsonb` and `json`. Out of the box, Npgsql allows reading and writing these types as strings and provides no further processing to avoid taking a dependency on an external JSON library, forcing Npgsql users to serialize and deserialize JSON values themselves. The Json.NET plugin removes this burden from users by performing serialization/deserialization within Npgsql itself.

## Setup

To avoid forcing a dependency on the Json.NET library for users not using spatial, Json.NET support is delivered as a separate plugin. To use the plugin, simply add a dependency on [Npgsql.Json.NET](https://www.nuget.org/packages/Npgsql.Json.NET) and set it up in one of the following ways:

### [NpgsqlDataSource](#tab/datasource)

> [!NOTE]
> `NpgsqlDataSource` was introduced in Npgsql 7.0, and is the recommended way to manage type mapping. If you're using an older version, see the other methods.

```c#
var dataSourceBuilder = new NpgsqlDataSourceBuilder(...);
dataSourceBuilder.UseJsonNet();
await using var dataSource = dataSourceBuilder.Build();
```

### [Global mapping](#tab/global)

If you're using an older version of Npgsql which doesn't yet support `NpgsqlDataSource`, you can configure mappings globally for all connections in your application:

```c#
NpgsqlConnection.GlobalTypeMapper.UseJsonNet();
```

For this to work, you must place this code at the beginning of your application, before any other Npgsql API is called. Note that in Npgsql 7.0, global type mappings are obsolete (but still supported) - `NpgsqlDataSource` is the recommended way to manage type mappings.

### [Connection mapping](#tab/connection)

> [!NOTE]
> This mapping method has been removed in Npgsql 7.0.

Older versions of Npgsql supported configuring a type mapping on an individual connection, as follows:

```c#
var conn = new NpgsqlConnection(...);
conn.TypeMapper.UseJsonNet();
```

***

## Arbitrary CLR Types

Once the plugin is set up, you can transparently read and write CLR objects as JSON values - the plugin will automatically have them serialized/deserialized:

```c#
// Write arbitrary CLR types as JSON
await using (var cmd = new NpgsqlCommand(@"INSERT INTO mytable (my_json_column) VALUES ($1)", conn))
{
    cmd.Parameters.Add(new() { Value = myClrInstance, NpgsqlDbType = NpgsqlDbType.Jsonb });
    await cmd.ExecuteNonQueryAsync();
}

// Read arbitrary CLR types as JSON
await using (var cmd = new NpgsqlCommand(@"SELECT my_json_column FROM mytable", conn))
await using (var reader = await cmd.ExecuteReaderAsync())
{
    await reader.ReadAsync();
    var someValue = reader.GetFieldValue<MyClrType>(0);
}
```

Note that in the example above, you must still specify `NpgsqlDbType.Json` (or `Jsonb`) to tell Npgsql that the parameter type is JSON. If you have several CLR types which you'll be using, you have the option of mapping them to JSON:

```c#
dataSourceBuilder.UseJsonNet(new[] { typeof(MyClrType) });
```

Note that the `UseJsonNet()` method accepts *two* type arrays: the first for types to map to `jsonb`, the second for types to map to `json`.

## JObject/JArray

You can also read and write Json.NET's JObject/JArray types directly:

```c#
var value = new JObject { ["Foo"] = 8 };
await using (var cmd = new NpgsqlCommand(@"INSERT INTO mytable (my_json_column) VALUES ($1)", conn))
{
    cmd.Parameters.Add(new() { Value = myClrInstance, NpgsqlDbType = NpgsqlDbType.Jsonb });
    await cmd.ExecuteNonQueryAsync();
}

await using (var cmd = new NpgsqlCommand(@"SELECT my_json_column FROM mytable", conn))
await using (var reader = await cmd.ExecuteReaderAsync())
{
    await reader.ReadAsync();
    var someValue = reader.GetFieldValue<JObject>(0);
}
```

## CLR Arrays

You can even read and write native CLR arrays as JSON:

```c#
await using (var cmd = new NpgsqlCommand(@"INSERT INTO mytable (my_json_column) VALUES ($1)", conn))
{
    cmd.Parameters.Add(new() { Value = new[] { 1, 2, 3 }, NpgsqlDbType = NpgsqlDbType.Jsonb) });
    await cmd.ExecuteNonQueryAsync();
}

await using (var cmd = new NpgsqlCommand(@"SELECT my_json_column FROM mytable", conn))
await using (var reader = await cmd.ExecuteReaderAsync())
{
    await reader.ReadAsync();
    var someValue = reader.GetFieldValue<int[]>(0);
}
```

And for extra credit, you can specify JSON by default for array types just like for regular CLR types:

```c#
dataSourceBuilder.UseJsonNet(new[] { typeof(int[]) });
```

This overwrites the default array mapping (which sends [PostgreSQL arrays](https://www.postgresql.org/docs/current/static/arrays.html)), making Npgsql send int arrays as JSON by default.
