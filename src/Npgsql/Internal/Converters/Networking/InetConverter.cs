using System;
using System.Buffers;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Npgsql.Internal.Converters;

sealed class InetConverter : PgBufferedConverter<IPAddress>
{
    const byte IPv4 = 2;
    const byte IPv6 = 3;

    public override bool CanConvert(DataFormat format, out BufferingRequirement bufferingRequirement)
    {
        bufferingRequirement = BufferingRequirement.FixedSize;
        return base.CanConvert(format, out _);
    }

    public override Size GetSize(SizeContext context, IPAddress value, ref object? writeState)
        => value is null // TODO: Remove this
            ? Size.Zero
            : value.AddressFamily switch
            {
                AddressFamily.InterNetwork => 8,
                AddressFamily.InterNetworkV6 => 20,
                _ => throw new InvalidCastException(
                    $"Can't handle IPAddress with AddressFamily {value.AddressFamily}, only InterNetwork or InterNetworkV6!")
            };

    protected override IPAddress ReadCore(PgReader reader)
    {
        var addressFamily = reader.ReadByte();
        Debug.Assert((AddressFamily)addressFamily is AddressFamily.InterNetwork or AddressFamily.InterNetworkV6);

        _ = reader.ReadByte(); // mask

        var isCidr = reader.ReadByte() == 1;
        Debug.Assert(!isCidr);

        var numBytes = reader.ReadByte();
        Span<byte> bytes = stackalloc byte[numBytes];
        reader.CopyTo(bytes);
        return new IPAddress(bytes);
    }

    protected override void WriteCore(PgWriter writer, IPAddress value)
    {
        switch (value.AddressFamily) {
        case AddressFamily.InterNetwork:
            writer.WriteByte(IPv4);
            writer.WriteByte(32); // mask
            break;
        case AddressFamily.InterNetworkV6:
            writer.WriteByte(IPv6);
            writer.WriteByte(128); // mask
            break;
        default:
            throw new InvalidCastException($"Can't handle IPAddress with AddressFamily {value.AddressFamily}, only InterNetwork or InterNetworkV6!");
        }

        writer.WriteByte(0);  // is_cidr, ignored on server side
        var bytes = value.GetAddressBytes();
        writer.WriteByte((byte)bytes.Length);
        writer.WriteRaw(new ReadOnlySequence<byte>(bytes));
    }
}
