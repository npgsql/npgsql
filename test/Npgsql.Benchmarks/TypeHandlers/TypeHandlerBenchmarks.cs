using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using Npgsql.Util;

#nullable disable

namespace Npgsql.Benchmarks.TypeHandlers;

public abstract class TypeHandlerBenchmarks<T>
{
    protected class Config : ManualConfig
    {
        public Config()
        {
            AddColumn(StatisticColumn.OperationsPerSecond);
            AddDiagnoser(MemoryDiagnoser.Default);
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

    readonly EndlessStream _stream;
    readonly NpgsqlTypeHandler _handler;
    readonly NpgsqlReadBuffer _readBuffer;
    readonly NpgsqlWriteBuffer _writeBuffer;
    T _value;
    int _elementSize;

    protected TypeHandlerBenchmarks(NpgsqlTypeHandler handler)
    {
        _stream = new EndlessStream();
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        _readBuffer = new NpgsqlReadBuffer(null, _stream, null, NpgsqlReadBuffer.MinimumSize, Encoding.UTF8, PGUtil.RelaxedUTF8Encoding);
        _writeBuffer = new NpgsqlWriteBuffer(null, _stream, null, NpgsqlWriteBuffer.MinimumSize, Encoding.UTF8);
    }

    protected static PostgresType GetPostgresType(string pgType)
    {
        using (var conn = BenchmarkEnvironment.OpenConnection())
        using (var cmd = new NpgsqlCommand($"SELECT NULL::{pgType}", conn))
        using (var reader = cmd.ExecuteReader())
            return reader.GetPostgresType(0);
    }

    public IEnumerable<T> Values() => ValuesOverride();

    protected virtual IEnumerable<T> ValuesOverride() => new[] { default(T) };

    [ParamsSource(nameof(Values))]
    public T Value
    {
        get => _value;
        set
        {
            NpgsqlLengthCache cache = null;

            _value = value;
            _elementSize = _handler.ValidateAndGetLength(value, ref cache, null);

            cache.Rewind();

            _handler.WriteWithLength(_value, _writeBuffer, cache, null, false);
            Buffer.BlockCopy(_writeBuffer.Buffer, 0, _readBuffer.Buffer, 0, _elementSize);

            _readBuffer.FilledBytes = _elementSize;
            _writeBuffer.WritePosition = 0;
        }
    }

    [Benchmark]
    public T Read()
    {
        _readBuffer.ReadPosition = sizeof(int);
        return _handler.Read<T>(_readBuffer, _elementSize);
    }

    [Benchmark]
    public void Write()
    {
        _writeBuffer.WritePosition = 0;
        _handler.WriteWithLength(_value, _writeBuffer, null, null, false);
    }
}