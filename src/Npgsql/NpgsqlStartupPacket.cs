// created on 9/6/2002 at 16:56

// Npgsql.NpgsqlStartupPacket.cs
//
// Author:
//    Francisco Jr. (fxjrlists@yahoo.com.br)
//
//    Copyright (C) 2002 The Npgsql Development Team
//    npgsql-general@gborg.postgresql.org
//    http://gborg.postgresql.org/project/npgsql/projdisplay.php
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

using System;
using System.IO;
using System.Net;
using System.Reflection;

namespace Npgsql
{
    /// <summary>
    /// This class represents a StartupPacket message of PostgreSQL
    /// protocol.
    /// </summary>
    ///
    internal sealed class NpgsqlStartupPacket : ClientMessage
    {
        // Logging related values
        private static readonly String CLASSNAME = MethodBase.GetCurrentMethod().DeclaringType.Name;

        // Private fields.
        private readonly Int32 packet_size;
        private readonly ProtocolVersion protocol_version;
        private readonly byte[] database_name;
        private readonly byte[] user_name;
        private readonly byte[] arguments;
        private readonly byte[] unused;
        private readonly byte[] optional_tty;

        public NpgsqlStartupPacket(Int32 packet_size, ProtocolVersion protocol_version, String database_name, String user_name,
                                   String arguments, String unused, String optional_tty)
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, CLASSNAME);
            // Just copy the values.

            // [FIXME] Validate params? We are the only clients, so, hopefully, we
            // know what to send.

            this.packet_size = packet_size;
            this.protocol_version = protocol_version;

            this.database_name = BackendEncoding.UTF8Encoding.GetBytes(database_name);
            this.user_name = BackendEncoding.UTF8Encoding.GetBytes(user_name);
            this.arguments = BackendEncoding.UTF8Encoding.GetBytes(arguments);
            this.unused = BackendEncoding.UTF8Encoding.GetBytes(unused);
            this.optional_tty = BackendEncoding.UTF8Encoding.GetBytes(optional_tty);
        }

        public override void WriteToStream(Stream output_stream)
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "WriteToStream");

            switch (protocol_version)
            {
                case ProtocolVersion.Version2:
                    WriteToStream_Ver_2(output_stream);
                    break;

                case ProtocolVersion.Version3:
                    WriteToStream_Ver_3(output_stream);
                    break;
            }
        }

        private void WriteToStream_Ver_2(Stream output_stream)
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "WriteToStream_Ver_2");

            // Packet length = 296
            output_stream
                .WriteBytes(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(this.packet_size)))
                .WriteBytes(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(PGUtil.ConvertProtocolVersion(this.protocol_version))))
                .WriteLimBytes(database_name, 64)
                .WriteLimBytes(user_name, 32)
                .WriteLimBytes(arguments, 64)
                .WriteLimBytes(unused, 64)
                .WriteLimBytes(optional_tty, 64)
                .Flush();
        }

        private void WriteToStream_Ver_3(Stream output_stream)
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "WriteToStream_Ver_3");

            output_stream
                .WriteInt32(4 + 4 + 5 + user_name.Length + 1 + 9 + database_name.Length + 1 + 10 + 4 + 1)
                .WriteInt32(PGUtil.ConvertProtocolVersion(this.protocol_version))
                .WriteStringNullTerminated("user")
                .WriteBytesNullTerminated(user_name)
                .WriteStringNullTerminated("database")
                .WriteBytesNullTerminated(database_name)
                .WriteStringNullTerminated("DateStyle")
                .WriteStringNullTerminated("ISO")
                .WriteBytes(0)
                .Flush();
        }
    }
}
