using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql
{
    /// <summary>
    /// Utility class for temporarily switching <see cref="SynchronizationContext"/> implementations.
    /// 
    /// This mechanism is used to temporarily set the current synchronization context to null while
    /// executing Npgsql code, making all await continuations execute on the thread pool. This replaces
    /// the need to place ConfigureAwait(false) everywhere, and should be used in all surface async methods,
    /// without exception.
    /// </summary>
    /// <remarks>
    /// Copied and slightly adjusted from Stephen Cleary's Nito.AsyncEx package
    /// (https://github.com/StephenCleary/AsyncEx/blob/master/src/Nito.AsyncEx.Tasks/SynchronizationContextSwitcher.cs)
    /// </remarks>
    sealed class SynchronizationContextSwitcher : IDisposable
    {
        /// <summary>
        /// The previous <see cref="SynchronizationContext"/>.
        /// </summary>
        private readonly SynchronizationContext _oldContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizationContextSwitcher"/> class, installing the new <see cref="SynchronizationContext"/>.
        /// </summary>
        /// <param name="newContext">The new <see cref="SynchronizationContext"/>. This can be <c>null</c> to remove an existing <see cref="SynchronizationContext"/>.</param>
        internal SynchronizationContextSwitcher(SynchronizationContext newContext)
        {
            _oldContext = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(newContext);
        }

        /// <summary>
        /// Restores the old <see cref="SynchronizationContext"/>.
        /// </summary>
        public void Dispose() => SynchronizationContext.SetSynchronizationContext(_oldContext);

        /// <summary>
        /// Executes a synchronous delegate without the current <see cref="SynchronizationContext"/>. The current context is restored when this function returns.
        /// </summary>
        /// <param name="action">The delegate to execute.</param>
        public static void NoContext(Action action)
        {
            using (new SynchronizationContextSwitcher(null))
                action();
        }

        /// <summary>
        /// Executes a synchronous or asynchronous delegate without the current <see cref="SynchronizationContext"/>. The current context is restored when this function synchronously returns.
        /// </summary>
        /// <param name="action">The delegate to execute.</param>
        public static T NoContext<T>(Func<T> action)
        {
            using (new SynchronizationContextSwitcher(null))
                return action();
        }
    }
}
