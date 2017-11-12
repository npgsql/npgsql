using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace Npgsql.Benchmarks
{
    [MemoryDiagnoser]
    public class ReadRows
    {
        [Params(1, 10, 100, 1000)]
        public int NumRows { get; set; }

        NpgsqlCommand Command { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            var conn = BenchmarkEnvironment.OpenConnection();
            Command = new NpgsqlCommand($"SELECT generate_series(1, {NumRows})", conn);
            Command.Prepare();
        }

        [Benchmark]
        public void Read()
        {
            using (var reader = Command.ExecuteReader())
                while (reader.Read()) { }
        }
    }
}
