using NpgsqlTypes;

// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

sealed class BoxConverter : PgBufferedConverter<NpgsqlBox>
{
    public override ConverterDescriptor GetDescriptor(in ConversionContext context)
        => new() { BufferRequirements = BufferRequirements.CreateFixedSize(sizeof(double) * 4) };

    public override NpgsqlBox Read(PgReader reader)
        => new(
            new NpgsqlPoint(reader.ReadDouble(), reader.ReadDouble()),
            new NpgsqlPoint(reader.ReadDouble(), reader.ReadDouble()));

    public override void Write(PgWriter writer, NpgsqlBox value)
    {
        writer.WriteDouble(value.Right);
        writer.WriteDouble(value.Top);
        writer.WriteDouble(value.Left);
        writer.WriteDouble(value.Bottom);
    }
}
