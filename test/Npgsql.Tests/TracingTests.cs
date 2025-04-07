using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using NUnit.Framework;

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

        using var activityListener = new ActivityListener();
        activityListener.ShouldListenTo = source => source.Name == "Npgsql";
        activityListener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded;
        activityListener.ActivityStopped = activity => activities.Add(activity);
        ActivitySource.AddActivityListener(activityListener);

        await using var dataSource = CreateDataSource();
        await using var conn = async
                ? await dataSource.OpenConnectionAsync()
                : dataSource.OpenConnection();

        Assert.That(activities.Count, Is.EqualTo(1));
        ValidateActivity(activities[0], conn, IsMultiplexing);

        if (!IsMultiplexing)
            return;

        activities.Clear();

        // For multiplexing, we clear the pool to force next query to open another physical connection
        dataSource.Clear();

        await conn.ExecuteScalarAsync("SELECT 1");

        Assert.That(activities.Count, Is.EqualTo(2));
        ValidateActivity(activities[0], conn, IsMultiplexing);

        // For multiplexing, query's activity can be considered as a parent for physical open's activity
        Assert.That(activities[0].Parent, Is.SameAs(activities[1]));

        static void ValidateActivity(Activity activity, NpgsqlConnection conn, bool isMultiplexing)
        {
            Assert.That(activity.DisplayName, Is.EqualTo(conn.Settings.Database));
            Assert.That(activity.OperationName, Is.EqualTo(conn.Settings.Database));
            Assert.That(activity.Status, Is.EqualTo(ActivityStatusCode.Ok));

            Assert.That(activity.Events.Count(), Is.EqualTo(0));

            var expectedTagCount = conn.Settings.Port == 5432 ? 8 : 9;
            Assert.That(activity.TagObjects.Count(), Is.EqualTo(expectedTagCount));

            Assert.IsFalse(activity.TagObjects.Any(x => x.Key == "db.statement"));

            var systemTag = activity.TagObjects.First(x => x.Key == "db.system");
            Assert.That(systemTag.Value, Is.EqualTo("postgresql"));

            var userTag = activity.TagObjects.First(x => x.Key == "db.user");
            Assert.That(userTag.Value, Is.EqualTo(conn.Settings.Username));

            var dbNameTag = activity.TagObjects.First(x => x.Key == "db.name");
            Assert.That(dbNameTag.Value, Is.EqualTo(conn.Settings.Database));

            var connStringTag = activity.TagObjects.First(x => x.Key == "db.connection_string");
            Assert.That(connStringTag.Value, Is.EqualTo(conn.ConnectionString));

            if (!isMultiplexing)
            {
                var connIDTag = activity.TagObjects.First(x => x.Key == "db.connection_id");
                Assert.That(connIDTag.Value, Is.EqualTo(conn.ProcessID));
            }
            else
                Assert.IsTrue(activity.TagObjects.Any(x => x.Key == "db.connection_id"));
        }
    }

    [Test]
    public async Task Basic_query([Values] bool async, [Values] bool batch)
    {
        if (IsMultiplexing && !async)
            return;

        var activities = new List<Activity>();

        using var activityListener = new ActivityListener();
        activityListener.ShouldListenTo = source => source.Name == "Npgsql";
        activityListener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded;
        activityListener.ActivityStopped = activity => activities.Add(activity);
        ActivitySource.AddActivityListener(activityListener);

        await using var dataSource = CreateDataSource();
        await using var conn = await dataSource.OpenConnectionAsync();

        // We're not interested in physical open's activity
        Assert.That(activities.Count, Is.EqualTo(1));
        activities.Clear();

        await ExecuteScalar(conn, async, batch, "SELECT 42");

        Assert.That(activities.Count, Is.EqualTo(1));
        var activity = activities[0];
        Assert.That(activity.DisplayName, Is.EqualTo(conn.Settings.Database));
        Assert.That(activity.OperationName, Is.EqualTo(conn.Settings.Database));
        Assert.That(activity.Status, Is.EqualTo(ActivityStatusCode.Ok));

        Assert.That(activity.Events.Count(), Is.EqualTo(1));
        var firstResponseEvent = activity.Events.First();
        Assert.That(firstResponseEvent.Name, Is.EqualTo("received-first-response"));

        var expectedTagCount = conn.Settings.Port == 5432 ? 9 : 10;
        Assert.That(activity.TagObjects.Count(), Is.EqualTo(expectedTagCount));

        var queryTag = activity.TagObjects.First(x => x.Key == "db.statement");
        Assert.That(queryTag.Value, Is.EqualTo("SELECT 42"));

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
            Assert.IsTrue(activity.TagObjects.Any(x => x.Key == "db.connection_id"));
    }

    [Test]
    public async Task Error_open([Values] bool async)
    {
        if (IsMultiplexing && !async)
            return;

        var activities = new List<Activity>();

        using var activityListener = new ActivityListener();
        activityListener.ShouldListenTo = source => source.Name == "Npgsql";
        activityListener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded;
        activityListener.ActivityStopped = activity => activities.Add(activity);
        ActivitySource.AddActivityListener(activityListener);

        await using var dataSource = CreateDataSource(x => x.Host = "not-existing-host");
        var ex = Assert.ThrowsAsync<SocketException>(async () =>
        {
            await using var conn = async
                ? await dataSource.OpenConnectionAsync()
                : dataSource.OpenConnection();
        })!;

        Assert.That(activities.Count, Is.EqualTo(1));
        var activity = activities[0];
        Assert.That(activity.DisplayName, Is.EqualTo(dataSource.Settings.Database));
        Assert.That(activity.OperationName, Is.EqualTo(dataSource.Settings.Database));
        Assert.That(activity.Status, Is.EqualTo(ActivityStatusCode.Error));
        Assert.That(activity.StatusDescription, Is.EqualTo(ex.Message));

        Assert.That(activity.Events.Count(), Is.EqualTo(1));
        var exceptionEvent = activity.Events.First();
        Assert.That(exceptionEvent.Name, Is.EqualTo("exception"));

        Assert.That(exceptionEvent.Tags.Count(), Is.EqualTo(4));

        var exceptionTypeTag = exceptionEvent.Tags.First(x => x.Key == "exception.type");
        Assert.That(exceptionTypeTag.Value, Is.EqualTo(ex.GetType().FullName));

        var exceptionMessageTag = exceptionEvent.Tags.First(x => x.Key == "exception.message");
        StringAssert.Contains(ex.Message, (string)exceptionMessageTag.Value!);

        var exceptionStacktraceTag = exceptionEvent.Tags.First(x => x.Key == "exception.stacktrace");
        StringAssert.Contains(ex.Message, (string)exceptionStacktraceTag.Value!);

        var exceptionEscapedTag = exceptionEvent.Tags.First(x => x.Key == "exception.escaped");
        Assert.That(exceptionEscapedTag.Value, Is.True);

        Assert.That(activity.TagObjects.Count(), Is.EqualTo(2));

        var systemTag = activity.TagObjects.First(x => x.Key == "db.system");
        Assert.That(systemTag.Value, Is.EqualTo("postgresql"));

        var connStringTag = activity.TagObjects.First(x => x.Key == "db.connection_string");
        Assert.That(connStringTag.Value, Is.EqualTo(dataSource.ConnectionString));
    }

    [Test]
    public async Task Error_query([Values] bool async, [Values] bool batch)
    {
        if (IsMultiplexing && !async)
            return;

        var activities = new List<Activity>();

        using var activityListener = new ActivityListener();
        activityListener.ShouldListenTo = source => source.Name == "Npgsql";
        activityListener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded;
        activityListener.ActivityStopped = activity => activities.Add(activity);
        ActivitySource.AddActivityListener(activityListener);

        await using var dataSource = CreateDataSource();
        await using var conn = await dataSource.OpenConnectionAsync();

        // We're not interested in physical open's activity
        Assert.That(activities.Count, Is.EqualTo(1));
        activities.Clear();

        Assert.ThrowsAsync<PostgresException>(async () => await ExecuteScalar(conn, async, batch, "SELECT * FROM non_existing_table"));

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
        Assert.That(exceptionTypeTag.Value, Is.EqualTo("Npgsql.PostgresException"));

        var exceptionMessageTag = exceptionEvent.Tags.First(x => x.Key == "exception.message");
        StringAssert.Contains("relation \"non_existing_table\" does not exist", (string)exceptionMessageTag.Value!);

        var exceptionStacktraceTag = exceptionEvent.Tags.First(x => x.Key == "exception.stacktrace");
        StringAssert.Contains("relation \"non_existing_table\" does not exist", (string)exceptionStacktraceTag.Value!);

        var exceptionEscapedTag = exceptionEvent.Tags.First(x => x.Key == "exception.escaped");
        Assert.That(exceptionEscapedTag.Value, Is.True);

        var expectedTagCount = conn.Settings.Port == 5432 ? 9 : 10;
        Assert.That(activity.TagObjects.Count(), Is.EqualTo(expectedTagCount));

        var queryTag = activity.TagObjects.First(x => x.Key == "db.statement");
        Assert.That(queryTag.Value, Is.EqualTo("SELECT * FROM non_existing_table"));

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
            Assert.IsTrue(activity.TagObjects.Any(x => x.Key == "db.connection_id"));
    }

    [Test]
    public async Task Configure_tracing([Values] bool async, [Values] bool batch)
    {
        if (IsMultiplexing && !async)
            return;

        var activities = new List<Activity>();

        using var activityListener = new ActivityListener();
        activityListener.ShouldListenTo = source => source.Name == "Npgsql";
        activityListener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded;
        activityListener.ActivityStopped = activity => activities.Add(activity);
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

        Assert.That(activities.Count, Is.EqualTo(0));

        await ExecuteScalar(conn, async, batch, "SELECT 2");

        Assert.That(activities.Count, Is.EqualTo(1));
        var activity = activities[0];
        Assert.That(activity.DisplayName, Is.EqualTo("unknown_query"));
        Assert.That(activity.OperationName, Is.EqualTo("unknown_query"));

        Assert.That(activity.Events.Count(), Is.EqualTo(0));

        var customTag = activity.TagObjects.First(x => x.Key == "custom_tag");
        Assert.That(customTag.Value, Is.EqualTo("custom_value"));
    }

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
