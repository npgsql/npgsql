using System;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.Internal;
using Npgsql.Internal.Postgres;
using Npgsql.Replication;
using NpgsqlTypes;

namespace Npgsql.Tests.Support;

/// <summary>
/// Helpers for testing replication connections against <see cref="PgPostmasterMock"/>.
/// </summary>
static class ReplicationMock
{
    /// <summary>
    /// How long a mock replication test may wait for the client or the mock before giving up, well below the 30 seconds
    /// after which CI kills a hanging test. Tests have to apply it to both sides: to the client via cancellation tokens and
    /// to the mock via <see cref="Task.WaitAsync(TimeSpan)"/>.
    /// </summary>
    internal static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);

    internal static uint Int4Oid => PostgresMinimalDatabaseInfo.DefaultTypeCatalog.GetOid(DataTypeNames.Int4).Value;
    internal static uint Int8Oid => PostgresMinimalDatabaseInfo.DefaultTypeCatalog.GetOid(DataTypeNames.Int8).Value;
    internal static uint TextOid => PostgresMinimalDatabaseInfo.DefaultTypeCatalog.GetOid(DataTypeNames.Text).Value;
    internal static uint ByteaOid => PostgresMinimalDatabaseInfo.DefaultTypeCatalog.GetOid(DataTypeNames.Bytea).Value;

    /// <summary>
    /// Opens a replication connection to the mock and returns it together with the mocked walsender.
    /// </summary>
    /// <remarks>
    /// The connection's feedback timers are disabled, since they would inject status updates into the message sequences
    /// the tests expect.
    /// </remarks>
    internal static async Task<(TConnection Connection, PgServerMock Server)> OpenReplicationConnection<TConnection>(
        this PgPostmasterMock postmaster)
        where TConnection : ReplicationConnection, new()
    {
        var connection = new TConnection
        {
            ConnectionString = postmaster.ConnectionString,
            WalReceiverStatusInterval = System.Threading.Timeout.InfiniteTimeSpan,
            WalReceiverTimeout = System.Threading.Timeout.InfiniteTimeSpan
        };

        using var cts = new CancellationTokenSource(Timeout);
        await connection.Open(cts.Token);
        return (connection, await postmaster.WaitForServerConnection());
    }

    /// <summary>
    /// Creates the description of a text-format result column, as a walsender sends it for replication commands.
    /// </summary>
    internal static FieldDescription Column(string name, uint typeOid)
    {
        var typeSize = typeOid == Int4Oid ? (short)4 : typeOid == Int8Oid ? (short)8 : (short)-1;
        return new(name, tableOID: 0, columnAttributeNumber: 0, typeOid, typeSize, typeModifier: -1, DataFormat.Text);
    }

    /// <summary>
    /// Encodes a value the way a walsender sends it in a text-format result column.
    /// </summary>
    internal static byte[] Text(string value) => Encoding.UTF8.GetBytes(value);

    /// <summary>
    /// Writes the response to IDENTIFY_SYSTEM, with the column types PostgreSQL 14 and 15 use.
    /// </summary>
    internal static PgServerMock WriteIdentifySystemResponse(
        this PgServerMock server,
        string systemId,
        uint timeline,
        NpgsqlLogSequenceNumber xLogPos,
        string? dbName)
        => server
            .WriteRowDescription(
                Column("systemid", TextOid),
                Column("timeline", Int4Oid),
                Column("xlogpos", TextOid),
                Column("dbname", TextOid))
            .WriteDataRow(
                Text(systemId),
                Text(timeline.ToString(CultureInfo.InvariantCulture)),
                Text(xLogPos.ToString()),
                dbName is null ? null : Text(dbName))
            .WriteCommandComplete("IDENTIFY_SYSTEM")
            .WriteReadyForQuery();
}
