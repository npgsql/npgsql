using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using Common.Logging;
using Common.Logging.Configuration;
using Common.Logging.Simple;
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

#if NET45
        [Test, Description("A minimal, simple, non-query scenario in async")]
        public async void ExecuteUpdateNonQueryAsync()
        {
            using (var metrics = TestMetrics.Start(TestRunTime, true))
            {
                while (!metrics.TimesUp)
                {
                    await ExecuteNonQueryAsync("set lock_timeout = 1000");
                    metrics.IncrementIterations();
                }
            }
        }
#endif

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

#if NET45
        [Test, Description("A minimal, simple, scalar scenario in async")]
        public async void ExecuteScalarAsync()
        {
            using (var metrics = TestMetrics.Start(TestRunTime, true))
            {
                while (!metrics.TimesUp)
                {
                    await ExecuteScalarAsync("SELECT 1 + 1");
                    metrics.IncrementIterations();
                }
            }
        }
#endif

        [Test, Description("A minimal, simple reader scenario")]
        [TestCase(100)]
        public void ExecuteReader(int rows)
        {
            for (var i = 0; i < rows; i++)
                ExecuteNonQuery("INSERT INTO DATA (field_int4) VALUES (10)");
            using (var metrics = TestMetrics.Start(TestRunTime, true))
            {
                while (!metrics.TimesUp)
                {
                    using (var cmd = new NpgsqlCommand("SELECT field_int4 FROM data", Conn))
                    using (var reader = cmd.ExecuteReader()) {
                        while (reader.Read()) {}
                    }
                    metrics.IncrementIterations();
                }
            }
        }

#if NET45
        [Test, Description("A minimal, simple reader scenario in async")]
        [TestCase(100)]
        public async void ExecuteReaderAsync(int rows)
        {
            for (var i = 0; i < rows; i++)
                ExecuteNonQuery("INSERT INTO DATA (field_int4) VALUES (10)");
            using (var metrics = TestMetrics.Start(TestRunTime, true))
            {
                while (!metrics.TimesUp)
                {
                    using (var cmd = new NpgsqlCommand("SELECT field_int4 FROM data", Conn))
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync()) { }
                    }
                    metrics.IncrementIterations();
                }
            }
        }
#endif

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

        #region Parameterized selects on various types

        [Test, Description("A single decimal roundtrip test")]
        public void ParameterizedSelectDecimalRoundTrip()
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
                        command.Prepare();

                        var data2 = (decimal)command.ExecuteScalar();
                        metrics.IncrementIterations();
                    }
                }
            }
        }

        [Test, Description("A large bytea roundtrip test")]
        public void ParameterizedSelectByteaRoundTrip()
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

                command.Prepare();

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

        [Test, Description("A bigint roundtrip test")]
        public void ParameterizedSelectBigIntRoundTrip()
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

                command.Prepare();

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

        [Test, Description("A bigint array roundtrip test")]
        public void ParameterizedSelectBigIntArrayRoundTrip()
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

                command.Prepare();

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

        [Test, Description("A text array roundtrip test")]
        public void ParameterizedSelectArrayRoundTrip()
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

                command.Prepare();

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

                command.Prepare();

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

        [Test, Description("A decimal array roundtrip test")]
        public void ParameterizedSelectDecimalArrayRoundTrip()
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

                command.Prepare();

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

                command.Prepare();

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

        #endregion

        [Test, Description("connect and disconnect, with and without pool")]
        [TestCase(true), TestCase(false)]
        public void ConnectWithPool(bool withPool)
        {
            NpgsqlConnectionStringBuilder csb = new NpgsqlConnectionStringBuilder(Conn.ConnectionString);
            csb.Pooling = withPool;
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
        [TestCase(1,    true)]
        [TestCase(10,   true)]
        [TestCase(100,  true)]
        [TestCase(1000, true)]
        [TestCase(1,    false)]
        [TestCase(10,   false)]
        [TestCase(100,  false)]
        [TestCase(1000, false)]
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

        protected override void SetupLogging()
        {
            // Disable logging because it impacts performance
            LogManager.Adapter = new NoOpLoggerFactoryAdapter();
        }

        #endregion
    }
}