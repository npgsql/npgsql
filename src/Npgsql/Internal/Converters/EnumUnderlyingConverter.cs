using System;
using System.Diagnostics;
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

    // Typed enum-underlying helpers. Reached only via the dispatch sites' RuntimeFeature.IsDynamicCodeSupported gate,
    // which ILC discharges to false under NAOT — the helpers are therefore unreachable under NAOT and don't get
    // instantiated per-T (no viral bloat). Under JIT, RyuJIT folds the typecode switch per concrete T at codegen
    // time, leaving only the matching case live, and the (T)(object)v JIT-elides the box+unbox for layout-compatible
    // enum/underlying — zero alloc per call.
    [MethodImpl(MethodImplOptions.NoInlining)]
    static T ReadAsEnumUnderlying<T>(PgReader reader)
    {
        Debug.Assert(typeof(T).IsEnum);
        return Type.GetTypeCode(typeof(T)) switch
        {
            TypeCode.Int32 or TypeCode.UInt32 => (T)(object)reader.ReadInt32(),
            TypeCode.Int64 or TypeCode.UInt64 => (T)(object)reader.ReadInt64(),
            TypeCode.Int16 or TypeCode.UInt16 => (T)(object)reader.ReadInt16(),
            TypeCode.Byte or TypeCode.SByte => (T)(object)checked((byte)reader.ReadInt16()),
            _ => throw new NotSupportedException()
        };
    }

    static bool IsDbNullAsEnumUnderlying<T>(T? value, object? writeState) => false;

    [MethodImpl(MethodImplOptions.NoInlining)]
    static Size BindAsEnumUnderlying<T>(in BindContext context, T value, ref object? writeState)
    {
        Debug.Assert(typeof(T).IsEnum);
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
                writer.WriteInt32((int)(object)value!);
                break;
            case TypeCode.Int64 or TypeCode.UInt64:
                writer.WriteInt64((long)(object)value!);
                break;
            case TypeCode.Int16 or TypeCode.UInt16:
                writer.WriteInt16((short)(object)value!);
                break;
            case TypeCode.Byte or TypeCode.SByte:
                writer.WriteInt16((byte)(object)value!);
                break;
            default: throw new NotSupportedException();
        }
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
    {
        return ConverterDescriptor.Invariant with { BufferRequirements = BufferRequirements.CreateFixedSize(WireSize()) };

        // Known PG wire sizes per .NET underlying type — matches the default Ado mappings. byte/sbyte/short/ushort
        // → Int2 (2 bytes), int/uint → Int4 (4 bytes), long/ulong → Int8 (8 bytes). PG has no unsigned types, so
        // unsigned .NET underlyings ride on the signed wire format with CreateChecked handling range.
        static int WireSize()
            => Type.GetTypeCode(typeof(T)) switch
            {
                TypeCode.Int32 or TypeCode.UInt32 => sizeof(int),
                TypeCode.Int64 or TypeCode.UInt64 => sizeof(long),
                TypeCode.Int16 or TypeCode.UInt16 or TypeCode.Byte or TypeCode.SByte => sizeof(short),
                _ => throw new NotSupportedException()
            };
    }

    public override T Read(PgReader reader)
        => IEnumUnderlyingConverter.ReadAsEnumUnderlying<T>(reader);

    public override void Write(PgWriter writer, T value)
        => IEnumUnderlyingConverter.WriteAsEnumUnderlying(writer, value);

    // Typed Enum.ToObject overload — exactly one allocation per call (the boxed enum), no intermediate boxed
    // underlying. No async machinery: reads are buffered, result is synchronously available.
    internal override ValueTask<object?> ReadAsObject(bool async, PgReader reader, CancellationToken cancellationToken)
        => Type.GetTypeCode(typeof(T)) switch
        {
            TypeCode.Int32 or TypeCode.UInt32 => new(Enum.ToObject(enumType, reader.ReadInt32())),
            TypeCode.Int64 or TypeCode.UInt64 => new(Enum.ToObject(enumType, reader.ReadInt64())),
            TypeCode.Int16 or TypeCode.UInt16 => new(Enum.ToObject(enumType, reader.ReadInt16())),
            TypeCode.Byte or TypeCode.SByte => new(Enum.ToObject(enumType, checked((byte)reader.ReadInt16()))),
            _ => throw new NotSupportedException()
        };
}
