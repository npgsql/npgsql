using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Npgsql.Internal;

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

    readonly PgConverter _converter;
    readonly PgReader _reader;
    readonly PgWriter _writer;
    readonly NpgsqlWriteBuffer _writeBuffer;
    readonly NpgsqlReadBuffer _readBuffer;
    readonly BufferRequirements _binaryRequirements;

    T _value;
    Size _elementSize;

    protected TypeHandlerBenchmarks(PgConverter handler)
    {
        var stream = new EndlessStream();
        _converter = handler ?? throw new ArgumentNullException(nameof(handler));
        _readBuffer = new NpgsqlReadBuffer(null, stream, null, NpgsqlReadBuffer.MinimumSize, NpgsqlWriteBuffer.UTF8Encoding, NpgsqlWriteBuffer.RelaxedUTF8Encoding);
        _writeBuffer =  new NpgsqlWriteBuffer(null, stream, null, NpgsqlWriteBuffer.MinimumSize, NpgsqlWriteBuffer.UTF8Encoding);
        _reader = new PgReader(_readBuffer);
        _writer = new PgWriter(new NpgsqlBufferWriter(_writeBuffer));
        _writer.Init(new PostgresMinimalDatabaseInfo());
        _converter.CanConvert(DataFormat.Binary, out _binaryRequirements);
    }

    public IEnumerable<T> Values() => ValuesOverride();

    protected virtual IEnumerable<T> ValuesOverride() => [default(T)];

    [ParamsSource(nameof(Values))]
    public T Value
    {
        get => _value;
        set
        {
            _value = value;
            object state = null;
            var size = _elementSize = _converter.GetSizeAsObject(new(DataFormat.Binary, _binaryRequirements.Write), value, ref state);
            var current = new ValueMetadata { Format = DataFormat.Binary, BufferRequirement = _binaryRequirements.Write, Size = size, WriteState = state };
            _writer.BeginWrite(async: false, current, CancellationToken.None).GetAwaiter().GetResult();
            _converter.WriteAsObject(_writer, value);
            Buffer.BlockCopy(_writeBuffer.Buffer, 0, _readBuffer.Buffer, 0, size.Value);

            _writer.Commit(size.Value);
            _readBuffer.FilledBytes = size.Value;
            _writeBuffer.WritePosition = 0;
        }
    }

    [Benchmark]
    public T Read()
    {
        _readBuffer.ReadPosition = sizeof(int);
        _reader.StartRead(_binaryRequirements.Read);
        var value = ((PgConverter<T>)_converter).Read(_reader);
        _reader.EndRead();
        return value;
    }

    [Benchmark]
    public void Write()
    {
        _writeBuffer.WritePosition = 0;
        var current = new ValueMetadata { Format = DataFormat.Binary, BufferRequirement = _binaryRequirements.Write, Size = _elementSize, WriteState = null };
        _writer.BeginWrite(async: false, current, CancellationToken.None).GetAwaiter().GetResult();
        ((PgConverter<T>)_converter).Write(_writer, _value);
    }
}
