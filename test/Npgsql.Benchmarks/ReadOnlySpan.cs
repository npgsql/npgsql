using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using NpgsqlTypes;

namespace Npgsql.Benchmarks
{
    [MemoryDiagnoser]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    public class ReadOnlySpan
    {
        const string Empty = "empty";
        const string IntRange = "[0,1]";
        const string DoubleRange = "[0.0,1.1]";
        const string DateRange = "[2018-06-03,2018-06-04]";

        [Benchmark(Baseline = true)]
        [BenchmarkCategory(nameof(Int32))]
        public NpgsqlRange<int> ParseEmptyIntRange() => NpgsqlRange<int>.Parse(Empty);

        [Benchmark]
        [BenchmarkCategory(nameof(Int32))]
        public NpgsqlRange<int> ParseIntRange() => NpgsqlRange<int>.Parse(IntRange);

        [Benchmark(Baseline = true)]
        [BenchmarkCategory(nameof(Double))]
        public NpgsqlRange<double> ParseEmptyDoubleRange() => NpgsqlRange<double>.Parse(Empty);

        [Benchmark]
        [BenchmarkCategory(nameof(Double))]
        public NpgsqlRange<double> ParseDoubleRange() => NpgsqlRange<double>.Parse(DoubleRange);

        [Benchmark(Baseline = true)]
        [BenchmarkCategory(nameof(DateTime))]
        public NpgsqlRange<DateTime> ParseEmptyDateRange() => NpgsqlRange<DateTime>.Parse(Empty);

        [Benchmark]
        [BenchmarkCategory(nameof(DateTime))]
        public NpgsqlRange<DateTime> ParseDateRange() => NpgsqlRange<DateTime>.Parse(DateRange);
    }
}
