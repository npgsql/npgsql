using System;
using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql.Internal;

[Experimental(NpgsqlDiagnostics.ConvertersExperimental)]
public abstract class PgConverter
{
    /// <summary>
    /// True when CLR null can reach this converter's API surface.
    /// Auto-derived from <see cref="TypeToConvert"/> (or from an internal wrapper's effective type if passed).
    /// Orthogonal to <see cref="HandleDbNull"/>: the two combine into <see cref="DbNullPredicateKind"/> as Custom (HandleDbNull true),
    /// Null (HandleDbNull false, TypeAcceptsNull true), or None (both false).
    /// </summary>
    internal bool TypeAcceptsNull { get; }
    internal DbNullPredicate DbNullPredicateKind
        => HandleDbNull ? DbNullPredicate.Custom
            : TypeAcceptsNull ? DbNullPredicate.Null
            : DbNullPredicate.None;
    public bool IsDbNullable => DbNullPredicateKind is not DbNullPredicate.None;

    private protected PgConverter(Type type, bool typeAcceptsNull)
    {
        TypeToConvert = type;
        TypeAcceptsNull = typeAcceptsNull;
    }

    /// <summary>
    /// True when the converter has a custom IsDbNullValue override that should be consulted to determine db-nullness.
    /// When false, db-nullness is decided purely based on whether the <see cref="TypeToConvert"/> accepts nulls naturally.
    /// </summary>
    protected internal bool HandleDbNull { get; init; }

    /// <summary>
    /// Whether this converter can handle the given format and with which buffer requirements.
    /// </summary>
    /// <param name="format">The data format.</param>
    /// <param name="bufferRequirements">Returns the buffer requirements.</param>
    /// <returns>Returns true if the given data format is supported.</returns>
    /// <remarks>The buffer requirements should not cover database NULL reads or writes, these are handled by the caller.</remarks>
    public abstract bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements);

    internal Type TypeToConvert { get; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    PgConverter<T> UnsafeAs<T>()
    {
        // Justification: avoid perf cost of casting to a known base class type per dispatch call.
        Debug.Assert(typeof(T) == TypeToConvert);
        Debug.Assert(this is PgConverter<T>);
        return Unsafe.As<PgConverter<T>>(this);
    }

    /// <summary>Reads a value from the reader as <typeparamref name="T"/>.</summary>
    /// <remarks>Dispatches to the typed converter when <typeparamref name="T"/> matches <see cref="TypeToConvert"/>; otherwise routes through the object-erased path.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public
#nullable disable // T may or may not be nullable depending on the converter's read behavior.
    T
#nullable restore
    Read<T>(PgReader reader)
        => typeof(T) != TypeToConvert
            ? (T)ReadAsObject(reader)!
            : UnsafeAs<T>().Read(reader);

    /// <summary>Asynchronously reads a value from the reader as <typeparamref name="T"/>.</summary>
    /// <remarks>Dispatches to the typed converter when <typeparamref name="T"/> matches <see cref="TypeToConvert"/>; otherwise routes through the object-erased path.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<
#nullable disable // T may or may not be nullable depending on the converter's read behavior.
    T
#nullable restore
    > ReadAsync<T>(PgReader reader, CancellationToken cancellationToken = default)
    {
        if (typeof(T) != TypeToConvert)
        {
            var task = ReadAsObjectAsync(reader, cancellationToken);
            return task.IsCompletedSuccessfully ? new((T)task.Result!) : ReadAndUnboxAsync(task);
        }

        return UnsafeAs<T>().ReadAsync(reader, cancellationToken);

        [MethodImpl(MethodImplOptions.NoInlining)]
        static async ValueTask<T> ReadAndUnboxAsync(ValueTask<object?> task)
            => (T)(await task.ConfigureAwait(false))!;
    }

    /// <summary>Writes a <typeparamref name="T"/> value to the writer.</summary>
    /// <remarks>Dispatches to the typed converter when <typeparamref name="T"/> matches <see cref="TypeToConvert"/>; otherwise routes through the object-erased path.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<T>(PgWriter writer, T value)
    {
        if (typeof(T) != TypeToConvert)
        {
            WriteAsObject(writer, value);
            return;
        }
        UnsafeAs<T>().Write(writer, value);
    }

    /// <summary>Asynchronously writes a <typeparamref name="T"/> value to the writer.</summary>
    /// <remarks>Dispatches to the typed converter when <typeparamref name="T"/> matches <see cref="TypeToConvert"/>; otherwise routes through the object-erased path.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask WriteAsync<T>(PgWriter writer, T value, CancellationToken cancellationToken = default)
        => typeof(T) != TypeToConvert
            ? WriteAsObjectAsync(writer, value, cancellationToken)
            : UnsafeAs<T>().WriteAsync(writer, value, cancellationToken);

    /// Checks whether <paramref name="value"/> is considered a database null by this converter.
    public bool IsDbNullAsObject(object? value, object? writeState)
    {
        if (value is null && !TypeAcceptsNull)
            ThrowInvalidNullValue();
        return DbNullPredicateKind switch
        {
            DbNullPredicate.Null => value is null,
            DbNullPredicate.None => false,
            DbNullPredicate.Custom => IsDbNullValueAsObject(value, writeState),
            _ => ThrowDbNullPredicateOutOfRange()
        };
    }

    private protected abstract bool IsDbNullValueAsObject(object? value, object? writeState);

    private protected abstract Size GetSizeAsObject(SizeContext context, object? value, ref object? writeState);

    /// Computes the serialized size for <paramref name="value"/>, producing any required <paramref name="writeState"/>.
    public Size BindAsObject(SizeContext context, object? value, ref object? writeState)
    {
        Debug.Assert(TypeAcceptsNull || value is not null);

        if (context.BufferRequirement is { Kind: SizeKind.Exact, Value: var byteCount })
            return byteCount;
        var size = GetSizeAsObject(context, value, ref writeState);

        switch (size.Kind)
        {
        case SizeKind.UpperBound:
            ThrowHelper.ThrowInvalidOperationException($"{nameof(SizeKind.UpperBound)} is not a valid return value for GetSize.");
            break;
        case SizeKind.Unknown:
            ThrowHelper.ThrowInvalidOperationException($"{nameof(SizeKind.Unknown)} is not a valid return value for GetSize.");
            break;
        }

        return size;
    }

    /// Reads a value from the reader.
    public object? ReadAsObject(PgReader reader)
        => ReadAsObject(async: false, reader, CancellationToken.None).Result;
    /// Asynchronously reads a value from the reader.
    public ValueTask<object?> ReadAsObjectAsync(PgReader reader, CancellationToken cancellationToken = default)
        => ReadAsObject(async: true, reader, cancellationToken);

    // Shared sync/async abstract to reduce virtual method table size overhead and code size for each NpgsqlConverter<T> instantiation.
    internal abstract ValueTask<object?> ReadAsObject(bool async, PgReader reader, CancellationToken cancellationToken);

    /// Writes <paramref name="value"/> to the writer.
    public void WriteAsObject(PgWriter writer, object? value)
        => WriteAsObject(async: false, writer, value, CancellationToken.None).GetAwaiter().GetResult();
    /// Asynchronously writes <paramref name="value"/> to the writer.
    public ValueTask WriteAsObjectAsync(PgWriter writer, object? value, CancellationToken cancellationToken = default)
        => WriteAsObject(async: true, writer, value, cancellationToken);

    // Shared sync/async abstract to reduce virtual method table size overhead and code size for each NpgsqlConverter<T> instantiation.
    internal abstract ValueTask WriteAsObject(bool async, PgWriter writer, object? value, CancellationToken cancellationToken);

    internal enum DbNullPredicate : byte
    {
        /// Never DbNull (struct types)
        None,
        /// DbNull when *user code*
        Custom,
        /// DbNull when value is null
        Null
    }

    [DoesNotReturn]
    private protected void ThrowIORequired(Size bufferRequirement)
        => throw new InvalidOperationException($"Buffer requirement '{bufferRequirement}' not respected for converter '{GetType().FullName}', expected no IO to be required.");

    private protected static bool ThrowInvalidNullValue()
        => throw new ArgumentNullException("value", "Null value given for non-nullable type converter");

    private protected bool ThrowDbNullPredicateOutOfRange()
        => throw new UnreachableException($"Unknown case {DbNullPredicateKind.ToString()}");
}

public abstract class PgConverter<T> : PgConverter
{
    private protected PgConverter() : base(typeof(T), default(T) is null) { }

    private protected PgConverter(Type effectiveType)
        : base(typeof(T), !effectiveType.IsValueType || Nullable.GetUnderlyingType(effectiveType) is not null) { }

#pragma warning disable CS0618 // Obsolete - delegates to ref overload for binary compat with existing overrides
    protected virtual bool IsDbNullValue(T? value, object? writeState)
    {
        // The obsolete ref overload is kept around for binary compatibility on the signature, but
        // mutating writeState during a null probe is no longer a supported behaviour. Detect the
        // mutation via a local captured before the forward and throw — a violating override is a
        // bug in the derived converter, not something to defend against here.
        var originalWriteState = writeState;
        var isDbNull = IsDbNullValue(value, ref writeState);
        if (!ReferenceEquals(writeState, originalWriteState))
            ThrowHelper.ThrowInvalidOperationException(
                $"{GetType().FullName} mutated writeState from its IsDbNullValue override. Override the overload without ref and produce write state only in GetSize.");
        return isDbNull;
    }
#pragma warning restore CS0618

    [Obsolete("Use the overload without ref.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual bool IsDbNullValue(T? value, ref object? writeState) => throw new NotSupportedException();

    private protected override bool IsDbNullValueAsObject(object? value, object? writeState)
        => IsDbNullValue((T?)value, writeState);

    /// Checks whether <paramref name="value"/> is considered a database null by this converter.
    public bool IsDbNull(T? value, object? writeState)
    {
        Debug.Assert(value is not null || TypeAcceptsNull, "TypeAcceptsNull issue, null reached the typed IsDbNull on a converter whose T does not accept null.");
        return DbNullPredicateKind switch
        {
            DbNullPredicate.Null => value is null,
            DbNullPredicate.None => false,
            DbNullPredicate.Custom => IsDbNullValue(value, writeState),
            _ => ThrowDbNullPredicateOutOfRange()
        };
    }

    [Obsolete("Use the overload without ref.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsDbNull(T? value, ref object? writeState)
        => IsDbNull(value, writeState);

    /// Reads a <typeparamref name="T"/> value from the reader.
    public abstract
#nullable disable // T may or may not be nullable depending on the derived converter's read behavior.
    T
#nullable restore
    Read(PgReader reader);

    public Size Bind(SizeContext context,
#nullable disable // T may or may not be nullable depending on the derived converter's IsDbNullValue override.
        T value,
#nullable restore
        ref object? writeState)
    {
        Debug.Assert(TypeAcceptsNull || value is not null);

        if (context.BufferRequirement is { Kind: SizeKind.Exact, Value: var byteCount })
            return byteCount;
        var size = GetSize(context, value, ref writeState);

        switch (size.Kind)
        {
        case SizeKind.UpperBound:
            ThrowHelper.ThrowInvalidOperationException($"{nameof(SizeKind.UpperBound)} is not a valid return value for GetSize.");
            break;
        case SizeKind.Unknown:
            ThrowHelper.ThrowInvalidOperationException($"{nameof(SizeKind.Unknown)} is not a valid return value for GetSize.");
            break;
        }

        return size;
    }

    /// Asynchronously reads a <typeparamref name="T"/> value from the reader.
    public abstract ValueTask<
#nullable disable // T may or may not be nullable depending on the derived converter's read behavior.
    T
#nullable restore
    > ReadAsync(PgReader reader, CancellationToken cancellationToken = default);

    /// Computes the serialized size for <paramref name="value"/>, producing any required <paramref name="writeState"/>.
    protected abstract Size GetSize(SizeContext context,
#nullable disable // T may or may not be nullable depending on the derived converter's IsDbNullValue override.
        T value,
#nullable restore
        ref object? writeState);

    /// Writes a <typeparamref name="T"/> value to the writer.
    public abstract void Write(PgWriter writer,
#nullable disable // T may or may not be nullable depending on the derived converter's IsDbNullValue override.
        T value
#nullable restore
        );

    /// Asynchronously writes a <typeparamref name="T"/> value to the writer.
    public abstract ValueTask WriteAsync(PgWriter writer,
#nullable disable // T may or may not be nullable depending on the derived converter's IsDbNullValue override.
        T value,
#nullable restore
        CancellationToken cancellationToken = default);

    private protected sealed override Size GetSizeAsObject(SizeContext context, object? value, ref object? writeState)
        => GetSize(context, (T)value!, ref writeState);
}

[Experimental(NpgsqlDiagnostics.ConvertersExperimental)]
static class PgConverterExtensions
{
    /// Checks whether <paramref name="value"/> is considered a database null under the given <paramref name="handling"/> policy.
    public static bool IsDbNullAsNestedObject(this PgConverter converter, object? value, object? writeState, NestedObjectDbNullHandling handling)
    {
        switch (handling)
        {
        case NestedObjectDbNullHandling.ExtendedThrowOnNull:
            if (value is null)
                ThrowHelper.ThrowArgumentNullException("Object-typed value cannot be null, a db null value must be used instead.", nameof(value));
            goto case NestedObjectDbNullHandling.Extended;
        case NestedObjectDbNullHandling.Extended:
            if (value is DBNull)
                return true;
            goto case NestedObjectDbNullHandling.Default;
        case NestedObjectDbNullHandling.Default:
            return value is null || converter.IsDbNullAsObject(value, writeState);
        default:
            ThrowHelper.ThrowUnreachableException();
            return default;
        }
    }
}

[Experimental(NpgsqlDiagnostics.ConvertersExperimental)]
[method: SetsRequiredMembers]
public readonly struct SizeContext(DataFormat format, Size bufferRequirement)
{
    public required Size BufferRequirement { get; init; } = bufferRequirement;
    public DataFormat Format { get; } = format;

    public NestedObjectDbNullHandling NestedObjectDbNullHandling { get; init; }
}

/// <summary>
/// How null-shaped values are pre-filtered when a container's element or field slot is erased to <see cref="object"/>.
/// CLR semantics are the floor (<see cref="Default"/>), extended modes layer database null sentinel recognition on top.
/// Strongly-typed slots resolve nulls through the nested converter directly and don't consult this knob.
/// </summary>
/// <remarks>
/// Parameter-shaped containers (e.g. an <c>object[]</c> parameter) use <see cref="Extended"/> because the
/// parameter layer treats database null sentinels as a first-class null expression alongside CLR null.
/// Typed composites generally use <see cref="Default"/>. These create a new serialization scope where database null sentinels are not recognized.
/// </remarks>
[Experimental(NpgsqlDiagnostics.ConvertersExperimental)]
public enum NestedObjectDbNullHandling
{
    /// <summary>CLR null becomes a database null. Database null sentinels are passed through to the nested converter.</summary>
    Default = 0,
    /// <summary>CLR null and database null sentinels both become a database null.</summary>
    Extended,
    /// <summary>CLR null throws. Database null sentinels become a database null.</summary>
    ExtendedThrowOnNull
}

class MultiWriteState : IDisposable
{
    public ArrayPool<(Size Size, object? WriteState)>? ArrayPool { get; set; }
    public ArraySegment<(Size Size, object? WriteState)> Data { get; set; }
    public bool AnyWriteState { get; set; }

    public void Dispose()
    {
        if (Data.Array is not { } array)
            return;

        if (AnyWriteState)
        {
            for (var i = Data.Offset; i < Data.Offset + Data.Count; i++)
                if (array[i].WriteState is IDisposable disposable)
                    disposable.Dispose();

            Array.Clear(Data.Array, Data.Offset, Data.Count);
        }

        ArrayPool?.Return(Data.Array);
    }
}
