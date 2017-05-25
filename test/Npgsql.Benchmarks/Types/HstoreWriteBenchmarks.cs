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
    public class HstoreWriteBenchmarks
    {
        [Params(0, 100, 1000, 10000)]
        public int DictSize { get; set; }

        readonly NpgsqlCommand _cmd;

        public HstoreWriteBenchmarks()
        {
            var conn = BenchmarkEnvironment.OpenConnection();

            using (var cmd = new NpgsqlCommand("CREATE TEMP TABLE foo (bar hstore)", conn))
                cmd.ExecuteNonQuery();

            _cmd = new NpgsqlCommand("INSERT INTO foo (bar) VALUES (@p)", conn);
            _cmd.Parameters.Add(new NpgsqlParameter("p", NpgsqlDbType.Hstore));
        }

        [Setup]
        public void Setup()
        {
            var d = new Dictionary<string, string>(DictSize);
            for (int i = 0; i < DictSize; i++)
                d.Add($"K{i}", $"V{i}");
            _cmd.Parameters.Single().Value = d;
        }

        [Benchmark]
        public void WriteHstore()
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
