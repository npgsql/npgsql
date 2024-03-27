namespace Npgsql.BackendMessages;

/// <summary>
/// DataRow is special in that it does not parse the actual contents of the backend message,
/// because in sequential mode the message will be traversed and processed sequentially by
/// <see cref="NpgsqlDataReader"/>.
/// </summary>
sealed class DataRowMessage : IBackendMessage
{
    public BackendMessageCode Code => BackendMessageCode.DataRow;

    internal int Length { get; private set; }
    internal short ColumnCount { get; private set; }

    internal DataRowMessage Load(int len, short columnCount)
    {
        Length = len;
        ColumnCount = columnCount;
        return this;
    }
}
