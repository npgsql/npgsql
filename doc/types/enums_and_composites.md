# Accessing PostgreSQL Enums and Composites

PostgreSQL supports [enum types](http://www.postgresql.org/docs/current/static/datatype-enum.html) and
[composite types](http://www.postgresql.org/docs/current/static/rowtypes.html) as database columns,
and Npgsql supports mapping these to your CLR types. This allows you to seamlessly and efficiently read and write enum values
to the database without worrying about conversions.

*Note: composite type support was introduced in Npgsql 3.1*

## Mapping CLR types to PostgreSQL enums and composites

In order to use enum and composite types with Npgsql, you must map your CLR type to the PostgreSQL enum or composite.
This must be done in advance, *before* reading or writing. The easiest way to do this is to call
`MapEnumGlobally` or `MapCompositeGlobally` before opening any connections; the following will set up a mapping between
your CLR SomeEnum type to a PostgreSQL enum called `some_enum` (more on the name translation later).

```c#
enum SomeEnum { ... }
NpgsqlConnection.MapEnumGlobally<SomeEnum>();
```

Similarly, the following will set up a mapping between CLR type SomeType and a PostgreSQL composite called `some_type`.
The CLR type can be a class or struct.

```c#
class SomeType { ... }
NpgsqlConnection.MapCompositeGlobally<SomeType>();
```

Note that the PostgreSQL types must have been create in advance (with `CREATE TYPE`).

If you don't want to map an enum for all your connections, you can register a mapping for one connection only by calling
`MapEnum` on an NpgsqlConnection instance.

## Name Translation

Since Npgsql 3.1, pluggable name translators are used to map CLR type and field names to PostgreSQL ones.
The default translation scheme is `NpgsqlSnakeCaseNameTranslator`, which maps names like SomeType to some_type.
However, when calling the mapping methods you can pass your own name translator which implements INpgsqlNameTranslator.

Finally, you may control mappings on a field-by-field basis via the [PgName] attribute. This will override the name
translator.

```c#
using NpgsqlTypes;

enum SomeEnum {
   [PgName("happy")]
   Good,
   [PgName("sad")]
   Bad
}
```

## Reading and Writing

Once your mapping is set up, you can read and write enums and composites like any other type:

```c#
// Writing
using (var cmd = new NpgsqlCommand("INSERT INTO some_table (some_enum, some_type) VALUES (@p1, @p2)", Conn)) {
    cmd.Parameters.AddWithValue("p1", SomeEnum.Good);
    cmd.Parameters.AddWithValue("p2", new SomeType { ... });
    cmd.ExecuteNonQuery();
}

// Reading
using (var cmd = new NpgsqlCommand("SELECT some_enum, some_type FROM some_table", Conn))
using (var reader = cmd.ExecuteReader()) {
    reader.Read();
    var enumValue = reader.GetFieldValue<SomeEnum>(0);
    var compositeValue = reader.GetFieldValue<SomeType>(1);
}
```

