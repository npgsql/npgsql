using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Npgsql.Util;
using NUnit.Framework;
using static Npgsql.Tests.TestUtil;

namespace Npgsql.Tests
{
    public class TransactionTests : MultiplexingTestBase
    {
        [Test]
        public async Task Commit()
        {
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "name TEXT", out var table);
                var tx = conn.BeginTransaction();
                await conn.ExecuteNonQueryAsync($"INSERT INTO {table} (name) VALUES ('X')", tx: tx);
                await tx.CommitAsync();
                Assert.That(await conn.ExecuteScalarAsync($"SELECT COUNT(*) FROM {table}"), Is.EqualTo(1));
                Assert.That(() => tx.Connection, Throws.Exception.TypeOf<InvalidOperationException>());
                await tx.DisposeAsync();
                Assert.That(() => tx.Connection, Throws.Exception.TypeOf<ObjectDisposedException>());
            }
        }

        [Test, Description("Basic insert within a rolled back transaction")]
        public async Task Rollback([Values(PrepareOrNot.NotPrepared, PrepareOrNot.Prepared)] PrepareOrNot prepare)
        {
            if (prepare == PrepareOrNot.Prepared && IsMultiplexing)
                return;

            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "name TEXT", out var table);
                var tx = conn.BeginTransaction();
                var cmd = new NpgsqlCommand($"INSERT INTO {table} (name) VALUES ('X')", conn, tx);
                if (prepare == PrepareOrNot.Prepared)
                    cmd.Prepare();
                cmd.ExecuteNonQuery();
                Assert.That(conn.ExecuteScalar($"SELECT COUNT(*) FROM {table}"), Is.EqualTo(1));
                tx.Rollback();
                Assert.That(tx.IsCompleted);
                Assert.That(await conn.ExecuteScalarAsync($"SELECT COUNT(*) FROM {table}"), Is.EqualTo(0));
                Assert.That(() => tx.Connection, Throws.Exception.TypeOf<InvalidOperationException>());
                await tx.DisposeAsync();
                Assert.That(() => tx.Connection, Throws.Exception.TypeOf<ObjectDisposedException>());
            }
        }

        [Test, Description("Basic insert within a rolled back transaction")]
        public async Task RollbackAsync([Values(PrepareOrNot.NotPrepared, PrepareOrNot.Prepared)] PrepareOrNot prepare)
        {
            if (prepare == PrepareOrNot.Prepared && IsMultiplexing)
                return;

            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "name TEXT", out var table);
                var tx = conn.BeginTransaction();
                var cmd = new NpgsqlCommand($"INSERT INTO {table} (name) VALUES ('X')", conn, tx);
                if (prepare == PrepareOrNot.Prepared)
                    cmd.Prepare();
                cmd.ExecuteNonQuery();
                Assert.That(conn.ExecuteScalar($"SELECT COUNT(*) FROM {table}"), Is.EqualTo(1));
                await tx.RollbackAsync();
                Assert.That(tx.IsCompleted);
                Assert.That(await conn.ExecuteScalarAsync($"SELECT COUNT(*) FROM {table}"), Is.EqualTo(0));
                Assert.That(() => tx.Connection, Throws.Exception.TypeOf<InvalidOperationException>());
                await tx.DisposeAsync();
                Assert.That(() => tx.Connection, Throws.Exception.TypeOf<ObjectDisposedException>());
            }
        }

        [Test, Description("Dispose a transaction in progress, should roll back")]
        public async Task RollbackOnDispose()
        {
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "name TEXT", out var table);
                var tx = conn.BeginTransaction();
                await conn.ExecuteNonQueryAsync($"INSERT INTO {table} (name) VALUES ('X')", tx: tx);
                await tx.DisposeAsync();
                Assert.That(await conn.ExecuteScalarAsync($"SELECT COUNT(*) FROM {table}"), Is.EqualTo(0));
            }
        }

        [Test]
        public async Task RollbackOnClose()
        {
            using (var conn1 = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn1, "name TEXT", out var table);

                NpgsqlTransaction tx;
                using (var conn2 = await OpenConnectionAsync())
                {
                    tx = conn2.BeginTransaction();
                    await conn2.ExecuteNonQueryAsync($"INSERT INTO {table} (name) VALUES ('X')", tx);
                }
                Assert.That(await conn1.ExecuteScalarAsync($"SELECT COUNT(*) FROM {table}"), Is.EqualTo(0));
                Assert.That(() => tx.Connection, Throws.Exception.TypeOf<ObjectDisposedException>());
            }
        }

        [Test, Description("Intentionally generates an error, putting us in a failed transaction block. Rolls back.")]
        public async Task RollbackFailed()
        {
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "name TEXT", out var table);
                var tx = conn.BeginTransaction();
                await conn.ExecuteNonQueryAsync($"INSERT INTO {table} (name) VALUES ('X')", tx: tx);
                Assert.That(async () => await conn.ExecuteNonQueryAsync("BAD QUERY"), Throws.Exception.TypeOf<PostgresException>());
                tx.Rollback();
                Assert.That(tx.IsCompleted);
                Assert.That(await conn.ExecuteScalarAsync($"SELECT COUNT(*) FROM {table}"), Is.EqualTo(0));
            }
        }

        [Test, Description("Commits an empty transaction")]
        public async Task EmptyCommit()
        {
            using (var conn = await OpenConnectionAsync())
                await conn.BeginTransaction().CommitAsync();
        }

        [Test, Description("Rolls back an empty transaction")]
        public async Task EmptyRollback()
        {
            using (var conn = await OpenConnectionAsync())
                await conn.BeginTransaction().RollbackAsync();
        }

        [Test, Description("Disposes an empty transaction")]
        public async Task EmptyDisposeTransaction()
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
                        .With.Property(nameof(PostgresException.SqlState)).EqualTo("25P01"));
            }
        }

        [Test, Description("Tests that the isolation levels are properly supported")]
        [TestCase(IsolationLevel.ReadCommitted,   "read committed")]
        [TestCase(IsolationLevel.ReadUncommitted, "read uncommitted")]
        [TestCase(IsolationLevel.RepeatableRead,  "repeatable read")]
        [TestCase(IsolationLevel.Serializable,    "serializable")]
        [TestCase(IsolationLevel.Snapshot,        "repeatable read")]
        [TestCase(IsolationLevel.Unspecified,     "read committed")]
        public async Task IsolationLevels(IsolationLevel level, string expectedName)
        {
            using (var conn = await OpenConnectionAsync())
            {
                var tx = conn.BeginTransaction(level);
                Assert.That(conn.ExecuteScalar("SHOW TRANSACTION ISOLATION LEVEL"), Is.EqualTo(expectedName));
                await tx.CommitAsync();
            }
        }

        [Test]
        public async Task IsolationLevelChaosUnsupported()
        {
            using (var conn = await OpenConnectionAsync())
                Assert.That(() => conn.BeginTransaction(IsolationLevel.Chaos), Throws.Exception.TypeOf<NotSupportedException>());
        }

        [Test, Description("Rollback of an already rolled back transaction")]
        public async Task RollbackTwice()
        {
            using (var conn = await OpenConnectionAsync())
            {
                var transaction = conn.BeginTransaction();
                transaction.Rollback();
                Assert.That(() => transaction.Rollback(), Throws.Exception.TypeOf<InvalidOperationException>());
            }
        }

        [Test, Description("Makes sure the creating a transaction via DbConnection sets the proper isolation level")]
        [IssueLink("https://github.com/npgsql/npgsql/issues/559")]
        public async Task DbConnectionDefaultIsolation()
        {
            using (var conn = await OpenConnectionAsync())
            {
                var dbConn = (DbConnection)conn;
                var tx = conn.BeginTransaction();
                Assert.That(tx.IsolationLevel, Is.EqualTo(IsolationLevel.ReadCommitted));
                tx.Rollback();

                tx = conn.BeginTransaction(IsolationLevel.Unspecified);
                Assert.That(tx.IsolationLevel, Is.EqualTo(IsolationLevel.ReadCommitted));
                tx.Rollback();
            }
        }

        [Test, Description("Makes sure that transactions started in SQL work, except in multiplexing")]
        public async Task ViaSql()
        {
            if (IsMultiplexing)
                Assert.Ignore("Multiplexing: not implemented");

            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "name TEXT", out var table);
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
        }

        [Test]
        public async Task Nested()
        {
            using (var conn = await OpenConnectionAsync())
            {
                conn.BeginTransaction();
                Assert.That(() => conn.BeginTransaction(), Throws.TypeOf<InvalidOperationException>());
            }
        }

        [Test]
        public void BeginTransactionBeforeOpen()
        {
            using (var conn = new NpgsqlConnection())
                Assert.That(() => conn.BeginTransaction(), Throws.Exception.TypeOf<InvalidOperationException>());
        }

        [Test]
        public async Task RollbackFailedTransactionWithTimeout()
        {
            using (var conn = await OpenConnectionAsync())
            {
                var tx = conn.BeginTransaction();
                using (var cmd = new NpgsqlCommand("BAD QUERY", conn, tx))
                {
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
            }
        }

        [Test, Description("If a custom command timeout is set, a failed transaction could not be rollbacked to a previous savepoint")]
        [IssueLink("https://github.com/npgsql/npgsql/issues/363")]
        [IssueLink("https://github.com/npgsql/npgsql/issues/184")]
        public async Task FailedTransactionCantRollbackToSavepointWithCustomTimeout()
        {
            using (var conn = await OpenConnectionAsync())
            {
                var transaction = conn.BeginTransaction();
                transaction.Save("TestSavePoint");

                using (var cmd = new NpgsqlCommand("SELECT unknown_thing", conn))
                {
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
            }
        }

        [Test, Description("Closes a (pooled) connection with a failed transaction and a custom timeout")]
        [IssueLink("https://github.com/npgsql/npgsql/issues/719")]
        public async Task FailedTransactionOnCloseWithCustomTimeout()
        {
            var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                Pooling = true
            }.ToString();
            using (var conn = await OpenConnectionAsync(connString))
            {
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
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/555")]
        public async Task TransactionOnRecycledConnection()
        {
            if (IsMultiplexing)
                Assert.Ignore("Multiplexing: fails");

            // Use application name to make sure we have our very own private connection pool
            using (var conn = new NpgsqlConnection(ConnectionString + $";Application Name={TestUtil.GetUniqueIdentifier(nameof(TransactionOnRecycledConnection))}"))
            {
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
        }

        [Test]
        public async Task Savepoint()
        {
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "name TEXT", out var table);
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
        }

        [Test]
        public async Task SavepointAsync()
        {
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "name TEXT", out var table);
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
        }

        [Test]
        public async Task SavepointWithSemicolon()
        {
            using (var conn = await OpenConnectionAsync())
            using (var tx = conn.BeginTransaction())
                Assert.That(() => tx.Save("a;b"), Throws.Exception.TypeOf<ArgumentException>());
        }

        [Test, Description("Check IsCompleted before, during and after a normal committed transaction")]
        [IssueLink("https://github.com/npgsql/npgsql/issues/985")]
        public async Task IsCompletedCommit()
        {
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "name TEXT", out var table);
                var tx = conn.BeginTransaction();
                Assert.That(!tx.IsCompleted);
                await conn.ExecuteNonQueryAsync($"INSERT INTO {table} (name) VALUES ('X')", tx: tx);
                Assert.That(!tx.IsCompleted);
                await tx.CommitAsync();
                Assert.That(tx.IsCompleted);
            }
        }

        [Test, Description("Check IsCompleted before, during, and after a successful but rolled back transaction")]
        [IssueLink("https://github.com/npgsql/npgsql/issues/985")]
        public async Task IsCompletedRollback()
        {
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "name TEXT", out var table);
                var tx = conn.BeginTransaction();
                Assert.That(!tx.IsCompleted);
                await conn.ExecuteNonQueryAsync($"INSERT INTO {table} (name) VALUES ('X')", tx: tx);
                Assert.That(!tx.IsCompleted);
                tx.Rollback();
                Assert.That(tx.IsCompleted);
            }
        }

        [Test, Description("Check IsCompleted before, during, and after a failed then rolled back transaction")]
        [IssueLink("https://github.com/npgsql/npgsql/issues/985")]
        public async Task IsCompletedRollbackFailed()
        {
            using (var conn = await OpenConnectionAsync())
            {
                await using var _ = await CreateTempTable(conn, "name TEXT", out var table);
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
        }

        [Test, Description("Tests that a if a DatabaseInfoFactory is registered for a database that doesn't support transactions, no transactions are created")]
        [Parallelizable(ParallelScope.None)]
        public async Task TransactionNotSupported()
        {
            if (IsMultiplexing)
                Assert.Ignore("Need to rethink/redo dummy transaction mode");

            var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                ApplicationName = nameof(TransactionNotSupported) + IsMultiplexing
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

        class NoTransactionDatabaseInfoFactory : INpgsqlDatabaseInfoFactory
        {
            public async Task<NpgsqlDatabaseInfo?> Load(NpgsqlConnection conn, NpgsqlTimeout timeout, bool async)
            {
                var db = new NoTransactionDatabaseInfo(conn);
                await db.LoadPostgresInfo(conn, timeout, async);
                return db;
            }
        }

        class NoTransactionDatabaseInfo : PostgresDatabaseInfo
        {
            public override bool SupportsTransactions => false;

            internal NoTransactionDatabaseInfo(NpgsqlConnection conn) : base(conn) {}
        }

        // Older tests

        [Test]
        public void Bug184RollbackFailsOnAbortedTransaction()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString);
            csb.CommandTimeout = 100000;

            using (var connTimeoutChanged = new NpgsqlConnection(csb.ToString())) {
                connTimeoutChanged.Open();
                using (var t = connTimeoutChanged.BeginTransaction()) {
                    try {
                        var command = new NpgsqlCommand("select count(*) from dta", connTimeoutChanged);
                        command.Transaction = t;
                        var result = command.ExecuteScalar();


                    } catch (Exception) {

                        t.Rollback();
                    }
                }
            }
        }

        public TransactionTests(MultiplexingMode multiplexingMode) : base(multiplexingMode) {}
    }
}
