using NpgsqlTypes;

// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

sealed class BoxConverter : PgBufferedConverter<NpgsqlBox>
{
    public override bool CanConvert(DataFormat format, out BufferingRequirement bufferingRequirement)
    {
        bufferingRequirement = BufferingRequirement.FixedSize;
        return base.CanConvert(format, out _);
    }

    public override Size GetSize(SizeContext context, NpgsqlBox value, ref object? writeState)
        => sizeof(double) * 4;

    protected override NpgsqlBox ReadCore(PgReader reader)
        => new(
            new NpgsqlPoint(reader.ReadDouble(), reader.ReadDouble()),
            new NpgsqlPoint(reader.ReadDouble(), reader.ReadDouble()));

    protected override void WriteCore(PgWriter writer, NpgsqlBox value)
    {
        writer.WriteDouble(value.Right);
        writer.WriteDouble(value.Top);
        writer.WriteDouble(value.Left);
        writer.WriteDouble(value.Bottom);
    }
}
