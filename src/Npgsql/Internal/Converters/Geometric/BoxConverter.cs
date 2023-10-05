using NpgsqlTypes;

// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

sealed class BoxConverter : PgBufferedConverter<NpgsqlBox>
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(double) * 4);
        return format is DataFormat.Binary;
    }

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
