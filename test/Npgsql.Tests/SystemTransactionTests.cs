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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Transactions;
using JetBrains.Annotations;
using NUnit.Framework;
using NUnit.Framework.Internal.Commands;

// This test suite contains ambient transaction tests, except those involving distributed transactions which are only
// supported on .NET Framework / Windows. Distributed transaction tests are in DistributedTransactionTests.

namespace Npgsql.Tests
{
    [NonParallelizable]
    public class SystemTransactionTests : TestBase
    {
        [Test, Description("Single connection enlisting explicitly, committing")]
        public void ExplicitEnlist()
        {
            using (var conn = new NpgsqlConnection(ConnectionStringEnlistOff))
            {
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
        }

        [Test, Description("Single connection enlisting implicitly, committing")]
        public void ImplicitEnlist()
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
        public void EnlistOff()
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
        public void RollbackExplicitEnlist()
        {
            using (var conn = OpenConnection())
            {
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
        }

        [Test, Description("Single connection enlisting implicitly, rollback")]
        public void RollbackImplicitEnlist()
        {
            using (new TransactionScope())
            using (var conn = OpenConnection(ConnectionStringEnlistOn))
            {
                Assert.That(conn.ExecuteNonQuery(@"INSERT INTO data (name) VALUES ('test')"), Is.EqualTo(1), "Unexpected insert rowcount");
                AssertNoDistributedIdentifier();
                AssertNoPreparedTransactions();
                // No commit
            }

            AssertNumberOfRows(0);
        }

        [Test]
        public void TwoConsecutiveConnections()
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

                // Consecutive connections used in same scope should not promote the
                // transaction to distributed.
                AssertNoDistributedIdentifier();
                AssertNoPreparedTransactions();
                scope.Complete();
            }
            AssertNumberOfRows(2);
        }

        [Test]
        public void CloseConnection()
        {
            var connString = new NpgsqlConnectionStringBuilder(ConnectionStringEnlistOn)
            {
                ApplicationName = nameof(CloseConnection),
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
            Assert.That(PoolManager.TryGetValue(connString, out var pool), Is.True);
            Assert.That(pool.State.Idle, Is.EqualTo(1));

            using (var conn = new NpgsqlConnection(connString))
                NpgsqlConnection.ClearPool(conn);
        }

        [Test]
        public void EnlistToTwoTransactions()
        {
            using (var conn = OpenConnection(ConnectionStringEnlistOff))
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
            using (var conn = OpenConnection(ConnectionStringEnlistOff))
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
            using (var conn = OpenConnection(ConnectionStringEnlistOff))
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
        public void ReuseConnectionRollback()
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
        public void TimeoutTriggersRollbackWhileBusy()
        {
            using (var conn = OpenConnection(ConnectionStringEnlistOff))
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
            using (var conn = OpenConnection(ConnectionStringEnlistOn))
            {
                using (var cmd = new NpgsqlCommand("SELECT * FROM data", conn))
                using (var reader = cmd.ExecuteReader(CommandBehavior.KeyInfo))
                {
                    reader.GetColumnSchema();
                    AssertNoDistributedIdentifier();
                    AssertNoPreparedTransactions();
                    tran.Complete();
                }
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1737")]
        public void Bug1737()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                Pooling = false,
                Enlist = true
            };

            // Case 1
            using (var scope = new TransactionScope())
            {
                using (var conn = OpenConnection(csb))
                using (var cmd = new NpgsqlCommand("SELECT 1", conn))
                    cmd.ExecuteNonQuery();
                scope.Complete();
            }

            // Case 2
            using (var scope = new TransactionScope())
            {
                using (var conn1 = OpenConnection(csb))
                using (var cmd = new NpgsqlCommand("SELECT 1", conn1))
                    cmd.ExecuteNonQuery();

                using (var conn2 = OpenConnection(csb))
                using (var cmd = new NpgsqlCommand("SELECT 1", conn2))
                    cmd.ExecuteNonQuery();

                scope.Complete();
            }
        }

        #region Utilities

        void AssertNoPreparedTransactions()
            => Assert.That(GetNumberOfPreparedTransactions(), Is.EqualTo(0), "Prepared transactions found");

        int GetNumberOfPreparedTransactions()
        {
            using (var conn = OpenConnection(ConnectionStringEnlistOff))
            using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM pg_prepared_xacts WHERE database = @database", conn))
            {
                cmd.Parameters.Add(new NpgsqlParameter("database", conn.Database));
                return (int)(long)cmd.ExecuteScalar();
            }
        }

        void AssertNumberOfRows(int expected)
          => Assert.That(_controlConn.ExecuteScalar(@"SELECT COUNT(*) FROM data"), Is.EqualTo(expected), "Unexpected data count");

        static void AssertNoDistributedIdentifier()
            => Assert.That(Transaction.Current?.TransactionInformation.DistributedIdentifier ?? Guid.Empty, Is.EqualTo(Guid.Empty), "Distributed identifier found");

        public static string ConnectionStringEnlistOn =
            new NpgsqlConnectionStringBuilder(ConnectionString) { Enlist = true }.ToString();

        public static string ConnectionStringEnlistOff =
            new NpgsqlConnectionStringBuilder(ConnectionString) { Enlist = false }.ToString();

        #endregion Utilities

        #region Setup

        [CanBeNull]
        NpgsqlConnection _controlConn;

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

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _controlConn?.Close();
            _controlConn = null;
        }

        #endregion
    }
}
