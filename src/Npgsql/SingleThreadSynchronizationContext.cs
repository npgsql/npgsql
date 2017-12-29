using System;
using System.Collections.Concurrent;
using System.Threading;
using JetBrains.Annotations;

namespace Npgsql
{
    sealed class SingleThreadSynchronizationContext : SynchronizationContext, IDisposable
    {
        readonly BlockingCollection<CallbackAndState> _tasks = new BlockingCollection<CallbackAndState>();
        [CanBeNull]
        Thread _thread;

        const int ThreadStayAliveMs = 10000;
        readonly string _threadName;

        internal SingleThreadSynchronizationContext(string threadName)
        {
            _threadName = threadName;
        }

        public override void Post(SendOrPostCallback callback, object state)
        {
            _tasks.Add(new CallbackAndState { Callback = callback, State = state });

            if (_thread == null)
            {
                lock (this)
                {
                    if (_thread != null)
                        return;
                    _thread = new Thread(WorkLoop) { Name = _threadName, IsBackground = true };
                    _thread.Start();
                }
            }
        }

        public void Dispose()
        {
            _tasks.CompleteAdding();
            lock (this)
            {
                _thread?.Join();
            }
        }

        void WorkLoop()
        {
            try
            {
                while (true)
                {
                    var taken = _tasks.TryTake(out var callbackAndState, ThreadStayAliveMs);
                    if (!taken)
                        return;
                    callbackAndState.Callback(callbackAndState.State);
                }
            }
            finally
            {
                lock (this) { _thread = null; }
            }
        }

        struct CallbackAndState
        {
            internal SendOrPostCallback Callback;
            internal object State;
        }
    }
}
