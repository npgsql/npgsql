namespace Npgsql.Internal.Converters;

sealed class UInt64Converter : PgBufferedConverter<ulong>
{
    public override bool CanConvert(DataFormat format, out BufferingRequirement bufferingRequirement)
    {
        bufferingRequirement = BufferingRequirement.FixedSize;
        return base.CanConvert(format, out _);
    }
    public override Size GetSize(SizeContext context, ulong value, ref object? writeState) => sizeof(ulong);
    protected override ulong ReadCore(PgReader reader) => reader.ReadUInt64();
    protected override void WriteCore(PgWriter writer, ulong value) => writer.WriteUInt64(value);
}
