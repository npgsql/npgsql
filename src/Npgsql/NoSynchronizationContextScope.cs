using System;
using System.Threading;

namespace Npgsql
{
    /// <summary>
    /// This mechanism is used to temporarily set the current synchronization context to null while
    /// executing Npgsql code, making all await continuations execute on the thread pool. This replaces
    /// the need to place ConfigureAwait(false) everywhere, and should be used in all surface async methods,
    /// without exception.
    ///
    /// Warning: do not use this directly in async methods, use it in sync wrappers of async methods
    /// (see https://github.com/npgsql/npgsql/issues/1593)
    /// </summary>
    /// <remarks>
    /// https://stackoverflow.com/a/28307965/640325
    /// </remarks>
    static class NoSynchronizationContextScope
    {
        internal static Disposable Enter() => new(SynchronizationContext.Current);

        internal struct Disposable : IDisposable
        {
            readonly SynchronizationContext? _synchronizationContext;

            internal Disposable(SynchronizationContext? synchronizationContext)
            {
                if (synchronizationContext != null)
                    SynchronizationContext.SetSynchronizationContext(null);

                _synchronizationContext = synchronizationContext;
            }

            public void Dispose()
                => SynchronizationContext.SetSynchronizationContext(_synchronizationContext);
        }
    }
}
