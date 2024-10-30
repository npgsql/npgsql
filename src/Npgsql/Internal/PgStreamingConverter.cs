using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql.Internal;

[Experimental(NpgsqlDiagnostics.ConvertersExperimental)]
public abstract class PgStreamingConverter<T>(bool customDbNullPredicate = false) : PgConverter<T>(customDbNullPredicate)
{
    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.None;
        return format is DataFormat.Binary;
    }

    // Workaround for trimming https://github.com/dotnet/runtime/issues/92850#issuecomment-1744521361
    internal Task<T>? ReadAsyncAsTask(PgReader reader, CancellationToken cancellationToken, out T result)
    {
        var task = ReadAsync(reader, cancellationToken);
        if (task.IsCompletedSuccessfully)
        {
            result = task.Result;
            return null;
        }
        result = default!;
        return task.AsTask();
    }

    internal sealed override unsafe ValueTask<object> ReadAsObject(
        bool async, PgReader reader, CancellationToken cancellationToken)
    {
        if (!async)
            return new(Read(reader)!);

        var task = ReadAsync(reader, cancellationToken);
        return task.IsCompletedSuccessfully
            ? new(task.Result!)
            : PgStreamingConverterHelpers.AwaitTask(task.AsTask(), new(this, &BoxResult));

        static object BoxResult(Task task)
        {
            // Justification: exact type Unsafe.As used to reduce generic duplication cost.
            Debug.Assert(task is Task<T>);
            // Using .Result on ValueTask is equivalent to GetAwaiter().GetResult(), this removes TaskAwaiter<T> rooting.
            return new ValueTask<T>(task: Unsafe.As<Task<T>>(task)).Result!;
        }
    }

    internal sealed override ValueTask WriteAsObject(bool async, PgWriter writer, object value, CancellationToken cancellationToken)
    {
        if (async)
            return WriteAsync(writer, (T)value, cancellationToken);

        Write(writer, (T)value);
        return new();
    }
}

// Using a function pointer here is safe against assembly unloading as the instance reference that the static pointer method lives on is
// passed along. As such the instance cannot be collected by the gc which means the entire assembly is prevented from unloading until we're
// done.
// The alternatives are:
// 1. Add a virtual method and make AwaitTask call into it (bloating the vtable of all derived types).
// 2. Using a delegate, meaning we add a static field + an alloc per T + metadata, slightly slower dispatch perf so overall strictly worse
// as well.
static class PgStreamingConverterHelpers
{
    // Split out from the generic class to amortize the huge size penalty per async state machine, which would otherwise be per
    // instantiation.
    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
    public static async ValueTask<object> AwaitTask(Task task, Continuation continuation)
    {
        await task.ConfigureAwait(false);
        var result = continuation.Invoke(task);
        // Guarantee the type stays loaded until the function pointer call is done.
        GC.KeepAlive(continuation.Handle);
        return result;
    }

    // Split out into a struct as unsafe and async don't mix, while we do want a nicely typed function pointer signature to prevent
    // mistakes.
    public readonly unsafe struct Continuation
    {
        public object Handle { get; }
        readonly delegate*<Task, object> _continuation;

        /// <param name="handle">A reference to the type that houses the static method <see ref="continuation"/> points to.</param>
        /// <param name="continuation">The continuation</param>
        public Continuation(object handle, delegate*<Task, object> continuation)
        {
            Handle = handle;
            _continuation = continuation;
        }

        public object Invoke(Task task) => _continuation(task);
    }
}
