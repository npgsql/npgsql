#region License
// The PostgreSQL License
//
// Copyright (C) 2015 The Npgsql Development Team
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

using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Npgsql.Tests
{
    [NonParallelizable]
    class PoolTests : TestBase
    {
        [Test]
        public void MinPoolSizeEqualsMaxPoolSize()
        {
            using (var conn = new NpgsqlConnection(new NpgsqlConnectionStringBuilder(ConnectionString) {
                MinPoolSize = 30,
                MaxPoolSize = 30
            }.ToString()))
            {
                conn.Open();
            }
        }

        [Test]
        public void MinPoolSizeLargerThanMaxPoolSize()
        {
            var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                MinPoolSize = 2,
                MaxPoolSize = 1
            }.ToString();

            Assert.That(() => new NpgsqlConnection(connString), Throws.Exception.TypeOf<ArgumentException>());
        }

        [Test]
        public void MinPoolSizeLargerThanPoolSizeLimit()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString);
            Assert.That(() => csb.MinPoolSize = ConnectorPool.PoolSizeLimit + 1, Throws.Exception.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void ReuseConnectorBeforeCreatingNew()
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
                var backendId = conn.Connector.BackendProcessId;
                conn.Close();
                conn.Open();
                Assert.That(conn.Connector.BackendProcessId, Is.EqualTo(backendId));
            }
        }

        [Test, Timeout(10000)]
        public void GetConnectorFromExhaustedPool()
        {
            var connString = new NpgsqlConnectionStringBuilder(ConnectionString) {
                MaxPoolSize = 1,
                Timeout = 0
            }.ToString();

            using (var conn1 = new NpgsqlConnection(connString))
            {
                conn1.Open();

                // Pool is exhausted
                using (var conn2 = new NpgsqlConnection(connString))
                {
                    new Timer(o => conn1.Close(), null, 1000, Timeout.Infinite);
                    conn2.Open();
                }
            }
        }

        //[Test, Explicit, Timeout(10000)]
        public async Task GetConnectorFromExhaustedPoolAsync()
        {
            var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                MaxPoolSize = 1,
                Timeout = 0
            }.ToString();

            using (var conn1 = new NpgsqlConnection(connString))
            {
                await conn1.OpenAsync();

                // Pool is exhausted
                using (var conn2 = new NpgsqlConnection(connString))
                using (new Timer(o => conn1.Close(), null, 1000, Timeout.Infinite))
                    await conn2.OpenAsync();
            }
        }

        [Test]
        public void TimeoutGettingConnectorFromExhaustedPool()
        {
            var connString = new NpgsqlConnectionStringBuilder(ConnectionString) {
                MaxPoolSize = 1,
                Timeout = 2
            }.ToString();

            using (var conn1 = new NpgsqlConnection(connString))
            {
                conn1.Open();
                // Pool is exhausted
                using (var conn2 = new NpgsqlConnection(connString))
                    Assert.That(() => conn2.Open(), Throws.Exception.TypeOf<NpgsqlException>());
            }
            // conn1 should now be back in the pool as idle
            using (var conn3 = new NpgsqlConnection(connString))
                conn3.Open();
        }

        [Test]
        public async Task TimeoutGettingConnectorFromExhaustedPoolAsync()
        {
            var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                MaxPoolSize = 1,
                Timeout = 2
            }.ToString();

            using (var conn1 = new NpgsqlConnection(connString))
            {
                await conn1.OpenAsync();

                // Pool is exhausted
                using (var conn2 = new NpgsqlConnection(connString))
                    Assert.That(async () => await conn2.OpenAsync(), Throws.Exception.TypeOf<NpgsqlException>());
            }
            // conn1 should now be back in the pool as idle
            using (var conn3 = new NpgsqlConnection(connString))
                conn3.Open();
        }

        //[Test, Timeout(10000)]
        //[Explicit("Timing-based")]
        public async Task CancelOpenAsync()
        {
            var connString = new NpgsqlConnectionStringBuilder(ConnectionString) {
                ApplicationName = nameof(CancelOpenAsync),
                MaxPoolSize = 1,
            }.ToString();

            using (var conn1 = new NpgsqlConnection(connString))
            {
                await conn1.OpenAsync();

                Assert.True(PoolManager.TryGetValue(connString, out var pool));
                AssertPoolState(pool, 0, 1, 0);

                // Pool is exhausted
                using (var conn2 = new NpgsqlConnection(connString))
                {
                    var cts = new CancellationTokenSource(1000);
                    var openTask = conn2.OpenAsync(cts.Token);
                    AssertPoolState(pool, 0, 1, 1);
                    Assert.That(async () => await openTask, Throws.Exception.TypeOf<TaskCanceledException>());
                }

                // The cancelled open attempt should have left a cancelled task completion source
                // in the pool's wait queue. Close our busy connection and make sure everything work as planned.
                AssertPoolState(pool, 0, 1, 0);
                using (var conn2 = new NpgsqlConnection(connString))
                using (new Timer(o => conn1.Close(), null, 1000, Timeout.Infinite))
                {
                    await conn2.OpenAsync();
                    AssertPoolState(pool, 0, 1, 0);
                }
                AssertPoolState(pool, 1, 0, 0);
            }
        }

        [Test, Description("Makes sure that when a pooled connection is closed it's properly reset, and that parameter settings aren't leaked")]
        public void ResetOnClose()
        {
            var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                SearchPath = "public"
            }.ToString();
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                Assert.That(conn.ExecuteScalar("SHOW search_path"), Is.Not.Contains("pg_temp"));
                var backendId = conn.Connector.BackendProcessId;
                conn.ExecuteNonQuery("SET search_path=pg_temp");
                conn.Close();

                conn.Open();
                Assert.That(conn.Connector.BackendProcessId, Is.EqualTo(backendId));
                Assert.That(conn.ExecuteScalar("SHOW search_path"), Is.EqualTo("public"));
            }
        }

        //[Test, NonParallelizable]
        //[Explicit("Flaky, based on timing")]
        public void PruneIdleConnectors()
        {
            var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                ApplicationName = nameof(PruneIdleConnectors),
                MinPoolSize = 2,
                ConnectionIdleLifetime = 2,
                ConnectionPruningInterval = 1
            }.ToString();
            using (var conn1 = OpenConnection(connString))
            using (var conn2 = OpenConnection(connString))
            using (var conn3 = OpenConnection(connString))
            {
                Assert.True(PoolManager.TryGetValue(connString, out var pool));

                conn1.Close();
                conn2.Close();
                AssertPoolState(pool, 2, 1);

                Thread.Sleep(1500);

                // Pruning attempted, but ConnectionIdleLifetime not yet reached
                AssertPoolState(pool, 2, 1);

                Thread.Sleep(1500);

                // ConnectionIdleLifetime reached, but only one idle connection should be pruned (MinPoolSize=2)
                AssertPoolState(pool, 1, 1);
            }
        }

        [Test, Description("Makes sure that when a waiting async open is is given a connection, the continuation is executed in the TP rather than on the closing thread")]
        public void CloseReleasesWaiterOnAnotherThread()
        {
            var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                ApplicationName = nameof(CloseReleasesWaiterOnAnotherThread),
                MaxPoolSize = 1
            }.ToString();
            var conn1 = new NpgsqlConnection(connString);
            try
            {
                conn1.Open();   // Pool is now exhausted

                Assert.True(PoolManager.TryGetValue(connString, out var pool));
                AssertPoolState(pool, 0, 1, 0);

                Func<Task<int>> asyncOpener = async () =>
                {
                    using (var conn2 = new NpgsqlConnection(connString))
                    {
                        await conn2.OpenAsync();
                        AssertPoolState(pool, 0, 1, 0);
                        return Thread.CurrentThread.ManagedThreadId;
                    }
                };

                // Start an async open which will not complete as the pool is exhausted.
                var asyncOpenerTask = asyncOpener();
                AssertPoolState(pool, 0, 1, 1);
                conn1.Close();  // Complete the async open by closing conn1
                var asyncOpenerThreadId = asyncOpenerTask.Result;
                AssertPoolState(pool, 1, 0, 0);

                Assert.That(asyncOpenerThreadId, Is.Not.EqualTo(Thread.CurrentThread.ManagedThreadId));
            }
            finally
            {
                conn1.Close();
                NpgsqlConnection.ClearPool(conn1);
            }
        }

        [Test]
        public void ClearPool()
        {
            NpgsqlConnection conn;
            using (conn = OpenConnection()) {}
            // Now have one connection in the pool
            Assert.True(PoolManager.TryGetValue(ConnectionString, out var pool));
            AssertPoolState(pool, 1, 0);

            NpgsqlConnection.ClearPool(conn);
            AssertPoolState(pool, 0, 0);
        }

        [Test]
        public void ClearWithBusy()
        {
            ConnectorPool pool;
            using (var conn = OpenConnection())
            {
                NpgsqlConnection.ClearPool(conn);
                // conn is still busy but should get closed when returned to the pool

                Assert.True(PoolManager.TryGetValue(ConnectionString, out pool));
                AssertPoolState(pool, 0, 1);
            }
            AssertPoolState(pool, 0, 0);
        }

        [Test]
        public void ClearWithNoPool()
        {
            var connString = new NpgsqlConnectionStringBuilder(ConnectionString) {
                ApplicationName = nameof(ClearWithNoPool)
            }.ToString();
            using (var conn = new NpgsqlConnection(connString))
                NpgsqlConnection.ClearPool(conn);
        }

        [Test, Description("https://github.com/npgsql/npgsql/commit/45e33ecef21f75f51a625c7b919a50da3ed8e920#r28239653")]
        public void PhysicalOpenFailure()
        {
            var connString = new NpgsqlConnectionStringBuilder(ConnectionString) {
                ApplicationName = nameof(PhysicalOpenFailure),
                Port = 44444,
                MaxPoolSize = 1
            }.ToString();
            using (var conn = new NpgsqlConnection(connString))
            {
                for (var i = 0; i < 1; i++)
                    Assert.That(() => conn.Open(), Throws.Exception.TypeOf<SocketException>());
                Assert.True(PoolManager.TryGetValue(connString, out var pool));
                AssertPoolState(pool, 0, 0);
            }
        }

        //[Test, Explicit]
        //[TestCase(10, 10, 30, true)]
        //[TestCase(10, 10, 30, false)]
        //[TestCase(10, 20, 30, true)]
        //[TestCase(10, 20, 30, false)]
        public void ExercisePool(int maxPoolSize, int numTasks, int seconds, bool async)
        {
            var connString = new NpgsqlConnectionStringBuilder(ConnectionString) {
                ApplicationName = nameof(ExercisePool),
                MaxPoolSize = maxPoolSize
            }.ToString();

            Console.WriteLine($"Spinning up {numTasks} parallel tasks for {seconds} seconds (MaxPoolSize={maxPoolSize})...");
            StopFlag = 0;
            var tasks = Enumerable.Range(0, numTasks).Select(i => Task.Run(async () =>
            {
                while (StopFlag == 0)
                    using (var conn = new NpgsqlConnection(connString))
                    {
                        if (async)
                            await conn.OpenAsync();
                        else
                            conn.Open();
                    }
            })).ToArray();

            Thread.Sleep(seconds * 1000);
            Interlocked.Exchange(ref StopFlag, 1);
            Console.WriteLine("Stopped. Waiting for all tasks to stop...");
            Task.WaitAll(tasks);
            Console.WriteLine("Done");
        }

        volatile int StopFlag;

        void AssertPoolState(ConnectorPool pool, int idle, int busy, int waiting=0)
        {
            var state = pool.State;
            Assert.That(state.Idle, Is.EqualTo(idle), $"Idle should be {idle} but is {state.Idle}");
            Assert.That(state.Busy, Is.EqualTo(busy), $"Busy should be {busy} but is {state.Busy}");
            Assert.That(state.Waiting, Is.EqualTo(waiting), $"Waiting should be {waiting} but is {state.Waiting}");
        }
    }
}
