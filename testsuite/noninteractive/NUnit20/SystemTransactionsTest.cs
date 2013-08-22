using System;
using System.Data;
using System.Transactions;
using Npgsql;
using NpgsqlTypes;
using NUnit.Framework;

namespace NpgsqlTests
{
    [TestFixture]
    public class SystemTransactionsTest : BaseClassTests
    {
        protected override NpgsqlConnection TheConnection
        {
            get { return _conn; }
        }

        protected override NpgsqlTransaction TheTransaction
        {
            get { return _t; }
            set { _t = value; }
        }

        protected virtual string TheConnectionString
        {
            get { return _connString; }
        }

        //[Test, Description("TransactionScope with a single connection, enlisting explicitly")]
        [Test]
        public void SimpleTransactionScopeWithExplicitEnlist()
        {
            try
            {
                var connectionString = TheConnectionString;
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    using (var scope = new TransactionScope())
                    {
                        connection.Open();
                        connection.EnlistTransaction(Transaction.Current);
                        var command = new NpgsqlCommand("insert into tablea(field_text) values (:p0)", connection);
                        command.Parameters.Add(new NpgsqlParameter("p0", "test"));
                        Assert.AreEqual(1, command.ExecuteNonQuery());
                        scope.Complete();
                    }
                }
                AssertNoTransactions();

                // Clean up data left in the last transaction.
                var cleanCommand = new NpgsqlCommand("delete from tablea where field_text = (:p0)", TheConnection);
                cleanCommand.Parameters.Add(new NpgsqlParameter("p0", "test"));
                Assert.AreEqual(1, cleanCommand.ExecuteNonQuery());

                // This transaction should commit.
                CommitTransaction = true;
            }
            catch (System.NotImplementedException)
            {
            }
            // Mono version below 3.0 doesn't implement transaction methods. So just ignore. 
        }

        //[Test, Description("TransactionScope with a single connection, enlisting implicitly")]
        [Test]
        public void SimpleTransactionScopeWithImplicitEnlist()
        {
            try
            {
                var connectionString = TheConnectionString + ";enlist=true";
                using (var scope = new TransactionScope())
                {
                    using (var connection = new NpgsqlConnection(connectionString))
                    {
                        connection.Open();
                        var command = new NpgsqlCommand("insert into tablea(field_text) values (:p0)", connection);
                        command.Parameters.Add(new NpgsqlParameter("p0", "test"));
                        Assert.AreEqual(1, command.ExecuteNonQuery());
                    }
                    scope.Complete();
                }
                AssertNoTransactions();


                // Clean up data left in the last transaction.
                var cleanCommand = new NpgsqlCommand("delete from tablea where field_text = (:p0)", TheConnection);
                cleanCommand.Parameters.Add(new NpgsqlParameter("p0", "test"));
                Assert.AreEqual(1, cleanCommand.ExecuteNonQuery());

                // This transaction should commit.
                CommitTransaction = true;
            }
            catch (System.NotImplementedException)
            {
            }
            // Mono version below 3.0 doesn't implement transaction methods. So just ignore. 
        }

        [Test]
        public void DistributedTransactionRollback()
        {
            int field_serial1;
            int field_serial2;
            try
            {
                var connectionString = TheConnectionString + ";enlist=true";
                using (var scope = new TransactionScope())
                {
                    //UseStringParameterWithNoNpgsqlDbType
                    using (var connection = new NpgsqlConnection(connectionString))
                    {
                        connection.Open();
                        var command = new NpgsqlCommand("insert into tablea(field_text) values (:p0)", connection);
                        command.Parameters.Add(new NpgsqlParameter("p0", "test"));
                        Assert.AreEqual(command.Parameters[0].NpgsqlDbType, NpgsqlDbType.Text);
                        Assert.AreEqual(command.Parameters[0].DbType, DbType.String);
                        object result = command.ExecuteNonQuery();
                        Assert.AreEqual(1, result);

                        field_serial1 = (int) new NpgsqlCommand("select max(field_serial) from tablea", connection).ExecuteScalar();
                        var command2 = new NpgsqlCommand("select field_text from tablea where field_serial = (select max(field_serial) from tablea)", connection);
                        result = command2.ExecuteScalar();
                        Assert.AreEqual("test", result);
                    }
                    //UseIntegerParameterWithNoNpgsqlDbType
                    using (var connection = new NpgsqlConnection(connectionString))
                    {
                        connection.Open();
                        var command = new NpgsqlCommand("insert into tablea(field_int4) values (:p0)", connection);
                        command.Parameters.Add(new NpgsqlParameter("p0", 5));
                        Assert.AreEqual(command.Parameters[0].NpgsqlDbType, NpgsqlDbType.Integer);
                        Assert.AreEqual(command.Parameters[0].DbType, DbType.Int32);
                        Object result = command.ExecuteNonQuery();
                        Assert.AreEqual(1, result);

                        field_serial2 = (int) new NpgsqlCommand("select max(field_serial) from tablea", connection).ExecuteScalar();
                        var command2 = new NpgsqlCommand( "select field_int4 from tablea where field_serial = (select max(field_serial) from tablea)", connection);
                        result = command2.ExecuteScalar();
                        Assert.AreEqual(5, result);

                        // using new connection here... make sure we can't see previous results even though
                        // it is the same distributed transaction
                        var command3 = new NpgsqlCommand("select field_text from tablea where field_serial = :p0", connection);
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
                AssertNoTransactions();
                // ensure they no longer exist since we rolled back
                AssertRowNotExist("field_text", field_serial1);
                AssertRowNotExist("field_int4", field_serial2);
            }
            catch (NotImplementedException)
            {

            }
        }

        [Test]
        public void FunctionTestTimestamptzParameterSupport()
        {
            try
            {
                var connectionString = TheConnectionString + ";enlist=true";
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

                        Assert.IsTrue(dt.Rows.Count > 1);
                    }
                }
            }
            catch (NotImplementedException)
            {

            }
        }

        private void AssertNoTransactions()
        {
            // ensure no transactions remain
            var command = new NpgsqlCommand("select count(*) from pg_prepared_xacts where database = :database", TheConnection);
            command.Parameters.Add(new NpgsqlParameter("database", TheConnection.Database));
            Assert.AreEqual(0, command.ExecuteScalar());
        }

        private void AssertRowNotExist(string columnName, int field_serial)
        {
            var command = new NpgsqlCommand("select " + columnName + " from tablea where field_serial = :p0", TheConnection);
            command.Parameters.Add(new NpgsqlParameter("p0", field_serial));
            object result = command.ExecuteScalar();
            Assert.AreEqual(null, result);
        }

        [Test]
        public void TwoDistributedInSequence()
        {
            DistributedTransactionRollback();
            DistributedTransactionRollback();
        }

        [Test]
        public void ReuseConnection()
        {
            try
            {
                var connectionString = TheConnectionString + ";enlist=true";
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
            catch (NotImplementedException)
            {

            }
        }
    }

    public class SystemTransactionsTestV2 : SystemTransactionsTest
    {
        protected override NpgsqlConnection TheConnection
        {
            get { return _connV2; }
        }

        protected override NpgsqlTransaction TheTransaction
        {
            get { return _tV2; }
            set { _tV2 = value; }
        }

        protected override string TheConnectionString
        {
            get { return _connV2String; }
        }
    }
}
