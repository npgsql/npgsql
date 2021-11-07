using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal;
using NpgsqlTypes;

namespace Npgsql.Replication.PgOutput.Messages
{
    /// <summary>
    /// Logical Replication Protocol update message for tables with REPLICA IDENTITY set to DEFAULT.
    /// </summary>
    public class DefaultUpdateMessage : UpdateMessage
    {
        readonly ReplicationTuple _newRow;

        /// <summary>
        /// Columns representing the new row.
        /// </summary>
        public override ReplicationTuple NewRow => _newRow;

        internal DefaultUpdateMessage(NpgsqlConnector connector)
            => _newRow = new(connector);

        internal UpdateMessage Populate(
            NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd, DateTime serverClock, uint? transactionXid,
            RelationMessage relation, ushort numColumns)
        {
            base.Populate(walStart, walEnd, serverClock, transactionXid, relation);

            _newRow.Reset(numColumns, relation.RowDescription);

            return this;
        }

        internal Task Consume(CancellationToken cancellationToken)
            => _newRow.Consume(cancellationToken);
    }
}
