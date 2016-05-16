#region License
// The PostgreSQL License
//
// Copyright (C) 2016 The Npgsql Development Team
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

#if NET451

using System;
using System.Data;
using System.Reflection;
using System.Transactions;
using Npgsql;
using NpgsqlTypes;
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
            using (var scope = new TransactionScope())
            {
                conn.Open();
                conn.EnlistTransaction(Transaction.Current);
                Assert.That(conn.ExecuteNonQuery(@"INSERT INTO data (name) VALUES('test')"), Is.EqualTo(1));
                scope.Complete();
            }
            AssertNoPreparedTransactions();
            using (var conn = OpenConnection())
                Assert.That(conn.ExecuteScalar(@"SELECT COUNT(*) FROM data"), Is.EqualTo(1));
        }

        [Test, Description("Single connection enlisting implicitly, committing")]
        public void ImplicitEnlist()
        {
            var connectionString = ConnectionString + ";enlist=true";
            using (var scope = new TransactionScope())
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    Assert.That(conn.ExecuteNonQuery(@"INSERT INTO data (name) VALUES('test')"), Is.EqualTo(1));
                }
                scope.Complete();
            }
            AssertNoPreparedTransactions();
            using (var conn = OpenConnection())
                Assert.That(conn.ExecuteScalar(@"SELECT COUNT(*) FROM data"), Is.EqualTo(1));
        }

        [Test, Description("Single connection rollback")]
        public void Rollback()
        {
            var connectionString = ConnectionString + ";enlist=true";
            using (var scope = new TransactionScope())
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                Assert.That(conn.ExecuteNonQuery(@"INSERT INTO data (name) VALUES('test')"), Is.EqualTo(1));
                // No commit
            }
            AssertNoPreparedTransactions();
            using (var conn = OpenConnection())
                Assert.That(conn.ExecuteScalar(@"SELECT COUNT(*) FROM data"), Is.EqualTo(0));
        }

        [Test]
        [Ignore("Causing a weird timeout issue for other tests? Also clean up...")]
        public void DistributedTransactionRollback()
        {
            int field_serial1;
            int field_serial2;
            var connectionString = ConnectionString + ";enlist=true";
            using (var scope = new TransactionScope())
            {
                //UseStringParameterWithNoNpgsqlDbType
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    var command = new NpgsqlCommand("insert into data (field_text) values (:p0)", connection);
                    command.Parameters.Add(new NpgsqlParameter("p0", "test"));
                    Assert.AreEqual(command.Parameters[0].NpgsqlDbType, NpgsqlDbType.Text);
                    Assert.AreEqual(command.Parameters[0].DbType, DbType.String);
                    object result = command.ExecuteNonQuery();
                    Assert.AreEqual(1, result);

                    field_serial1 = (int) new NpgsqlCommand("select max(field_serial) from data", connection).ExecuteScalar();
                    var command2 = new NpgsqlCommand("select field_text from data where field_serial = (select max(field_serial) from data)", connection);
                    result = command2.ExecuteScalar();
                    Assert.AreEqual("test", result);
                }
                //UseIntegerParameterWithNoNpgsqlDbType
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    var command = new NpgsqlCommand("insert into data(field_int4) values (:p0)", connection);
                    command.Parameters.Add(new NpgsqlParameter("p0", 5));
                    Assert.AreEqual(command.Parameters[0].NpgsqlDbType, NpgsqlDbType.Integer);
                    Assert.AreEqual(command.Parameters[0].DbType, DbType.Int32);
                    Object result = command.ExecuteNonQuery();
                    Assert.AreEqual(1, result);

                    field_serial2 = (int) new NpgsqlCommand("select max(field_serial) from data", connection).ExecuteScalar();
                    var command2 = new NpgsqlCommand( "select field_int4 from data where field_serial = (select max(field_serial) from data)", connection);
                    result = command2.ExecuteScalar();
                    Assert.AreEqual(5, result);

                    // using new connection here... make sure we can't see previous results even though
                    // it is the same distributed transaction
                    var command3 = new NpgsqlCommand("select field_text from data where field_serial = :p0", connection);
                    command3.Parameters.Add(new NpgsqlParameter("p0", field_serial1));
                    result = command3.ExecuteScalar();

                    // won't see value of "test" since that's
                    // another connection
                    Assert.AreEqual(null, result);
                }
                // not commiting here.
            }
            // This is an attempt to wait for the distributed transaction to rollback
            // not guaranteed to work, but should be good enough for testing purposes.
            System.Threading.Thread.Sleep(500);
            AssertNoPreparedTransactions();
            // ensure they no longer exist since we rolled back
            AssertRowNotExist("field_text", field_serial1);
            AssertRowNotExist("field_int4", field_serial2);
        }

        [Test]
        [Ignore("Causing a weird timeout issue for other tests? Also clean up...")]
        public void TwoDistributedInSequence()
        {
            DistributedTransactionRollback();
            DistributedTransactionRollback();
        }

        [Test, Description("Makes sure that when a timeout occurs, the transaction is rolled backed")]
        [IssueLink("https://github.com/npgsql/npgsql/issues/495")]
        [MonoIgnore("Mono's TransactionScope doesn't enforce the timeout: https://bugzilla.xamarin.com/show_bug.cgi?id=31197")]
        public void RollbackOnTimeout()
        {
            Assert.That(() =>
            {
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 0, 1)))
                {
                    using (var conn = new NpgsqlConnection(ConnectionString + ";enlist=true"))
                    {
                        conn.Open();
                        var cmd = new NpgsqlCommand(@"INSERT INTO data (name) VALUES ('HELLO')", conn);
                        cmd.ExecuteNonQuery(); // the update operation is expected to rollback
                        System.Threading.Thread.Sleep(2000);
                    }
                    scope.Complete();
                }
            }, Throws.Exception.AssignableTo<TransactionException>());
            using (var conn = OpenConnection())
                Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM data"), Is.EqualTo(0));
        }

        [Test, Description("Not sure what this test is supposed to check...")]
        [Ignore("There's a real failure here")]
        public void FunctionTestTimestamptzParameterSupport()
        {
            /*
            ExecuteNonQuery(@"INSERT INTO data (name) VALUES ('X')");
            ExecuteNonQuery(@"CREATE OR REPLACE FUNCTION testtimestamptzparameter(timestamptz) returns refcursor as
                              $BODY$
                              declare ref refcursor;
                              begin
                                      open ref for select * from data;
                                      return ref;
                              end
                              $BODY$
                              language 'plpgsql' volatile called on null input security invoker;");
            var connectionString = ConnectionString + ";enlist=true";
            using (var scope = new TransactionScope())
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    var command = new NpgsqlCommand("testtimestamptzparameter", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add(new NpgsqlParameter("p1", NpgsqlDbType.TimestampTZ));

                    //NpgsqlDataReader dr = command.ExecuteReader();

                    //Int32 count = 0;

                    //while (dr.Read())
                    //count++;
                    var da = new NpgsqlDataAdapter(command);
                    var dt = new DataTable();
                    da.Fill(dt);

                    Assert.AreEqual(1, dt.Rows.Count);
                }
            }
            */
        }

        private void AssertNoPreparedTransactions()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("select count(*) from pg_prepared_xacts where database = :database", conn))
            {
                cmd.Parameters.Add(new NpgsqlParameter("database", conn.Database));
                Assert.That(cmd.ExecuteScalar(), Is.EqualTo(0));
            }
        }

        private void AssertRowNotExist(string columnName, int field_serial)
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("select " + columnName + " from data where field_serial = :p0", conn))
            {
                cmd.Parameters.Add(new NpgsqlParameter("p0", field_serial));
                Assert.That(cmd.ExecuteScalar(), Is.Null);
            }
        }

        [Test]
        public void ReuseConnection()
        {
            var connectionString = ConnectionString + ";enlist=true";
            using (var scope = new TransactionScope())
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    conn.Close();
                    conn.Open();
                    conn.Close();
                }
            }
        }

        #region Setup

        [SetUp]
        public void SetUp()
        {
            using (var s = new TransactionScope(TransactionScopeOption.RequiresNew))
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

            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("DROP TABLE IF EXISTS data");
                conn.ExecuteNonQuery("CREATE TABLE data (name TEXT)");
            }
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