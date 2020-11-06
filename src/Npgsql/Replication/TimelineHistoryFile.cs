namespace Npgsql.Replication
{
    /// <summary>
    /// Represents a PostgreSQL timeline history file
    /// </summary>
    public readonly struct TimelineHistoryFile
    {
        internal  TimelineHistoryFile(string fileName, byte[] content)
        {
            FileName = fileName;
            Content = content;
        }

        /// <summary>
        /// File name of the timeline history file, e.g., 00000002.history.
        /// </summary>
        public string FileName { get; }


        // While it is pretty safe to assume that a timeline history file
        // only contains ASCII bytes since it is automatically written and
        // parsed by the PostgreSQL backend, we don't want to claim anything
        // about its content (we get it as bytes and we hand it over as bytes).

        /// <summary>
        /// Contents of the timeline history file.
        /// </summary>
        public byte[] Content { get; }
    }
}
