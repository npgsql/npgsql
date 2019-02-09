using System;

namespace Npgsql.CrateDb.Tests
{
    public abstract class TestBase
    {
        /// <summary>
        /// The connection string that will be used when opening the connection to the tests database.
        /// May be overridden in fixtures, e.g. to set special connection parameters
        /// </summary>
        public static string ConnectionString =>
            Environment.GetEnvironmentVariable("NPGSQL_CRATE_TEST_DB") ?? DefaultConnectionString;

        /// <summary>
        /// Unless the NPGSQL_CRATE_TEST_DB environment variable is defined, this is used as the connection string for the
        /// test database.
        /// </summary>
        const string DefaultConnectionString = "Server=localhost;Port=5435;User ID=crate;Database=npgsql_tests;Timeout=0;Command Timeout=0";

        protected virtual NpgsqlConnection OpenConnection(string connectionString = null)
        {
            if (connectionString == null)
                connectionString = ConnectionString;
            var conn = new NpgsqlConnection(connectionString);
            conn.Open();
            return conn;
        }
    }
}
