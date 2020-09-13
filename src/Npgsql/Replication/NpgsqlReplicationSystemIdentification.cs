using JetBrains.Annotations;
using NpgsqlTypes;

namespace Npgsql.Replication
{
    /// <summary>
    /// Contains server identification information returned from <see cref="NpgsqlReplicationConnection.IdentifySystem"/>.
    /// </summary>
    [PublicAPI]
    public class NpgsqlReplicationSystemIdentification
    {

        internal NpgsqlReplicationSystemIdentification(string systemId, uint timeline, NpgsqlLogSequenceNumber xLogPos, string dbName)
        {
            SystemId = systemId;
            Timeline = timeline;
            XLogPos = xLogPos;
            DbName = dbName;
        }

        /// <summary>
        /// The unique system identifier identifying the cluster.
        /// This can be used to check that the base backup used to initialize the standby came from the same cluster.
        /// </summary>
        [PublicAPI]
        public string SystemId { get; }

        /// <summary>
        /// Current timeline ID. Also useful to check that the standby is consistent with the master.
        /// </summary>
        [PublicAPI]
        public uint Timeline { get; }

        /// <summary>
        /// Current WAL flush location. Useful to get a known location in the write-ahead log where streaming can start.
        /// </summary>
        [PublicAPI]
        public NpgsqlLogSequenceNumber XLogPos { get; }

        /// <summary>
        /// Database connected to.
        /// </summary>
        [PublicAPI]
        public string? DbName { get; }
    }
}
