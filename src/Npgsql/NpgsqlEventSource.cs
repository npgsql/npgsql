using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Diagnostics.Tracing;
using System.Runtime.CompilerServices;

namespace Npgsql
{
    sealed class NpgsqlEventSource : EventSource
    {
        public static readonly NpgsqlEventSource Log = new NpgsqlEventSource();

        const string EventSourceName = "Npgsql";

        internal const int CommandStartId = 3;
        internal const int CommandStopId = 4;

#if !NETSTANDARD2_0
        IncrementingPollingCounter? _bytesWrittenPerSecondCounter;
        IncrementingPollingCounter? _bytesReadPerSecondCounter;

        IncrementingPollingCounter? _commandsPerSecondCounter;
        PollingCounter? _totalCommandsCounter;
        PollingCounter? _failedCommandsCounter;
        PollingCounter? _currentCommandsCounter;
        PollingCounter? _preparedCommandsRatioCounter;

        PollingCounter? _poolsCounter;
        PollingCounter? _idleConnectionsCounter;
        PollingCounter? _busyConnectionsCounter;

        PollingCounter? _multiplexingAverageCommandsPerBatchCounter;
        PollingCounter? _multiplexingAverageWaitsPerBatchCounter;
        PollingCounter? _multiplexingAverageWriteTimePerBatchCounter;

#endif
        long _bytesWritten;
        long _bytesRead;

        long _totalCommands;
        long _totalPreparedCommands;
        long _currentCommands;
        long _failedCommands;

        readonly object _poolsLock = new();
        readonly HashSet<ConnectorPool> _pools = new();

        long _multiplexingBatchesSent;
        long _multiplexingCommandsSent;
        long _multiplexingWaits;
        long _multiplexingTicksWritten;

        internal NpgsqlEventSource() : base(EventSourceName) {}

        // NOTE
        // - The 'Start' and 'Stop' suffixes on the following event names have special meaning in EventSource. They
        //   enable creating 'activities'.
        //   For more information, take a look at the following blog post:
        //   https://blogs.msdn.microsoft.com/vancem/2015/09/14/exploring-eventsource-activity-correlation-and-causation-features/
        // - A stop event's event id must be next one after its start event.

        internal void BytesWritten(long bytesWritten) => Interlocked.Add(ref _bytesWritten, bytesWritten);
        internal void BytesRead(long bytesRead) => Interlocked.Add(ref _bytesRead, bytesRead);

        public void CommandStart(string sql)
        {
            Interlocked.Increment(ref _totalCommands);
            Interlocked.Increment(ref _currentCommands);
            NpgsqlSqlEventSource.Log.CommandStart(sql);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void CommandStop()
        {
            Interlocked.Decrement(ref _currentCommands);
            NpgsqlSqlEventSource.Log.CommandStop();
        }

        internal void CommandStartPrepared() => Interlocked.Increment(ref _totalPreparedCommands);

        internal void CommandFailed() => Interlocked.Increment(ref _failedCommands);

        internal void PoolCreated(ConnectorPool pool)
        {
            lock (_poolsLock)
            {
                _pools.Add(pool);
            }
        }

        internal void MultiplexingBatchSent(int numCommands, int waits, Stopwatch stopwatch)
        {
            // TODO: CAS loop instead of 4 separate interlocked operations?
            Interlocked.Increment(ref _multiplexingBatchesSent);
            Interlocked.Add(ref _multiplexingCommandsSent, numCommands);
            Interlocked.Add(ref _multiplexingWaits, waits);
            Interlocked.Add(ref _multiplexingTicksWritten, stopwatch.ElapsedTicks);
        }

#if !NETSTANDARD2_0
        int GetIdleConnections()
        {
            // Note: there's no attempt here to be coherent in terms of race conditions, especially not with regards
            // to different counters. So idle and busy and be unsynchronized, as they're not polled together.
            lock (_poolsLock)
            {
                var sum = 0;
                foreach (var pool in _pools)
                {
                    sum += pool.Statistics.Idle;
                }
                return sum;
            }
        }

        int GetBusyConnections()
        {
            // Note: there's no attempt here to be coherent in terms of race conditions, especially not with regards
            // to different counters. So idle and busy and be unsynchronized, as they're not polled together.
            lock (_poolsLock)
            {
                var sum = 0;
                foreach (var pool in _pools)
                {
                    sum += pool.Statistics.Busy;
                }
                return sum;
            }
        }

        int GetPoolsCount()
        {
            lock (_poolsLock)
            {
                return _pools.Count;
            }
        }

        protected override void OnEventCommand(EventCommandEventArgs command)
        {
            if (command.Command == EventCommand.Enable)
            {
                // Comment taken from RuntimeEventSource in CoreCLR
                // NOTE: These counters will NOT be disposed on disable command because we may be introducing
                // a race condition by doing that. We still want to create these lazily so that we aren't adding
                // overhead by at all times even when counters aren't enabled.
                // On disable, PollingCounters will stop polling for values so it should be fine to leave them around.

                _bytesWrittenPerSecondCounter = new IncrementingPollingCounter("bytes-written-per-second", this, () => Interlocked.Read(ref _bytesWritten))
                {
                    DisplayName = "Bytes Written",
                    DisplayRateTimeScale = TimeSpan.FromSeconds(1)
                };

                _bytesReadPerSecondCounter = new IncrementingPollingCounter("bytes-read-per-second", this, () => Interlocked.Read(ref _bytesRead))
                {
                    DisplayName = "Bytes Read",
                    DisplayRateTimeScale = TimeSpan.FromSeconds(1)
                };

                _commandsPerSecondCounter = new IncrementingPollingCounter("commands-per-second", this, () => Interlocked.Read(ref _totalCommands))
                {
                    DisplayName = "Command Rate",
                    DisplayRateTimeScale = TimeSpan.FromSeconds(1)
                };

                _totalCommandsCounter = new PollingCounter("total-commands", this, () => Interlocked.Read(ref _totalCommands))
                {
                    DisplayName = "Total Commands",
                };

                _currentCommandsCounter = new PollingCounter("current-commands", this, () => Interlocked.Read(ref _currentCommands))
                {
                    DisplayName = "Current Commands"
                };

                _failedCommandsCounter = new PollingCounter("failed-commands", this, () => Interlocked.Read(ref _failedCommands))
                {
                    DisplayName = "Failed Commands"
                };

                _preparedCommandsRatioCounter = new PollingCounter(
                    "prepared-commands-ratio",
                    this,
                    () => (double)Interlocked.Read(ref _totalPreparedCommands) / Interlocked.Read(ref _totalCommands))
                {
                    DisplayName = "Prepared Commands Ratio",
                    DisplayUnits = "%"
                };

                _poolsCounter = new PollingCounter("connection-pools", this, () => GetPoolsCount())
                {
                    DisplayName = "Connection Pools"
                };

                _idleConnectionsCounter = new PollingCounter("idle-connections", this, () => GetIdleConnections())
                {
                    DisplayName = "Idle Connections"
                };

                _busyConnectionsCounter = new PollingCounter("busy-connections", this, () => GetBusyConnections())
                {
                    DisplayName = "Busy Connections"
                };

                _multiplexingAverageCommandsPerBatchCounter = new PollingCounter("multiplexing-average-commands-per-batch", this, () => (double)Interlocked.Read(ref _multiplexingCommandsSent) / Interlocked.Read(ref _multiplexingBatchesSent))
                {
                    DisplayName = "Average commands per multiplexing batch"
                };

                _multiplexingAverageWaitsPerBatchCounter = new PollingCounter("multiplexing-average-waits-per-batch", this, () => (double)Interlocked.Read(ref _multiplexingWaits) / Interlocked.Read(ref _multiplexingBatchesSent))
                {
                    DisplayName = "Average waits per multiplexing batch"
                };

                _multiplexingAverageWriteTimePerBatchCounter = new PollingCounter("multiplexing-average-write-time-per-batch", this, () => (double)Interlocked.Read(ref _multiplexingTicksWritten) / Interlocked.Read(ref _multiplexingBatchesSent) / 1000)
                {
                    DisplayName = "Average write time per multiplexing batch (us)",
                    DisplayUnits = "us"
                };
            }
        }
#endif
    }
}
