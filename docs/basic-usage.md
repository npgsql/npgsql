# Npgsql Basic Usage

## Parameters

When sending data values to the database, you should use parameters rather than including the values in the SQL as follows:

```c#
using (var cmd = new NpgsqlCommand("INSERT INTO table (col1) VALUES (@p)", conn))
{
    cmd.Parameters.AddWithValue("p", "some_value");
    cmd.ExecuteNonQuery();
}
```

The `@p` in your SQL is called a parameter *placeholder*; Npgsql will expect to find a parameter with that name in the command's parameter list, and will send it along with your query. This has the following advantages over embedding the value in your SQL:

1. Avoid SQL injection for user-provided inputs: the parameter data is sent to PostgreSQL separately from the SQL, and is never interpreted as SQL.
2. Required to make use of [prepared statements](prepare.md), which dramatically improve performance if you execute the same SQL many times.
3. Parameter data is sent in an efficient, binary format, rather than being represented as a string in your SQL.

Note that PostgreSQL does not support parameters in arbitrary locations - you can only parameterize data values. For example, trying to parameterize a table or column name will fail - parameters aren't a simple way to stick an arbitrary string in your SQL.

### Parameter types

PostgreSQL has a strongly-typed type system; columns and parameters have a type, and types are usually not implicitly converted to other types. This means you have to think about which type you will be sending: trying to insert a string into an integer column (or vice versa) will fail.

In the example above, we let Npgsql *infer* the PostgreSQL data type from the .NET type: when Npgsql sees a .NET `string`, it automatically sends a parameter of type `text` (note that this isn't the same as, say `varchar`). In many cases this will work just fine, and you don't need to worry. In some cases, however, you will need to explicitly set the parameter type. For example, Npgsql sends .NET `DateTime` as `timestamp without time zone`, but you may want to send a PostgreSQL `date` instead, which doesn't have a direct counterpart in .NET. For more information on supported types and their mappings, see [this page](http://localhost:8080/doc/types/basic.html).

`NpgsqlParameter` exposes several properties that allow you to specify the parameter's data type:

#### DbType

`DbType` is a portable enum that can be used to specify database types. While this approach will allow you to write portable code across databases, it obviously won't let you specify types that are specific to PostgreSQL.

#### NpgsqlDbType

`NpgsqlDbType` is an Npgsql-specific enum that contains (almost) all PostgreSQL types supported by Npgsql.

#### DataTypeName

`DataTypeName` is an Npgsql-specific string property which allows to directly set a PostgreSQL type name on the parameter. This is rarely needed - `NpgsqlDbType` should be suitable for the majority of cases. However, it may be useful if you're using unmapped user-defined types ([enums or composites](types/enums_and_composites.md)) or some PostgreSQL type which isn't included in `NpgsqlDbType` (because it's supported via an external plugin).

### Strongly-typed parameters

The standard ADO.NET parameter API is unfortunately weakly-typed: parameter values are set on `NpgsqlParameter.Value`, which, being an `object`, will box value types such as `int`. If you're sending lots of value types to the database, this will create large amounts of useless heap allocations and strain the garbage collector.

As an alternative, you can use `NpgsqlParameter<T>`. This generic class has a `TypedValue` member, which is similar to `NpgsqlParameter.Value` but is strongly-typed, thus avoiding the boxing and heap allocation. Note that this strongly-typed parameter API is entirely Npgsql-specific, and will make your code non-portable to other database. See [#8955](https://github.com/dotnet/corefx/issues/8955) for an issue discussing this at the ADO.NET level.

## Stored functions and procedures

PostgreSQL supports [stored (or server-side) functions](https://www.postgresql.org/docs/current/static/sql-createfunction.html), and since PostgreSQL 11 also [stored procedures](). These can be written in SQL (similar to views), or in [PL/pgSQL](https://www.postgresql.org/docs/current/static/plpgsql.html) (PostgreSQL's procedural language), [PL/Python](https://www.postgresql.org/docs/current/static/plpython.html) or several other server-side languages.

Once a function or procedure has been defined, calling it is a simple matter of executing a regular command:

```c#
// For functions
using (var cmd = new NpgsqlCommand("SELECT my_func(1, 2)", conn))
using (var reader = cmd.ExecuteReader()) { ... }

// For procedures
using (var cmd = new NpgsqlCommand("CALL my_proc(1, 2)", conn))
using (var reader = cmd.ExecuteReader()) { ... }
```

You can replace the parameter values above with regular placeholders (e.g. `@p1`), just like with a regular query.

In some other databases, calling a stored procedures involves setting the command's *behavior*:

```c#
using (var cmd = new NpgsqlCommand("my_func", conn))
{
    cmd.CommandType = CommandType.StoredProcedure;
    cmd.Parameters.AddWithValue("p1", "some_value");
    using (var reader = cmd.ExecuteReader()) { ... }
}
```

Npgsql supports this mainly for portability, but this style of calling has no advantage over the regular command shown above. When `CommandType.StoredProcedure` is set, Npgsql will simply generate the appropriate `SELECT my_func()` for you, nothing more. Unless you have specific portability requirements, it is recommended you simply avoid `CommandType.StoredProcedure` and construct the SQL yourself.

Note that if `CommandType.StoredProcedure` is set and your parameter instances have names, Npgsql will generate parameters with `named notation`: `SELECT my_func(p1 => 'some_value')`. This means that your NpgsqlParameter names must match your PostgreSQL function parameters, or the function call will fail. If you omit the names on your NpgsqlParameters, positional notation will be used instead. [See the PostgreSQL docs for more info](https://www.postgresql.org/docs/current/static/sql-syntax-calling-funcs.html).

Be aware that `CommandType.StoredProcedure` will generate a `SELECT` command - suitable for functions - and not a `CALL` command suitable for procedures. Npgsql has behaved this way since long before stored procedures were introduced, and changing this behavior would break backwards compatibility for many applications. The only way to call a stored procedure is to write your own `CALL my_proc(...)` command, without setting `CommandBehavior.StoredProcedure`.

### In/out parameters

In SQL Server (and possibly other databases), functions can have output parameters, input/output parameters, and a return value, which can be either a scalar or a table (TVF). To call functions with special parameter types, the `Direction` property must be set on the appropriate `DbParameter`. PostgreSQL functions, on the hand, always return a single table - they can all be considered TVFs. Somewhat confusingly, PostgreSQL does allow your functions to be defined with input/and output parameters:

```c#
CREATE FUNCTION dup(in int, out f1 int, out f2 text)
    AS $$ SELECT $1, CAST($1 AS text) || ' is text' $$
    LANGUAGE SQL;
```

However, the above syntax is nothing more than a definition of the function's resultset, and is identical to the following ([see the PostgreSQL docs](https://www.postgresql.org/docs/current/static/sql-createfunction.html)):
 
```c#
CREATE FUNCTION dup(int) RETURNS TABLE(f1 int, f2 text)
    AS $$ SELECT $1, CAST($1 AS text) || ' is text' $$
    LANGUAGE SQL;
```

In other words, PostgreSQL functions don't have output parameters that are distinct from the resultset they return - output parameters are just a syntax for describing that resultset. Because of this, on the Npgsql side there's no need to think about output (or input/output) parameters: simply invoke the function and process its resultset just like you would any regular resultset.

However, to help portability, Npgsql does provide support for output parameters as follows:

```c#
using (var cmd = new NpgsqlCommand("SELECT my_func()", conn))
{
    cmd.Parameters.Add(new NpgsqlParameter("p_out", DbType.String) { Direction = ParameterDirection.Output });
    cmd.ExecuteNonQuery();
    Console.WriteLine(cmd.Parameters[0].Value);
}
```

When Npgsql sees a parameter with `ParameterDirection.Output` (or `InputOutput`), it will simply search the function's resultset for a column whose name matches the parameter, and copy the first row's value into the output parameter. This provides no value whatsoever over processing the resultset yourself, and is discouraged - you should only use output parameters in Npgsql if you need to maintain portability with other databases which require it.


