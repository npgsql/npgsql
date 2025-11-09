using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using Npgsql.Internal;

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

    readonly PgConverter<T> _converter;
    readonly PgWriter _writer;
    readonly NpgsqlWriteBuffer _writeBuffer;
    readonly NpgsqlReadBuffer _readBuffer;
    readonly BufferRequirements _binaryRequirements;

    PgReader _reader;
    T? _value;
    PgValueBindingContext _bindingContext;

    protected TypeHandlerBenchmarks(PgConverter<T> handler)
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

    public IEnumerable<T?> Values() => ValuesOverride();

    protected virtual IEnumerable<T?> ValuesOverride() => [default];

    [ParamsSource(nameof(Values)), MaybeNull]
    public T Value
    {
        get => _value;
        set
        {
            _value = value;

            object? writeState = null;
            var size = _converter.GetSizeOrDbNull(DataFormat.Binary, _binaryRequirements.Write, value, ref writeState);
            _bindingContext = new PgValueBindingContext(DataFormat.Binary, _binaryRequirements.Write, size, writeState);

            if (!_bindingContext.IsDbNullBinding)
            {
                _writer.StartWrite(async: false, _bindingContext, CancellationToken.None).GetAwaiter().GetResult();
                _converter.Write(_writer, value!);
                _writer.EndWrite(_bindingContext.Size.Value);
            }

            Buffer.BlockCopy(_writeBuffer.Buffer, 0, _readBuffer.Buffer, 0, _writeBuffer.WritePosition);
            _readBuffer.FilledBytes = _writeBuffer.WritePosition;
            _writeBuffer.WritePosition = 0;
            _reader = new PgReader(_readBuffer);
            _reader.Init((size ?? -1).Value, DataFormat.Binary);
        }
    }

    [Benchmark]
    public T Read()
    {
        if (_bindingContext.IsDbNullBinding)
            return default!;

        _readBuffer.ReadPosition = 0;
        _reader.StartRead(_binaryRequirements.Read);
        var value = _converter.Read(_reader);
        _reader.EndRead();
        return value;
    }

    [Benchmark]
    public void Write()
    {
        if (_bindingContext.IsDbNullBinding)
            return;

        _writeBuffer.WritePosition = 0;
        _writer.StartWrite(async: false, _bindingContext, CancellationToken.None).GetAwaiter().GetResult();
        _converter.Write(_writer, Value!);
        _writer.EndWrite(_bindingContext.Size.GetValueOrDefault());
    }
}
