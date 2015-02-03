namespace Npgsql.BackendMessages
{
    internal class CloseCompletedMessage : BackendMessage
    {
        internal override BackendMessageCode Code { get { return BackendMessageCode.CloseComplete; } }
        internal static readonly CloseCompletedMessage Instance = new CloseCompletedMessage();
        CloseCompletedMessage() { }
    }
}
