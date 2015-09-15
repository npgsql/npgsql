using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql
{
    internal static class TaskExtensions
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
            var timeoutTask = Task.Delay(timeout.TimeLeft);
            if (task != await Task.WhenAny(task, timeoutTask))
                throw new TimeoutException();
            return await task;
        }

        /// <summary>
        /// Utility that simplifies awaiting a task with a timeout. If the given task does not
        /// complete within <paramref name="timeout"/>, a <see cref="TimeoutException"/> is thrown.
        /// </summary>
        /// <param name="task">The task to be awaited</param>
        /// <param name="timeout">How much time to allow <paramref name="task"/> to complete before throwing a <see cref="TimeoutException"/></param>
        /// <returns>An awaitable task that represents the original task plus the timeout</returns>
        internal static async Task WithTimeout(this Task task, NpgsqlTimeout timeout)
        {
            var timeoutTask = Task.Delay(timeout.TimeLeft);
            if (task != await Task.WhenAny(task, timeoutTask))
                throw new TimeoutException();
            await task;
        }

        /// <summary>
        /// Allows you to cancel awaiting for a non-cancellable task.
        /// </summary>
        /// <remarks>
        /// Read http://blogs.msdn.com/b/pfxteam/archive/2012/10/05/how-do-i-cancel-non-cancelable-async-operations.aspx
        /// and be very careful with this.
        /// </remarks>
        internal static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            using (cancellationToken.Register(
                        s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
                if (task != await Task.WhenAny(task, tcs.Task))
                    throw new TaskCanceledException(task);
            return await task;
        }

        /// <summary>
        /// Allows you to cancel awaiting for a non-cancellable task.
        /// </summary>
        /// <remarks>
        /// Read http://blogs.msdn.com/b/pfxteam/archive/2012/10/05/how-do-i-cancel-non-cancelable-async-operations.aspx
        /// and be very careful with this.
        /// </remarks>
        internal static async Task WithCancellation(this Task task, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            using (cancellationToken.Register(
                        s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
                if (task != await Task.WhenAny(task, tcs.Task))
                    throw new TaskCanceledException(task);
            await task;
        }

        internal static Task<T> WithCancellationAndTimeout<T>(this Task<T> task, NpgsqlTimeout timeout, CancellationToken cancellationToken)
        {
            return task
                .WithCancellation(cancellationToken)
                .WithTimeout(timeout);
        }

        internal static Task WithCancellationAndTimeout(this Task task, NpgsqlTimeout timeout, CancellationToken cancellationToken)
        {
            return task
                .WithCancellation(cancellationToken)
                .WithTimeout(timeout);
        }
    }
}
