using System;
using System.Data;
using System.Threading.Tasks;
using Npgsql.Internal;
using Npgsql.Tests.Support;
using Npgsql.Util;
using NUnit.Framework;
using static Npgsql.Tests.TestUtil;

// ReSharper disable MethodHasAsyncOverload
// ReSharper disable UseAwaitUsing

namespace Npgsql.Tests;

public class TransactionTests : MultiplexingTestBase
{
    [Test, Description("Basic insert within a commited transaction")]
    public async Task Commit([Values(PrepareOrNot.NotPrepared, PrepareOrNot.Prepared)] PrepareOrNot prepare)
    {
        if (prepare == PrepareOrNot.Prepared && IsMultiplexing)
            return;

        await using var conn = await OpenConnectionAsync();
        var table = await CreateTempTable(conn, "name TEXT");

        var tx = await conn.BeginTransactionAsync();
        await using (tx)
        {
            var cmd = new NpgsqlCommand($"INSERT INTO {table} (name) VALUES ('X')", conn, tx);
            if (prepare == PrepareOrNot.Prepared)
                cmd.Prepare();
            cmd.ExecuteNonQuery();
            Assert.That(conn.ExecuteScalar($"SELECT COUNT(*) FROM {table}"), Is.EqualTo(1));
            tx.Commit();
            Assert.That(tx.IsCompleted);
            Assert.That(() => tx.Connection, Throws.Nothing);
            Assert.That(await conn.ExecuteScalarAsync($"SELECT COUNT(*) FROM {table}"), Is.EqualTo(1));
        }

        // With multiplexing we can't assume that disposed NpgsqlTransaction will throw ObjectDisposedException
        // Because disposed NpgsqlTransaction might be reused by another thread
        if (!IsMultiplexing)
            Assert.That(() => tx.Connection, Throws.Exception.TypeOf<ObjectDisposedException>());
    }

    [Test, Description("Basic insert within a commited transaction")]
    public async Task CommitAsync([Values(PrepareOrNot.NotPrepared, PrepareOrNot.Prepared)] PrepareOrNot prepare)
    {
        if (prepare == PrepareOrNot.Prepared && IsMultiplexing)
            return;

        await using var conn = await OpenConnectionAsync();
        var table = await CreateTempTable(conn, "name TEXT");

        var tx = await conn.BeginTransactionAsync();
        await using (tx)
        {
            var cmd = new NpgsqlCommand($"INSERT INTO {table} (name) VALUES ('X')", conn, tx);
            if (prepare == PrepareOrNot.Prepared)
                cmd.Prepare();
            await cmd.ExecuteNonQueryAsync();
            Assert.That(conn.ExecuteScalar($"SELECT COUNT(*) FROM {table}"), Is.EqualTo(1));
            await tx.CommitAsync();
            Assert.That(tx.IsCompleted);
            Assert.That(() => tx.Connection, Throws.Nothing);
            Assert.That(await conn.ExecuteScalarAsync($"SELECT COUNT(*) FROM {table}"), Is.EqualTo(1));
        }

        Assert.That(() => tx.Connection, Throws.Exception.TypeOf<ObjectDisposedException>());
    }

    [Test, Description("Basic insert within a rolled back transaction")]
    public async Task Rollback([Values(PrepareOrNot.NotPrepared, PrepareOrNot.Prepared)] PrepareOrNot prepare)
    {
        if (prepare == PrepareOrNot.Prepared && IsMultiplexing)
            return;

        await using var conn = await OpenConnectionAsync();
        var table = await CreateTempTable(conn, "name TEXT");

        var tx = await conn.BeginTransactionAsync();
        await using (tx)
        {
            var cmd = new NpgsqlCommand($"INSERT INTO {table} (name) VALUES ('X')", conn, tx);
            if (prepare == PrepareOrNot.Prepared)
                cmd.Prepare();
            cmd.ExecuteNonQuery();
            Assert.That(conn.ExecuteScalar($"SELECT COUNT(*) FROM {table}"), Is.EqualTo(1));
            tx.Rollback();
            Assert.That(tx.IsCompleted);
            Assert.That(() => tx.Connection, Throws.Nothing);
            Assert.That(await conn.ExecuteScalarAsync($"SELECT COUNT(*) FROM {table}"), Is.EqualTo(0));
        }

        Assert.That(() => tx.Connection, Throws.Exception.TypeOf<ObjectDisposedException>());
    }

    [Test, Description("Basic insert within a rolled back transaction")]
    public async Task RollbackAsync([Values(PrepareOrNot.NotPrepared, PrepareOrNot.Prepared)] PrepareOrNot prepare)
    {
        if (prepare == PrepareOrNot.Prepared && IsMultiplexing)
            return;

        await using var conn = await OpenConnectionAsync();
        var table = await CreateTempTable(conn, "name TEXT");

        var tx = await conn.BeginTransactionAsync();
        await using (tx)
        {
            var cmd = new NpgsqlCommand($"INSERT INTO {table} (name) VALUES ('X')", conn, tx);
            if (prepare == PrepareOrNot.Prepared)
                cmd.Prepare();
            await cmd.ExecuteNonQueryAsync();
            Assert.That(conn.ExecuteScalar($"SELECT COUNT(*) FROM {table}"), Is.EqualTo(1));
            await tx.RollbackAsync();
            Assert.That(tx.IsCompleted);
            Assert.That(() => tx.Connection, Throws.Nothing);
            Assert.That(await conn.ExecuteScalarAsync($"SELECT COUNT(*) FROM {table}"), Is.EqualTo(0));
        }

        Assert.That(() => tx.Connection, Throws.Exception.TypeOf<ObjectDisposedException>());
    }

    [Test, Description("Dispose a transaction in progress, should roll back")]
    public async Task Rollback_on_Dispose()
    {
        await using var conn = await OpenConnectionAsync();
        var table = await CreateTempTable(conn, "name TEXT");

        await using (var tx = await conn.BeginTransactionAsync())
        {
            await conn.ExecuteNonQueryAsync($"INSERT INTO {table} (name) VALUES ('X')", tx: tx);
        }
        Assert.That(await conn.ExecuteScalarAsync($"SELECT COUNT(*) FROM {table}"), Is.EqualTo(0));
    }

    [Test]
    public async Task Rollback_on_Close()
    {
        await using var conn1 = await OpenConnectionAsync();
        var table = await CreateTempTable(conn1, "name TEXT");

        using (var conn2 = await OpenConnectionAsync())
        {
            var tx = await conn2.BeginTransactionAsync();
            await conn2.ExecuteNonQueryAsync($"INSERT INTO {table} (name) VALUES ('X')", tx);
        }

        Assert.That(await conn1.ExecuteScalarAsync($"SELECT COUNT(*) FROM {table}"), Is.EqualTo(0));
    }

    [Test, Description("Intentionally generates an error, putting us in a failed transaction block. Rolls back.")]
    public async Task Rollback_failed()
    {
        await using var conn = await OpenConnectionAsync();
        var table = await CreateTempTable(conn, "name TEXT");

        await using var tx = await conn.BeginTransactionAsync();
        await conn.ExecuteNonQueryAsync($"INSERT INTO {table} (name) VALUES ('X')", tx: tx);
        Assert.That(async () => await conn.ExecuteNonQueryAsync("BAD QUERY"), Throws.Exception.TypeOf<PostgresException>());
        tx.Rollback();
        Assert.That(tx.IsCompleted);
        Assert.That(await conn.ExecuteScalarAsync($"SELECT COUNT(*) FROM {table}"), Is.EqualTo(0));
    }

    [Test, Description("Commits an empty transaction")]
    public async Task Empty_commit()
    {
        await using var conn = await OpenConnectionAsync();
        await conn.BeginTransaction().CommitAsync();
    }

    [Test, Description("Rolls back an empty transaction")]
    public async Task Empty_rollback()
    {
        await using var conn = await OpenConnectionAsync();
        await conn.BeginTransaction().RollbackAsync();
    }

    [Test, Description("Disposes an empty transaction")]
    public async Task Empty_Dispose()
    {
        using var _ = CreateTempPool(ConnectionString, out var connString);

        using (var conn = await OpenConnectionAsync(connString))
        using (conn.BeginTransaction())
        { }

        using (var conn = await OpenConnectionAsync(connString))
        {
            // Make sure the pending BEGIN TRANSACTION didn't leak from the previous open
            Assert.That(async () => await conn.ExecuteNonQueryAsync("SAVEPOINT foo"),
                Throws.Exception.TypeOf<PostgresException>()
                    .With.Property(nameof(PostgresException.SqlState)).EqualTo(PostgresErrorCodes.NoActiveSqlTransaction));
        }
    }

    [Test, Description("Tests that the isolation levels are properly supported")]
    [TestCase(IsolationLevel.ReadCommitted,   "read committed")]
    [TestCase(IsolationLevel.ReadUncommitted, "read uncommitted")]
    [TestCase(IsolationLevel.RepeatableRead,  "repeatable read")]
    [TestCase(IsolationLevel.Serializable,    "serializable")]
    [TestCase(IsolationLevel.Snapshot,        "repeatable read")]
    [TestCase(IsolationLevel.Unspecified,     "read committed")]
    public async Task Isolation_levels(IsolationLevel level, string expectedName)
    {
        await using var conn = await OpenConnectionAsync();
        var tx = conn.BeginTransaction(level);
        Assert.That(conn.ExecuteScalar("SHOW TRANSACTION ISOLATION LEVEL"), Is.EqualTo(expectedName));
        await tx.CommitAsync();
    }

    [Test]
    public async Task IsolationLevel_Chaos_is_unsupported()
    {
        await using var conn = await OpenConnectionAsync();
        Assert.That(() => conn.BeginTransaction(IsolationLevel.Chaos), Throws.Exception.TypeOf<NotSupportedException>());
    }

    [Test, Description("Rollback of an already rolled back transaction")]
    public async Task Rollback_twice()
    {
        await using var conn = await OpenConnectionAsync();
        var transaction = conn.BeginTransaction();
        transaction.Rollback();
        Assert.That(() => transaction.Rollback(), Throws.Exception.TypeOf<InvalidOperationException>());
    }

    [Test, Description("Makes sure the creating a transaction via DbConnection sets the proper isolation level")]
    [IssueLink("https://github.com/npgsql/npgsql/issues/559")]
    public async Task Default_IsolationLevel()
    {
        await using var conn = await OpenConnectionAsync();
        var tx = conn.BeginTransaction();
        Assert.That(tx.IsolationLevel, Is.EqualTo(IsolationLevel.ReadCommitted));
        tx.Rollback();

        tx = conn.BeginTransaction(IsolationLevel.Unspecified);
        Assert.That(tx.IsolationLevel, Is.EqualTo(IsolationLevel.ReadCommitted));
        tx.Rollback();
    }

    [Test, Description("Makes sure that transactions started in SQL work, except in multiplexing")]
    public async Task Via_sql()
    {
        if (IsMultiplexing)
            Assert.Ignore("Multiplexing: not implemented");

        await using var conn = await OpenConnectionAsync();
        var table = await CreateTempTable(conn, "name TEXT");

        if (IsMultiplexing)
        {
            Assert.That(async () => await conn.ExecuteNonQueryAsync("BEGIN"), Throws.Exception.TypeOf<NotSupportedException>());
            return;
        }

        await conn.ExecuteNonQueryAsync("BEGIN");
        await conn.ExecuteNonQueryAsync($"INSERT INTO {table} (name) VALUES ('X')");
        await conn.ExecuteNonQueryAsync("ROLLBACK");
        Assert.That(conn.ExecuteScalar($"SELECT COUNT(*) FROM {table}"), Is.EqualTo(0));
    }

    [Test]
    public async Task Nested()
    {
        await using var conn = await OpenConnectionAsync();
        conn.BeginTransaction();
        Assert.That(() => conn.BeginTransaction(), Throws.TypeOf<InvalidOperationException>());
    }

    [Test]
    public void Begin_transaction_on_closed_connection_throws()
    {
        using var conn = new NpgsqlConnection();
        Assert.That(() => conn.BeginTransaction(), Throws.Exception.TypeOf<InvalidOperationException>());
    }

    [Test]
    public async Task Rollback_failed_transaction_with_timeout()
    {
        await using var conn = await OpenConnectionAsync();

        var tx = conn.BeginTransaction();
        using var cmd = new NpgsqlCommand("BAD QUERY", conn, tx);
        Assert.That(cmd.CommandTimeout != 1);
        cmd.CommandTimeout = 1;
        try
        {
            cmd.ExecuteScalar();
            Assert.Fail();
        }
        catch (PostgresException)
        {
            // Timeout at the backend is now 1
            await tx.RollbackAsync();
            Assert.That(await conn.ExecuteScalarAsync("SELECT 1"), Is.EqualTo(1));
        }
    }

    [Test, Description("If a custom command timeout is set, a failed transaction could not be rollbacked to a previous savepoint")]
    [IssueLink("https://github.com/npgsql/npgsql/issues/363")]
    [IssueLink("https://github.com/npgsql/npgsql/issues/184")]
    public async Task Failed_transaction_cannot_rollback_to_savepoint_with_custom_timeout()
    {
        await using var conn = await OpenConnectionAsync();

        var transaction = conn.BeginTransaction();
        transaction.Save("TestSavePoint");

        using var cmd = new NpgsqlCommand("SELECT unknown_thing", conn);
        cmd.CommandTimeout = 1;
        try
        {
            cmd.ExecuteScalar();
        }
        catch (PostgresException)
        {
            transaction.Rollback("TestSavePoint");
            Assert.That(conn.ExecuteScalar("SELECT 1"), Is.EqualTo(1));
        }
    }

    [Test, Description("Closes a (pooled) connection with a failed transaction and a custom timeout")]
    [IssueLink("https://github.com/npgsql/npgsql/issues/719")]
    public async Task Failed_transaction_on_close_with_custom_timeout()
    {
        var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            Pooling = true
        }.ToString();

        await using var conn = await OpenConnectionAsync(connString);

        conn.BeginTransaction();
        var backendProcessId = conn.ProcessID;
        using (var badCmd = new NpgsqlCommand("SEL", conn))
        {
            badCmd.CommandTimeout = NpgsqlCommand.DefaultTimeout + 1;
            Assert.That(() => badCmd.ExecuteNonQuery(), Throws.Exception.TypeOf<PostgresException>());
        }
        // Connection now in failed transaction state, and a custom timeout is in place
        conn.Close();
        conn.Open();
        conn.BeginTransaction();
        Assert.That(conn.ProcessID, Is.EqualTo(backendProcessId));
        Assert.That(conn.ExecuteScalar("SELECT 1"), Is.EqualTo(1));
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/555")]
    public async Task Transaction_on_recycled_connection()
    {
        if (IsMultiplexing)
            Assert.Ignore("Multiplexing: fails");

        // Use application name to make sure we have our very own private connection pool
        await using var conn = new NpgsqlConnection(ConnectionString + $";Application Name={GetUniqueIdentifier(nameof(Transaction_on_recycled_connection))}");
        conn.Open();
        var prevConnectorId = conn.Connector!.Id;
        conn.Close();
        conn.Open();
        Assert.That(conn.Connector.Id, Is.EqualTo(prevConnectorId), "Connection pool returned a different connector, can't test");
        var tx = conn.BeginTransaction();
        conn.ExecuteScalar("SELECT 1");
        await tx.CommitAsync();
        NpgsqlConnection.ClearPool(conn);
    }

    [Test]
    public async Task Savepoint()
    {
        await using var conn = await OpenConnectionAsync();
        var table = await CreateTempTable(conn, "name TEXT");
        const string name = "theSavePoint";

        using (var tx = conn.BeginTransaction())
        {
            tx.Save(name);

            await conn.ExecuteNonQueryAsync($"INSERT INTO {table} (name) VALUES ('savepointtest')", tx: tx);
            Assert.That(conn.ExecuteScalar($"SELECT COUNT(*) FROM {table}", tx: tx), Is.EqualTo(1));
            tx.Rollback(name);
            Assert.That(conn.ExecuteScalar($"SELECT COUNT(*) FROM {table}", tx: tx), Is.EqualTo(0));
            await conn.ExecuteNonQueryAsync($"INSERT INTO {table} (name) VALUES ('savepointtest')", tx: tx);
            tx.Release(name);
            Assert.That(conn.ExecuteScalar($"SELECT COUNT(*) FROM {table}", tx: tx), Is.EqualTo(1));

            await tx.CommitAsync();
        }
        Assert.That(await conn.ExecuteScalarAsync($"SELECT COUNT(*) FROM {table}"), Is.EqualTo(1));
    }

    [Test]
    public async Task Savepoint_async()
    {
        await using var conn = await OpenConnectionAsync();
        var table = await CreateTempTable(conn, "name TEXT");
        const string name = "theSavePoint";

        using (var tx = conn.BeginTransaction())
        {
            await tx.SaveAsync(name);

            await conn.ExecuteNonQueryAsync($"INSERT INTO {table} (name) VALUES ('savepointtest')", tx: tx);
            Assert.That(conn.ExecuteScalar($"SELECT COUNT(*) FROM {table}", tx: tx), Is.EqualTo(1));
            await tx.RollbackAsync(name);
            Assert.That(conn.ExecuteScalar($"SELECT COUNT(*) FROM {table}", tx: tx), Is.EqualTo(0));
            await conn.ExecuteNonQueryAsync($"INSERT INTO {table} (name) VALUES ('savepointtest')", tx: tx);
            await tx.ReleaseAsync(name);
            Assert.That(conn.ExecuteScalar($"SELECT COUNT(*) FROM {table}", tx: tx), Is.EqualTo(1));

            await tx.CommitAsync();
        }
        Assert.That(await conn.ExecuteScalarAsync($"SELECT COUNT(*) FROM {table}"), Is.EqualTo(1));
    }

    [Test]
    public async Task Savepoint_quoted()
    {
        await using var conn = await OpenConnectionAsync();
        await using var tx = conn.BeginTransaction();
        tx.Save("a;b");
        tx.Rollback("a;b");
    }

    [Test(Description = "Makes sure that creating a savepoint doesn't perform an additional roundtrip, but prepends to the next command")]
    public async Task Savepoint_prepends()
    {
        await using var postmasterMock = PgPostmasterMock.Start(ConnectionString);
        using var _ = CreateTempPool(postmasterMock.ConnectionString, out var connectionString);
        await using var conn = await OpenConnectionAsync(connectionString);
        var pgMock = await postmasterMock.WaitForServerConnection();

        using var tx = conn.BeginTransaction();
        var saveTask = tx.SaveAsync("foo");
        Assert.That(saveTask.Status, Is.EqualTo(TaskStatus.RanToCompletion));

        // If we're here, SaveAsync above didn't wait for any response, which is the right behavior

        await pgMock
            .WriteCommandComplete()
            .WriteReadyForQuery() // BEGIN response
            .WriteCommandComplete()
            .WriteReadyForQuery() // SAVEPOINT response
            .WriteScalarResponseAndFlush(1);

        await conn.ExecuteScalarAsync("SELECT 1");

        await pgMock.ExpectSimpleQuery("BEGIN TRANSACTION ISOLATION LEVEL READ COMMITTED");
        await pgMock.ExpectSimpleQuery("SAVEPOINT foo");
        await pgMock.ExpectExtendedQuery();
    }

    [Test, Description("Check IsCompleted before, during and after a normal committed transaction")]
    [IssueLink("https://github.com/npgsql/npgsql/issues/985")]
    public async Task IsCompleted_commit()
    {
        await using var conn = await OpenConnectionAsync();
        var table = await CreateTempTable(conn, "name TEXT");
        var tx = conn.BeginTransaction();
        Assert.That(!tx.IsCompleted);
        await conn.ExecuteNonQueryAsync($"INSERT INTO {table} (name) VALUES ('X')", tx: tx);
        Assert.That(!tx.IsCompleted);
        await tx.CommitAsync();
        Assert.That(tx.IsCompleted);
    }

    [Test, Description("Check IsCompleted before, during, and after a successful but rolled back transaction")]
    [IssueLink("https://github.com/npgsql/npgsql/issues/985")]
    public async Task IsCompleted_rollback()
    {
        await using var conn = await OpenConnectionAsync();
        var table = await CreateTempTable(conn, "name TEXT");
        var tx = conn.BeginTransaction();
        Assert.That(!tx.IsCompleted);
        await conn.ExecuteNonQueryAsync($"INSERT INTO {table} (name) VALUES ('X')", tx: tx);
        Assert.That(!tx.IsCompleted);
        tx.Rollback();
        Assert.That(tx.IsCompleted);
    }

    [Test, Description("Check IsCompleted before, during, and after a failed then rolled back transaction")]
    [IssueLink("https://github.com/npgsql/npgsql/issues/985")]
    public async Task IsCompleted_rollback_failed()
    {
        await using var conn = await OpenConnectionAsync();
        var table = await CreateTempTable(conn, "name TEXT");
        var tx = conn.BeginTransaction();
        Assert.That(!tx.IsCompleted);
        await conn.ExecuteNonQueryAsync($"INSERT INTO {table} (name) VALUES ('X')", tx: tx);
        Assert.That(!tx.IsCompleted);
        Assert.That(async () => await conn.ExecuteNonQueryAsync("BAD QUERY"), Throws.Exception.TypeOf<PostgresException>());
        Assert.That(!tx.IsCompleted);
        tx.Rollback();
        Assert.That(tx.IsCompleted);
        Assert.That(await conn.ExecuteScalarAsync($"SELECT COUNT(*) FROM {table}"), Is.EqualTo(0));
    }

    [Test, Description("Tests that a if a DatabaseInfoFactory is registered for a database that doesn't support transactions, no transactions are created")]
    [Parallelizable(ParallelScope.None)]
    public async Task Transaction_not_supported()
    {
        if (IsMultiplexing)
            Assert.Ignore("Need to rethink/redo dummy transaction mode");

        var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            ApplicationName = nameof(Transaction_not_supported) + IsMultiplexing
        }.ToString();

        NpgsqlDatabaseInfo.RegisterFactory(new NoTransactionDatabaseInfoFactory());
        try
        {
            using var conn = await OpenConnectionAsync(connString);
            using var tx = conn.BeginTransaction();

            // Detect that we're not really in a transaction
            var prevTxId = conn.ExecuteScalar("SELECT txid_current()");
            var nextTxId = conn.ExecuteScalar("SELECT txid_current()");
            // If we're in an actual transaction, the two IDs should be the same
            // https://stackoverflow.com/questions/1651219/how-to-check-for-pending-operations-in-a-postgresql-transaction
            Assert.That(nextTxId, Is.Not.EqualTo(prevTxId));
            conn.Close();
        }
        finally
        {
            NpgsqlDatabaseInfo.ResetFactories();
        }

        using (var conn = await OpenConnectionAsync(connString))
        {
            NpgsqlConnection.ClearPool(conn);
            conn.ReloadTypes();
        }

        // Check that everything is back to normal
        using (var conn = await OpenConnectionAsync(connString))
        using (var tx = conn.BeginTransaction())
        {
            var prevTxId = conn.ExecuteScalar("SELECT txid_current()");
            var nextTxId = conn.ExecuteScalar("SELECT txid_current()");
            Assert.That(nextTxId, Is.EqualTo(prevTxId));
        }
    }

    [Test]
    [IssueLink("https://github.com/npgsql/npgsql/issues/3248")]
    // More at #3254
    public async Task Bug3248_Dispose_transaction_Rollback()
    {
        if (!IsMultiplexing)
            return;

        using var conn = await OpenConnectionAsync();
        await using (var tx = conn.BeginTransaction())
        {
            Assert.That(conn.Connector, Is.Not.Null);
            Assert.That(async () => await conn.ExecuteScalarAsync("SELECT * FROM \"unknown_table\"", tx: tx),
                Throws.Exception.TypeOf<PostgresException>());
            Assert.That(conn.Connector, Is.Not.Null);
        }

        Assert.That(conn.Connector, Is.Null);
    }

    [Test]
    [IssueLink("https://github.com/npgsql/npgsql/issues/3248")]
    // More at #3254
    public async Task Bug3248_Dispose_connection_Rollback()
    {
        if (!IsMultiplexing)
            return;

        var conn = await OpenConnectionAsync();
        var tx = conn.BeginTransaction();
        Assert.That(conn.Connector, Is.Not.Null);
        Assert.That(async () => await conn.ExecuteScalarAsync("SELECT * FROM \"unknown_table\"", tx: tx),
            Throws.Exception.TypeOf<PostgresException>());
        Assert.That(conn.Connector, Is.Not.Null);

        await conn.DisposeAsync();
        Assert.That(conn.Connector, Is.Null);
    }

    [Test]
    [IssueLink("https://github.com/npgsql/npgsql/issues/3306")]
    [TestCase(true)]
    [TestCase(false)]
    public async Task Bug3306(bool inTransactionBlock)
    {
        var conn = await OpenConnectionAsync();
        var tx = await conn.BeginTransactionAsync();
        await conn.ExecuteNonQueryAsync("SELECT 1", tx);
        if (!inTransactionBlock)
            await tx.RollbackAsync();
        await conn.CloseAsync();

        conn = await OpenConnectionAsync();
        var tx2 = await conn.BeginTransactionAsync();

        await tx.DisposeAsync();

        Assert.That(tx.IsDisposed, Is.True);
        Assert.That(tx2.IsDisposed, Is.False);

        await conn.DisposeAsync();
    }

    [Test, IssueLink("https://github.com/npgsql/efcore.pg/issues/1593")]
    public async Task Access_connection_on_completed_transaction()
    {
        using var conn = await OpenConnectionAsync();
        using var tx = await conn.BeginTransactionAsync();
        tx.Commit();
        Assert.That(tx.Connection, Is.SameAs(conn));
    }

    [Test]
    public async Task Unbound_transaction_reuse()
    {
        var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            MinPoolSize = 1,
            MaxPoolSize = 1,
        };
        using var __ = CreateTempPool(csb.ToString(), out var connectionString);

        await using var conn = await OpenConnectionAsync();
        var table = await CreateTempTable(conn, "name TEXT");

        await using var conn1 = await OpenConnectionAsync(connectionString);
        var tx1 = conn1.BeginTransaction();
        await using (var ___ = tx1)
        {
            using var cmd1 = conn1.CreateCommand();
            cmd1.CommandText = $"INSERT INTO {table} (name) VALUES ('X'); SELECT 1";
            await using (var reader1 = await cmd1.ExecuteReaderAsync())
            {
                Assert.That(async () => await reader1.ReadAsync(), Is.EqualTo(true));
                Assert.That(() => reader1.GetInt32(0), Is.EqualTo(1));
                Assert.That(reader1.RecordsAffected, Is.EqualTo(1));
            }
            await tx1.CommitAsync();
            Assert.That(await conn1.ExecuteScalarAsync($"SELECT COUNT(*) FROM {table}"), Is.EqualTo(1));
            await conn1.CloseAsync();
        }

        await using var conn2 = await OpenConnectionAsync(connectionString);
        var tx2 = conn2.BeginTransaction();
        await using (var ___ = tx2)
        {
            Assert.That(tx2, Is.Not.SameAs(tx1));
            using var cmd2 = conn2.CreateCommand();
            cmd2.CommandText = $"INSERT INTO {table} (name) VALUES ('Y'); SELECT 2";
            await using (var reader2 = await cmd2.ExecuteReaderAsync())
            {
                Assert.That(async () => await reader2.ReadAsync(), Is.EqualTo(true));
                Assert.That(() => reader2.GetInt32(0), Is.EqualTo(2));
                Assert.That(reader2.RecordsAffected, Is.EqualTo(1));
            }
            await tx2.CommitAsync();
            Assert.That(await conn2.ExecuteScalarAsync($"SELECT COUNT(*) FROM {table}"), Is.EqualTo(2));
            await conn2.CloseAsync();
        }

        await using var conn3 = await OpenConnectionAsync(connectionString);
        var tx3 = conn3.BeginTransaction();
        await using (var ___ = tx3)
        {
            Assert.That(tx3, Is.SameAs(tx1));
            using var cmd3 = conn3.CreateCommand();
            cmd3.CommandText = $"INSERT INTO {table} (name) VALUES ('Z'); SELECT 3";
            await using (var reader3 = await cmd3.ExecuteReaderAsync())
            {
                Assert.That(async () => await reader3.ReadAsync(), Is.EqualTo(true));
                Assert.That(() => reader3.GetInt32(0), Is.EqualTo(3));
                Assert.That(reader3.RecordsAffected, Is.EqualTo(1));
            }
            await tx3.CommitAsync();
            Assert.That(await conn3.ExecuteScalarAsync($"SELECT COUNT(*) FROM {table}"), Is.EqualTo(3));
            await conn3.CloseAsync();
        }
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/3686")]
    public async Task Bug3686()
    {
        if (IsMultiplexing)
            return;

        var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            Pooling = false
        };

        await using var conn = await OpenConnectionAsync(csb);
        await using var tx = await conn.BeginTransactionAsync();
        await conn.ExecuteNonQueryAsync("SELECT 1", tx);
        await tx.CommitAsync();
        await conn.CloseAsync();
        Assert.DoesNotThrow(() =>
        {
            _ = tx.Connection;
        });
    }

    class NoTransactionDatabaseInfoFactory : INpgsqlDatabaseInfoFactory
    {
        public async Task<NpgsqlDatabaseInfo?> Load(NpgsqlConnector conn, NpgsqlTimeout timeout, bool async)
        {
            var db = new NoTransactionDatabaseInfo(conn);
            await db.LoadPostgresInfo(conn, timeout, async);
            return db;
        }
    }

    class NoTransactionDatabaseInfo : PostgresDatabaseInfo
    {
        public override bool SupportsTransactions => false;

        internal NoTransactionDatabaseInfo(NpgsqlConnector conn) : base(conn) {}
    }

    // Older tests

    [Test]
    public void Bug184_Rollback_fails_on_aborted_transaction()
    {
        var csb = new NpgsqlConnectionStringBuilder(ConnectionString);
        csb.CommandTimeout = 100000;

        using var connTimeoutChanged = new NpgsqlConnection(csb.ToString());
        connTimeoutChanged.Open();
        using var t = connTimeoutChanged.BeginTransaction();
        try {
            var command = new NpgsqlCommand("select count(*) from dta", connTimeoutChanged, t);
            _ = command.ExecuteScalar();
        } catch (Exception) {
            t.Rollback();
        }
    }

    public TransactionTests(MultiplexingMode multiplexingMode) : base(multiplexingMode) {}
}
