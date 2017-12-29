# Parameters

## Avoiding boxing with strongly-typed NpgsqlParameter<T>

In standard ADO.NET, data values are sent to the database via a subclass of DbParameter (NpgsqlParameter in Npgsql's case). This API requires you to set the data via `DbParameter.Value` which, being an `object`, will box value types such as int. If you're sending lots of value types to the database, this will create large amounts of useless heap allocations and strain the garbage collector.

Npgsql 3.3 introduces a solution to this problem: `NpgsqlParameter<T>`. This generic class has a `TypedValue` member, which is similar to `NpgsqlParameter.Value` but which is strongly-typed, thus avoiding the boxing and heap allocation.

Note that this strongly-typed parameter API is entirely Npgsql-specific, and will make your code non-portable to other database. See [#8955](https://github.com/dotnet/corefx/issues/8955) for an issue discussing this at the ADO.NET level.

