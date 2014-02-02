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
        private byte[] commandBytes = null;
        private string commandText = null;
        private readonly byte[] pgCommandBytes;

        public static readonly NpgsqlQuery BeginTransRepeatableRead = new NpgsqlQuery("BEGIN; SET TRANSACTION ISOLATION LEVEL REPEATABLE READ;");
        public static readonly NpgsqlQuery BeginTransSerializable = new NpgsqlQuery("BEGIN; SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;");
        public static readonly NpgsqlQuery BeginTransReadCommitted = new NpgsqlQuery("BEGIN; SET TRANSACTION ISOLATION LEVEL READ COMMITTED;");
        public static readonly NpgsqlQuery CommitTransaction = new NpgsqlQuery("COMMIT");
        public static readonly NpgsqlQuery RollbackTransaction = new NpgsqlQuery("ROLLBACK");
        public static readonly NpgsqlQuery DiscardAll = new NpgsqlQuery("DISCARD ALL");
        public static readonly NpgsqlQuery UnlistenAll = new NpgsqlQuery("UNLISTEN *");
        public static readonly NpgsqlQuery SetStmtTimeout10Sec = new NpgsqlQuery("SET statement_timeout = 10000");
        public static readonly NpgsqlQuery SetStmtTimeout20Sec = new NpgsqlQuery("SET statement_timeout = 20000");
        public static readonly NpgsqlQuery SetStmtTimeout30Sec = new NpgsqlQuery("SET statement_timeout = 30000");
        public static readonly NpgsqlQuery SetStmtTimeout60Sec = new NpgsqlQuery("SET statement_timeout = 60000");
        public static readonly NpgsqlQuery SetStmtTimeout90Sec = new NpgsqlQuery("SET statement_timeout = 90000");
        public static readonly NpgsqlQuery SetStmtTimeout120Sec = new NpgsqlQuery("SET statement_timeout = 120000");

        public NpgsqlQuery(byte[] command)
        {
            // Message length: Inr32 + command length + null terminator.
            int len = 4 + command.Length + 1;

            // Length + command code ('Q').
            pgCommandBytes = new byte[1 + len];

            MemoryStream commandWriter = new MemoryStream(pgCommandBytes);

            commandWriter
                .WriteBytes((byte)FrontEndMessageCode.Query)
                .WriteInt32(len)
                .WriteBytesNullTerminated(command);

            commandBytes = command;
        }

        public NpgsqlQuery(string command)
        {
            // Message length: Inr32 + command length + null terminator.
            int len = 4 + command.Length + 1;

            // Length + command code ('Q').
            pgCommandBytes = new byte[1 + len];

            MemoryStream commandWriter = new MemoryStream(pgCommandBytes);

            commandWriter
                .WriteBytes((byte)FrontEndMessageCode.Query)
                .WriteInt32(len)
                .WriteStringNullTerminated(command);

            commandText = command;
        }

        public override void WriteToStream(Stream outputStream)
        {
            if (NpgsqlEventLog.Level >= LogLevel.Debug)
            {
                // Log the string being sent.
                // If (this) was constructed with a byte[], then commandText has to be
                // initialized before the first Log call.
                if (commandText == null)
                {
                    commandText = BackendEncoding.UTF8Encoding.GetString(commandBytes);
                    commandBytes = null;
                }

                PGUtil.LogStringWritten(commandText);
            }

            outputStream.WriteBytes(pgCommandBytes);
        }
    }
}
