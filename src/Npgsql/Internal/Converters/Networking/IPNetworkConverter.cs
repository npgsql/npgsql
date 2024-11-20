using System.Net;

// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

sealed class IPNetworkConverter : PgBufferedConverter<IPNetwork>
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
        => CanConvertBufferedDefault(format, out bufferRequirements);

    public override Size GetSize(SizeContext context, IPNetwork value, ref object? writeState)
        => NpgsqlInetConverter.GetSizeImpl(context, value.BaseAddress, ref writeState);

    protected override IPNetwork ReadCore(PgReader reader)
    {
        var (ip, netmask) = NpgsqlInetConverter.ReadImpl(reader, shouldBeCidr: true);
        return new(ip, netmask);
    }

    protected override void WriteCore(PgWriter writer, IPNetwork value)
        => NpgsqlInetConverter.WriteImpl(writer, (value.BaseAddress, (byte)value.PrefixLength), isCidr: true);
}
