using System;
using System.Data.Common;
using System.Threading.Tasks;
using AdoNet.Specification.Tests;
using Testcontainers.PostgreSql;
using Testcontainers.Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Npgsql.Specification.Tests;

public class NpgsqlDbFactoryFixture(IMessageSink messageSink)
    : DbContainerFixture<PostgreSqlBuilder, PostgreSqlContainer>(messageSink), IDbFactoryFixture
{
    readonly IMessageSink _messageSink = messageSink;

    public DbProviderFactory Factory => NpgsqlFactory.Instance;

    public override DbProviderFactory DbProviderFactory => Factory;

    const string DefaultConnectionString =
        "Server=localhost;Username=npgsql_tests;Password=npgsql_tests;Database=npgsql_tests;Timeout=0;Command Timeout=0";

    string _connectionString = Environment.GetEnvironmentVariable("NPGSQL_TEST_DB") ?? DefaultConnectionString;

    public override string ConnectionString => _connectionString;

    protected override PostgreSqlBuilder Configure() => new PostgreSqlBuilder("postgres:18");

    protected override async Task InitializeAsync()
    {
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
        }
        catch (Exception ex) when (Environment.GetEnvironmentVariable("NPGSQL_TEST_DB") == null)
        {
            _messageSink.OnMessage(new DiagnosticMessage($"Failed to connect to \"{_connectionString}\"{Environment.NewLine}{ex}"));
            await base.InitializeAsync();
            _connectionString = base.ConnectionString;
        }
    }

    protected override async Task DisposeAsyncCore()
    {
        if (_connectionString != DefaultConnectionString)
        {
            await base.DisposeAsyncCore();
        }
    }
}
