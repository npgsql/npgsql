using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using NUnit.Framework;
using static Npgsql.Tests.TestUtil;

namespace Npgsql.Tests;

// This test suite contains ambient transaction tests, except those involving distributed transactions which are only
// supported on .NET Framework / Windows. Distributed transaction tests are in DistributedTransactionTests.
public class SystemTransactionTests : TestBase
{
    [Test, Description("Single connection enlisting explicitly, committing")]
    public void Explicit_enlist()
    {
        var dataSource = EnlistOffDataSource;
        var tableName = CreateTempTable(dataSource, "name TEXT");
        using var conn = dataSource.OpenConnection();
        using (var scope = new TransactionScope())
        {
            conn.EnlistTransaction(Transaction.Current);
            Assert.That(conn.ExecuteNonQuery(@$"INSERT INTO {tableName} (name) VALUES ('test')"), Is.EqualTo(1), "Unexpected insert rowcount");
            AssertNoDistributedIdentifier();
            AssertNoPreparedTransactions();
            scope.Complete();
        }
        AssertNoDistributedIdentifier();
        AssertNoPreparedTransactions();
        using (var tx = conn.BeginTransaction())
        {
            Assert.That(conn.ExecuteScalar(@$"SELECT COUNT(*) FROM {tableName}"), Is.EqualTo(1), "Unexpected data count");
            tx.Rollback();
        }
    }

    [Test, Description("Single connection enlisting implicitly, committing")]
    public void Implicit_enlist()
    {
        var dataSource = EnlistOnDataSource;
        var tableName = CreateTempTable(dataSource, "name TEXT");
        using var conn = dataSource.CreateConnection();
        using (var scope = new TransactionScope())
        {
            conn.Open();
            Assert.That(conn.ExecuteNonQuery(@$"INSERT INTO {tableName} (name) VALUES ('test')"), Is.EqualTo(1), "Unexpected insert rowcount");
            AssertNoDistributedIdentifier();
            AssertNoPreparedTransactions();
            scope.Complete();
        }
        using (var tx = conn.BeginTransaction())
        {
            Assert.That(conn.ExecuteScalar(@$"SELECT COUNT(*) FROM {tableName}"), Is.EqualTo(1), "Unexpected data count");
            tx.Rollback();
        }
    }

    [Test]
    public void Enlist_Off()
    {
        var dataSource = EnlistOffDataSource;
        var tableName = CreateTempTable(dataSource, "name TEXT");
        using (new TransactionScope())
        using (var conn1 = dataSource.OpenConnection())
        using (var conn2 = dataSource.OpenConnection())
        {
            Assert.That(conn1.EnlistedTransaction, Is.Null);
            Assert.That(conn1.ExecuteNonQuery(@$"INSERT INTO {tableName} (name) VALUES ('test')"), Is.EqualTo(1), "Unexpected insert rowcount");
            Assert.That(conn2.ExecuteScalar($"SELECT COUNT(*) FROM {tableName}"), Is.EqualTo(1), "Unexpected data count");
        }

        // Scope disposed and not completed => rollback, but no enlistment, so changes should still be there.
        using (var conn3 = dataSource.OpenConnection())
        {
            Assert.That(conn3.ExecuteScalar($"SELECT COUNT(*) FROM {tableName}"), Is.EqualTo(1), "Insert unexpectedly rollback-ed");
        }
    }

    [Test, Description("Single connection enlisting explicitly, rollback")]
    public void Rollback_explicit_enlist()
    {
        using var dataSource = CreateDataSource();
        var tableName = CreateTempTable(dataSource, "name TEXT");
        using var conn = dataSource.OpenConnection();
        using (new TransactionScope())
        {
            conn.EnlistTransaction(Transaction.Current);
            Assert.That(conn.ExecuteNonQuery(@$"INSERT INTO {tableName} (name) VALUES ('test')"), Is.EqualTo(1), "Unexpected insert rowcount");
            // No commit
        }
        AssertNoDistributedIdentifier();
        AssertNoPreparedTransactions();
        using (var tx = conn.BeginTransaction())
        {
            Assert.That(conn.ExecuteScalar(@$"SELECT COUNT(*) FROM {tableName}"), Is.EqualTo(0), "Unexpected data count");
            tx.Rollback();
        }
    }

    [Test, Description("Single connection enlisting implicitly, rollback")]
    [IssueLink("https://github.com/npgsql/npgsql/issues/2408")]
    public void Rollback_implicit_enlist([Values(true, false)] bool pooling)
    {
        using var dataSource = CreateDataSource(csb => csb.Pooling = pooling);
        var tableName = CreateTempTable(dataSource, "name TEXT");

        using (new TransactionScope())
        using (var conn = dataSource.OpenConnection())
        {
            Assert.That(conn.ExecuteNonQuery(@$"INSERT INTO {tableName} (name) VALUES ('test')"), Is.EqualTo(1), "Unexpected insert rowcount");
            AssertNoDistributedIdentifier();
            AssertNoPreparedTransactions();
            // No commit
        }

        AssertNumberOfRows(0, tableName);
    }

    [Test]
    public void Two_consecutive_connections()
    {
        var dataSource = EnlistOnDataSource;
        var tableName = CreateTempTable(dataSource, "name TEXT");
        using (var scope = new TransactionScope())
        {
            using (var conn1 = dataSource.OpenConnection())
            {
                Assert.That(conn1.ExecuteNonQuery(@$"INSERT INTO {tableName} (name) VALUES ('test1')"), Is.EqualTo(1), "Unexpected first insert rowcount");
            }

            using (var conn2 = dataSource.OpenConnection())
            {
                Assert.That(conn2.ExecuteNonQuery(@$"INSERT INTO {tableName} (name) VALUES ('test2')"), Is.EqualTo(1), "Unexpected second insert rowcount");
            }

            // Consecutive connections used in same scope should not promote the transaction to distributed.
            AssertNoDistributedIdentifier();
            AssertNoPreparedTransactions();
            scope.Complete();
        }
        AssertNumberOfRows(2, tableName);
    }

    [Test]
    public void Close_connection()
    {
        // We assert the number of idle connections below
        using var dataSource = CreateDataSource(csb => csb.Enlist = true);
        var tableName = CreateTempTable(dataSource, "name TEXT");
        using (var scope = new TransactionScope())
        using (var conn = dataSource.OpenConnection())
        {
            Assert.That(conn.ExecuteNonQuery(@$"INSERT INTO {tableName} (name) VALUES ('test')"), Is.EqualTo(1), "Unexpected insert rowcount");
            conn.Close();
            AssertNoDistributedIdentifier();
            AssertNoPreparedTransactions();
            scope.Complete();
        }
        AssertNumberOfRows(1, tableName);
        Assert.That(dataSource.Statistics.Idle, Is.EqualTo(1));
    }

    [Test]
    public void Enlist_to_two_transactions()
    {
        var dataSource = EnlistOffDataSource;
        var tableName = CreateTempTable(dataSource, "name TEXT");
        using var conn = dataSource.OpenConnection();
        var ctx = new CommittableTransaction();
        conn.EnlistTransaction(ctx);
        Assert.That(() => conn.EnlistTransaction(new CommittableTransaction()), Throws.Exception.TypeOf<InvalidOperationException>());
        ctx.Rollback();

        using var tx = conn.BeginTransaction();
        Assert.That(conn.ExecuteScalar(@$"SELECT COUNT(*) FROM {tableName}"), Is.EqualTo(0));
        tx.Rollback();
    }

    [Test]
    public void Enlist_twice_to_same_transaction()
    {
        var dataSource = EnlistOffDataSource;
        var tableName = CreateTempTable(dataSource, "name TEXT");
        using var conn = dataSource.OpenConnection();
        var ctx = new CommittableTransaction();
        conn.EnlistTransaction(ctx);
        conn.EnlistTransaction(ctx);
        ctx.Rollback();

        using var tx = conn.BeginTransaction();
        Assert.That(conn.ExecuteScalar(@$"SELECT COUNT(*) FROM {tableName}"), Is.EqualTo(0));
        tx.Rollback();
    }

    [Test]
    public void Scope_after_scope()
    {
        var dataSource = EnlistOffDataSource;
        var tableName = CreateTempTable(dataSource, "name TEXT");
        using var conn = dataSource.OpenConnection();
        using (new TransactionScope())
            conn.EnlistTransaction(Transaction.Current);
        using (new TransactionScope())
            conn.EnlistTransaction(Transaction.Current);

        using (var tx = conn.BeginTransaction())
        {
            Assert.That(conn.ExecuteScalar(@$"SELECT COUNT(*) FROM {tableName}"), Is.EqualTo(0));
            tx.Rollback();
        }
    }

    [Test]
    public void Reuse_connection()
    {
        // We check the ProcessID below
        using var dataSource = CreateDataSource(csb => csb.Enlist = true);
        var tableName = CreateTempTable(dataSource, "name TEXT");
        using (var scope = new TransactionScope())
        using (var conn = dataSource.CreateConnection())
        {
            conn.Open();
            var processId = conn.ProcessID;
            conn.ExecuteNonQuery(@$"INSERT INTO {tableName} (name) VALUES ('test1')");
            conn.Close();

            conn.Open();
            Assert.That(conn.ProcessID, Is.EqualTo(processId));
            conn.ExecuteNonQuery(@$"INSERT INTO {tableName} (name) VALUES ('test2')");
            conn.Close();

            scope.Complete();
        }
        AssertNumberOfRows(2, tableName);
    }

    [Test]
    public void Reuse_connection_rollback()
    {
        // We check the ProcessID below
        using var dataSource = CreateDataSource(csb => csb.Enlist = true);
        var tableName = CreateTempTable(dataSource, "name TEXT");
        using (new TransactionScope())
        using (var conn = dataSource.CreateConnection())
        {
            conn.Open();
            var processId = conn.ProcessID;
            conn.ExecuteNonQuery(@$"INSERT INTO {tableName} (name) VALUES ('test1')");
            conn.Close();

            conn.Open();
            Assert.That(conn.ProcessID, Is.EqualTo(processId));
            conn.ExecuteNonQuery(@$"INSERT INTO {tableName} (name) VALUES ('test2')");
            conn.Close();

            // No commit
        }
        AssertNumberOfRows(0, tableName);
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/3735")]
    public void Reuse_connection_resets_temp_tables()
    {
        // When a connection is closed inside a TransactionScope and then reopened,
        // temp tables should be discarded.
        using var dataSource = CreateDataSource(csb => csb.Enlist = true);
        using (new TransactionScope())
        using (var conn = dataSource.CreateConnection())
        {
            conn.Open();
            var processId = conn.ProcessID;

            // Create a temp table
            conn.ExecuteNonQuery("CREATE TEMP TABLE temp_test (id INT)");

            conn.Close();

            // Reopen - should get the same physical connection but with reset state
            conn.Open();
            Assert.That(conn.ProcessID, Is.EqualTo(processId), "Should reuse the same physical connection");

            // The temp table should have been discarded
            Assert.That(() => conn.ExecuteScalar("SELECT COUNT(*) FROM temp_test"),
                Throws.Exception.TypeOf<PostgresException>()
                    .With.Property(nameof(PostgresException.SqlState)).EqualTo(PostgresErrorCodes.UndefinedTable));
        }
    }

    [Test, Ignore("Timeout doesn't seem to fire on .NET Core / Linux")]
    public void Timeout_triggers_rollback_while_busy()
    {
        var dataSource = EnlistOffDataSource;
        var tableName = CreateTempTable(dataSource, "name TEXT");
        using (var conn = dataSource.OpenConnection())
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
        AssertNumberOfRows(0, tableName);
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1579")]
    public void Schema_connection_should_not_enlist()
    {
        var dataSource = EnlistOnDataSource;
        var tableName = CreateTempTable(dataSource, "name TEXT");
        using var tran = new TransactionScope();
        using var conn = dataSource.OpenConnection();
        using var cmd = new NpgsqlCommand($"SELECT * FROM {tableName}", conn);
        using var reader = cmd.ExecuteReader(CommandBehavior.KeyInfo);
        reader.GetColumnSchema();
        AssertNoDistributedIdentifier();
        AssertNoPreparedTransactions();
        tran.Complete();
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1737")]
    public void Single_unpooled_connection()
    {
        using var dataSource = CreateDataSource(csb =>
        {
            csb.Pooling = false;
            csb.Enlist = true;
        });
        using var scope = new TransactionScope();

        using (var conn = dataSource.OpenConnection())
        using (var cmd = new NpgsqlCommand("SELECT 1", conn))
            cmd.ExecuteNonQuery();

        scope.Complete();
    }

    [Test]
    [IssueLink("https://github.com/npgsql/npgsql/issues/4963"), IssueLink("https://github.com/npgsql/npgsql/issues/5783")]
    public void Single_closed_connection_in_transaction_scope([Values] bool pooling, [Values] bool multipleHosts)
    {
        using var dataSource = CreateDataSource(csb =>
        {
            csb.Pooling = pooling;
            csb.Enlist = true;
            csb.Host = multipleHosts ? "localhost,127.0.0.1" : csb.Host;
        });

        using (var scope = new TransactionScope())
        using (var conn = dataSource.OpenConnection())
        using (var cmd = new NpgsqlCommand("SELECT 1", conn))
        {
            cmd.ExecuteNonQuery();
            conn.Close();
            Assert.That(pooling ? dataSource.Statistics.Busy : dataSource.Statistics.Total, Is.EqualTo(1));
            scope.Complete();
        }

        Assert.That(pooling ? dataSource.Statistics.Busy : dataSource.Statistics.Total, Is.EqualTo(0));
    }

    [Test]
    [IssueLink("https://github.com/npgsql/npgsql/issues/6766")]
    public async Task Rollback_to_savepoint_after_connection_closed_in_failed_transaction([Values] bool async)
    {
        using var dataSource = CreateDataSource(csb => csb.Enlist = true);
        var table = "temp_rows_" + Guid.NewGuid().ToString("N");
        using (var setup = dataSource.OpenConnection())
            setup.ExecuteNonQuery($"CREATE TABLE {table} (id int)");

        try
        {
            using (var scope = new TransactionScope())
            {
                using (var conn = dataSource.OpenConnection())
                {
                    conn.ExecuteNonQuery($"INSERT INTO {table} VALUES (1)");
                    conn.ExecuteNonQuery("SAVEPOINT sp");
                }

                using (var conn = dataSource.OpenConnection())
                {
                    conn.ExecuteNonQuery($"INSERT INTO {table} VALUES (2)");
                    Assert.That(() => conn.ExecuteNonQuery("SELECT 1/0"), Throws.Exception.TypeOf<PostgresException>().With.Property(nameof(PostgresException.SqlState)).EqualTo("22012"));
                    // The connection is closed while the transaction is in a failed state
                    if (async)
                        await conn.CloseAsync();
                }

                using (var conn = dataSource.OpenConnection())
                {
                    conn.ExecuteNonQuery("ROLLBACK TO SAVEPOINT sp");
                    conn.ExecuteNonQuery($"INSERT INTO {table} VALUES (3)");
                }

                scope.Complete();
            }

            using var verify = dataSource.OpenConnection();
            Assert.That(verify.ExecuteScalar($"SELECT string_agg(id::text, ',' ORDER BY id) FROM {table}"), Is.EqualTo("1,3"));
        }
        finally
        {
            using var cleanup = dataSource.OpenConnection();
            cleanup.ExecuteNonQuery($"DROP TABLE IF EXISTS {table}");
        }
    }

    [Test]
    [IssueLink("https://github.com/npgsql/npgsql/issues/6766")]
    public void Reset_is_sent_when_connection_closed_in_failed_transaction_and_transaction_ends()
    {
        using var dataSource = CreateDataSource(csb =>
        {
            csb.Enlist = true;
            csb.MaxPoolSize = 1;
        });
        const long lockId = 6766;
        object? pid;

        using (var scope = new TransactionScope())
        using (var conn = dataSource.OpenConnection())
        {
            pid = conn.ExecuteScalar("SELECT pg_backend_pid()");
            conn.ExecuteNonQuery($"SELECT pg_advisory_lock({lockId})");
            Assert.That(() => conn.ExecuteNonQuery("SELECT 1/0"), Throws.Exception.TypeOf<PostgresException>().With.Property(nameof(PostgresException.SqlState)).EqualTo("22012"));
            conn.Close();
            // The scope isn't completed, so the transaction is rolled back
        }

        // The same physical connection is reused, and its session state must have been reset
        using var conn2 = dataSource.OpenConnection();
        Assert.That(conn2.ExecuteScalar("SELECT pg_backend_pid()"), Is.EqualTo(pid), "Expected the same physical connection to be reused");
        Assert.That(conn2.ExecuteScalar($"SELECT pg_advisory_unlock({lockId})"), Is.False);
    }

    [Test]
    [IssueLink("https://github.com/npgsql/npgsql/issues/6766")]
    public void Reset_is_sent_when_connection_reopened_after_failed_close_and_closed_after_recovery()
    {
        using var dataSource = CreateDataSource(csb =>
        {
            csb.Enlist = true;
            csb.MaxPoolSize = 1;
        });
        const long lockId = 6767;
        object? pid;

        using (var scope = new TransactionScope())
        {
            using (var conn = dataSource.OpenConnection())
            {
                pid = conn.ExecuteScalar("SELECT pg_backend_pid()");
                conn.ExecuteNonQuery("SAVEPOINT sp");
                conn.ExecuteNonQuery($"SELECT pg_advisory_lock({lockId})");
                Assert.That(() => conn.ExecuteNonQuery("SELECT 1/0"), Throws.Exception.TypeOf<PostgresException>().With.Property(nameof(PostgresException.SqlState)).EqualTo("22012"));
            }

            using (var conn = dataSource.OpenConnection())
                conn.ExecuteNonQuery("ROLLBACK TO SAVEPOINT sp");

            scope.Complete();
        }

        using var conn2 = dataSource.OpenConnection();
        Assert.That(conn2.ExecuteScalar("SELECT pg_backend_pid()"), Is.EqualTo(pid), "Expected the same physical connection to be reused");
        Assert.That(conn2.ExecuteScalar($"SELECT pg_advisory_unlock({lockId})"), Is.False);
    }

    [Test]
    [IssueLink("https://github.com/npgsql/npgsql/issues/6766")]
    public void Reset_is_not_duplicated_when_reopened_connection_outlives_failed_close_and_transaction()
    {
        using var dataSource = CreateDataSource(csb =>
        {
            csb.Enlist = true;
            csb.MaxPoolSize = 1;
        });
        const long lockId = 6768;
        NpgsqlConnection outer;
        object? pid;

        using (var scope = new TransactionScope())
        {
            using (var conn = dataSource.OpenConnection())
            {
                pid = conn.ExecuteScalar("SELECT pg_backend_pid()");
                conn.ExecuteNonQuery($"SELECT pg_advisory_lock({lockId})");
                Assert.That(() => conn.ExecuteNonQuery("SELECT 1/0"), Throws.Exception.TypeOf<PostgresException>().With.Property(nameof(PostgresException.SqlState)).EqualTo("22012"));
            }

            // Reopened (the same physical connection) and left open past the end of the transaction
            outer = dataSource.OpenConnection();
        }

        outer.Dispose();

        using var conn2 = dataSource.OpenConnection();
        Assert.That(conn2.ExecuteScalar("SELECT pg_backend_pid()"), Is.EqualTo(pid), "Expected the same physical connection to be reused");
        Assert.That(conn2.ExecuteScalar($"SELECT pg_advisory_unlock({lockId})"), Is.False);
        Assert.That(conn2.ExecuteScalar("SELECT 1"), Is.EqualTo(1));
    }

    [Test]
    [IssueLink("https://github.com/npgsql/npgsql/issues/6766")]
    public async Task Rollback_to_savepoint_after_connection_closed_in_failed_transaction_async_flow()
    {
        await using var dataSource = CreateDataSource(csb =>
        {
            csb.Enlist = true;
            csb.MaxPoolSize = 1;
        });
        var table = "temp_rows_" + Guid.NewGuid().ToString("N");
        await using (var setup = await dataSource.OpenConnectionAsync())
            await setup.ExecuteNonQueryAsync($"CREATE TABLE {table} (id int)");

        try
        {
            object? pid;
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await using (var conn = await dataSource.OpenConnectionAsync())
                {
                    pid = await conn.ExecuteScalarAsync("SELECT pg_backend_pid()");
                    await conn.ExecuteNonQueryAsync($"INSERT INTO {table} VALUES (1)");
                    await conn.ExecuteNonQueryAsync("SAVEPOINT sp");
                }

                await using (var conn = await dataSource.OpenConnectionAsync())
                {
                    await conn.ExecuteNonQueryAsync($"INSERT INTO {table} VALUES (2)");
                    Assert.That(async () => await conn.ExecuteNonQueryAsync("SELECT 1/0"),
                        Throws.Exception.TypeOf<PostgresException>().With.Property(nameof(PostgresException.SqlState)).EqualTo("22012"));
                }

                await using (var conn = await dataSource.OpenConnectionAsync())
                {
                    await conn.ExecuteNonQueryAsync("ROLLBACK TO SAVEPOINT sp");
                    Assert.That(await conn.ExecuteScalarAsync("SELECT pg_backend_pid()"), Is.EqualTo(pid));
                    await conn.ExecuteNonQueryAsync($"INSERT INTO {table} VALUES (3)");
                }

                scope.Complete();
            }

            await using var verify = await dataSource.OpenConnectionAsync();
            Assert.That(await verify.ExecuteScalarAsync($"SELECT string_agg(id::text, ',' ORDER BY id) FROM {table}"), Is.EqualTo("1,3"));
        }
        finally
        {
            await using var cleanup = await dataSource.OpenConnectionAsync();
            await cleanup.ExecuteNonQueryAsync($"DROP TABLE IF EXISTS {table}");
        }
    }

    [Test]
    [IssueLink("https://github.com/npgsql/npgsql/issues/3863")]
    public void Break_connector_while_in_transaction_scope_with_rollback([Values] bool pooling)
    {
        using var dataSource = CreateDataSource(csb => csb.Pooling = pooling);
        using var scope = new TransactionScope();
        var conn = dataSource.OpenConnection();

        conn.ExecuteNonQuery("SELECT 1");
        conn.Connector!.Break(new Exception(nameof(Break_connector_while_in_transaction_scope_with_rollback)));
    }

    [Test]
    [IssueLink("https://github.com/npgsql/npgsql/issues/3863")]
    public void Break_connector_while_in_transaction_scope_with_commit([Values] bool pooling)
    {
        using var dataSource = CreateDataSource(csb => csb.Pooling = pooling);
        var ex = Assert.Throws<TransactionInDoubtException>(() =>
        {
            using var scope = new TransactionScope();
            var conn = dataSource.OpenConnection();

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
        var dataSource = EnlistOnDataSource;
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

            var ex = Assert.Throws<TransactionException>(() => dataSource.OpenConnection())!;
            Assert.That(ex.Message, Is.EqualTo("The operation is not valid for the state of the transaction."));
        }
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1594")]
    public void Bug1594()
    {
        var dataSource = EnlistOnDataSource;
        var tableName = CreateTempTable(dataSource, "name TEXT");
        using var outerScope = new TransactionScope();

        using (var conn = dataSource.OpenConnection())
        using (var innerScope1 = new TransactionScope())
        {
            conn.ExecuteNonQuery(@$"INSERT INTO {tableName} (name) VALUES ('test1')");
            innerScope1.Complete();
        }

        using (dataSource.OpenConnection())
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
        var dataSource = EnlistOffDataSource;
        using var conn = dataSource.OpenConnection();
        using var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM pg_prepared_xacts WHERE database = @database", conn);
        cmd.Parameters.Add(new NpgsqlParameter("database", conn.Database));
        return (int)(long)cmd.ExecuteScalar()!;
    }

    void AssertNumberOfRows(int expected, string tableName)
    {
        using var conn = OpenConnection();
        Assert.That(conn.ExecuteScalar(@$"SELECT COUNT(*) FROM {tableName}"), Is.EqualTo(expected), "Unexpected data count");
    }

    static void AssertNoDistributedIdentifier()
        => Assert.That(Transaction.Current?.TransactionInformation.DistributedIdentifier ?? Guid.Empty, Is.EqualTo(Guid.Empty), "Distributed identifier found");

    #endregion Utilities

    #region Setup

    NpgsqlDataSource EnlistOnDataSource { get; set; } = default!;

    NpgsqlDataSource EnlistOffDataSource { get; set; } = default!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        EnlistOnDataSource = CreateDataSource(csb => csb.Enlist = true);
        EnlistOffDataSource = CreateDataSource(csb => csb.Enlist = false);
    }

    [OneTimeTearDown]
    public void OnTimeTearDown()
    {
        EnlistOnDataSource?.Dispose();
        EnlistOnDataSource = null!;
        EnlistOffDataSource?.Dispose();
        EnlistOffDataSource = null!;
    }

    internal static string CreateTempTable(NpgsqlDataSource dataSource, string columns)
    {
        var tableName = "temp_table" + Interlocked.Increment(ref _tempTableCounter);
        dataSource.ExecuteNonQuery(@$"
START TRANSACTION; SELECT pg_advisory_xact_lock(0);
DROP TABLE IF EXISTS {tableName} CASCADE;
COMMIT;
CREATE TABLE {tableName} ({columns})");
        return tableName;
    }

    #endregion
}
