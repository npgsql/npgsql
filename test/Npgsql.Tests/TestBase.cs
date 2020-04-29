using System;
using System.Threading;
using static Npgsql.Util.Statics;

namespace Npgsql.Tests
{
    public abstract class TestBase
    {
        /// <summary>
        /// The connection string that will be used when opening the connection to the tests database.
        /// May be overridden in fixtures, e.g. to set special connection parameters
        /// </summary>
        public static string ConnectionString =>
            Environment.GetEnvironmentVariable("NPGSQL_TEST_DB") ?? DefaultConnectionString;

        /// <summary>
        /// Unless the NPGSQL_TEST_DB environment variable is defined, this is used as the connection string for the
        /// test database.
        /// </summary>
        const string DefaultConnectionString = "Server=localhost;Username=test;Password=test;Database=npgsql_tests;Timeout=0;Command Timeout=0";

        static readonly object DatabaseCreationLock = new object();

        #region Utilities for use by tests

        protected virtual NpgsqlConnection CreateConnection(string? connectionString = null)
        {
            connectionString ??= ConnectionString;
            var conn = new NpgsqlConnection(connectionString);
            return conn;
        }

        protected virtual NpgsqlConnection OpenConnection(string? connectionString = null)
        {
            while (true)
            {
                var conn = CreateConnection(connectionString);
                try
                {
                    conn.Open();
                }
                catch (PostgresException e) when (e.SqlState == PostgresErrorCodes.InvalidCatalogName)
                {
                    // Database does not exist, attempt to create it by connecting to database postgres,
                    // which should always exist. Make sure we don't have multiple threads trying to create
                    // a database at the same time - PostgreSQL does not support this.
                    var gotLock = Monitor.TryEnter(DatabaseCreationLock);
                    if (gotLock)
                    {
                        using var _ = Defer(() => Monitor.Exit(DatabaseCreationLock));

                        var builder = new NpgsqlConnectionStringBuilder(connectionString ?? ConnectionString)
                        {
                            Pooling = false,
                            Database = "postgres"
                        };

                        using var adminConn = new NpgsqlConnection(builder.ConnectionString);
                        adminConn.Open();
                        adminConn.ExecuteNonQuery("CREATE DATABASE " + conn.Database);
                        adminConn.Close();
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        // Wait for the other thread to create it
                        lock (DatabaseCreationLock) {}
                    }

                    continue;
                }

                return conn;
            }
        }

        protected NpgsqlConnection OpenConnection(NpgsqlConnectionStringBuilder csb)
            => OpenConnection(csb.ToString());

        // In PG under 9.1 you can't do SELECT pg_sleep(2) in binary because that function returns void and PG doesn't know
        // how to transfer that. So cast to text server-side.
        protected static NpgsqlCommand CreateSleepCommand(NpgsqlConnection conn, int seconds = 1000)
            => new NpgsqlCommand($"SELECT pg_sleep({seconds}){(conn.PostgreSqlVersion < new Version(9, 1, 0) ? "::TEXT" : "")}", conn);

        protected bool IsRedshift => new NpgsqlConnectionStringBuilder(ConnectionString).ServerCompatibilityMode == ServerCompatibilityMode.Redshift;

        #endregion
    }
}
