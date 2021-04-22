using System;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Util;

namespace Npgsql
{
    static class TaskExtensions
    {
        /// <summary>
        /// Utility that simplifies awaiting a task with a timeout. If the given task does not
        /// complete within <paramref name="timeout"/>, a <see cref="TimeoutException"/> is thrown.
        /// </summary>
        /// <param name="task">The task to be awaited</param>
        /// <param name="timeout">How much time to allow <paramref name="task"/> to complete before throwing a <see cref="TimeoutException"/></param>
        /// <returns>An awaitable task that represents the original task plus the timeout</returns>
        internal static async Task<T> WithTimeout<T>(this Task<T> task, NpgsqlTimeout timeout)
        {
            if (!timeout.IsSet)
                return await task;
            var timeLeft = timeout.CheckAndGetTimeLeft();
            if (task != await Task.WhenAny(task, Task.Delay(timeLeft)))
                throw new TimeoutException();
            return await task;
        }

        /// <summary>
        /// Allows you to cancel awaiting for a non-cancellable task.
        /// </summary>
        /// <remarks>
        /// Read https://blogs.msdn.com/b/pfxteam/archive/2012/10/05/how-do-i-cancel-non-cancelable-async-operations.aspx
        /// and be very careful with this.
        /// </remarks>
        internal static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            using (cancellationToken.Register(s => ((TaskCompletionSource<bool>)s!).TrySetResult(true), tcs))
                if (task != await Task.WhenAny(task, tcs.Task))
                    throw new TaskCanceledException(task);
            return await task;
        }

        internal static Task<T> WithCancellationAndTimeout<T>(this Task<T> task, NpgsqlTimeout timeout, CancellationToken cancellationToken)
            => task.WithCancellation(cancellationToken).WithTimeout(timeout);

#if !NET5_0_OR_GREATER
        /// <summary>
        /// Utility that simplifies awaiting a task with a timeout. If the given task does not
        /// complete within <paramref name="timeout"/>, a <see cref="TimeoutException"/> is thrown.
        /// </summary>
        /// <param name="task">The task to be awaited</param>
        /// <param name="timeout">How much time to allow <paramref name="task"/> to complete before throwing a <see cref="TimeoutException"/></param>
        /// <returns>An awaitable task that represents the original task plus the timeout</returns>
        internal static async Task WithTimeout(this Task task, NpgsqlTimeout timeout)
        {
            if (!timeout.IsSet)
            {
                await task;
                return;
            }
            var timeLeft = timeout.CheckAndGetTimeLeft();
            if (task != await Task.WhenAny(task, Task.Delay(timeLeft)))
                throw new TimeoutException();
            await task;
        }

        /// <summary>
        /// Allows you to cancel awaiting for a non-cancellable task.
        /// </summary>
        /// <remarks>
        /// Read https://blogs.msdn.com/b/pfxteam/archive/2012/10/05/how-do-i-cancel-non-cancelable-async-operations.aspx
        /// and be very careful with this.
        /// </remarks>
        internal static async Task WithCancellation(this Task task, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            using (cancellationToken.Register(s => ((TaskCompletionSource<bool>)s!).TrySetResult(true), tcs))
                if (task != await Task.WhenAny(task, tcs.Task))
                    throw new TaskCanceledException(task);
            await task;
        }

        internal static Task WithCancellationAndTimeout(this Task task, NpgsqlTimeout timeout, CancellationToken cancellationToken)
            => task.WithCancellation(cancellationToken).WithTimeout(timeout);
#endif

        internal static async Task ExecuteWithTimeout(Func<CancellationToken, Task> func, NpgsqlTimeout timeout, CancellationToken cancellationToken)
        {
            CancellationTokenSource? combinedCts = null;
            try
            {
                var combinedCancellationToken = GetCombinedCancellationToken(ref combinedCts, timeout, cancellationToken);
                await func(combinedCancellationToken);
            }
            finally
            {
                combinedCts?.Dispose();
            }
        }

        internal static async Task<TResult> ExecuteWithTimeout<TResult>(Func<CancellationToken, Task<TResult>> func, NpgsqlTimeout timeout, CancellationToken cancellationToken)
        {
            CancellationTokenSource? combinedCts = null;
            try
            {
                var combinedCancellationToken = GetCombinedCancellationToken(ref combinedCts, timeout, cancellationToken);
                return await func(combinedCancellationToken);
            }
            finally
            {
                combinedCts?.Dispose();
            }
        }

        static CancellationToken GetCombinedCancellationToken(ref CancellationTokenSource? combinedCts, NpgsqlTimeout timeout, CancellationToken cancellationToken)
        {
            var finalCt = cancellationToken;

            if (timeout.IsSet)
            {
                combinedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                combinedCts.CancelAfter((int)timeout.CheckAndGetTimeLeft().TotalMilliseconds);
                finalCt = combinedCts.Token;
            }

            return finalCt;
        }
    }
}
