﻿#region License
// The PostgreSQL License
//
// Copyright (C) 2018 The Npgsql Development Team
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
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using JetBrains.Annotations;
using Npgsql.BackendMessages;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.NetworkHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-net-types.html
    /// </remarks>
    [TypeMapping("inet", NpgsqlDbType.Inet, new[] { typeof(NpgsqlInet), typeof(IPAddress) })]
    class InetHandler : NpgsqlSimpleTypeHandlerWithPsv<IPAddress, NpgsqlInet>, INpgsqlSimpleTypeHandler<string>
    {
        // ReSharper disable InconsistentNaming
        const byte IPv4 = 2;
        const byte IPv6 = 3;
        // ReSharper restore InconsistentNaming

        #region Read

        public override IPAddress Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
            => ((INpgsqlSimpleTypeHandler<NpgsqlInet>)this).Read(buf, len, fieldDescription).Address;

#pragma warning disable CA1801 // Review unused parameters
        internal static NpgsqlInet DoRead(NpgsqlReadBuffer buf, [CanBeNull] FieldDescription fieldDescription, int len, bool isCidrHandler)
        {
            buf.ReadByte();  // addressFamily
            var mask = buf.ReadByte();
            var isCidr = buf.ReadByte() == 1;
            Debug.Assert(isCidrHandler == isCidr);
            var numBytes = buf.ReadByte();
            var bytes = new byte[numBytes];
            for (var i = 0; i < numBytes; i++) {
                bytes[i] = buf.ReadByte();
            }
            return new NpgsqlInet(new IPAddress(bytes), mask);
        }
#pragma warning restore CA1801 // Review unused parameters

        protected override NpgsqlInet ReadPsv(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
            => DoRead(buf, fieldDescription, len, false);

        string INpgsqlSimpleTypeHandler<string>.Read(NpgsqlReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => ((INpgsqlSimpleTypeHandler<NpgsqlInet>)this).Read(buf, len, fieldDescription).ToString();

        #endregion Read

        #region Write

        public override int ValidateAndGetLength(IPAddress value, NpgsqlParameter parameter)
            => GetLength(value);

        public override int ValidateAndGetLength(NpgsqlInet value, NpgsqlParameter parameter)
            => GetLength(value.Address);

        public int ValidateAndGetLength(string value, NpgsqlParameter parameter)
            => GetLength(IPAddress.Parse(value));

        public override void Write(IPAddress value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => DoWrite(value, -1, buf, false);

        public override void Write(NpgsqlInet value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => DoWrite(value.Address, value.Netmask, buf, false);

        public void Write(string value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => DoWrite(IPAddress.Parse(value), -1, buf, false);

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
        {
            switch (value.AddressFamily)
            {
            case AddressFamily.InterNetwork:
                return 8;
            case AddressFamily.InterNetworkV6:
                return 20;
            default:
                throw new InvalidCastException($"Can't handle IPAddress with AddressFamily {value.AddressFamily}, only InterNetwork or InterNetworkV6!");
            }
        }

        #endregion Write
    }
}
