using Npgsql.Internal;

namespace Npgsql.BackendMessages;

sealed class BackendKeyDataMessage : IBackendMessage
{
    public BackendMessageCode Code => BackendMessageCode.BackendKeyData;

    internal int BackendProcessId { get; }
    internal int BackendSecretKey { get; }

    internal BackendKeyDataMessage(NpgsqlReadBuffer buf)
    {
        BackendProcessId = buf.ReadInt32();
        BackendSecretKey = buf.ReadInt32();
    }
}