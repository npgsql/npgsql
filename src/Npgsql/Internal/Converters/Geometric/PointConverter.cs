using NpgsqlTypes;

// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

sealed class PointConverter : PgBufferedConverter<NpgsqlPoint>
{
    public override ConverterDescriptor GetDescriptor(in DescriptorContext context)
        => ConverterDescriptor.Invariant with { BufferRequirements = BufferRequirements.CreateFixedSize(sizeof(double) * 2) };

    public override NpgsqlPoint Read(PgReader reader)
        => new(reader.ReadDouble(), reader.ReadDouble());

    public override void Write(PgWriter writer, NpgsqlPoint value)
    {
        writer.WriteDouble(value.X);
        writer.WriteDouble(value.Y);
    }
}
