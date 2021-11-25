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

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/4149")]
        public async Task SuccessfulResultTaskAsync() =>
            Assert.AreEqual(Value, await TaskExtensions.ExecuteWithCancellationAndTimeoutAsync(ct => GetResultTaskAsync(10, ct),
                NpgsqlTimeout.Infinite, CancellationToken.None));

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/4149")]
        public async Task SuccessfulVoidTaskAsync() =>
            await TaskExtensions.ExecuteWithCancellationAndTimeoutAsync(ct => GetVoidTaskAsync(10, ct),
                NpgsqlTimeout.Infinite, CancellationToken.None);

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/4149")]
        public void InfinitelyLongResultTaskTimeout() =>
            Assert.ThrowsAsync<TimeoutException>(async () =>
                await TaskExtensions.ExecuteWithCancellationAndTimeoutAsync(ct => GetResultTaskAsync(Timeout.Infinite, ct),
                    new NpgsqlTimeout(TimeSpan.FromMilliseconds(10)), CancellationToken.None));

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/4149")]
        public void InfinitelyLongVoidTaskTimeout() =>
            Assert.ThrowsAsync<TimeoutException>(async () =>
                await TaskExtensions.ExecuteWithCancellationAndTimeoutAsync(ct => GetVoidTaskAsync(Timeout.Infinite, ct),
                    new NpgsqlTimeout(TimeSpan.FromMilliseconds(10)), CancellationToken.None));

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/4149")]
        public void InfinitelyLongResultTaskCancellation()
        {
            using (var cts = new CancellationTokenSource(10))
                Assert.ThrowsAsync<TaskCanceledException>(async () =>
                    await TaskExtensions.ExecuteWithCancellationAndTimeoutAsync(ct => GetResultTaskAsync(Timeout.Infinite, ct),
                        NpgsqlTimeout.Infinite, cts.Token));
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/4149")]
        public void InfinitelyLongVoidTaskCancellation()
        {
            using (var cts = new CancellationTokenSource(10))
                Assert.ThrowsAsync<TaskCanceledException>(async () =>
                    await TaskExtensions.ExecuteWithCancellationAndTimeoutAsync(ct => GetVoidTaskAsync(Timeout.Infinite, ct),
                        NpgsqlTimeout.Infinite, cts.Token));
        }
    }
}
