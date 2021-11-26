using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

using Npgsql.Util;

namespace Npgsql.Tests
{
    public class TaskExtensionsTest : TestBase
    {
        const int Value = 777;
        async Task<int> GetResultTaskAsync(int timeout, CancellationToken ct)
        {
            await Task.Delay(timeout, ct);
            return Value;
        }

        Task GetVoidTaskAsync(int timeout, CancellationToken ct) => Task.Delay(timeout, ct);

        [Test]
        public async Task SuccessfulResultTaskAsync() =>
            Assert.AreEqual(Value, await TaskExtensions.ExecuteWithCancellationAndTimeoutAsync(ct => GetResultTaskAsync(10, ct),
                NpgsqlTimeout.Infinite, CancellationToken.None));

        [Test]
        public async Task SuccessfulVoidTaskAsync() =>
            await TaskExtensions.ExecuteWithCancellationAndTimeoutAsync(ct => GetVoidTaskAsync(10, ct),
                NpgsqlTimeout.Infinite, CancellationToken.None);

        [Test]
        public void InfinitelyLongTaskTimeout() =>
            Assert.ThrowsAsync<TimeoutException>(async () =>
                await TaskExtensions.ExecuteWithCancellationAndTimeoutAsync(ct => GetVoidTaskAsync(Timeout.Infinite, ct),
                    new NpgsqlTimeout(TimeSpan.FromMilliseconds(10)), CancellationToken.None));

        [Test]
        public void InfinitelyLongTaskCancellation()
        {
            using var cts = new CancellationTokenSource(10);
            Assert.ThrowsAsync<TaskCanceledException>(async () =>
                await TaskExtensions.ExecuteWithCancellationAndTimeoutAsync(ct => GetVoidTaskAsync(Timeout.Infinite, ct),
                    NpgsqlTimeout.Infinite, cts.Token));
        }

        /// <summary>
        /// The test creates a delayed execution Task that is being fake-cancelled and fails subsequently and triggers 'TaskScheduler.UnobservedTaskException event'.
        /// </summary>
        [Theory, IssueLink("https://github.com/npgsql/npgsql/issues/4149")]
        [TestCase("CancelAndTimeout")]
        [TestCase("CancelOnly")]
        [TestCase("TimeoutOnly")]
        public Task DelayedFaultedTaskCancellation(string testCase) => RunDelayedFaultedTaskTestAsync(async getUnobservedTaskException =>
        {
            var cancel = true;
            var timeout = true;
            switch (testCase)
            {
                case "TimeoutOnly":
                    cancel = false;
                    break;
                case "CancelOnly":
                    timeout = false;
                    break;
            }

            var notifyDelayCompleted = new SemaphoreSlim(0, 1);

            // Invoke the method that creates a delayed execution Task that fails subsequently.
            await CreateTaskAndPreemptWithCancellationAsync(500, cancel, timeout, notifyDelayCompleted);

            // Wait enough time for the non-cancelable task to fail and then do the GC collect to trigger the finalizer.
            await notifyDelayCompleted.WaitAsync();
            var timeEnd = DateTime.UtcNow.AddSeconds(3);
            while (getUnobservedTaskException() is null && timeEnd > DateTime.UtcNow)
            {
                await Task.Delay(100);
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        });

        static async Task RunDelayedFaultedTaskTestAsync(Func<Func<Exception?>, Task> test)
        {
            Exception? unobservedTaskException = null;

            // Subscribe to UnobservedTaskException event to store the Exception, if any.
            void OnUnobservedTaskException(object? source, UnobservedTaskExceptionEventArgs args)
            {
                if (!args.Observed)
                {
                    args.SetObserved();
                }
                unobservedTaskException = args.Exception;
            }
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

            try
            {
                await test(() => unobservedTaskException);

                // Verify the unobserved Task exception event has not been received.
                Assert.IsNull(unobservedTaskException, unobservedTaskException?.Message);
            }
            finally
            {
                TaskScheduler.UnobservedTaskException -= OnUnobservedTaskException;
            }
        }

        /// <summary>
        /// Create a delayed execution, non-Cancellable Task that fails subsequently after the Task goes out of scope.
        /// </summary>
        static async Task CreateTaskAndPreemptWithCancellationAsync(int delayMs, bool cancel, bool timeout, SemaphoreSlim notifyDelayCompleted)
        {
            var nonCancellableTask = Task.Delay(delayMs, CancellationToken.None)
                .ContinueWith(
                    async _ =>
                    {
                        try
                        {
                            await Task.FromException(new Exception("Unobserved Task Test Exception"));
                        }
                        finally
                        {
                            notifyDelayCompleted.Release();
                        }
                    })
                .Unwrap();

            var timeoutMs = delayMs / 5;
            using var cts = cancel ? new CancellationTokenSource(timeoutMs) : null;
            try
            {
                await TaskExtensions.ExecuteWithCancellationAndTimeoutAsync(
                    _ => nonCancellableTask,
                    timeout ? new NpgsqlTimeout(TimeSpan.FromMilliseconds(timeoutMs)) : NpgsqlTimeout.Infinite,
                    cts?.Token ?? CancellationToken.None);
            }
            catch (TimeoutException)
            {
                // Expected due to preemptive time out.
            }
            catch (OperationCanceledException) when (cts?.IsCancellationRequested == true)
            {
                // Expected due to preemptive cancellation.
            }
            Assert.False(nonCancellableTask.IsCompleted);
        }
    }
}
