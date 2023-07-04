using NpgsqlTypes;

// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

sealed class LineConverter : PgBufferedConverter<NpgsqlLine>
{
    public override bool CanConvert(DataFormat format, out BufferingRequirement bufferingRequirement)
    {
        bufferingRequirement = BufferingRequirement.FixedSize;
        return base.CanConvert(format, out _);
    }

    public override Size GetSize(SizeContext context, NpgsqlLine value, ref object? writeState)
        => sizeof(double) * 3;

    protected override NpgsqlLine ReadCore(PgReader reader)
        => new(reader.ReadDouble(), reader.ReadDouble(), reader.ReadDouble());

    protected override void WriteCore(PgWriter writer, NpgsqlLine value)
    {
        writer.WriteDouble(value.A);
        writer.WriteDouble(value.B);
        writer.WriteDouble(value.C);
    }
}
