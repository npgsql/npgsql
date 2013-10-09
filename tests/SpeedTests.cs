using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Npgsql;

namespace NpgsqlTests
{
    [Explicit]
    public class SpeedTests : TestBase
    {
        private static readonly TimeSpan TestRunTime = new TimeSpan(0, 0, 10); // 10 seconds

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

                var sw = Stopwatch.StartNew();
                int count = 0;
                while (sw.Elapsed < TestRunTime)
                {
                    dataParameter.Value = "yo";
                    command.ExecuteScalar();
                    count++;
                }
                sw.Stop();
                Console.WriteLine("Elapsed: {0}, Queries: {1}; {2:0.00}/second", sw.Elapsed, count, (double)count / ((double)sw.Elapsed.TotalMilliseconds / (double)1000));
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

                var sw = Stopwatch.StartNew();
                int count = 0;
                while (sw.Elapsed < TestRunTime)
                {
                    dataParameter.Value = "yo";
                    command.ExecuteScalar();
                    count++;
                }
                sw.Stop();
                Console.WriteLine("Elapsed: {0}, Queries: {1}; {2:0.00}/second", sw.Elapsed, count, (double)count / ((double)sw.Elapsed.TotalMilliseconds / (double)1000));
            }
        }

        private void ParameterizedSelectByteaRoundTripPrepared_Internal(bool prepare)
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

                var sw = Stopwatch.StartNew();
                int count = 0;
                while (sw.Elapsed < TestRunTime)
                {
                    var data2 = (byte[])command.ExecuteScalar();
                    count++;
                }
                sw.Stop();
                Console.WriteLine("Elapsed: {0}, Queries: {1}; {2:0.00}/second", sw.Elapsed, count, (double)count / ((double)sw.Elapsed.TotalMilliseconds / (double)1000));
            }
        }

        [Test, Description("A large bytea roundtrip test")]
        public void ParameterizedSelectByteaRoundTrip()
        {
            ParameterizedSelectByteaRoundTripPrepared_Internal(false);
        }

        [Test, Description("A large bytea roundtrip test, prepared")]
        public void ParameterizedSelectByteaRoundTripPrepared()
        {
            ParameterizedSelectByteaRoundTripPrepared_Internal(true);
        }

        [Test, Description("A large bytea roundtrip test, prepared, binary suppressed")]
        public void ParameterizedSelectByteaRoundTripPrepared_SuppressBinary()
        {
            using (SuppressBackendBinary())
            {
                ParameterizedSelectByteaRoundTripPrepared_Internal(true);
            }
        }

        private void ParameterizedSelectBigIntRoundTripPrepared_Internal(bool prepare)
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
                var sw = Stopwatch.StartNew();
                int count = 0;
                while (sw.Elapsed < TestRunTime)
                {
                    using (IDataReader r = command.ExecuteReader())
                    {
                        r.Read();
                        Int64 r10 = (Int64)r[9];
                    }
                    count++;
                }
                sw.Stop();
                Console.WriteLine("Elapsed: {0}, Queries: {1}; {2:0.00}/second", sw.Elapsed, count, (double)count / ((double)sw.Elapsed.TotalMilliseconds / (double)1000));
            }
        }

        [Test, Description("A bigint roundtrip test")]
        public void ParameterizedSelectBigIntRoundTrip()
        {
            ParameterizedSelectBigIntRoundTripPrepared_Internal(false);
        }

        [Test, Description("A bigint roundtrip test, prepared")]
        public void ParameterizedSelectBigIntRoundTripPrepared()
        {
            ParameterizedSelectBigIntRoundTripPrepared_Internal(true);
        }

        [Test, Description("A bigint roundtrip test, prepared, binary suppressed")]
        public void ParameterizedSelectBigIntRoundTripPrepared_SuppressBinary()
        {
            using (SuppressBackendBinary())
            {
                ParameterizedSelectBigIntRoundTripPrepared_Internal(true);
            }
        }

        private void ParameterizedSelectBigIntArrayRoundTripPrepared_Internal(bool prepare)
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
                var sw = Stopwatch.StartNew();
                int count = 0;
                while (sw.Elapsed < TestRunTime)
                {
                    command.ExecuteScalar();
                    count++;
                }
                sw.Stop();
                Console.WriteLine("Elapsed: {0}, Queries: {1}; {2:0.00}/second", sw.Elapsed, count, (double)count / ((double)sw.Elapsed.TotalMilliseconds / (double)1000));
            }
        }

        [Test, Description("A bigint array roundtrip test")]
        public void ParameterizedSelectBigIntArrayRoundTrip()
        {
            ParameterizedSelectBigIntArrayRoundTripPrepared_Internal(false);
        }

        [Test, Description("A bigint array roundtrip test, prepared")]
        public void ParameterizedSelectBigIntArrayRoundTripPrepared()
        {
            ParameterizedSelectBigIntArrayRoundTripPrepared_Internal(true);
        }

        [Test, Description("A bigint array roundtrip test, prepared, binary suppressed")]
        public void ParameterizedSelectBigIntArrayRoundTripPrepared_SuppressBinary()
        {
            using (SuppressBackendBinary())
            {
                ParameterizedSelectBigIntArrayRoundTripPrepared_Internal(true);
            }
        }

        private void ParameterizedSelectTextArrayRoundTripPrepared_Internal(bool prepare)
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
                var sw = Stopwatch.StartNew();
                int count = 0;
                while (sw.Elapsed < TestRunTime)
                {
                    command.ExecuteScalar();
                    count++;
                }
                sw.Stop();
                Console.WriteLine("Elapsed: {0}, Queries: {1}; {2:0.00}/second", sw.Elapsed, count, (double)count / ((double)sw.Elapsed.TotalMilliseconds / (double)1000));
            }
        }

        [Test, Description("A text array roundtrip test")]
        public void ParameterizedSelectTextArrayRoundTrip()
        {
            ParameterizedSelectTextArrayRoundTripPrepared_Internal(false);
        }

        [Test, Description("A text array roundtrip test, prepared")]
        public void ParameterizedSelectTextArrayRoundTripPrepared()
        {
            ParameterizedSelectTextArrayRoundTripPrepared_Internal(true);
        }

        [Test, Description("A text array roundtrip test, prepared, binary suppressed")]
        public void ParameterizedSelectTextArrayRoundTripPrepared_SuppressBinary()
        {
            using (SuppressBackendBinary())
            {
                ParameterizedSelectTextArrayRoundTripPrepared_Internal(true);
            }
        }

        #region Setup / Teardown / Utils

        private Stopwatch _watch;

        [SetUp]
        public void Setup()
        {
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