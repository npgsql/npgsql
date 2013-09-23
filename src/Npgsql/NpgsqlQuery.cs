// Npgsql.NpgsqlQuery.cs
//
// Author:
//     Dave Joyner <d4ljoyn@yahoo.com>
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
using System.Text;

namespace Npgsql
{
    /// <summary>
    /// Summary description for NpgsqlQuery
    /// </summary>
    internal sealed class NpgsqlQuery : ClientMessage
    {
        private readonly NpgsqlCommand _command;
        private readonly ProtocolVersion _protocolVersion;

        public NpgsqlQuery(NpgsqlCommand command, ProtocolVersion protocolVersion)
        {
            _command = command;
            _protocolVersion = protocolVersion;
        }

        public override void WriteToStream(Stream outputStream)
        {
            //NpgsqlEventLog.LogMsg( this.ToString() + _commandText, LogLevel.Debug  );

            byte[] commandText = _command.GetCommandText();

            if (NpgsqlEventLog.Level >= LogLevel.Debug)
            {
                // Log the string being sent.
                PGUtil.LogStringWritten(BackendEncoding.UTF8Encoding.GetString(commandText));
            }

            // Tell to mediator what command is being sent.
            _command.Connector.Mediator.SetSqlSent(commandText);

            // Workaround for seek exceptions when running under ms.net. TODO: Check why Npgsql may be letting behind data in the stream.
            // Whatever issue there was here seems to be rectified.  Leaving it active, per Francisco.
            // The problem is not well understood.  Needs more checking.
            // glenebob@gmail.com       09/20/2013
            outputStream.Flush();

            // Send the query to server.
            // Write the byte 'Q' to identify a query message.
            outputStream.WriteByte((byte)FrontEndMessageCode.Query);

            if (_protocolVersion == ProtocolVersion.Version3)
            {
                // Write message length. Int32 + string length + null terminator.
                PGUtil.WriteInt32(outputStream, 4 + commandText.Length + 1);
            }

            outputStream.WriteBytesNullTerminated(commandText);
        }
    }
}
