using System;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Util;

namespace Npgsql;

static class TaskExtensions
{
    /// <summary>
    /// Utility that executes a non-cancellable task and allows you to timeout and cancel awaiting for it.
    /// If the given task does not complete within <paramref name="timeout"/>, a <see cref="TimeoutException"/> is thrown.
    /// </summary>
    /// <param name="getTaskFunc">Gets a <see cref="Task{TResult}"/>.</param>
    /// <param name="timeout">The timeout after which the <see cref="Task"/> should be faulted with a <see cref="TimeoutException"/> if it hasn't otherwise completed.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for a cancellation request.</param>
    /// <typeparam name="TResult">The result <see cref="Type"/>.</typeparam>
    /// <returns>The <see cref="Task{TResult}"/> representing the asynchronous wait.</returns>
    internal static Task<TResult> ExecuteWithCancellationAndTimeoutAsync<TResult>(Func<CancellationToken, Task<TResult>> getTaskFunc,
        NpgsqlTimeout timeout, CancellationToken cancellationToken)
        => ExecuteWithTimeout(
                getTaskFunc, timeout, cancellationToken)
            // As the cancellation support of getResultAsync is not guaranteed on all platforms/frameworks
            // we apply the fake-timeout/cancel mechanism in all cases.
            .WithCancellationAndTimeout(timeout, cancellationToken);

    /// <summary>
    /// Utility that executes a non-cancellable task and allows you to timeout and cancel awaiting for it.
    /// If the given task does not complete within <paramref name="timeout"/>, a <see cref="TimeoutException"/> is thrown.
    /// </summary>
    /// <param name="getTaskFunc">Gets a <see cref="Task"/>.</param>
    /// <param name="timeout">The timeout after which the <see cref="Task"/> should be faulted with a <see cref="TimeoutException"/> if it hasn't otherwise completed.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for a cancellation request.</param>
    /// <returns>The <see cref="Task"/> representing the asynchronous wait.</returns>
    internal static Task ExecuteWithCancellationAndTimeoutAsync(Func<CancellationToken, Task> getTaskFunc, NpgsqlTimeout timeout,
        CancellationToken cancellationToken)
        => ExecuteWithTimeout(
                ct => getTaskFunc(ct).ContinueWith(_ => true,
                    TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion), timeout,
                cancellationToken)
            // As the cancellation support of getResultAsync is not guaranteed on all platforms/frameworks
            // we apply the fake-timeout/cancel mechanism in all cases.
            .WithCancellationAndTimeout(timeout, cancellationToken);

#if NET6_0_OR_GREATER
        static Task<T> WithCancellationAndTimeout<T>(this Task<T> task, NpgsqlTimeout timeout, CancellationToken cancellationToken)
            => task.WaitAsync(timeout.CheckAndGetTimeLeft(), cancellationToken);
#else
    /// <summary>
    /// Utility that simplifies awaiting a task with a timeout. If the given task does not
    /// complete within <paramref name="timeout"/>, a <see cref="TimeoutException"/> is thrown.
    /// </summary>
    /// <param name="task">The task to be awaited</param>
    /// <param name="timeout">How much time to allow <paramref name="task"/> to complete before throwing a <see cref="TimeoutException"/></param>
    /// <returns>An awaitable task that represents the original task plus the timeout</returns>
    static async Task<T> WithTimeout<T>(this Task<T> task, NpgsqlTimeout timeout)
    {
        if (timeout.IsSet)
        {
            var timeLeft = timeout.CheckAndGetTimeLeft();
            if (task != await Task.WhenAny(task, Task.Delay(timeLeft)))
                throw new TimeoutException();
        }

        return await task;
    }

    /// <summary>
    /// Allows you to cancel awaiting for a non-cancellable task.
    /// </summary>
    /// <remarks>
    /// Read https://blogs.msdn.com/b/pfxteam/archive/2012/10/05/how-do-i-cancel-non-cancelable-async-operations.aspx
    /// and be very careful with this.
    /// </remarks>
    static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource<bool>();
        using (cancellationToken.Register(s => ((TaskCompletionSource<bool>)s!).TrySetResult(true), tcs))
            if (task != await Task.WhenAny(task, tcs.Task))
                // This treatment is more consistent with .NET6 Task.WaitAsync(CancellationToken) than throwing new TaskCancelledException(task). 
                task = Task.FromCanceled<T>(cancellationToken);
        return await task;
    }

    static Task<T> WithCancellationAndTimeout<T>(this Task<T> task, NpgsqlTimeout timeout, CancellationToken cancellationToken)
        => task.WithCancellation(cancellationToken).WithTimeout(timeout);
#endif

    static async Task<TResult> ExecuteWithTimeout<TResult>(Func<CancellationToken, Task<TResult>> func, NpgsqlTimeout timeout,
        CancellationToken cancellationToken)
    {
        CancellationTokenSource? combinedCts = null;
        try
        {
            var combinedCancellationToken = GetCombinedCancellationToken(ref combinedCts, timeout, cancellationToken);
            return await func(combinedCancellationToken);
        }
        catch (OperationCanceledException) when (combinedCts?.IsCancellationRequested == true && !cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException();
        }
        finally
        {
            combinedCts?.Dispose();
        }
    }

    static CancellationToken GetCombinedCancellationToken(ref CancellationTokenSource? combinedCts, NpgsqlTimeout timeout,
        CancellationToken cancellationToken)
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
