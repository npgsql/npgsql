using System;
using System.Buffers;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal.Descriptors;
using Npgsql.PostgresTypes;
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
}

sealed class BitArrayBitStringConverter : PgStreamingConverter<BitArray>
{
    readonly ArrayPool<byte> _arrayPool;
    public BitArrayBitStringConverter(ArrayPool<byte>? arrayPool = null) => _arrayPool = arrayPool ?? ArrayPool<byte>.Shared;

    public static BitArray ReadValue(ReadOnlySequence<byte> byteSeq, int bits)
    {
        var bytes = byteSeq.ToArray();
        for (var i = 0; i < bytes.Length; i++)
        {
            ref var b = ref bytes[i];
            // http://graphics.stanford.edu/~seander/bithacks.html#ReverseByteWith64Bits
            b = b = (byte)(((b * 0x80200802UL) & 0x0884422110UL) * 0x0101010101UL >> 32);
        }

        return new(bytes) { Length = bits };
    }

    public override BitArray Read(PgReader reader)
    {
        var bits = reader.ReadInt32();
        return ReadValue(reader.ReadBytes(GetByteLengthFromBits(bits)), bits);
    }
    public override Size GetSize(SizeContext context, BitArray value, ref object? writeState) => sizeof(int) + GetByteLengthFromBits(value.Length);

    public override void Write(PgWriter writer, BitArray value)
    {
        var length = GetByteLengthFromBits(value.Length);
        var array = _arrayPool.Rent(length);
        value.CopyTo(array, 0);

        writer.WriteInt32(value.Length);
        writer.WriteRaw(new(array, 0, length));

        _arrayPool.Return(array);
    }

    public override async ValueTask<BitArray> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
    {
        var bits = reader.ReadInt32();
        return ReadValue(await reader.ReadBytesAsync(GetByteLengthFromBits(bits), cancellationToken).ConfigureAwait(false), bits);
    }

    public override async ValueTask WriteAsync(PgWriter writer, BitArray value, CancellationToken cancellationToken = default)
    {
        var length = (value.Length + 7) / 8;
        var array = _arrayPool.Rent(length);
        value.CopyTo(array, 0);

        writer.WriteInt32(value.Length);
        await writer.WriteRawAsync(new(array, 0, length), cancellationToken).ConfigureAwait(false);

        _arrayPool.Return(array);
    }
}

sealed class BitVector32BitStringConverter : PgBufferedConverter<BitVector32>
{
    static int MaxSize => sizeof(int) + sizeof(int);

    public override void GetBufferRequirements(DataFormat format, out Size readRequirement, out Size writeRequirement)
        => readRequirement = writeRequirement = Size.CreateUpperBound(MaxSize);

    public override bool CanConvert(DataFormat format, out BufferingRequirement bufferingRequirement)
    {
        bufferingRequirement = BufferingRequirement.Custom;
        return format is DataFormat.Binary;
    }

    protected override BitVector32 ReadCore(PgReader reader)
    {
        if (reader.Current.Size.Value > sizeof(int) + sizeof(int))
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

    public override void GetBufferRequirements(DataFormat format, out Size readRequirement, out Size writeRequirement)
    {
        readRequirement = Size.CreateUpperBound(MaxSize);
        writeRequirement = MaxSize;
    }

    public override bool CanConvert(DataFormat format, out BufferingRequirement bufferingRequirement)
    {
        bufferingRequirement = BufferingRequirement.Custom;
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

/// Note that for BIT(1), this resolver will return a bool by default, to align with SqlClient
/// (see discussion https://github.com/npgsql/npgsql/pull/362#issuecomment-59622101).
sealed class PolymorphicBitStringConverterResolver : PolymorphicReadConverterResolver
{
    BoolBitStringConverter? _boolConverter;
    BitArrayBitStringConverter? _bitArrayConverter;

    public PolymorphicBitStringConverterResolver(PgTypeId bitString) : base(bitString) { }

    protected override PgConverter Get(Field? field)
        => field?.TypeModifier is 1
            ? _boolConverter ??= new BoolBitStringConverter()
            : _bitArrayConverter ??= new BitArrayBitStringConverter();
}
