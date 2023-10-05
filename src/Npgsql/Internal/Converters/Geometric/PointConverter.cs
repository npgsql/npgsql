using NpgsqlTypes;

// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

sealed class PointConverter : PgBufferedConverter<NpgsqlPoint>
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(double) * 2);
        return format is DataFormat.Binary;
    }

    protected override NpgsqlPoint ReadCore(PgReader reader)
        => new(reader.ReadDouble(), reader.ReadDouble());

    protected override void WriteCore(PgWriter writer, NpgsqlPoint value)
    {
        writer.WriteDouble(value.X);
        writer.WriteDouble(value.Y);
    }
}
