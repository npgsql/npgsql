using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            Assert.That(activity.DisplayName, Is.EqualTo("connect " + conn.Settings.Database));
            Assert.That(activity.OperationName, Is.EqualTo("connect " + conn.Settings.Database));
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
        Assert.That(activity.DisplayName, Is.EqualTo("connect " + dataSource.Settings.Database));
        Assert.That(activity.OperationName, Is.EqualTo("connect " + dataSource.Settings.Database));
        Assert.That(activity.Status, Is.EqualTo(ActivityStatusCode.Error));
        Assert.That(activity.StatusDescription, Is.EqualTo(ex.Message));

        Assert.That(activity.Events.Count(), Is.EqualTo(1));
        var exceptionEvent = activity.Events.First();
        Assert.That(exceptionEvent.Name, Is.EqualTo("exception"));

        var exceptionTags = exceptionEvent.Tags.ToDictionary(t => t.Key, t => t.Value);
        Assert.That(exceptionTags, Has.Count.EqualTo(4));

        Assert.That(exceptionTags["exception.type"], Is.EqualTo(ex.GetType().FullName));
        Assert.That(exceptionTags["exception.message"], Does.Contain(ex.Message));
        Assert.That(exceptionTags["exception.stacktrace"], Does.Contain(ex.Message));
        Assert.That(exceptionTags["exception.escaped"], Is.True);

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

        await using var dataSource = CreateDataSource();
        await using var conn = await dataSource.OpenConnectionAsync();

        // We're not interested in physical open's activity
        Assert.That(activities.Count, Is.EqualTo(1));
        activities.Clear();

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
        Assert.That(exceptionTags, Has.Count.EqualTo(4));

        Assert.That(exceptionTags["exception.type"], Is.EqualTo("Npgsql.PostgresException"));
        Assert.That(exceptionTags["exception.message"], Does.Contain("relation \"non_existing_table\" does not exist"));
        Assert.That(exceptionTags["exception.stacktrace"], Does.Contain("relation \"non_existing_table\" does not exist"));
        Assert.That(exceptionTags["exception.escaped"], Is.True);

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
