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
}
