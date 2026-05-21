using NpgsqlTypes;

// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

sealed class LineConverter : PgBufferedConverter<NpgsqlLine>
{
    public override ConverterDescriptor GetDescriptor(in ConversionContext context)
        => new() { BufferRequirements = BufferRequirements.CreateFixedSize(sizeof(double) * 3) };

    public override NpgsqlLine Read(PgReader reader)
        => new(reader.ReadDouble(), reader.ReadDouble(), reader.ReadDouble());

    public override void Write(PgWriter writer, NpgsqlLine value)
    {
        writer.WriteDouble(value.A);
        writer.WriteDouble(value.B);
        writer.WriteDouble(value.C);
    }
}
