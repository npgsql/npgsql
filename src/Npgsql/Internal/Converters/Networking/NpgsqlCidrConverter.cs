using NpgsqlTypes;

// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

sealed class NpgsqlCidrConverter : PgBufferedConverter<NpgsqlCidr>
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
        => NpgsqlInetConverter.CanConvertImpl(format, out bufferRequirements);

    public override Size GetSize(SizeContext context, NpgsqlCidr value, ref object? writeState)
        => NpgsqlInetConverter.GetSizeImpl(context, value.Address, ref writeState);

    protected override NpgsqlCidr ReadCore(PgReader reader)
    {
        var (ip, netmask) = NpgsqlInetConverter.ReadImpl(reader, shouldBeCidr: true);
        return new(ip, netmask);
    }

    protected override void WriteCore(PgWriter writer, NpgsqlCidr value)
        => NpgsqlInetConverter.WriteImpl(writer, (value.Address, value.Netmask), isCidr: true);
}
