#if NETSTANDARD2_0
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace System.Threading
{
    // https://thomaslevesque.com/2015/06/04/async-and-cancellation-support-for-wait-handles/
    static class WaitHandleExtensions
    {
        internal static async Task<bool> WaitOneAsync(
            this WaitHandle handle, int millisecondsTimeout, CancellationToken cancellationToken  = default)
        {
            var tcs = new TaskCompletionSource<bool>();
            using var tokenRegistration =
                cancellationToken.Register(state => ((TaskCompletionSource<bool>)state!).TrySetCanceled(), tcs);

            RegisteredWaitHandle? registeredHandle = null;
            try
            {
                registeredHandle = ThreadPool.RegisterWaitForSingleObject(
                    handle,
                    (state, timedOut) => ((TaskCompletionSource<bool>)state!).TrySetResult(!timedOut),
                    state: tcs,
                    millisecondsTimeout,
                    executeOnlyOnce: true);
                return await tcs.Task;
            }
            finally
            {
                registeredHandle?.Unregister(null);
            }
        }

        internal static Task<bool> WaitOneAsync(this WaitHandle handle, TimeSpan timeout, CancellationToken cancellationToken  = default)
            => handle.WaitOneAsync((int)timeout.TotalMilliseconds, cancellationToken);

        internal static Task<bool> WaitOneAsync(this WaitHandle handle, CancellationToken cancellationToken = default)
            => handle.WaitOneAsync(Timeout.Infinite, cancellationToken);
    }
}

#endif
