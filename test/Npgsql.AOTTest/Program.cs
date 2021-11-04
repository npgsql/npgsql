using System;
using Npgsql;

var connectionString = Environment.GetEnvironmentVariable("NPGSQL_TEST_DB")
                       ?? "Server=localhost;Username=npgsql_tests;Password=npgsql_tests;Database=npgsql_tests;Timeout=0;Command Timeout=0";

await using var conn = new NpgsqlConnection(connectionString);
await conn.OpenAsync();
await using var cmd = new NpgsqlCommand("SELECT 'Hello World'", conn);
var result = (string)(await cmd.ExecuteScalarAsync())!;

if (result != "Hello World")
    throw new Exception($"Got {result} instead of the expected 'Hello World'");

Console.WriteLine("Successfully fetched value from PostgreSQL");
