using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;

namespace Npgsql.Messages
{
    internal class CommandCompleteMessage : ServerMessage
    {
        internal long? LastInsertedOID { get; private set; }
        internal int? RowsAffected { get; private set; }

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
                    var lastInsertedOID = long.Parse(tokens[1]);
                    if (lastInsertedOID != 0) {
                        LastInsertedOID = lastInsertedOID;
                    }
                    goto case "UPDATE";

                case "UPDATE":
                case "DELETE":
                case "COPY":
                    int rowsAffected;
                    if (int.TryParse(tokens[tokens.Length - 1], out rowsAffected)) {
                        RowsAffected = rowsAffected;
                    }
                    break;
            }
            return this;
        }

        internal override BackEndMessageCode Code { get { return BackEndMessageCode.CompletedResponse; } }
    }
}
