using System;
using NUnit.Framework;
using Npgsql.Internal;

namespace Npgsql.Tests;

/// <summary>
/// Direct unit tests for <see cref="PgWriter"/>'s measuring mechanisms.
/// </summary>
public class MeasuringPgWriterTests
{
    static (PgWriter writer, MeasuringSideBufferWriter sink) CreateMeasuringWriter()
    {
        var sink = new MeasuringSideBufferWriter();
        var writer = new PgWriter(sink);
        writer.Init(new PostgresMinimalDatabaseInfo(), 0);
        return (writer, sink);
    }

    [Test]
    public void Exact_writes_prefix_then_content()
    {
        var (writer, sink) = CreateMeasuringWriter();

        using (writer.BeginLengthPrefixingScope(bufferRequirement: Size.Create(3), size: Size.Create(3), state: null))
        {
            writer.WriteByte(0xAA);
            writer.WriteByte(0xBB);
            writer.WriteByte(0xCC);
        }

        writer.EndWrite(Size.Create(7));
        Assert.That(sink.WrittenSpan.ToArray(), Is.EqualTo(new byte[] { 0, 0, 0, 3, 0xAA, 0xBB, 0xCC }));
    }

    [Test]
    public void Unknown_backpatches_prefix_with_measured_size()
    {
        var (writer, sink) = CreateMeasuringWriter();

        using (writer.BeginLengthPrefixingScope(bufferRequirement: Size.Unknown, size: Size.Unknown, state: null))
        {
            writer.WriteByte(0xAA);
            writer.WriteByte(0xBB);
            writer.WriteByte(0xCC);
        }

        writer.EndWrite(Size.Create(7));
        Assert.That(sink.WrittenSpan.ToArray(), Is.EqualTo(new byte[] { 0, 0, 0, 3, 0xAA, 0xBB, 0xCC }));
    }

    [Test]
    public void Unknown_with_empty_inner_writes_zero_prefix()
    {
        var (writer, sink) = CreateMeasuringWriter();

        using (writer.BeginLengthPrefixingScope(bufferRequirement: Size.Unknown, size: Size.Unknown, state: null))
        {
            // no inner writes
        }

        writer.EndWrite(Size.Create(4));
        Assert.That(sink.WrittenSpan.ToArray(), Is.EqualTo(new byte[] { 0, 0, 0, 0 }));
    }

    [Test]
    public void UpperBound_routes_through_measuring_mode()
    {
        var (writer, sink) = CreateMeasuringWriter();

        // UpperBound of 100, actual content is 3 bytes; the prefix should reflect actual size, not the bound.
        using (writer.BeginLengthPrefixingScope(bufferRequirement: Size.CreateUpperBound(100), size: Size.CreateUpperBound(100), state: null))
        {
            writer.WriteByte(0xAA);
            writer.WriteByte(0xBB);
            writer.WriteByte(0xCC);
        }

        writer.EndWrite(Size.Create(7));
        Assert.That(sink.WrittenSpan.ToArray(), Is.EqualTo(new byte[] { 0, 0, 0, 3, 0xAA, 0xBB, 0xCC }));
    }

    [Test]
    public void Sequential_unknown_scopes_dont_pollute_each_other()
    {
        var (writer, sink) = CreateMeasuringWriter();

        using (writer.BeginLengthPrefixingScope(bufferRequirement: Size.Unknown, size: Size.Unknown, state: null))
        {
            writer.WriteByte(0xAA);
        }
        using (writer.BeginLengthPrefixingScope(bufferRequirement: Size.Unknown, size: Size.Unknown, state: null))
        {
            writer.WriteByte(0xBB);
            writer.WriteByte(0xCC);
        }

        writer.EndWrite(Size.Create(11));
        Assert.That(sink.WrittenSpan.ToArray(), Is.EqualTo(new byte[]
        {
            0, 0, 0, 1, 0xAA,        // first scope
            0, 0, 0, 2, 0xBB, 0xCC,  // second scope
        }));
    }

    [Test]
    public void Unknown_handles_inner_larger_than_initial_side_buffer_capacity()
    {
        var (writer, sink) = CreateMeasuringWriter();

        var content = new byte[8192];
        for (var i = 0; i < content.Length; i++)
            content[i] = (byte)(i & 0xFF);

        using (writer.BeginLengthPrefixingScope(bufferRequirement: Size.Unknown, size: Size.Unknown, state: null))
            writer.WriteBytes(content);

        writer.EndWrite(Size.Create(4 + content.Length));
        var bytes = sink.WrittenSpan.ToArray();
        Assert.That(bytes.Length, Is.EqualTo(4 + content.Length));
        // Prefix should be 8192 in big-endian
        Assert.That(bytes[0], Is.EqualTo(0));
        Assert.That(bytes[1], Is.EqualTo(0));
        Assert.That(bytes[2], Is.EqualTo(0x20));
        Assert.That(bytes[3], Is.EqualTo(0x00));
        Assert.That(bytes.AsSpan(4).ToArray(), Is.EqualTo(content));
    }

    [Test]
    public void Nested_unknown_scopes_stack_on_shared_side_buffer()
    {
        var (writer, sink) = CreateMeasuringWriter();

        using (writer.BeginLengthPrefixingScope(bufferRequirement: Size.Unknown, size: Size.Unknown, state: null))  // outer
        {
            writer.WriteByte(0x01);  // outer's pre-inner content
            using (writer.BeginLengthPrefixingScope(bufferRequirement: Size.Unknown, size: Size.Unknown, state: null))  // inner
            {
                writer.WriteByte(0xAA);
                writer.WriteByte(0xBB);
            }
            writer.WriteByte(0x02);  // outer's post-inner content
        }

        // Outer content layout: 0x01 + inner_prefix(4) + 0xAA 0xBB + 0x02 = 8 bytes
        // Outer prefix = 4 bytes with value 8
        // Total = 12
        writer.EndWrite(Size.Create(12));
        Assert.That(sink.WrittenSpan.ToArray(), Is.EqualTo(new byte[]
        {
            0, 0, 0, 8,    // outer prefix (outer content is 8 bytes)
            0x01,           // outer pre-inner
            0, 0, 0, 2,    // inner prefix (inner content is 2 bytes)
            0xAA, 0xBB,    // inner content
            0x02,           // outer post-inner
        }));
    }

    [Test]
    public void Triple_nested_measuring_scopes()
    {
        var (writer, sink) = CreateMeasuringWriter();

        using (writer.BeginLengthPrefixingScope(bufferRequirement: Size.Unknown, size: Size.Unknown, state: null))  // L1
        {
            using (writer.BeginLengthPrefixingScope(bufferRequirement: Size.Unknown, size: Size.Unknown, state: null))  // L2
            {
                using (writer.BeginLengthPrefixingScope(bufferRequirement: Size.Unknown, size: Size.Unknown, state: null))  // L3
                {
                    writer.WriteByte(0xFF);
                }
            }
        }

        // L3 content = 1 byte (0xFF), L3 framed = 4 + 1 = 5
        // L2 content = L3 framed = 5 bytes, L2 framed = 4 + 5 = 9
        // L1 content = L2 framed = 9 bytes, L1 framed = 4 + 9 = 13
        writer.EndWrite(Size.Create(13));
        Assert.That(sink.WrittenSpan.ToArray(), Is.EqualTo(new byte[]
        {
            0, 0, 0, 9,     // L1 prefix
            0, 0, 0, 5,     // L2 prefix
            0, 0, 0, 1,     // L3 prefix
            0xFF,            // L3 content
        }));
    }

    [Test]
    public void Chained_variable_prefix_calls_lambda_with_measured_size()
    {
        var (writer, sink) = CreateMeasuringWriter();

        // Variable-prefix lambda: writes a single-byte length prefix (sufficient for content < 256 bytes).
        Action<PgWriter, int> writeBytePrefix = static (w, size) => w.WriteByte((byte)size);

        using (writer.BeginLengthPrefixingScope(
            bufferRequirement: Size.Unknown,
            size: Size.Unknown,
            state: null,
            prefixingAction: writeBytePrefix))
        {
            writer.WriteByte(0xAA);
            writer.WriteByte(0xBB);
            writer.WriteByte(0xCC);
        }

        // 1-byte prefix(value=3) + 3 content bytes = 4
        writer.EndWrite(Size.Create(4));
        Assert.That(sink.WrittenSpan.ToArray(), Is.EqualTo(new byte[] { 3, 0xAA, 0xBB, 0xCC }));
    }

    [Test]
    public void Chained_variable_prefix_nested_inside_fixed_prefix_outer()
    {
        var (writer, sink) = CreateMeasuringWriter();
        Action<PgWriter, int> writeBytePrefix = static (w, size) => w.WriteByte((byte)size);

        using (writer.BeginLengthPrefixingScope(  // outer: int4 prefix (default)
            bufferRequirement: Size.Unknown, size: Size.Unknown, state: null))
        {
            writer.WriteByte(0x01);  // outer pre-content
            using (writer.BeginLengthPrefixingScope(  // inner: variable byte prefix (chained)
                bufferRequirement: Size.Unknown, size: Size.Unknown, state: null,
                prefixingAction: writeBytePrefix))
            {
                writer.WriteByte(0xAA);
                writer.WriteByte(0xBB);
            }
            writer.WriteByte(0x02);  // outer post-content
        }

        // outer content: 0x01 + (1-byte prefix(2) + 0xAA 0xBB) + 0x02 = 5 bytes
        // outer framed: 4 + 5 = 9 bytes
        writer.EndWrite(Size.Create(9));
        Assert.That(sink.WrittenSpan.ToArray(), Is.EqualTo(new byte[]
        {
            0, 0, 0, 5,    // outer int4 prefix (outer content is 5 bytes)
            0x01,           // outer pre
            2,              // inner 1-byte prefix (inner content is 2 bytes)
            0xAA, 0xBB,    // inner content
            0x02,           // outer post
        }));
    }

    [Test]
    public void Fixed_inside_chained_tags_along_on_chained_buffer()
    {
        var (writer, sink) = CreateMeasuringWriter();
        Action<PgWriter, int> writeBytePrefix = static (w, size) => w.WriteByte((byte)size);

        using (writer.BeginLengthPrefixingScope(  // outer: chained (variable byte prefix)
            bufferRequirement: Size.Unknown, size: Size.Unknown, state: null,
            prefixingAction: writeBytePrefix))
        {
            writer.WriteByte(0xAA);  // outer pre-content
            using (writer.BeginLengthPrefixingScope(  // inner: fixed int4 prefix — should tag along on chained's buffer
                bufferRequirement: Size.Unknown, size: Size.Unknown, state: null))
            {
                writer.WriteByte(0xBB);
                writer.WriteByte(0xCC);
            }
            writer.WriteByte(0xDD);  // outer post-content
        }

        // outer content: 0xAA + (int4 prefix(2) + 0xBB 0xCC) + 0xDD = 1+4+2+1 = 8 bytes
        // outer framed: byte_prefix(8) + 8 = 9 bytes total
        writer.EndWrite(Size.Create(9));
        Assert.That(sink.WrittenSpan.ToArray(), Is.EqualTo(new byte[]
        {
            8,              // outer 1-byte prefix
            0xAA,           // outer pre
            0, 0, 0, 2,    // inner int4 prefix
            0xBB, 0xCC,    // inner content
            0xDD,           // outer post
        }));
    }

    [Test]
    public void Fixed_chained_fixed_three_level_interleaving()
    {
        var (writer, sink) = CreateMeasuringWriter();
        Action<PgWriter, int> writeBytePrefix = static (w, size) => w.WriteByte((byte)size);

        using (writer.BeginLengthPrefixingScope(Size.Unknown, Size.Unknown, state: null))  // L1 fixed
        using (writer.BeginLengthPrefixingScope(Size.Unknown, Size.Unknown, state: null,
            prefixingAction: writeBytePrefix))  // L2 chained
        using (writer.BeginLengthPrefixingScope(Size.Unknown, Size.Unknown, state: null))  // L3 fixed
        {
            writer.WriteByte(0xFF);
        }

        // L3 framed = int4(1) + 0xFF = 5 bytes
        // L2 framed = byte(5) + 5 = 6 bytes
        // L1 framed = int4(6) + 6 = 10 bytes
        writer.EndWrite(Size.Create(10));
        Assert.That(sink.WrittenSpan.ToArray(), Is.EqualTo(new byte[]
        {
            0, 0, 0, 6,    // L1 prefix
            5,              // L2 prefix
            0, 0, 0, 1,    // L3 prefix
            0xFF,           // L3 content
        }));
    }

    [Test]
    public void Chained_fixed_chained_three_level_interleaving()
    {
        var (writer, sink) = CreateMeasuringWriter();
        Action<PgWriter, int> writeBytePrefix = static (w, size) => w.WriteByte((byte)size);

        using (writer.BeginLengthPrefixingScope(Size.Unknown, Size.Unknown, state: null,
            prefixingAction: writeBytePrefix))  // L1 chained
        using (writer.BeginLengthPrefixingScope(Size.Unknown, Size.Unknown, state: null))  // L2 fixed
        using (writer.BeginLengthPrefixingScope(Size.Unknown, Size.Unknown, state: null,
            prefixingAction: writeBytePrefix))  // L3 chained
        {
            writer.WriteByte(0xFF);
        }

        // L3 framed = byte(1) + 0xFF = 2 bytes
        // L2 framed = int4(2) + 2 = 6 bytes
        // L1 framed = byte(6) + 6 = 7 bytes
        writer.EndWrite(Size.Create(7));
        Assert.That(sink.WrittenSpan.ToArray(), Is.EqualTo(new byte[]
        {
            6,              // L1 prefix
            0, 0, 0, 2,    // L2 prefix
            1,              // L3 prefix
            0xFF,           // L3 content
        }));
    }

    [Test]
    public void State_correctly_restored_after_measuring_scope()
    {
        var (writer, sink) = CreateMeasuringWriter();

        // Write some bytes before measuring
        writer.WriteByte(0x01);
        writer.WriteByte(0x02);

        using (writer.BeginLengthPrefixingScope(bufferRequirement: Size.Unknown, size: Size.Unknown, state: null))
        {
            writer.WriteByte(0xAA);
        }

        // Subsequent writes should land directly in the parent buffer, after the measured scope.
        writer.WriteByte(0xFF);

        writer.EndWrite(Size.Create(8));
        Assert.That(sink.WrittenSpan.ToArray(), Is.EqualTo(new byte[]
        {
            0x01, 0x02,           // pre-scope
            0, 0, 0, 1, 0xAA,     // measuring scope (prefix + content)
            0xFF,                 // post-scope
        }));
    }

    [Test]
    public void No_prefix_writes_content_without_a_length()
    {
        var (writer, sink) = CreateMeasuringWriter();

        // lengthByteCount: null — no length prefix; the nested write is framed by the caller, or needs none.
        using (writer.BeginPassthroughScope(bufferRequirement: Size.Create(3), size: Size.Create(3), state: null))
        {
            writer.WriteByte(0xAA);
            writer.WriteByte(0xBB);
            writer.WriteByte(0xCC);
        }

        writer.EndWrite(Size.Create(3));
        Assert.That(sink.WrittenSpan.ToArray(), Is.EqualTo(new byte[] { 0xAA, 0xBB, 0xCC }));
    }

    [Test]
    public void No_prefix_with_unknown_size_does_not_throw()
    {
        var (writer, sink) = CreateMeasuringWriter();

        // Unknown size + no prefix — the LateBindingConverter shape: a transparent wrapper re-framing for a
        // measuring inner. The Unknown size must flow through without a Size.Value throw, and no prefix lands.
        using (writer.BeginPassthroughScope(bufferRequirement: Size.Unknown, size: Size.Unknown, state: null))
        {
            writer.WriteByte(0xAA);
            writer.WriteByte(0xBB);
        }

        writer.EndWrite(Size.Create(2));
        Assert.That(sink.WrittenSpan.ToArray(), Is.EqualTo(new byte[] { 0xAA, 0xBB }));
    }

    [Test]
    public void No_prefix_nested_inside_measured_scope()
    {
        var (writer, sink) = CreateMeasuringWriter();

        using (writer.BeginLengthPrefixingScope(bufferRequirement: Size.Unknown, size: Size.Unknown, state: null))  // outer int4
        {
            writer.WriteByte(0x01);
            // No-prefix inner — its content joins the outer's measured region with no length of its own.
            using (writer.BeginPassthroughScope(bufferRequirement: Size.Create(2), size: Size.Create(2), state: null))
            {
                writer.WriteByte(0xAA);
                writer.WriteByte(0xBB);
            }
            writer.WriteByte(0x02);
        }

        // outer content: 0x01 + 0xAA 0xBB + 0x02 = 4 bytes (no inner prefix); outer framed: int4(4) + 4 = 8.
        writer.EndWrite(Size.Create(8));
        Assert.That(sink.WrittenSpan.ToArray(), Is.EqualTo(new byte[]
        {
            0, 0, 0, 4,
            0x01, 0xAA, 0xBB, 0x02,
        }));
    }
}
