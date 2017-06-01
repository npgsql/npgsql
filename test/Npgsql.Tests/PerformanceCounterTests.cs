#region License
// The PostgreSQL License
//
// Copyright (C) 2017 The Npgsql Development Team
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
#endregion

#if NET451
using System;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;

namespace Npgsql.Tests
{
    [Parallelizable(ParallelScope.None)]
    [Explicit]
    public class PerformanceCounterTests : TestBase
    {
        [Test]
        public void HardConnectsAndDisconnectsPerSecond()
        {
            var hardConnectCounter = Counters.HardConnectsPerSecond.DiagnosticsCounter;
            var hardDisconnectCounter = Counters.HardDisconnectsPerSecond.DiagnosticsCounter;
            var softConnectCounter = Counters.SoftConnectsPerSecond.DiagnosticsCounter;
            var softDisconnectCounter = Counters.SoftDisconnectsPerSecond.DiagnosticsCounter;

            var nonPooledConnString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                Pooling = false
            }.ToString();
            using (var pooledConn = new NpgsqlConnection(ConnectionString))
            using (var nonPooledConn = new NpgsqlConnection(nonPooledConnString))
            {
                Thread.Sleep(2000);  // Let the counter reset
                Assert.That(hardConnectCounter.RawValue, Is.Zero);
                Assert.That(hardDisconnectCounter.RawValue, Is.Zero);
                Assert.That(softConnectCounter.RawValue, Is.Zero);
                Assert.That(softDisconnectCounter.RawValue, Is.Zero);
                pooledConn.Open();   // Pool is empty so this is a hard connect
                nonPooledConn.Open();
                Assert.That(hardConnectCounter.RawValue, Is.EqualTo(2));
                Assert.That(hardDisconnectCounter.RawValue, Is.Zero);
                Assert.That(softConnectCounter.RawValue, Is.EqualTo(1));
                Assert.That(softDisconnectCounter.RawValue, Is.Zero);
                pooledConn.Close();
                nonPooledConn.Close();
                // The non-pooled was a hard disconnect, the pooled was a soft
                Assert.That(hardConnectCounter.RawValue, Is.EqualTo(2));
                Assert.That(hardDisconnectCounter.RawValue, Is.EqualTo(1));
                Assert.That(softConnectCounter.RawValue, Is.EqualTo(1));
                Assert.That(softDisconnectCounter.RawValue, Is.EqualTo(1));
                pooledConn.Open();
                Assert.That(hardConnectCounter.RawValue, Is.EqualTo(2));
                Assert.That(hardDisconnectCounter.RawValue, Is.EqualTo(1));
                Assert.That(softConnectCounter.RawValue, Is.EqualTo(2));
                Assert.That(softDisconnectCounter.RawValue, Is.EqualTo(1));
                pooledConn.Close();
                Assert.That(hardConnectCounter.RawValue, Is.EqualTo(2));
                Assert.That(hardDisconnectCounter.RawValue, Is.EqualTo(1));
                Assert.That(softConnectCounter.RawValue, Is.EqualTo(2));
                Assert.That(softDisconnectCounter.RawValue, Is.EqualTo(2));
            }
        }

        [Test]
        public void NumberOfNonPooledConnections()
        {
            var counter = Counters.NumberOfNonPooledConnections.DiagnosticsCounter;

            var nonPooledConnString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                Pooling = false
            }.ToString();
            using (var pooledConn = new NpgsqlConnection(ConnectionString))
            using (var nonPooledConn = new NpgsqlConnection(nonPooledConnString))
            {
                Assert.That(counter.RawValue, Is.Zero);
                nonPooledConn.Open();
                Assert.That(counter.RawValue, Is.EqualTo(1));
                pooledConn.Open();
                Assert.That(counter.RawValue, Is.EqualTo(1));
                nonPooledConn.Close();
                Assert.That(counter.RawValue, Is.Zero);
                pooledConn.Close();
                Assert.That(counter.RawValue, Is.Zero);
            }
        }

        [Test]
        public void PooledConnections()
        {
            var totalCounter = Counters.NumberOfPooledConnections.DiagnosticsCounter;
            var activeCounter = Counters.NumberOfActiveConnections.DiagnosticsCounter;
            var freeCounter = Counters.NumberOfFreeConnections.DiagnosticsCounter;

            var nonPooledConnString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                Pooling = false
            }.ToString();
            using (var pooledConn = new NpgsqlConnection(ConnectionString))
            using (var nonPooledConn = new NpgsqlConnection(nonPooledConnString))
            {
                Assert.That(totalCounter.RawValue, Is.Zero);
                Assert.That(activeCounter.RawValue, Is.Zero);
                Assert.That(freeCounter.RawValue, Is.Zero);
                nonPooledConn.Open();
                Assert.That(totalCounter.RawValue, Is.Zero);
                Assert.That(activeCounter.RawValue, Is.Zero);
                Assert.That(freeCounter.RawValue, Is.Zero);
                pooledConn.Open();
                Assert.That(totalCounter.RawValue, Is.EqualTo(1));
                Assert.That(activeCounter.RawValue, Is.EqualTo(1));
                Assert.That(freeCounter.RawValue, Is.Zero);
                nonPooledConn.Close();
                Assert.That(totalCounter.RawValue, Is.EqualTo(1));
                Assert.That(activeCounter.RawValue, Is.EqualTo(1));
                Assert.That(freeCounter.RawValue, Is.Zero);
                pooledConn.Close();
                Assert.That(totalCounter.RawValue, Is.EqualTo(1));
                Assert.That(activeCounter.RawValue, Is.Zero);
                Assert.That(freeCounter.RawValue, Is.EqualTo(1));
            }
            NpgsqlConnection.ClearAllPools();
            Assert.That(totalCounter.RawValue, Is.Zero);
            Assert.That(activeCounter.RawValue, Is.Zero);
            Assert.That(freeCounter.RawValue, Is.Zero);
        }

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var countersExist = false;
            try
            {
                countersExist = PerformanceCounterCategory.Exists(Counter.DiagnosticsCounterCategory);
            }
            catch (UnauthorizedAccessException) {}

            Assert.That(countersExist, "The Npgsql performance counters haven't been created");

            var perfCtrSwitch = new TraceSwitch("ConnectionPoolPerformanceCounterDetail", "level of detail to track with connection pool performance counters");
            var verboseCounters = perfCtrSwitch.Level == TraceLevel.Verbose;

            if (!verboseCounters)
                Assert.Fail("Verbose performance counters not enabled");
        }

        [SetUp]
        public void Setup()
        {
            NpgsqlConnection.ClearAllPools();
        }
    }
}
#endif
