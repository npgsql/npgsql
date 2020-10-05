using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql.Tests
{
    public abstract class TestBase
    {
        /// <summary>
        /// The connection string that will be used when opening the connection to the tests database.
        /// May be overridden in fixtures, e.g. to set special connection parameters
        /// </summary>
        public virtual string ConnectionString => TestUtil.ConnectionString;

        #region Utilities for use by tests

        protected virtual NpgsqlConnection CreateConnection(string? connectionString = null)
            => new NpgsqlConnection(connectionString ?? ConnectionString);

        protected virtual NpgsqlConnection CreateConnection(Action<NpgsqlConnectionStringBuilder> builderAction)
        {
            var builder = new NpgsqlConnectionStringBuilder(ConnectionString);
            builderAction(builder);
            return new NpgsqlConnection(builder.ConnectionString);
        }

        protected virtual NpgsqlConnection OpenConnection(string? connectionString = null)
            => OpenConnection(connectionString, async: false).GetAwaiter().GetResult();

        protected virtual NpgsqlConnection OpenConnection(Action<NpgsqlConnectionStringBuilder> builderAction)
        {
            var builder = new NpgsqlConnectionStringBuilder(ConnectionString);
            builderAction(builder);
            return OpenConnection(builder.ConnectionString, async: false).GetAwaiter().GetResult();
        }

        protected virtual ValueTask<NpgsqlConnection> OpenConnectionAsync(string? connectionString = null)
            => OpenConnection(connectionString, async: true);

        protected virtual ValueTask<NpgsqlConnection> OpenConnectionAsync(
            Action<NpgsqlConnectionStringBuilder> builderAction)
        {
            var builder = new NpgsqlConnectionStringBuilder(ConnectionString);
            builderAction(builder);
            return OpenConnection(builder.ConnectionString, async: true);
        }

        async ValueTask<NpgsqlConnection> OpenConnection(string? connectionString, bool async)
        {
            var conn = CreateConnection(connectionString);
            try
            {
                if (async)
                    await conn.OpenAsync();
                else
                    conn.Open();
            }
            catch (PostgresException e)
            {
                if (e.SqlState == PostgresErrorCodes.InvalidCatalogName)
                    TestUtil.IgnoreExceptOnBuildServer("Please create a database npgsql_tests, owned by user npgsql_tests");
                else if (e.SqlState == PostgresErrorCodes.InvalidPassword && connectionString == TestUtil.DefaultConnectionString)
                    TestUtil.IgnoreExceptOnBuildServer("Please create a user npgsql_tests as follows: create user npgsql_tests with password 'npgsql_tests'");
                else
                    throw;
            }

            return conn;
        }

        protected NpgsqlConnection OpenConnection(NpgsqlConnectionStringBuilder csb)
            => OpenConnection(csb.ToString());

        protected virtual ValueTask<NpgsqlConnection> OpenConnectionAsync(NpgsqlConnectionStringBuilder csb)
            => OpenConnectionAsync(csb.ToString());

        // In PG under 9.1 you can't do SELECT pg_sleep(2) in binary because that function returns void and PG doesn't know
        // how to transfer that. So cast to text server-side.
        protected static NpgsqlCommand CreateSleepCommand(NpgsqlConnection conn, int seconds = 1000)
            => new NpgsqlCommand($"SELECT pg_sleep({seconds}){(conn.PostgreSqlVersion < new Version(9, 1, 0) ? "::TEXT" : "")}", conn);

        protected bool IsRedshift => new NpgsqlConnectionStringBuilder(ConnectionString).ServerCompatibilityMode == ServerCompatibilityMode.Redshift;

        #endregion
    }
}
