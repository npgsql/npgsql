using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using Npgsql;

namespace Npgsql.Tests;

[AttributeUsage(AttributeTargets.Assembly)]
public sealed class TestDatabaseAttribute(string databaseName) : Attribute
{
    public string DatabaseName { get; } = databaseName;
}

public static class TestDatabase
{
    static readonly object DatabaseCreationLock = new();
    static readonly ConcurrentDictionary<string, byte> EnsuredConnectionStrings = new(StringComparer.Ordinal);

    public static string CreateConnectionString(string defaultConnectionString, Assembly testAssembly)
    {
        var connectionString = Environment.GetEnvironmentVariable("NPGSQL_TEST_DB") ?? defaultConnectionString;
        var builder = new NpgsqlConnectionStringBuilder(connectionString);

        if (testAssembly.GetCustomAttribute<TestDatabaseAttribute>() is { } testDatabaseAttribute)
            builder.Database = testDatabaseAttribute.DatabaseName;

        return builder.ConnectionString;
    }

    public static void EnsureTestDatabase(string connectionString)
    {
        if (EnsuredConnectionStrings.ContainsKey(connectionString))
            return;

        lock (DatabaseCreationLock)
        {
            if (EnsuredConnectionStrings.ContainsKey(connectionString))
                return;

            using var conn = new NpgsqlConnection(connectionString);
            try
            {
                conn.Open();
            }
            catch (PostgresException e) when (e.SqlState == PostgresErrorCodes.InvalidPassword && Environment.GetEnvironmentVariable("NPGSQL_TEST_DB") is null)
            {
                throw new Exception("Please create a user npgsql_tests as follows: CREATE USER npgsql_tests PASSWORD 'npgsql_tests' SUPERUSER", e);
            }
            catch (PostgresException e) when (e.SqlState == PostgresErrorCodes.InvalidCatalogName)
            {
                var builder = new NpgsqlConnectionStringBuilder(connectionString)
                {
                    Pooling = false,
                    Database = "postgres"
                };

                using var adminConn = new NpgsqlConnection(builder.ConnectionString);
                adminConn.Open();
                try
                {
                    using var command = new NpgsqlCommand(
                        $"CREATE DATABASE {new NpgsqlCommandBuilder().QuoteIdentifier(conn.Database)}", adminConn);
                    command.ExecuteNonQuery();
                }
                catch (PostgresException createException) when (createException.SqlState == PostgresErrorCodes.DuplicateDatabase)
                {
                    // Another test process created the database after our first connection attempt failed.
                }
                adminConn.Close();
                OpenCreatedDatabase(conn);
            }

            EnsuredConnectionStrings.TryAdd(connectionString, 0);
        }
    }

    static void OpenCreatedDatabase(NpgsqlConnection conn)
    {
        var stopwatch = Stopwatch.StartNew();
        while (true)
        {
            try
            {
                conn.Open();
                return;
            }
            catch (PostgresException e) when (
                e.SqlState == PostgresErrorCodes.InvalidCatalogName &&
                stopwatch.Elapsed < TimeSpan.FromSeconds(10))
            {
                Thread.Sleep(100);
            }
        }
    }
}
