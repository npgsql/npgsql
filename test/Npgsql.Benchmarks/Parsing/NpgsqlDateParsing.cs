using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using NpgsqlTypes;

namespace Npgsql.Benchmarks.Parsing
{
    [MemoryDiagnoser]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    public class NpgsqlDateParsing
    {
        const string Infinity = "infinity";
        const string Date = "2018-06-03";

        [Benchmark(Baseline = true)]
        [BenchmarkCategory(nameof(Date))]
        public NpgsqlDate ParseInfiniteDate() => NpgsqlDate.Parse(Infinity);

        [Benchmark]
        [BenchmarkCategory(nameof(Date))]
        public NpgsqlDate ParseDate() => NpgsqlDate.Parse(Date);
    }
}
