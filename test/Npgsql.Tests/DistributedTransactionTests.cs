#if NET7_0_OR_GREATER

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Transactions;
using Npgsql.Internal;
using NUnit.Framework;
using static Npgsql.Tests.TestUtil;

namespace Npgsql.Tests;

[NonParallelizable]
public class DistributedTransactionTests : TestBase
{
    [Test]
    public void Two_connections_rollback_implicit_enlistment()
    {
        using var adminConn = OpenConnection();
        var table = CreateTempTable(adminConn, "name TEXT");

        using (new TransactionScope())
        using (var conn1 = OpenConnection(ConnectionStringEnlistOn))
        using (var conn2 = OpenConnection(ConnectionStringEnlistOn))
        {
            conn1.ExecuteNonQuery($"INSERT INTO {table} (name) VALUES ('test1')");
            conn2.ExecuteNonQuery($"INSERT INTO {table} (name) VALUES ('test2')");
        }

        Retry(() =>
        {
            AssertNoDistributedIdentifier();
            AssertNoPreparedTransactions();
            AssertNumberOfRows(adminConn, table, 0);
        });
    }

    [Test]
    public void Two_connections_rollback_explicit_enlistment()
    {
        using var adminConn = OpenConnection();
        var table = CreateTempTable(adminConn, "name TEXT");

        using (var conn1 = OpenConnection(ConnectionStringEnlistOff))
        using (var conn2 = OpenConnection(ConnectionStringEnlistOff))
        using (new TransactionScope())
        {
            conn1.EnlistTransaction(Transaction.Current);
            conn2.EnlistTransaction(Transaction.Current);

            Assert.That(conn1.ExecuteNonQuery($"INSERT INTO {table} (name) VALUES ('test1')"), Is.EqualTo(1), "Unexpected first insert rowcount");
            Assert.That(conn2.ExecuteNonQuery($"INSERT INTO {table} (name) VALUES ('test2')"), Is.EqualTo(1), "Unexpected second insert rowcount");
        }

        Retry(() =>
        {
            AssertNoDistributedIdentifier();
            AssertNoPreparedTransactions();
            AssertNumberOfRows(adminConn, table, 0);
        });
    }

    [Test]
    public void Two_connections_commit()
    {
        using var adminConn = OpenConnection();
        var table = CreateTempTable(adminConn, "name TEXT");

        using (var scope = new TransactionScope())
        using (var conn1 = OpenConnection(ConnectionStringEnlistOn))
        using (var conn2 = OpenConnection(ConnectionStringEnlistOn))
        {
            conn1.ExecuteNonQuery($"INSERT INTO {table} (name) VALUES ('test1')");
            conn2.ExecuteNonQuery($"INSERT INTO {table} (name) VALUES ('test2')");

            scope.Complete();
        }

        Retry(() =>
        {
            AssertNoDistributedIdentifier();
            AssertNoPreparedTransactions();
            AssertNumberOfRows(adminConn, table, 2);
        });
    }

    [Test]
    public void Two_connections_with_failure()
    {
        // Use our own data source since this test breaks the connection with a critical failure, affecting database state tracking.
        using var dataSource = NpgsqlDataSource.Create(ConnectionStringEnlistOn);
        using var adminConn = dataSource.OpenConnection();
        var table = CreateTempTable(adminConn, "name TEXT");

        using var scope = new TransactionScope();
        using var conn1 = dataSource.OpenConnection();
        using var conn2 = dataSource.OpenConnection();

        conn1.ExecuteNonQuery($"INSERT INTO {table} (name) VALUES ('test1')");
        conn2.ExecuteNonQuery($"INSERT INTO {table} (name) VALUES ('test2')");

        conn1.ExecuteNonQuery($"SELECT pg_terminate_backend({conn2.ProcessID})");
        scope.Complete();
        Assert.That(() => scope.Dispose(), Throws.Exception.TypeOf<TransactionAbortedException>());

        AssertNoDistributedIdentifier();
        AssertNoPreparedTransactions();
        AssertNumberOfRows(adminConn, table, 0);
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1737")]
    public void Multiple_unpooled_connections_do_not_reuse()
    {
        var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            Pooling = false,
            Enlist = true
        };

        using var scope = new TransactionScope();

        int processId;

        using (var conn1 = OpenConnection(csb))
        using (var cmd = new NpgsqlCommand("SELECT 1", conn1))
        {
            processId = conn1.ProcessID;
            cmd.ExecuteNonQuery();
        }

        using (var conn2 = OpenConnection(csb))
        using (var cmd = new NpgsqlCommand("SELECT 1", conn2))
        {
            // The connection reuse optimization isn't implemented for unpooled connections (though it could be)
            Assert.That(conn2.ProcessID, Is.Not.EqualTo(processId));
            cmd.ExecuteNonQuery();
        }

        scope.Complete();
    }

    [Test(Description = "Transaction race, bool distributed")]
    [Explicit("Fails on Appveyor (https://ci.appveyor.com/project/roji/npgsql/build/3.3.0-250)")]
    public void Transaction_race([Values(false, true)] bool distributed)
    {
        using var adminConn = OpenConnection();
        var table = CreateTempTable(adminConn, "name TEXT");

        for (var i = 1; i <= 100; i++)
        {
            var eventQueue = new ConcurrentQueue<TransactionEvent>();
            try
            {
                using (var tx = new TransactionScope())
                using (var conn1 = OpenConnection(ConnectionStringEnlistOn))
                {
                    eventQueue.Enqueue(new TransactionEvent("Scope started, connection enlisted"));
                    conn1.ExecuteNonQuery($"INSERT INTO {table} (name) VALUES ('test1')");
                    eventQueue.Enqueue(new TransactionEvent("Insert done"));

                    if (distributed)
                    {
                        EnlistResource.EscalateToDistributed(eventQueue);
                        AssertHasDistributedIdentifier();
                    }
                    else
                    {
                        EnlistResource.EnlistVolatile(eventQueue);
                        AssertNoDistributedIdentifier();
                    }

                    tx.Complete();
                    eventQueue.Enqueue(new TransactionEvent("Scope completed"));
                }

                eventQueue.Enqueue(new TransactionEvent("Scope disposed"));
                AssertNoDistributedIdentifier();

                if (distributed)
                {
                    // There may be a race condition here, where the prepared transaction above still hasn't completed.
                    // This is by design of MS DTC. Giving it up to 100ms to complete. If it proves flaky, raise
                    // maxLoop.
                    const int maxLoop = 20;
                    for (var j = 0; j < maxLoop; j++)
                    {
                        Thread.Sleep(10);
                        try
                        {
                            AssertNumberOfRows(adminConn, table, i);
                            break;
                        }
                        catch
                        {
                            if (j == maxLoop - 1)
                                throw;
                        }
                    }
                }
                else
                    AssertNumberOfRows(adminConn, table, i);
            }
            catch (Exception ex)
            {
                Assert.Fail(
                    @"Failed at iteration {0}.
Events:
{1}
Exception {2}",
                    i, FormatEventQueue(eventQueue), ex);
            }
        }
    }

    [Test(Description = "Connection reuse race after transaction, bool distributed"), Explicit]
    public void Connection_reuse_race_after_transaction([Values(false, true)] bool distributed)
    {
        using var adminConn = OpenConnection();
        var table = CreateTempTable(adminConn, "name TEXT");

        for (var i = 1; i <= 100; i++)
        {
            var eventQueue = new ConcurrentQueue<TransactionEvent>();
            try
            {
                using var conn1 = OpenConnection(ConnectionStringEnlistOff);

                using (var scope = new TransactionScope())
                {
                    conn1.EnlistTransaction(Transaction.Current);
                    eventQueue.Enqueue(new TransactionEvent("Scope started, connection enlisted"));

                    if (distributed)
                    {
                        EnlistResource.EscalateToDistributed(eventQueue);
                        AssertHasDistributedIdentifier();
                    }
                    else
                    {
                        EnlistResource.EnlistVolatile(eventQueue);
                        AssertNoDistributedIdentifier();
                    }

                    conn1.ExecuteNonQuery($"INSERT INTO {table} (name) VALUES ('test1')");
                    eventQueue.Enqueue(new TransactionEvent("Insert done"));

                    scope.Complete();
                    eventQueue.Enqueue(new TransactionEvent("Scope completed"));
                }

                eventQueue.Enqueue(new TransactionEvent("Scope disposed"));

                Assert.DoesNotThrow(() => conn1.ExecuteScalar($"SELECT COUNT(*) FROM {table}"));
            }
            catch (Exception ex)
            {
                Assert.Fail(
                    @"Failed at iteration {0}.
Events:
{1}
Exception {2}",
                    i, FormatEventQueue(eventQueue), ex);
            }
        }
    }

    [Test(Description = "Connection reuse race after rollback, bool distributed"), Explicit("Currently failing.")]
    public void Connection_reuse_race_after_rollback([Values(false, true)] bool distributed)
    {
        using var adminConn = OpenConnection();
        var table = CreateTempTable(adminConn, "name TEXT");

        for (var i = 1; i <= 100; i++)
        {
            var eventQueue = new ConcurrentQueue<TransactionEvent>();
            try
            {
                using var conn1 = OpenConnection(ConnectionStringEnlistOff);

                using (new TransactionScope())
                {
                    conn1.EnlistTransaction(Transaction.Current);
                    eventQueue.Enqueue(new TransactionEvent("Scope started, connection enlisted"));

                    if (distributed)
                    {
                        EnlistResource.EscalateToDistributed(eventQueue);
                        AssertHasDistributedIdentifier();
                    }
                    else
                    {
                        EnlistResource.EnlistVolatile(eventQueue);
                        AssertNoDistributedIdentifier();
                    }

                    conn1.ExecuteNonQuery($"INSERT INTO {table} (name) VALUES ('test1')");
                    eventQueue.Enqueue(new TransactionEvent("Insert done"));

                    eventQueue.Enqueue(new TransactionEvent("Scope not completed"));
                }

                eventQueue.Enqueue(new TransactionEvent("Scope disposed"));
                conn1.EnlistTransaction(null);
                eventQueue.Enqueue(new TransactionEvent("Connection enlisted with null"));
                Assert.DoesNotThrow(() => conn1.ExecuteScalar($"SELECT COUNT(*) FROM {table}"));
            }
            catch (Exception ex)
            {
                Assert.Fail(
                    @"Failed at iteration {0}.
Events:
{1}
Exception {2}",
                    i, FormatEventQueue(eventQueue), ex);
            }
        }
    }

    [Test(Description = "Connection reuse race chaining transactions, bool distributed")]
    [Explicit]
    public void Connection_reuse_race_chaining_transaction([Values(false, true)] bool distributed)
    {
        using var adminConn = OpenConnection();
        var table = CreateTempTable(adminConn, "name TEXT");

        for (var i = 1; i <= 100; i++)
        {
            var eventQueue = new ConcurrentQueue<TransactionEvent>();
            try
            {
                using var conn1 = OpenConnection(ConnectionStringEnlistOff);

                using (var scope = new TransactionScope())
                {
                    eventQueue.Enqueue(new TransactionEvent("First scope started"));
                    conn1.EnlistTransaction(Transaction.Current);
                    eventQueue.Enqueue(new TransactionEvent("First scope, connection enlisted"));

                    if (distributed)
                    {
                        EnlistResource.EscalateToDistributed(eventQueue);
                        AssertHasDistributedIdentifier();
                    }
                    else
                    {
                        EnlistResource.EnlistVolatile(eventQueue);
                        AssertNoDistributedIdentifier();
                    }

                    conn1.ExecuteNonQuery($"INSERT INTO {table} (name) VALUES ('test1')");
                    eventQueue.Enqueue(new TransactionEvent("First insert done"));

                    scope.Complete();
                    eventQueue.Enqueue(new TransactionEvent("First scope completed"));
                }
                eventQueue.Enqueue(new TransactionEvent("First scope disposed"));

                using (var scope = new TransactionScope())
                {
                    eventQueue.Enqueue(new TransactionEvent("Second scope started"));
                    conn1.EnlistTransaction(Transaction.Current);
                    eventQueue.Enqueue(new TransactionEvent("Second scope, connection enlisted"));

                    if (distributed)
                    {
                        EnlistResource.EscalateToDistributed(eventQueue);
                        AssertHasDistributedIdentifier();
                    }
                    else
                    {
                        EnlistResource.EnlistVolatile(eventQueue);
                        AssertNoDistributedIdentifier();
                    }

                    conn1.ExecuteNonQuery($"INSERT INTO {table} (name) VALUES ('test1')");
                    eventQueue.Enqueue(new TransactionEvent("Second insert done"));

                    scope.Complete();
                    eventQueue.Enqueue(new TransactionEvent("Second scope completed"));
                }
                eventQueue.Enqueue(new TransactionEvent("Second scope disposed"));
            }
            catch (Exception ex)
            {
                Assert.Fail(
                    @"Failed at iteration {0}.
Events:
{1}
Exception {2}",
                    i, FormatEventQueue(eventQueue), ex);
            }
        }
    }

    #region Utilities

    // MSDTC is asynchronous, i.e. Commit/Rollback may return before the transaction has actually completed in the database;
    // so allow some time for assertions to succeed.
    static void Retry(Action action)
    {
        const int Retries = 50;

        for (var i = 0; i < Retries; i++)
        {
            try
            {
                action();
                return;
            }
            catch (AssertionException)
            {
                if (i == Retries - 1)
                {
                    throw;
                }

                Thread.Sleep(100);
            }
        }
    }

    void AssertNoPreparedTransactions()
        => Assert.That(GetNumberOfPreparedTransactions(), Is.EqualTo(0), "Prepared transactions found");

    int GetNumberOfPreparedTransactions()
    {
        using (var conn = OpenConnection(ConnectionStringEnlistOff))
        using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM pg_prepared_xacts WHERE database = @database", conn))
        {
            cmd.Parameters.Add(new NpgsqlParameter("database", conn.Database));
            return (int)(long)cmd.ExecuteScalar()!;
        }
    }

    void AssertNumberOfRows(NpgsqlConnection connection, string table, int expected)
        => Assert.That(connection.ExecuteScalar($"SELECT COUNT(*) FROM {table}"), Is.EqualTo(expected), "Unexpected data count");

    static void AssertNoDistributedIdentifier()
        => Assert.That(Transaction.Current?.TransactionInformation.DistributedIdentifier ?? Guid.Empty, Is.EqualTo(Guid.Empty), "Distributed identifier found");

    static void AssertHasDistributedIdentifier()
        => Assert.That(Transaction.Current?.TransactionInformation.DistributedIdentifier ?? Guid.Empty, Is.Not.EqualTo(Guid.Empty), "Distributed identifier not found");

    public string ConnectionStringEnlistOn
        => new NpgsqlConnectionStringBuilder(ConnectionString) { Enlist = true }.ToString();

    public string ConnectionStringEnlistOff
        => new NpgsqlConnectionStringBuilder(ConnectionString) { Enlist = false }.ToString();

    static string FormatEventQueue(ConcurrentQueue<TransactionEvent> eventQueue)
    {
        eventQueue.Enqueue(new TransactionEvent(@"-------------
Start formatting event queue, going to sleep a bit for late events
-------------"));
        Thread.Sleep(20);
        var eventsMessage = new StringBuilder();
        foreach (var evt in eventQueue)
        {
            eventsMessage.AppendLine(evt.Message);
        }
        return eventsMessage.ToString();
    }

    // Idea from NHibernate test project, DtcFailuresFixture
    public class EnlistResource : IEnlistmentNotification
    {
        public static int Counter { get; set; }

        readonly bool _shouldRollBack;
        readonly string _name;
        readonly ConcurrentQueue<TransactionEvent>? _eventQueue;

        public static void EnlistVolatile(ConcurrentQueue<TransactionEvent> eventQueue)
            => EnlistVolatile(false, eventQueue);

        public static void EnlistVolatile(bool shouldRollBack = false, ConcurrentQueue<TransactionEvent>? eventQueue = null)
            => Enlist(false, shouldRollBack, eventQueue);

        public static void EscalateToDistributed(ConcurrentQueue<TransactionEvent> eventQueue)
            => EscalateToDistributed(false, eventQueue);

        public static void EscalateToDistributed(bool shouldRollBack = false, ConcurrentQueue<TransactionEvent>? eventQueue = null)
            => Enlist(true, shouldRollBack, eventQueue);

        static void Enlist(bool durable, bool shouldRollBack, ConcurrentQueue<TransactionEvent>? eventQueue)
        {
            Counter++;

            var name = $"{(durable ? "Durable" : "Volatile")} resource {Counter}";
            var resource = new EnlistResource(shouldRollBack, name, eventQueue);
            if (durable)
                Transaction.Current!.EnlistDurable(Guid.NewGuid(), resource, EnlistmentOptions.None);
            else
                Transaction.Current!.EnlistVolatile(resource, EnlistmentOptions.None);

            Transaction.Current.TransactionCompleted += resource.Current_TransactionCompleted!;

            eventQueue?.Enqueue(new TransactionEvent(name + ": enlisted"));
        }

        EnlistResource(bool shouldRollBack, string name, ConcurrentQueue<TransactionEvent>? eventQueue)
        {
            _shouldRollBack = shouldRollBack;
            _name = name;
            _eventQueue = eventQueue;
        }

        public void Prepare(PreparingEnlistment preparingEnlistment)
        {
            _eventQueue?.Enqueue(new TransactionEvent(_name + ": prepare phase start"));
            Thread.Sleep(1);
            if (_shouldRollBack)
            {
                _eventQueue?.Enqueue(new TransactionEvent(_name + ": prepare phase, calling rollback-ed"));
                preparingEnlistment.ForceRollback();
            }
            else
            {
                _eventQueue?.Enqueue(new TransactionEvent(_name + ": prepare phase, calling prepared"));
                preparingEnlistment.Prepared();
            }
            Thread.Sleep(1);
            _eventQueue?.Enqueue(new TransactionEvent(_name + ": prepare phase end"));
        }

        public void Commit(Enlistment enlistment)
        {
            _eventQueue?.Enqueue(new TransactionEvent(_name + ": commit phase start"));
            Thread.Sleep(1);
            _eventQueue?.Enqueue(new TransactionEvent(_name + ": commit phase, calling done"));
            enlistment.Done();
            Thread.Sleep(1);
            _eventQueue?.Enqueue(new TransactionEvent(_name + ": commit phase end"));
        }

        public void Rollback(Enlistment enlistment)
        {
            _eventQueue?.Enqueue(new TransactionEvent(_name + ": rollback phase start"));
            Thread.Sleep(1);
            _eventQueue?.Enqueue(new TransactionEvent(_name + ": rollback phase, calling done"));
            enlistment.Done();
            Thread.Sleep(1);
            _eventQueue?.Enqueue(new TransactionEvent(_name + ": rollback phase end"));
        }

        public void InDoubt(Enlistment enlistment)
        {
            _eventQueue?.Enqueue(new TransactionEvent(_name + ": in-doubt phase start"));
            Thread.Sleep(1);
            _eventQueue?.Enqueue(new TransactionEvent(_name + ": in-doubt phase, calling done"));
            enlistment.Done();
            Thread.Sleep(1);
            _eventQueue?.Enqueue(new TransactionEvent(_name + ": in-doubt phase end"));
        }

        void Current_TransactionCompleted(object sender, TransactionEventArgs e)
        {
            _eventQueue?.Enqueue(new TransactionEvent(_name + ": transaction completed start"));
            Thread.Sleep(1);
            _eventQueue?.Enqueue(new TransactionEvent(_name + ": transaction completed middle"));
            Thread.Sleep(1);
            _eventQueue?.Enqueue(new TransactionEvent(_name + ": transaction completed end"));
        }
    }

    public class TransactionEvent
    {
        public TransactionEvent(string message)
            => Message = $"{message} (TId {Thread.CurrentThread.ManagedThreadId})";
        public string Message { get; }
    }

    #endregion Utilities

    #region Setup

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Assert.Ignore("Distributed transactions are only supported on Windows");
            return;
        }

        using var connection = OpenConnection();

        // Make sure prepared transactions are enabled in postgresql.conf (disabled by default)
        if (int.Parse((string)connection.ExecuteScalar("SHOW max_prepared_transactions")!) == 0)
        {
            IgnoreExceptOnBuildServer("max_prepared_transactions is set to 0 in your postgresql.conf");
            return;
        }

        // Roll back any lingering prepared transactions from failed previous runs
        var lingeringTransactions = new List<string>();
        using (var cmd = new NpgsqlCommand("SELECT gid FROM pg_prepared_xacts WHERE database=@database", connection))
        {
            cmd.Parameters.AddWithValue("database", new NpgsqlConnectionStringBuilder(ConnectionString).Database!);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                lingeringTransactions.Add(reader.GetString(0));
        }
        foreach (var xactGid in lingeringTransactions)
            connection.ExecuteNonQuery($"ROLLBACK PREPARED '{xactGid}'");
    }

    [SetUp]
    public void SetUp()
        => EnlistResource.Counter = 0;

    internal static string CreateTempTable(NpgsqlConnection conn, string columns)
    {
        var tableName = "temp_table" + Interlocked.Increment(ref _tempTableCounter);
        conn.ExecuteNonQuery(@$"
START TRANSACTION; SELECT pg_advisory_xact_lock(0);
DROP TABLE IF EXISTS {tableName} CASCADE;
COMMIT;
CREATE TABLE {tableName} ({columns})");
        return tableName;
    }

    #endregion
}

#endif
