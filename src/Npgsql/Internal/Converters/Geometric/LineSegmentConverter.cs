using NpgsqlTypes;

// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

public class LineSegmentConverter : PgBufferedConverter<NpgsqlLSeg>
{
    public override bool CanConvert(DataFormat format, out BufferingRequirement bufferingRequirement)
    {
        bufferingRequirement = BufferingRequirement.FixedSize;
        return base.CanConvert(format, out _);
    }

    public override Size GetSize(SizeContext context, NpgsqlLSeg value, ref object? writeState)
        => sizeof(double) * 4;

    protected override NpgsqlLSeg ReadCore(PgReader reader)
        => new(reader.ReadDouble(), reader.ReadDouble(), reader.ReadDouble(), reader.ReadDouble());

    protected override void WriteCore(PgWriter writer, NpgsqlLSeg value)
    {
        writer.WriteDouble(value.Start.X);
        writer.WriteDouble(value.Start.Y);
        writer.WriteDouble(value.End.X);
        writer.WriteDouble(value.End.Y);
    }
}
