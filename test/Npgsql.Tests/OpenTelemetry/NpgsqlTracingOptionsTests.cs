using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace Npgsql.Tests.OpenTelemetry;

[NonParallelizable]
public class NpgsqlTracingOptionsTests : TestBase
{
    [Test]
    public void CommandExecution_start_stop()
    {
        using (var conn = OpenConnection())
        {
            conn.ExecuteScalar("SELECT 1");
        }

        Assert.That(_enrichInvocations, Has.Count.EqualTo(2));

        var (startActivity, startEventName, startObject) = _enrichInvocations[0];
        Assert.That(startEventName, Is.EqualTo("OnStartActivity"));
        Assert.That(startObject, Is.TypeOf<NpgsqlCommand>().With.Property("CommandText").EqualTo("SELECT 1"));
        Assert.That(startActivity.Kind, Is.EqualTo(ActivityKind.Client));

        var (stopActivity, stopEventName, stopObject) = _enrichInvocations[1];
        Assert.That(stopEventName, Is.EqualTo("OnStopActivity"));
        Assert.That(stopObject, Is.SameAs(startObject));
        Assert.That(stopActivity, Is.SameAs(startActivity));
    }

    [Test]
    public void CommandExecution_start_exception()
    {
        var exception = Assert.Throws<PostgresException>(() =>
        {
            using var conn = OpenConnection();
            conn.ExecuteScalar("BO SELECTA");
        });

        Assert.That(_enrichInvocations, Has.Count.EqualTo(2));

        var (startActivity, startEventName, startObject) = _enrichInvocations[0];
        Assert.That(startEventName, Is.EqualTo("OnStartActivity"));
        Assert.That(startObject, Is.TypeOf<NpgsqlCommand>().With.Property("CommandText").EqualTo("BO SELECTA"));
        Assert.That(startActivity.Kind, Is.EqualTo(ActivityKind.Client));

        var (stopActivity, stopEventName, stopObject) = _enrichInvocations[1];
        Assert.That(stopEventName, Is.EqualTo("OnException"));
        Assert.That(stopObject, Is.SameAs(exception));
        Assert.That(stopActivity, Is.SameAs(startActivity));
    }

    [SetUp]
    public void SetUp()
    {
        _enrichInvocations.Clear();
        _tracerProvider = Sdk.CreateTracerProviderBuilder()
            .AddNpgsql(o => o.EnrichCommandExecution = (activity, eventName, rawObject) => _enrichInvocations.Add((activity, eventName, rawObject)))
            .Build();
    }

    [TearDown]
    public void TearDown() => _tracerProvider.Dispose();

    TracerProvider _tracerProvider = null!;

    readonly List<(Activity activity, string eventName, object rawObject)> _enrichInvocations = new();
}
