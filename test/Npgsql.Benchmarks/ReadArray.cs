using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;

namespace Npgsql.Benchmarks
{
    class ReadArrayConfig : ManualConfig
    {
        public ReadArrayConfig()
        {
            Add(StatisticColumn.OperationsPerSecond);
            Add(MemoryDiagnoser.Default);
        }
    }


    public abstract class ReadArray<T>
    {
        readonly T _initializationValue;
        NpgsqlConnection _conn;
        NpgsqlCommand _cmd;
        NpgsqlDataReader _reader;

        [Params(0, 10, 1000, 100000)]
        public int NumArrayElements { get; set; }

        protected ReadArray(T initializationValue)
        {
            _initializationValue = initializationValue;
        }

        [GlobalSetup]
        public void GlobalSetup()
        {
            _conn = BenchmarkEnvironment.OpenConnection();
            _cmd = new NpgsqlCommand("SELECT @p1;", _conn);
            _cmd.Parameters.AddWithValue("@p1", GetArray());
            _reader = _cmd.ExecuteReader();
            _reader.Read();
        }

        T[] GetArray()
            => Enumerable.Repeat(_initializationValue, NumArrayElements).ToArray();

        [GlobalCleanup]
        public void Cleanup()
        {
            _reader.Dispose();
            _cmd.Dispose();
            _conn.Dispose();
        }

        [Benchmark]
        public void AsArray() => _reader.GetFieldValue<T[]>(0);

        [Benchmark]
        public object AsListOfT() => _reader.GetFieldValue<List<T>>(0);
    }

    [Config(typeof(ReadArrayConfig))]
    public class ReadArrayOfStrings : ReadArray<string>
    {
        public ReadArrayOfStrings() : base("FooBar") { }
    }

    [Config(typeof(ReadArrayConfig))]
    public class ReadArrayOfIntegers : ReadArray<int>
    {
        public ReadArrayOfIntegers() : base(42) { }
    }
}
