using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Properties;

namespace Npgsql;

sealed class NpgsqlDataSourceBatch : NpgsqlBatch
{
    internal NpgsqlDataSourceBatch(NpgsqlConnection connection)
        : base(static (conn, batch) => new NpgsqlDataSourceCommand(batch, DefaultBatchCommandsSize, conn), connection)
    {
    }

    // The below are incompatible with batches executed directly against DbDataSource, since no DbConnection
    // is involved at the user API level and the batch owns the DbConnection.
    public override void Prepare()
        => throw new NotSupportedException(NpgsqlStrings.NotSupportedOnDataSourceBatch);

    public override Task PrepareAsync(CancellationToken cancellationToken = default)
        => throw new NotSupportedException(NpgsqlStrings.NotSupportedOnDataSourceBatch);

    protected override DbConnection? DbConnection
    {
        get => throw new NotSupportedException(NpgsqlStrings.NotSupportedOnDataSourceBatch);
        set => throw new NotSupportedException(NpgsqlStrings.NotSupportedOnDataSourceBatch);
    }

    protected override DbTransaction? DbTransaction
    {
        get => throw new NotSupportedException(NpgsqlStrings.NotSupportedOnDataSourceBatch);
        set => throw new NotSupportedException(NpgsqlStrings.NotSupportedOnDataSourceBatch);
    }
}
