using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;

namespace Npgsql.Messages
{
    internal class CommandCompleteMessage : ServerMessage
    {
        private int? _rowsAffected;
        private long? _lastInsertedOID;

        internal CommandCompleteMessage Load(NpgsqlBuffer buf, int len)
        {
            _rowsAffected = null;
            _lastInsertedOID = null;

            var tag = buf.ReadString(len-1);
            buf.Skip(1);   // Null terminator
            var tokens = tag.Split();

            switch (tokens[0])
            {
                case "INSERT":
                    _lastInsertedOID = long.Parse(tokens[1]);
                    goto case "UPDATE";

                case "UPDATE":
                case "DELETE":
                case "COPY":
                    int rowsAffected;
                    if (int.TryParse(tokens[tokens.Length - 1], out rowsAffected)) {
                        _rowsAffected = rowsAffected;
                    }
                    break;
            }
            return this;
        }

        public long? LastInsertedOID
        {
            get { return _lastInsertedOID; }
        }

        public int? RowsAffected
        {
            get { return _rowsAffected; }
        }

        internal override BackEndMessageCode Code { get { return BackEndMessageCode.CompletedResponse; } }
    }
}
