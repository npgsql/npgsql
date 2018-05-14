using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;

namespace Npgsql.Benchmarks
{
    [Config(typeof(Config))]
    public class NumericPerf
    {
        readonly NpgsqlConnection _conn;
        readonly NpgsqlCommand _cmd;
        readonly NpgsqlDataReader _reader;

        public NumericPerf()
        {
            _conn = BenchmarkEnvironment.OpenConnection();
            _cmd = new NpgsqlCommand("SELECT '1.25'::numeric(10,1), '1.24'::numeric(10,1), '1.2'::numeric(10,2), '1.2'::numeric(10,3), '1.2'::numeric(10,4), '1.2'::numeric(10,5)", _conn);
            _reader = _cmd.ExecuteReader();
            _reader.Read();
        }

        [Benchmark]
        public void RoundUp() => _reader.GetFieldValue<decimal>(0);

        [Benchmark]
        public void RoundDown() => _reader.GetFieldValue<decimal>(1);

        [Benchmark]
        public void ExpandScale2() => _reader.GetFieldValue<decimal>(2);

        [Benchmark]
        public void ExpandScale3() => _reader.GetFieldValue<decimal>(3);

        [Benchmark]
        public void ExpandScale4() => _reader.GetFieldValue<decimal>(4);

        [Benchmark]
        public void ExpandScale5() => _reader.GetFieldValue<decimal>(5);

        class Config : ManualConfig
        {
            public Config() => Add(StatisticColumn.OperationsPerSecond);
        }
    }
}
