using BenchmarkDotNet.Attributes;

namespace Npgsql.Benchmarks;

public class ReadRows
{
    [Params(1, 10, 100, 1000)]
    public int NumRows { get; set; }

    NpgsqlCommandOrig Command { get; set; } = default!;

    [GlobalSetup]
    public void Setup()
    {
        var conn = BenchmarkEnvironment.OpenConnection();
        Command = new NpgsqlCommandOrig($"SELECT generate_series(1, {NumRows})", conn);
        Command.Prepare();
    }

    [Benchmark]
    public void Read()
    {
        using (var reader = Command.ExecuteReader())
            while (reader.Read()) { }
    }
}