using System;
using System.Diagnostics;
using System.Threading;
using static System.Threading.Timeout;

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
        CancellationToken _linkedToken;

#if DEBUG
        bool _isRunning;
#endif

        public TimeoutCancellationTokenSourceWrapper() => Timeout = InfiniteTimeSpan;

        public TimeoutCancellationTokenSourceWrapper(TimeSpan timeout) => Timeout = timeout;

        /// <summary>
        /// Set the timeout on the wrapped <see cref="CancellationTokenSource"/>
        /// and make sure that it hasn't been cancelled yet
        /// </summary>
        /// <param name="cancellationToken">An optional cancellation token that will be linked with the
        /// contained <see cref="CancellationTokenSource"/></param>
        /// <returns>The <see cref="CancellationToken"/> from the wrapped <see cref="CancellationTokenSource"/></returns>
        public CancellationToken Start(CancellationToken cancellationToken = default)
        {
#if DEBUG
            Debug.Assert(!_isRunning);
#endif

            // If we already wrap a linked token source or we want to wrap a linked token source and the token to
            // link isn't the same one that's already linked we always have to create a new CancellationTokenSource
            if ((_linkedToken.CanBeCanceled || cancellationToken.CanBeCanceled) && _linkedToken != cancellationToken)
            {
                _timeoutCts.Dispose();
                _timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                _timeoutCts.CancelAfter(Timeout);
            }
            else
            {
                _timeoutCts.CancelAfter(Timeout);
                if (_timeoutCts.IsCancellationRequested)
                {
                    _timeoutCts.Dispose();
                    _timeoutCts = new CancellationTokenSource(Timeout);
                }
            }

            _linkedToken = cancellationToken;
#if DEBUG
            _isRunning = true;
#endif
            return _timeoutCts.Token;
        }

        /// <summary>
        /// Restart the timeout on the wrapped <see cref="CancellationTokenSource"/> without reinitializing it,
        /// even if <see cref="IsCancellationRequested"/> is already set to <see langword="true"/>
        /// </summary>
        public void RestartTimeoutWithoutReset() => _timeoutCts.CancelAfter(Timeout);

        /// <summary>
        /// Reset the wrapper to contain a unstarted and uncancelled <see cref="CancellationTokenSource"/>
        /// in order make sure the next call to <see cref="Start"/> will not invalidate
        /// the cancellation token.
        /// </summary>
        /// <param name="cancellationToken">An optional cancellation token that will be linked with the
        /// contained <see cref="CancellationTokenSource"/></param>
        /// <returns>The <see cref="CancellationToken"/> from the wrapped <see cref="CancellationTokenSource"/></returns>
        public CancellationToken Reset(CancellationToken cancellationToken = default)
        {
            if ((_linkedToken.CanBeCanceled || cancellationToken.CanBeCanceled) && _linkedToken != cancellationToken)
            {
                _timeoutCts.Dispose();
                _timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            }
            else
            {
                _timeoutCts.CancelAfter(InfiniteTimeSpan);
                if (_timeoutCts.IsCancellationRequested)
                {
                    _timeoutCts.Dispose();
                    _timeoutCts = new CancellationTokenSource();
                }
            }

            _linkedToken = cancellationToken;
#if DEBUG
            _isRunning = false;
#endif
            return _timeoutCts.Token;
        }

        /// <summary>
        /// Set the timeout on the wrapped <see cref="CancellationTokenSource"/>
        /// to <see cref="System.Threading.Timeout.InfiniteTimeSpan"/>
        /// </summary>
        /// <remarks>
        /// <see cref="IsCancellationRequested"/> can still arrive at a state
        /// where it's value is <see langword="true"/> if the <see cref="CancellationToken"/>
        /// passed to <see cref="Start"/> gets a cancellation request.
        /// If this is the case it will be resolved upon the next call to <see cref="Start"/>
        /// or <see cref="Reset"/>
        /// </remarks>
        public void Stop()
        {
#if DEBUG
            Debug.Assert(_isRunning);
#endif

            _timeoutCts.CancelAfter(InfiniteTimeSpan);
#if DEBUG
            _isRunning = false;
#endif
        }

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

        public bool IsCancellationRequested => _timeoutCts.IsCancellationRequested;

        public void Dispose() => _timeoutCts.Dispose();
    }
}
