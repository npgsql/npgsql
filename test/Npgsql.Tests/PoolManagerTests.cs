using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Npgsql.Tests
{
    [NonParallelizable]
    class PoolManagerTests : TestBase
    {
        [Test]
        public void WithCanonicalConnString()
        {
            var connString = new NpgsqlConnectionStringBuilder(ConnectionString).ToString();
            using (var conn = new NpgsqlConnection(connString))
                conn.Open();
            var connString2 = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                ApplicationName = "Another connstring"
            }.ToString();
            using (var conn = new NpgsqlConnection(connString2))
                conn.Open();
        }

#if DEBUG
        [Test]
        public void ManyPools()
        {
            PoolManager.Reset();
            for (var i = 0; i < PoolManager.InitialPoolsSize + 1; i++)
            {
                var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
                {
                    ApplicationName = "App" + i
                }.ToString();
                using (var conn = new NpgsqlConnection(connString))
                    conn.Open();
            }
            PoolManager.Reset();
        }
#endif

        [Test]
        public void ClearAll()
        {
            using (OpenConnection()) {}
            // Now have one connection in the pool
            Assert.That(PoolManager.TryGetValue(ConnectionString, out var pool), Is.True);
            Assert.That(pool.State.Idle, Is.EqualTo(1));

            NpgsqlConnection.ClearAllPools();
            Assert.That(pool.State.Idle, Is.Zero);
            Assert.That(pool.State.Total, Is.Zero);
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
                Assert.That(PoolManager.TryGetValue(ConnectionString, out pool), Is.True);
                Assert.That(pool.State.Idle, Is.Zero);
                Assert.That(pool.State.Total, Is.EqualTo(1));
            }
            Assert.That(pool.State.Idle, Is.Zero);
            Assert.That(pool.State.Total, Is.Zero);
        }

        [SetUp]
        public void Setup() => PoolManager.Reset();

        [TearDown]
        public void Teardown() => PoolManager.Reset();
    }
}
