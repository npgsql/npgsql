#region License
// The PostgreSQL License
//
// Copyright (C) 2017 The Npgsql Development Team
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
using System.IO;
using System.Linq;
using System.Text;


namespace Npgsql.FrontendMessages
{
    class ServerStatusManage
    {
        internal static readonly ServerStatusManage Instance = new ServerStatusManage();
        ServerStatusManage() {}

        internal string Load(NpgsqlReadBuffer buf)
        {
            var beresp = (char)buf.ReadByte();
            switch (beresp)
            {
                case 'm':
                    /* 
                     * Just read up the stream length. ReadInt32() is used 
                     * for Assert() check and moving Readpoint. I guess...
                     */
                    buf.ReadInt32();

                    var buffer = new List<byte>();

                    for (int bRead = buf.ReadByte(); bRead != 0; bRead = buf.ReadByte())
                    {
                         if (bRead == -1)
                         {
                             throw new Exception("Stream does not have data.");
                         }
                         else
                         {
                             buffer.Add((byte)bRead);
                         }
                     }
                   
                    var dbState = buf.TextEncoding.GetString(buffer.ToArray());

                    return dbState;
                case 'E':
                    throw new Exception();
                default:
                    throw new NotSupportedException();
            }
           
        }

        internal void WriteFully(NpgsqlWriteBuffer buf)
        {
            buf.WriteInt32(8);
            buf.WriteInt16(1234);
            buf.WriteInt16(6777);
        }

    }
}
