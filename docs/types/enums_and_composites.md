# PostgreSQL enums and composites

PostgreSQL supports [enum types](http://www.postgresql.org/docs/current/static/datatype-enum.html) and [composite types](http://www.postgresql.org/docs/current/static/rowtypes.html) as database columns, and Npgsql supports reading and writing these. This allows you to seamlessly read and write enum and composite values to the database without worrying about conversions.

## Mapping your CLR types

The recommended way to work with enums and composites is to set up a mapping for your CLR types. Doing so provides the following advantages:

To set up a global mapping that applies to all connections, place this code before the initial connection is opened:

```c#
NpgsqlConnection.GlobalTypeMapper.MapEnum<SomeEnum>("some_enum_type");
NpgsqlConnection.GlobalTypeMapper.MapComposite<SomeType>("some_composite_type");
```

This sets up a mapping between your CLR types `SomeEnum` and `SomeType` to the PostgreSQL types `some_enum_type` and `some_composite_type`.

If you don't want to set up a mapping for all your connections, you can set it up one connection only:

```c#
var conn = new NpgsqlConnection(...);
conn.TypeMapper.MapEnum<SomeEnum>("some_enum_type");
conn.TypeMapper.MapComposite<SomeType>("some_composite_type");
```

After mapping, you can read and write your CLR types as usual:

```c#
// Writing
using (var cmd = new NpgsqlCommand("INSERT INTO some_table (some_enum_column, some_composite_column) VALUES (@p1, @p2)", conn))
{
    cmd.Parameters.Add(new NpgsqlParameter
    {
        ParameterName = "p1",
        Value = SomeEnum.Good
    });
    cmd.Parameters.Add(new NpgsqlParameter
    {
        ParameterName = "p2",
        Value = new SomeType { ... }
    });
    cmd.ExecuteNonQuery();
}

// Reading
using (var cmd = new NpgsqlCommand("SELECT some_enum_column, some_composite_column FROM some_table", conn))
using (var reader = cmd.ExecuteReader()) {
    reader.Read();
    var enumValue = reader.GetFieldValue<SomeEnum>(0);
    var compositeValue = reader.GetFieldValue<SomeType>(1);
}
```

Note that your PostgreSQL enum and composites types (`some_enum_type` and `some_composite_type` in the sample above) must be defined in your database before the first connection is created (see `CREATE TYPE`). If you're creating PostgreSQL types within your program, call `NpgsqlConnection.ReloadTypes()` to make sure Npgsql becomes properly aware of them.

## Name translation

CLR type and field names are usually Pascal case (e.g. `SomeType`), whereas in PostgreSQL they are snake case (e.g. `some_type`). To help make the mapping for enums and composites seamless, pluggable name translators are used translate all names. The default translation scheme is `NpgsqlSnakeCaseNameTranslator`, which maps names like `SomeType` to `some_type`, but you can specify others. The default name translator can be set for all your connections via `NpgsqlConnection.GlobalTypeMapper.DefaultNameTranslator`, or for a specific connection for `NpgsqlConnection.TypeMapper.DefaultNameTranslator`. You also have the option of specifying a name translator when setting up a mapping:

```c#
NpgsqlConnection.GlobalTypeMapper.MapComposite<SomeType>("some_type", new NpgsqlNullNameTranslator());
```

Finally, you may control mappings on a field-by-field basis via the `[PgName]` attribute. This will override the name translator.

```c#
using NpgsqlTypes;

enum SomeEnum {
   [PgName("happy")]
   Good,
   [PgName("sad")]
   Bad
}
```

## Reading and writing unmapped enums

In some cases, it may be desirable to interact with PostgreSQL enums without a pre-existing CLR enum type - this is useful mainly if your program doesn't know the database schema and types in advance, and needs to interact with any enum/composite type.

Npgsql allows reading and writing enums as simple strings:

```c#
// Writing enum as string
using (var cmd = new NpgsqlCommand("INSERT INTO some_table (some_enum_column) VALUES (@p1)", conn))
{
    cmd.Parameters.Add(new NpgsqlParameter
    {
        ParameterName = "p1",
        Value = "Good"
        DataTypeName = "some_enum_type"
    });
    cmd.ExecuteNonQuery();
}

// Reading enum as string
using (var cmd = new NpgsqlCommand("SELECT some_enum_column FROM some_table", conn))
using (var reader = cmd.ExecuteReader()) {
    reader.Read();
    var enumValue = reader.GetFieldValue<string>(0);
}
```
