# Exceptions, errors and notices

## Exception types

Most exceptions thrown by Npgsql are either of type <xref:Npgsql.NpgsqlException>, or wrapped by one; this allows your application to catch `NpgsqlException` where appropriate, for all database-related errors. Note that `NpgsqlException` is a sub-class of the general [System.Data.DbException](https://docs.microsoft.com/dotnet/api/system.data.common.dbexception), so if your application uses more than one database type, you can catch that as well.

When Npgsql itself encounters an error, it typically raises that as an `NpgsqlException` directly, possibly wrapping an inner exception. For example, if a networking error occurs while communicating with PostgreSQL, Npgsql will raise an `NpgsqlException` wrapping an `IOException`; this allow you both to identify the root cause of the problem, while still identifying it as database-related.

In other cases, PostgreSQL itself will report an error to PostgreSQL; Npgsql raises these by throwing a [PostgresExceptions](xref:Npgsql.PostgresException), which is a sub-class of `NpgsqlException` adding important contextual information on the error. Most importantly, `PostgresException` exposes the [SqlState](xref:Npgsql.PostgresException.SqlState) property, which contains the [PostgreSQL error code](https://www.npgsql.org/doc/api/Npgsql.PostgresException.html#Npgsql_PostgresException_SqlState). This value can be consulted to identify which error type occurred.

When executing multiple commands via <xref:Npgsql.NpgsqlBatch>, the <xref:Npgsql.NpgsqlException.BatchCommand> property references the command within the batch which triggered the exception. This allows you to understand exactly what happened, and access the specific SQL which triggered the error.

## PostgreSQL notices

Finally, PostgreSQL also raises "notices", which contain non-critical information on command execution. Notices are not errors: they do not indicate failure and can be safely ignored, although they may contain valuable information on the execution of your commands.

Npgsql logs notices in the *debug* logging level. To deal with notices programmatically, Npgsql also exposes the <xref:Npgsql.NpgsqlConnection.Notice> event, which you can hook into for any further processing:

```c#
conn.Notice += (_, args) => Console.WriteLine(args.Notice.MessageText);
```
