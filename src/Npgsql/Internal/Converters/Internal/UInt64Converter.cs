// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

sealed class UInt64Converter : PgBufferedConverter<ulong>
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(ulong));
        return format is DataFormat.Binary;
    }
    public override ulong Read(PgReader reader) => reader.ReadUInt64();
    public override void Write(PgWriter writer, ulong value) => writer.WriteUInt64(value);
}
