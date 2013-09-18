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
                parameters.Add("extra_float_digits", "3");
                parameters.Add("lc_monetary", "C");

                /*
                 * Try to set SSL negotiation to 0. As of 2010-03-29, recent problems in SSL library implementations made
                 * postgresql to add a parameter to set a value when to do this renegotiation or 0 to disable it.
                 * Currently, Npgsql has a problem with renegotiation so, we are trying to disable it here.
                 * This only works on postgresql servers where the ssl renegotiation settings is supported of course.
                 * See http://lists.pgfoundry.org/pipermail/npgsql-devel/2010-February/001065.html for more information.
                 */
                parameters.Add("ssl_renegotiation_limit", "0");


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
        private readonly String database_name;
        private readonly String user_name;
        private readonly String arguments;
        private readonly String unused;
        private readonly String optional_tty;

        public NpgsqlStartupPacketV2(String database_name, String user_name,
                                   String arguments, String unused, String optional_tty)
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, CLASSNAME);
            // Just copy the values.

            // [FIXME] Validate params? We are the only clients, so, hopefully, we
            // know what to send.

            this.protocol_version = ProtocolVersion.Version2;

            this.database_name = database_name;
            this.user_name = user_name;
            this.arguments = arguments;
            this.unused = unused;
            this.optional_tty = optional_tty;
        }


        public override void WriteToStream(Stream output_stream)
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "WriteToStream");

            // Packet length = 296
            output_stream
                .WriteBytes(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(296)))
                .WriteBytes(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(PGUtil.ConvertProtocolVersion(this.protocol_version))))
                .WriteLimBytes(BackendEncoding.UTF8Encoding.GetBytes(database_name), 64)
                .WriteLimBytes(BackendEncoding.UTF8Encoding.GetBytes(user_name), 32)
                .WriteLimBytes(BackendEncoding.UTF8Encoding.GetBytes(arguments), 64)
                .WriteLimBytes(BackendEncoding.UTF8Encoding.GetBytes(unused), 64)
                .WriteLimBytes(BackendEncoding.UTF8Encoding.GetBytes(optional_tty), 64)
                .Flush();
        }

    }


    internal sealed class NpgsqlStartupPacketV3 : NpgsqlStartupPacket
    {
        // Logging related values
        private static readonly String CLASSNAME = MethodBase.GetCurrentMethod().DeclaringType.Name;

        // Private fields.
        private readonly ProtocolVersion protocol_version;
        private readonly String database_name;
        private readonly String user_name;
        private readonly Dictionary<String, String> parameters;

        public NpgsqlStartupPacketV3(String database_name, String user_name,
                                   Dictionary<String, String> parameters)
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, CLASSNAME);
            // Just copy the values.

            // [FIXME] Validate params? We are the only clients, so, hopefully, we
            // know what to send.

            this.protocol_version = ProtocolVersion.Version3;

            this.database_name = database_name;
            this.user_name = user_name;
            this.parameters = parameters;
        }

        public override void WriteToStream(Stream output_stream)
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "WriteToStream");

            // write data to a temp stream
            MemoryStream stream = new MemoryStream();
            stream.WriteInt32(PGUtil.ConvertProtocolVersion(this.protocol_version));

            stream.WriteStringNullTerminated("user");
            stream.WriteBytesNullTerminated(BackendEncoding.UTF8Encoding.GetBytes(user_name));

            stream.WriteStringNullTerminated("database");
            stream.WriteBytesNullTerminated(BackendEncoding.UTF8Encoding.GetBytes(database_name));


            foreach(string param_name in parameters.Keys)
            {
                stream.WriteBytesNullTerminated(BackendEncoding.UTF8Encoding.GetBytes(param_name));
                stream.WriteBytesNullTerminated(BackendEncoding.UTF8Encoding.GetBytes(parameters[param_name]));
            }
            stream.WriteByte(0);

            byte[] data = stream.ToArray();

            // write data to socket
            output_stream.WriteInt32(4 + data.Length);
            output_stream.Write(data, 0,data.Length);

            output_stream.Flush();
        }
    }

}
