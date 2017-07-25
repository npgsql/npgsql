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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
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
            Assert.That(() => csb.MinPoolSize = PoolManager.PoolSizeLimit + 1, Throws.Exception.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void MinPoolSize()
        {
            var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                MinPoolSize = 2
            }.ToString();
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                conn.Close();
            }

            var pool = PoolManager.Pools[connString];
            Assert.That(pool.Idle, Has.Count.EqualTo(2));

            // Now open 2 connections and make sure they're good
            using (var conn1 = OpenConnection(connString))
            using (var conn2 = OpenConnection(connString))
            {
                Assert.That(pool.Idle, Has.Count.Zero);
                Assert.That(conn1.ExecuteScalar("SELECT 1"), Is.EqualTo(1));
                Assert.That(conn2.ExecuteScalar("SELECT 1"), Is.EqualTo(1));
            }
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

        [Test, Timeout(10000)]
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
                {
                    new Timer(o => conn1.Close(), null, 1000, Timeout.Infinite);
                    await conn2.OpenAsync();
                }
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

        [Test, Timeout(10000)]
        [Explicit("Flaky")]
        public async Task CancelOpenAsync()
        {
            var connString = new NpgsqlConnectionStringBuilder(ConnectionString) {
                ApplicationName = nameof(CancelOpenAsync),
                MaxPoolSize = 1,
            }.ToString();

            using (var conn1 = new NpgsqlConnection(connString))
            {
                await conn1.OpenAsync();

                // Pool is exhausted
                using (var conn2 = new NpgsqlConnection(connString))
                {
                    var cts = new CancellationTokenSource(1000);
                    Assert.That(async () => await conn2.OpenAsync(cts.Token), Throws.Exception.TypeOf<TaskCanceledException>());
                }

                // The cancelled open attempt should have left a cancelled task completion source
                // in the pool's wait queue. Make sure that's so
                using (var conn2 = new NpgsqlConnection(connString))
                {
                    new Timer(o => conn1.Close(), null, 1000, Timeout.Infinite);
                    await conn2.OpenAsync();
                }
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

        [Test]
        [Ignore("Flaky")]
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
                conn1.Close();
                conn2.Close();
                Assert.That(PoolManager.Pools[connString].Idle, Has.Count.EqualTo(2));
                Assert.That(PoolManager.Pools[connString].Busy, Is.EqualTo(1));

                Thread.Sleep(1500);
                // Pruning attempted, but ConnectionIdleLifetime not yet reached
                Assert.That(PoolManager.Pools[connString].Idle, Has.Count.EqualTo(2));
                Assert.That(PoolManager.Pools[connString].Busy, Is.EqualTo(1));

                Thread.Sleep(1500);
                // ConnectionIdleLifetime reached, but only one idle connection should be pruned (MinPoolSize=2)
                Assert.That(PoolManager.Pools[connString].Idle, Has.Count.EqualTo(1));
                Assert.That(PoolManager.Pools[connString].Busy, Is.EqualTo(1));
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

                Func<Task<int>> asyncOpener = async () =>
                {
                    using (var conn2 = new NpgsqlConnection(connString))
                    {
                        await conn2.OpenAsync();
                        return Thread.CurrentThread.ManagedThreadId;
                    }
                };

                // Start an async open which will not complete as the pool is exhausted.
                var asyncOpenerTask = asyncOpener();
                conn1.Close();  // Complete the async open by closing conn1
                var asyncOpenerThreadId = asyncOpenerTask.Result;

                Assert.That(asyncOpenerThreadId, Is.Not.EqualTo(Thread.CurrentThread.ManagedThreadId));
            }
            finally
            {
                conn1.Close();
                NpgsqlConnection.ClearPool(conn1);
            }
        }

        [Test]
        public void ClearAll()
        {
            using (OpenConnection()) {}
            // Now have one connection in the pool
            var pool = PoolManager.Pools[ConnectionString];
            Assert.That(pool.Idle, Has.Count.EqualTo(1));

            NpgsqlConnection.ClearAllPools();
            Assert.That(pool.Idle, Has.Count.Zero);
            Assert.That(pool.Busy, Is.Zero);
        }

        [Test]
        public void ClearAllWithBusy()
        {
            ConnectorPool pool;
            using (OpenConnection())
            {
                using (OpenConnection()) { }
                // We have one idle, one busy

                NpgsqlConnection.ClearAllPools();
                pool = PoolManager.Pools[ConnectionString];
                Assert.That(pool.Idle, Has.Count.Zero);
                Assert.That(pool.Busy, Is.EqualTo(1));
            }
            Assert.That(pool.Idle, Has.Count.Zero);
            Assert.That(pool.Busy, Is.Zero);
        }

        [Test]
        public void ClearPool()
        {
            NpgsqlConnection conn;
            using (conn = OpenConnection()) {}
            // Now have one connection in the pool
            var pool = PoolManager.Pools[ConnectionString];
            Assert.That(pool.Idle, Has.Count.EqualTo(1));

            NpgsqlConnection.ClearPool(conn);
            Assert.That(pool.Idle, Has.Count.Zero);
            Assert.That(pool.Busy, Is.Zero);
        }

        [Test]
        public void ClearWithBusy()
        {
            ConnectorPool pool;
            using (var conn = OpenConnection())
            {
                NpgsqlConnection.ClearPool(conn);
                // conn is still busy but should get closed when returned to the pool

                pool = PoolManager.Pools[ConnectionString];
                Assert.That(pool.Idle, Has.Count.Zero);
                Assert.That(pool.Busy, Is.EqualTo(1));
            }
            Assert.That(pool.Idle, Has.Count.Zero);
            Assert.That(pool.Busy, Is.Zero);
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
    }
}
