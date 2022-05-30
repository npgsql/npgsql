using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal;

namespace Npgsql;

/// <inheritdoc />
public class NpgsqlBatch : DbBatch
{
    internal const int DefaultBatchCommandsSize = 5;

    private protected NpgsqlCommand Command { get; }

    /// <inheritdoc />
    protected override DbBatchCommandCollection DbBatchCommands => BatchCommands;

    /// <inheritdoc cref="DbBatch.BatchCommands"/>
    public new NpgsqlBatchCommandCollection BatchCommands { get; }

    /// <inheritdoc />
    public override int Timeout
    {
        get => Command.CommandTimeout;
        set => Command.CommandTimeout = value;
    }

    /// <inheritdoc cref="DbBatch.Connection"/>
    public new NpgsqlConnection? Connection
    {
        get => Command.Connection;
        set => Command.Connection = value;
    }

    /// <inheritdoc />
    protected override DbConnection? DbConnection
    {
        get => Connection;
        set => Connection = (NpgsqlConnection?)value;
    }

    /// <inheritdoc cref="DbBatch.Transaction"/>
    public new NpgsqlTransaction? Transaction
    {
        get => Command.Transaction;
        set => Command.Transaction = value;
    }

    /// <inheritdoc />
    protected override DbTransaction? DbTransaction
    {
        get => Transaction;
        set => Transaction = (NpgsqlTransaction?)value;
    }

    /// <summary>
    /// Marks all of the batch's result columns as either known or unknown.
    /// Unknown results column are requested them from PostgreSQL in text format, and Npgsql makes no
    /// attempt to parse them. They will be accessible as strings only.
    /// </summary>
    internal bool AllResultTypesAreUnknown
    {
        get => Command.AllResultTypesAreUnknown;
        set => Command.AllResultTypesAreUnknown = value;
    }

    /// <summary>
    /// Initializes a new <see cref="NpgsqlBatch"/>.
    /// </summary>
    /// <param name="connection">A <see cref="NpgsqlConnection"/> that represents the connection to a PostgreSQL server.</param>
    /// <param name="transaction">The <see cref="NpgsqlTransaction"/> in which the <see cref="NpgsqlCommand"/> executes.</param>
    public NpgsqlBatch(NpgsqlConnection? connection = null, NpgsqlTransaction? transaction = null)
    {
        Command = new(DefaultBatchCommandsSize);
        BatchCommands = new NpgsqlBatchCommandCollection(Command.InternalBatchCommands);

        Connection = connection;
        Transaction = transaction;
    }

    internal NpgsqlBatch(NpgsqlConnector connector)
    {
        Command = new(connector, DefaultBatchCommandsSize);
        BatchCommands = new NpgsqlBatchCommandCollection(Command.InternalBatchCommands);
    }

    private protected NpgsqlBatch(NpgsqlDataSourceCommand command)
    {
        Command = command;
        BatchCommands = new NpgsqlBatchCommandCollection(Command.InternalBatchCommands);
    }

    /// <inheritdoc />
    protected override DbBatchCommand CreateDbBatchCommand()
        => new NpgsqlBatchCommand();

    /// <inheritdoc />
    protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        => ExecuteReader(behavior);

    /// <inheritdoc cref="DbBatch.ExecuteReader"/>
    public new NpgsqlDataReader ExecuteReader(CommandBehavior behavior = CommandBehavior.Default)
        => Command.ExecuteReader(behavior);

    /// <inheritdoc />
    protected override async Task<DbDataReader> ExecuteDbDataReaderAsync(
        CommandBehavior behavior,
        CancellationToken cancellationToken)
        => await ExecuteReaderAsync(behavior, cancellationToken);

    /// <inheritdoc cref="DbBatch.ExecuteReaderAsync(CancellationToken)"/>
    public new Task<NpgsqlDataReader> ExecuteReaderAsync(CancellationToken cancellationToken = default)
        => Command.ExecuteReaderAsync(cancellationToken);

    /// <inheritdoc cref="DbBatch.ExecuteReaderAsync(CommandBehavior,CancellationToken)"/>
    public new Task<NpgsqlDataReader> ExecuteReaderAsync(
        CommandBehavior behavior,
        CancellationToken cancellationToken = default)
        => Command.ExecuteReaderAsync(behavior, cancellationToken);

    /// <inheritdoc />
    public override int ExecuteNonQuery()
        => Command.ExecuteNonQuery();

    /// <inheritdoc />
    public override Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken = default)
        => Command.ExecuteNonQueryAsync(cancellationToken);

    /// <inheritdoc />
    public override object? ExecuteScalar()
        => Command.ExecuteScalar();

    /// <inheritdoc />
    public override Task<object?> ExecuteScalarAsync(CancellationToken cancellationToken = default)
        => Command.ExecuteScalarAsync(cancellationToken);

    /// <inheritdoc />
    public override void Prepare()
        => Command.Prepare();

    /// <inheritdoc />
    public override Task PrepareAsync(CancellationToken cancellationToken = default)
        => Command.PrepareAsync(cancellationToken);

    /// <inheritdoc />
    public override void Cancel() => Command.Cancel();
}