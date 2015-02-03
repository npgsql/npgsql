using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Npgsql.BackendMessages;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.NetworkHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/9.4/static/datatype-net-types.html
    /// </remarks>
    [TypeMapping("inet", NpgsqlDbType.Inet, new[] { typeof(NpgsqlInet), typeof(IPAddress) })]
    internal class InetHandler : TypeHandlerWithPsv<IPAddress, NpgsqlInet>,
        ISimpleTypeReader<IPAddress>, ISimpleTypeReader<NpgsqlInet>, ISimpleTypeWriter,
        ISimpleTypeReader<string>
    {
        const byte IPv4 = 2;
        const byte IPv6 = 3;

        public IPAddress Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return ((ISimpleTypeReader<NpgsqlInet>)this).Read(buf, fieldDescription, len).Address;
        }

        static internal NpgsqlInet DoRead(NpgsqlBuffer buf, FieldDescription fieldDescription, int len, bool isCidrHandler)
        {
            var addressFamily = buf.ReadByte();
            var mask = buf.ReadByte();
            var isCidr = buf.ReadByte() == 1;
            Contract.Assume(isCidrHandler == isCidr);
            var numBytes = buf.ReadByte();
            var bytes = new byte[numBytes];
            for (var i = 0; i < numBytes; i++) {
                bytes[i] = buf.ReadByte();
            }
            return new NpgsqlInet(new IPAddress(bytes), mask);            
        }

        NpgsqlInet ISimpleTypeReader<NpgsqlInet>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return DoRead(buf, fieldDescription, len, false);
        }

        string ISimpleTypeReader<string>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return ((ISimpleTypeReader<NpgsqlInet>)this).Read(buf, fieldDescription, len).ToString();
        }

        static internal int DoValidateAndGetLength(object value)
        {
            IPAddress ip;
            if (value is NpgsqlInet) {
                ip = ((NpgsqlInet)value).Address;
            } else {
                ip = value as IPAddress;
                if (ip == null) {
                    throw new InvalidCastException(String.Format("Can't send type {0} as inet", value.GetType()));
                }
            }

            switch (ip.AddressFamily) {
            case AddressFamily.InterNetwork:
                return 8;
            case AddressFamily.InterNetworkV6:
                return 20;
            default:
                throw new InvalidCastException(String.Format("Can't handle IPAddress with AddressFamily {0}, only InterNetwork or InterNetworkV6!", ip.AddressFamily));
            }            
        }

        public int ValidateAndGetLength(object value)
        {
            return DoValidateAndGetLength(value);
        }

        internal static void DoWrite(object value, NpgsqlBuffer buf, bool isCidrHandler)
        {
            IPAddress ip;
            int mask;
            if (value is NpgsqlInet) {
                var inet = ((NpgsqlInet)value);
                ip = inet.Address;
                mask = inet.Netmask;
            } else {
                ip = value as IPAddress;
                if (ip == null) {
                    throw new InvalidCastException(String.Format("Can't send type {0} as inet", value.GetType()));
                }
                mask = -1;
            }

            switch (ip.AddressFamily) {
            case AddressFamily.InterNetwork:
                buf.WriteByte(IPv4);
                if (mask == -1) {
                    mask = 32;
                }
                break;
            case AddressFamily.InterNetworkV6:
                buf.WriteByte(IPv6);
                if (mask == -1) {
                    mask = 128;
                }
                break;
            default:
                throw new InvalidCastException(String.Format("Can't handle IPAddress with AddressFamily {0}, only InterNetwork or InterNetworkV6!", ip.AddressFamily));
            }

            buf.WriteByte((byte)mask);
            buf.WriteByte((byte)(isCidrHandler ? 1 : 0));  // Ignored on server side
            var bytes = ip.GetAddressBytes();
            buf.WriteByte((byte)bytes.Length);
            buf.WriteBytesSimple(bytes, 0, bytes.Length);            
        }

        public void Write(object value, NpgsqlBuffer buf)
        {
            DoWrite(value, buf, false);
        }
    }
}
