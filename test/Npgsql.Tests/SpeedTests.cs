#region License
// The PostgreSQL License
//
// Copyright (C) 2015 The Npgsql Development Team
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
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;

using Npgsql;
using Npgsql.Logging;
using NpgsqlTypes;

namespace Npgsql.Tests
{
    [Explicit]
    public class SpeedTests : TestBase
    {
        public SpeedTests(string backendVersion) : base(backendVersion) { }

        private static readonly TimeSpan TestRunTime = new TimeSpan(0, 0, 10); // 10 seconds

        [Test, Description("A minimal, simple, non-query scenario")]
        public void ExecuteUpdateNonQuery()
        {
            using (var conn = OpenConnection())
            using (var metrics = TestMetrics.Start(TestRunTime, true))
            {
                while (!metrics.TimesUp)
                {
                    conn.ExecuteNonQuery("set lock_timeout = 1000");
                    metrics.IncrementIterations();
                }
            }
        }

        [Test, Description("A minimal, simple, non-query scenario in async")]
        public async void ExecuteUpdateNonQueryAsync()
        {
            using (var conn = OpenConnection())
            using (var metrics = TestMetrics.Start(TestRunTime, true))
            {
                while (!metrics.TimesUp)
                {
                    await conn.ExecuteNonQueryAsync("set lock_timeout = 1000");
                    metrics.IncrementIterations();
                }
            }
        }

        [Test, Description("A minimal, simple, scalar scenario")]
        public void ExecuteScalar()
        {
            using (var conn = OpenConnection())
            using (var metrics = TestMetrics.Start(TestRunTime, true))
            {
                while (!metrics.TimesUp)
                {
                    conn.ExecuteScalar("SELECT 1 + 1");
                    metrics.IncrementIterations();
                }
            }
        }

        [Test, Description("A minimal, simple, scalar scenario in async")]
        public async void ExecuteScalarAsync()
        {
            using (var conn = OpenConnection())
            using (var metrics = TestMetrics.Start(TestRunTime, true))
            {
                while (!metrics.TimesUp)
                {
                    await conn.ExecuteScalarAsync("SELECT 1 + 1");
                    metrics.IncrementIterations();
                }
            }
        }

        [Test, Description("A minimal, simple reader scenario")]
        [TestCase(100)]
        public void ExecuteReader(int rows)
        {
            using (var conn = OpenConnection())
            {
                for (var i = 0; i < rows; i++)
                    conn.ExecuteNonQuery("INSERT INTO DATA (field_int4) VALUES (10)");
                using (var metrics = TestMetrics.Start(TestRunTime, true))
                {
                    while (!metrics.TimesUp)
                    {
                        using (var cmd = new NpgsqlCommand("SELECT field_int4 FROM data", conn))
                        using (var reader = cmd.ExecuteReader())
                            while (reader.Read()) {}
                        metrics.IncrementIterations();
                    }
                }
            }
        }

        [Test, Description("A minimal, simple reader scenario in async")]
        [TestCase(100)]
        public async void ExecuteReaderAsync(int rows)
        {
            using (var conn = OpenConnection())
            {
                for (var i = 0; i < rows; i++)
                    conn.ExecuteNonQuery("INSERT INTO DATA (field_int4) VALUES (10)");
                using (var metrics = TestMetrics.Start(TestRunTime, true))
                {
                    while (!metrics.TimesUp)
                    {
                        using (var cmd = new NpgsqlCommand("SELECT field_int4 FROM data", conn))
                        using (var reader = await cmd.ExecuteReaderAsync())
                            while (await reader.ReadAsync()) { }
                        metrics.IncrementIterations();
                    }
                }
            }
        }

        [Test, Description("A normal insert command with one parameter")]
        public void ParameterizedInsert()
        {
            using (var conn = OpenConnection())
            using (var command = conn.CreateCommand())
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
            using (var conn = OpenConnection())
            using (var command = conn.CreateCommand())
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
            using (var conn = OpenConnection())
            using (var command = conn.CreateCommand())
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
            using (var conn = OpenConnection())
            using (var command = conn.CreateCommand())
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
            using (var conn = OpenConnection())
            using (var command = conn.CreateCommand())
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
            using (var conn = OpenConnection())
            using (var command = conn.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT :data";

                string[] data = new string[1000];

                for (int i = 0 ; i < 1000 ; i++)
                {
                    data[i] = $"A string with the number {i}, a ', a \", and a \\.";
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
            using (var conn = OpenConnection())
            using (var command = conn.CreateCommand())
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
            using (var conn = OpenConnection())
            using (var command = conn.CreateCommand())
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
            using (var conn = OpenConnection())
            using (var command = conn.CreateCommand())
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
            NpgsqlConnectionStringBuilder csb = new NpgsqlConnectionStringBuilder(ConnectionString);
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
            using (var conn = OpenConnection())
            using (var command = conn.CreateCommand())
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

        [Test]
        public void PoolTest()
        {
            var maxNumThreads = 8;
            var numIterations = 100000;
            var maxPoolSize = 4;

            Console.WriteLine($"Testing {numIterations} across up to {maxNumThreads} threads with max pool size {maxPoolSize}");
            for (var i = 1; i <= maxNumThreads; i++)
            {
                var iterations = numIterations / i;
                Console.WriteLine($"{i} threads ({iterations} iterations): {PoolTestCase(i, iterations, maxPoolSize)}");
            }
        }

        TimeSpan PoolTestCase(int numThreads, int numIterations, int maxPoolSize)
        {
            var connString = new NpgsqlConnectionStringBuilder(ConnectionString) {
                 MaxPoolSize = maxPoolSize,
                 Host = "mammoth",
                 Port=5432
            };

            //connString = new NpgsqlConnectionStringBuilder("Host=mammoth;Port=5432;Database=npgsql_tests;Username=npgsql_tests;Port=npgsql_tests;MaxPoolSize=" + maxPoolSize);
            Func<TimeSpan> testMethod = () =>
            {
                var openWatch = new Stopwatch();
                for (var i = 0; i < numIterations; i++)
                {
                    openWatch.Start();
                    var conn = new NpgsqlConnection("Host=mammoth;Port=5432;Database=npgsql_tests;Username=npgsql_tests;Password=npgsql_tests;MaxPoolSize=" + maxPoolSize);
                    openWatch.Stop();
                    conn.Close();
                }
                return openWatch.Elapsed;
            };

            var tasks = Enumerable.Range(0, numThreads).Select(i => Task.Run(testMethod)).ToArray();
            Task.WaitAll(tasks);

            return new TimeSpan(tasks.Sum(t => t.Result.Ticks));
        }

        #region Setup / Teardown / Utils

        protected override void SetupLogging()
        {
            // Disable logging because it impacts performance
            NpgsqlLogManager.Provider = new NoOpLoggingProvider();
        }

        #endregion
    }
}
