using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Npgsql
{
    static class PregeneratedMessages
    {
        static PregeneratedMessages()
        {
#pragma warning disable CS8625
            // This is the only use of a write buffer without a connector, for in-memory construction of
            // pregenerated messages.
            using var buf = new NpgsqlWriteBuffer(null, new MemoryStream(), null, NpgsqlWriteBuffer.MinimumSize, Encoding.ASCII);
#pragma warning restore CS8625

            BeginTransRepeatableRead    = Generate(buf, "BEGIN ISOLATION LEVEL REPEATABLE READ");
            BeginTransSerializable      = Generate(buf, "BEGIN TRANSACTION ISOLATION LEVEL SERIALIZABLE");
            BeginTransReadCommitted     = Generate(buf, "BEGIN TRANSACTION ISOLATION LEVEL READ COMMITTED");
            BeginTransReadUncommitted   = Generate(buf, "BEGIN TRANSACTION ISOLATION LEVEL READ UNCOMMITTED");
            CommitTransaction           = Generate(buf, "COMMIT");
            RollbackTransaction         = Generate(buf, "ROLLBACK");
            DiscardAll                  = Generate(buf, "DISCARD ALL");
        }

        internal static byte[] Generate(NpgsqlWriteBuffer buf, string query)
        {
            Debug.Assert(query.All(c => c < 128));

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

        internal static readonly byte[] DiscardAll;
    }
}
