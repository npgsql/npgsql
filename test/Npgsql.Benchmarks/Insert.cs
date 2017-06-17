using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using NpgsqlTypes;

namespace Npgsql.Benchmarks
{
    public class Insert
    {
        NpgsqlConnection _conn;
        NpgsqlCommand _truncateCmd;

        [Params(1, 100, 1000, 10000)]
        public int BatchSize { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            var connString = new NpgsqlConnectionStringBuilder(BenchmarkEnvironment.ConnectionString)
            {
                Pooling = false
            }.ToString();
            _conn = new NpgsqlConnection(connString);
            _conn.Open();

            using (var cmd = new NpgsqlCommand("CREATE TEMP TABLE data (int1 INT4, text1 TEXT, int2 INT4, text2 TEXT)", _conn))
                cmd.ExecuteNonQuery();

            _truncateCmd = new NpgsqlCommand("TRUNCATE data", _conn);
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            _conn.Close();
            _conn = null;
        }

        [Benchmark(Baseline = true)]
        public void Unbatched()
        {
            var cmd = new NpgsqlCommand("INSERT INTO data VALUES (@p0, @p1, @p2, @p3)", _conn);
            cmd.Parameters.AddWithValue("p0", NpgsqlDbType.Integer, 8);
            cmd.Parameters.AddWithValue("p1", NpgsqlDbType.Text, "foo");
            cmd.Parameters.AddWithValue("p2", NpgsqlDbType.Integer, 9);
            cmd.Parameters.AddWithValue("p3", NpgsqlDbType.Text, "bar");
            cmd.Prepare();

            for (var i = 0; i < BatchSize; i++)
                cmd.ExecuteNonQuery();
            _truncateCmd.ExecuteNonQuery();
        }

        [Benchmark]
        public void Batched()
        {
            var cmd = new NpgsqlCommand { Connection = _conn };
            var sb = new StringBuilder();
            for (var i = 0; i < BatchSize; i++)
            {
                var p1 = (i * 4).ToString();
                var p2 = (i * 4 + 1).ToString();
                var p3 = (i * 4 + 2).ToString();
                var p4 = (i * 4 + 3).ToString();
                sb.Append("INSERT INTO data VALUES (@").Append(p1).Append(", @").Append(p2).Append(", @").Append(p3).Append(", @").Append(p4).Append(");");
                cmd.Parameters.AddWithValue(p1, NpgsqlDbType.Integer, 8);
                cmd.Parameters.AddWithValue(p2, NpgsqlDbType.Text, "foo");
                cmd.Parameters.AddWithValue(p3, NpgsqlDbType.Integer, 9);
                cmd.Parameters.AddWithValue(p4, NpgsqlDbType.Text, "bar");
            }
            cmd.CommandText = sb.ToString();
            cmd.Prepare();
            cmd.ExecuteNonQuery();
            _truncateCmd.ExecuteNonQuery();
        }

        [Benchmark]
        public void Copy()
        {
            using (var s = _conn.BeginBinaryImport("COPY data (int1, text1, int2, text2) FROM STDIN BINARY"))
            {
                for (var i = 0; i < BatchSize; i++)
                {
                    s.StartRow();
                    s.Write(8);
                    s.Write("foo");
                    s.Write(9);
                    s.Write("bar");
                }
            }
            _truncateCmd.ExecuteNonQuery();
        }
    }
}
