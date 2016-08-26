using BenchmarkDotNet.Attributes;
// ReSharper disable UnusedMember.Global

namespace Npgsql.Benchmarks
{
    public class ConnectionCreationBenchmarks
    {
        static readonly NpgsqlConnectionStringBuilder ConnectionStringBuilder = new NpgsqlConnectionStringBuilder(ConnectionString);
        const string ConnectionString = "Host=foo;Database=bar;Username=user;Password=password";

        [Benchmark]
        public NpgsqlConnection CreateWithoutConnectionString() => new NpgsqlConnection(ConnectionStringBuilder);

        [Benchmark]
        public NpgsqlConnection CreateWithConnectionString() => new NpgsqlConnection(ConnectionString);
    }
}
