using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using NUnit.Framework;

// ReSharper disable MethodHasAsyncOverload

namespace Npgsql.Tests;

public class DataSourceTests : TestBase
{
    [Test]
    public async Task CreateConnection()
    {
        await using var dataSource = NpgsqlDataSource.Create(ConnectionString);
        await using var connection = dataSource.CreateConnection();
        Assert.That(connection.State, Is.EqualTo(ConnectionState.Closed));

        await connection.OpenAsync();
        Assert.That(await connection.ExecuteScalarAsync("SELECT 1"), Is.EqualTo(1));
    }

    [Test]
    public async Task OpenConnection([Values] bool async)
    {
        await using var dataSource = NpgsqlDataSource.Create(ConnectionString);
        await using var connection = async
            ? await dataSource.OpenConnectionAsync()
            : dataSource.OpenConnection();

        Assert.That(connection.State, Is.EqualTo(ConnectionState.Open));

        Assert.That(await connection.ExecuteScalarAsync("SELECT 1"), Is.EqualTo(1));
    }

    [Test]
    public async Task ExecuteScalar_on_connectionless_command([Values] bool async)
    {
        await using var dataSource = NpgsqlDataSource.Create(ConnectionString);
        await using var command = dataSource.CreateCommand();
        command.CommandText = "SELECT 1";

        if (async)
            Assert.That(await command.ExecuteScalarAsync(), Is.EqualTo(1));
        else
            Assert.That(command.ExecuteScalar(), Is.EqualTo(1));

        Assert.That(dataSource.Statistics, Is.EqualTo((Total: 1, Idle: 1, Busy: 0)));
    }

    [Test]
    public async Task ExecuteNonQuery_on_connectionless_command([Values] bool async)
    {
        await using var dataSource = NpgsqlDataSource.Create(ConnectionString);
        await using var command = dataSource.CreateCommand();
        command.CommandText = "SELECT 1";

        if (async)
            Assert.That(await command.ExecuteNonQueryAsync(), Is.EqualTo(-1));
        else
            Assert.That(command.ExecuteNonQuery(), Is.EqualTo(-1));

        Assert.That(dataSource.Statistics, Is.EqualTo((Total: 1, Idle: 1, Busy: 0)));
    }

    [Test]
    public async Task ExecuteReader_on_connectionless_command([Values] bool async)
    {
        await using var dataSource = NpgsqlDataSource.Create(ConnectionString);
        await using var command = dataSource.CreateCommand();
        command.CommandText = "SELECT 1";

        using (var reader = async ? await command.ExecuteReaderAsync() : command.ExecuteReader())
        {
            Assert.True(reader.Read());
            Assert.That(reader.GetInt32(0), Is.EqualTo(1));
        }

        Assert.That(dataSource.Statistics, Is.EqualTo((Total: 1, Idle: 1, Busy: 0)));
    }

    [Test]
    public async Task ExecuteScalar_on_connectionless_batch([Values] bool async)
    {
        await using var dataSource = NpgsqlDataSource.Create(ConnectionString);
        await using var batch = dataSource.CreateBatch();
        batch.BatchCommands.Add(new("SELECT 1"));
        batch.BatchCommands.Add(new("SELECT 2"));

        if (async)
            Assert.That(await batch.ExecuteScalarAsync(), Is.EqualTo(1));
        else
            Assert.That(batch.ExecuteScalar(), Is.EqualTo(1));

        Assert.That(dataSource.Statistics, Is.EqualTo((Total: 1, Idle: 1, Busy: 0)));
    }

    [Test]
    public async Task ExecuteNonQuery_on_connectionless_batch([Values] bool async)
    {
        await using var dataSource = NpgsqlDataSource.Create(ConnectionString);
        await using var batch = dataSource.CreateBatch();
        batch.BatchCommands.Add(new("SELECT 1"));
        batch.BatchCommands.Add(new("SELECT 2"));

        if (async)
            Assert.That(await batch.ExecuteNonQueryAsync(), Is.EqualTo(-1));
        else
            Assert.That(batch.ExecuteNonQuery(), Is.EqualTo(-1));

        Assert.That(dataSource.Statistics, Is.EqualTo((Total: 1, Idle: 1, Busy: 0)));
    }

    [Test]
    public async Task ExecuteReader_on_connectionless_batch([Values] bool async)
    {
        await using var dataSource = NpgsqlDataSource.Create(ConnectionString);
        await using var batch = dataSource.CreateBatch();
        batch.BatchCommands.Add(new("SELECT 1"));
        batch.BatchCommands.Add(new("SELECT 2"));

        using (var reader = async ? await batch.ExecuteReaderAsync() : batch.ExecuteReader())
        {
            Assert.True(reader.Read());
            Assert.That(reader.GetInt32(0), Is.EqualTo(1));
            Assert.True(reader.NextResult());
            Assert.True(reader.Read());
            Assert.That(reader.GetInt32(0), Is.EqualTo(2));
        }

        Assert.That(dataSource.Statistics, Is.EqualTo((Total: 1, Idle: 1, Busy: 0)));
    }

    [Test]
    public void Dispose()
    {
        using var dataSource = NpgsqlDataSource.Create(ConnectionString);
        var connection1 = dataSource.OpenConnection();
        var connection2 = dataSource.OpenConnection();
        connection1.Close();

        Assert.That(dataSource.Statistics, Is.EqualTo((Total: 2, Idle: 1, Busy: 1)));

        dataSource.Dispose();

        Assert.That(dataSource.Statistics, Is.EqualTo((Total: 1, Idle: 0, Busy: 1)));

        Assert.That(() => dataSource.OpenConnection(), Throws.Exception.TypeOf<ObjectDisposedException>());
        Assert.That(dataSource.Statistics, Is.EqualTo((Total: 1, Idle: 0, Busy: 1)));

        connection2.Close();
        Assert.That(dataSource.Statistics, Is.EqualTo((Total: 0, Idle: 0, Busy: 0)));
    }

    [Test]
    public async Task DisposeAsync()
    {
        await using var dataSource = NpgsqlDataSource.Create(ConnectionString);
        var connection1 = await dataSource.OpenConnectionAsync();
        var connection2 = await dataSource.OpenConnectionAsync();
        await connection1.CloseAsync();

        Assert.That(dataSource.Statistics, Is.EqualTo((Total: 2, Idle: 1, Busy: 1)));

        await dataSource.DisposeAsync();
        Assert.That(() => dataSource.OpenConnectionAsync(), Throws.Exception.TypeOf<ObjectDisposedException>());
        Assert.That(dataSource.Statistics, Is.EqualTo((Total: 1, Idle: 0, Busy: 1)));

        await connection2.CloseAsync();
        Assert.That(dataSource.Statistics, Is.EqualTo((Total: 0, Idle: 0, Busy: 0)));
    }

    [Test]
    public void No_password_without_PersistSecurityInfo()
    {
        if (string.IsNullOrEmpty(new NpgsqlConnectionStringBuilder(ConnectionString).Password))
            Assert.Fail("No password in default connection string, test cannot run");

        using var dataSource = NpgsqlDataSource.Create(ConnectionString);
        var parsedConnectionString = new NpgsqlConnectionStringBuilder(dataSource.ConnectionString);
        Assert.That(parsedConnectionString.Password, Is.Null);
    }

    [Test]
    public async Task Cannot_access_connection_transaction_on_data_source_command()
    {
        await using var command = SharedDataSource.CreateCommand();

        Assert.That(() => command.Connection, Throws.Exception.TypeOf<NotSupportedException>());
        Assert.That(() => command.Connection = null, Throws.Exception.TypeOf<NotSupportedException>());
        Assert.That(() => command.Transaction, Throws.Exception.TypeOf<NotSupportedException>());
        Assert.That(() => command.Transaction = null, Throws.Exception.TypeOf<NotSupportedException>());

        Assert.That(() => command.Prepare(), Throws.Exception.TypeOf<NotSupportedException>());
        Assert.That(() => command.PrepareAsync(), Throws.Exception.TypeOf<NotSupportedException>());
    }

    [Test]
    public async Task Cannot_access_connection_transaction_on_data_source_batch()
    {
        await using var batch = SharedDataSource.CreateBatch();

        Assert.That(() => batch.Connection, Throws.Exception.TypeOf<NotSupportedException>());
        Assert.That(() => batch.Connection = null, Throws.Exception.TypeOf<NotSupportedException>());
        Assert.That(() => batch.Transaction, Throws.Exception.TypeOf<NotSupportedException>());
        Assert.That(() => batch.Transaction = null, Throws.Exception.TypeOf<NotSupportedException>());

        Assert.That(() => batch.Prepare(), Throws.Exception.TypeOf<NotSupportedException>());
        Assert.That(() => batch.PrepareAsync(), Throws.Exception.TypeOf<NotSupportedException>());
    }

    [Test]
    public async Task Cannot_get_connection_after_dispose_pooled([Values] bool async)
    {
        var dataSource = NpgsqlDataSource.Create(ConnectionString);

        if (async)
        {
            await dataSource.DisposeAsync();
            Assert.That(() => dataSource.OpenConnectionAsync(), Throws.Exception.TypeOf<ObjectDisposedException>());
        }
        else
        {
            dataSource.Dispose();
            Assert.That(() => dataSource.OpenConnection(), Throws.Exception.TypeOf<ObjectDisposedException>());
        }
    }

    [Test]
    public async Task Cannot_get_connection_after_dispose_unpooled([Values] bool async)
    {
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder(ConnectionString) { Pooling = false };
        var dataSource = NpgsqlDataSource.Create(connectionStringBuilder);

        if (async)
        {
            await dataSource.DisposeAsync();
            Assert.That(() => dataSource.OpenConnectionAsync(), Throws.Exception.TypeOf<ObjectDisposedException>());
        }
        else
        {
            dataSource.Dispose();
            Assert.That(() => dataSource.OpenConnection(), Throws.Exception.TypeOf<ObjectDisposedException>());
        }
    }

    [Test] // #4752
    public async Task As_DbDataSource([Values] bool async)
    {
        await using DbDataSource dataSource = NpgsqlDataSource.Create(ConnectionString);
        await using var connection = async
            ? await dataSource.OpenConnectionAsync()
            : dataSource.OpenConnection();
        Assert.That(connection.State, Is.EqualTo(ConnectionState.Open));

        await using var command = dataSource.CreateCommand("SELECT 1");

        Assert.That(async
            ? await command.ExecuteScalarAsync()
            : command.ExecuteScalar(), Is.EqualTo(1));
    }
}
