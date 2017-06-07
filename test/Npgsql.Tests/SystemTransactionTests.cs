#region License
// The PostgreSQL License
//
// Copyright (C) 2017 The Npgsql Development Team
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

// TransactionScope exists in netstandard20, but distributed transactions do not
#if NET451

using System;
using System.Collections.Generic;
using System.Data;
using System.Transactions;
using JetBrains.Annotations;
using NUnit.Framework;

namespace Npgsql.Tests
{
    [Parallelizable(ParallelScope.None)]
    public class SystemTransactionTests : TestBase
    {
        [Test, Description("Single connection enlisting explicitly, committing")]
        public void ExplicitEnlist()
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
                using (var scope = new TransactionScope())
                {
                    conn.EnlistTransaction(Transaction.Current);
                    Assert.That(conn.ExecuteNonQuery(@"INSERT INTO data (name) VALUES ('test')"), Is.EqualTo(1));
                    AssertNoPreparedTransactions();
                    scope.Complete();
                }
                AssertNoPreparedTransactions();
                using (var tx = conn.BeginTransaction())
                {
                    Assert.That(conn.ExecuteScalar(@"SELECT COUNT(*) FROM data"), Is.EqualTo(1));
                    tx.Rollback();
                }
            }
        }

        [Test, Description("Single connection enlisting implicitly, committing")]
        public void ImplicitEnlist()
        {
            var conn = new NpgsqlConnection(ConnectionStringWithEnlist);
            using (var scope = new TransactionScope())
            {
                conn.Open();
                Assert.That(conn.ExecuteNonQuery(@"INSERT INTO data (name) VALUES ('test')"), Is.EqualTo(1));
                AssertNoPreparedTransactions();
                scope.Complete();
            }
            using (var tx = conn.BeginTransaction())
            {
                Assert.That(conn.ExecuteScalar(@"SELECT COUNT(*) FROM data"), Is.EqualTo(1));
                tx.Rollback();
            }
        }

        [Test, Description("Single connection rollback")]
        public void Rollback()
        {
            using (var conn = OpenConnection())
            {
                using (new TransactionScope())
                {
                    conn.EnlistTransaction(Transaction.Current);
                    Assert.That(conn.ExecuteNonQuery(@"INSERT INTO data (name) VALUES ('test')"), Is.EqualTo(1));
                    // No commit
                }
                AssertNoPreparedTransactions();
                using (var tx = conn.BeginTransaction())
                {
                    Assert.That(conn.ExecuteScalar(@"SELECT COUNT(*) FROM data"), Is.EqualTo(0));
                    tx.Rollback();
                }
            }
        }

        [Test]
        public void TwoConnections()
        {
            using (var conn1 = OpenConnection())
            using (var conn2 = OpenConnection())
            {
                using (var scope = new TransactionScope())
                {
                    conn1.EnlistTransaction(Transaction.Current);
                    conn2.EnlistTransaction(Transaction.Current);

                    Assert.That(conn1.ExecuteNonQuery(@"INSERT INTO data (name) VALUES ('test1')"), Is.EqualTo(1));
                    Assert.That(conn2.ExecuteNonQuery(@"INSERT INTO data (name) VALUES ('test2')"), Is.EqualTo(1));
                    scope.Complete();
                }
            }
            // TODO: There may be a race condition here, where the prepared transaction above still hasn't committed.
            AssertNoPreparedTransactions();
            AssertNumberOfRows(2);
        }

        [Test]
        public void TwoConnectionsRollback()
        {
            using (new TransactionScope())
            using (var conn1 = OpenConnection(ConnectionStringWithEnlist))
            using (var conn2 = OpenConnection(ConnectionStringWithEnlist))
            {
                Assert.That(conn1.ExecuteNonQuery(@"INSERT INTO data (name) VALUES ('test1')"), Is.EqualTo(1));
                Assert.That(conn2.ExecuteNonQuery(@"INSERT INTO data (name) VALUES ('test2')"), Is.EqualTo(1));
            }
            // TODO: There may be a race condition here, where the prepared transaction above still hasn't committed.
            AssertNoPreparedTransactions();
            AssertNumberOfRows(0);
        }

        [Test]
        public void TwoConnectionsWithFailure()
        {
            using (var conn1 = OpenConnection())
            using (var conn2 = OpenConnection())
            {
                var scope = new TransactionScope();
                conn1.EnlistTransaction(Transaction.Current);
                conn2.EnlistTransaction(Transaction.Current);

                Assert.That(conn1.ExecuteNonQuery(@"INSERT INTO data (name) VALUES ('test1')"), Is.EqualTo(1));
                Assert.That(conn2.ExecuteNonQuery(@"INSERT INTO data (name) VALUES ('test2')"), Is.EqualTo(1));
                conn1.ExecuteNonQuery($"SELECT pg_terminate_backend({conn2.ProcessID})");
                scope.Complete();
                Assert.That(() => scope.Dispose(), Throws.Exception.TypeOf<TransactionAbortedException>());
                AssertNoPreparedTransactions();
                using (var tx = conn1.BeginTransaction())
                {
                    Assert.That(conn1.ExecuteScalar(@"SELECT COUNT(*) FROM data"), Is.EqualTo(0));
                    tx.Rollback();
                }
            }
        }

        [Test]
        public void CloseConnection()
        {
            var connString = new NpgsqlConnectionStringBuilder(ConnectionStringWithEnlist)
            {
                ApplicationName = nameof(CloseConnection),
            }.ToString();
            using (var scope = new TransactionScope())
            using (var conn = OpenConnection(connString))
            {
                Assert.That(conn.ExecuteNonQuery(@"INSERT INTO data (name) VALUES ('test')"), Is.EqualTo(1));
                conn.Close();
                AssertNoPreparedTransactions();
                scope.Complete();
            }
            AssertNumberOfRows(1);
            var pool = PoolManager.Pools[connString];
            Assert.That(pool.Idle, Has.Count.EqualTo(1));

            using (var conn = new NpgsqlConnection(connString))
                NpgsqlConnection.ClearPool(conn);
        }

        [Test]
        public void EnlistToTwoTransactions()
        {
            using (var conn = OpenConnection())
            {
                var ctx = new CommittableTransaction();
                conn.EnlistTransaction(ctx);
                Assert.That(() => conn.EnlistTransaction(new CommittableTransaction()), Throws.Exception.TypeOf<InvalidOperationException>());
                ctx.Rollback();

                using (var tx = conn.BeginTransaction())
                {
                    Assert.That(conn.ExecuteScalar(@"SELECT COUNT(*) FROM data"), Is.EqualTo(0));
                    tx.Rollback();
                }
            }
        }

        [Test]
        public void EnlistTwiceToSameTransaction()
        {
            using (var conn = OpenConnection())
            {
                var ctx = new CommittableTransaction();
                conn.EnlistTransaction(ctx);
                conn.EnlistTransaction(ctx);
                ctx.Rollback();

                using (var tx = conn.BeginTransaction())
                {
                    Assert.That(conn.ExecuteScalar(@"SELECT COUNT(*) FROM data"), Is.EqualTo(0));
                    tx.Rollback();
                }
            }
        }

        [Test]
        public void ScopeAfterScope()
        {
            using (var conn = OpenConnection())
            {
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
        }

        [Test]
        public void ReuseConnection()
        {
            using (var scope = new TransactionScope())
            {
                using (var conn = new NpgsqlConnection(ConnectionStringWithEnlist))
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
            }
            AssertNumberOfRows(2);
        }

        [Test]
        public void ReuseConnectionRollback()
        {
            using (var scope = new TransactionScope())
            using (var conn = new NpgsqlConnection(ConnectionStringWithEnlist))
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
        public void ReuseConnectionWithEscalation()
        {
            using (new TransactionScope())
            {
                using (var conn1 = new NpgsqlConnection(ConnectionStringWithEnlist))
                {
                    conn1.Open();
                    var processId = conn1.ProcessID;
                    using (var conn2 = new NpgsqlConnection(ConnectionStringWithEnlist)) {}
                    conn1.Close();

                    conn1.Open();
                    Assert.That(conn1.ProcessID, Is.EqualTo(processId));
                    conn1.Close();
                }
            }
        }

        [Test]
        public void TimeoutTriggersRollbackWhileBusy()
        {
            using (var conn = OpenConnection())
            {
                using (new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromSeconds(1)))
                {
                    conn.EnlistTransaction(Transaction.Current);
                    Assert.That(() => CreateSleepCommand(conn, 5).ExecuteNonQuery(),
                        Throws.Exception.TypeOf<PostgresException>()
                            .With.Property(nameof(PostgresException.SqlState))
                            .EqualTo("57014"));

                }
            }
            AssertNumberOfRows(0);
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1579")]
        public void SchemaConnectionShouldntEnlist()
        {
            using (var tran = new TransactionScope())
            using (var conn = OpenConnection(ConnectionStringWithEnlist))
            {
                using (var cmd = new NpgsqlCommand("SELECT * FROM data", conn))
                using (var reader = cmd.ExecuteReader(CommandBehavior.KeyInfo))
                {
                    reader.GetColumnSchema();
                    AssertNoPreparedTransactions();
                    tran.Complete();
                }
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1594")]
        public void Bug1594()
        {
            using (new TransactionScope())
            {
                using (var conn = OpenConnection(ConnectionStringWithEnlist))
                using (var innerScope1 = new TransactionScope())
                {
                    conn.ExecuteNonQuery(@"INSERT INTO data (name) VALUES ('test1')");
                    innerScope1.Complete();
                }
                using (OpenConnection(ConnectionStringWithEnlist))
                using (new TransactionScope())
                {
                    // Don't complete, triggering rollback
                }
            }
        }

        void AssertNoPreparedTransactions()
            => Assert.That(GetNumberOfPreparedTransactions(), Is.EqualTo(0));

        int GetNumberOfPreparedTransactions()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM pg_prepared_xacts WHERE database = @database", conn))
            {
                cmd.Parameters.Add(new NpgsqlParameter("database", conn.Database));
                return (int)(long)cmd.ExecuteScalar();
            }
        }

        void AssertNumberOfRows(int expected)
          => Assert.That(_controlConn.ExecuteScalar(@"SELECT COUNT(*) FROM data"), Is.EqualTo(expected));

        public static string ConnectionStringWithEnlist =
            new NpgsqlConnectionStringBuilder(ConnectionString) { Enlist = true }.ToString();

        #region Setup

        [CanBeNull]
        NpgsqlConnection _controlConn;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            using (new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                try
                {
                    Transaction.Current.EnlistPromotableSinglePhase(new FakePromotableSinglePhaseNotification());
                }
                catch (NotImplementedException)
                {
                    Assert.Ignore("Promotable single phase transactions aren't supported (mono < 3.0.0?)");
                }
            }

            _controlConn = OpenConnection();

            // Make sure prepared transactions are enabled in postgresql.conf (disabled by default)
            if (int.Parse((string)_controlConn.ExecuteScalar("SHOW max_prepared_transactions")) == 0)
            {
                TestUtil.IgnoreExceptOnBuildServer("max_prepared_transactions is set to 0 in your postgresql.conf");
                _controlConn.Close();
            }

            // Rollback any lingering prepared transactions from failed previous runs
            var lingeringTrqnsqctions = new List<string>();
            using (var cmd = new NpgsqlCommand("SELECT gid FROM pg_prepared_xacts WHERE database=@database", _controlConn))
            {
                cmd.Parameters.AddWithValue("database", new NpgsqlConnectionStringBuilder(ConnectionString).Database);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        lingeringTrqnsqctions.Add(reader.GetString(0));
                }
            }
            foreach (var xactGid in lingeringTrqnsqctions)
                _controlConn.ExecuteNonQuery($"ROLLBACK PREPARED '{xactGid}'");

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

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _controlConn?.Close();
            _controlConn = null;
        }

        class FakePromotableSinglePhaseNotification : IPromotableSinglePhaseNotification
        {
            public byte[] Promote() { return null; }
            public void Initialize() {}
            public void SinglePhaseCommit(SinglePhaseEnlistment singlePhaseEnlistment) {}
            public void Rollback(SinglePhaseEnlistment singlePhaseEnlistment) {}
        }

        #endregion
    }
}
#endif
