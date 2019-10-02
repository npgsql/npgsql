namespace Npgsql.BackendMessages
{
    class ReadyForQueryMessage : IBackendMessage
    {
        public BackendMessageCode Code => BackendMessageCode.ReadyForQuery;

        internal TransactionStatus TransactionStatusIndicator { get; private set; }

        internal ReadyForQueryMessage Load(NpgsqlReadBuffer buf) {
            TransactionStatusIndicator = (TransactionStatus)buf.ReadByte();
            return this;
        }
    }
}
