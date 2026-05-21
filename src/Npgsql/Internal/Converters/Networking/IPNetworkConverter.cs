using System;
using System.Net;

// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

sealed class IPNetworkConverter : PgBufferedConverter<IPNetwork>
{
    public override ConverterDescriptor GetDescriptor(in DescriptorContext context)
        => NpgsqlInetConverter.GetDescriptorImpl(context);

    protected override Size BindValue(in BindContext context, IPNetwork value, ref object? writeState)
        => NpgsqlInetConverter.BindValueImpl(context, value.BaseAddress, ref writeState);

    public override IPNetwork Read(PgReader reader)
    {
        var (ip, netmask) = NpgsqlInetConverter.ReadImpl(reader, shouldBeCidr: true);
        return new(ip, netmask);
    }

    public override void Write(PgWriter writer, IPNetwork value)
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
