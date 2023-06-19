using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Npgsql.Internal.Converters;

// Using a function pointer here is safe against assembly unloading as the instance reference that the static pointer method lives on is passed along.
// As such the instance cannot be collected by the gc which means the entire assembly is prevented from unloading until we're done.
static class AsyncHelpers
{
    public static async void AwaitTask(Task task, CompletionSource tcs, Continuation continuation)
    {
        try
        {
            await task;
            continuation.Invoke(task, tcs);
        }
        catch (Exception ex)
        {
            tcs.SetException(ex);
        }
        // Guarantee the type stays loaded until the function pointer call is done.
        GC.KeepAlive(continuation.Handle);
    }

    public abstract class CompletionSource
    {
        public abstract void SetException(Exception exception);
    }

    public sealed class CompletionSource<T> : CompletionSource
    {
#if NETSTANDARD
        AsyncValueTaskMethodBuilder<T> _amb = AsyncValueTaskMethodBuilder<T>.Create();
#else
        PoolingAsyncValueTaskMethodBuilder<T> _amb = PoolingAsyncValueTaskMethodBuilder<T>.Create();
#endif
        public ValueTask<T> Task => _amb.Task;

        public void SetResult(T value)
            => _amb.SetResult(value);

        public override void SetException(Exception exception)
            => _amb.SetException(exception);
    }

    // Split out into a struct as unsafe and async don't mix, while we do want a nicely typed function pointer signature to prevent mistakes.
    public readonly unsafe struct Continuation
    {
        public object Handle { get; }
        readonly delegate*<Task, CompletionSource, void> _continuation;

        /// <param name="handle">A reference to the type that houses the static method <see ref="continuation"/> points to.</param>
        /// <param name="continuation">The continuation</param>
        public Continuation(object handle, delegate*<Task, CompletionSource, void> continuation)
        {
            Handle = handle;
            _continuation = continuation;
        }

        public void Invoke(Task task, CompletionSource tcs) => _continuation(task, tcs);
    }
}
