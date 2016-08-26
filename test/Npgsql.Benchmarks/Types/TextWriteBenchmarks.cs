using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
#if NET46
using BenchmarkDotNet.Diagnostics.Windows;
#endif
using NpgsqlTypes;

namespace Npgsql.Benchmarks.Types
{
    [Config(typeof(Config))]
    public class TextWriteBenchmarks
    {
        [Params(0, 100, 1000, 10000)]
        public int TextSize { get; set; }

        readonly NpgsqlCommand _cmd;

        public TextWriteBenchmarks()
        {
            var conn = BenchmarkEnvironment.OpenConnection();

            using (var cmd = new NpgsqlCommand("CREATE TEMP TABLE foo (bar TEXT)", conn))
                cmd.ExecuteNonQuery();

            _cmd = new NpgsqlCommand("INSERT INTO foo (bar) VALUES (@p)", conn);
            _cmd.Parameters.Add(new NpgsqlParameter("p", NpgsqlDbType.Text));
        }

        [Setup]
        public void Setup()
        {
            _cmd.Parameters.Single().Value = new string('x', TextSize);
        }

        [Benchmark]
        public void WriteText()
        {
            _cmd.ExecuteNonQuery();
        }

        class Config : ManualConfig
        {
            public Config()
            {
#if NET46
                Add(new MemoryDiagnoser());
#endif
                Add(StatisticColumn.OperationsPerSecond);
            }
        }
    }
}
