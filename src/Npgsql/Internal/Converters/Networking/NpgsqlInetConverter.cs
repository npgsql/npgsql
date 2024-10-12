using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using NpgsqlTypes;

// ReSharper disable once CheckNamespace
namespace Npgsql.Internal.Converters;

sealed class NpgsqlInetConverter : PgBufferedConverter<NpgsqlInet>
{
    const byte IPv4 = 2;
    const byte IPv6 = 3;

    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
        => CanConvertImpl(format, out bufferRequirements);

    internal static bool CanConvertImpl(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.Create(Size.CreateUpperBound(20));
        return format == DataFormat.Binary;
    }

    public override Size GetSize(SizeContext context, NpgsqlInet value, ref object? writeState)
        => GetSizeImpl(context, value.Address, ref writeState);

    internal static Size GetSizeImpl(SizeContext context, IPAddress ipAddress, ref object? writeState)
        => ipAddress.AddressFamily switch
        {
            AddressFamily.InterNetwork => 8,
            AddressFamily.InterNetworkV6 => 20,
            _ => throw new InvalidCastException(
                $"Can't handle IPAddress with AddressFamily {ipAddress.AddressFamily}, only InterNetwork or InterNetworkV6!")
        };

    protected override NpgsqlInet ReadCore(PgReader reader)
    {
        var (ip, netmask) = ReadImpl(reader, shouldBeCidr: false);
        return new(ip, netmask);
    }

    internal static (IPAddress Address, byte Netmask) ReadImpl(PgReader reader, bool shouldBeCidr)
    {
        _ = reader.ReadByte(); // addressFamily
        var mask = reader.ReadByte(); // mask

        var isCidr = reader.ReadByte() == 1;
        Debug.Assert(isCidr == shouldBeCidr);

        var numBytes = reader.ReadByte();
        Span<byte> bytes = stackalloc byte[numBytes];
        reader.Read(bytes);
        return (new IPAddress(bytes), mask);
    }

    protected override void WriteCore(PgWriter writer, NpgsqlInet value)
        => WriteImpl(writer, (value.Address, value.Netmask), isCidr: false);

    internal static void WriteImpl(PgWriter writer, (IPAddress Address, byte Netmask) value, bool isCidr)
    {
        writer.WriteByte(value.Address.AddressFamily switch
        {
            AddressFamily.InterNetwork => IPv4,
            AddressFamily.InterNetworkV6 => IPv6,
            _ => throw new InvalidCastException(
                $"Can't handle IPAddress with AddressFamily {value.Address.AddressFamily}, only InterNetwork or InterNetworkV6!")
        });

        writer.WriteByte(value.Netmask);
        writer.WriteByte((byte)(isCidr ? 1 : 0));  // Ignored on server side
        var bytes = value.Address.GetAddressBytes();
        writer.WriteByte((byte)bytes.Length);
        writer.WriteBytes(bytes);
    }
}
