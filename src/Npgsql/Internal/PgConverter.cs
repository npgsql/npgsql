using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql.Internal;

public readonly struct SizeContext
{
    public SizeContext(DataFormat format) => Format = format;
    public DataFormat Format { get; }
}

public enum BufferingRequirement : byte
{
    /// Streaming
    None,
    /// Entire value should be buffered
    Value,
    /// Fixed size value should be buffered
    FixedSize,
    /// Custom requirements from GetBufferRequirements
    Custom
}

public static class BufferingRequirementExtensions
{
    public static (Size ReadRequirement, Size WriteRequirement) ToBufferRequirements(this BufferingRequirement bufferingRequirement, DataFormat format, PgConverter converter)
    {
        Size read, write;
        switch (bufferingRequirement)
        {
        case BufferingRequirement.Custom:
            converter.GetBufferRequirements(format, out read, out write);
            break;
        case BufferingRequirement.FixedSize:
            object? state = null;
            read = write = converter.GetSizeAsObject(new(format), null!, ref state);
            break;
        default:
            read = write = bufferingRequirement switch
            {
                BufferingRequirement.None => Size.Zero,
                BufferingRequirement.Value => Size.Unknown,
                _ => Size.Unknown
            };
            break;
        }

        return (read, write);
    }
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

    public virtual bool CanConvert(DataFormat format, out BufferingRequirement bufferingRequirement)
    {
        bufferingRequirement = BufferingRequirement.None;
        return format is DataFormat.Binary;
    }

    /// When <see cref="CanConvert"/> returns BufferingRequirement.Custom this method can be called to determine the buffer requirements.
    public virtual void GetBufferRequirements(DataFormat format, out Size readRequirement, out Size writeRequirement)
        => throw new NotImplementedException();

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
        => throw new InvalidOperationException("Fixed sizedness for format not respected, expected no IO to be required.");

    private protected static bool ThrowInvalidNullValue()
        => throw new ArgumentNullException("Null value given for non-nullable type converter");
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

    public abstract Size GetSize(SizeContext context, [DisallowNull]T value, ref object? writeState);
    public abstract void Write(PgWriter writer, [DisallowNull] T value);
    public abstract ValueTask WriteAsync(PgWriter writer, [DisallowNull] T value, CancellationToken cancellationToken = default);

    internal sealed override Type TypeToConvert => typeof(T);

    // Make an allowance here for fixed size queries which won't know the default value of T.
    internal sealed override Size GetSizeAsObject(SizeContext context, object? value, ref object? writeState)
        => GetSize(context, value is null ? default! : DownCast(value), ref writeState);

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
#if !NETSTANDARD
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

    internal sealed override unsafe ValueTask<object> ReadAsObject(
        bool async, PgReader reader, CancellationToken cancellationToken = default)
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

    public sealed override T Read(PgReader reader)
    {
        if (reader.Remaining < reader.CurrentSize)
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
        if (writer.Current.Size is not { Kind: not SizeKind.Unknown } size || writer.ShouldFlush(size))
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

    public override bool CanConvert(DataFormat format, out BufferingRequirement bufferingRequirement)
    {
        bufferingRequirement = BufferingRequirement.Value;
        return format is DataFormat.Binary;
    }
}

public abstract class PgComposingConverter<T> : PgConverter<T>
{
    protected PgConverter EffectiveConverter { get; }

    protected PgComposingConverter(PgConverter effectiveConverter)
        : base(effectiveConverter.DbNullPredicateKind is DbNullPredicate.Custom)
        => EffectiveConverter = effectiveConverter;

    public override bool CanConvert(DataFormat format, out BufferingRequirement bufferingRequirement)
        => EffectiveConverter.CanConvert(format, out bufferingRequirement);

    public override void GetBufferRequirements(DataFormat format, out Size readRequirement, out Size writeRequirement)
        => EffectiveConverter.GetBufferRequirements(format, out readRequirement, out writeRequirement);

    internal sealed override ValueTask<object> ReadAsObject(bool async, PgReader reader, CancellationToken cancellationToken)
        => async
            ? EffectiveConverter.ReadAsObjectAsync(reader, cancellationToken)
            : new(EffectiveConverter.ReadAsObject(reader));

    internal sealed override ValueTask WriteAsObject(bool async, PgWriter writer, object value, CancellationToken cancellationToken)
    {
        if (async)
            return EffectiveConverter.WriteAsObjectAsync(writer, value, cancellationToken);

        EffectiveConverter.WriteAsObject(writer, value);
        return new();
    }
}

static class ConverterExtensions
{
    public static Size? GetSizeOrDbNull<T>(this PgConverter<T> converter, Size writeRequirement, SizeContext context, T? value, ref object? writeState)
    {
        if (converter.IsDbNull(value))
            return null;

        if (writeRequirement is { Kind: SizeKind.Exact, Value: var byteCount })
            return byteCount;
        var size = converter.GetSize(context, value, ref writeState);
        Debug.Assert(size.Kind is not SizeKind.UpperBound);
        return size;
    }
}
