using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using NpgsqlTypes;

namespace Npgsql.Benchmarks
{
    public class CopyExport
    {
        NpgsqlConnection _conn;
        const int Rows = 1000;

        [GlobalSetup]
        public void Setup()
        {
            _conn = BenchmarkEnvironment.OpenConnection();
            using (var cmd = new NpgsqlCommand("CREATE TEMP TABLE data (i1 INT, i2 INT, i3 INT, i4 INT, i5 INT, i6 INT, i7 INT, i8 INT, i9 INT, i10 INT)", _conn))
                cmd.ExecuteNonQuery();

            using (var cmd = new NpgsqlCommand("INSERT INTO data VALUES (1, 2, 3, 4, 5, 6, 7, 8, 9, 10)", _conn))
                for (var i = 0; i < Rows; i++)
                    cmd.ExecuteNonQuery();
        }

        [GlobalCleanup]
        public void Cleanup() => _conn.Dispose();

        [Benchmark]
        public int Export()
        {
            var sum = 0;
            unchecked
            {
                using (var exporter = _conn.BeginBinaryExport("COPY data TO STDOUT (FORMAT BINARY)"))
                    while (exporter.StartRow() != -1)
                        for (var col = 0; col < 10; col++)
                            sum += exporter.Read<int>(NpgsqlDbType.Integer);
            }
            return sum;
        }
    }
}
