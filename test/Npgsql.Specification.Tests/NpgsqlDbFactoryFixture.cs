using System;
using System.Data.Common;
using AdoNet.Specification.Tests;
using Npgsql.Tests;

namespace Npgsql.Specification.Tests;

public class NpgsqlDbFactoryFixture : IDbFactoryFixture
{
    public DbProviderFactory Factory => NpgsqlFactory.Instance;

    const string DefaultConnectionString =
        "Server=localhost;Username=npgsql_tests;Password=npgsql_tests;Database=npgsql_tests;Timeout=0;Command Timeout=0";

    static readonly Lazy<string> ConnectionStringSource = new(() =>
    {
        var connectionString = TestDatabase.CreateConnectionString(DefaultConnectionString, typeof(NpgsqlDbFactoryFixture).Assembly);
        TestDatabase.EnsureTestDatabase(connectionString);
        return connectionString;
    });

    public string ConnectionString => ConnectionStringSource.Value;
}
