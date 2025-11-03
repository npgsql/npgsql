using System;
using System.IO;
using Npgsql.Internal;
using NUnit.Framework;

namespace Npgsql.Tests;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)] // Parallel access to a single buffer
class WriteBufferTests
{
    [Test]
    public void Buffered_full_buffer_no_flush()
    {
        WriteBuffer.WritePosition += WriteBuffer.WriteSpaceLeft - sizeof(int);
        var writer = WriteBuffer.GetWriter(null!, FlushMode.NonBlocking);
        Assert.That(writer.ShouldFlush(sizeof(int)), Is.False);

        Assert.DoesNotThrow(() =>
        {
            Span<byte> intBytes = stackalloc byte[4];
            writer.WriteBytes(intBytes);
        });
    }

    [Test]
    public void GetWriter_Full_Buffer()
    {
        WriteBuffer.WritePosition += WriteBuffer.WriteSpaceLeft;
        var writer = WriteBuffer.GetWriter(null!, FlushMode.Blocking);
        Assert.That(writer.ShouldFlush(sizeof(byte)), Is.True);
        writer.Flush();
        Assert.That(writer.ShouldFlush(sizeof(byte)), Is.False);
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1275")]
    public void Chunked_string_with_full_buffer()
    {
        // Fill up the buffer entirely
        WriteBuffer.WriteBytes(new byte[WriteBuffer.Size], 0, WriteBuffer.Size);
        Assert.That(WriteBuffer.WriteSpaceLeft, Is.Zero);

        var data = new string('a', WriteBuffer.Size) + "hello";
        var byteLength = WriteBuffer.TextEncoding.GetByteCount(data);
        WriteBuffer.WriteString(data, byteLength, false);
        Assert.That(WriteBuffer.WritePosition, Is.EqualTo(5));
        Assert.That(WriteBuffer.Buffer.AsSpan(0, 5).ToArray(), Is.EqualTo(new byte[] { (byte)'h', (byte)'e', (byte)'l', (byte)'l', (byte)'o' }));
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/2849")]
    public void Chunked_string_encoding_fits()
    {
        WriteBuffer.WriteBytes(new byte[WriteBuffer.Size - 1], 0, WriteBuffer.Size - 1);
        Assert.That(WriteBuffer.WriteSpaceLeft, Is.EqualTo(1));

        // This unicode character is three bytes when encoded in UTF8
        var data = "\uD55C" + new string('a', WriteBuffer.Size);
        var byteLength = WriteBuffer.TextEncoding.GetByteCount(data);
        WriteBuffer.WriteString(data, byteLength, false);
        Assert.That(WriteBuffer.WritePosition, Is.EqualTo(3));
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/3733")]
    public void Chunked_string_encoding_fits_with_surrogates()
    {
        WriteBuffer.WriteBytes(new byte[WriteBuffer.Size - 1]);
        Assert.That(WriteBuffer.WriteSpaceLeft, Is.EqualTo(1));

        var cyclone = "🌀" + new string('a', WriteBuffer.Size);
        var byteLength = WriteBuffer.TextEncoding.GetByteCount(cyclone);
        WriteBuffer.WriteString(cyclone, byteLength, false);
        Assert.That(WriteBuffer.WritePosition, Is.EqualTo(4));
    }

    [SetUp]
    public void SetUp()
    {
        Underlying = new MemoryStream();
        WriteBuffer = new NpgsqlWriteBuffer(null, Underlying, null, NpgsqlReadBuffer.DefaultSize, NpgsqlWriteBuffer.UTF8Encoding);
        WriteBuffer.MessageLengthValidation = false;
    }

    // ReSharper disable once InconsistentNaming
    NpgsqlWriteBuffer WriteBuffer = default!;
    // ReSharper disable once InconsistentNaming
    MemoryStream Underlying = default!;
}
