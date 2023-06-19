using System;
using System.Buffers;
using System.Collections;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal.Descriptors;
using Npgsql.PostgresTypes;

namespace Npgsql.Internal.Converters;

sealed class BitArrayBitStringConverter : PgStreamingConverter<BitArray>
{
    readonly ArrayPool<byte> _arrayPool;
    public BitArrayBitStringConverter(ArrayPool<byte>? arrayPool = null) => _arrayPool = arrayPool ?? ArrayPool<byte>.Shared;

    public static BitArray ReadValue(ReadOnlySequence<byte> bytes)
        => new(bytes.ToArray());

    public override BitArray Read(PgReader reader) => ReadValue(reader.ReadBytes((reader.ReadInt32() + 7) / 8));
    public override Size GetSize(SizeContext context, BitArray value, ref object? writeState) => sizeof(int) + (value.Length + 7) / 8;

    public override void Write(PgWriter writer, BitArray value)
    {
        var array = _arrayPool.Rent((value.Length + 7) / 8);
        value.CopyTo(array, 0);

        writer.WriteInt32(value.Length);
        writer.WriteRaw(new ReadOnlySequence<byte>(array));

        _arrayPool.Return(array);
    }

    public override async ValueTask<BitArray> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
        => ReadValue(await reader.ReadBytesAsync((reader.ReadInt32() + 7) / 8, cancellationToken));

    public override async ValueTask WriteAsync(PgWriter writer, BitArray value, CancellationToken cancellationToken = default)
    {
        var array = _arrayPool.Rent((value.Length + 7) / 8);
        value.CopyTo(array, 0);

        writer.WriteInt32(value.Length);
        await writer.WriteRawAsync(new ReadOnlySequence<byte>(array), cancellationToken);

        _arrayPool.Return(array);
    }
}

sealed class BitVector32BitStringConverter : PgBufferedConverter<BitVector32>
{
    static int MaxSize => sizeof(int) + sizeof(int);

    public override void GetBufferRequirements(DataFormat format, out Size readRequirement, out Size writeRequirement)
        => readRequirement = writeRequirement = Size.CreateUpperBound(MaxSize);

    public override bool CanConvert(DataFormat format, out BufferingRequirement bufferingRequirement, out bool fixedSize)
    {
        bufferingRequirement = BufferingRequirement.Custom;
        fixedSize = false;
        return format is DataFormat.Binary;
    }

    protected override BitVector32 ReadCore(PgReader reader)
    {
        if (reader.Current.Size.Value > sizeof(int) + sizeof(int))
            throw new InvalidCastException("Can't read a BIT(N) with more than 32 bits to BitVector32, only up to BIT(32).");

        return new(reader.ReadInt32() is 0 ? 0 : reader.ReadInt32());
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
        => readRequirement = writeRequirement = Size.CreateUpperBound(MaxSize);

    public override bool CanConvert(DataFormat format, out BufferingRequirement bufferingRequirement, out bool fixedSize)
    {
        bufferingRequirement = BufferingRequirement.Custom;
        fixedSize = true;
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
