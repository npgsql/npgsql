using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql.Internal;

public struct SizeContext
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
    /// Custom requirements from GetBufferRequirements
    Custom
}

public static class BufferingRequirementExtensions
{
    public static (Size ReadRequirement, Size WriteRequirement) ToBufferRequirements(this BufferingRequirement bufferingRequirement, PgConverter converter)
    {
        Size read, write;
        if (bufferingRequirement is BufferingRequirement.Custom)
            converter.GetBufferRequirements(DataFormat.Binary, out read, out write);
        else
            read = write = bufferingRequirement switch
            {
                BufferingRequirement.None => Size.Zero,
                BufferingRequirement.Value => Size.Unknown,
                _ => Size.Unknown
            };
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

    /// When <see ref="fixedSize"/> is true GetSize can be called with a default value for the type and the given format without throwing.
    public virtual bool CanConvert(DataFormat format, out BufferingRequirement bufferingRequirement, out bool fixedSize)
    {
        // This is a reasonable default but it won't always be correct, it's up to the individual buffering converters to override this.
        fixedSize = false;
        bufferingRequirement = BufferingRequirement.None;
        return format is DataFormat.Binary;
    }

    /// When <see cref="CanConvert"/> returns BufferingRequirement.Custom this method can be called to determine the buffer requirements.
    public virtual void GetBufferRequirements(DataFormat format, out Size readRequirement, out Size writeRequirement) => throw new NotImplementedException();

    internal abstract Type TypeToConvert { get; }

    internal bool IsDbNullValueAsObject([NotNullWhen(false)] object? value)
    {
        return DbNullPredicateKind switch
        {
            DbNullPredicate.Null => value is null,
            DbNullPredicate.None => false,
            DbNullPredicate.PolymorphicNull => value is null or DBNull,
            _ => Custom()
        };

        // We do the null check to keep the NotNullWhen(false) invariant.
        bool Custom() => IsDbNullAsObject(value) || (value is null ? throw new ArgumentNullException("Null value given for non-nullable type converter") : false);
    }

    private protected abstract bool IsDbNullAsObject(object? value);

    internal abstract Size GetSizeAsObject(SizeContext context, object value, ref object? writeState);

    internal object ReadAsObject(PgReader reader) => ReadAsObject(async: false, reader, CancellationToken.None);
    internal ValueTask<object> ReadAsObjectAsync(PgReader reader, CancellationToken cancellationToken = default) => ReadAsObject(async: true, reader, cancellationToken);

    // Shared sync/async abstract to reduce virtual method table size overhead and code size for each NpgsqlConverter<T> instantiation.
    private protected abstract ValueTask<object> ReadAsObject(bool async, PgReader reader, CancellationToken cancellationToken);

    internal void WriteAsObject(PgWriter writer, object value) => WriteAsObject(async: false, writer, value, CancellationToken.None);
    internal ValueTask WriteAsObjectAsync(PgWriter writer, object value, CancellationToken cancellationToken = default) => WriteAsObject(async: true, writer, value, cancellationToken);

    // Shared sync/async abstract to reduce virtual method table size overhead and code size for each NpgsqlConverter<T> instantiation.
    private protected abstract ValueTask WriteAsObject(bool async, PgWriter writer, object value, CancellationToken cancellationToken);

    static DbNullPredicate InferDbNullPredicate(Type type, bool isNullDefaultValue)
    {
        if (type == typeof(object) || type == typeof(DBNull))
            return DbNullPredicate.PolymorphicNull;

        if (isNullDefaultValue)
            return DbNullPredicate.Null;

        return DbNullPredicate.None;
    }

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
    private protected static void ThrowIORequired() => throw new InvalidOperationException("Fixed sizedness for format not respected, expected no IO to be required.");
}

public abstract class PgConverter<T> : PgConverter
{
    private protected PgConverter(bool customDbNullPredicate)
        : base(typeof(T), default(T) is null, customDbNullPredicate) { }

    protected virtual bool IsDbNull(T? value) => throw new NotImplementedException();

    // Object null semantics as follows, if T is a struct (so excluding nullable) report false for null values, don't throw on the cast.
    // As a result this creates symmetry with IsDbNull when we're dealing with a struct T, as it cannot be passed null at all.
    private protected override bool IsDbNullAsObject(object? value) => (default(T) is null || value is not null) && IsDbNull((T?)value);

    public bool IsDbNullValue([NotNullWhen(false)] T? value)
    {
        return DbNullPredicateKind switch
        {
            DbNullPredicate.Null => value is null,
            DbNullPredicate.None => false,
            DbNullPredicate.PolymorphicNull => value is null or DBNull,
            _ => Custom()
        };

        // We do the null check to keep the NotNullWhen(false) invariant.
        bool Custom() => IsDbNull(value) || (value is null ? throw new ArgumentNullException("Null value given for non-nullable type converter") : false);
    }

    public abstract T Read(PgReader reader);
    public abstract ValueTask<T> ReadAsync(PgReader reader, CancellationToken cancellationToken = default);

    public abstract Size GetSize(SizeContext context, [DisallowNull] T value, ref object? writeState);
    public abstract void Write(PgWriter writer, [DisallowNull] T value);
    public abstract ValueTask WriteAsync(PgWriter writer, [DisallowNull] T value, CancellationToken cancellationToken = default);

    internal sealed override Type TypeToConvert => typeof(T);
    internal sealed override Size GetSizeAsObject(SizeContext context, object value, ref object? writeState)
        => GetSize(context, (T)value, ref writeState);
}

// Using a function pointer here is safe against assembly unloading as the instance reference that the static pointer method lives on is passed along.
// As such the instance cannot be collected by the gc which means the entire assembly is prevented from unloading until we're done.
// The alternatives are:
// 1. Add a virtual method and make AwaitTask call into it (bloating the vtable of all derived types).
// 2. Using a delegate, meaning we add a static field + an alloc per T + metadata, slightly slower dispatch perf so overall strictly worse as well.
static class PgStreamingConverterHelpers
{
    // Split out from the generic class to amortize the huge size penalty per async state machine, which would otherwise be per instantiation.
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

    // Split out into a struct as unsafe and async don't mix, while we do want a nicely typed function pointer signature to prevent mistakes.
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

    private protected sealed override unsafe ValueTask<object> ReadAsObject(bool async, PgReader reader, CancellationToken cancellationToken = default)
    {
        if (!async)
            return new(Read(reader)!);

        var task = ReadAsync(reader, cancellationToken);
        if (task.IsCompletedSuccessfully)
            return new(task.Result!);

        return PgStreamingConverterHelpers.AwaitTask(task.AsTask(), new(this, &BoxResult));

        static object BoxResult(Task task)
        {
            Debug.Assert(task is Task<T>);
            return new ValueTask<object>(Unsafe.As<Task<T>>(task)).Result;
        }
    }

    private protected sealed override ValueTask WriteAsObject(bool async, PgWriter writer, object value, CancellationToken cancellationToken)
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
        if (reader.Remaining < reader.Current.Size.Value)
            ThrowIORequired();

        return ReadCore(reader);
    }

    public sealed override ValueTask<T> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
        => new(Read(reader));

    private protected sealed override ValueTask WriteAsObject(bool async, PgWriter writer, object value, CancellationToken cancellationToken)
    {
        Write(writer, (T)value);
        return new();
    }

    public sealed override void Write(PgWriter writer, T value)
    {
        // If Kind is SizeKind.Unknown we're doing a buffering write.
        if (writer.Current.Size.Kind is SizeKind.Exact && writer.Remaining < writer.Current.Size.Value)
            ThrowIORequired();

        WriteCore(writer, value);
    }

    public sealed override ValueTask WriteAsync(PgWriter writer, [DisallowNull] T value, CancellationToken cancellationToken = default)
    {
        Write(writer, value);
        return new();
    }

    private protected sealed override ValueTask<object> ReadAsObject(bool async, PgReader reader, CancellationToken cancellationToken)
        => new(Read(reader)!);

    public override bool CanConvert(DataFormat format, out BufferingRequirement bufferingRequirement, out bool fixedSize)
    {
        bufferingRequirement = BufferingRequirement.Value;
        fixedSize = false;
        return format is DataFormat.Binary;
    }
}

public abstract class PgComposingConverter<T> : PgConverter<T>
{
    protected PgConverter EffectiveConverter { get; }

    protected PgComposingConverter(PgConverter effectiveConverter)
        : base(effectiveConverter.DbNullPredicateKind is DbNullPredicate.Custom)
        => EffectiveConverter = effectiveConverter;

    public override bool CanConvert(DataFormat format, out BufferingRequirement bufferingRequirement, out bool fixedSize)
        => EffectiveConverter.CanConvert(format, out bufferingRequirement, out fixedSize);

    public override void GetBufferRequirements(DataFormat format, out Size readRequirement, out Size writeRequirement)
        => EffectiveConverter.GetBufferRequirements(format, out readRequirement, out writeRequirement);

    private protected sealed override ValueTask<object> ReadAsObject(bool async, PgReader reader, CancellationToken cancellationToken)
        => async
            ? EffectiveConverter.ReadAsObjectAsync(reader, cancellationToken)
            : new(EffectiveConverter.ReadAsObject(reader));

    private protected sealed override ValueTask WriteAsObject(bool async, PgWriter writer, object value, CancellationToken cancellationToken)
    {
        if (async)
            return EffectiveConverter.WriteAsObjectAsync(writer, value, cancellationToken);

        EffectiveConverter.WriteAsObject(writer, value);
        return new();
    }
}
