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

        private void CommonTestSequence(string connectionString, out int field_serial1, out int field_serial2, bool sameConn = false)
        {
            object result1, result2;
            //UseStringParameterWithNoNpgsqlDbType
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                var command = new NpgsqlCommand("insert into data (field_text) values (:p0)", connection);
                command.Parameters.Add(new NpgsqlParameter("p0", "test"));
                Assert.AreEqual(command.Parameters[0].NpgsqlDbType, NpgsqlDbType.Text);
                Assert.AreEqual(command.Parameters[0].DbType, DbType.String);
                result1 = command.ExecuteNonQuery();
                Assert.AreEqual(1, result1);

                field_serial1 = (int)new NpgsqlCommand("select max(field_serial) from data", connection).ExecuteScalar();
                var command2 = new NpgsqlCommand("select field_text from data where field_serial = (select max(field_serial) from data)", connection);
                result1 = command2.ExecuteScalar();
                Assert.AreEqual("test", result1);
            }
            //UseIntegerParameterWithNoNpgsqlDbType
            using (var connection = new NpgsqlConnection(connectionString + (sameConn ? "" : ";Timeout = 20")))
            {
                connection.Open();
                var command = new NpgsqlCommand("insert into data(field_int4) values (:p0)", connection);
                command.Parameters.Add(new NpgsqlParameter("p0", 5));
                Assert.AreEqual(command.Parameters[0].NpgsqlDbType, NpgsqlDbType.Integer);
                Assert.AreEqual(command.Parameters[0].DbType, DbType.Int32);
                result2 = command.ExecuteNonQuery();
                Assert.AreEqual(1, result2);

                field_serial2 = (int)new NpgsqlCommand("select max(field_serial) from data", connection).ExecuteScalar();
                var command2 = new NpgsqlCommand("select field_int4 from data where field_serial = (select max(field_serial) from data)", connection);
                result2 = command2.ExecuteScalar();
                Assert.AreEqual(5, result2);

                // using new connection here... make sure we can't see previous results even though
                // it is the same distributed transaction
                var command3 = new NpgsqlCommand("select field_text from data where field_serial = :p0", connection);
                command3.Parameters.Add(new NpgsqlParameter("p0", field_serial1));
                result2 = command3.ExecuteScalar();

                if (!sameConn)
                {
                    // won't see value of "test" since that's
                    // another connection
                    Assert.AreEqual(null, result2);
                }
                else
                {

                    Assert.AreEqual(result1, result2);
                }
                
            }
        }

        private void CommonTestSequence2(string connectionString, int _case, out int field_serial1, out int field_serial2)
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

                field_serial1 = (int)new NpgsqlCommand("select max(field_serial) from data", connection).ExecuteScalar();
            }
            //UseIntegerParameterWithNoNpgsqlDbType
            using (var connection = new NpgsqlConnection(connectionString + ";Timeout = 20"))
            {
                connection.Open();
                var command = new NpgsqlCommand("insert into data(field_int4) values (:p0)", connection);
                command.Parameters.Add(new NpgsqlParameter("p0", 5));
                Assert.AreEqual(command.Parameters[0].NpgsqlDbType, NpgsqlDbType.Integer);
                Assert.AreEqual(command.Parameters[0].DbType, DbType.Int32);
                Object result = command.ExecuteNonQuery();
                Assert.AreEqual(1, result);

                field_serial2 = (int)new NpgsqlCommand("select max(field_serial) from data", connection).ExecuteScalar();
                switch (_case)
                {
                    case 1:
                        new NpgsqlCommand("select 1/0", connection).ExecuteNonQuery();
                        break;
                    case 2:
                        throw new CustomException("fake here");
                }
            }
        }

        [Test]
        public void DistributedTransactionRollback()
        {
            int field_serial1;
            int field_serial2;
            var connectionString = ConnectionString + ";enlist=true";

            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {
                CommonTestSequence(connectionString, out field_serial1, out field_serial2);
                // not commiting here.
            }
            AssertNoPreparedTransactions();
            // ensure they no longer exist since we rolled back
            AssertRowNotExist("field_text", field_serial1);
            AssertRowNotExist("field_int4", field_serial2);
        }

        [Test]
        public void DistributedTransactionRollbackDueSqlError()
        {
            // It tests correct rollback if exception is raised during call to PREPARE TRANSACTION
            // of a second transaction
            int field_serial1 = -1;
            int field_serial2 = -2;
            var connectionString = ConnectionString + ";enlist=true";
            bool got_exception = false;

            try
            {
                using (var scope = new TransactionScope())
                {
                    CommonTestSequence2(connectionString, 1, out field_serial1, out field_serial2);
                    // not commiting here.
                    scope.Complete();
                }
            }
            catch (NpgsqlException e)
            {
                Assert.That(e.Code, Is.EqualTo("22012"));
                got_exception = true;
            }
            Assert.True(got_exception);
            AssertNoPreparedTransactions();
            Assert.That(field_serial1, Is.Not.EqualTo(-1));
            Assert.That(field_serial2, Is.Not.EqualTo(-2));
            // ensure they no longer exist since we rolled back
            AssertRowNotExist("field_text", field_serial1);
            AssertRowNotExist("field_int4", field_serial2);
        }

        [Test]
        public void DistributedTransactionRollbackDueException()
        {
            // It tests correct rollback if exception is raised during call to PREPARE TRANSACTION
            // of a second transaction
            int field_serial1 = -1;
            int field_serial2 = -2;
            var connectionString = ConnectionString + ";enlist=true";
            bool got_exception = false;

            try
            {
                using (var scope = new TransactionScope())
                {
                    CommonTestSequence2(connectionString, 2, out field_serial1, out field_serial2);
                    // not commiting here.
                    scope.Complete();
                }
            }
            catch (CustomException e)
            {
                got_exception = true;
            }
            Assert.True(got_exception);
            AssertNoPreparedTransactions();
            Assert.That(field_serial1, Is.Not.EqualTo(-1));
            Assert.That(field_serial2, Is.Not.EqualTo(-2));
            // ensure they no longer exist since we rolled back
            AssertRowNotExist("field_text", field_serial1);
            AssertRowNotExist("field_int4", field_serial2);
        }

        [Test]
        public void DistributedTransactionRollbackDueSerializationError()
        {
            // It tests correct rollback if exception is raised during call to PREPARE TRANSACTION
            // of a second transaction
            int field_serial1 = -1;
            int field_serial2 = -2;
            var connectionString = ConnectionString + ";enlist=true";
            bool got_exception = false;
            if (Conn.PostgreSqlVersion < new Version(9, 1, 0))
            {
                // no serialization error in Serializable isolation level prior 9.1
                return;
            }

            try
            {
                using (var scope = new TransactionScope())
                {
                    CommonTestSequence(connectionString, out field_serial1, out field_serial2);
                    // not commiting here.
                    scope.Complete();
                }
            }
            catch (TransactionAbortedException e)
            {
                Assert.That((e.InnerException as NpgsqlException).Code, Is.EqualTo("40001"));
                got_exception = true;
            }
            Assert.True(got_exception);
            AssertNoPreparedTransactions();
            Assert.That(field_serial1, Is.Not.EqualTo(-1));
            Assert.That(field_serial2, Is.Not.EqualTo(-2));
            // ensure they no longer exist since we rolled back
            AssertRowNotExist("field_text", field_serial1);
            AssertRowNotExist("field_int4", field_serial2);
        }

        [Test]
        public void DistributedTransactionCommit()
        {
            int field_serial1;
            int field_serial2;
            var connectionString = ConnectionString + ";enlist=true";
            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {
                CommonTestSequence(connectionString, out field_serial1, out field_serial2);
                scope.Complete();
            }
            AssertNoPreparedTransactions();
            // ensure they are existing now
            AssertRowExist("field_text", field_serial1);
            AssertRowExist("field_int4", field_serial2);
        }

        [Test]
        public void DistributedTransactionCommit2()
        {
            int field_serial1;
            int field_serial2;
            var connectionString = ConnectionString + ";enlist=true;Serializable As=RepeatableRead";
            using (var scope = new TransactionScope())
            {
                CommonTestSequence(connectionString, out field_serial1, out field_serial2);
                scope.Complete();
            }
            AssertNoPreparedTransactions();
            // ensure they are existing now
            AssertRowExist("field_text", field_serial1);
            AssertRowExist("field_int4", field_serial2);
        }

        [Test]
        public void TwoDistributedInSequence()
        {
            DistributedTransactionRollback();
            DistributedTransactionRollback();
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

        private void AssertRowExist(string columnName, int field_serial)
        {
            using (var cmd = new NpgsqlCommand("select " + columnName + " from data where field_serial = :p0", Conn))
            {
                cmd.Parameters.Add(new NpgsqlParameter("p0", field_serial));
                Assert.That(cmd.ExecuteScalar(), Is.Not.Null);
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
                    var processId = connection.ProcessID;
                    connection.Close();
                    connection.Open();
                    Assert.AreEqual(processId, connection.ProcessID);
                    connection.Close();
                }
            }
        }

        [Test]
        public void ReuseConnection2()
        {
            var connectionString = ConnectionString + ";enlist=true";
            using (var scope = new TransactionScope())
            {
                int processId;
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    processId = connection.ProcessID;
                }
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    Assert.AreEqual(processId, connection.ProcessID);
                }
            }
        }

        [Test]
        public void ReuseConnection3()
        {
            int field_serial1;
            int field_serial2;
            var connectionString = ConnectionString + ";enlist=true";
            using (var scope = new TransactionScope())
            {
                CommonTestSequence(connectionString, out field_serial1, out field_serial2, true);
                scope.Complete();
            }
            AssertNoPreparedTransactions();
            // ensure they are existing now
            AssertRowExist("field_text", field_serial1);
            AssertRowExist("field_int4", field_serial2);
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

        class CustomException : Exception
        {
            public CustomException(string message)
                : base(message)
            {
            }
        }
        #endregion
    }
}
