using System;
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
        => NpgsqlInetConverter.WriteImpl(
            writer,
            (
                value.BaseAddress,
                value.PrefixLength <= byte.MaxValue
                    ? (byte)value.PrefixLength
                    : throw new ArgumentOutOfRangeException(nameof(value), "IPNetwork.PrefixLength is too large to fit in a byte")
            ),
            isCidr: true);
}
