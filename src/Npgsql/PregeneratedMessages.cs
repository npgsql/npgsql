using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Npgsql.Util;

namespace Npgsql
{
    static class PregeneratedMessages
    {
        static PregeneratedMessages()
        {
            var buf = new NpgsqlWriteBuffer(null, new MemoryStream(), NpgsqlWriteBuffer.MinimumSize, Encoding.ASCII);

            BeginTransRepeatableRead    = Generate(buf, "BEGIN ISOLATION LEVEL REPEATABLE READ");
            BeginTransSerializable      = Generate(buf, "BEGIN TRANSACTION ISOLATION LEVEL SERIALIZABLE");
            BeginTransReadCommitted     = Generate(buf, "BEGIN TRANSACTION ISOLATION LEVEL READ COMMITTED");
            BeginTransReadUncommitted   = Generate(buf, "BEGIN TRANSACTION ISOLATION LEVEL READ UNCOMMITTED");
            CommitTransaction           = Generate(buf, "COMMIT");
            RollbackTransaction         = Generate(buf, "ROLLBACK");
            KeepAlive                   = Generate(buf, "SELECT NULL");
            DiscardAll                  = Generate(buf, "DISCARD ALL");
        }

        internal static byte[] Generate(NpgsqlWriteBuffer buf, string query)
        {
            Debug.Assert(query != null && query.All(c => c < 128));

            var queryByteLen = Encoding.ASCII.GetByteCount(query);

            buf.WriteByte(FrontendMessageCode.Query);
            buf.WriteInt32(4 +            // Message length (including self excluding code)
                           queryByteLen + // Query byte length
                           1);            // Null terminator

            buf.WriteString(query, queryByteLen, false).Wait();
            buf.WriteByte(0);

            var bytes = buf.GetContents();
            buf.Clear();
            return bytes;
        }

        internal static readonly byte[] BeginTransRepeatableRead;
        internal static readonly byte[] BeginTransSerializable;
        internal static readonly byte[] BeginTransReadCommitted;
        internal static readonly byte[] BeginTransReadUncommitted;
        internal static readonly byte[] CommitTransaction;
        internal static readonly byte[] RollbackTransaction;
        internal static readonly byte[] KeepAlive;

        internal static readonly byte[] DiscardAll;
    }
}
