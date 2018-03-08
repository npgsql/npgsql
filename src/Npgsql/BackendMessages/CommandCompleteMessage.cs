#region License
// The PostgreSQL License
//
// Copyright (C) 2018 The Npgsql Development Team
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
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Npgsql.Logging;

namespace Npgsql.BackendMessages
{
    class CommandCompleteMessage : IBackendMessage
    {
        internal StatementType StatementType { get; private set; }
        internal uint OID { get; private set; }
        internal uint Rows { get; private set; }

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
                OID = ParseNumber(bytes, ref i);
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

        static uint ParseNumber(byte[] bytes, ref int pos)
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
