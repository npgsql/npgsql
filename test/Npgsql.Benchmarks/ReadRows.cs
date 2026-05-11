using BenchmarkDotNet.Attributes;

namespace Npgsql.Benchmarks;

public class ReadRows
{
    [Params(1, 10, 100, 1000)]
    public int NumRows { get; set; }

    NpgsqlConnection _conn = default!;
    NpgsqlCommand Command { get; set; } = default!;

    [GlobalSetup]
    public void Setup()
    {
        _conn = BenchmarkEnvironment.OpenConnection();
        Command = new NpgsqlCommand($"SELECT generate_series(1, {NumRows})", _conn);
        Command.Prepare();
    }

    [GlobalCleanup]
    public void Cleanup() => _conn.Dispose();

    [Benchmark]
    public void Read()
    {
        using (var reader = Command.ExecuteReader())
            while (reader.Read()) { }
    }
}