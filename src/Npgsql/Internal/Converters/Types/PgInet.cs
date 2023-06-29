using System;
using System.Diagnostics;
using System.Net;
using Npgsql.BackendMessages;

namespace Npgsql.Internal.Converters.Types;

static class PgInet
{
    internal static (IPAddress Address, int Subnet) Decode(
        NpgsqlReadBuffer buf,
        bool shouldBeCidr)
    {
        buf.ReadByte(); // addressFamily
        var mask = buf.ReadByte();
        var isCidr = buf.ReadByte() == 1;
        Debug.Assert(shouldBeCidr == isCidr);

        var numBytes = buf.ReadByte();
        Span<byte> bytes = stackalloc byte[numBytes];
        for (var i = 0; i < bytes.Length; i++)
            bytes[i] = buf.ReadByte();

        return (new IPAddress(bytes), mask);
    }
}
