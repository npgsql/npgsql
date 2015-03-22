using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace Npgsql.FrontendMessages
{
    /// <summary>
    /// A simple query message.
    /// 
    /// Note that since this is only used to send some specific control messages (e.g. start transaction)
    /// and never arbitrary-length user-provided queries, this message is treated as simple and not chunking,
    /// and only ASCII is supported.
    /// </summary>
    class QueryMessage : SimpleFrontendMessage
    {
        string Query { get; set; }

        internal const byte Code = (byte)'Q';

        internal QueryMessage(string query)
        {
            Contract.Requires(query != null && query.All(c => c < 128), "Not ASCII");
            Contract.Requires(PGUtil.UTF8Encoding.GetByteCount(query) + 5 < NpgsqlBuffer.MinimumBufferSize, "Too long");

            Query = query;
        }

        internal override int Length
        {
            get { return 1 + 4 + (Query.Length + 1); }
        }

        internal override void Write(NpgsqlBuffer buf)
        {
            Contract.Requires(Query != null && Query.All(c => c < 128));

            buf.WriteByte(Code);
            buf.WriteInt32(Length - 1);
            buf.WriteBytesNullTerminated(Encoding.ASCII.GetBytes(Query));
        }

        public override string ToString()
        {
            return String.Format("[Query={0}]", Query);
        }
    }
}
