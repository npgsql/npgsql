using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql.Internal.Converters;

/// <summary>
/// Non-generic marker for <see cref="EnumUnderlyingConverter{T}"/>. Used by <see cref="PgConcreteTypeInfo"/> at
/// construction time to enforce that only this converter may participate in enum reported-type widening.
/// </summary>
interface IEnumUnderlyingConverter
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IsEnumUnderlyingConversion<T>(PgConverter converter)
        => typeof(T).IsEnum && typeof(T).GetEnumUnderlyingType() == converter.TypeToConvert && converter is IEnumUnderlyingConverter;

    // Keep IsEnum directly at dispatch sites too: the NativeAOT scanner doesn't inline the predicate
    // above, so hiding the guard there would retain these helpers for non-enum callers.
    // T is either an enum or its underlying primitive. Direct underlying-type comparisons fold on
    // both CoreCLR and NativeAOT; Type.GetTypeCode can retain runtime enum checks under NativeAOT.
    [MethodImpl(MethodImplOptions.NoInlining)]
    static T ReadAsEnumUnderlying<T>(PgReader reader)
    {
        var type = typeof(T).IsEnum ? typeof(T).GetEnumUnderlyingType() : typeof(T);
        if (type == typeof(int) || type == typeof(uint))
            return Unsafe.BitCast<int, T>(reader.ReadInt32());
        if (type == typeof(long) || type == typeof(ulong))
            return Unsafe.BitCast<long, T>(reader.ReadInt64());
        if (type == typeof(short) || type == typeof(ushort))
            return Unsafe.BitCast<short, T>(reader.ReadInt16());
        if (type == typeof(byte))
            return Unsafe.BitCast<byte, T>(checked((byte)reader.ReadInt16()));
        if (type == typeof(sbyte))
            return Unsafe.BitCast<sbyte, T>(checked((sbyte)reader.ReadInt16()));
        throw new NotSupportedException();
    }

    static bool IsDbNullAsEnumUnderlying<T>(T? value, object? writeState) => false;

    [MethodImpl(MethodImplOptions.NoInlining)]
    static Size BindAsEnumUnderlying<T>(in BindContext context, T value, ref object? writeState)
    {
        if (context.IsBindOptional)
            return context.BufferRequirement;

        throw new NotSupportedException();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    static void WriteAsEnumUnderlying<T>(PgWriter writer, T value)
    {
        var type = typeof(T).IsEnum ? typeof(T).GetEnumUnderlyingType() : typeof(T);
        if (type == typeof(int) || type == typeof(uint))
            writer.WriteInt32(Unsafe.BitCast<T, int>(value));
        else if (type == typeof(long) || type == typeof(ulong))
            writer.WriteInt64(Unsafe.BitCast<T, long>(value));
        else if (type == typeof(short) || type == typeof(ushort))
            writer.WriteInt16(Unsafe.BitCast<T, short>(value));
        else if (type == typeof(byte))
            writer.WriteInt16(Unsafe.BitCast<T, byte>(value));
        else if (type == typeof(sbyte))
            writer.WriteInt16(Unsafe.BitCast<T, sbyte>(value));
        else
            throw new NotSupportedException();
    }

    // Known PG wire sizes per .NET underlying type — matches the default Ado mappings. byte/sbyte/short/ushort
    // → Int2 (2 bytes), int/uint → Int4 (4 bytes), long/ulong → Int8 (8 bytes). PG has no unsigned types, so
    // unsigned .NET underlyings are bit-reinterpreted onto the signed wire format of the same width.
    static int WireSize<T>()
    {
        if (typeof(T) == typeof(int) || typeof(T) == typeof(uint))
            return sizeof(int);
        if (typeof(T) == typeof(long) || typeof(T) == typeof(ulong))
            return sizeof(long);
        if (typeof(T) == typeof(short) || typeof(T) == typeof(ushort)
            || typeof(T) == typeof(byte) || typeof(T) == typeof(sbyte))
            return sizeof(short);
        throw new NotSupportedException();
    }

    // Typed Enum.ToObject overload — one allocation (the boxed enum), no intermediate boxed underlying.
    // Used by both the value and nullable converter's AsObject paths.
    [MethodImpl(MethodImplOptions.NoInlining)]
    static object ReadAsBoxedEnum<T>(PgReader reader, Type enumType)
    {
        if (typeof(T) == typeof(int) || typeof(T) == typeof(uint))
            return Enum.ToObject(enumType, reader.ReadInt32());
        if (typeof(T) == typeof(long) || typeof(T) == typeof(ulong))
            return Enum.ToObject(enumType, reader.ReadInt64());
        if (typeof(T) == typeof(short) || typeof(T) == typeof(ushort))
            return Enum.ToObject(enumType, reader.ReadInt16());
        if (typeof(T) == typeof(byte))
            return Enum.ToObject(enumType, checked((byte)reader.ReadInt16()));
        if (typeof(T) == typeof(sbyte))
            return Enum.ToObject(enumType, checked((sbyte)reader.ReadInt16()));
        throw new NotSupportedException();
    }
}

/// <summary>
/// Converter for a CLR enum's underlying primitive. Reads/writes the primitive directly using the PG wire format
/// Npgsql's default mappings use for the corresponding .NET type (byte/sbyte/short → smallint, int → integer,
/// long → bigint), and re-materializes the boxed value as the enum on the AsObject path via
/// <see cref="Enum.ToObject(Type, object)"/>. Inherits <see cref="PgBufferedConverter{T}"/> so all per-T async
/// accessor codegen is provided by the base — only the AsObject read needs an override here. Carrying the enum
/// type here lets callers holding a non-generic <see cref="PgConverter"/> reference get a correctly-boxed enum.
/// </summary>
sealed class EnumUnderlyingConverter<T>(Type enumType) : PgBufferedConverter<T>, IEnumUnderlyingConverter
    where T : unmanaged, IBinaryInteger<T>
{
    public override ConverterDescriptor GetDescriptor(in DescriptorContext context)
        => ConverterDescriptor.Invariant with { BufferRequirements = BufferRequirements.CreateFixedSize(IEnumUnderlyingConverter.WireSize<T>()) };

    public override T Read(PgReader reader)
        => IEnumUnderlyingConverter.ReadAsEnumUnderlying<T>(reader);

    public override void Write(PgWriter writer, T value)
        => IEnumUnderlyingConverter.WriteAsEnumUnderlying(writer, value);

    // Typed Enum.ToObject overload — exactly one allocation per call (the boxed enum), no intermediate boxed
    // underlying. No async machinery: reads are buffered, result is synchronously available.
    internal override ValueTask<object?> ReadAsObject(bool async, PgReader reader, CancellationToken cancellationToken)
        => new(IEnumUnderlyingConverter.ReadAsBoxedEnum<T>(reader, enumType));
}

/// <summary>
/// Nullable sibling of <see cref="EnumUnderlyingConverter{T}"/>. Same wire format, same boxing behavior on the
/// AsObject path (boxed <see cref="Nullable{T}"/> strips the wrapper, so a boxed enum unboxes to TEnum? at the
/// caller). Read/Write delegate to the shared <see cref="IEnumUnderlyingConverter"/> helpers; the framework's
/// null-handling stops at the wire-length check, so Read/Write only see non-null values here.
/// </summary>
sealed class EnumUnderlyingNullableConverter<T>(Type enumType) : PgBufferedConverter<T?>, IEnumUnderlyingConverter
    where T : unmanaged, IBinaryInteger<T>
{
    public override ConverterDescriptor GetDescriptor(in DescriptorContext context)
        => ConverterDescriptor.Invariant with { BufferRequirements = BufferRequirements.CreateFixedSize(IEnumUnderlyingConverter.WireSize<T>()) };

    public override T? Read(PgReader reader)
        => IEnumUnderlyingConverter.ReadAsEnumUnderlying<T>(reader);

    public override void Write(PgWriter writer, T? value)
        => IEnumUnderlyingConverter.WriteAsEnumUnderlying(writer, value.GetValueOrDefault());

    internal override ValueTask WriteAsObject(bool async, PgWriter writer, object? value, CancellationToken cancellationToken)
    {
        // A boxed enum can unbox as its underlying T, but not directly as Nullable<T>.
        Write(writer, value is null ? null : (T)value);
        return default;
    }

    internal override ValueTask<object?> ReadAsObject(bool async, PgReader reader, CancellationToken cancellationToken)
        => new(IEnumUnderlyingConverter.ReadAsBoxedEnum<T>(reader, enumType));
}
