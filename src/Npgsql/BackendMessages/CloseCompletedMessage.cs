namespace Npgsql.BackendMessages
{
    internal class CloseCompletedMessage : IBackendMessage
    {
        public BackendMessageCode Code { get { return BackendMessageCode.CloseComplete; } }
        internal static readonly CloseCompletedMessage Instance = new CloseCompletedMessage();
        CloseCompletedMessage() { }
    }
}
