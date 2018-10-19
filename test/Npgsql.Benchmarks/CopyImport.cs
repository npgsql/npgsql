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
    [Config("columns=OperationPerSecond")]
    public class CopyImport
    {
        NpgsqlConnection _conn;
        NpgsqlCommand _truncateCmd;
        const int Rows = 1000;

        [GlobalSetup]
        public void Setup()
        {
            _conn = BenchmarkEnvironment.OpenConnection();
            using (var cmd = new NpgsqlCommand("CREATE TEMP TABLE data (i1 INT, i2 INT, i3 INT, i4 INT, i5 INT, i6 INT, i7 INT, i8 INT, i9 INT, i10 INT)", _conn))
                cmd.ExecuteNonQuery();

            _truncateCmd = new NpgsqlCommand("TRUNCATE data", _conn);
            _truncateCmd.Prepare();
        }

        [GlobalCleanup]
        public void Cleanup() => _conn.Dispose();

        [IterationCleanup]
        public void IterationCleanup() => _truncateCmd.ExecuteNonQuery();

        [Benchmark]
        public void Import()
        {
            using (var importer = _conn.BeginBinaryImport("COPY data FROM STDIN (FORMAT BINARY)"))
            {
                for (var row = 0; row < Rows; row++)
                {
                    importer.StartRow();
                    for (var col = 0; col < 10; col++)
                        importer.Write(col, NpgsqlDbType.Integer);
                }
            }
        }
    }
}
