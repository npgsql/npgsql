using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using NpgsqlTypes;

namespace Npgsql.Benchmarks.Parsing
{
    [MemoryDiagnoser]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    public class NpgsqlRangeParsing
    {
        const string Empty = "empty";
        const string IntRange = "[0,1]";
        const string DoubleRange = "[0.0,1.1]";
        const string DateRange = "[2018-06-03,2018-06-04]";

        [Benchmark(Baseline = true)]
        [BenchmarkCategory(nameof(IntRange))]
        public NpgsqlRange<int> ParseEmptyIntRange() => NpgsqlRange<int>.Parse(Empty);

        [Benchmark]
        [BenchmarkCategory(nameof(IntRange))]
        public NpgsqlRange<int> ParseIntRange() => NpgsqlRange<int>.Parse(IntRange);

        [Benchmark(Baseline = true)]
        [BenchmarkCategory(nameof(DoubleRange))]
        public NpgsqlRange<double> ParseEmptyDoubleRange() => NpgsqlRange<double>.Parse(Empty);

        [Benchmark]
        [BenchmarkCategory(nameof(DoubleRange))]
        public NpgsqlRange<double> ParseDoubleRange() => NpgsqlRange<double>.Parse(DoubleRange);

        [Benchmark(Baseline = true)]
        [BenchmarkCategory(nameof(DateRange))]
        public NpgsqlRange<DateTime> ParseEmptyDateRange() => NpgsqlRange<DateTime>.Parse(Empty);

        [Benchmark]
        [BenchmarkCategory(nameof(DateRange))]
        public NpgsqlRange<DateTime> ParseDateRange() => NpgsqlRange<DateTime>.Parse(DateRange);
    }
}
