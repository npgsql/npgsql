using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Npgsql.FrontendMessages
{
    /// <summary>
    /// A frontend message of an arbitrary type that has been pregenerated for efficiency - it is kept
    /// in byte[] form and doesn't have to be serialized for each send.
    /// </summary>
    class PregeneratedMessage : SimpleFrontendMessage
    {
        readonly byte[] _data;
        readonly string _description;

        /// <summary>
        /// Constructs a new pregenerated message.
        /// </summary>
        /// <param name="data">The data to be sent for this message, not including the 4-byte length.</param>
        /// <param name="description">Optional string form/description for debugging</param>
        /// <param name="responseMessageCount">Returns how many messages PostgreSQL is expected to send in response to this message.</param>
        internal PregeneratedMessage(byte[] data, string description, int responseMessageCount)
        {
            Debug.Assert(data.Length < NpgsqlWriteBuffer.MinimumSize);

            _data = data;
            _description = description;
            ResponseMessageCount = responseMessageCount;
        }

        internal override int Length => _data.Length;

        internal override int ResponseMessageCount { get; }

        internal override void WriteFully(NpgsqlWriteBuffer buf)
        {
            buf.WriteBytes(_data, 0, _data.Length);
        }

        public override string ToString() =>  _description ?? "[?]";

        static PregeneratedMessage()
        {
            var buf = new NpgsqlWriteBuffer(null, new MemoryStream(), NpgsqlWriteBuffer.MinimumSize, Encoding.ASCII);
            var message = new QueryMessage(PGUtil.UTF8Encoding);

            BeginTransRepeatableRead    = Generate(buf, message, "BEGIN ISOLATION LEVEL REPEATABLE READ");
            BeginTransSerializable      = Generate(buf, message, "BEGIN TRANSACTION ISOLATION LEVEL SERIALIZABLE");
            BeginTransReadCommitted     = Generate(buf, message, "BEGIN TRANSACTION ISOLATION LEVEL READ COMMITTED");
            BeginTransReadUncommitted   = Generate(buf, message, "BEGIN TRANSACTION ISOLATION LEVEL READ UNCOMMITTED");
            CommitTransaction           = Generate(buf, message, "COMMIT");
            RollbackTransaction         = Generate(buf, message, "ROLLBACK");
            KeepAlive                   = Generate(buf, message, "SELECT NULL");
            DiscardAll                  = Generate(buf, message, "DISCARD ALL");
        }

        internal static PregeneratedMessage Generate(NpgsqlWriteBuffer buf, QueryMessage queryMessage, string query, int responseMessageCount=2)
        {
            Debug.Assert(query != null && query.All(c => c < 128));
            queryMessage.Populate(query);
            var description = queryMessage.ToString();
            queryMessage.Write(buf, false).Wait();
            var bytes = buf.GetContents();
            buf.Clear();
            return new PregeneratedMessage(bytes, description, responseMessageCount);
        }

        internal static readonly PregeneratedMessage BeginTransRepeatableRead;
        internal static readonly PregeneratedMessage BeginTransSerializable;
        internal static readonly PregeneratedMessage BeginTransReadCommitted;
        internal static readonly PregeneratedMessage BeginTransReadUncommitted;
        internal static readonly PregeneratedMessage CommitTransaction;
        internal static readonly PregeneratedMessage RollbackTransaction;
        internal static readonly PregeneratedMessage KeepAlive;

        internal static readonly PregeneratedMessage DiscardAll;
    }
}
