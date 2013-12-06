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
using System.Collections.Generic;

namespace Npgsql
{
    /// <summary>
    /// This class represents a StartupPacket message of PostgreSQL
    /// protocol.
    /// </summary>
    ///
    internal abstract class NpgsqlStartupPacket : ClientMessage
    {
        // Logging related values
        private static readonly String CLASSNAME = MethodBase.GetCurrentMethod().DeclaringType.Name;

        public static NpgsqlStartupPacket BuildStartupPacket(ProtocolVersion protocol_version, String database_name, String user_name,
                                                             NpgsqlConnectionStringBuilder  settings)
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "BuildStartupPacket");

            if (protocol_version == ProtocolVersion.Version2)
            {
                return new NpgsqlStartupPacketV2(database_name,user_name, "", "", "");
            }
            else
            {
                Dictionary<String, String> parameters = new Dictionary<String, String>();

                parameters.Add("DateStyle", "ISO");
                parameters.Add("client_encoding", "UTF8");
                parameters.Add("extra_float_digits", "2");
                parameters.Add("lc_monetary", "C");

                if (!string.IsNullOrEmpty(settings.ApplicationName))
                {
                    parameters.Add("application_name", settings.ApplicationName);
                }

                if (!string.IsNullOrEmpty(settings.SearchPath))
                {
                    parameters.Add("search_path", settings.SearchPath);
                }

                return new NpgsqlStartupPacketV3(database_name,user_name,parameters);
            }
        }

        protected NpgsqlStartupPacket() { }
    }

    internal sealed class NpgsqlStartupPacketV2 : NpgsqlStartupPacket
    {
        // Logging related values
        private static readonly String CLASSNAME = MethodBase.GetCurrentMethod().DeclaringType.Name;

        // Private fields.
        private readonly ProtocolVersion protocol_version;
        private readonly byte[] database_name;
        private readonly byte[] user_name;
        private readonly byte[] arguments;
        private readonly byte[] unused;
        private readonly byte[] optional_tty;

        public NpgsqlStartupPacketV2(String database_name, String user_name,
                                   String arguments, String unused, String optional_tty)
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, CLASSNAME);
            // Just copy the values.

            // [FIXME] Validate params? We are the only clients, so, hopefully, we
            // know what to send.

            this.protocol_version = ProtocolVersion.Version2;

            this.database_name = BackendEncoding.UTF8Encoding.GetBytes(database_name);
            this.user_name = BackendEncoding.UTF8Encoding.GetBytes(user_name);
            this.arguments = BackendEncoding.UTF8Encoding.GetBytes(arguments);
            this.unused = BackendEncoding.UTF8Encoding.GetBytes(unused);
            this.optional_tty = BackendEncoding.UTF8Encoding.GetBytes(optional_tty);
        }

        public override void WriteToStream(Stream output_stream)
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "WriteToStream");

            // Packet length = 296
            output_stream
                .WriteInt32(296)
                .WriteInt32(PGUtil.ConvertProtocolVersion(this.protocol_version))
                .WriteLimBytes(database_name, 64)
                .WriteLimBytes(user_name, 32)
                .WriteLimBytes(arguments, 64)
                .WriteLimBytes(unused, 64)
                .WriteLimBytes(optional_tty, 64);
        }

    }


    internal sealed class NpgsqlStartupPacketV3 : NpgsqlStartupPacket
    {
        // Logging related values
        private static readonly String CLASSNAME = MethodBase.GetCurrentMethod().DeclaringType.Name;

        // Private fields.
        private readonly ProtocolVersion protocol_version;
        private readonly List<byte[]> parameterNames = new List<byte[]>(10);
        private readonly List<byte[]> parameterValues = new List<byte[]>(10);

        public NpgsqlStartupPacketV3(String database_name, String user_name,
                                   Dictionary<String, String> parameters)
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, CLASSNAME);

            this.protocol_version = ProtocolVersion.Version3;

            //database
            parameterNames.Add(BackendEncoding.UTF8Encoding.GetBytes("database"));
            parameterValues.Add(BackendEncoding.UTF8Encoding.GetBytes(database_name));

            //user
            parameterNames.Add(BackendEncoding.UTF8Encoding.GetBytes("user"));
            parameterValues.Add(BackendEncoding.UTF8Encoding.GetBytes(user_name));

            //parameters
            foreach (KeyValuePair<String, String> param in parameters)
            {
                parameterNames.Add(BackendEncoding.UTF8Encoding.GetBytes(param.Key));
                parameterValues.Add(BackendEncoding.UTF8Encoding.GetBytes(param.Value));
            }
        }

        public override void WriteToStream(Stream output_stream)
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "WriteToStream");

            int packet_size = 4 + 4 + 1;
            for (int i = 0; i < parameterNames.Count; i++)
            {
                packet_size += (parameterNames[i].Length + parameterValues[i].Length + 2);
            }

            output_stream.WriteInt32(packet_size);
            output_stream.WriteInt32(PGUtil.ConvertProtocolVersion(this.protocol_version));

            for (int i = 0; i < parameterNames.Count; i++)
            {
                output_stream.WriteBytesNullTerminated(parameterNames[i]);
                output_stream.WriteBytesNullTerminated(parameterValues[i]);
            }
            output_stream.WriteByte(0);
        }
    }

}
