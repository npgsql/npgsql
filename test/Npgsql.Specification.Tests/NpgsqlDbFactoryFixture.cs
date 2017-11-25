using System;
using System.Data.Common;
using AdoNet.Specification.Tests;

namespace Npgsql.Specification.Tests
{
    public class NpgsqlDbFactoryFixture : IDbFactoryFixture
    {
        public DbProviderFactory Factory => NpgsqlFactory.Instance;

        const string DefaultConnectionString =
            "Server=localhost;Username=npgsql_tests;Password=npgsql_tests;Database=npgsql_tests;Timeout=0;Command Timeout=0";

        public string ConnectionString =>
            Environment.GetEnvironmentVariable("NPGSQL_TEST_DB") ?? DefaultConnectionString;
    }
}
