// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

sealed class UInt64Converter : PgBufferedConverter<ulong>
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(ulong));
        return format is DataFormat.Binary;
    }
    protected override ulong ReadCore(PgReader reader) => reader.ReadUInt64();
    protected override void WriteCore(PgWriter writer, ulong value) => writer.WriteUInt64(value);
}
