using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql.Internal;

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
        => (default(T) is null || value is not null) && IsDbNullValue(Downcast(value));

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
        => GetSize(context, Downcast(value), ref writeState);

    [MethodImpl(MethodImplOptions.NoInlining)]
    [return: NotNullIfNotNull(nameof(value))]
    static T? Downcast(object? value) => (T?)value;
}

static class PgConverterExtensions
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
        {
            for (var i = Data.Offset; i < array.Length; i++)
                if (array[i].WriteState is IDisposable disposable)
                    disposable.Dispose();

            Array.Clear(Data.Array, Data.Offset, Data.Count);
        }

        ArrayPool?.Return(Data.Array);
    }
}
