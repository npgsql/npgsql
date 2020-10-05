using System;
using System.Threading;

namespace Npgsql.Util
{
    /// <summary>
    /// Internal struct wrapping a <see cref="CancellationTokenSource"/> for timeouts.
    /// </summary>
    /// <remarks>
    /// Since there's no way to reset a <see cref="CancellationTokenSource"/> once it was cancelled,
    /// we need to make sure that an existing cancellation token source hasn't been cancelled,
    /// every time we start it (see https://github.com/dotnet/runtime/issues/4694).
    /// </remarks>
    class TimeoutCancellationTokenSourceWrapper : IDisposable
    {
        public TimeSpan Timeout { get; set; }
        CancellationTokenSource _timeoutCts = new CancellationTokenSource();

        public TimeoutCancellationTokenSourceWrapper(TimeSpan timeout) => Timeout = timeout;

        /// <summary>
        /// Set the timeout on the wrapped <see cref="CancellationTokenSource"/>
        /// and make sure that it hasn't been cancelled yet
        /// </summary>
        public void Start()
        {
            _timeoutCts.CancelAfter(Timeout);
            if (_timeoutCts.IsCancellationRequested)
            {
                _timeoutCts.Dispose();
                _timeoutCts = new CancellationTokenSource(Timeout);
            }
        }

        /// <summary>
        /// Reset the timeout on the wrapped <see cref="CancellationTokenSource"/>
        /// </summary>
        public void Restart() => _timeoutCts.CancelAfter(Timeout);

        /// <summary>
        /// Set the timeout on the wrapped <see cref="CancellationTokenSource"/>
        /// to <see cref="System.Threading.Timeout.InfiniteTimeSpan"/>
        /// </summary>
        public void Stop() => _timeoutCts.CancelAfter(System.Threading.Timeout.InfiniteTimeSpan);

        /// <summary>
        /// Cancel the wrapped <see cref="CancellationTokenSource"/>
        /// </summary>
        public void Cancel() => _timeoutCts.Cancel();

        /// <summary>
        /// The <see cref="CancellationToken"/> from the wrapped
        /// <see cref="CancellationTokenSource"/> .
        /// </summary>
        /// <remarks>
        /// The token is only valid after calling <see cref="Start"/>
        /// and before calling <see cref="Start"/> the next time.
        /// Otherwise you may end up with a token that has already been
        /// cancelled or belongs to a cancellation token source that has
        /// been disposed.
        /// </remarks>
        public CancellationToken Token => _timeoutCts.Token;

        public void Dispose() => _timeoutCts.Dispose();
    }
}
