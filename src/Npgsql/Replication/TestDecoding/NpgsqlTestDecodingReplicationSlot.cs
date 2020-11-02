using Npgsql.Replication.Internal;

namespace Npgsql.Replication.TestDecoding
{
    /// <summary>
    /// Acts as a proxy for a logical replication slot
    /// initialized for for the test_decoding logical decoding plugin.
    /// </summary>
    public class NpgsqlTestDecodingReplicationSlot : NpgsqlLogicalReplicationSlot
    {
        /// <summary>
        /// Creates a new <see cref="NpgsqlTestDecodingReplicationSlot"/> instance.
        /// </summary>
        /// <remarks>
        /// Create a <see cref="NpgsqlTestDecodingReplicationSlot"/> instance with this
        /// constructor to wrap an existing PostgreSQL replication slot that has
        /// been initialized for the test_decoding logical decoding plugin.
        /// </remarks>
        /// <param name="slotName">The name of the existing replication slot</param>
        public NpgsqlTestDecodingReplicationSlot(string slotName)
            : this(new NpgsqlReplicationSlotOptions(slotName)) { }

        /// <summary>
        /// Creates a new <see cref="NpgsqlTestDecodingReplicationSlot"/> instance.
        /// </summary>
        /// <remarks>
        /// Create a <see cref="NpgsqlTestDecodingReplicationSlot"/> instance with this
        /// constructor to wrap an existing PostgreSQL replication slot that has
        /// been initialized for the test_decoding logical decoding plugin.
        /// </remarks>
        /// <param name="options">The <see cref="NpgsqlReplicationSlotOptions"/> representing the existing replication slot</param>
        public NpgsqlTestDecodingReplicationSlot(NpgsqlReplicationSlotOptions options) : base("test_decoding", options) { }
    }
}
