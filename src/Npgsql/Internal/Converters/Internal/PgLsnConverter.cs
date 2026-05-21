using NpgsqlTypes;

// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

sealed class PgLsnConverter : PgBufferedConverter<NpgsqlLogSequenceNumber>
{
    public override ConverterDescriptor GetDescriptor(in ConversionContext context)
        => new() { BufferRequirements = BufferRequirements.CreateFixedSize(sizeof(ulong)) };
    public override NpgsqlLogSequenceNumber Read(PgReader reader) => new(reader.ReadUInt64());
    public override void Write(PgWriter writer, NpgsqlLogSequenceNumber value) => writer.WriteUInt64((ulong)value);
}
