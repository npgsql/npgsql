using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;

namespace Npgsql.BackendMessages
{
    internal class CommandCompleteMessage : BackendMessage
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

        internal override BackendMessageCode Code { get { return BackendMessageCode.CompletedResponse; } }
    }
}
