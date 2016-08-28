using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnostics.Windows;

// ReSharper disable UnusedMember.Global

namespace Npgsql.Benchmarks
{
    [Config(typeof(Config))]
    public class ConnectionCreationBenchmarks
    {
        static readonly NpgsqlConnectionStringBuilder ConnectionStringBuilder = new NpgsqlConnectionStringBuilder(ConnectionString);
        const string ConnectionString = "Host=foo;Database=bar;Username=user;Password=password";

        [Benchmark]
        public NpgsqlConnection CreateWithoutConnectionString() => new NpgsqlConnection(ConnectionStringBuilder);

        [Benchmark]
        public NpgsqlConnection CreateWithConnectionString() => new NpgsqlConnection(ConnectionString);

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
