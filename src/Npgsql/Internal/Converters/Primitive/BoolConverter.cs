// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

sealed class BoolConverter : PgBufferedConverter<bool>
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(byte));
        return format is DataFormat.Binary;
    }
    protected override bool ReadCore(PgReader reader) => reader.ReadByte() is not 0;
    protected override void WriteCore(PgWriter writer, bool value) => writer.WriteByte((byte)(value ? 1 : 0));
}
