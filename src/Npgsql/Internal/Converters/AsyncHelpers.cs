using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Npgsql.Internal.Converters;

// Using a function pointer here is safe against assembly unloading as the instance reference that the static pointer method lives on is passed along.
// As such the instance cannot be collected by the gc which means the entire assembly is prevented from unloading until we're done.
class AsyncHelpers
{
    static AsyncHelpers Instance => new();

    static async void AwaitTask(Task task, CompletionSource tcs, Continuation continuation)
    {
        try
        {
            await task.ConfigureAwait(false);
            continuation.Invoke(task, tcs);
        }
        catch (Exception ex)
        {
            tcs.SetException(ex);
        }
        // Guarantee the type stays loaded until the function pointer call is done.
        GC.KeepAlive(continuation.Handle);
    }

    abstract class CompletionSource
    {
        public abstract void SetException(Exception exception);
    }

    sealed class CompletionSource<T> : CompletionSource
    {
        PoolingAsyncValueTaskMethodBuilder<T> _amb = PoolingAsyncValueTaskMethodBuilder<T>.Create();

        public ValueTask<T> Task => _amb.Task;

        public void SetResult(T value)
            => _amb.SetResult(value);

        public override void SetException(Exception exception)
            => _amb.SetException(exception);
    }

    // Split out into a struct as unsafe and async don't mix, while we do want a nicely typed function pointer signature to prevent mistakes.
    readonly unsafe struct Continuation
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

    public static unsafe ValueTask<T> UnboxAsync<T>(ValueTask<object> task)
    {
        if (task.IsCompletedSuccessfully)
            return new(result: (T)task.Result);

        if (!typeof(T).IsValueType)
        {
            Debug.Assert(Unsafe.SizeOf<T>() == sizeof(nint));
            // Justification: elides the async method bloat/perf cost to transition from object to T
            return Unsafe.As<ValueTask<object>, ValueTask<T>>(ref task);
        }

        // Otherwise we do one additional allocation, this allow us to share state machine codegen for all Ts.
        var source = new CompletionSource<T>();
        AwaitTask(task.AsTask(), source, new(Instance, &UnboxAndComplete));
        return source.Task;

        static void UnboxAndComplete(Task task, CompletionSource completionSource)
        {
            // Justification: exact type Unsafe.As used to reduce generic duplication cost.
            Debug.Assert(task is Task<object>);
            Debug.Assert(completionSource is CompletionSource<T>);
            Unsafe.As<CompletionSource<T>>(completionSource).SetResult((T)new ValueTask<object>(task: Unsafe.As<Task<object>>(task)).Result);
        }
    }

    // Separate method from UnboxAsync as unboxing a boxed (e.g.) int via (T)boxed where T is int? incurs an allocation.
    public static unsafe ValueTask<T?> ToNullableAsync<T>(ValueTask<T> task) where T : struct
    {
        if (task.IsCompletedSuccessfully)
            return new(result: task.Result);

        // Otherwise we do one additional allocation, this allow us to share state machine codegen for all Ts.
        var source = new CompletionSource<T?>();
        AwaitTask(task.AsTask(), source, new(Instance, &UnboxAndComplete));
        return source.Task;

        static void UnboxAndComplete(Task task, CompletionSource completionSource)
        {
            // Justification: exact type Unsafe.As used to reduce generic duplication cost.
            Debug.Assert(task is Task<T>);
            Debug.Assert(completionSource is CompletionSource<T?>);
            Unsafe.As<CompletionSource<T?>>(completionSource).SetResult(new ValueTask<T>(task: Unsafe.As<Task<T>>(task)).Result);
        }
    }
}
