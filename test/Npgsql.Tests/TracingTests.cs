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
    public async Task Basic([Values] bool async)
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
        if (async)
            await conn.ExecuteScalarAsync("SELECT 42");
        else
            conn.ExecuteScalar("SELECT 42");

        Assert.That(activities.Count, Is.EqualTo(1));
        var activity = activities[0];
        Assert.That(activity.DisplayName, Is.EqualTo(conn.Settings.Database));
        Assert.That(activity.OperationName, Is.EqualTo(conn.Settings.Database));
        // TODO: set status code?
        Assert.That(activity.Status, Is.EqualTo(ActivityStatusCode.Unset));

        Assert.That(activity.Events.Count(), Is.EqualTo(1));
        var firstResponseEvent = activity.Events.First();
        Assert.That(firstResponseEvent.Name, Is.EqualTo("received-first-response"));

        var expectedTagCount = conn.Settings.Port == 5432 ? 10 : 11;
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

        var statusTag = activity.TagObjects.First(x => x.Key == "otel.status_code");
        Assert.That(statusTag.Value, Is.EqualTo("OK"));
    }

    [Test]
    public async Task Error([Values] bool async)
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
        if (async)
            Assert.ThrowsAsync<PostgresException>(async () => await conn.ExecuteScalarAsync("SELECT * FROM non_existing_table"));
        else
            Assert.Throws<PostgresException>(() => conn.ExecuteScalar("SELECT * FROM non_existing_table"));

        Assert.That(activities.Count, Is.EqualTo(1));
        var activity = activities[0];
        Assert.That(activity.DisplayName, Is.EqualTo(conn.Settings.Database));
        Assert.That(activity.OperationName, Is.EqualTo(conn.Settings.Database));
        // TODO: set status code?
        Assert.That(activity.Status, Is.EqualTo(ActivityStatusCode.Unset));

        Assert.That(activity.Events.Count(), Is.EqualTo(1));
        var exceptionEvent = activity.Events.First();
        Assert.That(exceptionEvent.Name, Is.EqualTo("exception"));

        Assert.That(exceptionEvent.Tags.Count(), Is.EqualTo(4));

        var exceptionTypeTag = exceptionEvent.Tags.First(x => x.Key == "exception.type");
        Assert.That(exceptionTypeTag.Value, Is.EqualTo("Npgsql.PostgresException"));

        var exceptionMessageTag = exceptionEvent.Tags.First(x => x.Key == "exception.message");
        StringAssert.Contains("relation \"non_existing_table\" does not exist", (string)exceptionMessageTag.Value!);

        var exceptionStacktraceTag = exceptionEvent.Tags.First(x => x.Key == "exception.stacktrace");
        // TODO: StackTrace shouldn't contain exception message?
        StringAssert.Contains("relation \"non_existing_table\" does not exist", (string)exceptionStacktraceTag.Value!);

        var exceptionEscapedTag = exceptionEvent.Tags.First(x => x.Key == "exception.escaped");
        Assert.That(exceptionEscapedTag.Value, Is.True);

        var expectedTagCount = conn.Settings.Port == 5432 ? 11 : 12;
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

        var statusTag = activity.TagObjects.First(x => x.Key == "otel.status_code");
        Assert.That(statusTag.Value, Is.EqualTo("ERROR"));

        var descriptionTag = activity.TagObjects.First(x => x.Key == "otel.status_description");
        Assert.That(descriptionTag.Value, Is.EqualTo(PostgresErrorCodes.UndefinedTable));
    }

    [Test]
    public async Task Configure_tracing([Values] bool async)
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
                .EnableFirstResponseEvent(enable: false)
                .ConfigureCommandFilter(cmd => cmd.CommandText.Contains('2'))
                .ConfigureCommandSpanNameProvider(_ => "unknown_query")
                .ConfigureCommandEnrichmentCallback((activity, _) => activity.AddTag("custom_tag", "custom_value"));
        });
        await using var dataSource = dataSourceBuilder.Build();
        await using var conn = await dataSource.OpenConnectionAsync();

        if (async)
            await conn.ExecuteScalarAsync("SELECT 1");
        else
            conn.ExecuteScalar("SELECT 1");

        Assert.That(activities.Count, Is.EqualTo(0));

        if (async)
            await conn.ExecuteScalarAsync("SELECT 2");
        else
            conn.ExecuteScalar("SELECT 2");

        Assert.That(activities.Count, Is.EqualTo(1));
        var activity = activities[0];
        Assert.That(activity.DisplayName, Is.EqualTo("unknown_query"));
        Assert.That(activity.OperationName, Is.EqualTo("unknown_query"));

        Assert.That(activity.Events.Count(), Is.EqualTo(0));

        var customTag = activity.TagObjects.First(x => x.Key == "custom_tag");
        Assert.That(customTag.Value, Is.EqualTo("custom_value"));
    }
}
