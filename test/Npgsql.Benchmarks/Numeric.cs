using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using Npgsql.TypeHandlers.NumericHandlers;
using System;
using System.IO;
using System.Text;

namespace Npgsql.Benchmarks
{
    [Config(typeof(Config))]
    public class Numeric
    {
        class Config : ManualConfig
        {
            public Config()
            {
                Add(StatisticColumn.OperationsPerSecond);
                Add(MemoryDiagnoser.Default);
            }
        }

        class EndlessStream : Stream
        {
            public override bool CanRead => true;
            public override bool CanSeek => true;
            public override bool CanWrite => true;
            public override long Length => long.MaxValue;
            public override long Position { get => 0L; set { } }
            public override void Flush() { }
            public override int Read(byte[] buffer, int offset, int count) => count;
            public override long Seek(long offset, SeekOrigin origin) => 0L;
            public override void SetLength(long value) { }
            public override void Write(byte[] buffer, int offset, int count) { }
        }

        readonly EndlessStream _stream = new EndlessStream();
        readonly NumericHandler _handler = new NumericHandler();
        readonly NpgsqlReadBuffer _readBuffer;
        readonly NpgsqlWriteBuffer _writeBuffer;
        decimal _value;
        int _elementSize;

        public Numeric()
        {
            _stream = new EndlessStream(); _handler = new NumericHandler();
            _readBuffer = new NpgsqlReadBuffer(null, _stream, NpgsqlReadBuffer.MinimumSize, Encoding.UTF8);
            _writeBuffer = new NpgsqlWriteBuffer(null, _stream, NpgsqlWriteBuffer.MinimumSize, Encoding.UTF8);
        }

        public decimal Value
        {
            get => _value;
            set
            {
                _value = value;
                _elementSize = _handler.ValidateAndGetLength(value, null);
                _handler.Write(_value, _writeBuffer, null);

                Buffer.BlockCopy(_writeBuffer.Buffer, 0, _readBuffer.Buffer, 0, _elementSize);

                _readBuffer.FilledBytes = _elementSize;
                _writeBuffer.WritePosition = 0;
            }
        }

        public static decimal[] Values => new decimal[]
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

        [Benchmark]
        public void Read()
        {
            _readBuffer.ReadPosition = 0;
            _handler.Read(_readBuffer, _elementSize);
        }

        [Benchmark]
        public void Write()
        {
            _writeBuffer.WritePosition = 0;
            _handler.Write(_value, _writeBuffer, null);
        }
    }
}
