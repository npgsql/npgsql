﻿using System.Diagnostics;

namespace Npgsql.BackendMessages
{
    class CommandCompleteMessage : IBackendMessage
    {
        internal StatementType StatementType { get; private set; }
        internal uint OID { get; private set; }
        internal ulong Rows { get; private set; }

        internal CommandCompleteMessage Load(NpgsqlReadBuffer buf, int len)
        {
            Rows = 0;
            OID = 0;

            var bytes = buf.Buffer;
            var i = buf.ReadPosition;
            buf.Skip(len);
            switch (bytes[i])
            {
            case (byte)'I':
                if (!AreEqual(bytes, i, "INSERT "))
                    goto default;
                StatementType = StatementType.Insert;
                i += 7;
                OID = (uint) ParseNumber(bytes, ref i);
                i++;
                Rows = ParseNumber(bytes, ref i);
                return this;

            case (byte)'D':
                if (!AreEqual(bytes, i, "DELETE "))
                    goto default;
                StatementType = StatementType.Delete;
                i += 7;
                Rows = ParseNumber(bytes, ref i);
                return this;

            case (byte)'U':
                if (!AreEqual(bytes, i, "UPDATE "))
                    goto default;
                StatementType = StatementType.Update;
                i += 7;
                Rows = ParseNumber(bytes, ref i);
                return this;

            case (byte)'S':
                if (!AreEqual(bytes, i, "SELECT "))
                    goto default;
                StatementType = StatementType.Select;
                i += 7;
                Rows = ParseNumber(bytes, ref i);
                return this;

            case (byte)'M':
                if (!AreEqual(bytes, i, "MOVE "))
                    goto default;
                StatementType = StatementType.Move;
                i += 5;
                Rows = ParseNumber(bytes, ref i);
                return this;

            case (byte)'F':
                if (!AreEqual(bytes, i, "FETCH "))
                    goto default;
                StatementType = StatementType.Fetch;
                i += 6;
                Rows = ParseNumber(bytes, ref i);
                return this;

            case (byte)'C':
                if (!AreEqual(bytes, i, "COPY "))
                    goto default;
                StatementType = StatementType.Copy;
                i += 5;
                Rows = ParseNumber(bytes, ref i);
                return this;

            default:
                StatementType = StatementType.Other;
                return this;
            }
        }

        static bool AreEqual(byte[] bytes, int pos, string s)
        {
            for (var i = 0; i < s.Length; i++)
            {
                if (bytes[pos+i] != s[i])
                    return false;
            }
            return true;
        }

        static ulong ParseNumber(byte[] bytes, ref int pos)
        {
            Debug.Assert(bytes[pos] >= '0' && bytes[pos] <= '9');
            uint result = 0;
            do
            {
                result = result * 10 + bytes[pos++] - '0';
            } while (bytes[pos] >= '0' && bytes[pos] <= '9');
            return result;
        }

        public BackendMessageCode Code => BackendMessageCode.CompletedResponse;
    }
}
