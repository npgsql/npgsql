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
            Query = query;
        }

        internal override int Length
        {
            get { return 5 + Query.Length; }
        }

        internal override void Write(NpgsqlBuffer buf)
        {
            Contract.Requires(Query != null && Query.All(c => c < 128));

            buf.WriteByte(Code);
            buf.WriteInt32(Length);
            buf.WriteBytesNullTerminated(Encoding.ASCII.GetBytes(Query));
        }

        public override string ToString()
        {
            return String.Format("[Query={0}]", Query);
        }
    }
}
