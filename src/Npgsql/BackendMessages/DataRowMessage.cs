namespace Npgsql.BackendMessages
{
    /// <summary>
    /// DataRow is special in that it does not parse the actual contents of the backend message,
    /// because in sequential mode the message will be traversed and processed sequentially by
    /// <see cref="NpgsqlSequentialDataReader"/>.
    /// </summary>
    class DataRowMessage : IBackendMessage
    {
        public BackendMessageCode Code => BackendMessageCode.DataRow;

        internal int Length { get; private set; }

        internal DataRowMessage Load(int len)
        {
            Length = len;
            return this;
        }
    }
}
