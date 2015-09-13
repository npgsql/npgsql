#region License
// The PostgreSQL License
//
// Copyright (C) 2015 The Npgsql Development Team
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
            transaction.Save("TestSavePoint");

            using (var command = new NpgsqlCommand("SELECT unknown_thing", Conn)) {
                command.CommandTimeout = 1;
                try {
                    command.ExecuteScalar();
                } catch (NpgsqlException) {
                    transaction.Rollback("TestSavePoint");
                    Assert.That(ExecuteScalar("SELECT 1"), Is.EqualTo(1));
                }
            }
        }

        [Test, Description("Closes a (pooled) connection with a failed transaction and a custom timeout")]
        [IssueLink("https://github.com/npgsql/npgsql/issues/719")]
        public void FailedTransactionOnCloseWithCustom()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString) { Pooling = true };
            using (var conn = new NpgsqlConnection(csb))
            {
                conn.Open();
                var backendProcessId = conn.ProcessID;
                conn.BeginTransaction();
                using (var badCmd = new NpgsqlCommand("SEL", conn))
                {
                    badCmd.CommandTimeout = NpgsqlCommand.DefaultTimeout + 1;
                    Assert.That(() => badCmd.ExecuteNonQuery(), Throws.Exception.TypeOf<NpgsqlException>());
                }
                // Connection now in failed transaction state, and a custom timeout is in place
                conn.Close();
                conn.Open();
                Assert.That(conn.ProcessID, Is.EqualTo(backendProcessId));
                Assert.That(ExecuteScalar("SELECT 1", conn), Is.EqualTo(1));
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
                tx.Save(name);

                ExecuteNonQuery("INSERT INTO data (name) VALUES ('savepointtest')", tx: tx);
                Assert.That(ExecuteScalar("SELECT COUNT(*) FROM data", tx: tx), Is.EqualTo(1));
                tx.Rollback(name);
                Assert.That(ExecuteScalar("SELECT COUNT(*) FROM data", tx: tx), Is.EqualTo(0));
                ExecuteNonQuery("INSERT INTO data (name) VALUES ('savepointtest')", tx: tx);
                tx.Release(name);
                Assert.That(ExecuteScalar("SELECT COUNT(*) FROM data", tx: tx), Is.EqualTo(1));

                tx.Commit();
            }
            Assert.That(ExecuteScalar("SELECT COUNT(*) FROM data"), Is.EqualTo(1));
        }

        [Test]
        public void SavepointWithSemicolon()
        {
            using (var tx = Conn.BeginTransaction())
                Assert.That(() => tx.Save("a;b"), Throws.Exception.TypeOf<ArgumentException>());
        }

        [Test]
        [IssueLink("https://github.com/npgsql/npgsql/issues/765")]
        public void PrependedRollbackWhileStartingNewTransaction()
        {
            var connString = new NpgsqlConnectionStringBuilder(ConnectionString) {
                CommandTimeout = 600,
                InternalCommandTimeout = 30
            };

            int backendId;
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                backendId = conn.Connector.BackendProcessId;
                conn.BeginTransaction();
                ExecuteNonQuery("SELECT 1", conn);
            }
            // Connector is back in the pool with a queued ROLLBACK
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                Assert.That(conn.Connector.BackendProcessId, Is.EqualTo(backendId));
                var tx = conn.BeginTransaction();
                // We've captured the transaction instance, a new begin transaction is now enqueued after the rollback
                ExecuteNonQuery("SELECT 1", conn);
                Assert.That(tx.Connection, Is.SameAs(conn));
                Assert.That(conn.Connector.Transaction, Is.SameAs(tx));
            }
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
