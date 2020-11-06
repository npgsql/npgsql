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
    public abstract class LogicalReplicationProtocolMessage : NpgsqlReplicationMessage
    {
    }
}
