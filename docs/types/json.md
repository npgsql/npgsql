# Mapping JSON

> [!NOTE]
> If you're using EF Core, please read the page on [JSON support in the EF provider](../../EFCore.PG/mapping/json.md). EF has specialized support for JSON beyond what is supported at the lower-level Npgsql layer.

PostgreSQL has rich, built-in support for storing JSON columns and efficiently performing complex queries operations on them. Newcomers can read more about the PostgreSQL support on [the JSON types page](https://www.postgresql.org/docs/current/datatype-json.html), and on the [functions and operators page](https://www.postgresql.org/docs/current/functions-json.html). Note that the below mapping mechanisms support both the `jsonb` and `json` types, although the former is almost always preferred for efficiency and functionality reasons.

Npgsql allows you to map PostgreSQL JSON columns in three different ways:

1. As simple strings
2. As strongly-typed user-defined types (POCOs)
3. As System.Text.Json DOM types (JsonDocument or JsonElement, [see docs](https://learn.microsoft.com/dotnet/standard/serialization/system-text-json/use-dom#use-jsondocument))
4. High-performance JSON parsing with [Utf8JsonReader](https://learn.microsoft.com/dotnet/standard/serialization/system-text-json/use-utf8jsonreader)
5. Newtonsoft Json.NET

## String mapping

The simplest form of mapping to JSON is as a regular .NET string:

```c#
// Write a string to a json column:
await using var command1 = new NpgsqlCommand("INSERT INTO test (data) VALUES ($1)", conn)
{
    Parameters = { new() { Value = """{ "a": 8, "b": 9 }""", NpgsqlDbType = NpgsqlDbType.Jsonb } }
};
await command1.ExecuteNonQueryAsync();

// Read jsonb data as a string:
await using var command2 = new NpgsqlCommand("SELECT data FROM test", conn);
await using var reader = await command2.ExecuteReaderAsync();
while (await reader.ReadAsync())
{
    Console.WriteLine(reader.GetString(0));
}
```

> [!NOTE]
> Note that when writing a string parameter as `jsonb`, you must specify `NpgsqlDbType.Jsonb`, otherwise Npgsql sends a `text` parameter which is incompatible with JSON.

With this mapping style, you're fully responsible for serializing/deserializing the JSON data yourself (e.g. with System.Text.Json) - Npgsql simply passes your strings to and from PostgreSQL.

## POCO mapping

> [!WARNING]
> As of Npgsql 8.0, POCO mapping is incompatible with NativeAOT. We plan to improve this, [please upvote this issue if you're interested](https://github.com/npgsql/npgsql/issues/5355).

If your column JSON contains documents with a stable schema, you can map them to your own .NET types (or POCOs). The provider will use System.Text.Json APIs under the hood to serialize instances of your types to JSON documents before sending them to the database, and to deserialize documents coming back from the database. This effectively allows mapping an arbitrary .NET type - or object graph - to a single column in the database.

Starting with Npgsql 8.0, to use this feature, you must first enable it by calling <xref:Npgsql.INpgsqlTypeMapperExtensions.EnableDynamicJson> on your <xref:Npgsql.NpgsqlDataSourceBuilder>, or, if you're not yet using data sources, on `NpgsqlConnection.GlobalTypeMapper`:

### [NpgsqlDataSource](#tab/datasource)

> [!NOTE]
> `NpgsqlDataSource` was introduced in Npgsql 7.0, and is the recommended way to manage type mapping. If you're using an older version, see the other methods.

```c#
var dataSourceBuilder = new NpgsqlDataSourceBuilder(...);
dataSourceBuilder.EnableDynamicJson();
await using var dataSource = dataSourceBuilder.Build();
```

### [Global mapping](#tab/global)

If you're not yet using `NpgsqlDataSource`, you can configure mappings globally for all connections in your application:

```c#
NpgsqlConnection.GlobalTypeMapper.EnableDynamicJson();
```

For this to work, you must place this code at the beginning of your application, before any other Npgsql API is called. Note that in Npgsql 7.0, global type mappings are obsolete (but still supported) - `NpgsqlDataSource` is the recommended way to manage type mappings.

***

Once you've enabled the feature, you can simply read and write instances of your POCOs directly; when writing, specify `NpgsqlDbType.Jsonb` to let Npgsql know you intend for it to get sent as JSON data:

```c#
// Write a POCO to a jsonb column:
var myPoco1 = new MyPoco { A = 8, B = 9 };

await using var command1 = new NpgsqlCommand("INSERT INTO test (data) VALUES ($1)", conn)
{
    Parameters = { new() { Value = myPoco1, NpgsqlDbType = NpgsqlDbType.Jsonb } }
};
await command1.ExecuteNonQueryAsync();

// Read jsonb data as a POCO:
await using var command2 = new NpgsqlCommand("SELECT data FROM test", conn);
await using var reader = await command2.ExecuteReaderAsync();
while (await reader.ReadAsync())
{
    var myPoco2 = reader.GetFieldValue<MyPoco>(0);
    Console.WriteLine(myPoco2.A);
}

class MyPoco
{
    public int A { get; set; }
    public int B { get; set; }
}
```

This mapping method is quite powerful, allowing you to read and write nested graphs of objects and arrays to PostgreSQL without having to deal with serialization yourself.

## System.Text.Json DOM types

There are cases in which mapping JSON data to POCOs isn't appropriate; for example, your JSON column may not contain a fixed schema and must be inspected to see what it contains; for these cases, Npgsql supports mapping JSON data to [JsonDocument](https://docs.microsoft.com/dotnet/api/system.text.json.jsondocument) or [JsonElement](https://docs.microsoft.com/dotnet/api/system.text.json.jsonelement) ([see docs](https://learn.microsoft.com/dotnet/standard/serialization/system-text-json/use-dom#use-jsondocument)):

```c#
var jsonDocument = JsonDocument.Parse("""{ "a": 8, "b": 9 }""");

// Write a JsonDocument:
await using var command1 = new NpgsqlCommand("INSERT INTO test (data) VALUES ($1)", conn)
{
    Parameters = { new() { Value = jsonDocument } }
};
await command1.ExecuteNonQueryAsync();

// Read jsonb data as a JsonDocument:
await using var command2 = new NpgsqlCommand("SELECT data FROM test", conn);
await using var reader = await command2.ExecuteReaderAsync();
while (await reader.ReadAsync())
{
    var document = reader.GetFieldValue<JsonDocument>(0);
    Console.WriteLine(document.RootElement.GetProperty("a").GetInt32());
}
```

## High-performance JSON parsing with Utf8JsonReader

If you're writing a very performance-sensitive application, using System.Text.Json to deserialize to POCOs or JsonDocument may incur too much overhead. If that's the case, you can use System.Text.Json's [Utf8JsonReader](https://learn.microsoft.com/dotnet/standard/serialization/system-text-json/use-utf8jsonreader) to parse JSON data from the database. Utf8JsonReader provides a low-level, forward-only API to parse the JSON data, one token at a time.

Utf8JsonReader requires JSON data as raw, UTF8-encoded binary data; fortunately, Npgsql allows reading `jsonb` as binary data, and if your PostgreSQL `client_encoding` is set to UTF8 (the default), you can feed data directly from PostgreSQL to Utf8JsonReader:

```c#
await using var command2 = new NpgsqlCommand("SELECT data FROM test", conn);
await using var reader = await command2.ExecuteReaderAsync();
while (await reader.ReadAsync())
{
    ParseJson(reader.GetFieldValue<byte[]>(0));
}

void ParseJson(byte[] utf8Data)
{
    var jsonReader = new Utf8JsonReader(utf8Data);
    // ... parse the data with jsonReader
}
```

Note that the above works well for small JSON columns; if you have large columns (above ~8k), consider streaming the JSON data instead. This can be done by passing `CommandBehavior.SequentialAccess` to `ExecuteReaderAsync`, and then calling `reader.GetStream()` on NpgsqlDataReader instead of `GetFieldValue<byte[]>`. To process streaming data with Utf8JsonReader, [see these docs](https://learn.microsoft.com/dotnet/standard/serialization/system-text-json/use-utf8jsonreader#read-from-a-stream-using-utf8jsonreader).

## Newtonsoft.JSON

System.Text.Json is the built-in, standard way to handle JSON in modern .NET. However, some users still prefer using Newtonoft Json.NET, and Npgsql includes support for that.

To use Json.NET, add the [Npgsql.Json.NET package](https://www.nuget.org/packages/Npgsql.Json.NET) to your project, and enable the plugin as follows:

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

***

Once you've enabled the feature, you can simply read and write instances of your POCOs directly; when writing, specify `NpgsqlDbType.Jsonb` to let Npgsql know you intend for it to get sent as JSON data:

```c#
// Write a POCO to a jsonb column:
var myPoco1 = new MyPoco { A = 8, B = 9 };

await using var command1 = new NpgsqlCommand("INSERT INTO test (data) VALUES ($1)", conn)
{
    Parameters = { new() { Value = myPoco1, NpgsqlDbType = NpgsqlDbType.Jsonb } }
};
await command1.ExecuteNonQueryAsync();

// Read jsonb data as a POCO:
await using var command2 = new NpgsqlCommand("SELECT data FROM test", conn);
await using var reader = await command2.ExecuteReaderAsync();
while (await reader.ReadAsync())
{
    var myPoco2 = reader.GetFieldValue<MyPoco>(0);
    Console.WriteLine(myPoco2.A);
}

class MyPoco
{
    public int A { get; set; }
    public int B { get; set; }
}
```

The plugin also allows you to read JObject/JArray for weakly-typed DOM mapping.
