using System;
using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql.Internal;

[Experimental(NpgsqlDiagnostics.ConvertersExperimental)]
public abstract class PgConverter
{
    internal DbNullPredicate DbNullPredicateKind { get; }
    public bool IsDbNullable => DbNullPredicateKind is not DbNullPredicate.None;

    private protected PgConverter(bool isNullDefaultValue, bool customDbNullPredicate = false)
        => DbNullPredicateKind = customDbNullPredicate ? DbNullPredicate.Custom : isNullDefaultValue ? DbNullPredicate.Null : DbNullPredicate.None;

    /// <summary>
    /// Whether this converter can handle the given format and with which buffer requirements.
    /// </summary>
    /// <param name="format">The data format.</param>
    /// <param name="bufferRequirements">Returns the buffer requirements.</param>
    /// <returns>Returns true if the given data format is supported.</returns>
    /// <remarks>The buffer requirements should not cover database NULL reads or writes, these are handled by the caller.</remarks>
    public abstract bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements);

    internal abstract Type TypeToConvert { get; }

    internal bool IsDbNullAsObject([NotNullWhen(false)] object? value, object? writeState)
        => DbNullPredicateKind switch
        {
            DbNullPredicate.Null => value is null,
            DbNullPredicate.None => value is null && ThrowInvalidNullValue(),
            // We do the null check to keep the NotNullWhen(false) invariant.
            DbNullPredicate.Custom => IsDbNullValueAsObject(value, writeState) || (value is null && ThrowInvalidNullValue()),
            _ => ThrowDbNullPredicateOutOfRange()
        };

    [Obsolete("Use the overload without ref.")]
    internal bool IsDbNullAsObject([NotNullWhen(false)] object? value, ref object? writeState)
        => IsDbNullAsObject(value, writeState);

    private protected abstract bool IsDbNullValueAsObject(object? value, object? writeState);

    [Obsolete("Use the overload without ref.")]
    private protected bool IsDbNullValueAsObject(object? value, ref object? writeState)
        => IsDbNullValueAsObject(value, writeState);

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
    private protected PgConverter(bool customDbNullPredicate)
        : base(default(T) is null, customDbNullPredicate) { }

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

    // Object null semantics as follows, if T is a struct (so excluding nullable) report false for null values, don't throw on the cast.
    // As a result this creates symmetry with IsDbNull when we're dealing with a struct T, as it cannot be passed null at all.
    private protected override bool IsDbNullValueAsObject(object? value, object? writeState)
        => (default(T) is null || value is not null) && IsDbNullValue((T?)value, writeState);

    public bool IsDbNull([NotNullWhen(false)] T? value, object? writeState)
        => DbNullPredicateKind switch
        {
            DbNullPredicate.Null => value is null,
            DbNullPredicate.None => false,
            // We do the null check to keep the NotNullWhen(false) invariant.
            DbNullPredicate.Custom => IsDbNullValue(value, writeState) || (value is null && ThrowInvalidNullValue()),
            _ => ThrowDbNullPredicateOutOfRange()
        };

    [Obsolete("Use the overload without ref.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsDbNull([NotNullWhen(false)] T? value, ref object? writeState)
        => IsDbNull(value, writeState);

    public abstract T Read(PgReader reader);
    public abstract ValueTask<T> ReadAsync(PgReader reader, CancellationToken cancellationToken = default);

    public abstract Size GetSize(SizeContext context, [DisallowNull]T value, ref object? writeState);
    public abstract void Write(PgWriter writer, [DisallowNull] T value);
    public abstract ValueTask WriteAsync(PgWriter writer, [DisallowNull] T value, CancellationToken cancellationToken = default);

    internal sealed override Type TypeToConvert => typeof(T);

    internal sealed override Size GetSizeAsObject(SizeContext context, object value, ref object? writeState)
        => GetSize(context, (T)value, ref writeState);
}

static class PgConverterExtensions
{
    public static Size? GetSizeOrDbNull<T>(this PgConverter<T> converter, DataFormat format, Size writeRequirement, T? value, ref object? writeState)
    {
        if (converter.IsDbNull(value, writeState))
            return null;

        if (writeRequirement is { Kind: SizeKind.Exact, Value: var byteCount })
            return byteCount;
        var size = converter.GetSize(new(format, writeRequirement), value, ref writeState);

        switch (size.Kind)
        {
        case SizeKind.UpperBound:
            ThrowHelper.ThrowInvalidOperationException($"{nameof(SizeKind.UpperBound)} is not a valid return value for GetSize.");
            break;
        case SizeKind.Unknown:
            // Not valid yet.
            ThrowHelper.ThrowInvalidOperationException($"{nameof(SizeKind.Unknown)} is not a valid return value for GetSize.");
            break;
        }

        return size;
    }

    public static Size? GetSizeOrDbNullAsObject(this PgConverter converter, DataFormat format, Size writeRequirement, object? value, ref object? writeState, NestedObjectDbNullHandling nestedObjectDbNullHandling = NestedObjectDbNullHandling.Default)
    {
        if (converter.IsDbNullAsObject(value, writeState))
            return null;

        if (writeRequirement is { Kind: SizeKind.Exact, Value: var byteCount })
            return byteCount;
        var size = converter.GetSizeAsObject(new(format, writeRequirement) { NestedObjectDbNullHandling = nestedObjectDbNullHandling }, value, ref writeState);

        switch (size.Kind)
        {
        case SizeKind.UpperBound:
            ThrowHelper.ThrowInvalidOperationException($"{nameof(SizeKind.UpperBound)} is not a valid return value for GetSize.");
            break;
        case SizeKind.Unknown:
            // Not valid yet.
            ThrowHelper.ThrowInvalidOperationException($"{nameof(SizeKind.Unknown)} is not a valid return value for GetSize.");
            break;
        }

        return size;
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
/// Specifies how db null values should be handled for object values within transparent containers (e.g. arrays or ranges).
/// </summary>
/// <remarks>
/// <para>
/// Strongly-typed code paths always delegate all db null logic to the individual converter.
/// Object-typed paths need this per conversion configuration to determine how the converter
/// should pre-process db null values (like DBNull.Value) before delegating to the nested converter.
/// </para>
/// <para>
/// When an object[] converter was resolved for some db parameter it expects its db null behavior to extend to the array elements.
/// Resolving that same converter for an object[] composite field should generally not cause such additional db null behavior to apply.
/// </para>
/// </remarks>
[Experimental(NpgsqlDiagnostics.ConvertersExperimental)]
public enum NestedObjectDbNullHandling
{
    /// <summary>Handle CLR null before delegating to the nested converter (or provider).</summary>
    Default = 0,
    /// <summary>Handle CLR null and additional db null values (e.g. DBNull.Value) before delegating to the nested converter (or provider).</summary>
    Extended,
    /// <summary>Same as <see cref="Extended"/>, but CLR null values will throw an exception.</summary>
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
