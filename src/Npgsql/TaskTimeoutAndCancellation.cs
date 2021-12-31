using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Util;

namespace Npgsql;

static class TaskTimeoutAndCancellation
{
    /// <summary>
    /// Utility that executes a non-cancellable task while allowing to timeout and/or cancel awaiting for it.
    /// If the given task does not complete within <paramref name="timeout"/>, a <see cref="TimeoutException"/> is thrown.
    /// </summary>
    /// <param name="getTaskFunc">Gets a <see cref="Task{TResult}"/>.</param>
    /// <param name="timeout">The timeout after which the <see cref="Task"/> should be faulted with a <see cref="TimeoutException"/> if it hasn't otherwise completed.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for a cancellation request.</param>
    /// <typeparam name="TResult">The result <see cref="Type"/>.</typeparam>
    /// <returns>The <see cref="Task{TResult}"/> representing the asynchronous wait.</returns>
    internal static async Task<TResult> WaitAsync<TResult>(Func<CancellationToken, Task<TResult>> getTaskFunc, NpgsqlTimeout timeout, CancellationToken cancellationToken)
    {
        Task<TResult>? task = default;
        await WaitAsync(ct => (Task)(task = getTaskFunc(ct)), timeout, cancellationToken);
        return await task!;
    }

    /// <summary>
    /// Utility that executes a non-cancellable task while allowing to timeout and/or cancel awaiting for it.
    /// If the given task does not complete within <paramref name="timeout"/>, a <see cref="TimeoutException"/> is thrown.
    /// </summary>
    /// <param name="getTaskFunc">Gets a <see cref="Task"/>.</param>
    /// <param name="timeout">The timeout after which the <see cref="Task"/> should be faulted with a <see cref="TimeoutException"/> if it hasn't otherwise completed.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for a cancellation request.</param>
    /// <returns>The <see cref="Task"/> representing the asynchronous wait.</returns>
    internal static async Task WaitAsync(Func<CancellationToken, Task> getTaskFunc, NpgsqlTimeout timeout,
        CancellationToken cancellationToken)
    {
        using var combinedCts = timeout.IsSet ? CancellationTokenSource.CreateLinkedTokenSource(cancellationToken) : null;
        var task = getTaskFunc(combinedCts?.Token ?? cancellationToken);
        try
        {
            try
            {
                await WithCancellationAndTimeout(task, timeout, cancellationToken);
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

#if NET6_0_OR_GREATER
    static Task WithCancellationAndTimeout(Task task, NpgsqlTimeout timeout, CancellationToken cancellationToken) =>
        task.WaitAsync(timeout.CheckAndGetTimeLeft(), cancellationToken);
#else
    static async Task WithCancellationAndTimeout(Task task, NpgsqlTimeout timeout, CancellationToken cancellationToken)
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
        if (timeout.IsSet)
        {
            var timeLeft = timeout.CheckAndGetTimeLeft();
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