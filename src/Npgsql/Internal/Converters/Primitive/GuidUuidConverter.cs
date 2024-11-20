using System;
using System.Buffers.Binary;
using System.Runtime.InteropServices;

namespace Npgsql.Internal.Converters;

sealed class GuidUuidConverter : PgBufferedConverter<Guid>
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(16 * sizeof(byte));
        return format is DataFormat.Binary;
    }

    protected override Guid ReadCore(PgReader reader)
        => new(reader.ReadBytes(16).FirstSpan, bigEndian: true);

    protected override void WriteCore(PgWriter writer, Guid value)
    {
        Span<byte> bytes = stackalloc byte[16];
        value.TryWriteBytes(bytes, bigEndian: true, out _);
        writer.WriteBytes(bytes);
    }

#if !NET8_0_OR_GREATER
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
#endif
}
