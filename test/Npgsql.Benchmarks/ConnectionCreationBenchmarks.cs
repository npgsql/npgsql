using System.Data.SqlClient;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;

// ReSharper disable UnusedMember.Global

namespace Npgsql.Benchmarks
{
    [Config(typeof(Config))]
    public class ConnectionCreationBenchmarks
    {
        const string NpgsqlConnectionString = "Host=foo;Database=bar;Username=user;Password=password";
        const string SqlClientConnectionString = @"Data Source=(localdb)\mssqllocaldb";

        [Benchmark]
        public NpgsqlConnection Npgsql() => new NpgsqlConnection(NpgsqlConnectionString);

        [Benchmark]
        public SqlConnection SqlClient() => new SqlConnection(SqlClientConnectionString);

        class Config : ManualConfig
        {
            public Config()
            {
                Add(StatisticColumn.OperationsPerSecond);
            }
        }
    }
}
