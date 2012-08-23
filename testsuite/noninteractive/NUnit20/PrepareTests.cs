using System;
using System.Data;

using Npgsql;
using NUnit.Framework;

namespace NpgsqlTests
{
    /// <summary>
    /// Summary description for PrepareTest.
    /// </summary>
    [TestFixture]
    public class PrepareTest : BaseClassTests
    {
        protected override NpgsqlConnection TheConnection {
            get { return _conn; }
        }
        protected override NpgsqlTransaction TheTransaction {
            get { return _t; }
            set { _t = value; }
        }
        protected override void SetUp()
        {
            base.SetUp();
            string sql = @"	CREATE TABLE public.preparetest
                         (
                         testid serial NOT NULL,
                         varchar_notnull varchar(100) NOT NULL,
                         varchar_null varchar(100),
                         integer_notnull int4 NOT NULL,
                         integer_null int4,
                         bigint_notnull int8 NOT NULL,
                         bigint_null int8
                         ) WITHOUT OIDS;";
            NpgsqlCommand cmd = new NpgsqlCommand(sql, TheConnection);
            cmd.ExecuteNonQuery();
            CommitTransaction = true;
        }


        
        protected override void TearDown()
        {
            
            string sql = @"	DROP TABLE public.preparetest;";
            NpgsqlCommand cmd = new NpgsqlCommand(sql, TheConnection);

            cmd.ExecuteNonQuery();
            CommitTransaction = true;
            base.TearDown();
        }
        

        [Test]
        public void TestInt8Null()
        {
            NpgsqlCommand cmd = GetCommand();


            // Default params work OK
            cmd.ExecuteNonQuery();

            cmd.Parameters[5].Value = System.DBNull.Value;
            //cmd.Parameters[5].Value = null;

            // This too
            cmd.ExecuteNonQuery();

            cmd.Prepare();

            // This will fail
            cmd.ExecuteNonQuery();
        }

        [Test]
        public void TestInt4Null()
        {
            NpgsqlCommand cmd = GetCommand();


            // Default params work OK
            cmd.ExecuteNonQuery();

            cmd.Parameters[3].Value = System.DBNull.Value;

            // This too
            cmd.ExecuteNonQuery();

            cmd.Prepare();

            // This will fail
            cmd.ExecuteNonQuery();
        }

        [Test]
        public void TestVarcharNull()
        {
            NpgsqlCommand cmd = GetCommand();


            // Default params work OK
            cmd.ExecuteNonQuery();

            cmd.Parameters[1].Value = System.DBNull.Value;

            // This too
            cmd.ExecuteNonQuery();

            cmd.Prepare();

            // This inserts a string with the value 'NULL'
            cmd.ExecuteNonQuery();
        }

        private NpgsqlCommand GetCommand()
        {
            string sql = @"	INSERT INTO preparetest(varchar_notnull, varchar_null, integer_notnull, integer_null, bigint_notnull, bigint_null)
                         VALUES(:param1, :param2, :param3, :param4, :param5, :param6)";
            NpgsqlCommand cmd = new NpgsqlCommand(sql, TheConnection);

            NpgsqlParameter p1 = new NpgsqlParameter("param1", DbType.String, 100);
            p1.Value = "One";
            cmd.Parameters.Add(p1);
            NpgsqlParameter p2 = new NpgsqlParameter("param2", DbType.String, 100);
            p2.Value = "Two";
            cmd.Parameters.Add(p2);
            NpgsqlParameter p3 = new NpgsqlParameter("param3", DbType.Int32);
            p3.Value = 3;
            cmd.Parameters.Add(p3);
            NpgsqlParameter p4 = new NpgsqlParameter("param4", DbType.Int32);
            p4.Value = 4;
            cmd.Parameters.Add(p4);
            NpgsqlParameter p5 = new NpgsqlParameter("param5", DbType.Int64);
            p5.Value = 5;
            cmd.Parameters.Add(p5);
            NpgsqlParameter p6 = new NpgsqlParameter("param6", DbType.Int64);
            p6.Value = 6;
            cmd.Parameters.Add(p6);

            return cmd;
        }

        [Test]
        public void TestSubquery()
        {
            string sql = "SELECT testid FROM preparetest WHERE :p1 IN (SELECT varchar_notnull FROM preparetest)";
            NpgsqlCommand cmd = new NpgsqlCommand(sql, TheConnection);
            NpgsqlParameter p1 = new NpgsqlParameter(":p1", DbType.String);
            p1.Value = "blahblah";
            cmd.Parameters.Add(p1);


            cmd.ExecuteNonQuery(); // Succeeds

            cmd.Prepare(); // Fails

            cmd.ExecuteNonQuery();
        }
    }
    [TestFixture]
    public class PrepareTestV2 : PrepareTest
    {
        protected override NpgsqlConnection TheConnection {
            get { return _connV2; }
        }
        protected override NpgsqlTransaction TheTransaction {
            get { return _tV2; }
        }
    }
}
