using System;
using Npgsql;

var connectionString = Environment.GetEnvironmentVariable("NPGSQL_TEST_DB")
                       ?? "Server=localhost;Username=npgsql_tests;Password=npgsql_tests;Database=npgsql_tests;Timeout=0;Command Timeout=0";

await using var conn = new NpgsqlConnection(connectionString);
await conn.OpenAsync();
await using var cmd = new NpgsqlCommand("SELECT 'Hello World'", conn);
await using var reader = await cmd.ExecuteReaderAsync();
while (await reader.ReadAsync())
{
    var value = reader.GetFieldValue<string>(0);
    if (value != "Hello World")
        throw new Exception($"Got {value} instead of the expected 'Hello World'");
}
