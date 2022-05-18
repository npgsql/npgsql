using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Npgsql;

sealed class SingleThreadSynchronizationContext : SynchronizationContext, IDisposable
{
    readonly BlockingCollection<CallbackAndState> _tasks = new();
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

        if (Interlocked.CompareExchange(ref _doingWork, 1, 0) == 0)
        {
            // No one is working, let's wait for the thread to complete
            var currentThread = _thread;
            currentThread?.Join();
            Debug.Assert(_thread is null);
            _thread = new Thread(WorkLoop) { Name = _threadName, IsBackground = true };
            _thread.Start();
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
                    _doingWork = 0;
                    
                    // Ensure _doingWork is written before checking Count
                    Interlocked.MemoryBarrier();

                    if (_tasks.Count == 0)
                    {
                        return;
                    }

                    // There is new work, let's check whether someone's waiting for us to complete
                    if (Interlocked.Exchange(ref _doingWork, 1) == 1)
                    {
                        // There actually is someone waiting for us to complete, just exiting
                        return;
                    }
                    
                    continue;
                }
                callbackAndState.Callback(callbackAndState.State);
            }
        }
        catch (Exception e)
        {
            Logger.LogError(e, $"Exception caught in {nameof(SingleThreadSynchronizationContext)}");
            _doingWork = 0;
        }
        finally
        {
            _thread = null;
        }
    }

    struct CallbackAndState
    {
        internal SendOrPostCallback Callback;
        internal object? State;
    }

    internal struct Disposable : IDisposable
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