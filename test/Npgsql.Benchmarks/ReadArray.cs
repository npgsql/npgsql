using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Npgsql.Benchmarks
{
    public abstract class ReadNullableArrayBase
    {
        public readonly struct ConfigStruct
        {
            public readonly int NumArrayElements;
            public readonly double? Ratio;

            public ConfigStruct(int numElements, double? ratio)
            {
                NumArrayElements = numElements;
                Ratio = ratio;
            }

            public override string ToString()
            {
                var writer = new StringWriter();
                writer.Write($"{NumArrayElements}");
                if (Ratio.HasValue)
                {
                    writer.Write($",{(int)Math.Truncate(100 / (1 + Ratio.Value))}%");
                }
                return writer.ToString();
            }
        }

        NpgsqlConnection _conn = default!;
        NpgsqlCommand _cmd = default!;
        NpgsqlDataReader _reader = default!;

        public IEnumerable<ConfigStruct> GetConfigValuesForNullable()
        {
            var numArrayElementsList = new[] { 0, 1, 10, 1000, 100000 };
            foreach (var numArrayElements in numArrayElementsList)
            {
                switch (numArrayElements)
                {
                    case 0:
                        yield return new ConfigStruct(numArrayElements, null);
                        break;
                    case 1:
                        yield return new ConfigStruct(numArrayElements, 0D);
                        yield return new ConfigStruct(numArrayElements, double.MaxValue);
                        break;
                    case 10:
                        yield return new ConfigStruct(numArrayElements, 0D);
                        yield return new ConfigStruct(numArrayElements, 0.5D);
                        yield return new ConfigStruct(numArrayElements, 1D);
                        yield return new ConfigStruct(numArrayElements, 2D);
                        yield return new ConfigStruct(numArrayElements, double.MaxValue);
                        break;
                    default:
                        yield return new ConfigStruct(numArrayElements, 0D);
                        yield return new ConfigStruct(numArrayElements, 0.125D);
                        yield return new ConfigStruct(numArrayElements, 0.5D);
                        yield return new ConfigStruct(numArrayElements, 1D);
                        yield return new ConfigStruct(numArrayElements, 2D);
                        yield return new ConfigStruct(numArrayElements, 8D);
                        yield return new ConfigStruct(numArrayElements, double.MaxValue);
                        break;
                }
            }
        }

        [ParamsSource(nameof(GetConfigValuesForNullable))]
        public virtual ConfigStruct Config { get; set; }

        protected void Setup<T>(T initializationValue)
        {
            _conn = BenchmarkEnvironment.OpenConnection();
            _cmd = new NpgsqlCommand("SELECT @p1;", _conn);
            _cmd.Parameters.AddWithValue("@p1", CreateArray(initializationValue));
            _reader = _cmd.ExecuteReader();
            _reader.Read();
        }

        protected virtual object CreateArray<T>(T initializationValue)
        {
            return Create(initializationValue).ToArray();

            IEnumerable<T> Create(T init)
            {
                for (var i = 0; i < Config.NumArrayElements; i++)
                {
                    Debug.Assert(Config.Ratio.HasValue);
                    if (0 == (int)Math.Truncate((i + 1) % (1 + Config.Ratio!.Value)))
// Unconstrained generics and nullable reference types currently don't interoperate
// if the return value might be null so we have to disable the warning here
#pragma warning disable CS8653
                        yield return default;
#pragma warning restore CS8653
                    else
                        yield return init;
                }
            }
        }

        protected void Cleanup()
        {
            _reader.Dispose();
            _cmd.Dispose();
            _conn.Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected T GetFieldValue<T>()
            => _reader.GetFieldValue<T>(0);
    }

    public abstract class ReadArrayBase : ReadNullableArrayBase
    {
        protected override object CreateArray<T>(T initializationValue)
            => Enumerable.Repeat(initializationValue, Config.NumArrayElements).ToArray();

        public IEnumerable<ConfigStruct> GetConfigValues()
        {
            var numArrayElementsList = new[] { 0, 1, 10, 1000, 100000 };
            foreach (var numArrayElements in numArrayElementsList)
            {
                yield return new ConfigStruct(numArrayElements, null);
            }
        }

        [ParamsSource(nameof(GetConfigValues))]
        public override ConfigStruct Config { get; set; }
    }

    public class ReadArray : ReadArrayBase
    {
        [GlobalSetup(Target = nameof(ReadIntArray))]
        public void GlobalSetupForNullableInt()
            => Setup(42);

        [GlobalSetup(Target = nameof(ReadStringArray))]
        public void GlobalSetupForString()
            => Setup("The Answer to the Ultimate Question of Life, The Universe, and Everything.");

        [GlobalCleanup]
        public void GlobalCleanup()
            => Cleanup();

        [Benchmark]
        public void ReadIntArray()
            => GetFieldValue<int[]>();

        [Benchmark]
        public void ReadStringArray()
            => GetFieldValue<string[]>();
    }

    public class ReadList : ReadArrayBase
    {
        [GlobalSetup(Target = nameof(ReadListOfInt))]
        public void GlobalSetupForNullableInt()
            => Setup(42);

        [GlobalSetup(Target = nameof(ReadListOfString))]
        public void GlobalSetupForString()
            => Setup("The Answer to the Ultimate Question of Life, The Universe, and Everything.");

        [GlobalCleanup]
        public void GlobalCleanup()
            => Cleanup();

        [Benchmark]
        public void ReadListOfInt()
            => GetFieldValue<List<int>>();

        [Benchmark]
        public void ReadListOfString()
            => GetFieldValue<List<string>>();
    }

    public class ReadArrayWithNulls: ReadNullableArrayBase
    {
        [GlobalSetup(Target = nameof(ReadNullableIntArray))]
        public void GlobalSetupForNullableInt()
            => Setup<int?>(42);

        [GlobalSetup(Target = nameof(ReadStringArray))]
        public void GlobalSetupForString()
            => Setup("The Answer to the Ultimate Question of Life, The Universe, and Everything.");

        [GlobalCleanup]
        public void GlobalCleanup()
            => Cleanup();

        [Benchmark]
        public void ReadNullableIntArray()
            => GetFieldValue<int?[]>();

        [Benchmark]
        public void ReadStringArray()
            => GetFieldValue<string[]>();
    }

    public class ReadListWithNulls : ReadNullableArrayBase
    {
        [GlobalSetup(Target = nameof(ReadListOfNullableInt))]
        public void GlobalSetupForNullableInt()
            => Setup<int?>(42);

        [GlobalSetup(Target = nameof(ReadListOfString))]
        public void GlobalSetupForString()
            => Setup("The Answer to the Ultimate Question of Life, The Universe, and Everything.");

        [GlobalCleanup]
        public void GlobalCleanup()
            => Cleanup();

        [Benchmark]
        public void ReadListOfNullableInt()
            => GetFieldValue<List<int?>>();

        [Benchmark]
        public void ReadListOfString()
            => GetFieldValue<List<string>>();
    }
}
