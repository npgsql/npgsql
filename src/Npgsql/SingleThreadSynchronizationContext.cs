using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using JetBrains.Annotations;

namespace Npgsql
{
    sealed class SingleThreadSynchronizationContext : SynchronizationContext, IDisposable
    {
        readonly BlockingCollection<CallbackAndState> _tasks = new BlockingCollection<CallbackAndState>();
        readonly object _lockObject = new object();
        [CanBeNull]
        volatile Thread _thread;
        bool _doingWork;

        const int ThreadStayAliveMs = 10000;
        readonly string _threadName;

        internal SingleThreadSynchronizationContext(string threadName)
        {
            _threadName = threadName;
        }

        public override void Post(SendOrPostCallback callback, object state)
        {
            _tasks.Add(new CallbackAndState { Callback = callback, State = state });

            lock (_lockObject)
            {
                if (!_doingWork)
                {
                    // Either there is no thread, or the current thread is exiting
                    // In which case, wait for it to complete
                    var currentThread = _thread;
                    currentThread?.Join();
                    Debug.Assert(_thread is null);
                    _doingWork = true;
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
                                _doingWork = false;
                                return;
                            }
                        }

                        continue;
                    }

                    try
                    {
                        Debug.Assert(_doingWork);
                        callbackAndState.Callback(callbackAndState.State);
                    }
                    catch (Exception)
                    {
                        // No logging until 5.0
                    }
                }
            }
            catch (Exception)
            {
                // Here we attempt to catch any exception coming from BlockingCollection _tasks
                lock (_lockObject)
                    _doingWork = false;
            }
            finally
            {
                Debug.Assert(!_doingWork);
                _thread = null;
            }
        }

        struct CallbackAndState
        {
            internal SendOrPostCallback Callback;
            internal object State;
        }
    }
}
