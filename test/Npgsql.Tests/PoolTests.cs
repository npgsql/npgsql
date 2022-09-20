using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using static Npgsql.Tests.TestUtil;

namespace Npgsql.Tests;

[NonParallelizable]
class PoolTests : TestBase
{
    [Test]
    public void MinPoolSize_equals_MaxPoolSize()
    {
        using var conn = CreateConnection(new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            ApplicationName = nameof(MinPoolSize_equals_MaxPoolSize),
            MinPoolSize = 30,
            MaxPoolSize = 30
        }.ToString());
        conn.Open();
    }

    [Test]
    public void MinPoolSize_bigger_than_MaxPoolSize_throws()
    {
        var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            ApplicationName = nameof(MinPoolSize_bigger_than_MaxPoolSize_throws),
            MinPoolSize = 2,
            MaxPoolSize = 1
        }.ToString();

        Assert.Throws<ArgumentException>(() => CreateConnection(connString));
    }

    [Test]
    public void Reuse_connector_before_creating_new()
    {
        var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            ApplicationName = nameof(Reuse_connector_before_creating_new),
        }.ToString();

        using var conn = CreateConnection(connString);
        conn.Open();
        var backendId = conn.Connector!.BackendProcessId;
        conn.Close();
        conn.Open();
        Assert.That(conn.Connector.BackendProcessId, Is.EqualTo(backendId));
    }

    [Test]
    public void Get_connector_from_exhausted_pool()
    {
        var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            ApplicationName = nameof(Get_connector_from_exhausted_pool),
            MaxPoolSize = 1,
            Timeout = 0
        }.ToString();

        using var conn1 = CreateConnection(connString);
        conn1.Open();

        // Pool is exhausted
        using var conn2 = CreateConnection(connString);
        _ = Task.Delay(1000).ContinueWith(_ => conn1.Close());
        conn2.Open();
    }

    //[Test, Explicit]
    public async Task Get_connector_from_exhausted_pool_async()
    {
        var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            ApplicationName = nameof(Get_connector_from_exhausted_pool_async),
            MaxPoolSize = 1,
            Timeout = 0
        }.ToString();

        using var conn1 = CreateConnection(connString);
        await conn1.OpenAsync();

        // Pool is exhausted
        using var conn2 = CreateConnection(connString);
        using (new Timer(o => conn1.Close(), null, 1000, Timeout.Infinite))
            await conn2.OpenAsync();
    }

    [Test]
    public async Task Timeout_getting_connector_from_exhausted_pool([Values(true, false)] bool async)
    {
        var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            MaxPoolSize = 1,
            Timeout = 2
        };

        using var _ = CreateTempPool(csb, out var connString);
        using (var conn1 = CreateConnection(connString))
        {
            await conn1.OpenAsync();
            // Pool is now exhausted

            await using var conn2 = CreateConnection(connString);
            var e = async
                ? Assert.ThrowsAsync<NpgsqlException>(async () => await conn2.OpenAsync())!
                : Assert.Throws<NpgsqlException>(() => conn2.Open())!;

            Assert.That(e.InnerException, Is.TypeOf<TimeoutException>());
        }

        // conn1 should now be back in the pool as idle
        using (var conn3 = CreateConnection(connString))
            conn3.Open();
    }

    [Test]
    public async Task Timeout_getting_connector_from_exhausted_pool_async()
    {
        var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            ApplicationName = nameof(Timeout_getting_connector_from_exhausted_pool_async),
            MaxPoolSize = 1,
            Timeout = 2
        }.ToString();

        using (var conn1 = CreateConnection(connString))
        {
            await conn1.OpenAsync();

            // Pool is exhausted
            using (var conn2 = CreateConnection(connString))
                Assert.That(async () => await conn2.OpenAsync(), Throws.Exception.TypeOf<NpgsqlException>());
        }
        // conn1 should now be back in the pool as idle
        using (var conn3 = CreateConnection(connString))
            conn3.Open();
    }

    [Test]
    [Explicit("Timing-based")]
    public async Task OpenAsync_cancel()
    {
        var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            ApplicationName = nameof(OpenAsync_cancel),
            MaxPoolSize = 1,
        }.ToString();

        using var conn1 = CreateConnection(connString);
        await conn1.OpenAsync();

        Assert.True(PoolManager.Pools.TryGetValue(connString, out var pool));
        AssertPoolState(pool, open: 1, idle: 0);

        // Pool is exhausted
        using (var conn2 = CreateConnection(connString))
        {
            var cts = new CancellationTokenSource(1000);
            var openTask = conn2.OpenAsync(cts.Token);
            AssertPoolState(pool, open: 1, idle: 0);
            Assert.That(async () => await openTask, Throws.Exception.TypeOf<OperationCanceledException>());
        }

        AssertPoolState(pool, open: 1, idle: 0);
        using (var conn2 = CreateConnection(connString))
        using (new Timer(o => conn1.Close(), null, 1000, Timeout.Infinite))
        {
            await conn2.OpenAsync();
            AssertPoolState(pool, open: 1, idle: 0);
        }
        AssertPoolState(pool, open: 1, idle: 1);
    }

    [Test, Description("Makes sure that when a pooled connection is closed it's properly reset, and that parameter settings aren't leaked")]
    public void ResetOnClose()
    {
        var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            ApplicationName = nameof(ResetOnClose),
            SearchPath = "public"
        }.ToString();
        using var conn = CreateConnection(connString);
        conn.Open();
        Assert.That(conn.ExecuteScalar("SHOW search_path"), Is.Not.Contains("pg_temp"));
        var backendId = conn.Connector!.BackendProcessId;
        conn.ExecuteNonQuery("SET search_path=pg_temp");
        conn.Close();

        conn.Open();
        Assert.That(conn.Connector.BackendProcessId, Is.EqualTo(backendId));
        Assert.That(conn.ExecuteScalar("SHOW search_path"), Is.EqualTo("public"));
    }

    [Test]
    public void ConnectionPruningInterval_zero_throws()
    {
        var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            ApplicationName = nameof(ConnectionPruningInterval_zero_throws),
            ConnectionPruningInterval = 0
        }.ToString();

        Assert.Throws<ArgumentException>(() => OpenConnection(connString));
    }

    [Test]
    public void ConnectionPruningInterval_bigger_than_ConnectionIdleLifetime_throws()
    {
        var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            ApplicationName = nameof(ConnectionPruningInterval_bigger_than_ConnectionIdleLifetime_throws),
            ConnectionIdleLifetime = 1,
            ConnectionPruningInterval = 2
        }.ToString();

        Assert.Throws<ArgumentException>(() => OpenConnection(connString));
    }

    [Theory, Explicit("Slow, and flaky under pressure, based on timing")]
    [TestCase(0, 2, 1, 2)] // min pool size 0, sample twice
    [TestCase(1, 2, 1, 2)] // min pool size 1, sample twice
    [TestCase(2, 2, 1, 2)] // min pool size 2, sample twice
    [TestCase(2, 3, 2, 2)] // test rounding up, should sample twice.
    [TestCase(2, 1, 1, 1)] // test sample once.
    [TestCase(2, 20, 3, 7)] // test high samples.
    public void Prune_idle_connectors(int minPoolSize, int connectionIdleLifeTime, int connectionPruningInterval, int samples)
    {
        var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            ApplicationName = nameof(Prune_idle_connectors),
            MinPoolSize = minPoolSize,
            ConnectionIdleLifetime = connectionIdleLifeTime,
            ConnectionPruningInterval = connectionPruningInterval
        }.ToString();

        var connectionPruningIntervalMs = connectionPruningInterval * 1000;

        using var conn1 = OpenConnection(connString);
        using var conn2 = OpenConnection(connString);
        using var conn3 = OpenConnection(connString);
        Assert.True(PoolManager.Pools.TryGetValue(connString, out var pool));

        conn1.Close();
        conn2.Close();
        AssertPoolState(pool!, open: 3, idle: 2);

        var paddingMs = 100; // 100ms
        var sleepInterval = connectionPruningIntervalMs + paddingMs;
        var total = 0;

        for (var i = 0; i < samples - 1; i++)
        {
            total += sleepInterval;
            Thread.Sleep(sleepInterval);
            // ConnectionIdleLifetime not yet reached.
            AssertPoolState(pool, open: 3, idle: 2);
        }

        // final cycle to do pruning.
        Thread.Sleep(Math.Max(sleepInterval, (connectionIdleLifeTime * 1000) - total));

        // ConnectionIdleLifetime reached, we still have one connection open minimum,
        // and as a result we have minPoolSize - 1 idle connections.
        AssertPoolState(pool, open: Math.Max(1, minPoolSize), idle: Math.Max(0, minPoolSize - 1));
    }

    [Test, Description("Makes sure that when a waiting async open is is given a connection, the continuation is executed in the TP rather than on the closing thread")]
    public void Close_releases_waiter_on_another_thread()
    {
        var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            ApplicationName = nameof(Close_releases_waiter_on_another_thread),
            MaxPoolSize = 1
        }.ToString();
        var conn1 = CreateConnection(connString);
        try
        {
            conn1.Open();   // Pool is now exhausted

            Assert.True(PoolManager.Pools.TryGetValue(connString, out var pool));
            AssertPoolState(pool, open: 1, idle: 0);

            Func<Task<int>> asyncOpener = async () =>
            {
                using (var conn2 = CreateConnection(connString))
                {
                    await conn2.OpenAsync();
                    AssertPoolState(pool, open: 1, idle: 0);
                }
                AssertPoolState(pool, open: 1, idle: 1);
                return Environment.CurrentManagedThreadId;
            };

            // Start an async open which will not complete as the pool is exhausted.
            var asyncOpenerTask = asyncOpener();
            conn1.Close();  // Complete the async open by closing conn1
            var asyncOpenerThreadId = asyncOpenerTask.GetAwaiter().GetResult();
            AssertPoolState(pool, open: 1, idle: 1);

            Assert.That(asyncOpenerThreadId, Is.Not.EqualTo(Environment.CurrentManagedThreadId));
        }
        finally
        {
            conn1.Close();
            NpgsqlConnection.ClearPool(conn1);
        }
    }

    [Test] //TODO: parallelize
    public void Release_waiter_on_connection_failure()
    {
        var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            Port = 9999,
            MaxPoolSize = 1
        };

        using var _ = CreateTempPool(csb, out var connString);
        var tasks = Enumerable.Range(0, 2).Select(i => Task.Run(async () =>
        {
            await using var conn = CreateConnection(connString);
            await conn.OpenAsync();
        })).ToArray();

        var ex = Assert.Throws<AggregateException>(() => Task.WaitAll(tasks))!;
        Assert.That(ex.InnerExceptions, Has.Count.EqualTo(2));
        foreach (var inner in ex.InnerExceptions)
            Assert.That(inner, Is.TypeOf<NpgsqlException>());
    }

    [Test]
    [TestCase(1)]
    [TestCase(2)]
    public void ClearPool(int iterations)
    {
        var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            ApplicationName = nameof(ClearPool)
        }.ToString();

        NpgsqlConnection conn;
        for (var i = 0; i < iterations; i++)
        {
            using (conn = OpenConnection(connString)) { }
            // Now have one connection in the pool
            Assert.True(PoolManager.Pools.TryGetValue(connString, out var pool));
            AssertPoolState(pool, open: 1, idle: 1);

            NpgsqlConnection.ClearPool(conn);
            AssertPoolState(pool, open: 0, idle: 0);
        }
    }

    [Test]
    public void ClearPool_with_busy()
    {
        var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            ApplicationName = nameof(ClearPool_with_busy)
        }.ToString();

        NpgsqlDataSource? pool;
        using (var conn = OpenConnection(connString))
        {
            NpgsqlConnection.ClearPool(conn);
            // conn is still busy but should get closed when returned to the pool

            Assert.True(PoolManager.Pools.TryGetValue(connString, out pool));
            AssertPoolState(pool, open: 1, idle: 0);
        }
        AssertPoolState(pool, open: 0, idle: 0);
    }

    [Test]
    public void ClearPool_with_no_pool()
    {
        var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            ApplicationName = nameof(ClearPool_with_no_pool)
        }.ToString();
        using var conn = CreateConnection(connString);
        NpgsqlConnection.ClearPool(conn);
    }

    [Test, Description("https://github.com/npgsql/npgsql/commit/45e33ecef21f75f51a625c7b919a50da3ed8e920#r28239653")]
    public void Open_physical_failure()
    {
        var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            ApplicationName = nameof(Open_physical_failure),
            Port = 44444,
            MaxPoolSize = 1
        }.ToString();
        using var conn = CreateConnection(connString);
        for (var i = 0; i < 1; i++)
            Assert.That(() => conn.Open(), Throws.Exception
                .TypeOf<NpgsqlException>()
                .With.InnerException.TypeOf<SocketException>());
        Assert.True(PoolManager.Pools.TryGetValue(connString, out var pool));
        AssertPoolState(pool, open: 0, idle: 0);
    }

    //[Test, Explicit]
    //[TestCase(10, 10, 30, true)]
    //[TestCase(10, 10, 30, false)]
    //[TestCase(10, 20, 30, true)]
    //[TestCase(10, 20, 30, false)]
    public void Exercise_pool(int maxPoolSize, int numTasks, int seconds, bool async)
    {
        var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            ApplicationName = nameof(Exercise_pool),
            MaxPoolSize = maxPoolSize
        }.ToString();

        Console.WriteLine($"Spinning up {numTasks} parallel tasks for {seconds} seconds (MaxPoolSize={maxPoolSize})...");
        StopFlag = 0;
        var tasks = Enumerable.Range(0, numTasks).Select(i => Task.Run(async () =>
        {
            while (StopFlag == 0)
            {
                using var conn = CreateConnection(connString);
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

    [Test]
    public async Task ConnectionLifetime()
    {
        var builder = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            ConnectionLifetime = 1
        };

        using var _ = CreateTempPool(builder, out var connectionString);
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();
        var processId = conn.ProcessID;
        await conn.CloseAsync();

        await Task.Delay(2000);

        await conn.OpenAsync();
        Assert.That(conn.ProcessID, Is.Not.EqualTo(processId));
    }

    #region Support

    volatile int StopFlag;

    void AssertPoolState(NpgsqlDataSource? pool, int open, int idle)
    {
        if (pool == null)
            throw new ArgumentNullException(nameof(pool));

        var (openState, idleState, _) = pool.Statistics;
        Assert.That(openState, Is.EqualTo(open), $"Open should be {open} but is {openState}");
        Assert.That(idleState, Is.EqualTo(idle), $"Idle should be {idle} but is {idleState}");
    }

    // With MaxPoolSize=1, opens many connections in parallel and executes a simple SELECT. Since there's only one
    // physical connection, all operations will be completely serialized
    [Test]
    public Task OnePhysicalConnectionManyCommands()
    {
        const int numParallelCommands = 10000;

        var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            MaxPoolSize = 1,
            MaxAutoPrepare = 5,
            AutoPrepareMinUsages = 5,
            Timeout = 0
        }.ToString();

        return Task.WhenAll(Enumerable.Range(0, numParallelCommands)
            .Select(async i =>
            {
                using var conn = new NpgsqlConnection(connString);
                await conn.OpenAsync();
                using var cmd = new NpgsqlCommand("SELECT " + i, conn);
                var result = await cmd.ExecuteScalarAsync();
                Assert.That(result, Is.EqualTo(i));
            }));
    }

    // When multiplexing, and the pool is totally saturated (at Max Pool Size and 0 idle connectors), we select
    // the connector with the least commands in flight and execute on it. We must never select a connector with
    // a pending transaction on it.
    // TODO: Test not tested
    [Test]
    [Ignore("Multiplexing: fails")]
    public void MultiplexedCommandDoesntGetExecutedOnTransactionedConnector()
    {
        var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            MaxPoolSize = 1,
            Timeout = 1
        }.ToString();

        using var connWithTx = OpenConnection(connString);
        using var tx = connWithTx.BeginTransaction();
        // connWithTx should now be bound with the only physical connector available.
        // Any commands execute should timeout

        using var conn2 = OpenConnection(connString);
        using var cmd = new NpgsqlCommand("SELECT 1", conn2);
        Assert.ThrowsAsync<NpgsqlException>(() => cmd.ExecuteScalarAsync());
    }

    protected override NpgsqlConnection CreateConnection(string? connectionString = null)
    {
        var conn = base.CreateConnection(connectionString);
        _cleanup.Add(conn);
        return conn;
    }

    readonly List<NpgsqlConnection> _cleanup = new();

    [TearDown]
    public void Cleanup()
    {
        foreach (var c in _cleanup)
        {
            NpgsqlConnection.ClearPool(c);
        }
        _cleanup.Clear();
    }

    #endregion
}
