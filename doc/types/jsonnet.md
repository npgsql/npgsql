# Json.NET Type Plugin

Since 4.0, Npgsql supports *type plugins*, which are external nuget packages that modify how Npgsql maps PostgreSQL values to CLR types. One of these is the Json.NET plugin, which allows Npgsql to automatically make use of [Newtonsoft Json.NET](http://www.newtonsoft.com/json) when reading and writing JSON data.

[PostgreSQL natively supports two JSON types](https://www.postgresql.org/docs/current/static/datatype-json.html): `jsonb` and `json`. Out of the box, Npgsql allows reading and writing these types as strings and provides no further processing to avoid taking a dependency on an external JSON library, forcing Npgsql users to serialize and deserialize JSON values themselves. The Json.NET plugin removes this burden from users by perform serialization/deserialization within Npgsql itself.

## Setup

To use the Json.NET plugin, simply add a dependency on [Npgsql.Json.NET](https://www.nuget.org/packages/Npgsql.Json.NET) and set it up:

```c#
using Npgsql;

// Place this at the beginning of your program to use Json.NET everywhere (recommended)
NpgsqlConnection.GlobalTypeMapper.UseJsonNet();

// Or to temporarily use JsonNet on a single connection only:
conn.TypeMapper.UseJsonNet();
```

## Arbitrary CLR Types

Once the plugin is set up, you can transparently read and write CLR objects as JSON values - the plugin will automatically have them serialized/deserialized:

```c#
// Write arbitrary CLR types as JSON
using (var cmd = new NpgsqlCommand(@"INSERT INTO mytable (my_json_column) VALUES (@p)", conn))
{
    cmd.Parameters.Add(new NpgsqlParameter("p", NpgsqlDbType.Jsonb) { Value = MyClrType });
    cmd.ExecuteNonQuery();
}

// Read arbitrary CLR types as JSON
using (var cmd = new NpgsqlCommand(@"SELECT my_json_column FROM mytable", conn))
using (var reader = cmd.ExecuteReader())
{
    reader.Read();
    var someValue = reader.GetFieldValue<MyClrType>(0);
}
```

Note that in the example above, you must still specify `NpgsqlDbType.Json` (or `Jsonb`) to tell Npgsql that the parameter type is JSON. If you have several CLR types which you'll be using, you have the option of mapping them to JSON:

```c#
NpgsqlConnection.GlobalTypeMapper.UseJsonNet(new[] { typeof(MyClrType) });
```

Note that the `UseJsonNet()` method accepts *two* type arrays: the first for types to map to `jsonb`, the second for types to map to `json`.

## JObject/JArray

You can also read and write Json.NET's JObject/JArray types directly:

```c#
var value = new JObject { ["Foo"] = 8 };
using (var cmd = new NpgsqlCommand(@"INSERT INTO mytable (my_json_column) VALUES (@p)", conn))
{
    cmd.Parameters.Add(new NpgsqlParameter("p", NpgsqlDbType.Jsonb) { Value = value });
    cmd.ExecuteNonQuery();
}

using (var cmd = new NpgsqlCommand(@"SELECT my_json_column FROM mytable", conn))
using (var reader = cmd.ExecuteReader())
{
    reader.Read();
    var someValue = reader.GetFieldValue<JObject>(0);
}
```

## CLR Arrays

You can even read and write native CLR arrays as JSON:

```c#
using (var cmd = new NpgsqlCommand(@"INSERT INTO mytable (my_json_column) VALUES (@p)", conn))
{
    cmd.Parameters.Add(new NpgsqlParameter("p", NpgsqlDbType.Jsonb) { Value = new[] { 1, 2, 3} });
    cmd.ExecuteNonQuery();
}

using (var cmd = new NpgsqlCommand(@"SELECT my_json_column FROM mytable", conn))
using (var reader = cmd.ExecuteReader())
{
    reader.Read();
    var someValue = reader.GetFieldValue<int[]>(0);
}
```

And for extra credit, you can specify JSON by default for array types just like for regular CLR types:

```c#
NpgsqlConnection.GlobalTypeMapper.UseJsonNet(new[] { typeof(int[] });
```

This overwrites the default array mapping (which sends [PostgreSQL arrays](https://www.postgresql.org/docs/current/static/arrays.html)), making Npgsql send int arrays as JSON by default.
