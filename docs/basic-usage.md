# Npgsql Basic Usage

## Data source

> [!NOTE]
> The data source concept was introduced in Npgsql 7.0. If you're using an older version, see [Connections without a data source](#connections-without-a-data-source) below.

Starting with Npgsql 7.0, the starting point for any database operation is <xref:Npgsql.NpgsqlDataSource>. The data source represents your PostgreSQL database, and can hand out connections to it, or support direct execution of SQL against it. The data source encapsulates the various Npgsql configuration needed to connect to PostgreSQL, as well the connection pooling which makes Npgsql efficient.

The simplest way to create a data source is the following:

```csharp
var connectionString = "Host=myserver;Username=mylogin;Password=mypass;Database=mydatabase";
await using var dataSource = NpgsqlDataSource.Create(connectionString);
```

In this code, a data source is created given a *connection string*, which is used to define which database to connect to, the authentication information to use, and various other connection-related parameters. The connection string consists of key/value pairs, separated with semicolons; many options are supported in Npgsql, these are documented on the [connection string page](connection-string-parameters.md).

Npgsql's data source supports additional configuration beyond the connection string, such as logging, advanced authentication options, type mapping management, and more. To further configure a data source, use <xref:Npgsql.NpgsqlDataSourceBuilder> as follows:

```csharp
var dataSourceBuilder = new NpgsqlDataSourceBuilder("Host=localhost;Username=test;Password=test");
dataSourceBuilder
    .UseLoggerFactory(loggerFactory) // Configure logging
    .UsePeriodicPasswordProvider() // Automatically rotate the password periodically
    .UseNodaTime(); // Use NodaTime for date/time types
await using var dataSource = dataSourceBuilder.Build();
```

For more information on data source configuration, consult the relevant documentation pages.

## Basic SQL Execution

Once you have a data source, an <xref:Npgsql.NpgsqlCommand> can be used to execute SQL against it:

```csharp
await using var command = dataSource.CreateCommand("SELECT some_field FROM some_table");
await using var reader = await command.ExecuteReaderAsync();

while (await reader.ReadAsync())
{
    Console.WriteLine(reader.GetString(0));
}
```

More information on executing commands is provided below.

## Connections

In the example above, we didn't deal with a database *connection*; we just executed a command directly against a data source representing the database. Npgsql internally arranges for a connection on which to execute your command, but you don't need to concern yourself with that.

However, in some situations it's necessary to interact with a connection, typically when some sort of state needs to persist across multiple command executions. The common example for this is a database transaction, where multiple commands need to be executed within the same transaction, on the same transaction. A data source also acts as a factory for connections, so you can do the following:

```csharp
await using var connection = await dataSource.OpenConnectionAsync();
```

At this point you have an open connection, and can execute commands against it much like we did against the data source above:

```csharp
await using var command = new NpgsqlCommand("SELECT '8'", connection);
await using var reader = await command.ExecuteReaderAsync();
// Consume the results
```

Connections must be disposed when they are no longer needed - not doing so will result in a connection leak, which can crash your program. In the above code sample, this is done via the `await using` C# construct, which ensures the connection is disposed even if an exception is later thrown. It's a good idea to keep connections open for as little time a possible: database connections are scarce resources, and keeping them open for unnecessarily long times can create unnecessary load in your application and in PostgreSQL.

### Pooling

Opening and closing physical connections to PostgreSQL is an expensive and long process. Therefore, Npgsql connections are *pooled* by default: closing or disposing a connection doesn't close the underlying physical connection, but rather returns it to an internal pool managed by Npgsql. The next time a connection is needed, that pooled connection is returned again. This makes open and close extremely fast operations; do not hesitate to perform them a lot if needed, rather than holding a connection needlessly open for a long time.

For information on tweaking the pooling behavior (or turning it off), see the [pooling section](connection-string-parameters.md#pooling) in the connection string page.

### Connections without a data source

The data source concept is new in Npgsql 7.0, and is the recommended way to use Npgsql. When using older versions, connections where instantiated directly, rather than obtaining them from a data source:

```csharp
await using var conn = new NpgsqlConnection(connectionString);
await conn.OpenAsync();
```

Direct instantiation of connection is still supported, but is discouraged for various reasons when using Npgsql 7.0.

## Other execution methods

Above, we executed SQL via [ExecuteReaderAsync](https://docs.microsoft.com/dotnet/api/system.data.common.dbcommand.executereaderasync). There are other ways to execute a command, based on what results you expect from it:

1. [ExecuteNonQueryAsync](https://docs.microsoft.com/dotnet/api/system.data.common.dbcommand.executenonqueryasync): executes SQL which doesn't return any results, typically `INSERT`, `UPDATE` or `DELETE` statements. Returns the number of rows affected.
2. [ExecuteScalarAsync](https://docs.microsoft.com/dotnet/api/system.data.common.dbcommand.executescalarasync): executes SQL which returns a single, scalar value.
3. [ExecuteReaderAsync](https://docs.microsoft.com/dotnet/api/system.data.common.dbcommand.executereaderasync): execute SQL which returns a full resultset. Returns an <xref:Npgsql.NpgsqlDataReader> which can be used to access the resultset (as in the above example).

For example, to execute a simple SQL `INSERT` which does not return anything, you can use `ExecuteNonQueryAsync` as follows:

```csharp
await using var command = dataSource.CreateCommand("INSERT INTO some_table (some_field) VALUES (8)");
await command.ExecuteNonQueryAsync();
```

Note that each execute method involves a database roundtrip. To execute multiple SQL statements in a single roundtrip, see the [batching section](#batching) below.

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

Transactions can be started by calling the standard ADO.NET method [`NpgsqlConnection.BeginTransaction()`](https://learn.microsoft.com/dotnet/api/system.data.common.dbconnection.begintransactionasync?view=net-6.0):

```csharp
await using var connection = await dataSource.OpenConnectionAsync();
await using var transaction = await connection.BeginTransactionAsync();

await using var command1 = new NpgsqlCommand("...", connection, transaction);
await command1.ExecuteNonQueryAsync();

await using var command2 = new NpgsqlCommand("...", connection, transaction);
await command2.ExecuteNonQueryAsync();

await transaction.CommitAsync();
```

PostgreSQL doesn't support nested or concurrent transactions - only one transaction may be in progress at any given moment (starting a transaction while another transaction is already in progress throws an exception). Because of this, it isn't necessary to pass the NpgsqlTransaction object returned from `BeginTransaction()` to commands you execute - starting a transaction means that all subsequent commands will automatically participate in the transaction, until either a commit or rollback is performed. However, for maximum portability it's recommended to set the transaction on your commands.

Although concurrent transactions aren't supported, PostgreSQL supports the concept of *savepoints* - you may set named savepoints in a transaction and roll back to them later without rolling back the entire transaction. Savepoints can be created, rolled back to, and released via [`NpgsqlTransaction.SaveAsync()`](https://docs.microsoft.com/dotnet/api/system.data.common.dbtransaction.saveasync), [`RollbackAsync()`](https://docs.microsoft.com/dotnet/api/system.data.common.dbtransaction.rollbackasync) and [`Release(name)`](https://docs.microsoft.com/dotnet/api/system.data.common.dbtransaction.releaseasync) respectively. [See the PostgreSQL documentation for more details.](https://www.postgresql.org/docs/current/static/tutorial-transactions.html).

When starting a transaction, you may optionally set the *isolation level*. [See the docs for more details.](https://www.postgresql.org/docs/current/static/transaction-iso.html)

### System.Transactions and distributed transactions

In addition to `BeginTransactionAsync()`, .NET includes System.Transactions, an alternative API for managing transactions - [read the MSDN docs to understand the concepts involved](https://msdn.microsoft.com/library/ee818746.aspx). Npgsql fully supports this API, and automatically enlists if a connection is opened within an ambient TransactionScopes.

When a transaction includes more than one database (or even more than one concurrent connections to the same database), the transaction is said to be *distributed*. .NET 7.0 brings the same distributed transaction support that .NET Framework supported, for Windows only. While Npgsql partially supports this mechanism, it does not implement the recovery parts of the distributed transaction, because of some design issues with .NET's support. While distributed transactions may work for you, it is discouraged to fully rely on them with Npgsql.

Note that if you open and close connections to the same database inside an ambient transaction, without ever having two connections open *at the same time*, Npgsql internally reuses the same connection, avoiding the need for a distributed transaction.

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

> [!NOTE]
> If you haven't started an explicit transaction with <xref:Npgsql.NpgsqlConnection.BeginTransaction>, a batch is automatically wrapped in an implicit transaction. That is, if a statement within the batch fails, all later statements are skipped and the entire batch is rolled back.

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
using var cmd = new NpgsqlCommand("SELECT my_func(1, 2)", conn);
using var reader = cmd.ExecuteReader();

// For procedures
using var cmd = new NpgsqlCommand("CALL my_proc(1, 2)", conn);
using var reader = cmd.ExecuteReader();
```

You can replace the parameter values above with regular placeholders (e.g. `$1`), just like with a regular query.

### CommandType.StoredProcedure

> [!WARNING]
> Starting with Npgsql 7.0, [`CommandType.StoredProcedure`](https://learn.microsoft.com/dotnet/api/system.data.commandtype#system-data-commandtype-storedprocedure) now invokes stored procedures, and not function as before. See the [release notes](release-notes/7.0.md#commandtype_storedprocedure) for more information and how to opt out of this change.

In some other databases, calling a stored procedures involves setting the command's `CommandType`:

```c#
using var command1 = new NpgsqlCommand("my_procedure", connection)
{
    CommandType = CommandType.StoredProcedure,
    Parameters =
    {
        new() { Value = 8 }
    }
};
await using var reader = await command1.ExecuteReaderAsync();
```

Npgsql supports this mainly for portability, but this style of calling has no advantage over the regular command shown above. When `CommandType.StoredProcedure` is set, Npgsql will simply generate the appropriate `CALL my_procedure($1)` for you, nothing more. Unless you have specific portability requirements, it is recommended you simply avoid `CommandType.StoredProcedure` and construct the SQL yourself.

Be aware that `CommandType.StoredProcedure` generates a `CALL` command, which is suitable for invoking stored procedures and not functions. Versions of Npgsql prior to 7.0 generated a `SELECT` command suitable for functions, and this legacy behavior can be enabled; see the [7.0 release notes](release-notes/7.0.md#commandtype_storedprocedure)

Note that if `CommandType.StoredProcedure` is set and your parameter instances have names, Npgsql generates parameters with `named notation`: `SELECT my_func(p1 => 'some_value')`. This means that your NpgsqlParameter names must match your PostgreSQL procedure or function parameters, or the call will fail. If you omit the names on your NpgsqlParameters, positional notation will be used instead. Note that positional parameters must always come before named ones. [See the PostgreSQL docs for more info](https://www.postgresql.org/docs/current/static/sql-syntax-calling-funcs.html).

### Function in/out parameters

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
