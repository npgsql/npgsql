namespace Npgsql.BackendMessages
{
    class ParseCompleteMessage : IBackendMessage
    {
        public BackendMessageCode Code => BackendMessageCode.ParseComplete;
        internal static readonly ParseCompleteMessage Instance = new ParseCompleteMessage();
        ParseCompleteMessage() { }
    }
}
