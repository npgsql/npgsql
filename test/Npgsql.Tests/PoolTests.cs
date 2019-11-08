using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Npgsql.Tests
{
    class PoolTests : TestBase
    {
        [Test]
        public void MinPoolSizeEqualsMaxPoolSize()
        {
            using (var conn = new NpgsqlConnection(new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                ApplicationName = nameof(MinPoolSizeEqualsMaxPoolSize),
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
                ApplicationName = nameof(MinPoolSizeLargerThanMaxPoolSize),
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
            var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                ApplicationName = nameof(ReuseConnectorBeforeCreatingNew),
            }.ToString();

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                var backendId = conn.Connector!.BackendProcessId;
                conn.Close();
                conn.Open();
                Assert.That(conn.Connector.BackendProcessId, Is.EqualTo(backendId));
            }
        }

        [Test, Timeout(10000)]
        public void GetConnectorFromExhaustedPool()
        {
            var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                ApplicationName = nameof(GetConnectorFromExhaustedPool),
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
                ApplicationName = nameof(GetConnectorFromExhaustedPoolAsync),
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
            var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                ApplicationName = nameof(TimeoutGettingConnectorFromExhaustedPool),
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
                ApplicationName = nameof(TimeoutGettingConnectorFromExhaustedPoolAsync),
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

        [Test, Timeout(10000)]
        //[Explicit("Timing-based")]
        public async Task CancelOpenAsync()
        {
            var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                ApplicationName = nameof(CancelOpenAsync),
                MaxPoolSize = 1,
            }.ToString();

            using (var conn1 = new NpgsqlConnection(connString))
            {
                await conn1.OpenAsync();

                Assert.True(PoolManager.TryGetValue(connString, out var pool));
                AssertPoolState(pool, 1, 0);

                // Pool is exhausted
                using (var conn2 = new NpgsqlConnection(connString))
                {
                    var cts = new CancellationTokenSource(1000);
                    var openTask = conn2.OpenAsync(cts.Token);
                    AssertPoolState(pool, 1, 0);
                    Assert.That(async () => await openTask, Throws.Exception.TypeOf<OperationCanceledException>());
                }

                // The cancelled open attempt should have left a cancelled task completion source
                // in the pool's wait queue. Close our busy connection and make sure everything work as planned.
                AssertPoolState(pool, 1, 0);
                using (var conn2 = new NpgsqlConnection(connString))
                using (new Timer(o => conn1.Close(), null, 1000, Timeout.Infinite))
                {
                    await conn2.OpenAsync();
                    AssertPoolState(pool, 1, 0);
                }
                AssertPoolState(pool, 1, 1);
            }
        }

        [Test, Description("Makes sure that when a pooled connection is closed it's properly reset, and that parameter settings aren't leaked")]
        public void ResetOnClose()
        {
            var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                ApplicationName = nameof(ResetOnClose),
                SearchPath = "public"
            }.ToString();
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                Assert.That(conn.ExecuteScalar("SHOW search_path"), Is.Not.Contains("pg_temp"));
                var backendId = conn.Connector!.BackendProcessId;
                conn.ExecuteNonQuery("SET search_path=pg_temp");
                conn.Close();

                conn.Open();
                Assert.That(conn.Connector.BackendProcessId, Is.EqualTo(backendId));
                Assert.That(conn.ExecuteScalar("SHOW search_path"), Is.EqualTo("public"));
            }
        }

        [Test, NonParallelizable]
        [Explicit("Flaky, based on timing")]
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
                AssertPoolState(pool!, 3, 2);

                Thread.Sleep(1500);

                // ConnectionIdleLifetime not yet reached.
                AssertPoolState(pool, 3, 2);

                Thread.Sleep(1500);

                // ConnectionIdleLifetime reached, one idle connection should be pruned (MinPoolSize=2)
                AssertPoolState(pool, 2, 1);
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
                AssertPoolState(pool, 1, 0);

                Func<Task<int>> asyncOpener = async () =>
                {
                    using (var conn2 = new NpgsqlConnection(connString))
                    {
                        await conn2.OpenAsync();
                        AssertPoolState(pool, 1, 0);
                        return Thread.CurrentThread.ManagedThreadId;
                    }
                };

                // Start an async open which will not complete as the pool is exhausted.
                var asyncOpenerTask = asyncOpener();
                AssertPoolState(pool, 1, 0);
                conn1.Close();  // Complete the async open by closing conn1
                var asyncOpenerThreadId = asyncOpenerTask.Result;
                AssertPoolState(pool, 1, 1);

                Assert.That(asyncOpenerThreadId, Is.Not.EqualTo(Thread.CurrentThread.ManagedThreadId));
            }
            finally
            {
                conn1.Close();
                NpgsqlConnection.ClearPool(conn1);
            }
        }

        [Test]
        public void ReleaseWaiterOnConnectionFailure()
        {
            var connectionString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                ApplicationName = nameof(ReleaseWaiterOnConnectionFailure),
                Port = 9999,
                MaxPoolSize = 1
            }.ToString();

            try
            {
                var tasks = Enumerable.Range(0, 2).Select(i => Task.Run(async () =>
                {
                    using var conn = new NpgsqlConnection(connectionString);
                    await conn.OpenAsync();
                })).ToArray();

                try
                {
                    Task.WaitAll(tasks);
                }
                catch (AggregateException e)
                {
                    foreach (var inner in e.InnerExceptions)
                        Assert.That(inner, Is.TypeOf<NpgsqlException>());
                    return;
                }
                Assert.Fail();
            }
            finally
            {
                NpgsqlConnection.ClearPool(new NpgsqlConnection(connectionString));
            }
        }

        [Test]
        public void ClearPool()
        {
            var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                ApplicationName = nameof(ClearPool)
            }.ToString();

            NpgsqlConnection conn;
            using (conn = OpenConnection(connString)) {}
            // Now have one connection in the pool
            Assert.True(PoolManager.TryGetValue(connString, out var pool));
            AssertPoolState(pool, 1, 1);

            NpgsqlConnection.ClearPool(conn);
            AssertPoolState(pool, 0, 0);
        }

        [Test]
        public void ClearWithBusy()
        {
            var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                ApplicationName = nameof(ClearWithBusy)
            }.ToString();

            ConnectorPool? pool;
            using (var conn = OpenConnection(connString))
            {
                NpgsqlConnection.ClearPool(conn);
                // conn is still busy but should get closed when returned to the pool

                Assert.True(PoolManager.TryGetValue(connString, out pool));
                AssertPoolState(pool, 1, 0);
            }
            AssertPoolState(pool, 0, 0);
        }

        [Test]
        public void ClearWithNoPool()
        {
            var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                ApplicationName = nameof(ClearWithNoPool)
            }.ToString();
            using (var conn = new NpgsqlConnection(connString))
                NpgsqlConnection.ClearPool(conn);
        }

        [Test, Description("https://github.com/npgsql/npgsql/commit/45e33ecef21f75f51a625c7b919a50da3ed8e920#r28239653")]
        public void PhysicalOpenFailure()
        {
            var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                ApplicationName = nameof(PhysicalOpenFailure),
                Port = 44444,
                MaxPoolSize = 1
            }.ToString();
            using (var conn = new NpgsqlConnection(connString))
            {
                for (var i = 0; i < 1; i++)
                    Assert.That(() => conn.Open(), Throws.Exception
                        .TypeOf<NpgsqlException>()
                        .With.InnerException.TypeOf<SocketException>());
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
            var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
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

        void AssertPoolState(ConnectorPool? pool, int open, int idle)
        {
            if (pool == null)
                throw new ArgumentNullException(nameof(pool));

            var state = pool.State;
            Assert.That(state.Open, Is.EqualTo(open), $"Open should be {open} but is {state.Open}");
            Assert.That(state.Idle, Is.EqualTo(idle), $"Idle should be {idle} but is {state.Idle}");
        }
    }
}
