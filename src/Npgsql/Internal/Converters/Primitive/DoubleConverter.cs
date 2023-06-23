namespace Npgsql.Internal.Converters;

sealed class DoubleConverter : PgBufferedConverter<double>
{
    public override bool CanConvert(DataFormat format, out BufferingRequirement bufferingRequirement)
    {
        bufferingRequirement = BufferingRequirement.FixedSize;
        return base.CanConvert(format, out _);
    }

    public override Size GetSize(SizeContext context, double value, ref object? writeState) => sizeof(double);

    protected override double ReadCore(PgReader reader) => reader.ReadDouble();
    protected override void WriteCore(PgWriter writer, double value) => writer.WriteDouble(value);
}
