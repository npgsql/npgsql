using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Npgsql;
using NUnit.Framework;

namespace NpgsqlTests
{
    public class TransactionTests : TestBase
    {
        [Test, Description("Basic insert within a committed transaction")]
        public void TransactionCommit()
        {
            var tx = Conn.BeginTransaction();
            ExecuteNonQuery("INSERT INTO data (field_text) VALUES ('X')", tx: tx);
            tx.Commit();
            Assert.That(ExecuteScalar("SELECT COUNT(*) FROM data"), Is.EqualTo(1));
        }

        [Test, Description("Basic insert within a rolled back transaction")]
        public void Rollback([Values(PrepareOrNot.NotPrepared, PrepareOrNot.Prepared)] PrepareOrNot prepare)
        {
            var tx = Conn.BeginTransaction();
            var cmd = new NpgsqlCommand("INSERT INTO data (field_text) VALUES ('X')", Conn, tx);
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
            var tx = Conn.BeginTransaction();
            ExecuteNonQuery("INSERT INTO data (field_text) VALUES ('X')", tx: tx);
            tx.Dispose();
            Assert.That(tx.Connection, Is.Null);
            Assert.That(ExecuteScalar("SELECT COUNT(*) FROM data"), Is.EqualTo(0));
        }

        [Test, Description("Intentionally generates an error, putting us in a failed transaction block. Rolls back.")]
        public void RollbackFailed()
        {
            var tx = Conn.BeginTransaction();
            ExecuteNonQuery("INSERT INTO data (field_text) VALUES ('X')", tx: tx);
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
               IsolationLevel.Unspecified,
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
                }
            }
        }

        // Older tests

        [Test]
        public void TestSavePoint()
        {
            const String theSavePoint = "theSavePoint";

            using (var transaction = Conn.BeginTransaction()) {
                transaction.Save(theSavePoint);

                ExecuteNonQuery("INSERT INTO data (field_text) VALUES ('savepointtest')");
                var result = ExecuteScalar("SELECT COUNT(*) FROM data WHERE field_text = 'savepointtest'");
                Assert.AreEqual(1, result);

                transaction.Rollback(theSavePoint);

                result = ExecuteScalar("SELECT COUNT(*) FROM data WHERE field_text = 'savepointtest'");
                Assert.AreEqual(0, result);
            }
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestSavePointWithSemicolon()
        {
            const String theSavePoint = "theSavePoint;";

            using (var transaction = Conn.BeginTransaction()) {
                transaction.Save(theSavePoint);

                ExecuteNonQuery("INSERT INTO data (field_text) VALUES ('savepointtest')");
                var result = ExecuteScalar("SELECT COUNT(*) FROM data WHERE field_text = 'savepointtest'");
                Assert.AreEqual(1, result);

                transaction.Rollback(theSavePoint);

                result = ExecuteScalar("SELECT COUNT(*) FROM data WHERE field_text = 'savepointtest'");
                Assert.AreEqual(0, result);
            }
        }

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
