using System;
using System.Data;
using System.Data.Common;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using NUnit.Framework;
using static Npgsql.Tests.TestUtil;

// ReSharper disable MethodHasAsyncOverload

namespace Npgsql.Tests;

public class DataSourceTests : TestBase
{
    [Test]
    public new async Task CreateConnection()
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

        await using (var reader = async ? await command.ExecuteReaderAsync() : command.ExecuteReader())
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
    public void Clear()
    {
        using var dataSource = NpgsqlDataSource.Create(ConnectionString);
        var connection1 = dataSource.OpenConnection();
        var connection2 = dataSource.OpenConnection();
        connection1.Close();

        Assert.That(dataSource.Statistics, Is.EqualTo((Total: 2, Idle: 1, Busy: 1)));

        dataSource.Clear();

        Assert.That(dataSource.Statistics, Is.EqualTo((Total: 1, Idle: 0, Busy: 1)));

        var connection3 = dataSource.OpenConnection();
        Assert.That(dataSource.Statistics, Is.EqualTo((Total: 2, Idle: 0, Busy: 2)));

        connection2.Close();
        Assert.That(dataSource.Statistics, Is.EqualTo((Total: 1, Idle: 0, Busy: 1)));

        connection3.Close();
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
        await using var command = DataSource.CreateCommand();

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
        await using var batch = DataSource.CreateBatch();

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

    [Test]
    public async Task Executing_command_on_disposed_datasource([Values] bool multiplexing)
    {
        var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            Multiplexing = multiplexing
        };
        DbDataSource dataSource = NpgsqlDataSource.Create(csb.ConnectionString);
        await using (var _ = await dataSource.OpenConnectionAsync()) {}
        await dataSource.DisposeAsync();
        await using var command = dataSource.CreateCommand("SELECT 1");
        Assert.ThrowsAsync<ObjectDisposedException>(command.ExecuteNonQueryAsync);
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/4840")]
    public async Task Multiplexing_connectionless_command_open_connection()
    {
        var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            Multiplexing = true
        };
        await using var dataSource = NpgsqlDataSource.Create(csb.ConnectionString);

        await using var conn = await dataSource.OpenConnectionAsync();
        await using var _ = await conn.BeginTransactionAsync();

        await using var command = dataSource.CreateCommand();
        command.CommandText = "SELECT 1";

        await using var reader = await command.ExecuteReaderAsync();
        Assert.True(reader.Read());
        Assert.That(reader.GetInt32(0), Is.EqualTo(1));
    }

    [Test]
    public async Task Connection_string_builder_settings_are_frozen_on_Build()
    {
        var builder = CreateDataSourceBuilder();
        builder.ConnectionStringBuilder.ApplicationName = "foo";
        await using var dataSource = builder.Build();

        builder.ConnectionStringBuilder.ApplicationName = "bar";

        await using var command = dataSource.CreateCommand("SHOW application_name");
        Assert.That(await command.ExecuteScalarAsync(), Is.EqualTo("foo"));
    }

    class Test
    {
        public int Id { get; set; }
    }

    [Test]
    public async Task ConfigureJsonOptions_is_order_independent()
    {
        // Expect failure, no options
        {
            var builder = CreateDataSourceBuilder();
            builder.EnableDynamicJson();
            await using var dataSource = builder.Build();

            await using var command = dataSource.CreateCommand("SELECT '{\"id\": 1}'::json;");
            using var reader = await command.ExecuteReaderAsync();
            reader.Read();
            Assert.That(reader.GetFieldValue<Test>(0).Id, Is.EqualTo(default(int)));
        }

        // Expect success, ConfigureJsonOptions before EnableDynamicJson
        {
            var builder = CreateDataSourceBuilder();
            builder.ConfigureJsonOptions(new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            builder.EnableDynamicJson();
            await using var dataSource = builder.Build();

            await using var command = dataSource.CreateCommand("SELECT '{\"id\": 1}'::json;");
            using var reader = await command.ExecuteReaderAsync();
            reader.Read();
            Assert.That(reader.GetFieldValue<Test>(0).Id, Is.EqualTo(1));
        }

        // Expect success, EnableDynamicJson before ConfigureJsonOptions
        {
            var builder = CreateDataSourceBuilder();
            builder.EnableDynamicJson();
            builder.ConfigureJsonOptions(new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            await using var dataSource = builder.Build();

            await using var command = dataSource.CreateCommand("SELECT '{\"id\": 1}'::json;");
            using var reader = await command.ExecuteReaderAsync();
            reader.Read();
            Assert.That(reader.GetFieldValue<Test>(0).Id, Is.EqualTo(1));
        }
    }

    [Test]
    public async Task ReloadTypes([Values] bool async)
    {
        await using var adminConnection = await OpenConnectionAsync();
        var type = await GetTempTypeName(adminConnection);

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.MapEnum<Mood>(type);
        await using var dataSource = dataSourceBuilder.Build();

        await using var connection = await dataSource.OpenConnectionAsync();
        await connection.ExecuteNonQueryAsync($"CREATE TYPE {type} AS ENUM ('sad', 'ok', 'happy')");

        if (async)
            await dataSource.ReloadTypesAsync();
        else
            dataSource.ReloadTypes();

        Assert.ThrowsAsync<InvalidCastException>(async () => await connection.ExecuteScalarAsync($"SELECT 'happy'::{type}"));

        // Close connection and reopen to make sure it picks up the new type and mapping from the data source
        await connection.CloseAsync();
        await connection.OpenAsync();

        Assert.DoesNotThrowAsync(async () => await connection.ExecuteScalarAsync($"SELECT 'happy'::{type}"));
    }

    [Test]
    public async Task ReloadTypes_across_data_sources([Values] bool async)
    {
        await using var adminConnection = await OpenConnectionAsync();
        var type = await GetTempTypeName(adminConnection);

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.MapEnum<Mood>(type);
        await using var dataSource1 = dataSourceBuilder.Build();
        await using var connection1 = await dataSource1.OpenConnectionAsync();

        await using var dataSource2 = dataSourceBuilder.Build();
        await using var connection2 = await dataSource2.OpenConnectionAsync();

        await connection1.ExecuteNonQueryAsync($"CREATE TYPE {type} AS ENUM ('sad', 'ok', 'happy')");

        if (async)
            await dataSource1.ReloadTypesAsync();
        else
            dataSource1.ReloadTypes();

        Assert.ThrowsAsync<InvalidCastException>(async () => await connection1.ExecuteScalarAsync($"SELECT 'happy'::{type}"));
        Assert.ThrowsAsync<InvalidCastException>(async () => await connection2.ExecuteScalarAsync($"SELECT 'happy'::{type}"));

        // Close connection and reopen to check that the new type and mapping is not available in dataSource2
        await connection2.CloseAsync();
        await connection2.OpenAsync();

        Assert.ThrowsAsync<InvalidCastException>(async () => await connection2.ExecuteScalarAsync($"SELECT 'happy'::{type}"));

        await dataSource2.ReloadTypesAsync();

        // Close connection2 and reopen to make sure it picks up the new type and mapping from dataSource2
        await connection2.CloseAsync();
        await connection2.OpenAsync();

        Assert.DoesNotThrowAsync(async () => await connection2.ExecuteScalarAsync($"SELECT 'happy'::{type}"));
    }

    enum Mood { Sad, Ok, Happy }
}
