using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql.Tests;
using Npgsql.Tests.Support;
using NUnit.Framework;

namespace Npgsql.DependencyInjection.Tests;

public class DependencyInjectionTests
{
    [Test]
    public async Task NpgsqlDataSource_is_registered_properly([Values] bool async)
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddNpgsqlDataSource(TestUtil.ConnectionString);

        await using var serviceProvider = serviceCollection.BuildServiceProvider();
        var dataSource = serviceProvider.GetRequiredService<NpgsqlDataSource>();

        await using var connection = async
            ? await dataSource.OpenConnectionAsync()
            : dataSource.OpenConnection();
    }

    [Test]
    public async Task NpgsqlMultiHostDataSource_is_registered_properly([Values] bool async)
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddMultiHostNpgsqlDataSource(TestUtil.ConnectionString);

        await using var serviceProvider = serviceCollection.BuildServiceProvider();
        var multiHostDataSource = serviceProvider.GetRequiredService<NpgsqlMultiHostDataSource>();
        var dataSource = serviceProvider.GetRequiredService<NpgsqlDataSource>();

        Assert.That(dataSource, Is.SameAs(multiHostDataSource));

        await using var connection = async
            ? await dataSource.OpenConnectionAsync()
            : dataSource.OpenConnection();
    }

    [Test]
    public void NpgsqlDataSource_is_registered_as_singleton_by_default()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddNpgsqlDataSource(TestUtil.ConnectionString);

        using var serviceProvider = serviceCollection.BuildServiceProvider();
        using var scope1 = serviceProvider.CreateScope();
        using var scope2 = serviceProvider.CreateScope();
        var scopeServiceProvider1 = scope1.ServiceProvider;
        var scopeServiceProvider2 = scope2.ServiceProvider;

        var dataSource1 = scopeServiceProvider1.GetRequiredService<NpgsqlDataSource>();
        var dataSource2 = scopeServiceProvider2.GetRequiredService<NpgsqlDataSource>();

        Assert.That(dataSource2, Is.SameAs(dataSource1));
    }

    [Test]
    public async Task NpgsqlConnection_is_registered_properly([Values] bool async)
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddNpgsqlDataSource(TestUtil.ConnectionString);

        using var serviceProvider = serviceCollection.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        var scopedServiceProvider = scope.ServiceProvider;

        var connection = scopedServiceProvider.GetRequiredService<NpgsqlConnection>();

        Assert.That(connection.State, Is.EqualTo(ConnectionState.Closed));

        if (async)
            await connection.OpenAsync();
        else
            connection.Open();
    }

    [Test]
    public void NpgsqlConnection_is_registered_as_transient_by_default()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddNpgsqlDataSource("Host=localhost;Username=test;Password=test");

        using var serviceProvider = serviceCollection.BuildServiceProvider();
        using var scope1 = serviceProvider.CreateScope();
        var scopedServiceProvider1 = scope1.ServiceProvider;

        var connection1 = scopedServiceProvider1.GetRequiredService<NpgsqlConnection>();
        var connection2 = scopedServiceProvider1.GetRequiredService<NpgsqlConnection>();

        Assert.That(connection2, Is.Not.SameAs(connection1));

        using var scope2 = serviceProvider.CreateScope();
        var scopedServiceProvider2 = scope2.ServiceProvider;

        var connection3 = scopedServiceProvider2.GetRequiredService<NpgsqlConnection>();
        Assert.That(connection3, Is.Not.SameAs(connection1));
    }

    [Test]
    public async Task LoggerFactory_is_picked_up_from_ServiceCollection()
    {
        var listLoggerProvider = new ListLoggerProvider();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging(b => b.AddProvider(listLoggerProvider));
        serviceCollection.AddNpgsqlDataSource(TestUtil.ConnectionString);
        await using var serviceProvider = serviceCollection.BuildServiceProvider();

        var dataSource = serviceProvider.GetRequiredService<NpgsqlDataSource>();
        await using var command = dataSource.CreateCommand("SELECT 1");

        using (listLoggerProvider.Record())
            _ = command.ExecuteNonQuery();

        Assert.That(listLoggerProvider.Log.Any(l => l.Id == NpgsqlEventId.CommandExecutionCompleted));
    }
}
