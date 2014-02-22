// created on 10/6/2002 at 21:33

// Npgsql.NpgsqlPasswordPacket.cs
//
// Author:
//    Francisco Jr. (fxjrlists@yahoo.com.br)
//
//    Copyright (C) 2002 The Npgsql Development Team
//    npgsql-general@gborg.postgresql.org
//    http://gborg.postgresql.org/project/npgsql/projdisplay.php
//
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
using System.Reflection;

namespace Npgsql
{
    /// <summary>
    /// This class represents a PasswordPacket message sent to backend
    /// PostgreSQL.
    /// </summary>
    internal sealed class NpgsqlPasswordPacket : ClientMessage
    {
        // Logging related values
        private static readonly String CLASSNAME = MethodBase.GetCurrentMethod().DeclaringType.Name;

        private readonly byte[] password;

        public NpgsqlPasswordPacket(byte[] password)
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, CLASSNAME);

            this.password = password;
        }

        public override void WriteToStream(Stream outputStream)
        {
            NpgsqlEventLog.LogMethodEnter(LogLevel.Debug, CLASSNAME, "WriteToStream");

            outputStream
                .WriteBytes((Byte)ASCIIBytes.p)
                .WriteInt32(4 + password.Length)
                .WriteBytes(password);
        }
    }
}
