using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NpgsqlTypes;
using NUnit.Framework;
using static Npgsql.Tests.TestUtil;

namespace Npgsql.Tests;

[NonParallelizable]
public class TracingTests(MultiplexingMode multiplexingMode) : MultiplexingTestBase(multiplexingMode)
{
    [Test]
    public async Task Basic_open([Values] bool async)
    {
        if (IsMultiplexing && !async)
            return;

        var activities = new List<Activity>();

        using var activityListener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == "Npgsql",
            Sample = (ref _) => ActivitySamplingResult.AllDataAndRecorded,
            ActivityStopped = activity => activities.Add(activity)
        };
        ActivitySource.AddActivityListener(activityListener);

        await using var dataSource = CreateDataSource();
        await using var conn = async
                ? await dataSource.OpenConnectionAsync()
                : dataSource.OpenConnection();

        Assert.That(activities, Has.Count.EqualTo(1));
        ValidateActivity(activities[0], conn, IsMultiplexing);

        if (!IsMultiplexing)
            return;

        activities.Clear();

        // For multiplexing, we clear the pool to force next query to open another physical connection
        dataSource.Clear();

        await conn.ExecuteScalarAsync("SELECT 1");

        Assert.That(activities, Has.Count.EqualTo(2));
        ValidateActivity(activities[0], conn, IsMultiplexing);

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
    public async Task Basic_query([Values] bool async, [Values] bool batch)
    {
        if (IsMultiplexing && !async)
            return;

        var activities = new List<Activity>();

        using var activityListener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == "Npgsql",
            Sample = (ref _) => ActivitySamplingResult.AllDataAndRecorded,
            ActivityStopped = activity => activities.Add(activity)
        };
        ActivitySource.AddActivityListener(activityListener);

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.Name = "TestTracingDataSource";
        await using var dataSource = dataSourceBuilder.Build();
        await using var conn = await dataSource.OpenConnectionAsync();

        // We're not interested in physical open's activity
        Assert.That(activities, Has.Count.EqualTo(1));
        activities.Clear();

        await ExecuteScalar(conn, async, batch, "SELECT 42");

        Assert.That(activities, Has.Count.EqualTo(1));
        var activity = activities[0];
        Assert.That(activity.DisplayName, Is.EqualTo("postgresql"));
        Assert.That(activity.OperationName, Is.EqualTo("postgresql"));
        Assert.That(activity.Status, Is.EqualTo(ActivityStatusCode.Unset));

        Assert.That(activity.Events.Count(), Is.EqualTo(1));
        var firstResponseEvent = activity.Events.First();
        Assert.That(firstResponseEvent.Name, Is.EqualTo("received-first-response"));

        var tags = activity.TagObjects.ToDictionary(t => t.Key, t => t.Value);
        Assert.That(tags, Has.Count.EqualTo(conn.Settings.Port == 5432 ? 6 : 7));

        Assert.That(tags["db.query.text"], Is.EqualTo("SELECT 42"));
        Assert.That(tags["db.system.name"], Is.EqualTo("postgresql"));
        Assert.That(tags["db.namespace"], Is.EqualTo(conn.Settings.Database));

        Assert.That(tags["db.npgsql.data_source"], Is.EqualTo("TestTracingDataSource"));

        if (IsMultiplexing)
            Assert.That(tags, Does.ContainKey("db.npgsql.connection_id"));
        else
            Assert.That(tags["db.npgsql.connection_id"], Is.EqualTo(conn.ProcessID));
    }

    [Test]
    public async Task Error_open([Values] bool async)
    {
        if (IsMultiplexing && !async)
            return;

        var activities = new List<Activity>();

        using var activityListener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == "Npgsql",
            Sample = (ref _) => ActivitySamplingResult.AllDataAndRecorded,
            ActivityStopped = activity => activities.Add(activity)
        };
        ActivitySource.AddActivityListener(activityListener);

        await using var dataSource = CreateDataSource(x => x.Host = "not-existing-host");
        var ex = Assert.ThrowsAsync<NpgsqlException>(async () =>
        {
            await using var conn = async
                ? await dataSource.OpenConnectionAsync()
                : dataSource.OpenConnection();
        })!;

        Assert.That(activities, Has.Count.EqualTo(1));
        var activity = activities[0];
        Assert.That(activity.DisplayName, Is.EqualTo("CONNECT " + dataSource.Settings.Database));
        Assert.That(activity.OperationName, Is.EqualTo("CONNECT " + dataSource.Settings.Database));
        Assert.That(activity.Status, Is.EqualTo(ActivityStatusCode.Error));
        Assert.That(activity.StatusDescription, Is.EqualTo(ex.Message));

        Assert.That(activity.Events.Count(), Is.EqualTo(1));
        var exceptionEvent = activity.Events.First();
        Assert.That(exceptionEvent.Name, Is.EqualTo("exception"));

        var exceptionTags = exceptionEvent.Tags.ToDictionary(t => t.Key, t => t.Value);
        Assert.That(exceptionTags, Has.Count.EqualTo(3));

        Assert.That(exceptionTags["exception.type"], Is.EqualTo(ex.GetType().FullName));
        Assert.That(exceptionTags["exception.message"], Does.Contain(ex.Message));
        Assert.That(exceptionTags["exception.stacktrace"], Does.Contain(ex.Message));

        var activityTags = activity.TagObjects.ToDictionary(t => t.Key, t => t.Value);
        Assert.That(activityTags, Has.Count.EqualTo(3));

        Assert.That(activityTags["db.system.name"], Is.EqualTo("postgresql"));
        Assert.That(activityTags["db.npgsql.data_source"], Is.EqualTo(dataSource.ConnectionString));

        Assert.That(activityTags["error.type"], Is.EqualTo("System.Net.Sockets.SocketException"));
    }

    [Test]
    public async Task Error_query([Values] bool async, [Values] bool batch)
    {
        if (IsMultiplexing && !async)
            return;

        var activities = new List<Activity>();

        using var activityListener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == "Npgsql",
            Sample = (ref _) => ActivitySamplingResult.AllDataAndRecorded,
            ActivityStopped = activity => activities.Add(activity)
        };
        ActivitySource.AddActivityListener(activityListener);

        await using var dataSource = CreateDataSource(DisablePhysicalOpenTracing);
        await using var conn = await dataSource.OpenConnectionAsync();

        Assert.ThrowsAsync<PostgresException>(async () => await ExecuteScalar(conn, async, batch, "SELECT * FROM non_existing_table"));

        Assert.That(activities, Has.Count.EqualTo(1));
        var activity = activities[0];
        Assert.That(activity.DisplayName, Is.EqualTo("postgresql"));
        Assert.That(activity.OperationName, Is.EqualTo("postgresql"));
        Assert.That(activity.Status, Is.EqualTo(ActivityStatusCode.Error));
        Assert.That(activity.StatusDescription, Is.EqualTo(PostgresErrorCodes.UndefinedTable));

        Assert.That(activity.Events.Count(), Is.EqualTo(1));
        var exceptionEvent = activity.Events.First();
        Assert.That(exceptionEvent.Name, Is.EqualTo("exception"));

        var exceptionTags = exceptionEvent.Tags.ToDictionary(t => t.Key, t => t.Value);
        Assert.That(exceptionTags, Has.Count.EqualTo(3));

        Assert.That(exceptionTags["exception.type"], Is.EqualTo("Npgsql.PostgresException"));
        Assert.That(exceptionTags["exception.message"], Does.Contain("relation \"non_existing_table\" does not exist"));
        Assert.That(exceptionTags["exception.stacktrace"], Does.Contain("relation \"non_existing_table\" does not exist"));

        var activityTags = activity.TagObjects.ToDictionary(t => t.Key, t => t.Value);
        Assert.That(activityTags, Has.Count.EqualTo(conn.Settings.Port == 5432 ? 8 : 9));

        Assert.That(activityTags["db.query.text"], Is.EqualTo("SELECT * FROM non_existing_table"));
        Assert.That(activityTags["db.system.name"], Is.EqualTo("postgresql"));
        Assert.That(activityTags["db.namespace"], Is.EqualTo(conn.Settings.Database));

        Assert.That(activityTags["db.response.status_code"], Is.EqualTo(PostgresErrorCodes.UndefinedTable));
        Assert.That(activityTags["error.type"], Is.EqualTo(PostgresErrorCodes.UndefinedTable));

        Assert.That(activityTags["db.npgsql.data_source"], Is.EqualTo(conn.ConnectionString));

        if (IsMultiplexing)
            Assert.That(activityTags, Does.ContainKey("db.npgsql.connection_id"));
        else
            Assert.That(activityTags["db.npgsql.connection_id"], Is.EqualTo(conn.ProcessID));
    }

    [Test]
    public async Task Configure_tracing([Values] bool async, [Values] bool batch)
    {
        if (IsMultiplexing && !async)
            return;

        var activities = new List<Activity>();

        using var activityListener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == "Npgsql",
            Sample = (ref _) => ActivitySamplingResult.AllDataAndRecorded,
            ActivityStopped = activity => activities.Add(activity)
        };
        ActivitySource.AddActivityListener(activityListener);

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.ConfigureTracing(options =>
        {
            options
                .EnablePhysicalOpenTracing(enable: false)
                .EnableFirstResponseEvent(enable: false)
                .ConfigureCommandFilter(cmd => cmd.CommandText.Contains('2'))
                .ConfigureBatchFilter(batch => batch.BatchCommands[0].CommandText.Contains('2'))
                .ConfigureCommandSpanNameProvider(_ => "unknown_query")
                .ConfigureBatchSpanNameProvider(_ => "unknown_query")
                .ConfigureCommandEnrichmentCallback((activity, _) => activity.AddTag("custom_tag", "custom_value"))
                .ConfigureBatchEnrichmentCallback((activity, _) => activity.AddTag("custom_tag", "custom_value"));
        });
        await using var dataSource = dataSourceBuilder.Build();
        await using var conn = await dataSource.OpenConnectionAsync();

        // We disabled physical open tracing
        Assert.That(activities.Count, Is.EqualTo(0));

        await ExecuteScalar(conn, async, batch, "SELECT 1");

        Assert.That(activities, Is.Empty);

        await ExecuteScalar(conn, async, batch, "SELECT 2");

        Assert.That(activities, Has.Count.EqualTo(1));
        var activity = activities[0];
        Assert.That(activity.DisplayName, Is.EqualTo("unknown_query"));
        Assert.That(activity.OperationName, Is.EqualTo("unknown_query"));

        Assert.That(activity.Events.Count(), Is.EqualTo(0));

        var tags = activity.TagObjects.ToDictionary(t => t.Key, t => t.Value);
        Assert.That(tags["custom_tag"], Is.EqualTo("custom_value"));
    }

    [Test]
    public async Task Basic_binary_import([Values] bool async)
    {
        if (IsMultiplexing && !async)
            return;

        var activities = new List<Activity>();

        using var activityListener = new ActivityListener();
        activityListener.ShouldListenTo = source => source.Name == "Npgsql";
        activityListener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded;
        activityListener.ActivityStopped = activity => activities.Add(activity);
        ActivitySource.AddActivityListener(activityListener);

        await using var dataSource = CreateDataSource(DisablePhysicalOpenTracing);
        await using var conn = await dataSource.OpenConnectionAsync();

        var table = await CreateTempTable(conn, "field_text TEXT, field_int2 SMALLINT");

        // We're not interested in temp table creation's activity
        Assert.That(activities.Count, Is.EqualTo(1));
        activities.Clear();

        var longString = new StringBuilder(conn.Settings.WriteBufferSize + 50).Append('a').ToString();

        var copyFromCommand = $"COPY {table} (field_text, field_int2) FROM STDIN BINARY";

        await using (var writer = async
            ? await conn.BeginBinaryImportAsync(copyFromCommand)
            : conn.BeginBinaryImport(copyFromCommand))
        {
            ulong rowsWritten;
            if (async)
            {
                await writer.StartRowAsync();
                await writer.WriteAsync("Hello");
                await writer.WriteAsync((short)8, NpgsqlDbType.Smallint);

                await writer.WriteRowAsync(default, "Something", (short)9);

                await writer.StartRowAsync();
                await writer.WriteAsync(longString, "text");
                await writer.WriteNullAsync();

                rowsWritten = await writer.CompleteAsync();
            }
            else
            {
                writer.StartRow();
                writer.Write("Hello");
                writer.Write((short)8, NpgsqlDbType.Smallint);

                writer.WriteRow("Something", (short)9);

                writer.StartRow();
                writer.Write(longString, "text");
                writer.WriteNull();

                rowsWritten = writer.Complete();
            }

            Assert.That(rowsWritten, Is.EqualTo(3));
        }

        Assert.That(activities.Count, Is.EqualTo(1));
        var activity = activities[0];
        Assert.That(activity.DisplayName, Is.EqualTo(conn.Settings.Database));
        Assert.That(activity.OperationName, Is.EqualTo(conn.Settings.Database));
        Assert.That(activity.Status, Is.EqualTo(ActivityStatusCode.Ok));

        var expectedTagCount = conn.Settings.Port == 5432 ? 11 : 12;
        Assert.That(activity.TagObjects.Count(), Is.EqualTo(expectedTagCount));

        var queryTag = activity.TagObjects.First(x => x.Key == "db.statement");
        Assert.That(queryTag.Value, Is.EqualTo(copyFromCommand));

        var operationTag = activity.TagObjects.First(x => x.Key == "db.operation");
        Assert.That(operationTag.Value, Is.EqualTo("COPY FROM"));

        var rowsTag = activity.TagObjects.First(x => x.Key == "db.rows");
        Assert.That(rowsTag.Value, Is.EqualTo(3));

        var systemTag = activity.TagObjects.First(x => x.Key == "db.system");
        Assert.That(systemTag.Value, Is.EqualTo("postgresql"));

        var userTag = activity.TagObjects.First(x => x.Key == "db.user");
        Assert.That(userTag.Value, Is.EqualTo(conn.Settings.Username));

        var dbNameTag = activity.TagObjects.First(x => x.Key == "db.name");
        Assert.That(dbNameTag.Value, Is.EqualTo(conn.Settings.Database));

        var connStringTag = activity.TagObjects.First(x => x.Key == "db.connection_string");
        Assert.That(connStringTag.Value, Is.EqualTo(conn.ConnectionString));

        if (!IsMultiplexing)
        {
            var connIDTag = activity.TagObjects.First(x => x.Key == "db.connection_id");
            Assert.That(connIDTag.Value, Is.EqualTo(conn.ProcessID));
        }
        else
            Assert.That(activity.TagObjects.Any(x => x.Key == "db.connection_id"));
    }

    [Test]
    public async Task Configure_tracing_binary_import([Values] bool async)
    {
        if (IsMultiplexing && !async)
            return;

        var activities = new List<Activity>();

        using var activityListener = new ActivityListener();
        activityListener.ShouldListenTo = source => source.Name == "Npgsql";
        activityListener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded;
        activityListener.ActivityStopped = activity => activities.Add(activity);
        ActivitySource.AddActivityListener(activityListener);

        await using var dataSource = CreateDataSource(builder =>
        {
            DisablePhysicalOpenTracing(builder);
            builder.ConfigureTracing(options =>
            {
                options
                    .ConfigureCopyOperationFilter((command, type) => type == CopyOperationType.BinaryImport && command.Contains("int2"))
                    .ConfigureCopyOperationSpanNameProvider((_, type) => type == CopyOperationType.BinaryImport ? "custom_binary_import" : null)
                    .ConfigureCopyOperationEnrichmentCallback((activity, _, type) =>
                    {
                        if (type == CopyOperationType.BinaryImport) {
                            activity.AddTag("custom_tag", "custom_value");
                        }
                    });
            });
        });

        await using var conn = await dataSource.OpenConnectionAsync();

        // Filtered out
        {
            var table = await CreateTempTable(conn, "field_text TEXT, field_int SMALLINT");

            // We're not interested in temp table creation's activity
            Assert.That(activities.Count, Is.EqualTo(1));
            activities.Clear();

            var copyFromCommand = $"COPY {table} (field_text, field_int) FROM STDIN BINARY";

            await using (var writer = async
                ? await conn.BeginBinaryImportAsync(copyFromCommand)
                : conn.BeginBinaryImport(copyFromCommand))
            { }

            Assert.That(activities.Count, Is.EqualTo(0));
        }

        // Filtered in
        {
            var table = await CreateTempTable(conn, "field_text TEXT, field_int2 SMALLINT");

            // We're not interested in temp table creation's activity
            Assert.That(activities.Count, Is.EqualTo(1));
            activities.Clear();

            var copyFromCommand = $"COPY {table} (field_text, field_int2) FROM STDIN BINARY";

            await using (var writer = async
                ? await conn.BeginBinaryImportAsync(copyFromCommand)
                : conn.BeginBinaryImport(copyFromCommand))
            { }

            Assert.That(activities.Count, Is.EqualTo(1));
            var activity = activities[0];
            Assert.That(activity.DisplayName, Is.EqualTo("custom_binary_import"));
            Assert.That(activity.OperationName, Is.EqualTo("custom_binary_import"));

            Assert.That(activity.Events.Count(), Is.EqualTo(0));

            var customTag = activity.TagObjects.First(x => x.Key == "custom_tag");
            Assert.That(customTag.Value, Is.EqualTo("custom_value"));
        }
    }

    [Test]
    public async Task Cancel_binary_import([Values] bool async)
    {
        if (IsMultiplexing && !async)
            return;

        var activities = new List<Activity>();

        using var activityListener = new ActivityListener();
        activityListener.ShouldListenTo = source => source.Name == "Npgsql";
        activityListener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded;
        activityListener.ActivityStopped = activity => activities.Add(activity);
        ActivitySource.AddActivityListener(activityListener);

        await using var dataSource = CreateDataSource(DisablePhysicalOpenTracing);
        await using var conn = await dataSource.OpenConnectionAsync();

        var table = await CreateTempTable(conn, "field_text TEXT, field_int2 SMALLINT");

        // We're not interested in temp table creation's activity
        Assert.That(activities.Count, Is.EqualTo(1));
        activities.Clear();

        var longString = new StringBuilder(conn.Settings.WriteBufferSize + 50).Append('a').ToString();

        var copyFromCommand = $"COPY {table} (field_text, field_int2) FROM STDIN BINARY";

        await using (var writer = async
            ? await conn.BeginBinaryImportAsync(copyFromCommand)
            : conn.BeginBinaryImport(copyFromCommand))
        {
            if (async)
            {
                await writer.StartRowAsync();
                await writer.WriteAsync("Hello");
                await writer.WriteAsync((short)8, NpgsqlDbType.Smallint);

                await ((ICancelable)writer).CancelAsync();
            }
            else
            {
                writer.StartRow();
                writer.Write("Hello");
                writer.Write((short)8, NpgsqlDbType.Smallint);

                ((ICancelable)writer).Cancel();
            }
        }

        Assert.That(activities.Count, Is.EqualTo(1));
        var activity = activities[0];
        Assert.That(activity.DisplayName, Is.EqualTo(conn.Settings.Database));
        Assert.That(activity.OperationName, Is.EqualTo(conn.Settings.Database));
        Assert.That(activity.Status, Is.EqualTo(ActivityStatusCode.Error));
        Assert.That(activity.StatusDescription, Is.EqualTo("Cancelled"));

        var expectedTagCount = conn.Settings.Port == 5432 ? 10 : 11;
        Assert.That(activity.TagObjects.Count(), Is.EqualTo(expectedTagCount));

        var queryTag = activity.TagObjects.First(x => x.Key == "db.statement");
        Assert.That(queryTag.Value, Is.EqualTo(copyFromCommand));

        var systemTag = activity.TagObjects.First(x => x.Key == "db.system");
        Assert.That(systemTag.Value, Is.EqualTo("postgresql"));

        var userTag = activity.TagObjects.First(x => x.Key == "db.user");
        Assert.That(userTag.Value, Is.EqualTo(conn.Settings.Username));

        var dbNameTag = activity.TagObjects.First(x => x.Key == "db.name");
        Assert.That(dbNameTag.Value, Is.EqualTo(conn.Settings.Database));

        var connStringTag = activity.TagObjects.First(x => x.Key == "db.connection_string");
        Assert.That(connStringTag.Value, Is.EqualTo(conn.ConnectionString));

        if (!IsMultiplexing)
        {
            var connIDTag = activity.TagObjects.First(x => x.Key == "db.connection_id");
            Assert.That(connIDTag.Value, Is.EqualTo(conn.ProcessID));
        }
        else
            Assert.That(activity.TagObjects.Any(x => x.Key == "db.connection_id"));
    }

    [Test]
    public async Task Error_binary_import([Values] bool async)
    {
        if (IsMultiplexing && !async)
            return;

        var activities = new List<Activity>();

        using var activityListener = new ActivityListener();
        activityListener.ShouldListenTo = source => source.Name == "Npgsql";
        activityListener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded;
        activityListener.ActivityStopped = activity => activities.Add(activity);
        ActivitySource.AddActivityListener(activityListener);

        await using var dataSource = CreateDataSource(DisablePhysicalOpenTracing);
        await using var conn = await dataSource.OpenConnectionAsync();

        var copyFromCommand = $"COPY non_existing_table (field_text, field_int2) FROM STDIN BINARY";

        var ex = Assert.ThrowsAsync<PostgresException>(async () =>
        {
            await using var writer = async
                ? await conn.BeginBinaryImportAsync(copyFromCommand)
                : conn.BeginBinaryImport(copyFromCommand);
        });

        Assert.That(activities.Count, Is.EqualTo(1));
        var activity = activities[0];
        Assert.That(activity.DisplayName, Is.EqualTo(conn.Settings.Database));
        Assert.That(activity.OperationName, Is.EqualTo(conn.Settings.Database));
        Assert.That(activity.Status, Is.EqualTo(ActivityStatusCode.Error));
        Assert.That(activity.StatusDescription, Is.EqualTo(PostgresErrorCodes.UndefinedTable));

        Assert.That(activity.Events.Count(), Is.EqualTo(1));
        var exceptionEvent = activity.Events.First();
        Assert.That(exceptionEvent.Name, Is.EqualTo("exception"));

        Assert.That(exceptionEvent.Tags.Count(), Is.EqualTo(4));

        var exceptionTypeTag = exceptionEvent.Tags.First(x => x.Key == "exception.type");
        Assert.That(exceptionTypeTag.Value, Is.EqualTo(ex.GetType().FullName));

        var exceptionMessageTag = exceptionEvent.Tags.First(x => x.Key == "exception.message");
        Assert.That((string)exceptionMessageTag.Value!, Does.Contain(ex.Message));

        var exceptionStacktraceTag = exceptionEvent.Tags.First(x => x.Key == "exception.stacktrace");
        Assert.That((string)exceptionStacktraceTag.Value!, Does.Contain(ex.Message));

        var exceptionEscapedTag = exceptionEvent.Tags.First(x => x.Key == "exception.escaped");
        Assert.That(exceptionEscapedTag.Value, Is.True);

        var expectedTagCount = conn.Settings.Port == 5432 ? 10 : 11;
        Assert.That(activity.TagObjects.Count(), Is.EqualTo(expectedTagCount));

        var queryTag = activity.TagObjects.First(x => x.Key == "db.statement");
        Assert.That(queryTag.Value, Is.EqualTo(copyFromCommand));

        var systemTag = activity.TagObjects.First(x => x.Key == "db.system");
        Assert.That(systemTag.Value, Is.EqualTo("postgresql"));

        var userTag = activity.TagObjects.First(x => x.Key == "db.user");
        Assert.That(userTag.Value, Is.EqualTo(conn.Settings.Username));

        var dbNameTag = activity.TagObjects.First(x => x.Key == "db.name");
        Assert.That(dbNameTag.Value, Is.EqualTo(conn.Settings.Database));

        var connStringTag = activity.TagObjects.First(x => x.Key == "db.connection_string");
        Assert.That(connStringTag.Value, Is.EqualTo(conn.ConnectionString));

        if (!IsMultiplexing)
        {
            var connIDTag = activity.TagObjects.First(x => x.Key == "db.connection_id");
            Assert.That(connIDTag.Value, Is.EqualTo(conn.ProcessID));
        }
        else
            Assert.That(activity.TagObjects.Any(x => x.Key == "db.connection_id"));
    }

    [Test]
    public async Task Error_binary_import_during_import([Values] bool async)
    {
        if (IsMultiplexing && !async)
            return;

        var activities = new List<Activity>();

        using var activityListener = new ActivityListener();
        activityListener.ShouldListenTo = source => source.Name == "Npgsql";
        activityListener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded;
        activityListener.ActivityStopped = activity => activities.Add(activity);
        ActivitySource.AddActivityListener(activityListener);

        await using var dataSource = CreateDataSource(DisablePhysicalOpenTracing);
        await using var conn = await dataSource.OpenConnectionAsync();;

        var table = await CreateTempTable(conn, "foo INT UNIQUE");

        // We're not interested in temp table creation's activity
        Assert.That(activities.Count, Is.EqualTo(1));
        activities.Clear();

        var copyFromCommand = $"COPY {table} (foo) FROM STDIN BINARY";

        var ex = Assert.ThrowsAsync<PostgresException>(async () =>
        {
            await using var writer = async
                ? await conn.BeginBinaryImportAsync(copyFromCommand)
                : conn.BeginBinaryImport(copyFromCommand);

            if (async)
            {
                await writer.StartRowAsync();
                await writer.WriteAsync(8);
                await writer.StartRowAsync();
                await writer.WriteAsync(8);
                await writer.CompleteAsync();
            }
            else
            {
                writer.StartRow();
                writer.Write(8);
                writer.StartRow();
                writer.Write(8);
                writer.Complete();
            }
            Assert.Fail("Commit should have thrown");
        });

        Assert.That(activities.Count, Is.EqualTo(1));
        var activity = activities[0];
        Assert.That(activity.DisplayName, Is.EqualTo(conn.Settings.Database));
        Assert.That(activity.OperationName, Is.EqualTo(conn.Settings.Database));
        Assert.That(activity.Status, Is.EqualTo(ActivityStatusCode.Error));
        Assert.That(activity.StatusDescription, Is.EqualTo(PostgresErrorCodes.UniqueViolation));

        Assert.That(activity.Events.Count(), Is.EqualTo(1));
        var exceptionEvent = activity.Events.First();
        Assert.That(exceptionEvent.Name, Is.EqualTo("exception"));

        Assert.That(exceptionEvent.Tags.Count(), Is.EqualTo(4));

        var exceptionTypeTag = exceptionEvent.Tags.First(x => x.Key == "exception.type");
        Assert.That(exceptionTypeTag.Value, Is.EqualTo(ex.GetType().FullName));

        var exceptionMessageTag = exceptionEvent.Tags.First(x => x.Key == "exception.message");
        Assert.That((string)exceptionMessageTag.Value!, Does.Contain(ex.Message));

        var exceptionStacktraceTag = exceptionEvent.Tags.First(x => x.Key == "exception.stacktrace");
        Assert.That((string)exceptionStacktraceTag.Value!, Does.Contain(ex.Message));

        var exceptionEscapedTag = exceptionEvent.Tags.First(x => x.Key == "exception.escaped");
        Assert.That(exceptionEscapedTag.Value, Is.True);

        var expectedTagCount = conn.Settings.Port == 5432 ? 10 : 11;
        Assert.That(activity.TagObjects.Count(), Is.EqualTo(expectedTagCount));

        var queryTag = activity.TagObjects.First(x => x.Key == "db.statement");
        Assert.That(queryTag.Value, Is.EqualTo(copyFromCommand));

        var systemTag = activity.TagObjects.First(x => x.Key == "db.system");
        Assert.That(systemTag.Value, Is.EqualTo("postgresql"));

        var userTag = activity.TagObjects.First(x => x.Key == "db.user");
        Assert.That(userTag.Value, Is.EqualTo(conn.Settings.Username));

        var dbNameTag = activity.TagObjects.First(x => x.Key == "db.name");
        Assert.That(dbNameTag.Value, Is.EqualTo(conn.Settings.Database));

        var connStringTag = activity.TagObjects.First(x => x.Key == "db.connection_string");
        Assert.That(connStringTag.Value, Is.EqualTo(conn.ConnectionString));

        if (!IsMultiplexing)
        {
            var connIDTag = activity.TagObjects.First(x => x.Key == "db.connection_id");
            Assert.That(connIDTag.Value, Is.EqualTo(conn.ProcessID));
        }
        else
            Assert.That(activity.TagObjects.Any(x => x.Key == "db.connection_id"));
    }

    [Test]
    public async Task Error_binary_import_commit_in_middle_of_row([Values] bool async)
    {
        if (IsMultiplexing && !async)
            return;

        var activities = new List<Activity>();

        using var activityListener = new ActivityListener();
        activityListener.ShouldListenTo = source => source.Name == "Npgsql";
        activityListener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded;
        activityListener.ActivityStopped = activity => activities.Add(activity);
        ActivitySource.AddActivityListener(activityListener);

        await using var dataSource = CreateDataSource(DisablePhysicalOpenTracing);
        await using var conn = await dataSource.OpenConnectionAsync();

        var table = await CreateTempTable(conn, "field_text TEXT, field_int2 SMALLINT");

        // We're not interested in temp table creation's activity
        Assert.That(activities.Count, Is.EqualTo(1));
        activities.Clear();

        var copyFromCommand = $"COPY {table} (field_text, field_int2) FROM STDIN BINARY";

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await using var writer = async
                ? await conn.BeginBinaryImportAsync(copyFromCommand)
                : conn.BeginBinaryImport(copyFromCommand);

            if (async)
            {
                await writer.StartRowAsync();
                await writer.WriteAsync(8);
                await writer.WriteAsync("hello");
                await writer.StartRowAsync();
                await writer.WriteAsync(9);
                await writer.CompleteAsync();
            }
            else
            {
                writer.StartRow();
                writer.Write(8);
                writer.Write("hello");
                writer.StartRow();
                writer.Write(9);
                writer.Complete();
            }
            Assert.Fail("Commit should have thrown");
        });

        Assert.That(activities.Count, Is.EqualTo(1));
        var activity = activities[0];
        Assert.That(activity.DisplayName, Is.EqualTo(conn.Settings.Database));
        Assert.That(activity.OperationName, Is.EqualTo(conn.Settings.Database));
        Assert.That(activity.Status, Is.EqualTo(ActivityStatusCode.Error));
        Assert.That(activity.StatusDescription, Is.EqualTo("Cancelled"));

        var expectedTagCount = conn.Settings.Port == 5432 ? 10 : 11;
        Assert.That(activity.TagObjects.Count(), Is.EqualTo(expectedTagCount));

        var queryTag = activity.TagObjects.First(x => x.Key == "db.statement");
        Assert.That(queryTag.Value, Is.EqualTo(copyFromCommand));

        var systemTag = activity.TagObjects.First(x => x.Key == "db.system");
        Assert.That(systemTag.Value, Is.EqualTo("postgresql"));

        var userTag = activity.TagObjects.First(x => x.Key == "db.user");
        Assert.That(userTag.Value, Is.EqualTo(conn.Settings.Username));

        var dbNameTag = activity.TagObjects.First(x => x.Key == "db.name");
        Assert.That(dbNameTag.Value, Is.EqualTo(conn.Settings.Database));

        var connStringTag = activity.TagObjects.First(x => x.Key == "db.connection_string");
        Assert.That(connStringTag.Value, Is.EqualTo(conn.ConnectionString));

        if (!IsMultiplexing)
        {
            var connIDTag = activity.TagObjects.First(x => x.Key == "db.connection_id");
            Assert.That(connIDTag.Value, Is.EqualTo(conn.ProcessID));
        }
        else
            Assert.That(activity.TagObjects.Any(x => x.Key == "db.connection_id"));
    }

    [Test]
    public async Task Basic_binary_export([Values] bool async)
    {
        if (IsMultiplexing && !async)
            return;

        var activities = new List<Activity>();

        using var activityListener = new ActivityListener();
        activityListener.ShouldListenTo = source => source.Name == "Npgsql";
        activityListener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded;
        activityListener.ActivityStopped = activity => activities.Add(activity);
        ActivitySource.AddActivityListener(activityListener);

        await using var dataSource = CreateDataSource(DisablePhysicalOpenTracing);
        await using var conn = await dataSource.OpenConnectionAsync();

        var table = await CreateTempTable(conn, "field_text TEXT, field_int2 SMALLINT");
        Assert.That(activities.Count, Is.EqualTo(1));
        activities.Clear();

        // Insert exactly one row before export
        var insertCmd = $"INSERT INTO {table} (field_text, field_int2) VALUES ('Hello', 8)";
        await conn.ExecuteNonQueryAsync(insertCmd);
        Assert.That(activities.Count, Is.EqualTo(1));
        activities.Clear();

        var copyToCommand = $"COPY {table} (field_text, field_int2) TO STDOUT BINARY";
        var rowsExported = 0;
        await using (var reader = async
            ? await conn.BeginBinaryExportAsync(copyToCommand)
            : conn.BeginBinaryExport(copyToCommand))
        {
            if (async)
            {
                while (await reader.StartRowAsync() != -1)
                {
                    await reader.ReadAsync<string>();
                    await reader.ReadAsync<short>();
                    rowsExported++;
                }
            }
            else
            {
                while (reader.StartRow() != -1)
                {
                    reader.Read<string>();
                    reader.Read<short>();
                    rowsExported++;
                }
            }
        }

        Assert.That(rowsExported, Is.EqualTo(1), "Should export exactly one row");
        Assert.That(activities.Count, Is.EqualTo(1));
        var activity = activities[0];
        Assert.That(activity.DisplayName, Is.EqualTo(conn.Settings.Database));
        Assert.That(activity.OperationName, Is.EqualTo(conn.Settings.Database));
        Assert.That(activity.Status, Is.EqualTo(ActivityStatusCode.Ok));

        var expectedTagCount = conn.Settings.Port == 5432 ? 11 : 12;
        Assert.That(activity.TagObjects.Count(), Is.EqualTo(expectedTagCount));

        var queryTag = activity.TagObjects.First(x => x.Key == "db.statement");
        Assert.That(queryTag.Value, Is.EqualTo(copyToCommand));

        var operationTag = activity.TagObjects.First(x => x.Key == "db.operation");
        Assert.That(operationTag.Value, Is.EqualTo("COPY TO"));

        var rowsTag = activity.TagObjects.First(x => x.Key == "db.rows");
        Assert.That(rowsTag.Value, Is.EqualTo(rowsExported));

        var systemTag = activity.TagObjects.First(x => x.Key == "db.system");
        Assert.That(systemTag.Value, Is.EqualTo("postgresql"));

        var userTag = activity.TagObjects.First(x => x.Key == "db.user");
        Assert.That(userTag.Value, Is.EqualTo(conn.Settings.Username));

        var dbNameTag = activity.TagObjects.First(x => x.Key == "db.name");
        Assert.That(dbNameTag.Value, Is.EqualTo(conn.Settings.Database));

        var connStringTag = activity.TagObjects.First(x => x.Key == "db.connection_string");
        Assert.That(connStringTag.Value, Is.EqualTo(conn.ConnectionString));

        if (!IsMultiplexing)
        {
            var connIDTag = activity.TagObjects.First(x => x.Key == "db.connection_id");
            Assert.That(connIDTag.Value, Is.EqualTo(conn.ProcessID));
        }
        else
            Assert.That(activity.TagObjects.Any(x => x.Key == "db.connection_id"));
    }

    [Test]
    public async Task Configure_tracing_binary_export([Values] bool async)
    {
        if (IsMultiplexing && !async)
            return;

        var activities = new List<Activity>();

        using var activityListener = new ActivityListener();
        activityListener.ShouldListenTo = source => source.Name == "Npgsql";
        activityListener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded;
        activityListener.ActivityStopped = activity => activities.Add(activity);
        ActivitySource.AddActivityListener(activityListener);

        await using var dataSource = CreateDataSource(builder =>
        {
            DisablePhysicalOpenTracing(builder);
            builder.ConfigureTracing(options =>
            {
                options
                    .ConfigureCopyOperationFilter((command, type) => type == CopyOperationType.BinaryExport && command.Contains("int2"))
                    .ConfigureCopyOperationSpanNameProvider((_, type) => type == CopyOperationType.BinaryExport ? "custom_binary_export" : null)
                    .ConfigureCopyOperationEnrichmentCallback((activity, _, type) =>
                    {
                        if (type == CopyOperationType.BinaryExport)
                        {
                            activity.AddTag("custom_tag", "custom_value");
                        }
                    });
            });
        });
        await using var conn = await dataSource.OpenConnectionAsync();

        // Filered out
        {
            var table = await CreateTempTable(conn, "field_text TEXT, field_int SMALLINT");
            Assert.That(activities.Count, Is.EqualTo(1));
            activities.Clear();

            // Insert exactly one row before export
            var insertCmd = $"INSERT INTO {table} (field_text, field_int) VALUES ('Hello', 8)";
            await conn.ExecuteNonQueryAsync(insertCmd);
            Assert.That(activities.Count, Is.EqualTo(1));
            activities.Clear();

            var copyToCommand = $"COPY {table} (field_text, field_int) TO STDOUT BINARY";
            await using (var reader = async
                ? await conn.BeginBinaryExportAsync(copyToCommand)
                : conn.BeginBinaryExport(copyToCommand))
            { }

            Assert.That(activities.Count, Is.EqualTo(0));
        }

        // Filtered in
        {
            var table = await CreateTempTable(conn, "field_text TEXT, field_int2 SMALLINT");
            Assert.That(activities.Count, Is.EqualTo(1));
            activities.Clear();

            // Insert exactly one row before export
            var insertCmd = $"INSERT INTO {table} (field_text, field_int2) VALUES ('Hello', 8)";
            await conn.ExecuteNonQueryAsync(insertCmd);
            Assert.That(activities.Count, Is.EqualTo(1));
            activities.Clear();

            var copyToCommand = $"COPY {table} (field_text, field_int2) TO STDOUT BINARY";
            await using (var reader = async
                ? await conn.BeginBinaryExportAsync(copyToCommand)
                : conn.BeginBinaryExport(copyToCommand))
            { }

            Assert.That(activities.Count, Is.EqualTo(1));
            var activity = activities[0];
            Assert.That(activity.DisplayName, Is.EqualTo("custom_binary_export"));
            Assert.That(activity.OperationName, Is.EqualTo("custom_binary_export"));

            Assert.That(activity.Events.Count(), Is.EqualTo(0));

            var customTag = activity.TagObjects.First(x => x.Key == "custom_tag");
            Assert.That(customTag.Value, Is.EqualTo("custom_value"));
        }
    }

    [Test]
    public async Task Cancel_binary_export([Values] bool async)
    {
        if (IsMultiplexing && !async)
            return;

        var activities = new List<Activity>();

        using var activityListener = new ActivityListener();
        activityListener.ShouldListenTo = source => source.Name == "Npgsql";
        activityListener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded;
        activityListener.ActivityStopped = activity => activities.Add(activity);
        ActivitySource.AddActivityListener(activityListener);

        await using var dataSource = CreateDataSource(DisablePhysicalOpenTracing);
        await using var conn = await dataSource.OpenConnectionAsync();

        // This must be large enough to cause Postgres to queue up CopyData messages.
        const string copyToCommand = "COPY (select md5(random()::text) as id from generate_series(1, 100000)) TO STDOUT BINARY";
        var exporter = async
            ? await conn.BeginBinaryExportAsync(copyToCommand)
            : conn.BeginBinaryExport(copyToCommand);
        if (async)
        {
            await exporter.StartRowAsync();
            await exporter.ReadAsync<string>();
            await exporter.CancelAsync();
            await exporter.DisposeAsync();
        }
        else
        {
            exporter.StartRow();
            exporter.Read<string>();
            exporter.Cancel();
            exporter.Dispose();
        }

        Assert.That(activities.Count, Is.EqualTo(1));
        var activity = activities[0];
        Assert.That(activity.DisplayName, Is.EqualTo(conn.Settings.Database));
        Assert.That(activity.OperationName, Is.EqualTo(conn.Settings.Database));
        Assert.That(activity.Status, Is.EqualTo(ActivityStatusCode.Error));
        Assert.That(activity.StatusDescription, Is.EqualTo("Cancelled"));

        var expectedTagCount = conn.Settings.Port == 5432 ? 10 : 11;
        Assert.That(activity.TagObjects.Count(), Is.EqualTo(expectedTagCount));

        var queryTag = activity.TagObjects.First(x => x.Key == "db.statement");
        Assert.That(queryTag.Value, Is.EqualTo(copyToCommand));

        var operationTag = activity.TagObjects.First(x => x.Key == "db.operation");
        Assert.That(operationTag.Value, Is.EqualTo("COPY TO"));

        var systemTag = activity.TagObjects.First(x => x.Key == "db.system");
        Assert.That(systemTag.Value, Is.EqualTo("postgresql"));

        var userTag = activity.TagObjects.First(x => x.Key == "db.user");
        Assert.That(userTag.Value, Is.EqualTo(conn.Settings.Username));

        var dbNameTag = activity.TagObjects.First(x => x.Key == "db.name");
        Assert.That(dbNameTag.Value, Is.EqualTo(conn.Settings.Database));

        var connStringTag = activity.TagObjects.First(x => x.Key == "db.connection_string");
        Assert.That(connStringTag.Value, Is.EqualTo(conn.ConnectionString));

        if (!IsMultiplexing)
        {
            var connIDTag = activity.TagObjects.First(x => x.Key == "db.connection_id");
            Assert.That(connIDTag.Value, Is.EqualTo(conn.ProcessID));
        }
        else
            Assert.That(activity.TagObjects.Any(x => x.Key == "db.connection_id"));
    }

    [Test]
    public async Task Error_binary_export([Values] bool async)
    {
        if (IsMultiplexing && !async)
            return;

        var activities = new List<Activity>();

        using var activityListener = new ActivityListener();
        activityListener.ShouldListenTo = source => source.Name == "Npgsql";
        activityListener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded;
        activityListener.ActivityStopped = activity => activities.Add(activity);
        ActivitySource.AddActivityListener(activityListener);

        await using var dataSource = CreateDataSource(DisablePhysicalOpenTracing);
        await using var conn = await dataSource.OpenConnectionAsync();

        var copyToCommand = $"COPY non_existing_table (field_text, field_int2) TO STDOUT BINARY";
        var ex = Assert.ThrowsAsync<PostgresException>(async () =>
        {
            await using var reader = async
                ? await conn.BeginBinaryExportAsync(copyToCommand)
                : conn.BeginBinaryExport(copyToCommand);
        });

        Assert.That(activities.Count, Is.EqualTo(1));
        var activity = activities[0];
        Assert.That(activity.DisplayName, Is.EqualTo(conn.Settings.Database));
        Assert.That(activity.OperationName, Is.EqualTo(conn.Settings.Database));
        Assert.That(activity.Status, Is.EqualTo(ActivityStatusCode.Error));
        Assert.That(activity.StatusDescription, Is.EqualTo(PostgresErrorCodes.UndefinedTable));

        Assert.That(activity.Events.Count(), Is.EqualTo(1));
        var exceptionEvent = activity.Events.First();
        Assert.That(exceptionEvent.Name, Is.EqualTo("exception"));

        Assert.That(exceptionEvent.Tags.Count(), Is.EqualTo(4));

        var exceptionTypeTag = exceptionEvent.Tags.First(x => x.Key == "exception.type");
        Assert.That(exceptionTypeTag.Value, Is.EqualTo(ex.GetType().FullName));

        var exceptionMessageTag = exceptionEvent.Tags.First(x => x.Key == "exception.message");
        Assert.That((string)exceptionMessageTag.Value!, Does.Contain(ex.Message));

        var exceptionStacktraceTag = exceptionEvent.Tags.First(x => x.Key == "exception.stacktrace");
        Assert.That((string)exceptionStacktraceTag.Value!, Does.Contain(ex.Message));

        var exceptionEscapedTag = exceptionEvent.Tags.First(x => x.Key == "exception.escaped");
        Assert.That(exceptionEscapedTag.Value, Is.True);

        var expectedTagCount = conn.Settings.Port == 5432 ? 10 : 11;
        Assert.That(activity.TagObjects.Count(), Is.EqualTo(expectedTagCount));

        var queryTag = activity.TagObjects.First(x => x.Key == "db.statement");
        Assert.That(queryTag.Value, Is.EqualTo(copyToCommand));

        var systemTag = activity.TagObjects.First(x => x.Key == "db.system");
        Assert.That(systemTag.Value, Is.EqualTo("postgresql"));

        var userTag = activity.TagObjects.First(x => x.Key == "db.user");
        Assert.That(userTag.Value, Is.EqualTo(conn.Settings.Username));

        var dbNameTag = activity.TagObjects.First(x => x.Key == "db.name");
        Assert.That(dbNameTag.Value, Is.EqualTo(conn.Settings.Database));

        var connStringTag = activity.TagObjects.First(x => x.Key == "db.connection_string");
        Assert.That(connStringTag.Value, Is.EqualTo(conn.ConnectionString));

        if (!IsMultiplexing)
        {
            var connIDTag = activity.TagObjects.First(x => x.Key == "db.connection_id");
            Assert.That(connIDTag.Value, Is.EqualTo(conn.ProcessID));
        }
        else
            Assert.That(activity.TagObjects.Any(x => x.Key == "db.connection_id"));
    }

    [Test]
    public async Task Basic_raw_copy_tracing([Values] bool async)
    {
        if (IsMultiplexing && !async)
            return;

        var activities = new List<Activity>();

        using var activityListener = new ActivityListener();
        activityListener.ShouldListenTo = source => source.Name == "Npgsql";
        activityListener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded;
        activityListener.ActivityStopped = activity => activities.Add(activity);
        ActivitySource.AddActivityListener(activityListener);

        await using var dataSource = CreateDataSource(DisablePhysicalOpenTracing);
        await using var conn = await dataSource.OpenConnectionAsync();

        var table = await CreateTempTable(conn, "field_text TEXT, field_int2 SMALLINT");
        Assert.That(activities.Count, Is.EqualTo(1));
        activities.Clear();

        // Insert a row for export
        await conn.ExecuteNonQueryAsync($"INSERT INTO {table} (field_text, field_int2) VALUES ('Hello', 8)");
        activities.Clear();

        // Raw binary export
        var copyToCommand = $"COPY {table} (field_text, field_int2) TO STDIN BINARY";
        using (var stream = async
            ? await conn.BeginRawBinaryCopyAsync(copyToCommand)
            : conn.BeginRawBinaryCopy(copyToCommand))
        {
            var buffer = new byte[1024];
            while (stream.Read(buffer, 0, buffer.Length) > 0) { }
        }

        Assert.That(activities.Count, Is.EqualTo(1));
        var activity = activities[0];
        Assert.That(activity.DisplayName, Is.EqualTo(conn.Settings.Database));
        Assert.That(activity.OperationName, Is.EqualTo(conn.Settings.Database));
        Assert.That(activity.Status, Is.EqualTo(ActivityStatusCode.Ok));
        Assert.That(activity.TagObjects.Any(x => x.Key == "db.rows"), Is.False, "db.rows should not be present for raw copy");
        Assert.That(activity.TagObjects.Any(x => x.Key == "db.statement" && (string?)x.Value == copyToCommand));
    }

    [Test]
    public async Task Configure_raw_copy_tracing([Values] bool async)
    {
        if (IsMultiplexing && !async)
            return;

        var activities = new List<Activity>();

        using var activityListener = new ActivityListener();
        activityListener.ShouldListenTo = source => source.Name == "Npgsql";
        activityListener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded;
        activityListener.ActivityStopped = activity => activities.Add(activity);
        ActivitySource.AddActivityListener(activityListener);

        await using var dataSource = CreateDataSource(builder =>
        {
            DisablePhysicalOpenTracing(builder);
            builder.ConfigureTracing(options =>
            {
                options
                    .ConfigureCopyOperationFilter((command, type) => type == CopyOperationType.RawBinary && command.Contains("int2"))
                    .ConfigureCopyOperationSpanNameProvider((_, type) => type == CopyOperationType.RawBinary ? "custom_raw_binary" : null)
                    .ConfigureCopyOperationEnrichmentCallback((activity, _, type) =>
                    {
                        if (type == CopyOperationType.RawBinary)
                        {
                            activity.AddTag("custom_tag", "custom_value");
                        }
                    });
            });
        });
        await using var conn = await dataSource.OpenConnectionAsync();

        // Filtered out
        {
            var table = await CreateTempTable(conn, "field_text TEXT, field_int SMALLINT");
            Assert.That(activities.Count, Is.EqualTo(1));
            activities.Clear();

            // Insert a row for export
            await conn.ExecuteNonQueryAsync($"INSERT INTO {table} (field_text, field_int) VALUES ('Hello', 8)");
            activities.Clear();

            // Raw binary export
            var copyToCommand = $"COPY {table} (field_text, field_int) TO STDIN BINARY";
            using (var stream = async
                ? await conn.BeginRawBinaryCopyAsync(copyToCommand)
                : conn.BeginRawBinaryCopy(copyToCommand))
            { }

            Assert.That(activities.Count, Is.EqualTo(0));
        }

        // Filtered in
        {
            var table = await CreateTempTable(conn, "field_text TEXT, field_int2 SMALLINT");
            Assert.That(activities.Count, Is.EqualTo(1));
            activities.Clear();

            // Insert a row for export
            await conn.ExecuteNonQueryAsync($"INSERT INTO {table} (field_text, field_int2) VALUES ('Hello', 8)");
            activities.Clear();

            // Raw binary export
            var copyToCommand = $"COPY {table} (field_text, field_int2) TO STDIN BINARY";
            using (var stream = async
                ? await conn.BeginRawBinaryCopyAsync(copyToCommand)
                : conn.BeginRawBinaryCopy(copyToCommand))
            { }

            Assert.That(activities.Count, Is.EqualTo(1));
            var activity = activities[0];
            Assert.That(activity.DisplayName, Is.EqualTo("custom_raw_binary"));
            Assert.That(activity.OperationName, Is.EqualTo("custom_raw_binary"));

            Assert.That(activity.Events.Count(), Is.EqualTo(0));

            var customTag = activity.TagObjects.First(x => x.Key == "custom_tag");
            Assert.That(customTag.Value, Is.EqualTo("custom_value"));
        }
    }

    [Test]
    public async Task Cancel_raw_copy_read_tracing([Values] bool async)
    {
        if (IsMultiplexing && !async)
            return;

        var activities = new List<Activity>();

        using var activityListener = new ActivityListener();
        activityListener.ShouldListenTo = source => source.Name == "Npgsql";
        activityListener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded;
        activityListener.ActivityStopped = activity => activities.Add(activity);
        ActivitySource.AddActivityListener(activityListener);

        await using var dataSource = CreateDataSource(DisablePhysicalOpenTracing);
        await using var conn = await dataSource.OpenConnectionAsync();

        var table = await CreateTempTable(conn, "field_text TEXT, field_int2 SMALLINT");
        Assert.That(activities.Count, Is.EqualTo(1));
        activities.Clear();

        // Insert a row for export
        await conn.ExecuteNonQueryAsync($"INSERT INTO {table} (field_text, field_int2) VALUES ('Hello', 8)");
        activities.Clear();

        // Raw binary export
        var copyToCommand = $"COPY {table} (field_text, field_int2) TO STDIN BINARY";
        var stream = async
            ? await conn.BeginRawBinaryCopyAsync(copyToCommand)
            : conn.BeginRawBinaryCopy(copyToCommand);
        var buffer = new byte[1024];
        if (async)
        {
            var _ = await stream.ReadAsync(buffer, 0, buffer.Length);
            await stream.CancelAsync();
            await stream.DisposeAsync();
        }
        else
        {
            var _ = stream.Read(buffer, 0, buffer.Length);
            stream.Cancel();
            stream.Dispose();
        }

        Assert.That(activities.Count, Is.EqualTo(1));
        var activity = activities[0];
        Assert.That(activity.DisplayName, Is.EqualTo(conn.Settings.Database));
        Assert.That(activity.OperationName, Is.EqualTo(conn.Settings.Database));
        Assert.That(activity.Status, Is.EqualTo(ActivityStatusCode.Error));
        Assert.That(activity.StatusDescription, Is.EqualTo("Cancelled"));
        Assert.That(activity.TagObjects.Any(x => x.Key == "db.rows"), Is.False, "db.rows should not be present for raw copy");
        Assert.That(activity.TagObjects.Any(x => x.Key == "db.statement" && (string?)x.Value == copyToCommand));
    }

    [Test]
    public async Task Cancel_raw_copy_write_tracing([Values] bool async)
    {
        if (IsMultiplexing && !async)
            return;

        var activities = new List<Activity>();

        using var activityListener = new ActivityListener();
        activityListener.ShouldListenTo = source => source.Name == "Npgsql";
        activityListener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded;
        activityListener.ActivityStopped = activity => activities.Add(activity);
        ActivitySource.AddActivityListener(activityListener);

        await using var dataSource = CreateDataSource(DisablePhysicalOpenTracing);
        await using var conn = await dataSource.OpenConnectionAsync();

        var table = await CreateTempTable(conn, "field_text TEXT, field_int2 SMALLINT");
        Assert.That(activities.Count, Is.EqualTo(1));
        activities.Clear();

        // Raw binary import
        var copyToCommand = $"COPY {table} (field_text, field_int2) FROM STDIN BINARY";
        var stream = async
            ? await conn.BeginRawBinaryCopyAsync(copyToCommand)
            : conn.BeginRawBinaryCopy(copyToCommand);
        var garbage = new byte[] { 1, 2, 3, 4 };
        if (async)
        {
            await stream.WriteAsync(garbage);
            await stream.FlushAsync();
            await stream.CancelAsync();
            await stream.DisposeAsync();
        }
        else
        {
            stream.Write(garbage);
            stream.Flush();
            stream.Cancel();
            stream.Dispose();
        }

        Assert.That(activities.Count, Is.EqualTo(1));
        var activity = activities[0];
        Assert.That(activity.DisplayName, Is.EqualTo(conn.Settings.Database));
        Assert.That(activity.OperationName, Is.EqualTo(conn.Settings.Database));
        Assert.That(activity.Status, Is.EqualTo(ActivityStatusCode.Error));
        Assert.That(activity.StatusDescription, Is.EqualTo("Cancelled"));
        Assert.That(activity.TagObjects.Any(x => x.Key == "db.rows"), Is.False, "db.rows should not be present for raw copy");
        Assert.That(activity.TagObjects.Any(x => x.Key == "db.statement" && (string?)x.Value == copyToCommand));
    }

    [Test]
    public async Task Error_raw_copy_tracing([Values] bool async)
    {
        if (IsMultiplexing && !async)
            return;

        var activities = new List<Activity>();

        using var activityListener = new ActivityListener();
        activityListener.ShouldListenTo = source => source.Name == "Npgsql";
        activityListener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded;
        activityListener.ActivityStopped = activity => activities.Add(activity);
        ActivitySource.AddActivityListener(activityListener);

        await using var dataSource = CreateDataSource(DisablePhysicalOpenTracing);
        await using var conn = await dataSource.OpenConnectionAsync();

        var copyFromCommand = $"COPY non_existing_table (field_text, field_int2) FROM STDIN BINARY";
        var ex = Assert.ThrowsAsync<PostgresException>(async () =>
        {
            await using var stream = async
                ? await conn.BeginRawBinaryCopyAsync(copyFromCommand)
                : conn.BeginRawBinaryCopy(copyFromCommand);
        });

        Assert.That(activities.Count, Is.EqualTo(1));
        var activity = activities[0];
        Assert.That(activity.Status, Is.EqualTo(ActivityStatusCode.Error));
        Assert.That(activity.TagObjects.Any(x => x.Key == "db.rows"), Is.False, "db.rows should not be present for raw copy");
        Assert.That(activity.TagObjects.Any(x => x.Key == "db.statement" && (string?)x.Value == copyFromCommand));

        Assert.That(activity.Events.Count(), Is.EqualTo(1));
        var exceptionEvent = activity.Events.First();
        Assert.That(exceptionEvent.Name, Is.EqualTo("exception"));

        Assert.That(exceptionEvent.Tags.Count(), Is.EqualTo(4));

        var exceptionTypeTag = exceptionEvent.Tags.First(x => x.Key == "exception.type");
        Assert.That(exceptionTypeTag.Value, Is.EqualTo(ex.GetType().FullName));

        var exceptionMessageTag = exceptionEvent.Tags.First(x => x.Key == "exception.message");
        Assert.That((string)exceptionMessageTag.Value!, Does.Contain(ex.Message));

        var exceptionStacktraceTag = exceptionEvent.Tags.First(x => x.Key == "exception.stacktrace");
        Assert.That((string)exceptionStacktraceTag.Value!, Does.Contain(ex.Message));
    }

    [Test]
    public async Task Basic_text_import([Values] bool async)
    {
        if (IsMultiplexing && !async)
            return;

        var activities = new List<Activity>();

        using var activityListener = new ActivityListener();
        activityListener.ShouldListenTo = source => source.Name == "Npgsql";
        activityListener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded;
        activityListener.ActivityStopped = activity => activities.Add(activity);
        ActivitySource.AddActivityListener(activityListener);

        await using var dataSource = CreateDataSource(DisablePhysicalOpenTracing);
        await using var conn = await dataSource.OpenConnectionAsync();

        var table = await CreateTempTable(conn, "field_text TEXT, field_int2 SMALLINT");

        // We're not interested in temp table creation's activity
        Assert.That(activities.Count, Is.EqualTo(1));
        activities.Clear();

        var longString = new StringBuilder(conn.Settings.WriteBufferSize + 50).Append('a').ToString();

        var copyFromCommand = $"COPY {table} (field_text, field_int2) FROM STDIN";

        await using (var writer = async
            ? await conn.BeginTextImportAsync(copyFromCommand)
            : conn.BeginTextImport(copyFromCommand))
        {
            const string line = "Hello\t8\n";
            if (async)
            {
                await writer.WriteAsync(line);
            }
            else
            {
                writer.Write(line);
            }
        }

        Assert.That(activities.Count, Is.EqualTo(1));
        var activity = activities[0];
        Assert.That(activity.DisplayName, Is.EqualTo(conn.Settings.Database));
        Assert.That(activity.OperationName, Is.EqualTo(conn.Settings.Database));
        Assert.That(activity.Status, Is.EqualTo(ActivityStatusCode.Ok));

        var expectedTagCount = conn.Settings.Port == 5432 ? 10 : 11;
        Assert.That(activity.TagObjects.Count(), Is.EqualTo(expectedTagCount));

        var queryTag = activity.TagObjects.First(x => x.Key == "db.statement");
        Assert.That(queryTag.Value, Is.EqualTo(copyFromCommand));

        var operationTag = activity.TagObjects.First(x => x.Key == "db.operation");
        Assert.That(operationTag.Value, Is.EqualTo("COPY FROM"));

        var systemTag = activity.TagObjects.First(x => x.Key == "db.system");
        Assert.That(systemTag.Value, Is.EqualTo("postgresql"));

        var userTag = activity.TagObjects.First(x => x.Key == "db.user");
        Assert.That(userTag.Value, Is.EqualTo(conn.Settings.Username));

        var dbNameTag = activity.TagObjects.First(x => x.Key == "db.name");
        Assert.That(dbNameTag.Value, Is.EqualTo(conn.Settings.Database));

        var connStringTag = activity.TagObjects.First(x => x.Key == "db.connection_string");
        Assert.That(connStringTag.Value, Is.EqualTo(conn.ConnectionString));

        if (!IsMultiplexing)
        {
            var connIDTag = activity.TagObjects.First(x => x.Key == "db.connection_id");
            Assert.That(connIDTag.Value, Is.EqualTo(conn.ProcessID));
        }
        else
            Assert.That(activity.TagObjects.Any(x => x.Key == "db.connection_id"));
    }

    [Test]
    public async Task Configure_tracing_text_import([Values] bool async)
    {
        if (IsMultiplexing && !async)
            return;

        var activities = new List<Activity>();

        using var activityListener = new ActivityListener();
        activityListener.ShouldListenTo = source => source.Name == "Npgsql";
        activityListener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded;
        activityListener.ActivityStopped = activity => activities.Add(activity);
        ActivitySource.AddActivityListener(activityListener);

        await using var dataSource = CreateDataSource(builder =>
        {
            DisablePhysicalOpenTracing(builder);
            builder.ConfigureTracing(options =>
            {
                options
                    .ConfigureCopyOperationFilter((command, type) => type == CopyOperationType.TextImport && command.Contains("int2"))
                    .ConfigureCopyOperationSpanNameProvider((_, type) => type == CopyOperationType.TextImport ? "custom_text_import" : null)
                    .ConfigureCopyOperationEnrichmentCallback((activity, _, type) =>
                    {
                        if (type == CopyOperationType.TextImport)
                        {
                            activity.AddTag("custom_tag", "custom_value");
                        }
                    });
            });
        });

        await using var conn = await dataSource.OpenConnectionAsync();

        // Filtered out
        {
            var table = await CreateTempTable(conn, "field_text TEXT, field_int SMALLINT");

            // We're not interested in temp table creation's activity
            Assert.That(activities.Count, Is.EqualTo(1));
            activities.Clear();

            var copyFromCommand = $"COPY {table} (field_text, field_int) FROM STDIN";

            await using (var writer = async
                ? await conn.BeginTextImportAsync(copyFromCommand)
                : conn.BeginTextImport(copyFromCommand))
            { }

            Assert.That(activities.Count, Is.EqualTo(0));
        }

        // Filtered in
        {
            var table = await CreateTempTable(conn, "field_text TEXT, field_int2 SMALLINT");

            // We're not interested in temp table creation's activity
            Assert.That(activities.Count, Is.EqualTo(1));
            activities.Clear();

            var copyFromCommand = $"COPY {table} (field_text, field_int2) FROM STDIN";

            await using (var writer = async
                ? await conn.BeginTextImportAsync(copyFromCommand)
                : conn.BeginTextImport(copyFromCommand))
            { }

            Assert.That(activities.Count, Is.EqualTo(1));
            var activity = activities[0];
            Assert.That(activity.DisplayName, Is.EqualTo("custom_text_import"));
            Assert.That(activity.OperationName, Is.EqualTo("custom_text_import"));

            Assert.That(activity.Events.Count(), Is.EqualTo(0));

            var customTag = activity.TagObjects.First(x => x.Key == "custom_tag");
            Assert.That(customTag.Value, Is.EqualTo("custom_value"));
        }
    }

    [Test]
    public async Task Cancel_text_import([Values] bool async)
    {
        if (IsMultiplexing && !async)
            return;

        var activities = new List<Activity>();

        using var activityListener = new ActivityListener();
        activityListener.ShouldListenTo = source => source.Name == "Npgsql";
        activityListener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded;
        activityListener.ActivityStopped = activity => activities.Add(activity);
        ActivitySource.AddActivityListener(activityListener);

        await using var dataSource = CreateDataSource(DisablePhysicalOpenTracing);
        await using var conn = await dataSource.OpenConnectionAsync();

        var table = await CreateTempTable(conn, "field_text TEXT, field_int2 SMALLINT");

        // We're not interested in temp table creation's activity
        Assert.That(activities.Count, Is.EqualTo(1));
        activities.Clear();

        var longString = new StringBuilder(conn.Settings.WriteBufferSize + 50).Append('a').ToString();

        var copyFromCommand = $"COPY {table} (field_text, field_int2) FROM STDIN";

        var writer = (NpgsqlCopyTextWriter)(async
            ? await conn.BeginTextImportAsync(copyFromCommand)
            : conn.BeginTextImport(copyFromCommand));

        const string line = "Hello\t8\n";
        if (async)
        {
            await writer.WriteAsync(line);
            await writer.FlushAsync();
            await writer.CancelAsync();
        }
        else
        {
            writer.Write(line);
            writer.Flush();
            writer.Cancel();
        }

        Assert.That(activities.Count, Is.EqualTo(1));
        var activity = activities[0];
        Assert.That(activity.DisplayName, Is.EqualTo(conn.Settings.Database));
        Assert.That(activity.OperationName, Is.EqualTo(conn.Settings.Database));
        Assert.That(activity.Status, Is.EqualTo(ActivityStatusCode.Error));
        Assert.That(activity.StatusDescription, Is.EqualTo("Cancelled"));

        var expectedTagCount = conn.Settings.Port == 5432 ? 10 : 11;
        Assert.That(activity.TagObjects.Count(), Is.EqualTo(expectedTagCount));

        var queryTag = activity.TagObjects.First(x => x.Key == "db.statement");
        Assert.That(queryTag.Value, Is.EqualTo(copyFromCommand));

        var systemTag = activity.TagObjects.First(x => x.Key == "db.system");
        Assert.That(systemTag.Value, Is.EqualTo("postgresql"));

        var userTag = activity.TagObjects.First(x => x.Key == "db.user");
        Assert.That(userTag.Value, Is.EqualTo(conn.Settings.Username));

        var dbNameTag = activity.TagObjects.First(x => x.Key == "db.name");
        Assert.That(dbNameTag.Value, Is.EqualTo(conn.Settings.Database));

        var connStringTag = activity.TagObjects.First(x => x.Key == "db.connection_string");
        Assert.That(connStringTag.Value, Is.EqualTo(conn.ConnectionString));

        if (!IsMultiplexing)
        {
            var connIDTag = activity.TagObjects.First(x => x.Key == "db.connection_id");
            Assert.That(connIDTag.Value, Is.EqualTo(conn.ProcessID));
        }
        else
            Assert.That(activity.TagObjects.Any(x => x.Key == "db.connection_id"));
    }

    [Test]
    public async Task Error_text_import([Values] bool async)
    {
        if (IsMultiplexing && !async)
            return;

        var activities = new List<Activity>();

        using var activityListener = new ActivityListener();
        activityListener.ShouldListenTo = source => source.Name == "Npgsql";
        activityListener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded;
        activityListener.ActivityStopped = activity => activities.Add(activity);
        ActivitySource.AddActivityListener(activityListener);

        await using var dataSource = CreateDataSource(DisablePhysicalOpenTracing);
        await using var conn = await dataSource.OpenConnectionAsync();

        var copyFromCommand = $"COPY non_existing_table (field_text, field_int2) FROM STDIN";

        var ex = Assert.ThrowsAsync<PostgresException>(async () =>
        {
            await using var writer = async
                ? await conn.BeginTextImportAsync(copyFromCommand)
                : conn.BeginTextImport(copyFromCommand);
        });

        Assert.That(activities.Count, Is.EqualTo(1));
        var activity = activities[0];
        Assert.That(activity.DisplayName, Is.EqualTo(conn.Settings.Database));
        Assert.That(activity.OperationName, Is.EqualTo(conn.Settings.Database));
        Assert.That(activity.Status, Is.EqualTo(ActivityStatusCode.Error));
        Assert.That(activity.StatusDescription, Is.EqualTo(PostgresErrorCodes.UndefinedTable));

        Assert.That(activity.Events.Count(), Is.EqualTo(1));
        var exceptionEvent = activity.Events.First();
        Assert.That(exceptionEvent.Name, Is.EqualTo("exception"));

        Assert.That(exceptionEvent.Tags.Count(), Is.EqualTo(4));

        var exceptionTypeTag = exceptionEvent.Tags.First(x => x.Key == "exception.type");
        Assert.That(exceptionTypeTag.Value, Is.EqualTo(ex.GetType().FullName));

        var exceptionMessageTag = exceptionEvent.Tags.First(x => x.Key == "exception.message");
        Assert.That((string)exceptionMessageTag.Value!, Does.Contain(ex.Message));

        var exceptionStacktraceTag = exceptionEvent.Tags.First(x => x.Key == "exception.stacktrace");
        Assert.That((string)exceptionStacktraceTag.Value!, Does.Contain(ex.Message));

        var exceptionEscapedTag = exceptionEvent.Tags.First(x => x.Key == "exception.escaped");
        Assert.That(exceptionEscapedTag.Value, Is.True);

        var expectedTagCount = conn.Settings.Port == 5432 ? 9 : 10;
        Assert.That(activity.TagObjects.Count(), Is.EqualTo(expectedTagCount));

        var queryTag = activity.TagObjects.First(x => x.Key == "db.statement");
        Assert.That(queryTag.Value, Is.EqualTo(copyFromCommand));

        var systemTag = activity.TagObjects.First(x => x.Key == "db.system");
        Assert.That(systemTag.Value, Is.EqualTo("postgresql"));

        var userTag = activity.TagObjects.First(x => x.Key == "db.user");
        Assert.That(userTag.Value, Is.EqualTo(conn.Settings.Username));

        var dbNameTag = activity.TagObjects.First(x => x.Key == "db.name");
        Assert.That(dbNameTag.Value, Is.EqualTo(conn.Settings.Database));

        var connStringTag = activity.TagObjects.First(x => x.Key == "db.connection_string");
        Assert.That(connStringTag.Value, Is.EqualTo(conn.ConnectionString));

        if (!IsMultiplexing)
        {
            var connIDTag = activity.TagObjects.First(x => x.Key == "db.connection_id");
            Assert.That(connIDTag.Value, Is.EqualTo(conn.ProcessID));
        }
        else
            Assert.That(activity.TagObjects.Any(x => x.Key == "db.connection_id"));
    }

    [Test]
    public async Task Basic_text_export([Values] bool async)
    {
        if (IsMultiplexing && !async)
            return;

        var activities = new List<Activity>();

        using var activityListener = new ActivityListener();
        activityListener.ShouldListenTo = source => source.Name == "Npgsql";
        activityListener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded;
        activityListener.ActivityStopped = activity => activities.Add(activity);
        ActivitySource.AddActivityListener(activityListener);

        await using var dataSource = CreateDataSource(DisablePhysicalOpenTracing);
        await using var conn = await dataSource.OpenConnectionAsync();

        var table = await CreateTempTable(conn, "field_text TEXT, field_int2 SMALLINT");

        // We're not interested in temp table creation's activity
        Assert.That(activities.Count, Is.EqualTo(1));
        activities.Clear();

        // Insert exactly one row before export
        var insertCmd = $"INSERT INTO {table} (field_text, field_int2) VALUES ('Hello', 8)";
        await conn.ExecuteNonQueryAsync(insertCmd);
        Assert.That(activities.Count, Is.EqualTo(1));
        activities.Clear();

        var longString = new StringBuilder(conn.Settings.WriteBufferSize + 50).Append('a').ToString();

        var copyFromCommand = $"COPY {table} (field_text, field_int2) TO STDIN";

        using (var reader = async
            ? await conn.BeginTextExportAsync(copyFromCommand)
            : conn.BeginTextExport(copyFromCommand))
        {
            var chars = new char[30];
            if (async)
            {
                await reader.ReadAsync(chars);
            }
            else
            {
                reader.Read(chars);
            }

            Assert.That(new string(chars, 0, 8), Is.EqualTo("Hello\t8\n"));
        }

        Assert.That(activities.Count, Is.EqualTo(1));
        var activity = activities[0];
        Assert.That(activity.DisplayName, Is.EqualTo(conn.Settings.Database));
        Assert.That(activity.OperationName, Is.EqualTo(conn.Settings.Database));
        Assert.That(activity.Status, Is.EqualTo(ActivityStatusCode.Ok));

        var expectedTagCount = conn.Settings.Port == 5432 ? 10 : 11;
        Assert.That(activity.TagObjects.Count(), Is.EqualTo(expectedTagCount));

        var queryTag = activity.TagObjects.First(x => x.Key == "db.statement");
        Assert.That(queryTag.Value, Is.EqualTo(copyFromCommand));

        var operationTag = activity.TagObjects.First(x => x.Key == "db.operation");
        Assert.That(operationTag.Value, Is.EqualTo("COPY TO"));

        var systemTag = activity.TagObjects.First(x => x.Key == "db.system");
        Assert.That(systemTag.Value, Is.EqualTo("postgresql"));

        var userTag = activity.TagObjects.First(x => x.Key == "db.user");
        Assert.That(userTag.Value, Is.EqualTo(conn.Settings.Username));

        var dbNameTag = activity.TagObjects.First(x => x.Key == "db.name");
        Assert.That(dbNameTag.Value, Is.EqualTo(conn.Settings.Database));

        var connStringTag = activity.TagObjects.First(x => x.Key == "db.connection_string");
        Assert.That(connStringTag.Value, Is.EqualTo(conn.ConnectionString));

        if (!IsMultiplexing)
        {
            var connIDTag = activity.TagObjects.First(x => x.Key == "db.connection_id");
            Assert.That(connIDTag.Value, Is.EqualTo(conn.ProcessID));
        }
        else
            Assert.That(activity.TagObjects.Any(x => x.Key == "db.connection_id"));
    }

    [Test]
    public async Task Configure_tracing_text_export([Values] bool async)
    {
        if (IsMultiplexing && !async)
            return;

        var activities = new List<Activity>();

        using var activityListener = new ActivityListener();
        activityListener.ShouldListenTo = source => source.Name == "Npgsql";
        activityListener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded;
        activityListener.ActivityStopped = activity => activities.Add(activity);
        ActivitySource.AddActivityListener(activityListener);

        await using var dataSource = CreateDataSource(builder =>
        {
            DisablePhysicalOpenTracing(builder);
            builder.ConfigureTracing(options =>
            {
                options
                    .ConfigureCopyOperationFilter((command, type) => type == CopyOperationType.TextExport && command.Contains("int2"))
                    .ConfigureCopyOperationSpanNameProvider((_, type) => type == CopyOperationType.TextExport ? "custom_text_export" : null)
                    .ConfigureCopyOperationEnrichmentCallback((activity, _, type) =>
                    {
                        if (type == CopyOperationType.TextExport)
                        {
                            activity.AddTag("custom_tag", "custom_value");
                        }
                    });
            });
        });

        await using var conn = await dataSource.OpenConnectionAsync();

        // Filtered out
        {
            var table = await CreateTempTable(conn, "field_text TEXT, field_int SMALLINT");
            // We're not interested in temp table creation's activity
            Assert.That(activities.Count, Is.EqualTo(1));
            activities.Clear();
            // Insert exactly one row before export
            var insertCmd = $"INSERT INTO {table} (field_text, field_int) VALUES ('Hello', 8)";
            await conn.ExecuteNonQueryAsync(insertCmd);
            Assert.That(activities.Count, Is.EqualTo(1));
            activities.Clear();
            var copyFromCommand = $"COPY {table} (field_text, field_int) TO STDIN";
            using (var reader = async
                ? await conn.BeginTextExportAsync(copyFromCommand)
                : conn.BeginTextExport(copyFromCommand))
            { }

            Assert.That(activities.Count, Is.EqualTo(0));
        }

        // Filtered in
        {
            var table = await CreateTempTable(conn, "field_text TEXT, field_int2 SMALLINT");

            // We're not interested in temp table creation's activity
            Assert.That(activities.Count, Is.EqualTo(1));
            activities.Clear();

            // Insert exactly one row before export
            var insertCmd = $"INSERT INTO {table} (field_text, field_int2) VALUES ('Hello', 8)";
            await conn.ExecuteNonQueryAsync(insertCmd);
            Assert.That(activities.Count, Is.EqualTo(1));
            activities.Clear();

            var longString = new StringBuilder(conn.Settings.WriteBufferSize + 50).Append('a').ToString();

            var copyFromCommand = $"COPY {table} (field_text, field_int2) TO STDIN";

            using (var reader = async
                ? await conn.BeginTextExportAsync(copyFromCommand)
                : conn.BeginTextExport(copyFromCommand))
            { }

            Assert.That(activities.Count, Is.EqualTo(1));
            var activity = activities[0];
            Assert.That(activity.DisplayName, Is.EqualTo("custom_text_export"));
            Assert.That(activity.OperationName, Is.EqualTo("custom_text_export"));

            Assert.That(activity.Events.Count(), Is.EqualTo(0));

            var customTag = activity.TagObjects.First(x => x.Key == "custom_tag");
            Assert.That(customTag.Value, Is.EqualTo("custom_value"));
        }
    }

    [Test]
    public async Task Cancel_text_export([Values] bool async)
    {
        if (IsMultiplexing && !async)
            return;

        var activities = new List<Activity>();

        using var activityListener = new ActivityListener();
        activityListener.ShouldListenTo = source => source.Name == "Npgsql";
        activityListener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded;
        activityListener.ActivityStopped = activity => activities.Add(activity);
        ActivitySource.AddActivityListener(activityListener);

        await using var dataSource = CreateDataSource(DisablePhysicalOpenTracing);
        await using var conn = await dataSource.OpenConnectionAsync();

        var table = await CreateTempTable(conn, "field_text TEXT, field_int2 SMALLINT");

        // We're not interested in temp table creation's activity
        Assert.That(activities.Count, Is.EqualTo(1));
        activities.Clear();

        var longString = new StringBuilder(conn.Settings.WriteBufferSize + 50).Append('a').ToString();

        var copyFromCommand = $"COPY {table} (field_text, field_int2) TO STDIN";

        using (var reader = (NpgsqlCopyTextReader)(async
            ? await conn.BeginTextExportAsync(copyFromCommand)
            : conn.BeginTextExport(copyFromCommand)))
        {
            if (async)
            {
                await reader.CancelAsync();
            }
            else
            {
                reader.Cancel();
            }
        }

        Assert.That(activities.Count, Is.EqualTo(1));
        var activity = activities[0];
        Assert.That(activity.DisplayName, Is.EqualTo(conn.Settings.Database));
        Assert.That(activity.OperationName, Is.EqualTo(conn.Settings.Database));
        Assert.That(activity.Status, Is.EqualTo(ActivityStatusCode.Error));
        Assert.That(activity.StatusDescription, Is.EqualTo("Cancelled"));

        var expectedTagCount = conn.Settings.Port == 5432 ? 10 : 11;
        Assert.That(activity.TagObjects.Count(), Is.EqualTo(expectedTagCount));

        var queryTag = activity.TagObjects.First(x => x.Key == "db.statement");
        Assert.That(queryTag.Value, Is.EqualTo(copyFromCommand));

        var systemTag = activity.TagObjects.First(x => x.Key == "db.system");
        Assert.That(systemTag.Value, Is.EqualTo("postgresql"));

        var userTag = activity.TagObjects.First(x => x.Key == "db.user");
        Assert.That(userTag.Value, Is.EqualTo(conn.Settings.Username));

        var dbNameTag = activity.TagObjects.First(x => x.Key == "db.name");
        Assert.That(dbNameTag.Value, Is.EqualTo(conn.Settings.Database));

        var connStringTag = activity.TagObjects.First(x => x.Key == "db.connection_string");
        Assert.That(connStringTag.Value, Is.EqualTo(conn.ConnectionString));

        if (!IsMultiplexing)
        {
            var connIDTag = activity.TagObjects.First(x => x.Key == "db.connection_id");
            Assert.That(connIDTag.Value, Is.EqualTo(conn.ProcessID));
        }
        else
            Assert.That(activity.TagObjects.Any(x => x.Key == "db.connection_id"));
    }

    [Test]
    public async Task Error_text_export([Values] bool async)
    {
        if (IsMultiplexing && !async)
            return;

        var activities = new List<Activity>();

        using var activityListener = new ActivityListener();
        activityListener.ShouldListenTo = source => source.Name == "Npgsql";
        activityListener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded;
        activityListener.ActivityStopped = activity => activities.Add(activity);
        ActivitySource.AddActivityListener(activityListener);

        await using var dataSource = CreateDataSource(DisablePhysicalOpenTracing);
        await using var conn = await dataSource.OpenConnectionAsync();

        var copyFromCommand = $"COPY non_existing_table (field_text, field_int2) TO STDIN";

        var ex = Assert.ThrowsAsync<PostgresException>(async () =>
        {
            var reader = async
                ? await conn.BeginTextExportAsync(copyFromCommand)
                : conn.BeginTextExport(copyFromCommand);
        });

        Assert.That(activities.Count, Is.EqualTo(1));
        var activity = activities[0];
        Assert.That(activity.DisplayName, Is.EqualTo(conn.Settings.Database));
        Assert.That(activity.OperationName, Is.EqualTo(conn.Settings.Database));
        Assert.That(activity.Status, Is.EqualTo(ActivityStatusCode.Error));
        Assert.That(activity.StatusDescription, Is.EqualTo(PostgresErrorCodes.UndefinedTable));

        Assert.That(activity.Events.Count(), Is.EqualTo(1));
        var exceptionEvent = activity.Events.First();
        Assert.That(exceptionEvent.Name, Is.EqualTo("exception"));

        Assert.That(exceptionEvent.Tags.Count(), Is.EqualTo(4));

        var exceptionTypeTag = exceptionEvent.Tags.First(x => x.Key == "exception.type");
        Assert.That(exceptionTypeTag.Value, Is.EqualTo(ex.GetType().FullName));

        var exceptionMessageTag = exceptionEvent.Tags.First(x => x.Key == "exception.message");
        Assert.That((string)exceptionMessageTag.Value!, Does.Contain(ex.Message));

        var exceptionStacktraceTag = exceptionEvent.Tags.First(x => x.Key == "exception.stacktrace");
        Assert.That((string)exceptionStacktraceTag.Value!, Does.Contain(ex.Message));

        var exceptionEscapedTag = exceptionEvent.Tags.First(x => x.Key == "exception.escaped");
        Assert.That(exceptionEscapedTag.Value, Is.True);

        var expectedTagCount = conn.Settings.Port == 5432 ? 9 : 10;
        Assert.That(activity.TagObjects.Count(), Is.EqualTo(expectedTagCount));

        var queryTag = activity.TagObjects.First(x => x.Key == "db.statement");
        Assert.That(queryTag.Value, Is.EqualTo(copyFromCommand));

        var systemTag = activity.TagObjects.First(x => x.Key == "db.system");
        Assert.That(systemTag.Value, Is.EqualTo("postgresql"));

        var userTag = activity.TagObjects.First(x => x.Key == "db.user");
        Assert.That(userTag.Value, Is.EqualTo(conn.Settings.Username));

        var dbNameTag = activity.TagObjects.First(x => x.Key == "db.name");
        Assert.That(dbNameTag.Value, Is.EqualTo(conn.Settings.Database));

        var connStringTag = activity.TagObjects.First(x => x.Key == "db.connection_string");
        Assert.That(connStringTag.Value, Is.EqualTo(conn.ConnectionString));

        if (!IsMultiplexing)
        {
            var connIDTag = activity.TagObjects.First(x => x.Key == "db.connection_id");
            Assert.That(connIDTag.Value, Is.EqualTo(conn.ProcessID));
        }
        else
            Assert.That(activity.TagObjects.Any(x => x.Key == "db.connection_id"));
    }

    static void DisablePhysicalOpenTracing(NpgsqlDataSourceBuilder dsb) => dsb.ConfigureTracing(tob => tob.EnablePhysicalOpenTracing(false));

    async Task<object?> ExecuteScalar(NpgsqlConnection connection, bool async, bool isBatch, string query)
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
