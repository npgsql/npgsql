using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using Npgsql;
using NUnit.Framework;

namespace Npgsql.Tests
{
    public class TransactionTests : TestBase
    {
        [Test, Description("Basic insert within a committed transaction")]
        public void Commit()
        {
            ExecuteNonQuery("CREATE TEMP TABLE data (name TEXT)");
            var tx = Conn.BeginTransaction();
            ExecuteNonQuery("INSERT INTO data (name) VALUES ('X')", tx: tx);
            tx.Commit();
            Assert.That(ExecuteScalar("SELECT COUNT(*) FROM data"), Is.EqualTo(1));
        }

        [Test, Description("Basic insert within a rolled back transaction")]
        public void Rollback([Values(PrepareOrNot.NotPrepared, PrepareOrNot.Prepared)] PrepareOrNot prepare)
        {
            ExecuteNonQuery("CREATE TEMP TABLE data (name TEXT)");
            var tx = Conn.BeginTransaction();
            var cmd = new NpgsqlCommand("INSERT INTO data (name) VALUES ('X')", Conn, tx);
            if (prepare == PrepareOrNot.Prepared) { cmd.Prepare(); }
            cmd.ExecuteNonQuery();
            Assert.That(ExecuteScalar("SELECT COUNT(*) FROM data"), Is.EqualTo(1));
            tx.Rollback();
            Assert.That(tx.Connection, Is.Null);
            Assert.That(ExecuteScalar("SELECT COUNT(*) FROM data"), Is.EqualTo(0));
        }

        [Test, Description("Dispose a transaction in progress, should roll back")]
        public void RollbackOnDispose()
        {
            ExecuteNonQuery("CREATE TEMP TABLE data (name TEXT)");
            var tx = Conn.BeginTransaction();
            ExecuteNonQuery("INSERT INTO data (name) VALUES ('X')", tx: tx);
            tx.Dispose();
            Assert.That(tx.Connection, Is.Null);
            Assert.That(ExecuteScalar("SELECT COUNT(*) FROM data"), Is.EqualTo(0));
        }

        [Test]
        public void RollbackOnClose()
        {
            ExecuteNonQuery("DROP TABLE IF EXISTS rollback_on_close");
            ExecuteNonQuery("CREATE TABLE rollback_on_close (name TEXT)");

            NpgsqlTransaction tx;
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
                tx = conn.BeginTransaction();
                ExecuteNonQuery("INSERT INTO rollback_on_close (name) VALUES ('X')", conn, tx);
            }
            Assert.That(tx.Connection, Is.Null);
            Assert.That(ExecuteScalar("SELECT COUNT(*) FROM rollback_on_close"), Is.EqualTo(0));
        }

        [Test, Description("Intentionally generates an error, putting us in a failed transaction block. Rolls back.")]
        public void RollbackFailed()
        {
            ExecuteNonQuery("CREATE TEMP TABLE data (name TEXT)");
            var tx = Conn.BeginTransaction();
            ExecuteNonQuery("INSERT INTO data (name) VALUES ('X')", tx: tx);
            Assert.That(() => ExecuteNonQuery("BAD QUERY"), Throws.Exception);
            tx.Rollback();
            Assert.That(tx.Connection, Is.Null);
            Assert.That(ExecuteScalar("SELECT COUNT(*) FROM data"), Is.EqualTo(0));
        }

        [Test, Description("Commits an empty transaction")]
        public void EmptyCommit()
        {
            Conn.BeginTransaction().Commit();
        }

        [Test, Description("Rolls back an empty transaction")]
        public void EmptyRollback()
        {
            Conn.BeginTransaction().Rollback();
        }

        [Test, Description("Tests that the isolation levels are properly supported")]
        public void IsolationLevels()
        {
            foreach (var level in new[] {
               IsolationLevel.Unspecified,
               IsolationLevel.ReadCommitted,
               IsolationLevel.ReadUncommitted,
               IsolationLevel.RepeatableRead,
               IsolationLevel.Serializable,
               IsolationLevel.Snapshot,
            }) {
                var tx = Conn.BeginTransaction(level);
                tx.Commit();
            }

            foreach (var level in new[] {
               IsolationLevel.Chaos,
            }) {
                var level2 = level;
                Assert.That(() => Conn.BeginTransaction(level2), Throws.Exception.TypeOf<NotSupportedException>());
            }
        }

        [Test, Description("Rollback of an already rolled back transaction")]
        public void RollbackTwice()
        {
            var transaction = Conn.BeginTransaction();
            transaction.Rollback();
            Assert.That(() => transaction.Rollback(), Throws.Exception.TypeOf<InvalidOperationException>());
        }

        [Test, Description("Makes sure the creating a transaction via DbConnection sets the proper isolation level")]
        [IssueLink("https://github.com/npgsql/npgsql/issues/559")]
        public void DbConnectionDefaultIsolation()
        {
            var dbConn = (DbConnection)Conn;
            var tx = dbConn.BeginTransaction();
            Assert.That(tx.IsolationLevel, Is.EqualTo(IsolationLevel.ReadCommitted));
            tx.Rollback();

            tx = dbConn.BeginTransaction(IsolationLevel.Unspecified);
            Assert.That(tx.IsolationLevel, Is.EqualTo(IsolationLevel.ReadCommitted));
            tx.Rollback();
        }

        [Test, Description("Makes sure that transactions started in SQL work")]
        public void ViaSql()
        {
            ExecuteNonQuery("CREATE TEMP TABLE data (name TEXT)");
            ExecuteNonQuery("BEGIN");
            ExecuteNonQuery("INSERT INTO data (name) VALUES ('X')");
            ExecuteNonQuery("ROLLBACK");
            Assert.That(ExecuteScalar("SELECT COUNT(*) FROM data"), Is.EqualTo(0));
        }

        [Test]
        public void RollbackFailedTransactionWithTimeout()
        {
            var tx = Conn.BeginTransaction();
            using (var cmd = new NpgsqlCommand("BAD QUERY", Conn, tx))
            {
                Assert.That(cmd.CommandTimeout != 1);
                cmd.CommandTimeout = 1;
                try
                {
                    cmd.ExecuteScalar();
                    Assert.Fail();
                }
                catch (NpgsqlException)
                {
                    // Timeout at the backend is now 1
                    tx.Rollback();
                    Assert.That(ExecuteScalar("SELECT 1"), Is.EqualTo(1));
                }
            }
        }

        [Test, Description("If a custom command timeout is set, a failed transaction could not be rollbacked to a previous savepoint")]
        [IssueLink("https://github.com/npgsql/npgsql/issues/363")]
        [IssueLink("https://github.com/npgsql/npgsql/issues/184")]
        public void FailedTransactionCantRollbackToSavepointWithCustomTimeout()
        {
            var transaction = Conn.BeginTransaction();
            transaction.CreateSavepoint("TestSavePoint");

            using (var command = new NpgsqlCommand("SELECT unknown_thing", Conn)) {
                command.CommandTimeout = 1;
                try {
                    command.ExecuteScalar();
                } catch (NpgsqlException) {
                    transaction.RollbackToSavepoint("TestSavePoint");
                    Assert.That(ExecuteScalar("SELECT 1"), Is.EqualTo(1));
                }
            }
        }

        [Test]
        [IssueLink("https://github.com/npgsql/npgsql/issues/555")]
        public void TransactionOnRecycledConnection()
        {
            var conn = new NpgsqlConnection(ConnectionString);
            conn.Open();
            var prevConnectorId = conn.Connector.Id;
            conn.Close();
            conn.Open();
            Assert.That(conn.Connector.Id, Is.EqualTo(prevConnectorId), "Connection pool returned a different connector, can't test");
            var tx = conn.BeginTransaction();
            ExecuteScalar("SELECT 1", conn);
            tx.Commit();
            conn.Close();
        }

        [Test]
        public void Savepoint()
        {
            ExecuteNonQuery("CREATE TEMP TABLE data (name TEXT)");
            const string name = "theSavePoint";

            using (var tx = Conn.BeginTransaction())
            {
                tx.CreateSavepoint(name);

                ExecuteNonQuery("INSERT INTO data (name) VALUES ('savepointtest')", tx: tx);
                Assert.That(ExecuteScalar("SELECT COUNT(*) FROM data", tx: tx), Is.EqualTo(1));
                tx.RollbackToSavepoint(name);
                Assert.That(ExecuteScalar("SELECT COUNT(*) FROM data", tx: tx), Is.EqualTo(0));
                ExecuteNonQuery("INSERT INTO data (name) VALUES ('savepointtest')", tx: tx);
                tx.ReleaseSavepoint(name);
                Assert.That(ExecuteScalar("SELECT COUNT(*) FROM data", tx: tx), Is.EqualTo(1));

                tx.Commit();
            }
            Assert.That(ExecuteScalar("SELECT COUNT(*) FROM data"), Is.EqualTo(1));
        }

        [Test]
        public void SavepointWithSemicolon()
        {
            using (var tx = Conn.BeginTransaction())
                Assert.That(() => tx.CreateSavepoint("a;b"), Throws.Exception.TypeOf<ArgumentException>());
        }

        // Older tests

        [Test]
        public void Bug184RollbackFailsOnAbortedTransaction()
        {
            NpgsqlConnectionStringBuilder csb = new NpgsqlConnectionStringBuilder(ConnectionString);
            csb.CommandTimeout = 100000;

            using (NpgsqlConnection connTimeoutChanged = new NpgsqlConnection(csb.ToString())) {
                connTimeoutChanged.Open();
                using (var t = connTimeoutChanged.BeginTransaction()) {
                    try {
                        var command = new NpgsqlCommand("select count(*) from dta", connTimeoutChanged);
                        command.Transaction = t;
                        Object result = command.ExecuteScalar();


                    } catch (Exception) {

                        t.Rollback();
                    }

                }
            }
        }

        public TransactionTests(string backendVersion) : base(backendVersion) {}
    }
}
