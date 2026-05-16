using BenchmarkDotNet.Attributes;
using Microsoft.Data.SqlClient;

// ReSharper disable UnusedMember.Global

namespace Npgsql.Benchmarks;

[OperationsPerSecond]
public class ConnectionCreationBenchmarks
{
    const string NpgsqlConnectionString = "Host=foo;Database=bar;Username=user;Password=password";
    const string SqlClientConnectionString = @"Data Source=(localdb)\mssqllocaldb";

    [Benchmark]
    public NpgsqlConnection Npgsql() => new(NpgsqlConnectionString);

    [Benchmark]
    public SqlConnection SqlClient() => new(SqlClientConnectionString);
}
