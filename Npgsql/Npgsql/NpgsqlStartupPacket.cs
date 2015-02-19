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
    internal sealed class NpgsqlStartupPacket : ClientMessage
    {
        // Logging related values
        private static readonly String CLASSNAME = MethodBase.GetCurrentMethod().DeclaringType.Name;

        // Private fields.
        private readonly List<byte[]> parameterNames = new List<byte[]>(10);
        private readonly List<byte[]> parameterValues = new List<byte[]>(10);

        public NpgsqlStartupPacket(String database_name, String user_name, NpgsqlConnectionStringBuilder  settings)
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "BuildStartupPacket");

            Dictionary<String, String> parameters = new Dictionary<String, String>();

            parameters.Add("DateStyle", "ISO");
            parameters.Add("client_encoding", "UTF8");
            parameters.Add("extra_float_digits", "2");

            if (! string.IsNullOrEmpty(settings.ApplicationName))
            {
                parameters.Add("application_name", settings.ApplicationName);
            }

            if (! string.IsNullOrEmpty(settings.SearchPath))
            {
                parameters.Add("search_path", settings.SearchPath);
            }

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

            output_stream
                .WriteInt32(packet_size)
                .WriteInt32(PGUtil.ConvertProtocolVersion(ProtocolVersion.Version3));

            for (int i = 0; i < parameterNames.Count; i++)
            {
                output_stream
                    .WriteBytesNullTerminated(parameterNames[i])
                    .WriteBytesNullTerminated(parameterValues[i]);
            }

            output_stream.WriteByte(0);
        }
    }
}
