// Npgsql.NpgsqlQuery.cs
//
// Author:
// 	Dave Joyner <d4ljoyn@yahoo.com>
//
//	Copyright (C) 2002 The Npgsql Development Team
//	npgsql-general@gborg.postgresql.org
//	http://gborg.postgresql.org/project/npgsql/projdisplay.php
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
		internal static int SplitStringLimit = 8 * 1024 * 1024;

		public NpgsqlQuery(NpgsqlCommand command, ProtocolVersion protocolVersion)
		{
			_command = command;
			_protocolVersion = protocolVersion;
		}

		public override void WriteToStream(Stream outputStream)
		{
			var commandText = _command.GetCommandText();
			// Log the string being sent.

			if (NpgsqlEventLog.Level >= LogLevel.Debug)
				PGUtil.LogStringWritten(commandText);

			// This method needs refactory.
			// The code below which deals with writing string to stream needs to be redone to use
			// PGUtil.WriteString() as before. The problem is that WriteString is using too much strings (concatenation).
			// Find a way to optimize that. 

			// Tell to mediator what command is being sent.
			_command.Connector.Mediator.SqlSent = commandText;

			// Workaround for seek exceptions when running under ms.net. TODO: Check why Npgsql may be letting behind data in the stream.
			outputStream.Flush();

			// Send the query to server.
			// Write the byte 'Q' to identify a query message.
			outputStream.WriteByte((byte)FrontEndMessageCode.Query);

			//Work out the encoding of the string (null-terminated) once and take the length from having done so
			//rather than doing so repeatedly.
			if (commandText.Length > SplitStringLimit)
			{
				if (_protocolVersion == ProtocolVersion.Version3)
				{
					// Write message length. Int32 + string length + null terminator.
					PGUtil.WriteInt32(outputStream, 4 + UTF8Encoding.GetByteCount(commandText) + 1);
				}
				int cur = 0;
				while (cur < commandText.Length)
				{
					var left = commandText.Length - cur;
					var commandChunk = commandText.Substring(cur, left > SplitStringLimit ? SplitStringLimit : left);
					var bytes = UTF8Encoding.GetBytes(commandChunk);
					outputStream.Write(bytes, 0, bytes.Length);
					cur += SplitStringLimit;
				}
				outputStream.WriteByte(0);
			}
			else
			{
				var bytes = UTF8Encoding.GetBytes(commandText);

				if (_protocolVersion == ProtocolVersion.Version3)
				{
					// Write message length. Int32 + string length + null terminator.
					PGUtil.WriteInt32(outputStream, 4 + bytes.Length + 1);
				}

				outputStream.Write(bytes, 0, bytes.Length);
				outputStream.WriteByte(0);
			}
		}
	}
}
