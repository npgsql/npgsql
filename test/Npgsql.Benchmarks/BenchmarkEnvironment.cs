using System;

namespace Npgsql.Benchmarks;

static class BenchmarkEnvironment
{
    internal static string ConnectionString => Environment.GetEnvironmentVariable("NPGSQL_TEST_DB") ?? DefaultConnectionString;

    /// <summary>
    /// Unless the NPGSQL_TEST_DB environment variable is defined, this is used as the connection string for the
    /// test database.
    /// </summary>
    const string DefaultConnectionString = "Server=localhost;User ID=npgsql_tests;Password=npgsql_tests;Database=npgsql_tests";

    internal static NpgsqlConnection GetConnection() => new(ConnectionString);

    internal static NpgsqlConnection OpenConnection()
    {
        var conn = GetConnection();
        conn.Open();
        return conn;
    } 
}