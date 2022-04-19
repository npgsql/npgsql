using System;
using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace Npgsql.Tests.OpenTelemetry;

[NonParallelizable]
[TestFixture(false, false)]
[TestFixture(false, true)]
[TestFixture(true, false)]
[TestFixture(true, true)]
public class EnrichCommandExecutionTests : TestBase
{
    [Test]
    public void CommandExecutes_invokes_start_firstresponse_stop()
    {
        using (var conn = OpenConnection())
        {
            conn.ExecuteScalar("SELECT 1");
        }

        Assert.That(_enrichInvocations, Has.Count.EqualTo(3));

        var (startActivity, startEventName, startObject) = _enrichInvocations[0];
        Assert.That(startEventName, Is.EqualTo("OnStartActivity"));
        Assert.That(startObject, Is.TypeOf<NpgsqlCommand>().With.Property("CommandText").EqualTo("SELECT 1"));
        Assert.That(startActivity.Kind, Is.EqualTo(ActivityKind.Client));

        var (responseActivity, responseEventName, responseObject) = _enrichInvocations[1];
        Assert.That(responseEventName, Is.EqualTo("OnFirstResponse"));
        Assert.That(responseObject, Is.SameAs(startObject));
        Assert.That(responseActivity, Is.SameAs(startActivity));

        var (stopActivity, stopEventName, stopObject) = _enrichInvocations[2];
        Assert.That(stopEventName, Is.EqualTo("OnStopActivity"));
        Assert.That(stopObject, Is.SameAs(startObject));
        Assert.That(stopActivity, Is.SameAs(startActivity));
    }

    [Test]
    public void CommandThrows_invokes_start_exception()
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
        Assert.That(stopObject, Is.TypeOf<ValueTuple<NpgsqlCommand, Exception>>());
        var (stopCommand, stopException) = (ValueTuple<NpgsqlCommand, Exception>)stopObject;
        Assert.That(stopCommand.CommandText, Is.EqualTo("BO SELECTA"));
        Assert.That(stopException, Is.SameAs(exception));
        Assert.That(stopActivity, Is.SameAs(startActivity));
    }

    [Test]
    public void CommandThrows_invokes_start_exception_patternmatch()
    {
        var exception = Assert.Throws<PostgresException>(() =>
        {
            using var conn = OpenConnection();
            conn.ExecuteScalar("BO SELECTA");
        });

        Assert.That(_enrichInvocations, Has.Count.EqualTo(2));
        var (_, stopEventName, stopObject) = _enrichInvocations[1];

        switch (stopEventName, stopObject)
        {
        case ("OnException", (NpgsqlCommand stopCommand, Exception stopException)):
            Assert.That(stopCommand.CommandText, Is.EqualTo("BO SELECTA"));
            Assert.That(stopException, Is.SameAs(exception));
            break;
        default:
            Assert.Fail($"{nameof(stopEventName)}: '{stopEventName}', {nameof(stopObject)}.GetType(): '{stopObject.GetType()}'");
            break;
        }
    }

    [SetUp]
    public void SetUp()
    {
        _enrichInvocations.Clear();
        _tracerProvider = Sdk.CreateTracerProviderBuilder()
            .AddNpgsql(o =>
            {
                o.EnrichCommandExecution = (activity, eventName, rawObject) => _enrichInvocations.Add((activity, eventName, rawObject));
                o.RecordCommandExecutionException = _recordException;
                o.RecordCommandExecutionFirstResponse = _recordFirstResponse;
            })
            .Build();
    }

    [TearDown]
    public void TearDown() => _tracerProvider.Dispose();

    TracerProvider _tracerProvider = null!;

    readonly List<(Activity activity, string eventName, object rawObject)> _enrichInvocations = new();

    readonly bool _recordException;
    readonly bool _recordFirstResponse;

    public EnrichCommandExecutionTests(bool recordException, bool recordFirstResponse)
    {
        _recordException = recordException;
        _recordFirstResponse = recordFirstResponse;
    }
}
