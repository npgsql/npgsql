using System;
using System.Buffers.Binary;
using System.Runtime.InteropServices;

namespace Npgsql.Internal.Converters;

public class GuidUuidConverter : PgBufferedConverter<Guid>
{
    public override bool CanConvert(DataFormat format, out BufferingRequirement bufferingRequirement)
    {
        bufferingRequirement = BufferingRequirement.FixedSize;
        return base.CanConvert(format, out _);
    }
    public override Size GetSize(SizeContext context, Guid value, ref object? writeState) => 16;

    protected override Guid ReadCore(PgReader reader)
        => new GuidRaw
        {
            Data1 = reader.ReadInt32(),
            Data2 = reader.ReadInt16(),
            Data3 = reader.ReadInt16(),
            Data4 = BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(reader.ReadInt64()) : reader.ReadInt64()
        }.Value;

    protected override void WriteCore(PgWriter writer, Guid value)
    {
        var raw = new GuidRaw(value);

        writer.WriteInt32(raw.Data1);
        writer.WriteInt16(raw.Data2);
        writer.WriteInt16(raw.Data3);
        writer.WriteInt64(BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(raw.Data4) : raw.Data4);
    }

    // TODO ifdef once https://github.com/dotnet/runtime/issues/86798 gets merged (ideally before 8.0)
    // The following table shows .NET GUID vs Postgres UUID (RFC 4122) layouts.
    //
    // Note that the first fields are converted from/to native endianness (handled by the Read*
    // and Write* methods), while the last field is always read/written in big-endian format.
    //
    // We're reverting endianness on little endian systems to get it into big endian format.
    //
    // | Bits | Bytes | Name  | Endianness (GUID) | Endianness (RFC 4122) |
    // | ---- | ----- | ----- | ----------------- | --------------------- |
    // | 32   | 4     | Data1 | Native            | Big                   |
    // | 16   | 2     | Data2 | Native            | Big                   |
    // | 16   | 2     | Data3 | Native            | Big                   |
    // | 64   | 8     | Data4 | Big               | Big                   |
    [StructLayout(LayoutKind.Explicit)]
    struct GuidRaw
    {
        [FieldOffset(0)] public Guid Value;
        [FieldOffset(0)] public int Data1;
        [FieldOffset(4)] public short Data2;
        [FieldOffset(6)] public short Data3;
        [FieldOffset(8)] public long Data4;
        public GuidRaw(Guid value) : this() => Value = value;
    }
}
