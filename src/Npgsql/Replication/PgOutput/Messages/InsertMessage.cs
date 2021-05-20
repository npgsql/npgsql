using NpgsqlTypes;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql.Replication.PgOutput.Messages
{
    /// <summary>
    /// Logical Replication Protocol insert message
    /// </summary>
    public sealed class InsertMessage : TransactionalMessage
    {
        /// <summary>
        /// ID of the relation corresponding to the ID in the relation message.
        /// </summary>
        public uint RelationId { get; private set; }

        /// <summary>
        /// Columns representing the new row.
        /// </summary>
        public ReplicationDataRecord NewRow { get; private set; } = default!;

        internal InsertMessage Populate(
            NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd, DateTime serverClock, uint? transactionXid, uint relationId,
            ReplicationDataRecord newRow)
        {
            base.Populate(walStart, walEnd, serverClock, transactionXid);
            RelationId = relationId;
            NewRow = newRow;
            return this;
        }

        /// <inheritdoc />
#if NET5_0_OR_GREATER
        public override InsertMessage Clone()
#else
        public override PgOutputReplicationMessage Clone()
#endif
        {
            var clone = new InsertMessage();
            clone.Populate(WalStart, WalEnd, ServerClock, TransactionXid, RelationId, NewRow.Clone(async: false).GetAwaiter().GetResult());
            return clone;
        }

        /// <summary>
        /// Returns a buffered clone of this <see cref="InsertMessage"/>, which can be accessed after other replication messages have been retrieved.
        /// </summary>
        /// <param name="cancellationToken">
        /// An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/>.
        /// </param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task<InsertMessage> CloneAsync(CancellationToken cancellationToken = default)
        {
            var clone = new InsertMessage();
            clone.Populate(WalStart, WalEnd, ServerClock, TransactionXid, RelationId, await NewRow.Clone(async: true, cancellationToken));
            return clone;
        }
    }
}
