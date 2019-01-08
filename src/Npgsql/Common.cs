using System.Diagnostics;
using System.Threading.Tasks;

namespace Npgsql
{
    /// <summary>
    /// Base class for all classes which represent a message sent by the PostgreSQL backend.
    /// </summary>
    interface IBackendMessage
    {
        BackendMessageCode Code { get; }
    }

    /// <summary>
    /// Base class for all classes which represent a message sent to the PostgreSQL backend.
    /// Concrete classes which directly inherit this represent arbitrary-length messages which can chunked.
    /// </summary>
    abstract class FrontendMessage
    {
        /// <param name="buf">the buffer into which to write the message.</param>
        /// <param name="async"></param>
        /// <returns>
        /// Whether there was enough space in the buffer to contain the entire message.
        /// If false, the buffer should be flushed and write should be called again.
        /// </returns>
        internal abstract Task Write(NpgsqlWriteBuffer buf, bool async);

        /// <summary>
        /// Returns how many messages PostgreSQL is expected to send in response to this message.
        /// Used for message prepending.
        /// </summary>
        internal virtual int ResponseMessageCount => 1;
    }

    /// <summary>
    /// Represents a simple frontend message which is typically small and fits well within
    /// the write buffer. The message is first queries for the number of bytes it requires,
    /// and then writes itself out.
    /// </summary>
    abstract class SimpleFrontendMessage : FrontendMessage
    {
        /// <summary>
        /// Returns the number of bytes needed to write this message.
        /// </summary>
        internal abstract int Length { get; }

        /// <summary>
        /// Writes the message contents into the buffer.
        /// </summary>
        internal abstract void WriteFully(NpgsqlWriteBuffer buf);

        internal sealed override Task Write(NpgsqlWriteBuffer buf, bool async)
        {
            if (buf.WriteSpaceLeft < Length)
                return FlushAndWrite(buf, async);
            Debug.Assert(Length <= buf.WriteSpaceLeft, $"Message of type {GetType().Name} has length {Length} which is bigger than the buffer ({buf.WriteSpaceLeft})");
            WriteFully(buf);
            return PGUtil.CompletedTask;
        }

        async Task FlushAndWrite(NpgsqlWriteBuffer buf, bool async)
        {
            await buf.Flush(async);
            Debug.Assert(Length <= buf.WriteSpaceLeft, $"Message of type {GetType().Name} has length {Length} which is bigger than the buffer ({buf.WriteSpaceLeft})");
            WriteFully(buf);
        }
    }

    enum BackendMessageCode : byte
    {
        AuthenticationRequest = (byte)'R',
        BackendKeyData        = (byte)'K',
        BindComplete          = (byte)'2',
        CloseComplete         = (byte)'3',
        CompletedResponse     = (byte)'C',
        CopyData              = (byte)'d',
        CopyDone              = (byte)'c',
        CopyBothResponse      = (byte)'W',
        CopyInResponse        = (byte)'G',
        CopyOutResponse       = (byte)'H',
        DataRow               = (byte)'D',
        EmptyQueryResponse    = (byte)'I',
        ErrorResponse         = (byte)'E',
        FunctionCall          = (byte)'F',
        FunctionCallResponse  = (byte)'V',
        NoData                = (byte)'n',
        NoticeResponse        = (byte)'N',
        NotificationResponse  = (byte)'A',
        ParameterDescription  = (byte)'t',
        ParameterStatus       = (byte)'S',
        ParseComplete         = (byte)'1',
        PasswordPacket        = (byte)' ',
        PortalSuspended       = (byte)'s',
        ReadyForQuery         = (byte)'Z',
        RowDescription        = (byte)'T',
    }

    enum StatementOrPortal : byte
    {
        Statement = (byte)'S',
        Portal = (byte)'P'
    }

    /// <summary>
    /// Specifies the type of SQL statement, e.g. SELECT
    /// </summary>
    public enum StatementType
    {
#pragma warning disable 1591
        Select,
        Insert,
        Delete,
        Update,
        CreateTableAs,
        Move,
        Fetch,
        Copy,
        Other
#pragma warning restore 1591
    }
}
