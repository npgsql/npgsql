using NpgsqlTypes;

// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

sealed class LineConverter : PgBufferedConverter<NpgsqlLine>
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.CreateFixedSize(sizeof(double) * 3);
        return format is DataFormat.Binary;
    }

    public override NpgsqlLine Read(PgReader reader)
        => new(reader.ReadDouble(), reader.ReadDouble(), reader.ReadDouble());

    public override void Write(PgWriter writer, NpgsqlLine value)
    {
        writer.WriteDouble(value.A);
        writer.WriteDouble(value.B);
        writer.WriteDouble(value.C);
    }
}
