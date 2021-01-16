using Npgsql.Replication.Internal;

namespace Npgsql.Replication.TestDecoding
{
    /// <summary>
    /// Acts as a proxy for a logical replication slot
    /// initialized for for the test_decoding logical decoding plugin.
    /// </summary>
    public class TestDecodingReplicationSlot : LogicalReplicationSlot
    {
        /// <summary>
        /// Creates a new <see cref="TestDecodingReplicationSlot"/> instance.
        /// </summary>
        /// <remarks>
        /// Create a <see cref="TestDecodingReplicationSlot"/> instance with this
        /// constructor to wrap an existing PostgreSQL replication slot that has
        /// been initialized for the test_decoding logical decoding plugin.
        /// </remarks>
        /// <param name="slotName">The name of the existing replication slot</param>
        public TestDecodingReplicationSlot(string slotName)
            : this(new ReplicationSlotOptions(slotName)) { }

        /// <summary>
        /// Creates a new <see cref="TestDecodingReplicationSlot"/> instance.
        /// </summary>
        /// <remarks>
        /// Create a <see cref="TestDecodingReplicationSlot"/> instance with this
        /// constructor to wrap an existing PostgreSQL replication slot that has
        /// been initialized for the test_decoding logical decoding plugin.
        /// </remarks>
        /// <param name="options">The <see cref="ReplicationSlotOptions"/> representing the existing replication slot</param>
        public TestDecodingReplicationSlot(ReplicationSlotOptions options) : base("test_decoding", options) { }
    }
}
