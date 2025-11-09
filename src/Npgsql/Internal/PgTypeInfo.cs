using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Npgsql.Internal.Postgres;

namespace Npgsql.Internal;

[Experimental(NpgsqlDiagnostics.ConvertersExperimental)]
public abstract class PgTypeInfo
{
    PgTypeInfo(PgSerializerOptions options, Type type, Type? unboxedType)
    {
        if (unboxedType is not null && !type.IsAssignableFrom(unboxedType))
            throw new ArgumentException("A value of unboxed type is not assignable to converter type", nameof(unboxedType));

        Options = options;
        IsBoxing = unboxedType is not null;
        Type = unboxedType ?? type;
        SupportsReading = GetDefaultSupportsReading(type, unboxedType);
        SupportsWriting = true;
    }

    private protected PgTypeInfo(PgSerializerOptions options, Type type, PgTypeId? pgTypeId, Type? unboxedType = null)
        : this(options, type, unboxedType)
        => PgTypeId = pgTypeId is { } id ? options.GetCanonicalTypeId(id) : null;

    private protected PgTypeInfo(PgSerializerOptions options, PgConverter converter, PgTypeId pgTypeId, Type? unboxedType = null)
        : this(options, converter.TypeToConvert, pgTypeId, unboxedType)
        => Converter = converter;

    public Type Type { get; }
    public PgSerializerOptions Options { get; }

    public bool SupportsReading { get; init; }
    public bool SupportsWriting { get; init; }
    public DataFormat? PreferredFormat { get; init; }

    // Doubles as the storage for the converter coming from a default provider result (used to confirm whether we can use cached info).
    protected PgConverter? Converter { get; }

    // Used for internal converters to save on binary bloat.
    internal bool IsBoxing { get; }

    public PgTypeId? PgTypeId { get; }

    public PgConcreteTypeInfo GetConcreteTypeInfo(Field field)
    {
        if (this is not PgProviderTypeInfo providerTypeInfo)
            return (PgConcreteTypeInfo)this;

        return providerTypeInfo.GetConcreteTypeInfo(field) ?? providerTypeInfo.GetDefaultConcreteTypeInfo(field.PgTypeId);
    }

    public PgConcreteTypeInfo GetConcreteTypeInfo<T>(T? value)
    {
        if (this is not PgProviderTypeInfo providerTypeInfo)
            return (PgConcreteTypeInfo)this;

        return providerTypeInfo.GetConcreteTypeInfo(value, PgTypeId) ?? providerTypeInfo.GetDefaultConcreteTypeInfo(PgTypeId);
    }

    // Note: this api is not called GetConcreteTypeInfoAsObject as the semantics are extended, DBNull is a NULL value for all object values.
    public PgConcreteTypeInfo GetObjectConcreteTypeInfo(object? value)
    {
        switch (this)
        {
        case PgConcreteTypeInfo v:
            return v;
        case PgProviderTypeInfo providerTypeInfo:
            PgConcreteTypeInfo? concreteTypeInfo = null;
            if (value is not DBNull)
                concreteTypeInfo = providerTypeInfo.GetAsObjectConcreteTypeInfo(value, PgTypeId);
            return concreteTypeInfo ?? providerTypeInfo.GetDefaultConcreteTypeInfo(PgTypeId);
        default:
            return ThrowNotSupported();
        }

        static PgConcreteTypeInfo ThrowNotSupported()
            => throw new NotSupportedException("Should not happen, please file a bug.");
    }

    // We assume a boxing type info does not support reading as the converter won't be able to produce the derived type statically.
    // Cases like Array converters unboxing to int[], int[,] etc. are the exception and the reason why SupportsReading is a settable property.
    internal static bool GetDefaultSupportsReading(Type type, Type? reportedType)
        => reportedType is null || ComputeUnboxedType(type, reportedType) is not { } unboxedType || unboxedType == type;

    protected static Type? ComputeUnboxedType(Type converterType, Type reportedType)
    {
        // The hierarchy that should hold for things to work correctly is object < converterType < matchedType.
        Debug.Assert(converterType.IsAssignableFrom(reportedType) || reportedType == typeof(object));

        // A special case for object matches, where we return a more specific type than was reported.
        // This is to report e.g. Array converters as Array when their reported type was object.
        if (reportedType == typeof(object))
        {
            return converterType != typeof(object) ? converterType : null;
        }

        // This is to report e.g. Array converters as int[,,,] when their reported type was such.
        return reportedType != converterType ? reportedType : null;
    }
}

public sealed class PgProviderTypeInfo : PgTypeInfo
{
    readonly PgConcreteTypeInfoProvider _typeInfoProvider;
    readonly PgConcreteTypeInfo? _defaultConcrete;

    // TODO pull validate from options + internal exempt for perf?
    internal bool ValidateProviderResults => true;

    // We always mark providers with type object as boxing, as they may freely return converters for any type (see PgConcreteTypeInfoProvider.Validate).
    public PgProviderTypeInfo(PgSerializerOptions options, PgConcreteTypeInfoProvider typeInfoProvider, PgTypeId? pgTypeId, Type? reportedType = null)
        : base(options, typeInfoProvider.TypeToConvert, pgTypeId,
            (reportedType is null ? null : ComputeUnboxedType(typeInfoProvider.TypeToConvert, reportedType))
            ?? (typeInfoProvider.TypeToConvert == typeof(object) ? typeof(object) : null))
    {
        _typeInfoProvider = typeInfoProvider;
        // Always validate the default provider result, the info will be re-used so there is no real downside.
        _defaultConcrete = typeInfoProvider.GetDefaultInternal(
            validate: true, options.PortableTypeIds, pgTypeId is { } id ? options.GetCanonicalTypeId(id) : null);
    }

    public PgConcreteTypeInfo? GetConcreteTypeInfo<T>(T? value, PgTypeId? expectedPgTypeId)
    {
        if (expectedPgTypeId is { } id && PgTypeId is { } decidedId && id != decidedId)
            ThrowUnexpectedPgTypeId(nameof(expectedPgTypeId));

        return _typeInfoProvider is PgConcreteTypeInfoProvider<T> providerT
            ? providerT.GetInternal(this, value, expectedPgTypeId ?? PgTypeId)
            : ThrowNotSupportedType(typeof(T));

        PgConcreteTypeInfo ThrowNotSupportedType(Type? type)
            => throw new NotSupportedException(IsBoxing
                ? $"TypeInfo only supports boxing conversions, call {nameof(GetAsObjectConcreteTypeInfo)} or {nameof(GetObjectConcreteTypeInfo)} instead."
                : $"TypeInfo is not of type {type}");
    }

    public PgConcreteTypeInfo? GetAsObjectConcreteTypeInfo(object? value, PgTypeId? expectedPgTypeId)
    {
        if (expectedPgTypeId is { } id && PgTypeId is { } decidedId && id != decidedId)
            ThrowUnexpectedPgTypeId(nameof(expectedPgTypeId));

        return _typeInfoProvider.GetAsObjectInternal(this, value, expectedPgTypeId ?? PgTypeId);
    }

    public new PgConcreteTypeInfo? GetConcreteTypeInfo(Field field)
    {
        if (PgTypeId is { } decidedId && field.PgTypeId != decidedId)
            ThrowUnexpectedPgTypeId(nameof(field));

        return _typeInfoProvider.GetInternal(this, field);
    }

    public PgConcreteTypeInfo GetDefaultConcreteTypeInfo(PgTypeId? pgTypeId)
    {
        if (pgTypeId is { } id && PgTypeId is { } decidedId && id != decidedId)
            ThrowUnexpectedPgTypeId(nameof(pgTypeId));

        return _typeInfoProvider.GetDefaultInternal(ValidateProviderResults, Options.PortableTypeIds, pgTypeId ?? PgTypeId);
    }

    public PgConcreteTypeInfoProvider GetTypeInfoProvider() => _typeInfoProvider;

    static void ThrowUnexpectedPgTypeId(string parameterName)
        => throw new ArgumentException($"PgTypeId does not match the decided value on this {nameof(PgProviderTypeInfo)}.", parameterName);
}

public sealed class PgConcreteTypeInfo : PgTypeInfo
{
    readonly bool _canBinaryConvert;
    readonly BufferRequirements _binaryBufferRequirements;

    readonly bool _canTextConvert;
    readonly BufferRequirements _textBufferRequirements;

    public PgConcreteTypeInfo(PgSerializerOptions options, PgConverter converter, PgTypeId pgTypeId, Type? reportedType = null)
        : base(options, converter, pgTypeId, reportedType is null ? null : ComputeUnboxedType(converter.TypeToConvert, reportedType))
    {
        _canBinaryConvert = converter.CanConvert(DataFormat.Binary, out _binaryBufferRequirements);
        _canTextConvert = converter.CanConvert(DataFormat.Text, out _textBufferRequirements);
    }

    public new PgConverter Converter => base.Converter!;
    public new PgTypeId PgTypeId => base.PgTypeId.GetValueOrDefault();

    bool CanConvert(PgConverter converter, DataFormat format, out BufferRequirements bufferRequirements)
    {
        switch (format)
        {
        case DataFormat.Binary:
            bufferRequirements = _binaryBufferRequirements;
            return _canBinaryConvert;
        case DataFormat.Text:
            bufferRequirements = _textBufferRequirements;
            return _canTextConvert;
        }

        return converter.CanConvert(format, out bufferRequirements);
    }

    public BufferRequirements? GetBufferRequirements(DataFormat format)
    {
        var success = CanConvert(Converter, format, out var bufferRequirements);
        return success ? bufferRequirements : null;
    }

    // Having it here so we can easily extend any behavior.
    internal void DisposeWriteState(object writeState)
    {
        if (writeState is IDisposable disposable)
            disposable.Dispose();
    }

    // TryBind for reading.
    internal bool TryBind(DataFormat format, out PgConverterInfo info)
    {
        if (!CanConvert(Converter, format, out var bufferRequirements))
        {
            info = default;
            return false;
        }
        info = new(this, bufferRequirements.Read);
        return true;
    }

    // Bind for reading.
    internal PgConverterInfo Bind(DataFormat format)
    {
        if (!TryBind(format, out var info))
            ThrowHelper.ThrowInvalidOperationException($"Converter does not support {format} format.");

        return info;
    }

    // Bind for writing.
    /// When result is null, the value was interpreted to be a SQL NULL.
    internal PgValueBindingContext BindValue<T>(T? value, DataFormat? formatPreference = null)
    {
        // Basically exists to catch cases like object[] resolving a polymorphic read converter, better to fail during binding than writing.
        if (!SupportsWriting)
            ThrowHelper.ThrowNotSupportedException($"Writing {Type} is not supported for this type info.");

        var format = ResolveFormat(Converter, out var bufferRequirements, formatPreference ?? PreferredFormat);

        object? writeState = null;
        if (((PgConverter<T>)Converter).GetSizeOrDbNull(format, bufferRequirements.Write, value, ref writeState) is not { } size)
            return new(format, bufferRequirements.Write, null, null);

        return new(format, bufferRequirements.Write, size, writeState);
    }

    // Bind for writing.
    // Note: this api is not called BindAsObject as the semantics are extended, DBNull is a NULL value for all object values.
    /// When result is null or DBNull, the value was interpreted to be a SQL NULL.
    internal PgValueBindingContext BindObjectValue(object? value, DataFormat? formatPreference = null)
    {
        // Basically exists to catch cases like object[] resolving a polymorphic read converter, better to fail during binding than writing.
        if (!SupportsWriting)
            throw new NotSupportedException($"Writing {Type} is not supported for this type info.");

        var format = ResolveFormat(Converter, out var bufferRequirements, formatPreference ?? PreferredFormat);

        // Given SQL values are effectively a union of T | NULL we support DBNull.Value to signify a NULL value for all types except DBNull in this api.
        object? writeState = null;
        if (value is DBNull && Type != typeof(DBNull) || Converter.GetSizeOrDbNullAsObject(format, bufferRequirements.Write, value, ref writeState) is not { } size)
        {
            return new(format, bufferRequirements.Write, null, null);
        }

        return new(format, bufferRequirements.Write, size, writeState);
    }

    DataFormat ResolveFormat(PgConverter converter, out BufferRequirements bufferRequirements, DataFormat? formatPreference = null)
    {
        // First try to check for preferred support.
        switch (formatPreference)
        {
        case DataFormat.Binary when CanConvert(converter, DataFormat.Binary, out bufferRequirements):
            return DataFormat.Binary;
        case DataFormat.Text when CanConvert(converter, DataFormat.Text, out bufferRequirements):
            return DataFormat.Text;
        default:
            // The common case, no preference given (or no match) means we default to binary if supported.
            if (CanConvert(converter, DataFormat.Binary, out bufferRequirements))
                return DataFormat.Binary;
            if (CanConvert(converter, DataFormat.Text, out bufferRequirements))
                return DataFormat.Text;

            ThrowHelper.ThrowInvalidOperationException("Converter doesn't support any data format.");
            bufferRequirements = default;
            return default;
        }
    }
}

readonly struct PgValueBindingContext
{
    public DataFormat DataFormat { get; }
    public Size BufferRequirement { get; }
    public Size? Size { get; }
    public object? WriteState { get; }

    internal PgValueBindingContext(DataFormat dataFormat, Size bufferRequirement, Size? size, object? writeState)
    {
        DataFormat = dataFormat;
        BufferRequirement = bufferRequirement;
        Size = size;
        WriteState = writeState;
    }

    [MemberNotNullWhen(false, nameof(Size))]
    public bool IsDbNullBinding => Size is null;
}

readonly struct PgConverterInfo
{
    readonly PgConcreteTypeInfo _typeInfo;

    public PgConverterInfo(PgConcreteTypeInfo pgTypeInfo, Size bufferRequirement)
    {
        _typeInfo = pgTypeInfo;
        BufferRequirement = bufferRequirement;
    }

    public bool IsDefault => _typeInfo is null;

    public Type TypeToConvert => _typeInfo.Type;

    public PgTypeInfo TypeInfo => _typeInfo;

    public PgConverter Converter => _typeInfo.Converter;
    public Size BufferRequirement { get; }

    /// Whether Converter.TypeToConvert matches PgTypeInfo.Type, if it doesn't object apis should be used.
    public bool IsBoxingConverter => _typeInfo.IsBoxing;
}
