# Prepared Statements

## Introduction

Most applications repeat the same SQL statements many times, passing different parameters. In such cases, it's very beneficial to *prepare* commands - this will send the command's statement(s) to PostgreSQL, which will parse and plan for them. The prepared statements can then be used on execution, saving valuable planning time. The more complex your queries, the more you'll notice the performance gain; but even very simple queries tend to benefit from preparation.

Following is a benchmark Npgsql.Benchmarks.Prepare, which measures the execution time of the same query, executed prepared and unprepared. `TablesToJoin` is a parameter which increases the query complexity - it determines how many tables the query joins from.

       Method | TablesToJoin |           Mean |      StdDev |     Op/s | Scaled | Scaled-StdDev | Allocated |
------------- |------------- |--------------- |------------ |--------- |------- |-------------- |---------- |
   **Unprepared** |            **0** |     **73.5353 us** |   **0.6033 us** | **13598.91** |   **1.00** |          **0.00** |   **3.36 kB** |
     Prepared |            0 |     47.0393 us |   0.4579 us |  21258.8 |   0.64 |          0.01 |    1.4 kB |
   **Unprepared** |            **1** |    **106.7306 us** |   **1.2891 us** |  **9369.39** |   **1.00** |          **0.00** |    **3.5 kB** |
     Prepared |            1 |     59.4692 us |   0.7175 us | 16815.41 |   0.56 |          0.01 |    1.4 kB |
   **Unprepared** |            **2** |    **189.4992 us** |   **2.0745 us** |  **5277.07** |   **1.00** |          **0.00** |   **3.73 kB** |
     Prepared |            2 |     76.0352 us |   1.5647 us | 13151.81 |   0.40 |          0.01 |    1.4 kB |
   **Unprepared** |            **5** |  **1,089.1583 us** |   **9.5710 us** |   **918.14** |   **1.00** |          **0.00** |   **4.35 kB** |
     Prepared |            5 |    111.6496 us |   0.9084 us |  8956.59 |   0.10 |          0.00 |    1.4 kB |
   **Unprepared** |           **10** | **23,293.9641 us** | **215.0200 us** |    **42.93** |   **1.00** |          **0.00** |   **5.63 kB** |
     Prepared |           10 |    180.7498 us |   1.8213 us |  5532.51 |   0.01 |          0.00 |    1.4 kB |

As is immediately apparent, even an extremely simple scenario (TablesToJoin=0, SQL=SELECT 1), preparing the query with PostgreSQL provides a 36% speedup. As query complexity increases by adding join tables, the gap widens dramatically.

The only potential disadvantage of prepared statements is that they hold server-side resources (e.g. cached plans). If you're dynamically generating SQL queries, make sure you don't overwhelm the server by preparing too much. Most reasonable applications shouldn't have to worry about this.

## Simple Preparation
     
To prepare your commands, simply use the following standard ADO.NET code:

```c#
var cmd = new NpgsqlCommand(...);
cmd.Parameters.Add("param", NpgsqlDbType.In
cmd.Prepare();
// Set parameters
cmd.ExecuteNonQuery();
// And so on
```

Note that all parameters must be set before calling `Prepare()` - they are part of the information transmitted to PostgreSQL and used to effectively plan the statement. You must also set the `DbType` or `NpgsqlDbType` on your parameters to unambiguously specify the data type (setting the value isn't support).

Note that preparation happens on individual statements, and not on commands, which can contain multiple statements, batching them together. This can be important in cases such as the following:

```c#
var cmd = new NpgsqlCommand("UPDATE foo SET bar=@bar WHERE baz=@baz; UPDATE foo SET bar=@bar WHERE baz=@baz");
// set parameters.
cmd.Prepare();
```

Although there are two statements in this command, the same prepared statement is used to execute since the SQL is identical.

## Persistency

Prior to 3.2, prepared statements were closed when their owning command was disposed. This significantly reduced their usefulness, especially since closing a pooled connection automatically closed all prepared statements. For applications where connections are short-lived, such as most web applications, this effectively made prepared statements useless.

Starting from 3.2, all prepared statements are persistent - they no longer get closed when a command or connection is closed. Npgsql keeps track of statements prepared on each physical connection; if you prepare the same SQL a second time on the same connection, Npgsql will simply reuse the prepared statement from the first preparation. This means that in an application with short-lived, pooled connections, prepared statements will gradually be created as the application warms up and the connections are first used. Then, opening a new pooled connection will return a physical connection that already has a prepared statement for your SQL, providing a very substantial performance boost. For example:

```c#
using (var conn = new NpgsqlConnection(...)
using (var cmd = new NpgsqlCommand("<some_sql>", conn) {
    conn.Open();
    conn.Prepare();   // First time on this physical connection, Npgsql prepares with PostgreSQL
    conn.ExecuteNonQuery();
}

using (var conn = new NpgsqlConnection(...)
using (var cmd = new NpgsqlCommand("<some_sql>", conn) {
    conn.Open();      // We assume the pool returned the same physical connection used above
    conn.Prepare();   // The connection already has a prepared statement for <some_sql>, this doesn't need to do anything
    conn.ExecuteNonQuery();
}
```

You can still choose to close a prepared statement by calling `NpgsqlCommand.Unprepare()`. You can also unprepare all statements on a given connection by calling `NpgsqlConnection.UnprepareAll()`.

## Automatic Preparation

While the preparation examples shown above provide a very significant performance boost, they depend on you calling the `Prepare()` command. Unfortunately, if you're using some data layer above ADO.NET, such as [Dapper](https://github.com/StackExchange/dapper-dot-net) or [Entity Framework](https://docs.microsoft.com/en-us/ef/), chances are these layers don't prepare for you. While issues exist for both [Dapper](https://github.com/StackExchange/dapper-dot-net/issues/474) and [Entity Framework Core](https://github.com/aspnet/EntityFramework/issues/5459), they don't take advantage of prepared statement at the moment.

Npgsql 3.2 introduces automatic preparation. When turned on, this will make Npgsql track the statements you execute and automatically prepare them when you reach a certain threshold. When you reach that threshold, the statement is automatically prepared, and from that point on will be executed as prepared, yielding all the performance benefits discussed above. To turn on this feature, you simply need to set the `Max Auto Prepare` connection string parameter, which determines how many statements can be automatically prepared on the connection at any given time (this parameter defaults to 0, disabling the feature). A second parameter, `Auto Prepare Min Usages`, determines how many times a statement needs to be executed before it is auto-prepared (defaults to 5). Since no code changes are required, you can simply try setting `Max Auto Prepare` and running your application to see an immediate speed increase. Note also that, like explicitly-prepared statements, auto-prepared statements are persistent, allowing you to reap the performance benefits in short-lived connection applications.

Note that if you're coding directly against Npgsql or ADO.NET, explicitly preparing your commands with `Prepare()` is still recommended over letting Npgsql prepare automatically. Automatic preparation does incur a slight performance cost compared to explicit preparation, because of the internal LRU cache and various book-keeping data structures. Explicitly preparing also allows you to better control exactly which statements are prepared and which aren't, and ensures your statements will always stay prepared, and never get ejected because of the LRU mechanism.

Note that automatic preparation is a complex new feature which should be considered somewhat experimental; test carefully, and if you see any strange behavior or problem try turning it off.

The following benchmark results show automatic preparation alongside explicit preparation (and unprepared execution):

       Method | TablesToJoin |           Mean |      StdDev |     Op/s | Scaled | Scaled-StdDev | Allocated |
------------- |------------- |--------------- |------------ |--------- |------- |-------------- |---------- |
   **Unprepared** |            **0** |     **73.5353 us** |   **0.6033 us** | **13598.91** |   **1.00** |          **0.00** |   **3.36 kB** |
 AutoPrepared |            0 |     50.6906 us |   0.5909 us | 19727.53 |   0.69 |          0.01 |   2.19 kB |
     Prepared |            0 |     47.0393 us |   0.4579 us |  21258.8 |   0.64 |          0.01 |    1.4 kB |
   **Unprepared** |            **1** |    **106.7306 us** |   **1.2891 us** |  **9369.39** |   **1.00** |          **0.00** |    **3.5 kB** |
 AutoPrepared |            1 |     62.2421 us |   0.4101 us | 16066.31 |   0.58 |          0.01 |   2.35 kB |
     Prepared |            1 |     59.4692 us |   0.7175 us | 16815.41 |   0.56 |          0.01 |    1.4 kB |
   **Unprepared** |            **2** |    **189.4992 us** |   **2.0745 us** |  **5277.07** |   **1.00** |          **0.00** |   **3.73 kB** |
 AutoPrepared |            2 |     87.4430 us |   4.0486 us | 11436.02 |   0.46 |          0.02 |   2.56 kB |
     Prepared |            2 |     76.0352 us |   1.5647 us | 13151.81 |   0.40 |          0.01 |    1.4 kB |
   **Unprepared** |            **5** |  **1,089.1583 us** |   **9.5710 us** |   **918.14** |   **1.00** |          **0.00** |   **4.35 kB** |
 AutoPrepared |            5 |    131.4522 us |   1.0954 us |  7607.33 |   0.12 |          0.00 |   3.15 kB |
     Prepared |            5 |    111.6496 us |   0.9084 us |  8956.59 |   0.10 |          0.00 |    1.4 kB |
   **Unprepared** |           **10** | **23,293.9641 us** | **215.0200 us** |    **42.93** |   **1.00** |          **0.00** |   **5.63 kB** |
 AutoPrepared |           10 |    199.7185 us |   2.4733 us |  5007.05 |   0.01 |          0.00 |   4.16 kB |
     Prepared |           10 |    180.7498 us |   1.8213 us |  5532.51 |   0.01 |          0.00 |    1.4 kB |