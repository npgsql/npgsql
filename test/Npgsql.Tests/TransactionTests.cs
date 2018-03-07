#region License
// The PostgreSQL License
//
// Copyright (C) 2018 The Npgsql Development Team
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
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using NUnit.Framework;

namespace Npgsql.Tests
{
    public class TransactionTests : TestBase
    {
        [Test, Description("Basic insert within a committed transaction")]
        public void Commit()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (name TEXT)");
                var tx = conn.BeginTransaction();
                conn.ExecuteNonQuery("INSERT INTO data (name) VALUES ('X')", tx: tx);
                tx.Commit();
                Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM data"), Is.EqualTo(1));
            }
        }

        [Test]
        public async Task CommitAsync()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (name TEXT)");
                var tx = conn.BeginTransaction();
                conn.ExecuteNonQuery("INSERT INTO data (name) VALUES ('X')", tx: tx);
                await tx.CommitAsync();
                Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM data"), Is.EqualTo(1));
            }
        }

        [Test, Description("Basic insert within a rolled back transaction")]
        public void Rollback([Values(PrepareOrNot.NotPrepared, PrepareOrNot.Prepared)] PrepareOrNot prepare)
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (name TEXT)");
                var tx = conn.BeginTransaction();
                var cmd = new NpgsqlCommand("INSERT INTO data (name) VALUES ('X')", conn, tx);
                if (prepare == PrepareOrNot.Prepared)
                    cmd.Prepare();
                cmd.ExecuteNonQuery();
                Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM data"), Is.EqualTo(1));
                tx.Rollback();
                Assert.That(tx.IsCompleted);
                Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM data"), Is.EqualTo(0));
            }
        }

        [Test, Description("Basic insert within a rolled back transaction")]
        public async Task RollbackAsync([Values(PrepareOrNot.NotPrepared, PrepareOrNot.Prepared)] PrepareOrNot prepare)
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (name TEXT)");
                var tx = conn.BeginTransaction();
                var cmd = new NpgsqlCommand("INSERT INTO data (name) VALUES ('X')", conn, tx);
                if (prepare == PrepareOrNot.Prepared)
                    cmd.Prepare();
                cmd.ExecuteNonQuery();
                Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM data"), Is.EqualTo(1));
                await tx.RollbackAsync();
                Assert.That(tx.IsCompleted);
                Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM data"), Is.EqualTo(0));
            }
        }

        [Test, Description("Dispose a transaction in progress, should roll back")]
        public void RollbackOnDispose()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (name TEXT)");
                var tx = conn.BeginTransaction();
                conn.ExecuteNonQuery("INSERT INTO data (name) VALUES ('X')", tx: tx);
                tx.Dispose();
                Assert.That(tx.IsCompleted);
                Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM data"), Is.EqualTo(0));
            }
        }

        [Test]
        public void RollbackOnClose()
        {
            var tableName = nameof(RollbackOnClose);
            using (var conn1 = OpenConnection())
            {
                conn1.ExecuteNonQuery($"DROP TABLE IF EXISTS {tableName}");
                conn1.ExecuteNonQuery($"CREATE TABLE {tableName} (name TEXT)");

                NpgsqlTransaction tx;
                using (var conn2 = OpenConnection())
                {
                    tx = conn2.BeginTransaction();
                    conn2.ExecuteNonQuery($"INSERT INTO {tableName} (name) VALUES ('X')", tx);
                }
                Assert.That(tx.IsCompleted);
                Assert.That(conn1.ExecuteScalar($"SELECT COUNT(*) FROM {tableName}"), Is.EqualTo(0));
                conn1.ExecuteNonQuery($"DROP TABLE {tableName}");
            }
        }

        [Test, Description("Intentionally generates an error, putting us in a failed transaction block. Rolls back.")]
        public void RollbackFailed()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (name TEXT)");
                var tx = conn.BeginTransaction();
                conn.ExecuteNonQuery("INSERT INTO data (name) VALUES ('X')", tx: tx);
                Assert.That(() => conn.ExecuteNonQuery("BAD QUERY"), Throws.Exception.TypeOf<PostgresException>());
                tx.Rollback();
                Assert.That(tx.IsCompleted);
                Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM data"), Is.EqualTo(0));
            }
        }

        [Test, Description("Commits an empty transaction")]
        public void EmptyCommit()
        {
            using (var conn = OpenConnection())
                conn.BeginTransaction().Commit();
        }

        [Test, Description("Rolls back an empty transaction")]
        public void EmptyRollback()
        {
            using (var conn = OpenConnection())
                conn.BeginTransaction().Rollback();
        }

        [Test, Description("Tests that the isolation levels are properly supported")]
        public void IsolationLevels()
        {
            using (var conn = OpenConnection())
            {
                foreach (var level in new[]
                {
                    IsolationLevel.Unspecified,
                    IsolationLevel.ReadCommitted,
                    IsolationLevel.ReadUncommitted,
                    IsolationLevel.RepeatableRead,
                    IsolationLevel.Serializable,
                    IsolationLevel.Snapshot,
                })
                {
                    var tx = conn.BeginTransaction(level);
                    tx.Commit();
                }

                foreach (var level in new[]
                {
                    IsolationLevel.Chaos,
                })
                {
                    var level2 = level;
                    Assert.That(() => conn.BeginTransaction(level2), Throws.Exception.TypeOf<NotSupportedException>());
                }
            }
        }

        [Test, Description("Rollback of an already rolled back transaction")]
        public void RollbackTwice()
        {
            using (var conn = OpenConnection())
            {
                var transaction = conn.BeginTransaction();
                transaction.Rollback();
                Assert.That(() => transaction.Rollback(), Throws.Exception.TypeOf<InvalidOperationException>());
            }
        }

        [Test, Description("Makes sure the creating a transaction via DbConnection sets the proper isolation level")]
        [IssueLink("https://github.com/npgsql/npgsql/issues/559")]
        public void DbConnectionDefaultIsolation()
        {
            using (var conn = OpenConnection())
            {
                var dbConn = (DbConnection)conn;
                var tx = dbConn.BeginTransaction();
                Assert.That(tx.IsolationLevel, Is.EqualTo(IsolationLevel.ReadCommitted));
                tx.Rollback();

                tx = dbConn.BeginTransaction(IsolationLevel.Unspecified);
                Assert.That(tx.IsolationLevel, Is.EqualTo(IsolationLevel.ReadCommitted));
                tx.Rollback();
            }
        }

        [Test, Description("Makes sure that transactions started in SQL work")]
        public void ViaSql()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (name TEXT)");
                conn.ExecuteNonQuery("BEGIN");
                conn.ExecuteNonQuery("INSERT INTO data (name) VALUES ('X')");
                conn.ExecuteNonQuery("ROLLBACK");
                Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM data"), Is.EqualTo(0));
            }
        }

        [Test]
        public void Nested()
        {
            using (var conn = OpenConnection())
            {
                conn.BeginTransaction();
                Assert.That(() => conn.BeginTransaction(), Throws.TypeOf<NotSupportedException>());
            }
        }

        [Test]
        public void BeginTransactionBeforeOpen()
        {
            using (var conn = new NpgsqlConnection())
                Assert.That(() => conn.BeginTransaction(), Throws.Exception.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void RollbackFailedTransactionWithTimeout()
        {
            using (var conn = OpenConnection())
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
                        tx.Rollback();
                        Assert.That(conn.ExecuteScalar("SELECT 1"), Is.EqualTo(1));
                    }
                }
            }
        }

        [Test, Description("If a custom command timeout is set, a failed transaction could not be rollbacked to a previous savepoint")]
        [IssueLink("https://github.com/npgsql/npgsql/issues/363")]
        [IssueLink("https://github.com/npgsql/npgsql/issues/184")]
        public void FailedTransactionCantRollbackToSavepointWithCustomTimeout()
        {
            using (var conn = OpenConnection())
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
        public void FailedTransactionOnCloseWithCustom()
        {
            var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                Pooling = true
            }.ToString();
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                var backendProcessId = conn.ProcessID;
                conn.BeginTransaction();
                using (var badCmd = new NpgsqlCommand("SEL", conn))
                {
                    badCmd.CommandTimeout = NpgsqlCommand.DefaultTimeout + 1;
                    Assert.That(() => badCmd.ExecuteNonQuery(), Throws.Exception.TypeOf<PostgresException>());
                }
                // Connection now in failed transaction state, and a custom timeout is in place
                conn.Close();
                conn.Open();
                Assert.That(conn.ProcessID, Is.EqualTo(backendProcessId));
                Assert.That(conn.ExecuteScalar("SELECT 1"), Is.EqualTo(1));
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/555")]
        public void TransactionOnRecycledConnection()
        {
            // Use application name to make sure we have our very own private connection pool
            using (var conn = new NpgsqlConnection(ConnectionString + $";Application Name={TestUtil.GetUniqueIdentifier(nameof(TransactionOnRecycledConnection))}"))
            {
                conn.Open();
                var prevConnectorId = conn.Connector.Id;
                conn.Close();
                conn.Open();
                Assert.That(conn.Connector.Id, Is.EqualTo(prevConnectorId), "Connection pool returned a different connector, can't test");
                var tx = conn.BeginTransaction();
                conn.ExecuteScalar("SELECT 1");
                tx.Commit();
                NpgsqlConnection.ClearPool(conn);
            }
        }

        [Test]
        public void Savepoint()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (name TEXT)");
                const string name = "theSavePoint";

                using (var tx = conn.BeginTransaction())
                {
                    tx.Save(name);

                    conn.ExecuteNonQuery("INSERT INTO data (name) VALUES ('savepointtest')", tx: tx);
                    Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM data", tx: tx), Is.EqualTo(1));
                    tx.Rollback(name);
                    Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM data", tx: tx), Is.EqualTo(0));
                    conn.ExecuteNonQuery("INSERT INTO data (name) VALUES ('savepointtest')", tx: tx);
                    tx.Release(name);
                    Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM data", tx: tx), Is.EqualTo(1));

                    tx.Commit();
                }
                Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM data"), Is.EqualTo(1));
            }
        }

        [Test]
        public void SavepointWithSemicolon()
        {
            using (var conn = OpenConnection())
            using (var tx = conn.BeginTransaction())
                Assert.That(() => tx.Save("a;b"), Throws.Exception.TypeOf<ArgumentException>());
        }

        [Test, Description("Check IsCompleted before, during and after a normal committed transaction")]
        [IssueLink("https://github.com/npgsql/npgsql/issues/985")]
        public void IsCompletedCommit()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (name TEXT)");
                var tx = conn.BeginTransaction();
                Assert.That(!tx.IsCompleted);
                conn.ExecuteNonQuery("INSERT INTO data (name) VALUES ('X')", tx: tx);
                Assert.That(!tx.IsCompleted);
                tx.Commit();
                Assert.That(tx.IsCompleted);
            }
        }

        [Test, Description("Check IsCompleted before, during, and after a successful but rolled back transaction")]
        [IssueLink("https://github.com/npgsql/npgsql/issues/985")]
        public void IsCompletedRollback()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (name TEXT)");
                var tx = conn.BeginTransaction();
                Assert.That(!tx.IsCompleted);
                conn.ExecuteNonQuery("INSERT INTO data (name) VALUES ('X')", tx: tx);
                Assert.That(!tx.IsCompleted);
                tx.Rollback();
                Assert.That(tx.IsCompleted);
            }
        }


        [Test, Description("Check IsCompleted before, during, and after a failed then rolled back transaction")]
        [IssueLink("https://github.com/npgsql/npgsql/issues/985")]
        public void IsCompletedRollbackFailed()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (name TEXT)");
                var tx = conn.BeginTransaction();
                Assert.That(!tx.IsCompleted);
                conn.ExecuteNonQuery("INSERT INTO data (name) VALUES ('X')", tx: tx);
                Assert.That(!tx.IsCompleted);
                Assert.That(() => conn.ExecuteNonQuery("BAD QUERY"), Throws.Exception.TypeOf<PostgresException>());
                Assert.That(!tx.IsCompleted);
                tx.Rollback();
                Assert.That(tx.IsCompleted);
                Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM data"), Is.EqualTo(0));
            }
        }

        [Test, Description("Tests that a if a DatabaseInfoFactory is registered for a database that doesn't support transactions, no transactions are created")]
        [Parallelizable(ParallelScope.None)]
        public void TransactionNotSupported()
        {
            var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                ApplicationName = nameof(TransactionNotSupported)
            }.ToString();

            NpgsqlDatabaseInfo.RegisterFactory(new NoTransactionDatabaseInfoFactory());
            using (var conn = OpenConnection(connString))
            using (var tx = conn.BeginTransaction())
            {
                // Detect that we're not really in a transaction
                var prevTxId = conn.ExecuteScalar("SELECT txid_current()");
                var nextTxId = conn.ExecuteScalar("SELECT txid_current()");
                // If we're in an actual transaction, the two IDs should be the same
                // https://stackoverflow.com/questions/1651219/how-to-check-for-pending-operations-in-a-postgresql-transaction
                Assert.That(nextTxId, Is.Not.EqualTo(prevTxId));
                conn.Close();
            }

            NpgsqlDatabaseInfo.ResetFactories();

            using (var conn = OpenConnection(connString))
            {
                NpgsqlConnection.ClearPool(conn);
                conn.ReloadTypes();
            }

            // Check that everything is back to normal
            using (var conn = OpenConnection(connString))
            using (var tx = conn.BeginTransaction())
            {
                var prevTxId = conn.ExecuteScalar("SELECT txid_current()");
                var nextTxId = conn.ExecuteScalar("SELECT txid_current()");
                Assert.That(nextTxId, Is.EqualTo(prevTxId));
            }
        }

        class NoTransactionDatabaseInfoFactory : INpgsqlDatabaseInfoFactory
        {
            public async Task<NpgsqlDatabaseInfo> Load(NpgsqlConnection conn, NpgsqlTimeout timeout, bool async)
            {
                var db = new NoTransactionDatabaseInfo();
                await db.LoadPostgresInfo(conn, timeout, async);
                return db;
            }
        }

        class NoTransactionDatabaseInfo : PostgresDatabaseInfo
        {
            public override bool SupportsTransactions => false;
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
    }
}
