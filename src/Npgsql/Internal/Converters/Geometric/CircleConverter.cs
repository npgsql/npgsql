using NpgsqlTypes;

// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

sealed class CircleConverter : PgBufferedConverter<NpgsqlCircle>
{
    public override ConverterDescriptor GetDescriptor(in DescriptorContext context)
        => new() { BufferRequirements = BufferRequirements.CreateFixedSize(sizeof(double) * 3) };

    public override NpgsqlCircle Read(PgReader reader)
        => new(reader.ReadDouble(), reader.ReadDouble(), reader.ReadDouble());

    public override void Write(PgWriter writer, NpgsqlCircle value)
    {
        writer.WriteDouble(value.X);
        writer.WriteDouble(value.Y);
        writer.WriteDouble(value.Radius);
    }
}
