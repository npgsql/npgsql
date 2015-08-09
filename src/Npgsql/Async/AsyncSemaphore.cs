using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Npgsql.Async
{
    /// <summary>
    /// AsyncSemaphore
    /// </summary>
    public class AsyncSemaphore
    {
        private readonly static Task Completed = Task.FromResult(true);
        private readonly Queue<TaskCompletionSource<bool>> _waiters = new Queue<TaskCompletionSource<bool>>();
        private int _currentCount; 

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="initialCount"></param>
        public AsyncSemaphore(int initialCount)
        {
            if (initialCount < 0) throw new ArgumentOutOfRangeException("initialCount"); 

            _currentCount = initialCount;
        }

        /// <summary>
        /// WaitAsync
        /// </summary>
        /// <returns></returns>
        public Task WaitAsync()
        {
            lock (_waiters)
            {
                if (_currentCount > 0)
                {
                    --_currentCount;
                    return Completed;
                }
                else
                {
                    var waiter = new TaskCompletionSource<bool>();
                    _waiters.Enqueue(waiter);
                    return waiter.Task;
                }
            } 
        }

        /// <summary>
        /// Release
        /// </summary>
        public void Release()
        {
            TaskCompletionSource<bool> toRelease = null;
            lock (_waiters)
            {
                if (_waiters.Count > 0)
                    toRelease = _waiters.Dequeue();
                else
                    ++_currentCount;
            }
            if (toRelease != null)
            {
                toRelease.SetResult(true);
            } 
        }
    }
}