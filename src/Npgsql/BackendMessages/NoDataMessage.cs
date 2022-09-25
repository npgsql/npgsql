namespace Npgsql.BackendMessages;

sealed class NoDataMessage : IBackendMessage
{
    public BackendMessageCode Code => BackendMessageCode.NoData;
    internal static readonly NoDataMessage Instance = new();
    NoDataMessage() { }
}