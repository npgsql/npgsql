namespace Npgsql.Internal.Converters;

sealed class UInt32Converter : PgBufferedConverter<uint>
{
    public override bool CanConvert(DataFormat format, out BufferingRequirement bufferingRequirement)
    {
        bufferingRequirement = BufferingRequirement.FixedSize;
        return base.CanConvert(format, out _);
    }
    public override Size GetSize(SizeContext context, uint value, ref object? writeState) => sizeof(uint);
    protected override uint ReadCore(PgReader reader) => reader.ReadUInt32();
    protected override void WriteCore(PgWriter writer, uint value) => writer.WriteUInt32(value);
}
