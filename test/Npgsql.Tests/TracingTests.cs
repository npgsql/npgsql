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
        await using var conn = await OpenConnectionAsync();
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
}
