using System;
using System.Buffers;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal.Postgres;
using static Npgsql.Internal.Converters.BitStringHelpers;

namespace Npgsql.Internal.Converters;

static class BitStringHelpers
{
    public static int GetByteLengthFromBits(int n)
    {
        const int BitShiftPerByte = 3;
        Debug.Assert(n >= 0);
        // Due to sign extension, we don't need to special case for n == 0, since ((n - 1) >> 3) + 1 = 0
        // This doesn't hold true for ((n - 1) / 8) + 1, which equals 1.
        return (int)((uint)(n - 1 + (1 << BitShiftPerByte)) >> BitShiftPerByte);
    }

    // http://graphics.stanford.edu/~seander/bithacks.html#ReverseByteWith64Bits
    public static byte ReverseBits(byte b) => (byte)(((b * 0x80200802UL) & 0x0884422110UL) * 0x0101010101UL >> 32);
}

sealed class BitArrayBitStringConverter : PgStreamingConverter<BitArray>
{
    public override BitArray Read(PgReader reader)
    {
        if (reader.ShouldBuffer(sizeof(int)))
            reader.Buffer(sizeof(int));

        var bits = reader.ReadInt32();
        return ReadValue(reader.ReadBytes(GetByteLengthFromBits(bits)), bits);
    }
    public override async ValueTask<BitArray> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
    {
        if (reader.ShouldBuffer(sizeof(int)))
            await reader.BufferAsync(sizeof(int), cancellationToken).ConfigureAwait(false);

        var bits = reader.ReadInt32();
        return ReadValue(await reader.ReadBytesAsync(GetByteLengthFromBits(bits), cancellationToken).ConfigureAwait(false), bits);
    }

    internal static BitArray ReadValue(ReadOnlySequence<byte> byteSeq, int bits)
    {
        var bytes = byteSeq.ToArray();
        for (var i = 0; i < bytes.Length; i++)
        {
            ref var b = ref bytes[i];
            b = ReverseBits(b);
        }

        return new(bytes) { Length = bits };
    }

    public override Size GetSize(SizeContext context, BitArray value, ref object? writeState)
        => sizeof(int) + GetByteLengthFromBits(value.Length);

    public override void Write(PgWriter writer, BitArray value)
        => Write(async: false, writer, value, CancellationToken.None).GetAwaiter().GetResult();
    public override ValueTask WriteAsync(PgWriter writer, BitArray value, CancellationToken cancellationToken = default)
        => Write(async: true, writer, value, cancellationToken);

    async ValueTask Write(bool async, PgWriter writer, BitArray value, CancellationToken cancellationToken = default)
    {
        var byteCount = writer.Current.Size.Value - sizeof(int);
        var array = ArrayPool<byte>.Shared.Rent(byteCount);
        for (var pos = 0; pos < byteCount; pos++)
        {
            var bitPos = pos*8;
            var bits = Math.Min(8, value.Length - bitPos);
            var b = 0;
            for (var i = 0; i < bits; i++)
                b += (value[bitPos + i] ? 1 : 0) << (8 - i - 1);
            array[pos] = (byte)b;
        }

        if (writer.ShouldFlush(sizeof(int)))
            await writer.Flush(async, cancellationToken).ConfigureAwait(false);

        writer.WriteInt32(value.Length);
        if (async)
            await writer.WriteBytesAsync(new ReadOnlyMemory<byte>(array, 0, byteCount), cancellationToken).ConfigureAwait(false);
        else
            writer.WriteBytes(new ReadOnlySpan<byte>(array, 0, byteCount));

        ArrayPool<byte>.Shared.Return(array);
    }
}

sealed class BitVector32BitStringConverter : PgBufferedConverter<BitVector32>
{
    static int MaxSize => sizeof(int) + sizeof(int);

    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.Create(Size.CreateUpperBound(MaxSize));
        return format is DataFormat.Binary;
    }

    protected override BitVector32 ReadCore(PgReader reader)
    {
        if (reader.CurrentRemaining > sizeof(int) + sizeof(int))
            throw new InvalidCastException("Can't read a BIT(N) with more than 32 bits to BitVector32, only up to BIT(32).");

        var bits = reader.ReadInt32();
        return GetByteLengthFromBits(bits) switch
        {
            4 => new(reader.ReadInt32()),
            3 => new((reader.ReadInt16() << 8) + reader.ReadByte()),
            2 => new(reader.ReadInt16() << 16),
            1 => new(reader.ReadByte() << 24),
            _ => new(0)
        };
    }

    public override Size GetSize(SizeContext context, BitVector32 value, ref object? writeState)
        => value.Data is 0 ? 4 : MaxSize;

    protected override void WriteCore(PgWriter writer, BitVector32 value)
    {
        if (value.Data == 0)
            writer.WriteInt32(0);
        else
        {
            writer.WriteInt32(32);
            writer.WriteInt32(value.Data);
        }
    }
}

sealed class BoolBitStringConverter : PgBufferedConverter<bool>
{
    static int MaxSize => sizeof(int) + sizeof(byte);

    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.Create(Size.CreateUpperBound(MaxSize), MaxSize);
        return format is DataFormat.Binary;
    }

    protected override bool ReadCore(PgReader reader)
    {
        var bits = reader.ReadInt32();
        return bits switch
        {
            > 1 => throw new InvalidCastException("Can't read a BIT(N) type to bool, only BIT(1)."),
            // We make an accommodation for varbit with no data.
            0 => false,
            _ => (reader.ReadByte() & 128) is not 0
        };
    }

    public override Size GetSize(SizeContext context, bool value, ref object? writeState) => MaxSize;
    protected override void WriteCore(PgWriter writer, bool value)
    {
        writer.WriteInt32(1);
        writer.WriteByte(value ? (byte)128 : (byte)0);
    }
}

sealed class StringBitStringConverter : PgStreamingConverter<string>
{
    public override string Read(PgReader reader)
        => Read(async: false, reader, CancellationToken.None).GetAwaiter().GetResult();
    public override ValueTask<string> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
        => Read(async: true, reader, cancellationToken);

    async ValueTask<string> Read(bool async, PgReader reader, CancellationToken cancellationToken)
    {
        if (reader.ShouldBuffer(sizeof(int)))
            await reader.Buffer(async, sizeof(int), cancellationToken).ConfigureAwait(false);

        var bits = reader.ReadInt32();
        var bytes = async
            ? await reader.ReadBytesAsync(GetByteLengthFromBits(bits), cancellationToken).ConfigureAwait(false)
            : reader.ReadBytes(GetByteLengthFromBits(bits));

        var bitArray = BitArrayBitStringConverter.ReadValue(bytes, bits);
        var sb = new StringBuilder(bits);
        for (var i = 0; i < bitArray.Count; i++)
            sb.Append(bitArray[i] ? '1' : '0');

        return sb.ToString();
    }

    public override Size GetSize(SizeContext context, string value, ref object? writeState)
    {
        if (value.AsSpan().IndexOfAnyExcept('0', '1') is not -1 and var index)
            throw new ArgumentException($"Invalid bitstring character '{value[index]}' at index: {index}", nameof(value));

        return sizeof(int) + GetByteLengthFromBits(value.Length);
    }

    public override void Write(PgWriter writer, string value)
        => Write(async: false, writer, value, CancellationToken.None).GetAwaiter().GetResult();
    public override ValueTask WriteAsync(PgWriter writer, string value, CancellationToken cancellationToken = default)
        => Write(async: true, writer, value, cancellationToken);

    async ValueTask Write(bool async, PgWriter writer, string value, CancellationToken cancellationToken)
    {
        var byteCount = writer.Current.Size.Value - sizeof(int);
        var array = ArrayPool<byte>.Shared.Rent(byteCount);
        for (var pos = 0; pos < byteCount; pos++)
        {
            var bitPos = pos*8;
            var bits = Math.Min(8, value.Length - bitPos);
            var b = 0;
            for (var i = 0; i < bits; i++)
                b += (value[bitPos + i] == '1' ? 1 : 0) << (8 - i - 1);
            array[pos] = (byte)b;
        }

        if (writer.ShouldFlush(sizeof(int)))
            await writer.Flush(async, cancellationToken).ConfigureAwait(false);

        writer.WriteInt32(value.Length);
        if (async)
            await writer.WriteBytesAsync(new ReadOnlyMemory<byte>(array, 0, byteCount), cancellationToken).ConfigureAwait(false);
        else
            writer.WriteBytes(new ReadOnlySpan<byte>(array, 0, byteCount));

        ArrayPool<byte>.Shared.Return(array);
    }
}

/// Note that for BIT(1), this resolver will return a bool by default, to align with SqlClient
/// (see discussion https://github.com/npgsql/npgsql/pull/362#issuecomment-59622101).
sealed class PolymorphicBitStringConverterResolver : PolymorphicConverterResolver<object>
{
    BoolBitStringConverter? _boolConverter;
    BitArrayBitStringConverter? _bitArrayConverter;

    public PolymorphicBitStringConverterResolver(PgTypeId bitString) : base(bitString) { }

    protected override PgConverter Get(Field? field)
        => field?.TypeModifier is 1
            ? _boolConverter ??= new BoolBitStringConverter()
            : _bitArrayConverter ??= new BitArrayBitStringConverter();
}
