namespace Npgsql.BackendMessages
{
    class BindCompleteMessage : IBackendMessage
    {
        public BackendMessageCode Code => BackendMessageCode.BindComplete;
        internal static readonly BindCompleteMessage Instance = new BindCompleteMessage();
        BindCompleteMessage() { }
    }
}
