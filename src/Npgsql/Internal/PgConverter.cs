using System;
using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql.Internal;

[Experimental(NpgsqlDiagnostics.ConvertersExperimental)]
public abstract class PgConverter
{
    internal DbNullPredicate DbNullPredicateKind { get; }
    public bool IsDbNullable => DbNullPredicateKind is not DbNullPredicate.None;

    private protected PgConverter(Type type, bool isNullDefaultValue, bool customDbNullPredicate = false)
        => DbNullPredicateKind = customDbNullPredicate ? DbNullPredicate.Custom : InferDbNullPredicate(type, isNullDefaultValue);

    /// <summary>
    /// Whether this converter can handle the given format and with which buffer requirements.
    /// </summary>
    /// <param name="format">The data format.</param>
    /// <param name="bufferRequirements">Returns the buffer requirements.</param>
    /// <returns>Returns true if the given data format is supported.</returns>
    /// <remarks>The buffer requirements should not cover database NULL reads or writes, these are handled by the caller.</remarks>
    public abstract bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements);

    internal abstract Type TypeToConvert { get; }

    internal bool IsDbNullAsObject([NotNullWhen(false)] object? value, ref object? writeState)
        => DbNullPredicateKind switch
        {
            DbNullPredicate.Null => value is null,
            DbNullPredicate.None => false,
            DbNullPredicate.PolymorphicNull => value is null or DBNull,
            // We do the null check to keep the NotNullWhen(false) invariant.
            DbNullPredicate.Custom => IsDbNullValueAsObject(value, ref writeState) || (value is null && ThrowInvalidNullValue()),
            _ => ThrowDbNullPredicateOutOfRange()
        };

    private protected abstract bool IsDbNullValueAsObject(object? value, ref object? writeState);

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

    static DbNullPredicate InferDbNullPredicate(Type type, bool isNullDefaultValue)
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
    private protected void ThrowIORequired(Size bufferRequirement)
        => throw new InvalidOperationException($"Buffer requirement '{bufferRequirement}' not respected for converter '{GetType().FullName}', expected no IO to be required.");

    private protected static bool ThrowInvalidNullValue()
        => throw new ArgumentNullException("value", "Null value given for non-nullable type converter");

    private protected bool ThrowDbNullPredicateOutOfRange()
        => throw new UnreachableException($"Unknown case {DbNullPredicateKind.ToString()}");

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

    protected virtual bool IsDbNullValue(T? value, ref object? writeState) => throw new NotSupportedException();

    // Object null semantics as follows, if T is a struct (so excluding nullable) report false for null values, don't throw on the cast.
    // As a result this creates symmetry with IsDbNull when we're dealing with a struct T, as it cannot be passed null at all.
    private protected override bool IsDbNullValueAsObject(object? value, ref object? writeState)
        => (default(T) is null || value is not null) && IsDbNullValue((T?)value, ref writeState);

    public bool IsDbNull([NotNullWhen(false)] T? value, ref object? writeState)
        => DbNullPredicateKind switch
        {
            DbNullPredicate.Null => value is null,
            DbNullPredicate.None => false,
            DbNullPredicate.PolymorphicNull => value is null or DBNull,
            // We do the null check to keep the NotNullWhen(false) invariant.
            DbNullPredicate.Custom => IsDbNullValue(value, ref writeState) || (value is null && ThrowInvalidNullValue()),
            _ => ThrowDbNullPredicateOutOfRange()
        };

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
        if (converter.IsDbNull(value, ref writeState))
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

    public static Size? GetSizeOrDbNullAsObject(this PgConverter converter, DataFormat format, Size writeRequirement, object? value, ref object? writeState)
    {
        if (converter.IsDbNullAsObject(value, ref writeState))
            return null;

        if (writeRequirement is { Kind: SizeKind.Exact, Value: var byteCount })
            return byteCount;
        var size = converter.GetSizeAsObject(new(format, writeRequirement), value, ref writeState);

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

    internal static PgConverter<T> UnsafeDowncast<T>(this PgConverter converter)
    {
        // Justification: avoid perf cost of casting to a known base class type per read/write, see callers.
        Debug.Assert(converter is PgConverter<T>);
        return Unsafe.As<PgConverter<T>>(converter);
    }
}

[method: SetsRequiredMembers]
public readonly struct SizeContext(DataFormat format, Size bufferRequirement)
{
    public required Size BufferRequirement { get; init; } = bufferRequirement;
    public DataFormat Format { get; } = format;
}

class MultiWriteState : IDisposable
{
    public required ArrayPool<(Size Size, object? WriteState)>? ArrayPool { get; init; }
    public required ArraySegment<(Size Size, object? WriteState)> Data { get; init; }
    public required bool AnyWriteState { get; init; }

    public void Dispose()
    {
        if (Data.Array is not { } array)
            return;

        if (AnyWriteState)
        {
            for (var i = Data.Offset; i < array.Length; i++)
                if (array[i].WriteState is IDisposable disposable)
                    disposable.Dispose();

            Array.Clear(Data.Array, Data.Offset, Data.Count);
        }

        ArrayPool?.Return(Data.Array);
    }
}
