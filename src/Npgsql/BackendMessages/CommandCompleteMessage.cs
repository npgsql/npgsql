#region License
// The PostgreSQL License
//
// Copyright (C) 2015 The Npgsql Development Team
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
using System.Linq;
using System.Text;

namespace Npgsql.BackendMessages
{
    internal class CommandCompleteMessage : IBackendMessage
    {
        internal uint? LastInsertedOID { get; private set; }
        internal uint? RowsAffected { get; private set; }

        internal CommandCompleteMessage Load(NpgsqlBuffer buf, int len)
        {
            RowsAffected = null;
            LastInsertedOID = null;

            var tag = buf.ReadString(len-1);
            buf.Skip(1);   // Null terminator
            var tokens = tag.Split();

            switch (tokens[0])
            {
                case "INSERT":
                    var lastInsertedOID = uint.Parse(tokens[1]);
                    if (lastInsertedOID != 0) {
                        LastInsertedOID = lastInsertedOID;
                    }
                    goto case "UPDATE";

                case "UPDATE":
                case "DELETE":
                case "COPY":
                    uint rowsAffected;
                    if (uint.TryParse(tokens[tokens.Length - 1], out rowsAffected)) {
                        RowsAffected = rowsAffected;
                    }
                    break;
            }
            return this;
        }

        public BackendMessageCode Code { get { return BackendMessageCode.CompletedResponse; } }
    }
}
