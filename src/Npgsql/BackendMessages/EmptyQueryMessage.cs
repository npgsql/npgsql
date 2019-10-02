namespace Npgsql.BackendMessages
{
    class EmptyQueryMessage : IBackendMessage
    {
        public BackendMessageCode Code => BackendMessageCode.EmptyQueryResponse;
        internal static readonly EmptyQueryMessage Instance = new EmptyQueryMessage();
        EmptyQueryMessage() { }
    }
}
