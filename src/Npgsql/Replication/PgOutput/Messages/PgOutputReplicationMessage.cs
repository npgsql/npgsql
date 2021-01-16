﻿using NpgsqlTypes;
using System;

namespace Npgsql.Replication.PgOutput.Messages
{
    /// <summary>
    /// The base class of all Logical Replication Protocol Messages
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/protocol-logicalrep-message-formats.html for details about the
    /// protocol.
    /// </remarks>
    public abstract class PgOutputReplicationMessage : ReplicationMessage
    {
        /// <inheritdoc />
        public override string ToString() => GetType().Name;

        /// <summary>
        /// Returns a clone of this message, which can be accessed after other replication messages have been retrieved.
        /// </summary>
        public abstract PgOutputReplicationMessage Clone();
    }
}
