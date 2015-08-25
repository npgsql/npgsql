---
layout: doc
title: Enum Support
---

PostgreSQL supports enum types as database columns ([see docs](http://www.postgresql.org/docs/current/static/datatype-enum.html)),
and npgsql supports mapping these to your CLR enum types. This allows you to seamlessly and efficiently read and write enum values
to the database without worrying about conversions.

## Mapping CLR enums to PostgreSQL enums

In order to use enums with npgsql, you must map your CLR enum type to the PostgreSQL enum. This must be done in advance,
*before* reading or writing enums. The easiest way to do this is to call `RegisterEnumGlobally` before opening any connections:

{% highlight C# %}
enum SomeEnum { ... }
NpgsqlConnection.RegisterEnumGlobally<SomeEnum>("some_enum");
{% endhighlight %}

This will set up a mapping between your `SomeEnum` CLR type to the PostgreSQL `some_enum` type,
which must have been created in advance with `CREATE TYPE some_enum AS ENUM`. If you omit the
PostgreSQL type name parameter, npgsql will default to the CLR type's name (`SomeEnum`).

If you don't want to map an enum for all your connections, you can register a mapping for one connection only by calling
`MapEnum` on an NpgsqlConnection instance.

## Mapping Field Names

By default, expects the CLR and PostgreSQL field names to match exactly - including case sensitivity.
If your CLR enum's fields differ from the PostgreSQL's enum's fields, you can use the `[EnumLabel]` attribute to let npgsql
know about it:

{% highlight C# %}
enum SomeEnum {
   [EnumLabel("happy")]
   Good,
   [EnumLabel("sad")]
   Bad
}
{% endhighlight %}

## Reading and Writing

Once your mapping is set up, you can read and write enums like any other type:

{% highlight C# %}
// Writing
using (var cmd = new NpgsqlCommand("INSERT INTO some_table (some_enum) VALUES (@p)", Conn)) {
    cmd.Parameters.AddWithValue("p", SomeEnum.Good);
    cmd.ExecuteNonQuery();
}

// Reading
using (var cmd = new NpgsqlCommand("SELECT some_enum FROM some_table", Conn)) {
    var value = (SomeEnum)cmd.ExecuteScalar();
}
{% endhighlight %}

