using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Npgsql.BackendMessages;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using NpgsqlTypes;

#pragma warning disable 618

namespace Npgsql.Internal.TypeHandlers.NetworkHandlers
{
    /// <summary>
    /// A type handler for the PostgreSQL cidr data type.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/datatype-net-types.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    public partial class InetHandler : NpgsqlSimpleTypeHandlerWithPsv<IPAddress, (IPAddress Address, int Subnet)>,
        INpgsqlSimpleTypeHandler<NpgsqlInet>
    {
        // ReSharper disable InconsistentNaming
        const byte IPv4 = 2;
        const byte IPv6 = 3;
        // ReSharper restore InconsistentNaming

        public InetHandler(PostgresType pgType) : base(pgType) {}

        #region Read

        /// <inheritdoc />
        public override IPAddress Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
            => DoRead(buf, len, fieldDescription, false).Address;

#pragma warning disable CA1801 // Review unused parameters
        internal static (IPAddress Address, int Subnet) DoRead(
            NpgsqlReadBuffer buf,
            int len,
            FieldDescription? fieldDescription,
            bool isCidrHandler)
        {
            buf.ReadByte(); // addressFamily
            var mask = buf.ReadByte();
            var isCidr = buf.ReadByte() == 1;
            Debug.Assert(isCidrHandler == isCidr);
            var numBytes = buf.ReadByte();
            var bytes = new byte[numBytes];
            for (var i = 0; i < numBytes; i++)
                bytes[i] = buf.ReadByte();

            return (new IPAddress(bytes), mask);
        }
#pragma warning restore CA1801 // Review unused parameters

        /// <inheritdoc />
        protected override (IPAddress Address, int Subnet) ReadPsv(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
            => DoRead(buf, len, fieldDescription, false);

        NpgsqlInet INpgsqlSimpleTypeHandler<NpgsqlInet>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
        {
            var (address, subnet) = DoRead(buf, len, fieldDescription, false);
            return new NpgsqlInet(address, subnet);
        }

        #endregion Read

        #region Write

        /// <inheritdoc />
        public override int ValidateAndGetLength(IPAddress value, NpgsqlParameter? parameter)
            => GetLength(value);

        /// <inheritdoc />
        public override int ValidateAndGetLength((IPAddress Address, int Subnet) value, NpgsqlParameter? parameter)
            => GetLength(value.Address);

        /// <inheritdoc />
        public int ValidateAndGetLength(NpgsqlInet value, NpgsqlParameter? parameter)
            => GetLength(value.Address);

        /// <inheritdoc />
        public override void Write(IPAddress value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
            => DoWrite(value, -1, buf, false);

        /// <inheritdoc />
        public override void Write((IPAddress Address, int Subnet) value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
            => DoWrite(value.Address, value.Subnet, buf, false);

        /// <inheritdoc />
        public void Write(NpgsqlInet value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
            => DoWrite(value.Address, value.Netmask, buf, false);

        internal static void DoWrite(IPAddress ip, int mask, NpgsqlWriteBuffer buf, bool isCidrHandler)
        {
            switch (ip.AddressFamily) {
            case AddressFamily.InterNetwork:
                buf.WriteByte(IPv4);
                if (mask == -1)
                    mask = 32;
                break;
            case AddressFamily.InterNetworkV6:
                buf.WriteByte(IPv6);
                if (mask == -1)
                    mask = 128;
                break;
            default:
                throw new InvalidCastException($"Can't handle IPAddress with AddressFamily {ip.AddressFamily}, only InterNetwork or InterNetworkV6!");
            }

            buf.WriteByte((byte)mask);
            buf.WriteByte((byte)(isCidrHandler ? 1 : 0));  // Ignored on server side
            var bytes = ip.GetAddressBytes();
            buf.WriteByte((byte)bytes.Length);
            buf.WriteBytes(bytes, 0, bytes.Length);
        }

        internal static int GetLength(IPAddress value)
            => value.AddressFamily switch
            {
                AddressFamily.InterNetwork   => 8,
                AddressFamily.InterNetworkV6 => 20,
                _ => throw new InvalidCastException($"Can't handle IPAddress with AddressFamily {value.AddressFamily}, only InterNetwork or InterNetworkV6!")
            };

        #endregion Write
    }
}
