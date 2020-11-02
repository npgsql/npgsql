using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Replication.Logical;
using Npgsql.Replication.Internal;
using Npgsql.Replication.PgOutput.Messages;
using Npgsql.TypeHandlers.DateTimeHandlers;
using NpgsqlTypes;

namespace Npgsql.Replication
{
    /// <summary>
    /// Represents a logical replication connection to a PostgreSQL server
    /// </summary>
    public sealed class NpgsqlLogicalReplicationConnection : NpgsqlReplicationConnection
    {
        private protected override ReplicationMode ReplicationMode => ReplicationMode.Logical;
    }
}
