using System.Diagnostics;

namespace Npgsql.FrontendMessages
{
    class CancelRequestMessage : SimpleFrontendMessage
    {
        internal int BackendProcessId { get; }
        internal int BackendSecretKey { get; }

        const int CancelRequestCode = 1234 << 16 | 5678;

        internal CancelRequestMessage(int backendProcessId, int backendSecretKey)
        {
            BackendProcessId = backendProcessId;
            BackendSecretKey = backendSecretKey;
        }

        internal override int Length => 16;

        internal override void WriteFully(NpgsqlWriteBuffer buf)
        {
            Debug.Assert(BackendProcessId != 0);

            buf.WriteInt32(Length);
            buf.WriteInt32(CancelRequestCode);
            buf.WriteInt32(BackendProcessId);
            buf.WriteInt32(BackendSecretKey);
        }

        public override string ToString() => $"[CancelRequest(BackendProcessId={BackendProcessId})]";
    }
}
