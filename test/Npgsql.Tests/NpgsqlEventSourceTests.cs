using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using NUnit.Framework;

namespace Npgsql.Tests
{
    [NonParallelizable]
    public class NpgsqlEventSourceTests : TestBase
    {
        [Test]
        public void CommandStartStop()
        {
            using (var conn = OpenConnection())
            {
                // There is a new pool created, which sends a few queries to load pg types
                ClearEvents();
                conn.ExecuteScalar("SELECT 1");
            }

            var commandStart = _events.Single(e => e.EventId == NpgsqlEventSource.CommandStartId);
            Assert.That(commandStart.EventName, Is.EqualTo("CommandStart"));

            var commandStop = _events.Single(e => e.EventId == NpgsqlEventSource.CommandStopId);
            Assert.That(commandStop.EventName, Is.EqualTo("CommandStop"));
        }

        [OneTimeSetUp]
        public void EnableEventSource()
        {
            _listener = new TestEventListener(_events);
            _listener.EnableEvents(NpgsqlSqlEventSource.Log, EventLevel.Informational);
        }

        [OneTimeTearDown]
        public void DisableEventSource()
        {
            _listener.DisableEvents(NpgsqlSqlEventSource.Log);
            _listener.Dispose();
        }

        [SetUp]
        public void ClearEvents() => _events.Clear();

        TestEventListener _listener = null!;

        readonly List<EventWrittenEventArgs> _events = new List<EventWrittenEventArgs>();

        class TestEventListener : EventListener
        {
            readonly List<EventWrittenEventArgs> _events;
            public TestEventListener(List<EventWrittenEventArgs> events) => _events = events;
            protected override void OnEventWritten(EventWrittenEventArgs eventData) => _events.Add(eventData);
        }
    }
}
