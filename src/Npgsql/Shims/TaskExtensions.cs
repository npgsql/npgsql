#if !NET6_0_OR_GREATER
using System.Collections.Generic;

namespace System.Threading.Tasks;

static class TaskExtensions
{
    /// <summary>
    /// Gets a <see cref="Task"/> that will complete when this <see cref="Task"/> completes, when the specified timeout expires, or when the specified <see cref="CancellationToken"/> has cancellation requested.
    /// </summary>
    /// <param name="task">The <see cref="Task"/> representing the asynchronous wait.</param>
    /// <param name="timeout">The timeout after which the <see cref="Task"/> should be faulted with a <see cref="TimeoutException"/> if it hasn't otherwise completed.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for a cancellation request.</param>
    /// <returns>The <see cref="Task"/> representing the asynchronous wait.</returns>
    /// <remarks>This method reproduces new to the .NET 6.0 API <see cref="Task"/>.WaitAsync.</remarks>
    public static async Task WaitAsync(this Task task, TimeSpan timeout, CancellationToken cancellationToken)
    {
        var tasks = new List<Task>(3);

        Task? cancellationTask = default;
        CancellationTokenRegistration registration = default;
        if (cancellationToken.CanBeCanceled)
        {
            var tcs = new TaskCompletionSource<bool>();
            registration = cancellationToken.Register(s => ((TaskCompletionSource<bool>)s!).TrySetResult(true), tcs);
            cancellationTask = tcs.Task;
            tasks.Add(cancellationTask);
        }

        Task? delayTask = default;
        CancellationTokenSource? delayCts = default;
        if (timeout != Timeout.InfiniteTimeSpan)
        {
            var timeLeft = timeout;
            delayCts = new CancellationTokenSource();
            delayTask = Task.Delay(timeLeft, delayCts.Token);
            tasks.Add(delayTask);
        }

        try
        {
            if (tasks.Count != 0)
            {
                tasks.Add(task);
                var result = await Task.WhenAny(tasks).ConfigureAwait(false);
                if (result == cancellationTask)
                {
                    task = Task.FromCanceled(cancellationToken);
                }
                else if (result == delayTask)
                {
                    task = Task.FromException(new TimeoutException());
                }
            }
            await task.ConfigureAwait(false);
        }
        finally
        {
            delayCts?.Cancel();
            delayCts?.Dispose();
            registration.Dispose();
        }
    }
}
#endif
