namespace Npgsql.BackendMessages
{
    class NoDataMessage : IBackendMessage
    {
        public BackendMessageCode Code => BackendMessageCode.NoData;
        internal static readonly NoDataMessage Instance = new NoDataMessage();
        NoDataMessage() { }
    }
}
