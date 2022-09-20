using Npgsql.Internal;
using Npgsql.Util;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql.Tests;

[NonParallelizable] // Parallel access to a single buffer
class ReadBufferTests
{
    [Test]
    public void Skip()
    {
        for (byte i = 0; i < 50; i++)
            Writer.WriteByte(i);

        ReadBuffer.Ensure(10);
        ReadBuffer.Skip(7);
        Assert.That(ReadBuffer.ReadByte(), Is.EqualTo(7));
        ReadBuffer.Skip(10);
        ReadBuffer.Ensure(1);
        Assert.That(ReadBuffer.ReadByte(), Is.EqualTo(18));
        ReadBuffer.Skip(20);
        ReadBuffer.Ensure(1);
        Assert.That(ReadBuffer.ReadByte(), Is.EqualTo(39));
    }

    [Test]
    public void ReadSingle()
    {
        const float expected = 8.7f;
        var bytes = BitConverter.GetBytes(expected);
        Array.Reverse(bytes);
        Writer.Write(bytes);

        ReadBuffer.Ensure(4);
        Assert.That(ReadBuffer.ReadSingle(), Is.EqualTo(expected));
    }

    [Test]
    public void ReadDouble()
    {
        const double expected = 8.7;
        var bytes = BitConverter.GetBytes(expected);
        Array.Reverse(bytes);
        Writer.Write(bytes);

        ReadBuffer.Ensure(8);
        Assert.That(ReadBuffer.ReadDouble(), Is.EqualTo(expected));
    }

    [Test]
    public void ReadNullTerminatedString_buffered_only()
    {
        Writer
            .Write(PGUtil.UTF8Encoding.GetBytes(new string("foo")))
            .WriteByte(0)
            .Write(PGUtil.UTF8Encoding.GetBytes(new string("bar")))
            .WriteByte(0);

        ReadBuffer.Ensure(1);

        Assert.That(ReadBuffer.ReadNullTerminatedString(), Is.EqualTo("foo"));
        Assert.That(ReadBuffer.ReadNullTerminatedString(), Is.EqualTo("bar"));
    }

    [Test]
    public async Task ReadNullTerminatedString_with_io()
    {
        Writer.Write(PGUtil.UTF8Encoding.GetBytes(new string("Chunked ")));
        ReadBuffer.Ensure(1);
        var task = ReadBuffer.ReadNullTerminatedString(async: true);
        Assert.That(!task.IsCompleted);

        Writer
            .Write(PGUtil.UTF8Encoding.GetBytes(new string("string")))
            .WriteByte(0)
            .Write(PGUtil.UTF8Encoding.GetBytes(new string("bar")))
            .WriteByte(0);
        Assert.That(task.IsCompleted);
        Assert.That(await task, Is.EqualTo("Chunked string"));
        Assert.That(ReadBuffer.ReadNullTerminatedString(), Is.EqualTo("bar"));
    }

#pragma warning disable CS8625
    [SetUp]
    public void SetUp()
    {
        var stream = new MockStream();
        ReadBuffer = new NpgsqlReadBuffer(null, stream, null, NpgsqlReadBuffer.DefaultSize, PGUtil.UTF8Encoding, PGUtil.RelaxedUTF8Encoding);
        Writer = stream.Writer;
    }
#pragma warning restore CS8625

    // ReSharper disable once InconsistentNaming
    NpgsqlReadBuffer ReadBuffer = default!;
    // ReSharper disable once InconsistentNaming
    MockStream.MockStreamWriter Writer = default!;

    class MockStream : Stream
    {
        const int Size = 8192;

        internal MockStreamWriter Writer { get; }

        public MockStream() => Writer = new MockStreamWriter(this);

        TaskCompletionSource<object> _tcs = new();
        readonly byte[] _data = new byte[Size];
        int _filled;

        public override int Read(byte[] buffer, int offset, int count)
            => Read(buffer, offset, count, async: false).GetAwaiter().GetResult();

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            => Read(buffer, offset, count, async: true);

        async Task<int> Read(byte[] buffer, int offset, int count, bool async)
        {
            if (_filled == 0)
            {
                _tcs = new TaskCompletionSource<object>();
                if (async)
                    await _tcs.Task;
                else
                    _tcs.Task.Wait();
            }

            count = Math.Min(count, _filled);
            new Span<byte>(_data, 0, count).CopyTo(new Span<byte>(buffer, offset, count));
            new Span<byte>(_data, count, _filled - count).CopyTo(_data);
            _filled -= count;
            return count;
        }

        internal class MockStreamWriter
        {
            readonly MockStream _stream;

            public MockStreamWriter(MockStream stream) => _stream = stream;

            public MockStreamWriter WriteByte(byte b)
            {
                Span<byte> bytes = stackalloc byte[1];
                bytes[0] = b;
                Write(bytes);
                return this;
            }

            public MockStreamWriter Write(ReadOnlySpan<byte> bytes)
            {
                if (_stream._filled + bytes.Length > Size)
                    throw new Exception("Mock stream overrun");
                bytes.CopyTo(new Span<byte>(_stream._data, _stream._filled, bytes.Length));
                _stream._filled += bytes.Length;
                _stream._tcs.TrySetResult(new());
                return this;
            }
        }

        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void Flush() => throw new NotSupportedException();

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();
        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }
    }
}
