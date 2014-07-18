using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using NUnit.Framework;

using Npgsql;
using NpgsqlTypes;

namespace NpgsqlTests
{
    [Explicit]
    public class SpeedTests : TestBase
    {
        public SpeedTests(string backendVersion) : base(backendVersion) { }

        private static readonly TimeSpan TestRunTime = new TimeSpan(0, 0, 10); // 10 seconds

        [Test, Description("A minimal, simple, non-query scenario")]
        public void ExecuteUpdateNonQuery()
        {
            using (var metrics = TestMetrics.Start(TestRunTime, true))
            {
                while (!metrics.TimesUp)
                {
                    ExecuteNonQuery("set lock_timeout = 1000");
                    metrics.IncrementIterations();
                }
            }
        }

        [Test, Description("A minimal, simple, scalar scenario")]
        public void ExecuteScalar()
        {
            using (var metrics = TestMetrics.Start(TestRunTime, true))
            {
                while (!metrics.TimesUp)
                {
                    ExecuteScalar("SELECT 1 + 1");
                    metrics.IncrementIterations();
                }
            }
        }

        [Test, Description("A normal insert command with one parameter")]
        public void ParameterizedInsert()
        {
            using (var command = Conn.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = "INSERT INTO data (field_text) values (:data)";

                IDbDataParameter dataParameter = command.CreateParameter();
                dataParameter.Direction = ParameterDirection.Input;
                dataParameter.DbType = DbType.String;
                dataParameter.ParameterName = "data";

                command.Parameters.Add(dataParameter);

                using (var metrics = TestMetrics.Start(TestRunTime, true))
                {
                    while (! metrics.TimesUp)
                    {
                        dataParameter.Value = "yo";
                        command.ExecuteScalar();
                        metrics.IncrementIterations();
                    }
                }
            }
        }

        [Test, Description("A prepared insert command with one parameter")]
        public void ParameterizedPreparedInsert()
        {
            using (var command = Conn.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = "INSERT INTO data (field_text) values (:data)";

                IDbDataParameter dataParameter = command.CreateParameter();
                dataParameter.Direction = ParameterDirection.Input;
                dataParameter.DbType = DbType.String;
                dataParameter.ParameterName = "data";

                command.Parameters.Add(dataParameter);
                command.Prepare();

                using (var metrics = TestMetrics.Start(TestRunTime, true))
                {
                    while (! metrics.TimesUp)
                    {
                        dataParameter.Value = "yo";
                        command.ExecuteScalar();
                        metrics.IncrementIterations();
                    }
                }
            }
        }

        private void ParameterizedSelectDecimalRoundTrip_Internal(bool prepare)
        {
            using (var command = Conn.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT :data";

                NpgsqlParameter dataParameter = command.CreateParameter();
                dataParameter.Direction = ParameterDirection.Input;
                dataParameter.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Numeric;
                dataParameter.ParameterName = "data";
                command.Parameters.Add(dataParameter);
                dataParameter.Value = 12345678.12345678;

                using (var metrics = TestMetrics.Start(TestRunTime, true))
                {
                    while (! metrics.TimesUp)
                    {
                        if (prepare)
                        {
                            command.Prepare();
                        }

                        var data2 = (decimal)command.ExecuteScalar();
                        metrics.IncrementIterations();
                    }
                }
            }
        }

        [Test, Description("A single decimal roundtrip test")]
        public void ParameterizedSelectDecimalRoundTrip()
        {
            ParameterizedSelectDecimalRoundTrip_Internal(false);
        }

        [Test, Description("A single decimal roundtrip test, prepared")]
        public void ParameterizedSelectDecimalRoundTripPrepared()
        {
            ParameterizedSelectDecimalRoundTrip_Internal(true);
        }

        private void ParameterizedSelectByteaRoundTrip_Internal(bool prepare)
        {
            using (var command = Conn.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT :data";

                byte[] data = new byte[100000];
                for (int i = 0  ; i < data.Length ; i++)
                {
                    data[i] = (byte)(i % 255);
                }
                NpgsqlParameter dataParameter = command.CreateParameter();
                dataParameter.Direction = ParameterDirection.Input;
                dataParameter.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Bytea;
                dataParameter.ParameterName = "data";
                command.Parameters.Add(dataParameter);
                dataParameter.Value = data;

                if (prepare)
                {
                    command.Prepare();
                }

                using (var metrics = TestMetrics.Start(TestRunTime, true))
                {
                    while (! metrics.TimesUp)
                    {
                        var data2 = (byte[])command.ExecuteScalar();
                        metrics.IncrementIterations();
                    }
                }
            }
        }

        [Test, Description("A large bytea roundtrip test")]
        public void ParameterizedSelectByteaRoundTrip()
        {
            ParameterizedSelectByteaRoundTrip_Internal(false);
        }

        [Test, Description("A large bytea roundtrip test, prepared")]
        public void ParameterizedSelectByteaRoundTripPrepared()
        {
            ParameterizedSelectByteaRoundTrip_Internal(true);
        }

        [Test, Description("A large bytea roundtrip test, prepared, binary suppressed")]
        public void ParameterizedSelectByteaRoundTripPrepared_SuppressBinary()
        {
            if (SuppressBinaryBackendEncoding != null)
            {
                using (SuppressBackendBinary())
                {
                    ParameterizedSelectByteaRoundTrip_Internal(true);
                }
            }
            else
            {
                Console.WriteLine("Binary suppression not supported in this version of Npgsql; test not executed");
            }
        }

        private void ParameterizedSelectBigIntRoundTrip_Internal(bool prepare)
        {
            using (var command = Conn.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT :data1, :data2, :data3, :data4, :data5, :data6, :data7, :data8, :data9, :data10";

                for (int i = 0 ; i < 10 ; i++)
                {
                    NpgsqlParameter dataParameter = command.CreateParameter();
                    dataParameter.Direction = ParameterDirection.Input;
                    dataParameter.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Bigint;
                    dataParameter.ParameterName = string.Format("data{0}", i + 1);
                    command.Parameters.Add(dataParameter);
                    dataParameter.Value = 0xFFFFFFFFFFFFFFF;
                }

                if (prepare)
                {
                    command.Prepare();
                }

                using (var metrics = TestMetrics.Start(TestRunTime, true))
                {
                    while (! metrics.TimesUp)
                    {
                        using (IDataReader r = command.ExecuteReader())
                        {
                            r.Read();
                            Int64 r10 = (Int64)r[9];
                        }
                        metrics.IncrementIterations();
                    }
                }
            }
        }

        [Test, Description("A bigint roundtrip test")]
        public void ParameterizedSelectBigIntRoundTrip()
        {
            ParameterizedSelectBigIntRoundTrip_Internal(false);
        }

        [Test, Description("A bigint roundtrip test, prepared")]
        public void ParameterizedSelectBigIntRoundTripPrepared()
        {
            ParameterizedSelectBigIntRoundTrip_Internal(true);
        }

        [Test, Description("A bigint roundtrip test, prepared, binary suppressed")]
        public void ParameterizedSelectBigIntRoundTripPrepared_SuppressBinary()
        {
            if (SuppressBinaryBackendEncoding != null)
            {
                using (SuppressBackendBinary())
                {
                    ParameterizedSelectBigIntRoundTrip_Internal(true);
                }
            }
            else
            {
                Console.WriteLine("Binary suppression not supported in this version of Npgsql; test not executed");
            }
        }

        private void ParameterizedSelectBigIntArrayRoundTrip_Internal(bool prepare)
        {
            using (var command = Conn.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT :data";

                Int64[] data = new Int64[1000];

                for (int i = 0 ; i < 1000 ; i++)
                {
                    data[i] = (Int64)i + 0xFFFFFFFFFFFFFFF;
                }

                NpgsqlParameter dataParameter = command.CreateParameter();
                dataParameter.Direction = ParameterDirection.Input;
                dataParameter.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Bigint | NpgsqlTypes.NpgsqlDbType.Array;
                dataParameter.ParameterName = "data";
                command.Parameters.Add(dataParameter);
                dataParameter.Value = data;

                if (prepare)
                {
                    command.Prepare();
                }

                using (var metrics = TestMetrics.Start(TestRunTime, true))
                {
                    while (! metrics.TimesUp)
                    {
                        command.ExecuteScalar();
                        metrics.IncrementIterations();
                    }
                }
            }
        }

        [Test, Description("A bigint array roundtrip test")]
        public void ParameterizedSelectBigIntArrayRoundTrip()
        {
            ParameterizedSelectBigIntArrayRoundTrip_Internal(false);
        }

        [Test, Description("A bigint array roundtrip test, prepared")]
        public void ParameterizedSelectBigIntArrayRoundTripPrepared()
        {
            ParameterizedSelectBigIntArrayRoundTrip_Internal(true);
        }

        [Test, Description("A bigint array roundtrip test, prepared, binary suppressed")]
        public void ParameterizedSelectBigIntArrayRoundTripPrepared_SuppressBinary()
        {
            if (SuppressBinaryBackendEncoding != null)
            {
                using (SuppressBackendBinary())
                {
                    ParameterizedSelectBigIntArrayRoundTrip_Internal(true);
                }
            }
            else
            {
                Console.WriteLine("Binary suppression not supported in this version of Npgsql; test not executed");
            }
        }

        private void ParameterizedSelectTextArrayRoundTrip_Internal(bool prepare)
        {
            using (var command = Conn.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT :data";

                string[] data = new string[1000];

                for (int i = 0 ; i < 1000 ; i++)
                {
                    data[i] = string.Format("A string with the number {0}, a ', a \", and a \\.", i);
                }

                NpgsqlParameter dataParameter = command.CreateParameter();
                dataParameter.Direction = ParameterDirection.Input;
                dataParameter.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Text | NpgsqlTypes.NpgsqlDbType.Array;
                dataParameter.ParameterName = "data";
                command.Parameters.Add(dataParameter);
                dataParameter.Value = data;

                if (prepare)
                {
                    command.Prepare();
                }

                using (var metrics = TestMetrics.Start(TestRunTime, true))
                {
                    while (! metrics.TimesUp)
                    {
                        try
                        {
                            command.ExecuteScalar();
                        }
                        catch (NpgsqlException e)
                        {
                            if (e.Message.Length > 500)
                            {
                                Console.WriteLine(string.Format("Error: {0}", e.Message.Substring(0, 100)));
                                throw new Exception(e.Message.Substring(0, 500));
                            }
                            else
                            {
                                throw;
                            }
                        }
                        metrics.IncrementIterations();
                    }
                }
            }
        }

        [Test, Description("A text array roundtrip test")]
        public void ParameterizedSelectTextArrayRoundTrip()
        {
            ParameterizedSelectTextArrayRoundTrip_Internal(false);
        }

        [Test, Description("A text array roundtrip test, prepared")]
        public void ParameterizedSelectTextArrayRoundTripPrepared()
        {
            ParameterizedSelectTextArrayRoundTrip_Internal(true);
        }

        [Test, Description("A text array roundtrip test, prepared, binary suppressed")]
        public void ParameterizedSelectTextArrayRoundTripPrepared_SuppressBinary()
        {
            if (SuppressBinaryBackendEncoding != null)
            {
                using (SuppressBackendBinary())
                {
                    ParameterizedSelectTextArrayRoundTrip_Internal(true);
                }
            }
            else
            {
                Console.WriteLine("Binary suppression not supported in this version of Npgsql; test not executed");
            }
        }

        private void ParameterizedSelectByteaArrayRoundTrip_Internal(bool prepare)
        {
            using (var command = Conn.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT :data";

                byte[] bytes = new byte[50000];
                for (int i = 0  ; i < bytes.Length ; i++)
                {
                    bytes[i] = (byte)(i % 255);
                }
                byte[][] data = new byte[][] { bytes, bytes };

                NpgsqlParameter dataParameter = command.CreateParameter();
                dataParameter.Direction = ParameterDirection.Input;
                dataParameter.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Bytea | NpgsqlTypes.NpgsqlDbType.Array;
                dataParameter.ParameterName = "data";
                command.Parameters.Add(dataParameter);
                dataParameter.Value = data;

                if (prepare)
                {
                    command.Prepare();
                }

                using (var metrics = TestMetrics.Start(TestRunTime, true))
                {
                    while (! metrics.TimesUp)
                    {
                        try
                        {
                            command.ExecuteScalar();
                        }
                        catch (NpgsqlException e)
                        {
                            if (e.Message.Length > 500)
                            {
                                Console.WriteLine(string.Format("Error: {0}", e.Message.Substring(0, 100)));
                                throw new Exception(e.Message.Substring(0, 500));
                            }
                            else
                            {
                                throw;
                            }
                        }
                        metrics.IncrementIterations();
                    }
                }
            }
        }

        [Test, Description("A bytea array roundtrip test")]
        public void ParameterizedSelectByteaArrayRoundTrip()
        {
            ParameterizedSelectByteaArrayRoundTrip_Internal(false);
        }

        [Test, Description("A bytea array roundtrip test, prepared")]
        public void ParameterizedSelectByteaArrayRoundTripPrepared()
        {
            ParameterizedSelectByteaArrayRoundTrip_Internal(true);
        }

        [Test, Description("A bytea array roundtrip test, prepared, binary suppressed")]
        public void ParameterizedSelectByteaArrayRoundTripPrepared_SuppressBinary()
        {
            if (SuppressBinaryBackendEncoding != null)
            {
                using (SuppressBackendBinary())
                {
                    ParameterizedSelectByteaArrayRoundTrip_Internal(true);
                }
            }
            else
            {
                Console.WriteLine("Binary suppression not supported in this version of Npgsql; test not executed");
            }
        }

        private void ParameterizedSelectDecimalArrayRoundTrip_Internal(bool prepare)
        {
            using (var command = Conn.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT :data";

                decimal[] data = new decimal[1000];

                for (int i = 0 ; i < 1000 ; i++)
                {
                    data[i] = i;
                }

                NpgsqlParameter dataParameter = command.CreateParameter();
                dataParameter.Direction = ParameterDirection.Input;
                dataParameter.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Numeric | NpgsqlTypes.NpgsqlDbType.Array;
                dataParameter.ParameterName = "data";
                command.Parameters.Add(dataParameter);
                dataParameter.Value = data;

                if (prepare)
                {
                    command.Prepare();
                }

                using (var metrics = TestMetrics.Start(TestRunTime, true))
                {
                    while (! metrics.TimesUp)
                    {
                        command.ExecuteScalar();
                        metrics.IncrementIterations();
                    }
                }
            }
        }

        [Test, Description("A decimal array roundtrip test")]
        public void ParameterizedSelectDecimalArrayRoundTrip()
        {
            ParameterizedSelectDecimalArrayRoundTrip_Internal(false);
        }

        [Test, Description("A timestamp array roundtrip test, prepared")]
        public void ParameterizedSelectDecimalArrayRoundTripPrepared()
        {
            ParameterizedSelectDecimalArrayRoundTrip_Internal(true);
        }

        private void ParameterizedSelectMoneyArrayRoundTrip_Internal(bool prepare)
        {
            using (var command = Conn.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT :data";

                decimal[] data = new decimal[1000];

                for (int i = 0 ; i < 1000 ; i++)
                {
                    data[i] = i;
                }

                NpgsqlParameter dataParameter = command.CreateParameter();
                dataParameter.Direction = ParameterDirection.Input;
                dataParameter.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Money | NpgsqlTypes.NpgsqlDbType.Array;
                dataParameter.ParameterName = "data";
                command.Parameters.Add(dataParameter);
                dataParameter.Value = data;

                if (prepare)
                {
                    command.Prepare();
                }

                using (var metrics = TestMetrics.Start(TestRunTime, true))
                {
                    while (! metrics.TimesUp)
                    {
                        command.ExecuteScalar();
                        metrics.IncrementIterations();
                    }
                }
            }
        }

        [Test, Description("A money array roundtrip test")]
        public void ParameterizedSelectMoneyArrayRoundTrip()
        {
            ParameterizedSelectMoneyArrayRoundTrip_Internal(false);
        }

        [Test, Description("A money array roundtrip test, prepared")]
        public void ParameterizedSelectMoneyArrayRoundTripPrepared()
        {
            ParameterizedSelectMoneyArrayRoundTrip_Internal(true);
        }

        [Test, Description("connect and disconnect with pool")]
        public void ConnectWithPool()
        {
            NpgsqlConnectionStringBuilder csb = new NpgsqlConnectionStringBuilder(Conn.ConnectionString);
            csb.Pooling = true;
            String conStr = csb.ConnectionString;
            using (var metrics = TestMetrics.Start(TestRunTime, true))
            {
                while (!metrics.TimesUp)
                {
                    var con = new NpgsqlConnection(conStr);
                    con.Open();
                    con.Dispose();
                    metrics.IncrementIterations();
                }
            }
        }

        [Test, Description("connect and disconnect without pool")]
        public void ConnectWithoutPool()
        {
            NpgsqlConnectionStringBuilder csb = new NpgsqlConnectionStringBuilder(Conn.ConnectionString);
            csb.Pooling = false;
            String conStr = csb.ConnectionString;
            using (var metrics = TestMetrics.Start(TestRunTime, true))
            {
                while (!metrics.TimesUp)
                {
                    var con = new NpgsqlConnection(conStr);
                    con.Open();
                    con.Dispose();
                    metrics.IncrementIterations();
                }
            }
        }

        [Test, Description("Many parameter substitution test")]
        public void ParameterizedPrepareManyFields()
        {
            using (var command = Conn.CreateCommand())
            {
                StringWriter sql = new StringWriter();

                sql.WriteLine("SELECT");
                sql.WriteLine(":p01, :p02, :p03, :p04, :p05, :p06, :p07, :p08, :p09, :p10,");
                sql.WriteLine(":p11, :p12, :p13, :p14, :p15, :p16, :p17, :p18, :p19, :p20");

                command.CommandText = sql.ToString();

                for (int i = 0 ; i < 20 ; i++)
                {
                    command.Parameters.AddWithValue(string.Format("p{0:00}", i + 1), NpgsqlDbType.Text, string.Format("String parameter value {0}", i + 1));
                }

                using (var metrics = TestMetrics.Start(TestRunTime, true))
                {
                    while (! metrics.TimesUp)
                    {
                        command.Prepare();
                        metrics.IncrementIterations();
                    }
                }
            }
        }

        [Test]
        public void ParameterCollectionGetValueFor1ItemDifferentCase()
        {
            this.PerformanceWithNParameters(1, true);
        }

        [Test]
        public void ParameterCollectionGetValueFor10ItemsDifferentCase()
        {
            this.PerformanceWithNParameters(10, true);
        }

        [Test]
        public void ParameterCollectionGetValueFor100ItemsDifferentCase()
        {
            this.PerformanceWithNParameters(100, true);
        }

        [Test]
        public void ParameterCollectionGetValueFor1000ItemsDifferentCase()
        {
            this.PerformanceWithNParameters(1000, true);
        }

        [Test]
        public void ParameterCollectionGetValueFor1ItemSameCase()
        {
            this.PerformanceWithNParameters(1, false);
        }

        [Test]
        public void ParameterCollectionGetValueFor10ItemsSameCase()
        {
            this.PerformanceWithNParameters(10, false);
        }

        [Test]
        public void ParameterCollectionGetValueFor100ItemsSameCase()
        {
            this.PerformanceWithNParameters(100, false);
        }

        [Test]
        public void ParameterCollectionGetValueFor1000ItemsSameCase()
        {
            this.PerformanceWithNParameters(1000, false);
        }

        private void PerformanceWithNParameters(int n, bool differentCase)
        {
            var command = new NpgsqlCommand();
            var collection = command.Parameters;
            for (int i = 0; i < n; i++)
            {
                collection.AddWithValue("value" + i, i);
            }

            using (var metrics = TestMetrics.Start(TestRunTime, true))
            {
                while (!metrics.TimesUp)
                {
                    for (int i = 0; i < n && !metrics.TimesUp; i++)
                    {
                        NpgsqlParameter param;
                        if (!collection.TryGetValue((differentCase ? "VALUE" : "value") + i, out param))
                        {
                            throw new Exception();
                        }

                        metrics.IncrementIterations();
                    }
                }
            }
        }

        #region Setup / Teardown / Utils

        private Stopwatch _watch;

        [SetUp]
        public void Setup()
        {
            NpgsqlEventLog.Level = LogLevel.None;
            NpgsqlEventLog.EchoMessages = false;
            _watch = new Stopwatch();
            _watch.Start();
        }

        [TearDown]
        public void Teardown()
        {
            _watch.Stop();
            Console.WriteLine("Total test running time: {0}ms",  _watch.ElapsedMilliseconds);
        }

        #endregion
    }
}