using NpgsqlTypes;

// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

sealed class LineSegmentConverter : PgBufferedConverter<NpgsqlLSeg>
{
    public override ConverterDescriptor GetDescriptor(in DescriptorContext context)
        => ConverterDescriptor.Invariant with { BufferRequirements = BufferRequirements.CreateFixedSize(sizeof(double) * 4) };

    public override NpgsqlLSeg Read(PgReader reader)
        => new(reader.ReadDouble(), reader.ReadDouble(), reader.ReadDouble(), reader.ReadDouble());

    public override void Write(PgWriter writer, NpgsqlLSeg value)
    {
        writer.WriteDouble(value.Start.X);
        writer.WriteDouble(value.Start.Y);
        writer.WriteDouble(value.End.X);
        writer.WriteDouble(value.End.Y);
    }
}
