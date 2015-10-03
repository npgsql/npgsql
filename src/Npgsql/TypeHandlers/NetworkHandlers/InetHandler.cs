#region License
// The PostgreSQL License
//
// Copyright (C) 2015 The Npgsql Development Team
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
#endregion

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
    /// http://www.postgresql.org/docs/current/static/datatype-net-types.html
    /// </remarks>
    [TypeMapping("inet", NpgsqlDbType.Inet, new[] { typeof(NpgsqlInet), typeof(IPAddress) })]
    internal class InetHandler : SimpleTypeHandlerWithPsv<IPAddress, NpgsqlInet>, ISimpleTypeHandler<string>
    {
        const byte IPv4 = 2;
        const byte IPv6 = 3;

        public override IPAddress Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            return ((ISimpleTypeHandler<NpgsqlInet>)this).Read(buf, len, fieldDescription).Address;
        }

        static internal NpgsqlInet DoRead(NpgsqlBuffer buf, FieldDescription fieldDescription, int len, bool isCidrHandler)
        {
            buf.ReadByte();  // addressFamily
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

        internal override NpgsqlInet ReadPsv(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            return DoRead(buf, fieldDescription, len, false);
        }

        string ISimpleTypeHandler<string>.Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            return ((ISimpleTypeHandler<NpgsqlInet>)this).Read(buf, len, fieldDescription).ToString();
        }

        static internal int DoValidateAndGetLength(object value)
        {
            IPAddress ip;
            if (value is NpgsqlInet) {
                ip = ((NpgsqlInet)value).Address;
            } else {
                ip = value as IPAddress;
                if (ip == null) {
                    throw new InvalidCastException($"Can't send type {value.GetType()} as inet");
                }
            }

            switch (ip.AddressFamily) {
            case AddressFamily.InterNetwork:
                return 8;
            case AddressFamily.InterNetworkV6:
                return 20;
            default:
                throw new InvalidCastException(
                    $"Can't handle IPAddress with AddressFamily {ip.AddressFamily}, only InterNetwork or InterNetworkV6!");
            }
        }

        public override int ValidateAndGetLength(object value, NpgsqlParameter parameter)
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
                    throw new InvalidCastException($"Can't send type {value.GetType()} as inet");
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
                throw new InvalidCastException(
                    $"Can't handle IPAddress with AddressFamily {ip.AddressFamily}, only InterNetwork or InterNetworkV6!");
            }

            buf.WriteByte((byte)mask);
            buf.WriteByte((byte)(isCidrHandler ? 1 : 0));  // Ignored on server side
            var bytes = ip.GetAddressBytes();
            buf.WriteByte((byte)bytes.Length);
            buf.WriteBytes(bytes, 0, bytes.Length);
        }

        public override void Write(object value, NpgsqlBuffer buf, NpgsqlParameter parameter)
        {
            DoWrite(value, buf, false);
        }
    }
}
