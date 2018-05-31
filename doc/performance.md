# Performance

## Prepared Statements

One of the most important (and easy) ways to improve your application's performance is to prepare your commands. Even if you're not coding against ADO.NET directly (e.g. using Dapper or an O/RM), Npgsql has an automatic preparation feature which allows you to benefit from the performance gains associated with prepared statements. [See this blog post](http://www.roji.org/prepared-statements-in-npgsql-3-2) and/or [the documentation](prepare.md) for more details.

## Batching/Pipelining

When you execute a command, Npgsql executes a roundtrip to the database. If you execute multiple commands (say, inserting 3 rows or performing multiple selects), you're executing multiple roundtrips; each command has to complete before the next command can start execution. Depending on your network latency, this can considerably degrade your application's performance.

You can batch multiple SQL statements in a single command, executing them a single roundtrip:

```c#
using (var cmd = new NpgsqlCommand("SELECT ...; SELECT ..."))
using (var reader = cmd.ExecuteReader())
{
    while (reader.Read()) {
        // Read first resultset
    }
    reader.NextResult();
    while (reader.Read()) {
        // Read second resultset
    }
}
```

## Performance Counters

Npgsql 3.2 includes support for *performance counters*, which provide visibility into connections and the connection pool - this helps you understand what your application is doing in real-time, whether there's a connection leak, etc. Npgsql counter support is very similar to [that of other ADO.NET providers, such as SqlClient](https://msdn.microsoft.com/en-us/library/ms254503(v=vs.110).aspx), it's recommended that your read that page first.

Using performance counters first involves setting them up on your Windows system. To do this you will need to install Npgsql's MSI, which is available on the [github releases page](https://github.com/npgsql/npgsql/releases). Note that GAC installation isn't necessary (or recommended). Once the counters are installed, fire up the Windows Performance Monitor and look for the category ".NET Data Provider for PostgreSQL (Npgsql)".

In addition, you will need to pass `Use Perf Counters=true` on your connection string. Once you start your Npgsql application with this addition, you should start seeing real-time data in the Performance Monitor.

Performance counters are currently only available on Windows with .NET Framework (.NET Core doesn't include performance counters yet).

## Disable enlisting to TransactionScope

By default, Npgsql will enlist to ambient transactions. This occurs when a connection is opened while inside a `TransactionScope`, and can provide a powerful programming model for working with transactions. However, this involves checking whether an ambient transaction is in progress each time a (pooled) connection is open, an operation that takes more time than you'd think. Scenarios where connections are very short-lived and open/close happens very frequently can benefit from removing this check - simply include `Enlist=false` in the connection string. Note that you can still enlist manually by calling `NpgsqlConnection.Enlist()`.

## Pooled Connection Reset

When a pooled connection is closed, Npgsql will arrange for its state to be reset the next time it's used. This prevents leakage of state from one usage cycle of a physical connection to another one. For example, you may change certain PostgreSQL parameters (e.g. `statement_timeout`), and it's undesirable for this change to persist when the connection is closed.

Connection reset happens via the PostgreSQL [`DISCARD ALL` command](https://www.postgresql.org/docs/current/static/sql-discard.html), or, if there are any prepared statements at the time of closing, by a combination of the equivalent statements described in the docs (to prevent closing those statements). Note that these statements aren't actually sent when closing the connection - they're written into Npgsql's internal write buffer, and will be sent with the first user statement after the connection is reopened. This prevents a costly database roundtrip.

If you really want to squeeze every last bit of performance from PostgreSQL, you may disable connect reset by specifying `No Reset On Close` on your connection string - this will slightly improve performance in scenarios where connection are very short-lived, and especially if prepared statements are in use.

## Reading Large Values

When reading results from PostgreSQL, Npgsql first reads raw binary data from the network into an internal read buffer, and then parses that data as you call methods such as `NpgsqlDataReader.GetString()`. While this allows for efficient network reads, it's worth thinking about the size of this buffer, which is 8K by default. Under normal usage,, Npgsql attempts to read each row into the buffer; if that entire row fits in 8K, you'll have optimal performance. However, if a row is bigger than 8K, Npgsql will allocate an "oversize buffer", which will be used until the connection is closed or returned to the pool. If you're not careful, this can create significant memory churn that will slow down your application. To avoid this, if you know you're going to be reading 16k rows, you can specify `Read Buffer Size=18000` in your connection string (leaving some margin for protocol overhead), this will ensure that the read buffer is reused and no extra allocation occur.

Another option is to pass `CommandBehavior.SequentialAccess` to `NpgsqlCommand.ExecuteReader()`. Sequential mode means that Npgsql will no longer read entire rows into its buffer, but will rather fill up the buffer as needed, reading more data only when it's empty. The same 8K read buffer will be used regardless of the row's total size, and Npgsql will take care of the details. In sequential mode, however, you must read the row's fields in the order in which you specified them; you cannot read the 2nd field and then go back to the 1st field, and trying to do so will generate an exception. Similarly, you cannot read the same field twice - once you've read a field, it has been consumed.

For more information on `CommandBehavior.SequentialAccess`, see [this page](https://msdn.microsoft.com/en-us/library/87z0hy49(v=vs.110).aspx). If you decide to use this feature, be aware that it isn't used as often and may therefore contain bugs.

You can also control the socket's receive buffer size (not to be confused with Npgsql's internal buffer) by setting the `Socket Receive Buffer Size` connection string parameter.

## Writing Large Values

Writing is somewhat similar - Npgsql has an internally write buffer (also 8K by default). When writing your query's SQL and parameters to PostgreSQL, Npgsql always writes "sequentially", that is, filling up the 8K buffer and flushing it when full. You can use `Write Buffer Size` to control the buffer's size.

You can also control the socket's receive buffer size (not to be confused with Npgsql's internal buffer) by setting the `Socket Receive Buffer Size` connection string parameter.

## Avoiding boxing when writing parameter values

See [this page](parameters.md).

## Unix Domain Socket

If you're on Linux or macOS and are connecting to a PostgreSQL server on the same machine, you can boost performance a little by connecting via Unix domain socket rather than via a regular TCP/IP socket. To do this, simply specify the directory of your PostgreSQL sockets in the `Host` connection string parameter - if this parameter starts with a slash, it will be taken to mean a filesystem path.

