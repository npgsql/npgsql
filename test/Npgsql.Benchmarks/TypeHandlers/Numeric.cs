using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Npgsql.Internal.Converters;

namespace Npgsql.Benchmarks.TypeHandlers;

[Config(typeof(Config))]
public class Int16() : TypeHandlerBenchmarks<short>(new Int2Converter<short>());

[Config(typeof(Config))]
public class Int32() : TypeHandlerBenchmarks<int>(new Int4Converter<int>());

[Config(typeof(Config))]
public class Int64() : TypeHandlerBenchmarks<long>(new Int8Converter<long>());

[Config(typeof(Config))]
public class Single() : TypeHandlerBenchmarks<float>(new RealConverter<float>());

[Config(typeof(Config))]
public class Double() : TypeHandlerBenchmarks<double>(new DoubleConverter<double>());

[Config(typeof(Config))]
public class Numeric() : TypeHandlerBenchmarks<decimal>(new DecimalNumericConverter<decimal>())
{
    protected override IEnumerable<decimal> ValuesOverride() =>
    [
        0.0000000000000000000000000001M,
        0.000000000000000000000001M,
        0.00000000000000000001M,
        0.0000000000000001M,
        0.000000000001M,
        0.00000001M,
        0.0001M,
        1M,
        10000M,
        100000000M,
        1000000000000M,
        10000000000000000M,
        100000000000000000000M,
        1000000000000000000000000M,
        10000000000000000000000000000M
    ];
}

[Config(typeof(Config))]
public class Money() : TypeHandlerBenchmarks<decimal>(new MoneyConverter<decimal>());
