using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Npgsql.FrontendMessages
{
    /// <summary>
    /// A simple query message.
    /// </summary>
    class QueryMessage : FrontendMessage
    {
        readonly Encoding _encoding;
        string _query;

        const byte Code = (byte)'Q';

        internal QueryMessage(Encoding encoding)
        {
            _encoding = encoding;
        }

        internal QueryMessage Populate(string query)
        {
            Debug.Assert(query != null);

            _query = query;
            return this;
        }

        internal override async Task Write(NpgsqlWriteBuffer buf, bool async)
        {
            if (buf.WriteSpaceLeft < 1 + 4)
                await buf.Flush(async);
            var queryByteLen = _encoding.GetByteCount(_query);

            buf.WriteByte(Code);
            buf.WriteInt32(4 +            // Message length (including self excluding code)
                           queryByteLen + // Query byte length
                           1);            // Null terminator

            await buf.WriteString(_query, queryByteLen, async);
            if (buf.WriteSpaceLeft < 1)
                await buf.Flush(async);
            buf.WriteByte(0);
        }

        public override string ToString() => $"[Query={_query}]";
    }
}
