using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Properties;

namespace Npgsql;

/// <summary>
/// Represents a command that is executed directly against a Npgsql data source
/// </summary>
public class NpgsqlDataSourceCommand : NpgsqlCommandOrig
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NpgsqlDataSourceCommand"/> class
    /// </summary>
    /// <param name="connection">An instance of <see cref="NpgsqlConnection"/></param>
    internal NpgsqlDataSourceCommand(NpgsqlConnection connection)
        : base(cmdText: null, connection)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NpgsqlDataSourceCommand"/> class
    /// </summary>
    /// <param name="batchCommandCapacity">The initial command capacity for batching commands.</param>
    /// <param name="connection">An instance of <see cref="NpgsqlConnection"/></param>
    internal NpgsqlDataSourceCommand(int batchCommandCapacity, NpgsqlConnection connection)
        : base(batchCommandCapacity, connection)
    {
    }

    /// <summary>
    /// Executes the command as reader and returns a <see cref="NpgsqlDataReaderOrig"/> object
    /// </summary>
    /// <param name="behavior">An instance of <see cref="CommandBehavior"/></param>
    /// <param name="async">True if the operation is async, false otherwise</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public override async ValueTask<NpgsqlDataReaderOrig> ExecuteReader(
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
    /// <summary>
    /// Not Supported for NpgsqlDataSourceCommand
    /// </summary>
    public override void Prepare()
        => throw new NotSupportedException(NpgsqlStrings.NotSupportedOnDataSourceCommand);

    /// <summary>
    /// Not Supported for NpgsqlDataSourceCommand
    /// </summary>
    public override Task PrepareAsync(CancellationToken cancellationToken = default)
        => throw new NotSupportedException(NpgsqlStrings.NotSupportedOnDataSourceCommand);

    /// <summary>
    /// Not Supported for NpgsqlDataSourceCommand
    /// </summary>
    protected override DbConnection? DbConnection
    {
        get => throw new NotSupportedException(NpgsqlStrings.NotSupportedOnDataSourceCommand);
        set => throw new NotSupportedException(NpgsqlStrings.NotSupportedOnDataSourceCommand);
    }

    /// <summary>
    /// Not Supported for NpgsqlDataSourceCommand
    /// </summary>
    protected override DbTransaction? DbTransaction
    {
        get => throw new NotSupportedException(NpgsqlStrings.NotSupportedOnDataSourceCommand);
        set => throw new NotSupportedException(NpgsqlStrings.NotSupportedOnDataSourceCommand);
    }
}
