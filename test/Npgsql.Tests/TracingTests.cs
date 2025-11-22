using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NpgsqlTypes;
using NUnit.Framework;
using static Npgsql.Tests.TestUtil;

namespace Npgsql.Tests;

[NonParallelizable]
[TestFixture(MultiplexingMode.NonMultiplexing, true)]
[TestFixture(MultiplexingMode.NonMultiplexing, false)]
[TestFixture(MultiplexingMode.Multiplexing, true)]
[TestFixture(MultiplexingMode.Multiplexing, false)]
public class TracingTests : MultiplexingTestBase
{
    #region Physical open

    [Test]
    public async Task PhysicalOpen()
    {
        using var activityListener = StartListener(out var activities);
        await using var dataSource = CreateDataSource();
        await using var connection = Async
            ? await dataSource.OpenConnectionAsync()
            : dataSource.OpenConnection();

        Assert.That(activities, Has.Count.EqualTo(1));
        ValidateActivity(activities[0], connection, IsMultiplexing);

        if (!IsMultiplexing)
            return;

        activities.Clear();

        // For multiplexing, we clear the pool to force next query to open another physical connection
        dataSource.Clear();

        await connection.ExecuteScalarAsync("SELECT 1");

        Assert.That(activities, Has.Count.EqualTo(2));
        ValidateActivity(activities[0], connection, IsMultiplexing);

        // For multiplexing, query's activity can be considered as a parent for physical open's activity
        Assert.That(activities[0].Parent, Is.SameAs(activities[1]));

        static void ValidateActivity(Activity activity, NpgsqlConnection conn, bool isMultiplexing)
        {
            Assert.That(activity.DisplayName, Is.EqualTo("CONNECT " + conn.Settings.Database));
            Assert.That(activity.OperationName, Is.EqualTo("CONNECT " + conn.Settings.Database));
            Assert.That(activity.Status, Is.EqualTo(ActivityStatusCode.Unset));

            Assert.That(activity.Events.Count(), Is.EqualTo(0));

            var tags = activity.TagObjects.ToDictionary(t => t.Key, t => t.Value);
            Assert.That(tags, Has.Count.EqualTo(conn.Settings.Port == 5432 ? 5 : 6));

            Assert.That(tags["db.system.name"], Is.EqualTo("postgresql"));
            Assert.That(tags["db.namespace"], Is.EqualTo(conn.Settings.Database));

            Assert.That(tags, Does.Not.ContainKey("db.query.text"));

            Assert.That(tags["db.npgsql.data_source"], Is.EqualTo(conn.ConnectionString));

            if (isMultiplexing)
                Assert.That(tags, Does.ContainKey("db.npgsql.connection_id"));
            else
                Assert.That(tags["db.npgsql.connection_id"], Is.EqualTo(conn.ProcessID));
        }
    }

    [Test]
    public async Task PhysicalOpen_error()
    {
        using var activityListener = StartListener(out var activities);
        await using var dataSource = CreateDataSource(x => x.Host = "not-existing-host");
        var exception = Assert.ThrowsAsync<NpgsqlException>(async () =>
        {
            await using var connection = Async
                ? await dataSource.OpenConnectionAsync()
                : dataSource.OpenConnection();
        })!;

        var activity = GetSingleActivity(activities, "CONNECT " + dataSource.Settings.Database, "CONNECT " + dataSource.Settings.Database, ActivityStatusCode.Error, exception.Message);

        Assert.That(activity.Events.Count(), Is.EqualTo(1));
        var exceptionEvent = activity.Events.First();
        Assert.That(exceptionEvent.Name, Is.EqualTo("exception"));

        var exceptionTags = exceptionEvent.Tags.ToDictionary(t => t.Key, t => t.Value);
        Assert.That(exceptionTags, Has.Count.EqualTo(3));

        Assert.That(exceptionTags["exception.type"], Is.EqualTo(exception.GetType().FullName));
        Assert.That(exceptionTags["exception.message"], Does.Contain(exception.Message));
        Assert.That(exceptionTags["exception.stacktrace"], Does.Contain(exception.Message));

        var activityTags = activity.TagObjects.ToDictionary(t => t.Key, t => t.Value);
        Assert.That(activityTags, Has.Count.EqualTo(3));

        Assert.That(activityTags["db.system.name"], Is.EqualTo("postgresql"));
        Assert.That(activityTags["db.npgsql.data_source"], Is.EqualTo(dataSource.ConnectionString));

        Assert.That(activityTags["error.type"], Is.EqualTo("System.Net.Sockets.SocketException"));
    }

    [Test]
    public async Task PhysicalOpen_disable()
    {
        using var activityListener = StartListener(out var activities);
        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.ConfigureTracing(options => options.EnablePhysicalOpenTracing(enable: false));
        await using var dataSource = dataSourceBuilder.Build();

        await using var connection = Async ? await dataSource.OpenConnectionAsync() : dataSource.OpenConnection();

        Assert.That(activities, Is.Empty);
    }

    #endregion Physical open

    #region Command execution

    [Test]
    public async Task CommandExecute([Values] bool batch)
    {
        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.Name = "TestTracingDataSource";
        dataSourceBuilder.ConfigureTracing(o => o.EnablePhysicalOpenTracing(false));
        await using var dataSource = dataSourceBuilder.Build();
        await using var connection = await dataSource.OpenConnectionAsync();

        using var activityListener = StartListener(out var activities);

        await ExecuteScalar(connection, Async, batch, "SELECT 42");

        var activity = GetSingleActivity(activities, "postgresql", "postgresql");

        Assert.That(activity.Events.Count(), Is.EqualTo(1));
        var firstResponseEvent = activity.Events.First();
        Assert.That(firstResponseEvent.Name, Is.EqualTo("received-first-response"));

        var tags = activity.TagObjects.ToDictionary(t => t.Key, t => t.Value);
        Assert.That(tags, Has.Count.EqualTo(connection.Settings.Port == 5432 ? 6 : 7));

        Assert.That(tags["db.query.text"], Is.EqualTo("SELECT 42"));
        Assert.That(tags["db.system.name"], Is.EqualTo("postgresql"));
        Assert.That(tags["db.namespace"], Is.EqualTo(connection.Settings.Database));

        Assert.That(tags["db.npgsql.data_source"], Is.EqualTo("TestTracingDataSource"));

        if (IsMultiplexing)
            Assert.That(tags, Does.ContainKey("db.npgsql.connection_id"));
        else
            Assert.That(tags["db.npgsql.connection_id"], Is.EqualTo(connection.ProcessID));
    }

    [Test]
    public async Task CommandExecute_error([Values] bool batch)
    {
        await using var dataSource = CreateDataSource(ds => ds.ConfigureTracing(o => o.EnablePhysicalOpenTracing(false)));
        await using var connection = await dataSource.OpenConnectionAsync();

        using var activityListener = StartListener(out var activities);

        Assert.ThrowsAsync<PostgresException>(async () => await ExecuteScalar(connection, Async, batch, "SELECT * FROM non_existing_table"));

        var activity = GetSingleActivity(activities, "postgresql", "postgresql", ActivityStatusCode.Error, PostgresErrorCodes.UndefinedTable);

        Assert.That(activity.Events.Count(), Is.EqualTo(1));
        var exceptionEvent = activity.Events.First();
        Assert.That(exceptionEvent.Name, Is.EqualTo("exception"));

        var exceptionTags = exceptionEvent.Tags.ToDictionary(t => t.Key, t => t.Value);
        Assert.That(exceptionTags, Has.Count.EqualTo(3));

        Assert.That(exceptionTags["exception.type"], Is.EqualTo("Npgsql.PostgresException"));
        Assert.That(exceptionTags["exception.message"], Does.Contain("relation \"non_existing_table\" does not exist"));
        Assert.That(exceptionTags["exception.stacktrace"], Does.Contain("relation \"non_existing_table\" does not exist"));

        var activityTags = activity.TagObjects.ToDictionary(t => t.Key, t => t.Value);
        Assert.That(activityTags, Has.Count.EqualTo(connection.Settings.Port == 5432 ? 8 : 9));

        Assert.That(activityTags["db.query.text"], Is.EqualTo("SELECT * FROM non_existing_table"));
        Assert.That(activityTags["db.system.name"], Is.EqualTo("postgresql"));
        Assert.That(activityTags["db.namespace"], Is.EqualTo(connection.Settings.Database));

        Assert.That(activityTags["db.response.status_code"], Is.EqualTo(PostgresErrorCodes.UndefinedTable));
        Assert.That(activityTags["error.type"], Is.EqualTo(PostgresErrorCodes.UndefinedTable));

        Assert.That(activityTags["db.npgsql.data_source"], Is.EqualTo(connection.ConnectionString));

        if (IsMultiplexing)
            Assert.That(activityTags, Does.ContainKey("db.npgsql.connection_id"));
        else
            Assert.That(activityTags["db.npgsql.connection_id"], Is.EqualTo(connection.ProcessID));
    }

    [Test]
    public async Task CommandExecute_ConfigureTracing([Values] bool batch)
    {
        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.ConfigureTracing(options =>
        {
            options
                .EnablePhysicalOpenTracing(false)
                .EnableFirstResponseEvent(enable: false)
                .ConfigureCommandFilter(cmd => cmd.CommandText.Contains('2'))
                .ConfigureBatchFilter(batch => batch.BatchCommands[0].CommandText.Contains('2'))
                .ConfigureCommandSpanNameProvider(_ => "unknown_query")
                .ConfigureBatchSpanNameProvider(_ => "unknown_query")
                .ConfigureCommandEnrichmentCallback((activity, _) => activity.AddTag("custom_tag", "custom_value"))
                .ConfigureBatchEnrichmentCallback((activity, _) => activity.AddTag("custom_tag", "custom_value"));
        });
        await using var dataSource = dataSourceBuilder.Build();
        await using var connection = await dataSource.OpenConnectionAsync();

        using var activityListener = StartListener(out var activities);

        await ExecuteScalar(connection, Async, batch, "SELECT 1");

        Assert.That(activities, Is.Empty);

        await ExecuteScalar(connection, Async, batch, "SELECT 2");

        var activity = GetSingleActivity(activities, "unknown_query", "unknown_query");

        Assert.That(activity.Events.Count(), Is.EqualTo(0));

        var tags = activity.TagObjects.ToDictionary(t => t.Key, t => t.Value);
        Assert.That(tags["custom_tag"], Is.EqualTo("custom_value"));
    }

    #endregion Command execution

    #region Binary import

    [Test]
    public async Task BinaryImport()
    {
        await using var dataSource = CreateDataSource(ds => ds.ConfigureTracing(o => o.EnablePhysicalOpenTracing(false)));
        await using var connection = await dataSource.OpenConnectionAsync();

        var table = await CreateTempTable(connection, "field_text TEXT, field_int2 SMALLINT");

        using var activityListener = StartListener(out var activities);

        var copyFromCommand = $"COPY {table} (field_text, field_int2) FROM STDIN BINARY";

        if (Async)
        {
            await using var writer = await connection.BeginBinaryImportAsync(copyFromCommand);

            await writer.StartRowAsync();
            await writer.WriteAsync("Hello");
            await writer.WriteAsync((short)8, NpgsqlDbType.Smallint);

            await writer.CompleteAsync();
        }
        else
        {
            using var writer = connection.BeginBinaryImport(copyFromCommand);

            writer.StartRow();
            writer.Write("Hello");
            writer.Write((short)8, NpgsqlDbType.Smallint);

            writer.Complete();
        }

        var activity = GetSingleActivity(activities, "COPY FROM");

        var tags = activity.TagObjects.ToDictionary(t => t.Key, t => t.Value);
        Assert.That(tags, Has.Count.EqualTo(connection.Settings.Port == 5432 ? 8 : 9));

        Assert.That(tags["db.query.text"], Is.EqualTo(copyFromCommand));
        Assert.That(tags["db.operation.name"], Is.EqualTo("COPY FROM"));
        Assert.That(tags["db.system.name"], Is.EqualTo("postgresql"));
        Assert.That(tags["db.namespace"], Is.EqualTo(connection.Settings.Database));

        Assert.That(tags["db.npgsql.data_source"], Is.EqualTo(connection.ConnectionString));
        Assert.That(tags["db.npgsql.rows"], Is.EqualTo(1));

        if (IsMultiplexing)
            Assert.That(tags, Does.ContainKey("db.npgsql.connection_id"));
        else
            Assert.That(tags["db.npgsql.connection_id"], Is.EqualTo(connection.ProcessID));
    }

    [Test]
    public async Task BinaryImport_cancel()
    {
        await using var dataSource = CreateDataSource(ds => ds.ConfigureTracing(o => o.EnablePhysicalOpenTracing(false)));
        await using var connection = await dataSource.OpenConnectionAsync();

        var table = await CreateTempTable(connection, "field_text TEXT, field_int2 SMALLINT");

        using var activityListener = StartListener(out var activities);

        var copyFromCommand = $"COPY {table} (field_text, field_int2) FROM STDIN BINARY";

        if (Async)
        {
            await using var writer = await connection.BeginBinaryImportAsync(copyFromCommand);
            await writer.StartRowAsync();
            await writer.WriteAsync("Hello");
            await writer.WriteAsync((short)8, NpgsqlDbType.Smallint);
            // No Complete() call - disposing cancels
        }
        else
        {
            using var writer = connection.BeginBinaryImport(copyFromCommand);
            writer.StartRow();
            writer.Write("Hello");
            writer.Write((short)8, NpgsqlDbType.Smallint);
            // No Complete() call - disposing cancels
        }

        _ = GetSingleActivity(activities, "COPY FROM", "COPY FROM", ActivityStatusCode.Error, "Cancelled");
    }

    [Test]
    public async Task BinaryImport_error()
    {
        await using var dataSource = CreateDataSource(ds => ds.ConfigureTracing(o => o.EnablePhysicalOpenTracing(false)));
        await using var connection = await dataSource.OpenConnectionAsync();

        using var activityListener = StartListener(out var activities);

        var copyFromCommand = $"COPY non_existing_table (field_text, field_int2) FROM STDIN BINARY";

        var ex = Assert.ThrowsAsync<PostgresException>(async () =>
        {
            await using var writer = Async
                ? await connection.BeginBinaryImportAsync(copyFromCommand)
                : connection.BeginBinaryImport(copyFromCommand);
        });

        var activity = GetSingleActivity(activities, "COPY FROM", "COPY FROM", ActivityStatusCode.Error, PostgresErrorCodes.UndefinedTable);

        Assert.That(activity.Events.Count(), Is.EqualTo(1));
        var exceptionEvent = activity.Events.First();
        Assert.That(exceptionEvent.Name, Is.EqualTo("exception"));

        var exceptionTags = exceptionEvent.Tags.ToDictionary(t => t.Key, t => t.Value);
        Assert.That(exceptionTags, Has.Count.EqualTo(3));

        Assert.That(exceptionTags["exception.type"], Is.EqualTo("Npgsql.PostgresException"));
        Assert.That(exceptionTags["exception.message"], Does.Contain("relation \"non_existing_table\" does not exist"));
        Assert.That(exceptionTags["exception.stacktrace"], Does.Contain("relation \"non_existing_table\" does not exist"));
    }

    #endregion Binary import

    #region Binary export

    [Test]
    public async Task BinaryExport()
    {
        await using var dataSource = CreateDataSource(ds => ds.ConfigureTracing(o => o.EnablePhysicalOpenTracing(false)));
        await using var connection = await dataSource.OpenConnectionAsync();

        var table = await CreateTempTable(connection, "field_text TEXT, field_int2 SMALLINT");
        await connection.ExecuteNonQueryAsync($"INSERT INTO {table} (field_text, field_int2) VALUES ('Hello', 8)");

        using var activityListener = StartListener(out var activities);

        var copyToCommand = $"COPY {table} (field_text, field_int2) TO STDOUT BINARY";

        if (Async)
        {
            await using var reader = await connection.BeginBinaryExportAsync(copyToCommand);
            while (await reader.StartRowAsync() != -1)
            {
                _ = await reader.ReadAsync<string>();
                _ = await reader.ReadAsync<short>();
            }
        }
        else
        {
            using var reader = connection.BeginBinaryExport(copyToCommand);
            while (reader.StartRow() != -1)
            {
                _ = reader.Read<string>();
                _ = reader.Read<short>();
            }
        }

        var activity = GetSingleActivity(activities, "COPY TO");

        var tags = activity.TagObjects.ToDictionary(t => t.Key, t => t.Value);
        Assert.That(tags, Has.Count.EqualTo(connection.Settings.Port == 5432 ? 8 : 9));

        Assert.That(tags["db.query.text"], Is.EqualTo(copyToCommand));
        Assert.That(tags["db.operation.name"], Is.EqualTo("COPY TO"));
        Assert.That(tags["db.system.name"], Is.EqualTo("postgresql"));
        Assert.That(tags["db.namespace"], Is.EqualTo(connection.Settings.Database));

        Assert.That(tags["db.npgsql.data_source"], Is.EqualTo(connection.ConnectionString));
        Assert.That(tags["db.npgsql.rows"], Is.EqualTo(1));

        if (IsMultiplexing)
            Assert.That(tags, Does.ContainKey("db.npgsql.connection_id"));
        else
            Assert.That(tags["db.npgsql.connection_id"], Is.EqualTo(connection.ProcessID));
    }

    [Test]
    public async Task BinaryExport_cancel()
    {
        await using var dataSource = CreateDataSource(ds => ds.ConfigureTracing(o => o.EnablePhysicalOpenTracing(false)));
        await using var conn = await dataSource.OpenConnectionAsync();

        using var activityListener = StartListener(out var activities);

        // This must be large enough to cause Postgres to queue up CopyData messages.
        const string copyToCommand = "COPY (select md5(random()::text) as id from generate_series(1, 100000)) TO STDOUT BINARY";

        if (Async)
        {
            await using var exporter = await conn.BeginBinaryExportAsync(copyToCommand);
            await exporter.StartRowAsync();
            await exporter.ReadAsync<string>();
            await exporter.CancelAsync();
        }
        else
        {
            using var exporter = await conn.BeginBinaryExportAsync(copyToCommand);
            exporter.StartRow();
            exporter.Read<string>();
            exporter.Cancel();
        }

        _ = GetSingleActivity(activities, "COPY TO", "COPY TO", ActivityStatusCode.Error, "Cancelled");
    }

    [Test]
    public async Task BinaryExport_error()
    {
        await using var dataSource = CreateDataSource(ds => ds.ConfigureTracing(o => o.EnablePhysicalOpenTracing(false)));
        await using var connection = await dataSource.OpenConnectionAsync();

        using var activityListener = StartListener(out var activities);

        var copyToCommand = $"COPY non_existing_table (field_text, field_int2) TO STDOUT BINARY";
        var ex = Assert.ThrowsAsync<PostgresException>(async () =>
        {
            await using var reader = Async
                ? await connection.BeginBinaryExportAsync(copyToCommand)
                : connection.BeginBinaryExport(copyToCommand);
        });

        var activity = GetSingleActivity(activities, "COPY TO", "COPY TO", ActivityStatusCode.Error, PostgresErrorCodes.UndefinedTable);

        Assert.That(activity.Events.Count(), Is.EqualTo(1));
        var exceptionEvent = activity.Events.First();
        Assert.That(exceptionEvent.Name, Is.EqualTo("exception"));

        var exceptionTags = exceptionEvent.Tags.ToDictionary(t => t.Key, t => t.Value);
        Assert.That(exceptionTags, Has.Count.EqualTo(3));

        Assert.That(exceptionTags["exception.type"], Is.EqualTo("Npgsql.PostgresException"));
        Assert.That(exceptionTags["exception.message"], Does.Contain("relation \"non_existing_table\" does not exist"));
        Assert.That(exceptionTags["exception.stacktrace"], Does.Contain("relation \"non_existing_table\" does not exist"));
    }

    #endregion Binary export

    #region Raw binary

    [Test]
    public async Task RawBinaryExport()
    {
        await using var dataSource = CreateDataSource(ds => ds.ConfigureTracing(o => o.EnablePhysicalOpenTracing(false)));
        await using var connection = await dataSource.OpenConnectionAsync();

        var table = await CreateTempTable(connection, "field_text TEXT, field_int2 SMALLINT");
        await connection.ExecuteNonQueryAsync($"INSERT INTO {table} (field_text, field_int2) VALUES ('Hello', 8)");

        using var activityListener = StartListener(out var activities);

        // Raw binary export
        var copyToCommand = $"COPY {table} (field_text, field_int2) TO STDIN BINARY";
        var buffer = new byte[1024];
        if (Async)
        {
            await using var stream = await connection.BeginRawBinaryCopyAsync(copyToCommand);
            while (await stream.ReadAsync(buffer, 0, buffer.Length) > 0) { }
        }
        else
        {
            using var stream = connection.BeginRawBinaryCopy(copyToCommand);
            while (stream.Read(buffer, 0, buffer.Length) > 0) { }
        }

        var activity = GetSingleActivity(activities, "COPY");

        var tags = activity.TagObjects.ToDictionary(t => t.Key, t => t.Value);

        Assert.That(tags, Has.Count.EqualTo(connection.Settings.Port == 5432 ? 7 : 8));

        Assert.That(tags["db.query.text"], Is.EqualTo(copyToCommand));
        Assert.That(tags["db.operation.name"], Is.EqualTo("COPY TO"));
        Assert.That(tags["db.system.name"], Is.EqualTo("postgresql"));
        Assert.That(tags["db.namespace"], Is.EqualTo(connection.Settings.Database));

        Assert.That(tags["db.npgsql.data_source"], Is.EqualTo(connection.ConnectionString));

        if (IsMultiplexing)
            Assert.That(tags, Does.ContainKey("db.npgsql.connection_id"));
        else
            Assert.That(tags["db.npgsql.connection_id"], Is.EqualTo(connection.ProcessID));

        Assert.That(tags, Does.Not.ContainKey("db.npgsql.rows"));
    }

    [Test]
    public async Task RawBinaryExport_cancel()
    {
        await using var dataSource = CreateDataSource(ds => ds.ConfigureTracing(o => o.EnablePhysicalOpenTracing(false)));
        await using var connection = await dataSource.OpenConnectionAsync();

        var table = await CreateTempTable(connection, "field_text TEXT, field_int2 SMALLINT");
        await connection.ExecuteNonQueryAsync($"INSERT INTO {table} (field_text, field_int2) VALUES ('Hello', 8)");

        using var activityListener = StartListener(out var activities);

        var copyToCommand = $"COPY {table} (field_text, field_int2) TO STDIN BINARY";
        var buffer = new byte[1024];
        if (Async)
        {
            await using var stream = await connection.BeginRawBinaryCopyAsync(copyToCommand);
            var _ = await stream.ReadAsync(buffer, 0, buffer.Length);
            await stream.CancelAsync();
        }
        else
        {
            using var stream = connection.BeginRawBinaryCopy(copyToCommand);
            var _ = stream.Read(buffer, 0, buffer.Length);
            stream.Cancel();
        }

        _ = GetSingleActivity(activities, "COPY", "COPY", ActivityStatusCode.Error, "Cancelled");
    }

    [Test]
    public async Task RawBinaryImport_cancel()
    {
        await using var dataSource = CreateDataSource(ds => ds.ConfigureTracing(o => o.EnablePhysicalOpenTracing(false)));
        await using var connection = await dataSource.OpenConnectionAsync();

        var table = await CreateTempTable(connection, "field_text TEXT, field_int2 SMALLINT");

        using var activityListener = StartListener(out var activities);

        var copyToCommand = $"COPY {table} (field_text, field_int2) FROM STDIN BINARY";
        byte[] garbage = [1, 2, 3, 4];
        if (Async)
        {
            await using var stream = await connection.BeginRawBinaryCopyAsync(copyToCommand);
            await stream.WriteAsync(garbage);
            await stream.FlushAsync();
            await stream.CancelAsync();
        }
        else
        {
            using var stream = connection.BeginRawBinaryCopy(copyToCommand);
            stream.Write(garbage);
            stream.Flush();
            stream.Cancel();
        }

        _ = GetSingleActivity(activities, "COPY", "COPY", ActivityStatusCode.Error, "Cancelled");
    }

    [Test]
    public async Task RawBinaryImport_error()
    {
        await using var dataSource = CreateDataSource(ds => ds.ConfigureTracing(o => o.EnablePhysicalOpenTracing(false)));
        await using var connection = await dataSource.OpenConnectionAsync();

        using var activityListener = StartListener(out var activities);

        var copyFromCommand = $"COPY non_existing_table (field_text, field_int2) FROM STDIN BINARY";
        var ex = Assert.ThrowsAsync<PostgresException>(async () =>
        {
            await using var stream = Async
                ? await connection.BeginRawBinaryCopyAsync(copyFromCommand)
                : connection.BeginRawBinaryCopy(copyFromCommand);
        });

        var activity = GetSingleActivity(activities, "COPY", "COPY", ActivityStatusCode.Error, PostgresErrorCodes.UndefinedTable);

        Assert.That(activity.Events.Count(), Is.EqualTo(1));
        var exceptionEvent = activity.Events.First();
        Assert.That(exceptionEvent.Name, Is.EqualTo("exception"));

        var exceptionTags = exceptionEvent.Tags.ToDictionary(t => t.Key, t => t.Value);
        Assert.That(exceptionTags, Has.Count.EqualTo(3));

        Assert.That(exceptionTags["exception.type"], Is.EqualTo("Npgsql.PostgresException"));
        Assert.That(exceptionTags["exception.message"], Does.Contain("relation \"non_existing_table\" does not exist"));
        Assert.That(exceptionTags["exception.stacktrace"], Does.Contain("relation \"non_existing_table\" does not exist"));
    }

    #endregion Raw binary

    #region Text COPY

    [Test]
    public async Task TextImport()
    {
        await using var dataSource = CreateDataSource(ds => ds.ConfigureTracing(o => o.EnablePhysicalOpenTracing(false)));
        await using var connection = await dataSource.OpenConnectionAsync();

        var table = await CreateTempTable(connection, "field_text TEXT, field_int2 SMALLINT");

        using var activityListener = StartListener(out var activities);

        var copyFromCommand = $"COPY {table} (field_text, field_int2) FROM STDIN";

        if (Async)
        {
            await using var writer = await connection.BeginTextImportAsync(copyFromCommand);
            await writer.WriteAsync("Hello\t8\n");
        }
        else
        {
            using var writer = connection.BeginTextImport(copyFromCommand);
            writer.Write("Hello\t8\n");
        }

        var activity = GetSingleActivity(activities, "COPY FROM");

        var tags = activity.TagObjects.ToDictionary(t => t.Key, t => t.Value);

        Assert.That(tags, Has.Count.EqualTo(connection.Settings.Port == 5432 ? 7 : 8));

        Assert.That(tags["db.query.text"], Is.EqualTo(copyFromCommand));
        Assert.That(tags["db.operation.name"], Is.EqualTo("COPY FROM"));
        Assert.That(tags["db.system.name"], Is.EqualTo("postgresql"));
        Assert.That(tags["db.namespace"], Is.EqualTo(connection.Settings.Database));

        Assert.That(tags["db.npgsql.data_source"], Is.EqualTo(connection.ConnectionString));

        if (IsMultiplexing)
            Assert.That(tags, Does.ContainKey("db.npgsql.connection_id"));
        else
            Assert.That(tags["db.npgsql.connection_id"], Is.EqualTo(connection.ProcessID));

        Assert.That(tags, Does.Not.ContainKey("db.npgsql.rows"));
    }

    [Test]
    public async Task TextExport()
    {
        await using var dataSource = CreateDataSource();
        await using var connection = await dataSource.OpenConnectionAsync();

        var table = await CreateTempTable(connection, "field_text TEXT, field_int2 SMALLINT");

        var insertCmd = $"INSERT INTO {table} (field_text, field_int2) VALUES ('Hello', 8)";
        await connection.ExecuteNonQueryAsync(insertCmd);

        using var activityListener = StartListener(out var activities);

        var copyFromCommand = $"COPY {table} (field_text, field_int2) TO STDIN";

        var chars = new char[30];
        if (Async)
        {
            await using var reader = await connection.BeginTextExportAsync(copyFromCommand);
            _ = await reader.ReadAsync(chars);
        }
        else
        {
            using var reader = connection.BeginTextExport(copyFromCommand);
            _ = reader.Read(chars);
        }

        var activity = GetSingleActivity(activities, "COPY TO");

        var tags = activity.TagObjects.ToDictionary(t => t.Key, t => t.Value);

        Assert.That(tags, Has.Count.EqualTo(connection.Settings.Port == 5432 ? 7 : 8));

        Assert.That(tags["db.query.text"], Is.EqualTo(copyFromCommand));
        Assert.That(tags["db.operation.name"], Is.EqualTo("COPY TO"));
        Assert.That(tags["db.system.name"], Is.EqualTo("postgresql"));
        Assert.That(tags["db.namespace"], Is.EqualTo(connection.Settings.Database));

        Assert.That(tags["db.npgsql.data_source"], Is.EqualTo(connection.ConnectionString));

        if (IsMultiplexing)
            Assert.That(tags, Does.ContainKey("db.npgsql.connection_id"));
        else
            Assert.That(tags["db.npgsql.connection_id"], Is.EqualTo(connection.ProcessID));

        Assert.That(tags, Does.Not.ContainKey("db.npgsql.rows"));
    }

    // Text COPY is implemented over NpgsqlRawCopyStream internally, without any additional tracing-related logic.
    // So we do only basic direct coverage and depend on the general raw tests for the rest.

    #endregion Text COPY

    // All ConfigureTracing() aspects of COPY are implemented in a single code path for all COPY paths, so we test just one.

    [Test]
    public async Task Copy_ConfigureTracing()
    {
        await using var dataSource = CreateDataSource(builder => builder.ConfigureTracing(options =>
            options
                .EnablePhysicalOpenTracing(false)
                .ConfigureCopyOperationFilter(command => command.Contains("filter_in"))
                .ConfigureCopyOperationSpanNameProvider(_ => "custom_binary_import")
                .ConfigureCopyOperationEnrichmentCallback((activity, _) => activity.AddTag("custom_tag", "custom_value"))));

        await using var conn = await dataSource.OpenConnectionAsync();

        var table = await CreateTempTable(conn, "field_text TEXT, field_int_filter_in SMALLINT");
        var copyCommand = $"COPY {table} (field_text, field_int_filter_in) FROM STDIN BINARY";

        var filteredOutTable = await CreateTempTable(conn, "field_text TEXT, field_int_filter_out SMALLINT");
        var filteredOutCopyCommand = $"COPY {filteredOutTable} (field_text, field_int_filter_out) FROM STDIN BINARY";

        using var activityListener = StartListener(out var activities);


        if (Async)
        {
            await using (var writer = await conn.BeginBinaryImportAsync(copyCommand))
            {
                await writer.CompleteAsync();
            }

            await using (var writer = await conn.BeginBinaryImportAsync(filteredOutCopyCommand))
            {
                await writer.CompleteAsync();
            }
        }
        else
        {
            using (var writer = conn.BeginBinaryImport(copyCommand))
            {
                writer.Complete();
            }

            using (var writer = conn.BeginBinaryImport(filteredOutCopyCommand))
            {
                writer.Complete();
            }
        }

        // There should be just one activity since one of the two COPY commands is filtered out
        var activity = GetSingleActivity(activities, "custom_binary_import", "custom_binary_import");

        var tags = activity.TagObjects.ToDictionary(t => t.Key, t => t.Value);
        Assert.That(tags["custom_tag"], Is.EqualTo("custom_value"));
    }

    private bool Async { get; }

    public TracingTests(MultiplexingMode multiplexingMode, bool async) : base(multiplexingMode)
    {
        if (IsMultiplexing && !async)
            Assert.Ignore("Sync not supported in multiplexing mode");

        Async = async;
    }

    static ActivityListener StartListener(out List<Activity> activities)
    {
        var a = new List<Activity>();

        var activityListener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == "Npgsql",
            Sample = (ref _) => ActivitySamplingResult.AllDataAndRecorded,
            ActivityStopped = activity => a.Add(activity)
        };
        ActivitySource.AddActivityListener(activityListener);

        activities = a;
        return activityListener;
    }

    static Activity GetSingleActivity(
        List<Activity> activities,
        string? expectedDisplayName,
        string? expectedOperationName = null,
        ActivityStatusCode? expectedStatusCode = null,
        string? expectedStatusDescription = null)
    {
        Assert.That(activities, Has.Count.EqualTo(1));
        var activity = activities[0];
        Assert.That(activity.DisplayName, Is.EqualTo(expectedDisplayName));
        Assert.That(activity.OperationName, Is.EqualTo(expectedOperationName ?? expectedDisplayName));
        Assert.That(activity.Status, Is.EqualTo(expectedStatusCode ?? ActivityStatusCode.Unset));
        Assert.That(activity.StatusDescription, Is.EqualTo(expectedStatusDescription));

        return activity;
    }

    static async Task<object?> ExecuteScalar(NpgsqlConnection connection, bool async, bool isBatch, string query)
    {
        if (!isBatch)
        {
            if (async)
                return await connection.ExecuteScalarAsync(query);
            else
                return connection.ExecuteScalar(query);
        }
        else
        {
            await using var batch = connection.CreateBatch();
            var batchCommand = batch.CreateBatchCommand();
            batchCommand.CommandText = query;
            batch.BatchCommands.Add(batchCommand);

            if (async)
                return await batch.ExecuteScalarAsync();
            else
                return batch.ExecuteScalar();
        }
    }
}
