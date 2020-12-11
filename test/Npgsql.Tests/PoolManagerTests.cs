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
            PoolManager.ClearAll();
            for (var i = 0; i < PoolManager.InitialPoolsSize + 1; i++)
            {
                var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
                {
                    ApplicationName = "App" + i
                }.ToString();
                using (var conn = new NpgsqlConnection(connString))
                    conn.Open();
            }
            PoolManager.ClearAll();
        }
#endif

        [Test]
        public void ClearAll()
        {
            using (OpenConnection()) {}
            // Now have one connection in the pool
            Assert.That(PoolManager.TryGetValue(ConnectionString, out var pool), Is.True);
            Assert.That(pool!.Statistics.Idle, Is.EqualTo(1));

            NpgsqlConnection.ClearAllPools();
            Assert.That(pool.Statistics.Idle, Is.Zero);
            Assert.That(pool.Statistics.Total, Is.Zero);
        }

        [Test]
        public void ClearAllWithBusy()
        {
            using (OpenConnection())
            {
                using (OpenConnection()) { }
                // We have one idle, one busy

                NpgsqlConnection.ClearAllPools();
                Assert.That(PoolManager.TryGetValue(ConnectionString, out var pool), Is.False);
            }
        }

        [SetUp]
        public void Setup() => PoolManager.ClearAll();

        [TearDown]
        public void Teardown() => PoolManager.ClearAll();
    }
}
