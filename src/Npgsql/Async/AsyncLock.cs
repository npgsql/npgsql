using System;
using System.Threading;
using System.Threading.Tasks;
#pragma warning disable 1591

namespace Npgsql.Async
{
    /// <summary>
    /// AsyncLock 
    /// </summary>
    public class AsyncLock
    {
        private readonly AsyncSemaphore _semaphore;

        public AsyncLock()
        {
            _semaphore = new AsyncSemaphore(1);
        }

        public async Task LockAsync(Func<Task> asyncAction)
        {
            if (asyncAction == null) throw new ArgumentNullException("asyncAction");

            try
            {
                await _semaphore.WaitAsync();
                await asyncAction();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task LockAsync(Action action)
        {
            if (action == null) throw new ArgumentNullException("action");

            try
            {
                await _semaphore.WaitAsync();
                action();
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}