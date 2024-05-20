using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql.Internal.Converters;

static class AsyncHelpers
{
    public static void OnCompletedWithSource(Task task, CompletionSource source, CompletionSourceContinuation continuation)
    {
        _ = Core(task, source, continuation);

        // Have our state machine be pooled, but don't return the task, source.Task should be used instead.
        [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
        async ValueTask Core(Task task, CompletionSource source, CompletionSourceContinuation continuation)
        {
            try
            {
                await task.ConfigureAwait(false);
                continuation.Invoke(task, source);
            }
            catch (Exception ex)
            {
                source.SetException(ex);
            }
            // Guarantee the type stays loaded until the function pointer call is done.
            continuation.KeepAlive();
        }
    }

    public abstract class CompletionSource
    {
        public abstract void SetException(Exception exception);
    }

    public sealed class CompletionSource<T> : CompletionSource
    {
        AsyncValueTaskMethodBuilder<T> _amb = AsyncValueTaskMethodBuilder<T>.Create();

        public ValueTask<T> Task => _amb.Task;

        public void SetResult(T value)
            => _amb.SetResult(value);

        public override void SetException(Exception exception)
            => _amb.SetException(exception);
    }

    public sealed class PoolingCompletionSource<T> : CompletionSource
    {
        PoolingAsyncValueTaskMethodBuilder<T> _amb = PoolingAsyncValueTaskMethodBuilder<T>.Create();

        public ValueTask<T> Task => _amb.Task;

        public void SetResult(T value)
            => _amb.SetResult(value);

        public override void SetException(Exception exception)
            => _amb.SetException(exception);
    }

    // Using a function pointer here is safe against assembly unloading as the instance reference that the static pointer method lives on is passed along.
    // As such the instance cannot be collected by the gc which means the entire assembly is prevented from unloading until we're done.
    // Split out into a struct as unsafe and async don't mix, while we do want a nicely typed function pointer signature to prevent mistakes.
    public readonly unsafe struct CompletionSourceContinuation
    {
        readonly object _handle;
        readonly delegate*<Task, CompletionSource, void> _continuation;

        /// <param name="handle">A reference to the type that houses the static method <see ref="continuation"/> points to.</param>
        /// <param name="continuation">The continuation</param>
        public CompletionSourceContinuation(object handle, delegate*<Task, CompletionSource, void> continuation)
        {
            _handle = handle;
            _continuation = continuation;
        }

        public void KeepAlive() => GC.KeepAlive(_handle);

        public void Invoke(Task task, CompletionSource tcs) => _continuation(task, tcs);
    }

    public static unsafe ValueTask<T?> ReadAsyncAsNullable<T>(this PgConverter<T?> instance, PgConverter<T> effectiveConverter, PgReader reader, CancellationToken cancellationToken)
        where T : struct
    {
        // Cheap if we have all the data.
        var task = effectiveConverter.ReadAsync(reader, cancellationToken);
        if (task.IsCompletedSuccessfully)
            return new(new T?(task.Result));

        // Otherwise we do one additional allocation, this allow us to share state machine codegen for all Ts.
        var source = new PoolingCompletionSource<T?>();
        OnCompletedWithSource(task.AsTask(), source, new(instance, &UnboxAndComplete));
        return source.Task;

        static void UnboxAndComplete(Task task, CompletionSource completionSource)
        {
            // Justification: exact type Unsafe.As used to reduce generic duplication cost.
            Debug.Assert(task is Task<T>);
            Debug.Assert(completionSource is PoolingCompletionSource<T?>);
            Unsafe.As<PoolingCompletionSource<T?>>(completionSource).SetResult(new T?(new ValueTask<T>(Unsafe.As<Task<T>>(task)).Result));
        }
    }

    public static unsafe ValueTask<T> ReadAsObjectAsyncAsT<T>(this PgConverter<T> instance, PgConverter effectiveConverter, PgReader reader, CancellationToken cancellationToken)
    {
        // Cheap if we have all the data.
        var task = effectiveConverter.ReadAsObjectAsync(reader, cancellationToken);
        if (task.IsCompletedSuccessfully)
            return new((T)task.Result);

        // Otherwise we do one additional allocation, this allow us to share state machine codegen for all Ts.
        var source = new PoolingCompletionSource<T>();
        OnCompletedWithSource(task.AsTask(), source, new(instance, &UnboxAndComplete));
        return source.Task;

        static void UnboxAndComplete(Task task, CompletionSource completionSource)
        {
            // Justification: exact type Unsafe.As used to reduce generic duplication cost.
            Debug.Assert(task is Task<object>);
            Debug.Assert(completionSource is PoolingCompletionSource<T>);
            Unsafe.As<PoolingCompletionSource<T>>(completionSource).SetResult((T)new ValueTask<object>(Unsafe.As<Task<object>>(task)).Result);
        }
    }
}
