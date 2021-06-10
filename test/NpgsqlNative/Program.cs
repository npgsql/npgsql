using System;
using Npgsql;

/// <summary>
/// Unless the NPGSQL_TEST_DB environment variable is defined, this is used as the connection string for the
/// test database.
/// </summary>
var DefaultConnectionString = "Server=localhost;Username=npgsql_tests;Password=npgsql_tests;Database=npgsql_tests;Timeout=0;Command Timeout=0";

/// <summary>
/// The connection string that will be used when opening the connection to the tests database.
/// May be overridden in fixtures, e.g. to set special connection parameters
/// </summary>
var ConnectionString = Environment.GetEnvironmentVariable("NPGSQL_TEST_DB") ?? DefaultConnectionString;

await using var conn = new NpgsqlConnection(ConnectionString);
await conn.OpenAsync();
await using var reader = await new NpgsqlCommand("SELECT '{1,2,3,NULL}'::bigint[]", conn).ExecuteReaderAsync();
while (await reader.ReadAsync())
{
    var value = reader.GetFieldValue<long?[]>(0);
    Console.WriteLine(value);
}
