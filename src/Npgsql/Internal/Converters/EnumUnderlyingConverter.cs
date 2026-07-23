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
        => typeof(T).IsEnum && Enum.GetUnderlyingType(typeof(T)) == converter.TypeToConvert && converter is IEnumUnderlyingConverter;

    // Typed enum-underlying helpers. The enum-typed instantiations are reached only via the dispatch sites'
    // RuntimeFeature.IsDynamicCodeSupported gate, which ILC discharges to false under NAOT — so they don't get
    // instantiated per enum-T (no viral bloat). Under JIT, RyuJIT folds the typecode switch per concrete T at
    // codegen time, leaving only the matching case live. T is the enum or (via the converter's own Read/Write)
    // its underlying primitive; Unsafe.BitCast reinterprets the layout-identical bits — signed and unsigned
    // alike — without boxing.
    [MethodImpl(MethodImplOptions.NoInlining)]
    static T ReadAsEnumUnderlying<T>(PgReader reader)
        => Type.GetTypeCode(typeof(T)) switch
        {
            TypeCode.Int32 or TypeCode.UInt32 => Unsafe.BitCast<int, T>(reader.ReadInt32()),
            TypeCode.Int64 or TypeCode.UInt64 => Unsafe.BitCast<long, T>(reader.ReadInt64()),
            TypeCode.Int16 or TypeCode.UInt16 => Unsafe.BitCast<short, T>(reader.ReadInt16()),
            TypeCode.Byte or TypeCode.SByte => Unsafe.BitCast<byte, T>(checked((byte)reader.ReadInt16())),
            _ => throw new NotSupportedException()
        };

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
        switch (Type.GetTypeCode(typeof(T)))
        {
            case TypeCode.Int32 or TypeCode.UInt32:
                writer.WriteInt32(Unsafe.BitCast<T, int>(value));
                break;
            case TypeCode.Int64 or TypeCode.UInt64:
                writer.WriteInt64(Unsafe.BitCast<T, long>(value));
                break;
            case TypeCode.Int16 or TypeCode.UInt16:
                writer.WriteInt16(Unsafe.BitCast<T, short>(value));
                break;
            case TypeCode.Byte or TypeCode.SByte:
                writer.WriteInt16(Unsafe.BitCast<T, byte>(value));
                break;
            default: throw new NotSupportedException();
        }
    }

    // Known PG wire sizes per .NET underlying type — matches the default Ado mappings. byte/sbyte/short/ushort
    // → Int2 (2 bytes), int/uint → Int4 (4 bytes), long/ulong → Int8 (8 bytes). PG has no unsigned types, so
    // unsigned .NET underlyings are bit-reinterpreted onto the signed wire format of the same width.
    static int WireSize<T>()
        => Type.GetTypeCode(typeof(T)) switch
        {
            TypeCode.Int32 or TypeCode.UInt32 => sizeof(int),
            TypeCode.Int64 or TypeCode.UInt64 => sizeof(long),
            TypeCode.Int16 or TypeCode.UInt16 or TypeCode.Byte or TypeCode.SByte => sizeof(short),
            _ => throw new NotSupportedException()
        };

    // Typed Enum.ToObject overload — one allocation (the boxed enum), no intermediate boxed underlying.
    // Used by both the value and nullable converter's AsObject paths.
    [MethodImpl(MethodImplOptions.NoInlining)]
    static object ReadAsBoxedEnum<T>(PgReader reader, Type enumType)
        => Type.GetTypeCode(typeof(T)) switch
        {
            TypeCode.Int32 or TypeCode.UInt32 => Enum.ToObject(enumType, reader.ReadInt32()),
            TypeCode.Int64 or TypeCode.UInt64 => Enum.ToObject(enumType, reader.ReadInt64()),
            TypeCode.Int16 or TypeCode.UInt16 => Enum.ToObject(enumType, reader.ReadInt16()),
            TypeCode.Byte or TypeCode.SByte => Enum.ToObject(enumType, checked((byte)reader.ReadInt16())),
            _ => throw new NotSupportedException()
        };
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

    internal override ValueTask<object?> ReadAsObject(bool async, PgReader reader, CancellationToken cancellationToken)
        => new(IEnumUnderlyingConverter.ReadAsBoxedEnum<T>(reader, enumType));
}
