# Npgsql Basic Usage

## Connections

The starting point for any database operation is acquiring an <xref:Npgsql.NpgsqlConnection>; this represents a connection to the database on which commands can be executed. Connections can be instantiated directly, and must then be opened before they can be used:

```c#
var connectionString = "Host=myserver;Username=mylogin;Password=mypass;Database=mydatabase";

await using var conn = new NpgsqlConnection(connectionString);
await conn.OpenAsync();
```

In .NET, the connection string is used to define which database to connect to, the authentication information to use, and various other connection-related parameters; it consists of key/value pairs, separated with semicolons. Npgsql supports many options, these are documented on the [connection string page](connection-string-parameters.md).

Connections must be disposed when they are no longer needed - not doing so will result in a connection leak, which can crash your program. In the above code sample, this is done via the `await using` C# construct, which ensures the connection is disposed even if an exception is later thrown. It's a good idea to keep connections open for as little time a possible: database connections are scarce resources, and keeping them open for unnecessarily long times can create unnecessary load in your application and in PostgreSQL.

### Pooling

Opening and closing physical connections to PostgreSQL is an expensive and long process. Therefore, Npgsql connections are *pooled* by default: closing or disposing a connection doesn't close the underlying physical connection, but rather returns it to an internal pool managed by Npgsql. The next time a connection is opened, that pooled connection is returned again. The makes open and close extremely fast operations; do not hesitate to perform them a lot if needed, rather than holding a connection needlessly open for a long time.

For information on tweaking the pooling behavior (or turning it off), see the [pooling section](connection-string-parameters.html#pooling) in the connection string page.

## Commands

Once you have an open connection, a command can be used to execute SQL on it:

```c#
// Retrieve all rows
await using var cmd = new NpgsqlCommand("SELECT some_field FROM data", conn);
await using var reader = await cmd.ExecuteReaderAsync();

while (await reader.ReadAsync())
{
    Console.WriteLine(reader.GetString(0));
}
```

The command contains the SQL to be executed, as wel as any parameters (see the [parameters section](#parameters) below). Commands can be executed in the following three ways:

1. [ExecuteNonQueryAsync](https://docs.microsoft.com/dotnet/api/system.data.common.dbcommand.executenonqueryasync): executes SQL which doesn't return any results, typically `INSERT`, `UPDATE` or `DELETE` statements. Returns the number of rows affected.
2. [ExecuteScalarAsync](https://docs.microsoft.com/dotnet/api/system.data.common.dbcommand.executescalarasync): executes SQL which returns a single, scalar value.
3. [ExecuteReaderAsync](https://docs.microsoft.com/dotnet/api/system.data.common.dbcommand.executereaderasync): execute SQL which returns a full resultset. Returns an <xref:Npgsql.NpgsqlDataReader> which can be used to access the resultset (as in the above example).

To execute multiple SQL statements in a single roundtrip, see [batching below](#batching).

## Parameters

When sending data values to the database, always consider using parameters rather than including the values in the SQL as follows:

```c#
await using var cmd = new NpgsqlCommand("INSERT INTO table (col1) VALUES ($1), ($2)", conn)
{
    Parameters =
    {
        new() { Value = "some_value" },
        new() { Value = "some_other_value" }
    }
};

await cmd.ExecuteNonQueryAsync();
```

The `$1` and `$2` in your SQL are *parameter placeholders*: they refer to the corresponding parameter in the command's parameter list, and are sent along with your query. This has the following advantages over embedding the value in your SQL:

1. Parameters protect against SQL injection for user-provided inputs: the parameter data is sent to PostgreSQL separately from the SQL, and is never interpreted as SQL.
2. Parameters are required to make use of [prepared statements](prepare.md), which significantly improve performance if you execute the same SQL many times.
3. Parameter data is sent in an efficient, binary format, rather than being represented as a string in your SQL.

Note that PostgreSQL does not support parameters in arbitrary locations - you can only parameterize data values. For example, trying to parameterize a table or column name will fail - parameters aren't a simple way to stick an arbitrary string in your SQL.

### Positional and named placeholders

Starting with Npgsql 6.0, the recommended placeholder style is *positional* (`$1`, `$2`); this is the native parameter style used by PostgreSQL, and your SQL can therefore be sent to the database as-is, without any manipulation.

For legacy and compatibility reasons, Npgsql also supports *named placeholders*. This allows the above code to be written as follows:

```c#
await using var cmd = new NpgsqlCommand("INSERT INTO table (col1) VALUES (@p1), (@p2)", conn)
{
    Parameters =
    {
        new("p1", "some_value"),
        new("p2", "some_other_value")
    }
};

await cmd.ExecuteNonQueryAsync();
```

Rather than matching placeholders to parameters by their position, Npgsql matches these parameter by name. This can be useful when porting database code from other databases, where named placeholders are used. However, since this placeholder style isn't natively supported by PostgreSQL, Npgsql must parse your SQL and rewrite it to use positional placeholders under the hood; this rewriting has a performance price, and some forms of SQL may not be parsed correctly. It's recommended to use positional placeholders whenever possible.

For more information, see [this blog post](https://www.roji.org/parameters-batching-and-sql-rewriting).

### Parameter types

PostgreSQL has a strongly-typed type system: columns and parameters have a type, and types are usually not implicitly converted to other types. This means you have to think about which type you will be sending: trying to insert a string into an integer column (or vice versa) will fail.

In the example above, we let Npgsql *infer* the PostgreSQL data type from the .NET type: when Npgsql sees a .NET `string`, it automatically sends a parameter of PostgreSQL type `text` (note that this isn't the same as, say `varchar`). In many cases this will work just fine, and you don't need to worry. In some cases, however, you will need to explicitly set, or *coerce*, the parameter type. For example, although Npgsql sends .NET `string` as `text` by default, it also supports sending `jsonb`. For more information on supported types and their mappings, see [this page](types/basic.md).

`NpgsqlParameter` exposes several properties that allow you to coerce the parameter's data type:

* `DbType`: a portable enum that can be used to specify database types. While this approach will allow you to write portable code across databases, it won't let you specify types that are specific to PostgreSQL. This is useful mainly if you're avoiding Npgsql-specific types, using [`DbConnection`](https://docs.microsoft.com/dotnet/api/system.data.common.dbconnection) and [`DbCommand`](https://docs.microsoft.com/dotnet/api/system.data.common.dbcommand) rather than <xref:Npgsql.NpgsqlConnection> and <xref:Npgsql.NpgsqlCommand>.
* `NpgsqlDbType`: an Npgsql-specific enum that contains (almost) all PostgreSQL types supported by Npgsql.
* `DataTypeName`: an Npgsql-specific string property which allows to directly set a PostgreSQL type name on the parameter. This is rarely needed - `NpgsqlDbType` should be suitable for the majority of cases. However, it may be useful if you're using unmapped user-defined types ([enums or composites](types/enums_and_composites.md)) or some PostgreSQL type which isn't included in `NpgsqlDbType` (because it's supported via an external plugin).

### Strongly-typed parameters

The standard ADO.NET parameter API is unfortunately weakly-typed: parameter values are set on `NpgsqlParameter.Value`, which, being an `object`, will box value types such as `int`. If you're sending lots of value types to the database, this will create large amounts of useless heap allocations and strain the garbage collector.

As an alternative, you can use `NpgsqlParameter<T>`. This generic class has a `TypedValue` member, which is similar to `NpgsqlParameter.Value` but is strongly-typed, thus avoiding the boxing and heap allocation. Note that this strongly-typed parameter API is entirely Npgsql-specific, and will make your code non-portable to other database. See [#8955](https://github.com/dotnet/corefx/issues/8955) for an issue discussing this at the ADO.NET level.

## Transactions

### Basic transactions

Transactions can be started by calling the standard ADO.NET method [`NpgsqlConnection.BeginTransaction()`](https://docs.microsoft.com/en-us/dotnet/api/system.data.common.dbconnection.begintransaction?view=net-6.0#system-data-common-dbconnection-begintransaction).

PostgreSQL doesn't support nested or concurrent transactions - only one transaction may be in progress at any given moment. Starting a transaction while another transaction is already in progress will throw an exception. Because of this, it isn't necessary to pass the NpgsqlTransaction object returned from `BeginTransaction()` to commands you execute - starting a transaction means that all subsequent commands will automatically participate in the transaction, until either a commit or rollback is performed. However, for maximum portability it's recommended to set the transaction on your commands.

Although concurrent transactions aren't supported, PostgreSQL supports the concept of *savepoints* - you may set named savepoints in a transaction and roll back to them later without rolling back the entire transaction. Savepoints can be created, rolled back to, and released via [`NpgsqlTransaction.SaveAsync()`](https://docs.microsoft.com/dotnet/api/system.data.common.dbtransaction.saveasync), [`RollbackAsync()`](https://docs.microsoft.com/dotnet/api/system.data.common.dbtransaction.rollbackasync) and [`Release(name)`](https://docs.microsoft.com/dotnet/api/system.data.common.dbtransaction.releaseasync) respectively. [See the PostgreSQL documentation for more details.](https://www.postgresql.org/docs/current/static/tutorial-transactions.html).

When starting a transaction, you may optionally set the *isolation level*. [See the docs for more details.](https://www.postgresql.org/docs/current/static/transaction-iso.html)

### System.Transactions and distributed transactions

In addition to `BeginTransaction()`, .NET includes System.Transactions, an alternative API for managing transactions - [read the MSDN docs to understand the concepts involved](https://msdn.microsoft.com/en-us/library/ee818746.aspx). Npgsql fully supports this API, and starting with version 3.3 will automatically enlist to ambient TransactionScopes (you can disable enlistment by specifying `Enlist=false` in your connection string).

When more than one connection (or resource) enlists in the same transaction, the transaction is said to be *distributed*. While .NET Framework supports distributed transaction and Npgsql had limited support for them, .NET Core and .NET 5.0+ do not. It is therefore currently not possible to make use of distributed transactions in modern versions of .NET

Note that if you open and close connections to the same database inside an ambient transaction, without ever having two connections open *at the same time*, Npgsql internally reuses the same connection, avoiding the need for a distributed transaction.

<!-- Distributed transactions allow you to perform changes atomically across more than one database (or resource) via a two-phase commit protocol - [here is the MSDN documentation](https://msdn.microsoft.com/en-us/library/windows/desktop/ms681205(v=vs.85).aspx). Npgsql supports distributed transactions - support has been rewritten for version 3.2, fixing many previous issues. However, at this time Npgsql enlists as a *volatile resource manager*, meaning that if your application crashes while performing, recovery will not be managed properly. For more information about this, [see this page and the related ones](https://msdn.microsoft.com/en-us/library/ee818750.aspx). If you would like to see better distributed transaction recovery (i.e. durable resource manager enlistment), please say so [on this issue](https://github.com/npgsql/npgsql/issues/1378) and subscribe to it for updates.
-->

## Batching

Let's say you need to execute two SQL statements for some reason. This can naively be done as follows:

```c#
await using var cmd = new NpgsqlCommand("INSERT INTO table (col1) VALUES ('foo')", conn);
await cmd.ExecuteNonQueryAsync();

cmd.CommandText = "SELECT * FROM table";
await using var reader = await cmd.ExecuteReaderAsync();
```

The above code needlessly performs two roundtrips to the database: your program will not send the `SELECT` until after the `INSERT` has completed and confirmation for that has been received. Network latency can make this very inefficient: as the distance between your .NET client and PostgreSQL increases, the time spent waiting for packets to cross the network can severely impact your application's performance.

Instead, you can ask Npgsql to send the two SQL statements in a single roundtrip, by using batching:

```c#
await using var batch = new NpgsqlBatch(conn)
{
    BatchCommands =
    {
        new("INSERT INTO table (col1) VALUES ('foo')"),
        new("SELECT * FROM table")
    }
};

await using var reader = await cmd.ExecuteReaderAsync();
```

An <xref:Npgsql.NpgsqlBatch> simply contains a list of `NpgsqlBatchCommands`, each of which has a `CommandText` and a list of parameters (much like an <xref:Npgsql.NpgsqlCommand>). All statements and parameters are efficiently packed into a single packet - when possible - and sent to PostgreSQL.

### Legacy batching

Prior to Npgsql 6.0, `NpgsqlBatch` did not yet exist, and batching could be done as follows:

```c#
await using var cmd = new NpgsqlCommand("INSERT INTO table (col1) VALUES ('foo'); SELECT * FROM table", conn);
await using var reader = await cmd.ExecuteReaderAsync();
```

This packs multiple SQL statements into the `CommandText` of a single `NpgsqlCommand`, delimiting them with semi-colons. This technique is still supported, and can be useful when porting database code from other database. However, legacy batching is generally discouraged since it isn't natively supported by PostgreSQL, forcing Npgsql to parse the SQL to find semicolons. This is similar to *named parameter placeholders*, [see this section for more details](#positional-and-named-placeholders).

## Stored functions and procedures

PostgreSQL supports [stored (or server-side) functions](https://www.postgresql.org/docs/current/static/sql-createfunction.html), and since PostgreSQL 11 also [stored procedures](https://www.postgresql.org/docs/current/sql-createprocedure.html). These can be written in SQL (similar to views), or in [PL/pgSQL](https://www.postgresql.org/docs/current/static/plpgsql.html) (PostgreSQL's procedural language), [PL/Python](https://www.postgresql.org/docs/current/static/plpython.html) or several other server-side languages.

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
