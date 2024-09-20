# PostgreSQL enums and composites

PostgreSQL supports [enum types](http://www.postgresql.org/docs/current/static/datatype-enum.html) and [composite types](http://www.postgresql.org/docs/current/static/rowtypes.html) as database columns, and Npgsql supports reading and writing these. This allows you to seamlessly read and write enum and composite values to the database without worrying about conversions.

## Creating your types

Let's assume you've created some enum and composite types in PostgreSQL:

```sql
CREATE TYPE mood AS ENUM ('sad', 'ok', 'happy');

CREATE TYPE inventory_item AS (
    name            text,
    supplier_id     integer,
    price           numeric
);
```

To use these types with Npgsql, you must first define corresponding CLR types that will be mapped to the PostgreSQL types:

```csharp
public enum Mood
{
    Sad,
    Ok,
    Happy
}

public class InventoryItem
{
    public string Name { get; set; } = "";
    public int SupplierId { get; set; }
    public decimal Price { get; set; }
}
```

## Mapping your CLR types

Once your types are defined both in PostgreSQL and in C#, you can now configure the mapping between them with Npgsql.

### [NpgsqlDataSource](#tab/datasource)

> [!NOTE]
> `NpgsqlDataSource` was introduced in Npgsql 7.0, and is the recommended way to manage type mapping. If you're using an older version, see the other methods.

```csharp
var dataSourceBuilder = new NpgsqlDataSourceBuilder(...);
dataSourceBuilder.MapEnum<Mood>();
dataSourceBuilder.MapComposite<InventoryItem>();
await using var dataSource = dataSourceBuilder.Build();
```

### [Global mapping](#tab/global)

If you're using an older version of Npgsql which doesn't yet support `NpgsqlDataSource`, you can configure mappings globally for all connections in your application:

```csharp
NpgsqlConnection.GlobalTypeMapper.MapEnum<Mood>();
NpgsqlConnection.GlobalTypeMapper.MapComposite<InventoryItem>();
```

For this to work, you must place this code at the beginning of your application, before any other Npgsql API is called. Note that in Npgsql 7.0, global type mappings are obsolete (but still supported) - `NpgsqlDataSource` is the recommended way to manage type mappings.

### [Connection mapping](#tab/connection)

> [!NOTE]
> This mapping method has been removed in Npgsql 7.0.

Older versions of Npgsql supported configuring a type mapping on an individual connection, as follows:

```csharp
var conn = new NpgsqlConnection(...);
conn.TypeMapper.MapEnum<Mood>();
conn.TypeMapper.MapComposite<InventoryItem>();
```

***

Whatever the method used, your CLR types `Mood` and `InventoryItem` are now mapped to the PostgreSQL types `mood` and `inventory_item`.

## Using your mapped types

Once your mapping is in place, you can read and write your CLR types as usual:

```csharp
// Writing
await using (var cmd = new NpgsqlCommand("INSERT INTO some_table (my_enum, my_composite) VALUES ($1, $2)", conn))
{
    cmd.Parameters.Add(new() { Value = Mood.Happy });
    cmd.Parameters.Add(new()
    {
        Value = new InventoryItem { ... }
    });
    cmd.ExecuteNonQuery();
}

// Reading
await using (var cmd = new NpgsqlCommand("SELECT my_enum, my_composite FROM some_table", conn))
await using (var reader = cmd.ExecuteReader()) {
    reader.Read();
    var enumValue = reader.GetFieldValue<Mood>(0);
    var compositeValue = reader.GetFieldValue<InventoryItem>(1);
}
```

Note that your PostgreSQL enum and composites types (`mood` and `inventory_data` in the sample above) must be defined in your database before the first connection is created (see `CREATE TYPE`). If you're creating PostgreSQL types within your program, call `NpgsqlConnection.ReloadTypes()` to make sure Npgsql becomes properly aware of them.

## Name translation

CLR type and field names are usually Pascal case (e.g. `InventoryData`), whereas in PostgreSQL they are snake case (e.g. `inventory_data`). To help make the mapping for enums and composites seamless, pluggable name translators are used translate all names. The default translation scheme is `NpgsqlSnakeCaseNameTranslator`, which maps names like `SomeType` to `some_type`, but you can specify others. The default name translator can be set for all your connections via `NpgsqlConnection.GlobalTypeMapper.DefaultNameTranslator`, or for a specific connection for `NpgsqlConnection.TypeMapper.DefaultNameTranslator`. You also have the option of specifying a name translator when setting up a mapping:

```csharp
NpgsqlConnection.GlobalTypeMapper.MapComposite<InventoryData>("inventory_data", new NpgsqlNullNameTranslator());
```

Finally, you may control mappings on a field-by-field basis via the `[PgName]` attribute. This overrides the name translator.

```csharp
public enum Mood
{
    [PgName("depressed")]
    Sad,
    Ok,
    [PgName("ebullient")]
    Happy
}
```

## Reading and writing unmapped enums

In some cases, it may be desirable to interact with PostgreSQL enums without a pre-existing CLR enum type - this is useful mainly if your program doesn't know the database schema and types in advance, and needs to interact with any enum/composite type.

Npgsql allows reading and writing enums as simple strings:

```csharp
// Writing enum as string
await using (var cmd = new NpgsqlCommand("INSERT INTO some_table (my_enum) VALUES ($1)", conn))
{
    cmd.Parameters.Add(new()
    {
        Value = "Happy"
        DataTypeName = "mood"
    });
    cmd.ExecuteNonQuery();
}

// Reading enum as string
await using (var cmd = new NpgsqlCommand("SELECT my_enum FROM some_table", conn))
await using (var reader = cmd.ExecuteReader()) {
    reader.Read();
    var enumValue = reader.GetFieldValue<string>(0);
}
```
