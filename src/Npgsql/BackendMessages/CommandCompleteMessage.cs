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
using Npgsql.Logging;

namespace Npgsql.BackendMessages
{
    internal class CommandCompleteMessage : IBackendMessage
    {
        internal StatementType StatementType { get; private set; }
        internal uint OID { get; private set; }
        internal uint Rows { get; private set; }

        static readonly NpgsqlLogger Log = NpgsqlLogManager.GetCurrentClassLogger();

        internal CommandCompleteMessage Load(NpgsqlBuffer buf, int len)
        {
            Rows = 0;
            OID = 0;

            var tag = buf.ReadString(len-1);
            buf.Skip(1);   // Null terminator
            var tokens = tag.Split();

            if (tokens.Length == 0) {
                return this;
            }

            switch (tokens[0])
            {
            case "INSERT":
                StatementType = StatementType.Insert;

                uint oid;
                if (uint.TryParse(tokens[1], out oid))
                {
                    OID = oid;
                }
                else
                {
                    Log.Error("Ignoring unparseable OID in CommandComplete: " + tokens[1]);
                }

                ParseRows(tokens[2]);
                break;

            case "DELETE":
                StatementType = StatementType.Delete;
                ParseRows(tokens[1]);
                break;

            case "UPDATE":
                StatementType = StatementType.Update;
                ParseRows(tokens[1]);
                break;

            case "SELECT":
                StatementType = StatementType.Select;
                // PostgreSQL 8.4 and below doesn't include the number of rows
                if (tokens.Length > 1) {
                    ParseRows(tokens[1]);
                }
                break;

            case "MOVE":
                StatementType = StatementType.Move;
                ParseRows(tokens[1]);
                break;

            case "FETCH":
                StatementType = StatementType.Fetch;
                ParseRows(tokens[1]);
                break;

            case "COPY":
                StatementType = StatementType.Copy;
                if (tokens.Length > 1) {
                    ParseRows(tokens[1]);
                }
                break;

            case "CREATE":
                if (tag.StartsWith("CREATE TABLE AS"))
                {
                    StatementType = StatementType.CreateTableAs;
                    ParseRows(tokens[3]);
                    break;
                }
                goto default;

            default:
                StatementType = StatementType.Other;
                break;
            }
            return this;
        }

        void ParseRows(string token)
        {
            uint rows;
            if (uint.TryParse(token, out rows))
            {
                Rows = rows;
            }
            else
            {
                Log.Error("Ignoring unparseable rows in CommandComplete: " + token);
            }
        }

        public BackendMessageCode Code => BackendMessageCode.CompletedResponse;
    }
}
