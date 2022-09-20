using NUnit.Framework;

namespace Npgsql.Tests;

[NonParallelizable]
class PoolManagerTests : TestBase
{
    [Test]
    public void With_canonical_connection_string()
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
    public void Many_pools()
    {
        PoolManager.Reset();
        for (var i = 0; i < 15; i++)
        {
            var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                ApplicationName = "App" + i
            }.ToString();
            using var conn = new NpgsqlConnection(connString);
            conn.Open();
        }
        PoolManager.Reset();
    }
#endif

    [Test]
    public void ClearAllPools()
    {
        using (OpenConnection()) {}
        // Now have one connection in the pool
        Assert.That(PoolManager.Pools.TryGetValue(ConnectionString, out var pool), Is.True);
        Assert.That(pool!.Statistics.Idle, Is.EqualTo(1));

        NpgsqlConnection.ClearAllPools();
        Assert.That(pool.Statistics.Idle, Is.Zero);
        Assert.That(pool.Statistics.Total, Is.Zero);
    }

    [Test]
    public void ClearAllPools_with_busy()
    {
        NpgsqlDataSource? pool;
        using (OpenConnection())
        {
            using (OpenConnection()) { }
            // We have one idle, one busy

            NpgsqlConnection.ClearAllPools();
            Assert.That(PoolManager.Pools.TryGetValue(ConnectionString, out pool), Is.True);
            Assert.That(pool!.Statistics.Idle, Is.Zero);
            Assert.That(pool.Statistics.Total, Is.EqualTo(1));
        }
        Assert.That(pool.Statistics.Idle, Is.Zero);
        Assert.That(pool.Statistics.Total, Is.Zero);
    }

    [SetUp]
    public void Setup() => PoolManager.Reset();

    [TearDown]
    public void Teardown() => PoolManager.Reset();
}
