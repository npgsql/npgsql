using System;
using System.Data;
using System.Transactions;
using NUnit.Framework;

namespace Npgsql.Tests;

// This test suite contains ambient transaction tests, except those involving distributed transactions which are only
// supported on .NET Framework / Windows. Distributed transaction tests are in DistributedTransactionTests.
[NonParallelizable]
public class SystemTransactionTests : TestBase
{
    [Test, Description("Single connection enlisting explicitly, committing")]
    public void Explicit_enlist()
    {
        using var conn = new NpgsqlConnection(ConnectionStringEnlistOff);
        conn.Open();
        using (var scope = new TransactionScope())
        {
            conn.EnlistTransaction(Transaction.Current);
            Assert.That(conn.ExecuteNonQuery(@"INSERT INTO data (name) VALUES ('test')"), Is.EqualTo(1), "Unexpected insert rowcount");
            AssertNoDistributedIdentifier();
            AssertNoPreparedTransactions();
            scope.Complete();
        }
        AssertNoDistributedIdentifier();
        AssertNoPreparedTransactions();
        using (var tx = conn.BeginTransaction())
        {
            Assert.That(conn.ExecuteScalar(@"SELECT COUNT(*) FROM data"), Is.EqualTo(1), "Unexpected data count");
            tx.Rollback();
        }
    }

    [Test, Description("Single connection enlisting implicitly, committing")]
    public void Implicit_enlist()
    {
        var conn = new NpgsqlConnection(ConnectionStringEnlistOn);
        using (var scope = new TransactionScope())
        {
            conn.Open();
            Assert.That(conn.ExecuteNonQuery(@"INSERT INTO data (name) VALUES ('test')"), Is.EqualTo(1), "Unexpected insert rowcount");
            AssertNoDistributedIdentifier();
            AssertNoPreparedTransactions();
            scope.Complete();
        }
        using (var tx = conn.BeginTransaction())
        {
            Assert.That(conn.ExecuteScalar(@"SELECT COUNT(*) FROM data"), Is.EqualTo(1), "Unexpected data count");
            tx.Rollback();
        }
    }

    [Test]
    public void Enlist_Off()
    {
        using (new TransactionScope())
        using (var conn1 = OpenConnection(ConnectionStringEnlistOff))
        using (var conn2 = OpenConnection(ConnectionStringEnlistOff))
        {
            Assert.That(conn1.EnlistedTransaction, Is.Null);
            Assert.That(conn1.ExecuteNonQuery(@"INSERT INTO data (name) VALUES ('test')"), Is.EqualTo(1), "Unexpected insert rowcount");
            Assert.That(conn2.ExecuteScalar("SELECT COUNT(*) FROM data"), Is.EqualTo(1), "Unexpected data count");
        }

        // Scope disposed and not completed => rollback, but no enlistment, so changes should still be there.
        using (var conn3 = OpenConnection(ConnectionStringEnlistOff))
        {
            Assert.That(conn3.ExecuteScalar("SELECT COUNT(*) FROM data"), Is.EqualTo(1), "Insert unexpectedly rollback-ed");
        }
    }

    [Test, Description("Single connection enlisting explicitly, rollback")]
    public void Rollback_explicit_enlist()
    {
        using var conn = OpenConnection();
        using (new TransactionScope())
        {
            conn.EnlistTransaction(Transaction.Current);
            Assert.That(conn.ExecuteNonQuery(@"INSERT INTO data (name) VALUES ('test')"), Is.EqualTo(1), "Unexpected insert rowcount");
            // No commit
        }
        AssertNoDistributedIdentifier();
        AssertNoPreparedTransactions();
        using (var tx = conn.BeginTransaction())
        {
            Assert.That(conn.ExecuteScalar(@"SELECT COUNT(*) FROM data"), Is.EqualTo(0), "Unexpected data count");
            tx.Rollback();
        }
    }

    [Test, Description("Single connection enlisting implicitly, rollback")]
    [IssueLink("https://github.com/npgsql/npgsql/issues/2408")]
    public void Rollback_implicit_enlist([Values(true, false)] bool pooling)
    {
        var connectionString = new NpgsqlConnectionStringBuilder(ConnectionStringEnlistOn)
        {
            Pooling = pooling
        }.ToString();

        using (new TransactionScope())
        using (var conn = OpenConnection(connectionString))
        {
            Assert.That(conn.ExecuteNonQuery(@"INSERT INTO data (name) VALUES ('test')"), Is.EqualTo(1), "Unexpected insert rowcount");
            AssertNoDistributedIdentifier();
            AssertNoPreparedTransactions();
            // No commit
        }

        AssertNumberOfRows(0);
    }

    [Test]
    public void Two_consecutive_connections()
    {
        using (var scope = new TransactionScope())
        {
            using (var conn1 = OpenConnection(ConnectionStringEnlistOn))
            {
                Assert.That(conn1.ExecuteNonQuery(@"INSERT INTO data (name) VALUES ('test1')"), Is.EqualTo(1), "Unexpected first insert rowcount");
            }

            using (var conn2 = OpenConnection(ConnectionStringEnlistOn))
            {
                Assert.That(conn2.ExecuteNonQuery(@"INSERT INTO data (name) VALUES ('test2')"), Is.EqualTo(1), "Unexpected second insert rowcount");
            }

            // Consecutive connections used in same scope should not promote the transaction to distributed.
            AssertNoDistributedIdentifier();
            AssertNoPreparedTransactions();
            scope.Complete();
        }
        AssertNumberOfRows(2);
    }

    [Test]
    public void Close_connection()
    {
        var connString = new NpgsqlConnectionStringBuilder(ConnectionStringEnlistOn)
        {
            ApplicationName = nameof(Close_connection),
        }.ToString();
        using (var scope = new TransactionScope())
        using (var conn = OpenConnection(connString))
        {
            Assert.That(conn.ExecuteNonQuery(@"INSERT INTO data (name) VALUES ('test')"), Is.EqualTo(1), "Unexpected insert rowcount");
            conn.Close();
            AssertNoDistributedIdentifier();
            AssertNoPreparedTransactions();
            scope.Complete();
        }
        AssertNumberOfRows(1);
        Assert.True(PoolManager.Pools.TryGetValue(connString, out var pool));
        Assert.That(pool!.Statistics.Idle, Is.EqualTo(1));

        using (var conn = new NpgsqlConnection(connString))
            NpgsqlConnection.ClearPool(conn);
    }

    [Test]
    public void Enlist_to_two_transactions()
    {
        using var conn = OpenConnection(ConnectionStringEnlistOff);
        var ctx = new CommittableTransaction();
        conn.EnlistTransaction(ctx);
        Assert.That(() => conn.EnlistTransaction(new CommittableTransaction()), Throws.Exception.TypeOf<InvalidOperationException>());
        ctx.Rollback();

        using var tx = conn.BeginTransaction();
        Assert.That(conn.ExecuteScalar(@"SELECT COUNT(*) FROM data"), Is.EqualTo(0));
        tx.Rollback();
    }

    [Test]
    public void Enlist_twice_to_same_transaction()
    {
        using var conn = OpenConnection(ConnectionStringEnlistOff);
        var ctx = new CommittableTransaction();
        conn.EnlistTransaction(ctx);
        conn.EnlistTransaction(ctx);
        ctx.Rollback();

        using var tx = conn.BeginTransaction();
        Assert.That(conn.ExecuteScalar(@"SELECT COUNT(*) FROM data"), Is.EqualTo(0));
        tx.Rollback();
    }

    [Test]
    public void Scope_after_scope()
    {
        using var conn = OpenConnection(ConnectionStringEnlistOff);
        using (new TransactionScope())
            conn.EnlistTransaction(Transaction.Current);
        using (new TransactionScope())
            conn.EnlistTransaction(Transaction.Current);

        using (var tx = conn.BeginTransaction())
        {
            Assert.That(conn.ExecuteScalar(@"SELECT COUNT(*) FROM data"), Is.EqualTo(0));
            tx.Rollback();
        }
    }

    [Test]
    public void Reuse_connection()
    {
        using (var scope = new TransactionScope())
        using (var conn = new NpgsqlConnection(ConnectionStringEnlistOn))
        {
            conn.Open();
            var processId = conn.ProcessID;
            conn.ExecuteNonQuery(@"INSERT INTO data (name) VALUES ('test1')");
            conn.Close();

            conn.Open();
            Assert.That(conn.ProcessID, Is.EqualTo(processId));
            conn.ExecuteNonQuery(@"INSERT INTO data (name) VALUES ('test2')");
            conn.Close();

            scope.Complete();
        }
        AssertNumberOfRows(2);
    }

    [Test]
    public void Reuse_connection_rollback()
    {
        using (new TransactionScope())
        using (var conn = new NpgsqlConnection(ConnectionStringEnlistOn))
        {
            conn.Open();
            var processId = conn.ProcessID;
            conn.ExecuteNonQuery(@"INSERT INTO data (name) VALUES ('test1')");
            conn.Close();

            conn.Open();
            Assert.That(conn.ProcessID, Is.EqualTo(processId));
            conn.ExecuteNonQuery(@"INSERT INTO data (name) VALUES ('test2')");
            conn.Close();

            // No commit
        }
        AssertNumberOfRows(0);
    }

    [Test, Ignore("Timeout doesn't seem to fire on .NET Core / Linux")]
    public void Timeout_triggers_rollback_while_busy()
    {
        using (var conn = OpenConnection(ConnectionStringEnlistOff))
        {
            using (new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromSeconds(1)))
            {
                conn.EnlistTransaction(Transaction.Current);
                Assert.That(() => CreateSleepCommand(conn, 5).ExecuteNonQuery(),
                    Throws.Exception.TypeOf<PostgresException>()
                        .With.Property(nameof(PostgresException.SqlState))
                        .EqualTo(PostgresErrorCodes.QueryCanceled));

            }
        }
        AssertNumberOfRows(0);
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1579")]
    public void Schema_connection_should_not_enlist()
    {
        using var tran = new TransactionScope();
        using var conn = OpenConnection(ConnectionStringEnlistOn);
        using var cmd = new NpgsqlCommandOrig("SELECT * FROM data", conn);
        using var reader = cmd.ExecuteReader(CommandBehavior.KeyInfo);
        reader.GetColumnSchema();
        AssertNoDistributedIdentifier();
        AssertNoPreparedTransactions();
        tran.Complete();
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1737")]
    public void Single_unpooled_connection()
    {
        var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            Pooling = false,
            Enlist = true
        };


        using var scope = new TransactionScope();

        using (var conn = OpenConnection(csb))
        using (var cmd = new NpgsqlCommandOrig("SELECT 1", conn))
            cmd.ExecuteNonQuery();

        scope.Complete();
    }

    [Test]
    [IssueLink("https://github.com/npgsql/npgsql/issues/3863")]
    public void Break_connector_while_in_transaction_scope_with_rollback([Values] bool pooling)
    {
        var csb = new NpgsqlConnectionStringBuilder(ConnectionStringEnlistOn)
        {
            Pooling = pooling,
        };

        using var scope = new TransactionScope();
        var conn = OpenConnection(csb);

        conn.ExecuteNonQuery("SELECT 1");
        conn.Connector!.Break(new Exception(nameof(Break_connector_while_in_transaction_scope_with_rollback)));
    }

    [Test]
    [IssueLink("https://github.com/npgsql/npgsql/issues/3863")]
    public void Break_connector_while_in_transaction_scope_with_commit([Values] bool pooling)
    {
        var csb = new NpgsqlConnectionStringBuilder(ConnectionStringEnlistOn)
        {
            Pooling = pooling,
        };

        var ex = Assert.Throws<TransactionInDoubtException>(() =>
        {
            using var scope = new TransactionScope();
            var conn = OpenConnection(csb);

            conn.ExecuteNonQuery("SELECT 1");
            conn.Connector!.Break(new Exception(nameof(Break_connector_while_in_transaction_scope_with_commit)));

            scope.Complete();
        })!;
        Assert.That(ex.InnerException, Is.TypeOf<ObjectDisposedException>());
        Assert.That(ex.InnerException!.InnerException, Is.TypeOf<Exception>());
        Assert.That(ex.InnerException!.InnerException!.Message, Is.EqualTo(nameof(Break_connector_while_in_transaction_scope_with_commit)));
    }

    [Test]
    [IssueLink("https://github.com/npgsql/npgsql/issues/4085")]
    public void Open_connection_with_enlist_and_aborted_TransactionScope()
    {
        var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            Enlist = true
        };

        for (var i = 0; i < 2; i++)
        {
            using var outerScope = new TransactionScope();

            try
            {
                using var innerScope = new TransactionScope();
                throw new Exception("Random exception to abort the transaction scope");
            }
            catch (Exception)
            {
            }

            var ex = Assert.Throws<TransactionException>(() => OpenConnection(csb))!;
            Assert.That(ex.Message, Is.EqualTo("The operation is not valid for the state of the transaction."));
        }
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1594")]
    public void Bug1594()
    {
        using var outerScope = new TransactionScope();

        using (var conn = OpenConnection(ConnectionStringEnlistOn))
        using (var innerScope1 = new TransactionScope())
        {
            conn.ExecuteNonQuery(@"INSERT INTO data (name) VALUES ('test1')");
            innerScope1.Complete();
        }

        using (OpenConnection(ConnectionStringEnlistOn))
        using (new TransactionScope())
        {
            // Don't complete, triggering rollback
        }
    }

    #region Utilities

    void AssertNoPreparedTransactions()
        => Assert.That(GetNumberOfPreparedTransactions(), Is.EqualTo(0), "Prepared transactions found");

    int GetNumberOfPreparedTransactions()
    {
        using var conn = OpenConnection(ConnectionStringEnlistOff);
        using var cmd = new NpgsqlCommandOrig("SELECT COUNT(*) FROM pg_prepared_xacts WHERE database = @database", conn);
        cmd.Parameters.Add(new NpgsqlParameter("database", conn.Database));
        return (int)(long)cmd.ExecuteScalar()!;
    }

    void AssertNumberOfRows(int expected)
        => Assert.That(_controlConn.ExecuteScalar(@"SELECT COUNT(*) FROM data"), Is.EqualTo(expected), "Unexpected data count");

    static void AssertNoDistributedIdentifier()
        => Assert.That(Transaction.Current?.TransactionInformation.DistributedIdentifier ?? Guid.Empty, Is.EqualTo(Guid.Empty), "Distributed identifier found");

    public readonly string ConnectionStringEnlistOn;
    public readonly string ConnectionStringEnlistOff;

    #endregion Utilities

    #region Setup

    public SystemTransactionTests()
    {
        ConnectionStringEnlistOn = new NpgsqlConnectionStringBuilder(ConnectionString) { Enlist = true }.ToString();
        ConnectionStringEnlistOff = new NpgsqlConnectionStringBuilder(ConnectionString) { Enlist = false }.ToString();
    }

    NpgsqlConnection _controlConn = default!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _controlConn = OpenConnection();

        // All tests in this fixture should have exclusive access to the database they're running on.
        // If we run these tests in parallel (i.e. two builds in parallel) they will interfere.
        // Solve this by taking a PostgreSQL advisory lock for the lifetime of the fixture.
        _controlConn.ExecuteNonQuery("SELECT pg_advisory_lock(666)");

        _controlConn.ExecuteNonQuery("DROP TABLE IF EXISTS data");
        _controlConn.ExecuteNonQuery("CREATE TABLE data (name TEXT)");
    }

    [SetUp]
    public void SetUp()
    {
        _controlConn.ExecuteNonQuery("TRUNCATE data");
    }

#pragma warning disable CS8625
    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _controlConn?.Close();
        _controlConn = null;
    }
#pragma warning restore CS8625

    #endregion
}
