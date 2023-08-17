using System;
using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql.Internal;

interface IResumableRead
{
    bool Supported { get; }
}

public readonly struct SizeContext
{
    [SetsRequiredMembers]
    public SizeContext(DataFormat format, Size bufferRequirement)
    {
        Format = format;
        BufferRequirement = bufferRequirement;
    }

    public DataFormat Format { get; }
    public required Size BufferRequirement { get; init; }
}

public readonly struct BufferRequirements : IEquatable<BufferRequirements>
{
    readonly Size _readRequirement;
    readonly Size _writeRequirement;

    BufferRequirements(Size readRequirement, Size writeRequirement)
    {
        _readRequirement = readRequirement;
        _writeRequirement = writeRequirement;
    }

    public Size Read => _readRequirement.IsDefault ? ThrowDefaultException() : _readRequirement;
    public Size Write => _writeRequirement.IsDefault ? ThrowDefaultException() : _writeRequirement;

    public bool IsFixedSize => Write is { Kind: SizeKind.Exact, Value : > 0 } && _readRequirement == _writeRequirement;

    /// Streaming
    public static BufferRequirements None => new(Size.Zero, Size.Zero);
    /// Entire value should be buffered
    public static BufferRequirements Value => new(Size.Unknown, Size.Unknown);
    /// Fixed size value should be buffered
    public static BufferRequirements CreateFixedSize(int byteCount) => new(byteCount, byteCount);
    /// Custom requirements
    public static BufferRequirements Create(Size requirement) => new(requirement, requirement);
    public static BufferRequirements Create(Size readRequirement, Size writeRequirement) => new(readRequirement, writeRequirement);

    public BufferRequirements Combine(BufferRequirements other)
    {
        return new BufferRequirements(
            CombineRequirements(_readRequirement, other._readRequirement),
            CombineRequirements(_writeRequirement, other._writeRequirement)
        );

        // The oddity, we shouldn't add sizes to zero, which is supposed to mean streaming.
        static Size CombineRequirements(Size left, Size right)
            => left == Size.Zero || right == Size.Zero ? Size.Zero : left.Combine(right);
    }

    public BufferRequirements Combine(int byteCount)
        => Combine(CreateFixedSize(byteCount));

    static Size ThrowDefaultException() => throw new InvalidOperationException($"This operation cannot be performed on a default instance of {nameof(BufferRequirements)}.");

    public bool Equals(BufferRequirements other) => _readRequirement.Equals(other._readRequirement) && _writeRequirement.Equals(other._writeRequirement);
    public override bool Equals(object? obj) => obj is BufferRequirements other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(_readRequirement, _writeRequirement);
    public static bool operator ==(BufferRequirements left, BufferRequirements right) => left.Equals(right);
    public static bool operator !=(BufferRequirements left, BufferRequirements right) => !left.Equals(right);
}

public abstract class PgConverter
{
    internal DbNullPredicate DbNullPredicateKind { get; }
    internal bool IsNullDefaultValue { get; }

    private protected PgConverter(Type type, bool isNullDefaultValue, bool customDbNullPredicate = false)
    {
        IsNullDefaultValue = isNullDefaultValue;
        DbNullPredicateKind = customDbNullPredicate ? DbNullPredicate.Custom : InferDbNullPredicate(type, isNullDefaultValue);
    }

    public abstract bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements);

    internal abstract Type TypeToConvert { get; }

    internal bool IsDbNullAsObject([NotNullWhen(false)] object? value)
        => DbNullPredicateKind switch
        {
            DbNullPredicate.Null => value is null,
            DbNullPredicate.None => false,
            DbNullPredicate.PolymorphicNull => value is null or DBNull,
            // We do the null check to keep the NotNullWhen(false) invariant.
            _ => IsDbNullValueAsObject(value) || (value is null && ThrowInvalidNullValue())
        };

    private protected abstract bool IsDbNullValueAsObject(object? value);

    internal abstract Size GetSizeAsObject(SizeContext context, object value, ref object? writeState);

    internal object ReadAsObject(PgReader reader)
        => ReadAsObject(async: false, reader, CancellationToken.None).GetAwaiter().GetResult();
    internal ValueTask<object> ReadAsObjectAsync(PgReader reader, CancellationToken cancellationToken = default)
        => ReadAsObject(async: true, reader, cancellationToken);

    // Shared sync/async abstract to reduce virtual method table size overhead and code size for each NpgsqlConverter<T> instantiation.
    internal abstract ValueTask<object> ReadAsObject(bool async, PgReader reader, CancellationToken cancellationToken);

    internal void WriteAsObject(PgWriter writer, object value)
        => WriteAsObject(async: false, writer, value, CancellationToken.None).GetAwaiter().GetResult();
    internal ValueTask WriteAsObjectAsync(PgWriter writer, object value, CancellationToken cancellationToken = default)
        => WriteAsObject(async: true, writer, value, cancellationToken);

    // Shared sync/async abstract to reduce virtual method table size overhead and code size for each NpgsqlConverter<T> instantiation.
    internal abstract ValueTask WriteAsObject(bool async, PgWriter writer, object value, CancellationToken cancellationToken);

    private protected static DbNullPredicate InferDbNullPredicate(Type type, bool isNullDefaultValue)
        => type == typeof(object) || type == typeof(DBNull)
            ? DbNullPredicate.PolymorphicNull
            : isNullDefaultValue
                ? DbNullPredicate.Null
                : DbNullPredicate.None;

    internal enum DbNullPredicate : byte
    {
        /// Never DbNull (struct types)
        None,
        /// DbNull when *user code*
        Custom,
        /// DbNull when value is null
        Null,
        /// DbNull when value is null or DBNull
        PolymorphicNull
    }

    [DoesNotReturn]
    private protected static void ThrowIORequired()
        => throw new InvalidOperationException("Buffer requirements for format not respected, expected no IO to be required.");

    private protected static bool ThrowInvalidNullValue()
        => throw new ArgumentNullException("value", "Null value given for non-nullable type converter");

    protected bool CanConvertBufferedDefault(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.Value;
        return format is DataFormat.Binary;
    }
}

public abstract class PgConverter<T> : PgConverter
{
    private protected PgConverter(bool customDbNullPredicate)
        : base(typeof(T), default(T) is null, customDbNullPredicate) { }

    protected virtual bool IsDbNullValue(T? value) => throw new NotImplementedException();

    // Object null semantics as follows, if T is a struct (so excluding nullable) report false for null values, don't throw on the cast.
    // As a result this creates symmetry with IsDbNull when we're dealing with a struct T, as it cannot be passed null at all.
    private protected override bool IsDbNullValueAsObject(object? value)
        => (default(T) is null || value is not null) && IsDbNullValue(DownCast(value));

    public bool IsDbNull([NotNullWhen(false)] T? value)
        => DbNullPredicateKind switch
        {
            DbNullPredicate.Null => value is null,
            DbNullPredicate.None => false,
            DbNullPredicate.PolymorphicNull => value is null or DBNull,
            // We do the null check to keep the NotNullWhen(false) invariant.
            _ => IsDbNullValue(value) || (value is null && ThrowInvalidNullValue())
        };

    public abstract T Read(PgReader reader);
    public abstract ValueTask<T> ReadAsync(PgReader reader, CancellationToken cancellationToken = default);

    public abstract Size GetSize(SizeContext context, T value, ref object? writeState);
    public abstract void Write(PgWriter writer, [DisallowNull] T value);
    public abstract ValueTask WriteAsync(PgWriter writer, [DisallowNull] T value, CancellationToken cancellationToken = default);

    internal sealed override Type TypeToConvert => typeof(T);

    internal sealed override Size GetSizeAsObject(SizeContext context, object value, ref object? writeState)
        => GetSize(context, DownCast(value), ref writeState);

    [MethodImpl(MethodImplOptions.NoInlining)]
    [return: NotNullIfNotNull(nameof(value))]
    static T? DownCast(object? value) => (T?)value;
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
#if NET6_0_OR_GREATER
    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
#endif
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

public abstract class PgStreamingConverter<T> : PgConverter<T>
{
    protected PgStreamingConverter(bool customDbNullPredicate = false) : base(customDbNullPredicate) { }

    public override bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        bufferRequirements = BufferRequirements.None;
        return format is DataFormat.Binary;
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
            Debug.Assert(task is Task<T>);
            return new ValueTask<object>(Unsafe.As<Task<T>>(task)).Result;
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

public abstract class PgBufferedConverter<T> : PgConverter<T>
{
    protected PgBufferedConverter(bool customDbNullPredicate = false) : base(customDbNullPredicate) { }

    protected abstract T ReadCore(PgReader reader);
    protected abstract void WriteCore(PgWriter writer, T value);

    public override Size GetSize(SizeContext context, T value, ref object? writeState) => throw new NotImplementedException();

    public sealed override T Read(PgReader reader)
    {
        if (reader.ShouldBuffer(reader.CurrentBufferRequirement))
            ThrowIORequired();

        return ReadCore(reader);
    }

    public sealed override ValueTask<T> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
        => new(Read(reader));

    internal sealed override ValueTask WriteAsObject(bool async, PgWriter writer, object value, CancellationToken cancellationToken)
    {
        Write(writer, (T)value);
        return new();
    }

    public sealed override void Write(PgWriter writer, T value)
    {
        // If Kind is SizeKind.Unknown we're doing a buffering write.
        if (writer.CurrentBufferRequirement is { Kind: not SizeKind.Unknown } size && writer.ShouldFlush(size))
            ThrowIORequired();

        WriteCore(writer, value);
    }

    public sealed override ValueTask WriteAsync(PgWriter writer, [DisallowNull] T value, CancellationToken cancellationToken = default)
    {
        Write(writer, value);
        return new();
    }

    internal sealed override ValueTask<object> ReadAsObject(bool async, PgReader reader, CancellationToken cancellationToken)
        => new(Read(reader)!);
}

static class ConverterExtensions
{
    public static Size? GetSizeOrDbNull<T>(this PgConverter<T> converter, DataFormat format, Size writeRequirement, T? value, ref object? writeState)
    {
        if (converter.IsDbNull(value))
            return null;

        if (writeRequirement is { Kind: SizeKind.Exact, Value: > 0 and var byteCount })
            return byteCount;
        var size = converter.GetSize(new(format, writeRequirement), value, ref writeState);
        if (size.Kind is SizeKind.UpperBound)
            throw new InvalidOperationException("SizeKind.UpperBound is not a valid return value for GetSize.");
        return size;
    }

    public static Size? GetSizeOrDbNullAsObject(this PgConverter converter, DataFormat format, Size writeRequirement, object? value, ref object? writeState)
    {
        if (converter.IsDbNullAsObject(value))
            return null;

        if (writeRequirement is { Kind: SizeKind.Exact, Value: > 0 and var byteCount })
            return byteCount;
        var size = converter.GetSizeAsObject(new(format, writeRequirement), value, ref writeState);
        if (size.Kind is SizeKind.UpperBound)
            throw new InvalidOperationException("SizeKind.UpperBound is not a valid return value for GetSize.");
        return size;
    }
}

class MultiWriteState
{
    public required ArrayPool<(Size Size, object? WriteState)>? ArrayPool { get; init; }
    public required ArraySegment<(Size Size, object? WriteState)> Data { get; init; }
    public required bool AnyWriteState { get; init; }

    public void Dispose()
    {
        if (Data.Array is not { } array)
            return;

        if (AnyWriteState)
            for (var i = Data.Offset; i < array.Length; i++)
            {
                ref var state = ref array[i].WriteState;
                if (array[i].WriteState is IDisposable disposable)
                    disposable.Dispose();
                state = null;
            }

        ArrayPool?.Return(Data.Array!);
    }
}
