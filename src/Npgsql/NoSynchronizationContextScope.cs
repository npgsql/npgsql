using System;
using System.Threading;

namespace Npgsql;

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
public static class NoSynchronizationContextScope
{
    /// <summary>
    /// Enter a using block and temporarily disable the synchronization context.
    /// </summary>
    /// <returns>A disposable object that will restore the synchronization context when disposed.</returns>
    public static Disposable Enter() => new(SynchronizationContext.Current);

    /// <summary>
    /// A disposable struct that will restore the synchronization context when disposed.
    /// </summary>
    public struct Disposable : IDisposable
    {
        readonly SynchronizationContext? _synchronizationContext;

        internal Disposable(SynchronizationContext? synchronizationContext)
        {
            if (synchronizationContext != null)
                SynchronizationContext.SetSynchronizationContext(null);

            _synchronizationContext = synchronizationContext;
        }

        /// <summary>
        /// Restores the synchronization context to its previous value.
        /// </summary>
        public void Dispose()
            => SynchronizationContext.SetSynchronizationContext(_synchronizationContext);
    }
}