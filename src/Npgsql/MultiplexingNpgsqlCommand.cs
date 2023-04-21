using System;
using System.Data;
using System.Diagnostics;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Npgsql.Internal;
using Npgsql.Util;

namespace Npgsql;

sealed class MultiplexingNpgsqlCommand : NpgsqlDataSourceCommand
{
    readonly MultiplexingDataSource _dataSource;

    // TODO: Maybe pool these?
    internal ManualResetValueTaskSource<NpgsqlConnector> ExecutionCompletion { get; }
        = new();

    public MultiplexingNpgsqlCommand(MultiplexingDataSource dataSource) : base(null) => _dataSource = dataSource;

    internal override async ValueTask<NpgsqlDataReader> ExecuteReader(CommandBehavior behavior, bool async,
        CancellationToken cancellationToken)
    {
        // The waiting on the ExecutionCompletion ManualResetValueTaskSource is necessarily
        // asynchronous, so allowing sync would mean sync-over-async.
        if (!async)
            ThrowHelper.ThrowNotSupportedException("Synchronous command execution is not supported when multiplexing is on");

        NpgsqlDataReader? reader = null;

        try
        {
            if (IsWrappedByBatch)
            {
                foreach (var batchCommand in InternalBatchCommands)
                {
                    batchCommand.Parameters.ProcessParameters(_dataSource.TypeMapper, validateValues: true, CommandType);
                    ProcessRawQuery(null, standardConformingStrings: true, batchCommand);
                }
            }
            else
            {
                Parameters.ProcessParameters(_dataSource.TypeMapper, validateValues: true, CommandType);
                ProcessRawQuery(null, standardConformingStrings: true, batchCommand: null);
            }

            State = CommandState.InProgress;

            // TODO: Experiment: do we want to wait on *writing* here, or on *reading*?
            // Previous behavior was to wait on reading, which throw the exception from ExecuteReader (and not from
            // the first read). But waiting on writing would allow us to do sync writing and async reading.
            ExecutionCompletion.Reset();
            try
            {
                await _dataSource.MultiplexCommandWriter.WriteAsync(this, cancellationToken);
            }
            catch (ChannelClosedException ex)
            {
                Debug.Assert(ex.InnerException is not null);
                throw ex.InnerException;
            }
            var connector = await new ValueTask<NpgsqlConnector>(ExecutionCompletion, ExecutionCompletion.Version);

            reader = connector.DataReader;
            reader.Init(this, behavior, InternalBatchCommands);
            connector.CurrentReader = reader;
            await reader.NextResultAsync(cancellationToken);

            return reader;
        }
        catch (Exception e)
        {
            if (reader is not null)
                await reader.Cleanup(async: true, isDisposing: true);

            TraceSetException(e);

            State = CommandState.Idle;

            throw;
        }
    }

    protected override void Dispose(bool disposing) => State = CommandState.Disposed;
}
