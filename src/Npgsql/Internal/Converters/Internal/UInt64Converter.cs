// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

sealed class UInt64Converter : PgBufferedConverter<ulong>
{
    public override ConverterDescriptor GetDescriptor(in ConversionContext context)
        => new() { BufferRequirements = BufferRequirements.CreateFixedSize(sizeof(ulong)) };
    public override ulong Read(PgReader reader) => reader.ReadUInt64();
    public override void Write(PgWriter writer, ulong value) => writer.WriteUInt64(value);
}
