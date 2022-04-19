using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace Npgsql.Tests.OpenTelemetry;

[NonParallelizable]
public class RecordCommandExecutionEventTests : TestBase
{
    [TestCase(true)]
    [TestCase(false)]
    public void CommandExecutes_records_firstresponse(bool recordException)
    {
        var activities = SetUp(o =>
        {
            o.RecordCommandExecutionException = recordException;
        });

        using (var conn = OpenConnection())
        {
            conn.ExecuteScalar("SELECT 1");
        }

        Assert.That(activities, Has.Count.EqualTo(1));

        var activityEvents = activities[0].Events.ToList();
        Assert.That(activityEvents, Has.Count.EqualTo(1));

        var activityEvent = activityEvents[0];
        Assert.That(activityEvent.Name, Is.EqualTo("received-first-response"));
    }

    [TestCase(true)]
    [TestCase(false)]
    public void CommandExecutes_does_not_record_firstresponse(bool recordException)
    {
        var activities = SetUp(o =>
        {
            o.RecordCommandExecutionFirstResponse = false;
            o.RecordCommandExecutionException = recordException;
        });

        using (var conn = OpenConnection())
        {
            conn.ExecuteScalar("SELECT 1");
        }

        Assert.That(activities, Has.Count.EqualTo(1));

        var activityEvents = activities[0].Events.ToList();
        Assert.That(activityEvents, Is.Empty);
    }

    [TestCase(true)]
    [TestCase(false)]
    public void CommandThrows_records_exception(bool recordFirstResponse)
    {
        var activities = SetUp(o =>
        {
            o.RecordCommandExecutionFirstResponse = recordFirstResponse;
        });

        var exception = Assert.Throws<PostgresException>(() =>
        {
            using var conn = OpenConnection();
            conn.ExecuteScalar("BO SELECTA");
        });

        Assert.That(activities, Has.Count.EqualTo(1));

        var activityEvents = activities[0].Events.ToList();
        Assert.That(activityEvents, Has.Count.EqualTo(1));

        var activityEvent = activityEvents[0];
        Assert.That(activityEvent.Name, Is.EqualTo("exception"));

        var activityEventTags = activityEvent.Tags.ToDictionary(x => x.Key, x => x.Value);
        Assert.That(activityEventTags, Contains.Key("exception.type").WithValue($"{nameof(Npgsql)}.{nameof(PostgresException)}"));
        Assert.That(activityEventTags, Contains.Key("exception.message").WithValue(exception!.Message));
        Assert.That(activityEventTags, Contains.Key("exception.stacktrace"));
    }

    [TestCase(true)]
    [TestCase(false)]
    public void CommandThrows_does_not_record_exception(bool recordFirstResponse)
    {
        var activities = SetUp(o =>
        {
            o.RecordCommandExecutionException = false;
            o.RecordCommandExecutionFirstResponse = recordFirstResponse;
        });

        var exception = Assert.Throws<PostgresException>(() =>
        {
            using var conn = OpenConnection();
            conn.ExecuteScalar("BO SELECTA");
        });

        Assert.That(activities, Has.Count.EqualTo(1));

        var activityEvents = activities[0].Events.ToList();
        Assert.That(activityEvents, Is.Empty);
    }

    IReadOnlyList<Activity> SetUp(Action<NpgsqlTracingOptions> configure)
    {
        var collector = new ActivityCollector();
        _tracerProvider = Sdk.CreateTracerProviderBuilder()
            .AddNpgsql(configure)
            .AddProcessor(collector)
            .Build();
        return collector.Activities;
    }

    [TearDown]
    public void TearDown() => _tracerProvider?.Dispose();

    TracerProvider _tracerProvider = null!;

    class ActivityCollector : BaseProcessor<Activity>
    {
        readonly List<Activity> _activities = new ();

        public IReadOnlyList<Activity> Activities => _activities;

        public override void OnStart(Activity activity) => _activities.Add(activity);
    }
}
