# Accessing PostgreSQL Enums and Composites

PostgreSQL supports [enum types](http://www.postgresql.org/docs/current/static/datatype-enum.html) and [composite types](http://www.postgresql.org/docs/current/static/rowtypes.html) as database columns, and Npgsql supports reading and writing these. This allows you to seamlessly read and write enum and composite values to the database without worrying about conversions.

As of version 3.3, you're no longer required to map your enums and composites in order to use them. Here's a quick code sample to get you started:

```c#
// Writing
using (var cmd = new NpgsqlCommand("INSERT INTO some_table (some_enum, some_composite) VALUES (@p1, @p2)", Conn))
{
    cmd.Parameters.Add(new NpgsqlParameter
    {
        ParameterName = "some_enum",
        Value = SomeEnum.Good,
        DataTypeName = "pg_enum_type"
    });
    cmd.Parameters.Add(new NpgsqlParameter
    {
        ParameterName = "some_composite",
        Value = new SomeType { ... },
        DataTypeName = "pg_composite_type"
    });
    cmd.ExecuteNonQuery();
}

// Reading
using (var cmd = new NpgsqlCommand("SELECT some_enum, some_composite FROM some_table", Conn))
using (var reader = cmd.ExecuteReader()) {
    reader.Read();
    var enumValue = reader.GetFieldValue<SomeEnum>(0);
    var compositeValue = reader.GetFieldValue<SomeType>(1);
}
```

As long as you CLR types `SomeEnum` and `SomeType` contain fields/properties which correspond to the PostgreSQL type being read/written, everything will work as expected. Note that the default name translator is used (see name translation below).

Note that your PostgreSQL enum and composites types (`pg_enum_type` and `pg_composite_type` in the sample above) must be defined in your database before the first connection is created (see `CREATE TYPE`). If you're creating PostgreSQL types within your program, call `NpgsqlConnection.ReloadTypes()` to make sure Npgsql becomes properly aware of them.

## Mapping your CLR Types

It is still possible, and in some cases necessary, to set up an explicit mapping for your CLR types. Doing so provides the following advantages:
1. You know longer need to specify the `DataTypeName` property on your parameter. Npgsql will infer the data type from your provided CLR type.
2. Untyped read methods such as `NpgsqlDataReader.GetValue()` will return your CLR type, instead of a dynamic object (see below). In general you should be using the typed `NpgsqlDataReader.GetFieldValue()`, so this shouldn't be important.
3. You can customize the name mapping on a per-type basis (see below).

To set up a global mapping for all your connections, put this code before your first open:

```c#
NpgsqlConnection.GlobalTypeMapper.MapEnum<SomeEnum>("some_enum");
NpgsqlConnection.GlobalTypeMapper.MapComposite<SomeType>("some_composite");
```

This sets up a mapping between your CLR types `SomeEnum` and `SomeType` to the PostgreSQL types `some_enum` and `some_composite`.

If you don't want to set up a mapping for all your connections, you can set it up one connection only:

```c#
var conn = new NpgsqlConnection(...);
conn.TypeMapper.MapEnum<SomeEnum>("some_enum");
conn.TypeMapper.MapComposite<SomeType>("some_composite");
```

## Name Translation

CLR type and field names are usually camelcase (e.g. `SomeType`), whereas in PostgreSQL they are snake_case (e.g. `some_type`). To help make the mapping for enums and composites seamless, pluggable name translators are used translate all names. The default translation scheme is `NpgsqlSnakeCaseNameTranslator`, which maps names like `SomeType` to `some_type`, but you can specify others. The default name translator can be set for all your connections via `NpgsqlConnection.GlobalTypeMapper.DefaultNameTranslator`, or for a specific connection for `NpgsqlConnection.TypeMapper.DefaultNameTranslator`. You also have the option of specifyin a name translator when setting up a mapping:

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

## Reading and Writing Dynamically (without CLR types)

In some cases, it may be desirable to interact with PostgreSQL enums and composites without a pre-existing CLR type - this is useful mainly if your program doesn't know the database schema and types in advance, and needs to interact with any enum/composite type. Note that using CLR types is safer and faster (for composites), and should be preferred when possible.

Enums can be read and written as simple strings:

```c#
// Writing enum as string
using (var cmd = new NpgsqlCommand("INSERT INTO some_table (some_enum) VALUES (@p1)", Conn))
{
    cmd.Parameters.Add(new NpgsqlParameter
    {
        ParameterName = "some_enum",
        Value = "Good"
        DataTypeName = "pg_enum_type"
    });
    cmd.ExecuteNonQuery();
}

// Reading enum as string
using (var cmd = new NpgsqlCommand("SELECT some_enum FROM some_table", Conn))
using (var reader = cmd.ExecuteReader()) {
    reader.Read();
    var enumValue = reader.GetFieldValue<string>(0);
}
```

Composites can be read and written as C# dynamic ExpandoObjects:

```c#
// Writing composite as ExpandoObject
using (var cmd = new NpgsqlCommand("INSERT INTO some_table (some_composite) VALUES (@p1)", Conn))
{
    var someComposite = new ExpandoObject();
    some_composite.Foo = 8;
    some_composite.Bar = "hello";
    cmd.Parameters.Add(new NpgsqlParameter
    {
        ParameterName = "some_enum",
        Value = someComposite,
        DataTypeName = "pg_enum_type"
    });
    cmd.ExecuteNonQuery();
}

// Reading composite as ExpandoObject
using (var cmd = new NpgsqlCommand("SELECT some_composite FROM some_table", Conn))
using (var reader = cmd.ExecuteReader()) {
    reader.Read();
    var compositeValue = (dynamic)reader.GetValue(0);
    Console.WriteLine(compositeValue.Foo);
    Console.WriteLine(compositeValue.Bar);
}
```
