using System;
using System.Data;
using System.Reflection;
using System.Transactions;
using Npgsql;
using NpgsqlTypes;
using NUnit.Framework;

namespace NpgsqlTests
{
    [TestFixture]
    public class SystemTransactionsTest : TestBase
    {
        public SystemTransactionsTest(string backendVersion) : base(backendVersion) { }

        [Test, Description("Single connection enlisting explicitly, committing")]
        public void ExplicitEnlist()
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                using (var scope = new TransactionScope())
                {
                    connection.Open();
                    connection.EnlistTransaction(Transaction.Current);
                    Assert.That(ExecuteNonQuery(@"INSERT INTO data (field_text) VALUES('test')", connection), Is.EqualTo(1));
                    scope.Complete();
                }
            }
            AssertNoPreparedTransactions();
            Assert.That(ExecuteScalar(@"SELECT COUNT(*) FROM data"), Is.EqualTo(1));
        }

        [Test, Description("Single connection enlisting implicitly, committing")]
        public void ImplicitEnlist()
        {
            var connectionString = ConnectionString + ";enlist=true";
            using (var scope = new TransactionScope())
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    Assert.That(ExecuteNonQuery(@"INSERT INTO data (field_text) VALUES('test')", connection), Is.EqualTo(1));
                }
                scope.Complete();
            }
            AssertNoPreparedTransactions();
            Assert.That(ExecuteScalar(@"SELECT COUNT(*) FROM data"), Is.EqualTo(1));
        }

        [Test, Description("Single connection rollback")]
        public void Rollback()
        {
            var connectionString = ConnectionString + ";enlist=true";
            using (var scope = new TransactionScope())
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    Assert.That(ExecuteNonQuery(@"INSERT INTO data (field_text) VALUES('test')", connection), Is.EqualTo(1));
                    // No commit
                }
            }
            AssertNoPreparedTransactions();
            Assert.That(ExecuteScalar(@"SELECT COUNT(*) FROM data"), Is.EqualTo(0));
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
        public void RollbackOnTimeout()
        {
            Assert.That(() =>
            {
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 0, 1)))
                {
                    using (var conn = new NpgsqlConnection(ConnectionString + ";enlist=true"))
                    {
                        conn.Open();
                        var cmd = new NpgsqlCommand(@"INSERT INTO data (field_text) VALUES ('HELLO')", conn);
                        cmd.ExecuteNonQuery(); // the update operation is expected to rollback
                        System.Threading.Thread.Sleep(2000);
                    }
                    scope.Complete();
                }
            }, Throws.Exception.TypeOf<TransactionAbortedException>());
            Assert.That(ExecuteScalar("SELECT COUNT(*) FROM data"), Is.EqualTo(0));
        }

        [Test, Description("Not sure what this test is supposed to check...")]
        public void FunctionTestTimestamptzParameterSupport()
        {
            ExecuteNonQuery(@"INSERT INTO data (field_text) VALUES ('X')");
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
        }

        private void AssertNoPreparedTransactions()
        {
            using (var cmd = new NpgsqlCommand("select count(*) from pg_prepared_xacts where database = :database", Conn))
            {
                cmd.Parameters.Add(new NpgsqlParameter("database", Conn.Database));
                Assert.That(cmd.ExecuteScalar(), Is.EqualTo(0));
            }
        }

        private void AssertRowNotExist(string columnName, int field_serial)
        {
            using (var cmd = new NpgsqlCommand("select " + columnName + " from data where field_serial = :p0", Conn))
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
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    connection.Close();
                    connection.Open();
                    connection.Close();
                }
            }
        }

        #region Setup

        [SetUp]
        public void CheckPromotableSupport()
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
