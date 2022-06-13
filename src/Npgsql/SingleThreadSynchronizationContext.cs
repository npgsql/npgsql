﻿using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Npgsql;

sealed class SingleThreadSynchronizationContext : SynchronizationContext, IDisposable
{
    readonly BlockingCollection<CallbackAndState> _tasks = new();
    readonly object _lockObject = new();
    volatile Thread? _thread;
    int _doingWork;

    const int ThreadStayAliveMs = 10000;
    readonly string _threadName;

    static readonly ILogger Logger = NpgsqlLoggingConfiguration.ConnectionLogger;

    internal SingleThreadSynchronizationContext(string threadName)
        => _threadName = threadName;

    internal Disposable Enter() => new(this);

    public override void Post(SendOrPostCallback callback, object? state)
    {
        _tasks.Add(new CallbackAndState { Callback = callback, State = state });

        lock (_lockObject)
        {
            if (_doingWork == 0)
            {
                // Either there is no thread, or the current thread is exiting
                // In which case, wait for it to complete
                var currentThread = _thread;
                currentThread?.Join();
                Debug.Assert(_thread is null);
                _thread = new Thread(WorkLoop) { Name = _threadName, IsBackground = true };
                _thread.Start();
            }
        }
    }

    public void Dispose()
    {
        _tasks.CompleteAdding();

        var thread = _thread;
        thread?.Join();

        _tasks.Dispose();
    }

    void WorkLoop()
    {
        SetSynchronizationContext(this);

        try
        {
            while (true)
            {
                var taken = _tasks.TryTake(out var callbackAndState, ThreadStayAliveMs);
                if (!taken)
                {
                    lock (_lockObject)
                    {
                        if (_tasks.Count == 0)
                        {
                            _doingWork = 0;
                            return;
                        }
                    }

                    continue;
                }

                try
                {
                    Debug.Assert(_doingWork == 1);
                    callbackAndState.Callback(callbackAndState.State);
                }
                catch (Exception e)
                {
                    Logger.LogError(e, $"Exception caught in {nameof(SingleThreadSynchronizationContext)}");
                }
            }
        }
        catch (Exception e)
        {
            // Here we attempt to catch any exception comming from BlockingCollection _tasks
            Logger.LogError(e, $"Exception caught in {nameof(SingleThreadSynchronizationContext)}");
            _doingWork = 0;
        }
        finally
        {
            Debug.Assert(_doingWork == 0);
            _thread = null;
        }
    }

    struct CallbackAndState
    {
        internal SendOrPostCallback Callback;
        internal object? State;
    }

    internal readonly struct Disposable : IDisposable
    {
        readonly SynchronizationContext? _synchronizationContext;

        internal Disposable(SynchronizationContext synchronizationContext)
        {
            _synchronizationContext = Current;
            SetSynchronizationContext(synchronizationContext);
        }

        public void Dispose()
            => SetSynchronizationContext(_synchronizationContext);
    }
}