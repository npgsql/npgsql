using NpgsqlTypes;

// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

sealed class LineSegmentConverter : PgBufferedConverter<NpgsqlLSeg>
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(double) * 4);
        return format is DataFormat.Binary;
    }

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
