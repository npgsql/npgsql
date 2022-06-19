using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Properties;

namespace Npgsql;

sealed class NpgsqlDataSourceCommand : NpgsqlCommand
{
    internal NpgsqlDataSourceCommand(NpgsqlConnection connection)
        : base(cmdText: null, connection)
    {
    }

    // For NpgsqlBatch only
    internal NpgsqlDataSourceCommand(int batchCommandCapacity, NpgsqlConnection connection)
        : base(batchCommandCapacity, connection)
    {
    }

    internal override async ValueTask<NpgsqlDataReader> ExecuteReader(
        CommandBehavior behavior,
        bool async,
        CancellationToken cancellationToken)
    {
        await InternalConnection!.Open(async, cancellationToken);

        try
        {
            return await base.ExecuteReader(
                    behavior | CommandBehavior.CloseConnection,
                    async,
                    cancellationToken)
                .ConfigureAwait(false);
        }
        catch
        {
            try
            {
                await InternalConnection.Close(async);
            }
            catch
            {
                // Swallow to allow the original exception to bubble up
            }

            throw;
        }
    }

    // The below are incompatible with commands executed directly against DbDataSource, since no DbConnection
    // is involved at the user API level and the command owns the DbConnection.
    public override void Prepare()
        => throw new NotSupportedException(NpgsqlStrings.NotSupportedOnDataSourceCommand);

    public override Task PrepareAsync(CancellationToken cancellationToken = default)
        => throw new NotSupportedException(NpgsqlStrings.NotSupportedOnDataSourceCommand);

    protected override DbConnection? DbConnection
    {
        get => throw new NotSupportedException(NpgsqlStrings.NotSupportedOnDataSourceCommand);
        set => throw new NotSupportedException(NpgsqlStrings.NotSupportedOnDataSourceCommand);
    }

    protected override DbTransaction? DbTransaction
    {
        get => throw new NotSupportedException(NpgsqlStrings.NotSupportedOnDataSourceCommand);
        set => throw new NotSupportedException(NpgsqlStrings.NotSupportedOnDataSourceCommand);
    }
}
