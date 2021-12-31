using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Util;

namespace Npgsql;

static class TaskTimeoutAndCancellation
{
    /// <summary>
    /// Utility that executes a potentially non-cancellable <see cref="Task{TResult}"/> while allowing to timeout and/or cancel awaiting for it.
    /// If the given task does not complete within <paramref name="timeout"/>, a <see cref="TimeoutException"/> is thrown.
    /// The executed <see cref="Task{TResult}"/> may be left in an incomplete state after the <see cref="Task{TResult}"/> that this method returns completes dues to timeout and/or cancellation request.
    /// The method guarantees that the abandoned, incomplete <see cref="Task{TResult}"/> is not going to produce <see cref="TaskScheduler.UnobservedTaskException"/> event if it eventually fails later.
    /// </summary>
    /// <param name="getTaskFunc">Gets the <see cref="Task{TResult}"/> for execution with a combined <see cref="CancellationToken"/> that attempts to cancel the <see cref="Task{TResult}"/> in an event of the timeout or external cancellation request.</param>
    /// <param name="timeout">The timeout after which the <see cref="Task{TResult}"/> should be faulted with a <see cref="TimeoutException"/> if it hasn't otherwise completed.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for a cancellation request.</param>
    /// <typeparam name="TResult">The result <see cref="Type"/>.</typeparam>
    /// <returns>The <see cref="Task{TResult}"/> representing the asynchronous wait.</returns>
    internal static async Task<TResult> ExecuteWithTimeoutAndCancellationAsync<TResult>(Func<CancellationToken, Task<TResult>> getTaskFunc, NpgsqlTimeout timeout, CancellationToken cancellationToken)
    {
        Task<TResult>? task = default;
        await ExecuteWithTimeoutAndCancellationAsync(ct => (Task)(task = getTaskFunc(ct)), timeout, cancellationToken);
        return await task!;
    }

    /// <summary>
    /// Utility that executes a potentially non-cancellable <see cref="Task"/> while allowing to timeout and/or cancel awaiting for it.
    /// If the given task does not complete within <paramref name="timeout"/>, a <see cref="TimeoutException"/> is thrown.
    /// The executed <see cref="Task"/> may be left in an incomplete state after the <see cref="Task"/> that this method returns completes dues to timeout and/or cancellation request.
    /// The method guarantees that the abandoned, incomplete <see cref="Task"/> is not going to produce <see cref="TaskScheduler.UnobservedTaskException"/> event if it eventually fails later.
    /// </summary>
    /// <param name="getTaskFunc">Gets the <see cref="Task"/> for execution with a combined <see cref="CancellationToken"/> that attempts to cancel the <see cref="Task"/> in an event of the timeout or external cancellation request.</param>
    /// <param name="timeout">The timeout after which the <see cref="Task"/> should be faulted with a <see cref="TimeoutException"/> if it hasn't otherwise completed.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for a cancellation request.</param>
    /// <returns>The <see cref="Task"/> representing the asynchronous wait.</returns>
    internal static async Task ExecuteWithTimeoutAndCancellationAsync(Func<CancellationToken, Task> getTaskFunc, NpgsqlTimeout timeout,
        CancellationToken cancellationToken)
    {
        using var combinedCts = timeout.IsSet ? CancellationTokenSource.CreateLinkedTokenSource(cancellationToken) : null;
        var task = getTaskFunc(combinedCts?.Token ?? cancellationToken);
        try
        {
            try
            {
                await task.WaitAsync(timeout.CheckAndGetTimeLeft(), cancellationToken);
            }
            catch (TimeoutException) when (!task!.IsCompleted)
            {
                // Attempt to stop the Task in progress.
                combinedCts?.Cancel();
                throw;
            }
        }
        catch
        {
            // Prevent unobserved Task notifications by observing the failed Task exception.
            // To test: comment the next line out and re-run TaskExtensionsTest.DelayedFaultedTaskCancellation.
            _ = task.ContinueWith(t => _ = t.Exception, CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted,
                TaskScheduler.Current);
            throw;
        }
    }

#if !NET6_0_OR_GREATER
    /// <summary>
    /// Gets a <see cref="Task"/> that will complete when this <see cref="Task"/> completes, when the specified timeout expires, or when the specified <see cref="CancellationToken"/> has cancellation requested.
    /// </summary>
    /// <param name="task">The <see cref="Task"/> representing the asynchronous wait.</param>
    /// <param name="timeout">The timeout after which the <see cref="Task"/> should be faulted with a <see cref="TimeoutException"/> if it hasn't otherwise completed.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for a cancellation request.</param>
    /// <returns>The <see cref="Task"/> representing the asynchronous wait.  It may or may not be the same instance as the current instance.</returns>
    static async Task WaitAsync(this Task task, TimeSpan timeout, CancellationToken cancellationToken)
    {
        var tasks = new List<Task>();

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
                var result = await Task.WhenAny(tasks);
                if (result == cancellationTask)
                {
                    task = Task.FromCanceled(cancellationToken);
                }
                else if (result == delayTask)
                {
                    task = Task.FromException(new TimeoutException());
                }
            }
            await task;
        }
        finally
        {
            delayCts?.Cancel();
            delayCts?.Dispose();
            registration.Dispose();
        }
    }
#endif
}