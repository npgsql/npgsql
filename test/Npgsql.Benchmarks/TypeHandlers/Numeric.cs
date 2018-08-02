using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Npgsql.TypeHandlers.NumericHandlers;

namespace Npgsql.Benchmarks.TypeHandlers
{
    [Config(typeof(Config))]
    public class Int16 : TypeHandlerBenchmarks<short>
    {
        public Int16() : base(new Int16Handler()) { }
    }

    [Config(typeof(Config))]
    public class Int32 : TypeHandlerBenchmarks<int>
    {
        public Int32() : base(new Int32Handler()) { }
    }

    [Config(typeof(Config))]
    public class Int64 : TypeHandlerBenchmarks<long>
    {
        public Int64() : base(new Int64Handler()) { }
    }

    [Config(typeof(Config))]
    public class Single : TypeHandlerBenchmarks<float>
    {
        public Single() : base(new SingleHandler()) { }
    }

    [Config(typeof(Config))]
    public class Double : TypeHandlerBenchmarks<double>
    {
        public Double() : base(new DoubleHandler()) { }
    }

    [Config(typeof(Config))]
    public class Numeric : TypeHandlerBenchmarks<decimal>
    {
        public Numeric() : base(new NumericHandler()) { }

        protected override IEnumerable<decimal> ValuesOverride() => new[]
        {
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
            10000000000000000000000000000M,
        };
    }

    [Config(typeof(Config))]
    public class Money : TypeHandlerBenchmarks<decimal>
    {
        public Money() : base(new MoneyHandler()) { }
    }
}
