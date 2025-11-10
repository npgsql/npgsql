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

        return providerTypeInfo.GetConcreteTypeInfo(field) ?? providerTypeInfo.GetDefaultConcreteTypeInfo(null);
    }

    public PgConcreteTypeInfo GetConcreteTypeInfo<T>(T? value, out object? writeState)
    {
        if (this is not PgProviderTypeInfo providerTypeInfo)
        {
            writeState = null;
            return (PgConcreteTypeInfo)this;
        }

        return providerTypeInfo.GetConcreteTypeInfo(default, value, out writeState) ?? providerTypeInfo.GetDefaultConcreteTypeInfo(null);
    }

    // Note: this api is not called GetConcreteTypeInfoAsObject as the semantics are extended, DBNull is a NULL value for all object values.
    public PgConcreteTypeInfo GetObjectConcreteTypeInfo(object? value, out object? writeState)
    {
        writeState = null;
        switch (this)
        {
        case PgConcreteTypeInfo v:
            return v;
        case PgProviderTypeInfo providerTypeInfo:
            PgConcreteTypeInfo? concreteTypeInfo = null;
            if (value is not DBNull)
                concreteTypeInfo = providerTypeInfo.GetAsObjectConcreteTypeInfo(default, value, out writeState);
            return concreteTypeInfo ?? providerTypeInfo.GetDefaultConcreteTypeInfo(null);
        default:
            return ThrowNotSupported();
        }

        static PgConcreteTypeInfo ThrowNotSupported()
            => throw new NotSupportedException("Should not happen, please file a bug.");
    }

    // We assume a boxing type info does not support reading as the converter won't be able to produce the derived type statically.
    // Cases like Array converters unboxing to int[], int[,] etc. are the exception and the reason why SupportsReading is a settable property.
    internal static bool GetDefaultSupportsReading(Type type, Type? requestedType)
        => requestedType is null || ComputeUnboxedType(type, requestedType) is not { } unboxedType || unboxedType == type;

    protected static Type? ComputeUnboxedType(Type converterType, Type requestedType)
    {
        // The hierarchy that should hold for things to work correctly is object < converterType < requestedType.
        Debug.Assert(converterType.IsAssignableFrom(requestedType) || requestedType == typeof(object));

        // A special case for object matches, where we return a more specific type than was requested.
        // This is to report e.g. Array converters as Array when their requested type was object.
        if (requestedType == typeof(object))
        {
            return converterType != typeof(object) ? converterType : null;
        }

        // This is to report e.g. Array converters as int[,,,] when their requested type was such.
        return requestedType != converterType ? requestedType : null;
    }
}

public sealed class PgProviderTypeInfo : PgTypeInfo
{
    readonly PgConcreteTypeInfoProvider _typeInfoProvider;
    readonly PgConcreteTypeInfo? _defaultConcrete;

    public PgProviderTypeInfo(PgSerializerOptions options, PgConcreteTypeInfoProvider typeInfoProvider, PgTypeId? pgTypeId)
        : this(options, typeInfoProvider, pgTypeId, requestedType: null)
    {}

    // We always mark providers with type object as boxing, as they may freely return converters for any type (see PgConcreteTypeInfoProvider.Validate).
    internal PgProviderTypeInfo(PgSerializerOptions options, PgConcreteTypeInfoProvider typeInfoProvider, PgTypeId? pgTypeId, Type? requestedType)
        : base(options, typeInfoProvider.TypeToConvert, pgTypeId,
            (requestedType is null ? null : ComputeUnboxedType(typeInfoProvider.TypeToConvert, requestedType))
            ?? (typeInfoProvider.TypeToConvert == typeof(object) ? typeof(object) : null))
    {
        _typeInfoProvider = typeInfoProvider;

        // Always validate the default provider result, the info will be re-used so there is no real downside.
        var result = typeInfoProvider.GetDefault(pgTypeId is { } id ? options.GetCanonicalTypeId(id) : null);
        ValidateResult(nameof(PgConcreteTypeInfoProvider.GetDefault), result, typeInfoProvider.TypeToConvert, options.PortableTypeIds);
        _defaultConcrete = result;
    }

    public PgConcreteTypeInfo GetDefaultConcreteTypeInfo(PgTypeId? pgTypeId)
    {
        if (pgTypeId is { } id && PgTypeId is { } decidedId && id != decidedId)
            ThrowUnexpectedPgTypeId(nameof(pgTypeId));

        var result = _typeInfoProvider.GetDefault(pgTypeId ?? PgTypeId);
        ValidateResult(nameof(PgConcreteTypeInfoProvider.GetDefault), result);
        return result;
    }

    public new PgConcreteTypeInfo? GetConcreteTypeInfo(Field field)
    {
        if (PgTypeId is { } decidedId && field.PgTypeId != decidedId)
            ThrowUnexpectedPgTypeId(nameof(field));

        var result = _typeInfoProvider.GetForField(field);
        if (result is not null)
            ValidateResult(nameof(PgConcreteTypeInfoProvider.GetForField), result);
        return result;
    }

    public PgConcreteTypeInfo? GetConcreteTypeInfo<T>(ProviderValueContext context, T? value, out object? writeState)
    {
        if (PgTypeId is { } pgTypeId)
        {
            if (context.ExpectedPgTypeId is not { } expectedId)
            {
                context = context with { ExpectedPgTypeId = pgTypeId };
            }
            else if (pgTypeId != expectedId)
                ThrowUnexpectedPgTypeId(nameof(context.ExpectedPgTypeId));
        }

        writeState = null;
        var result = _typeInfoProvider is PgConcreteTypeInfoProvider<T> providerT
            ? providerT.GetForValue(context, value, ref writeState)
            : ThrowNotSupportedType(typeof(T));

        if (result is not null)
            ValidateResult(nameof(PgConcreteTypeInfoProvider<>.GetForValue), result);
        return result;

        PgConcreteTypeInfo ThrowNotSupportedType(Type? type)
            => throw new NotSupportedException(IsBoxing
                ? $"TypeInfo only supports boxing conversions, call {nameof(GetAsObjectConcreteTypeInfo)} or {nameof(GetObjectConcreteTypeInfo)} instead."
                : $"TypeInfo is not of type {type}");
    }

    public PgConcreteTypeInfo? GetAsObjectConcreteTypeInfo(ProviderValueContext context, object? value, out object? writeState)
    {
        if (PgTypeId is { } pgTypeId)
        {
            if (context.ExpectedPgTypeId is not { } expectedId)
            {
                context = context with { ExpectedPgTypeId = pgTypeId };
            }
            else if (pgTypeId != expectedId)
                ThrowUnexpectedPgTypeId(nameof(context.ExpectedPgTypeId));
        }

        writeState = null;
        var result = _typeInfoProvider.GetForValueAsObject(context, value, ref writeState);
        if (result is not null)
            ValidateResult(nameof(PgConcreteTypeInfoProvider.GetForValueAsObject), result);
        return result;
    }

    public static PgConcreteTypeInfoProvider GetProvider(PgProviderTypeInfo instance) => instance._typeInfoProvider;

    static void ThrowUnexpectedPgTypeId(string parameterName)
        => throw new ArgumentException($"PgTypeId does not match the decided value on this {nameof(PgProviderTypeInfo)}.", parameterName);

    void ValidateResult(string methodName, PgConcreteTypeInfo result)
        => ValidateResult(methodName, result, _typeInfoProvider.TypeToConvert, Options.PortableTypeIds);

    static void ValidateResult(string methodName, PgConcreteTypeInfo result, Type expectedTypeToConvert, bool expectPortableTypeIds)
    {
        if (expectedTypeToConvert != typeof(object) && result.Converter.TypeToConvert != expectedTypeToConvert)
            throw new InvalidOperationException($"'{methodName}' returned a {nameof(result.Converter)} of type {result.Converter.TypeToConvert} instead of {expectedTypeToConvert} unexpectedly.");

        if (expectPortableTypeIds && result.PgTypeId.IsOid || !expectPortableTypeIds && result.PgTypeId.IsDataTypeName)
            throw new InvalidOperationException($"'{methodName}' returned a concrete type info with a {nameof(result.PgTypeId)} that was not in canonical form.");
    }
}

public sealed class PgConcreteTypeInfo : PgTypeInfo
{
    readonly bool _canBinaryConvert;
    readonly BufferRequirements _binaryBufferRequirements;

    readonly bool _canTextConvert;
    readonly BufferRequirements _textBufferRequirements;

    public PgConcreteTypeInfo(PgSerializerOptions options, PgConverter converter, PgTypeId pgTypeId)
        : this(options, converter, pgTypeId, requestedType: null)
    {}

    internal PgConcreteTypeInfo(PgSerializerOptions options, PgConverter converter, PgTypeId pgTypeId, Type? requestedType)
        : base(options, converter, pgTypeId, requestedType is null ? null : ComputeUnboxedType(converter.TypeToConvert, requestedType))
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
    internal bool TryBindField(DataFormat format, out PgFieldBinding binding)
    {
        if (!CanConvert(Converter, format, out var bufferRequirements))
        {
            binding = default;
            return false;
        }
        binding = new(format, bufferRequirements.Read);
        return true;
    }

    // Bind for reading.
    internal PgFieldBinding BindField(DataFormat format)
    {
        if (!TryBindField(format, out var info))
            ThrowHelper.ThrowInvalidOperationException($"Converter does not support {format} format.");

        return info;
    }

    // Bind for writing.
    /// When result is null, the value was interpreted to be a SQL NULL.
    internal PgValueBinding BindParameterValue<T>(T? value, object? writeState, DataFormat? formatPreference = null)
    {
        // Basically exists to catch cases like object[] resolving a polymorphic read converter, better to fail during binding than writing.
        if (!SupportsWriting)
            ThrowHelper.ThrowNotSupportedException($"Writing {Type} is not supported for this type info.");

        var format = ResolveFormat(Converter, out var bufferRequirements, formatPreference ?? PreferredFormat);

        if (((PgConverter<T>)Converter).GetSizeOrDbNull(format, bufferRequirements.Write, value, ref writeState) is not { } size)
            return new(format, bufferRequirements.Write, null, null);

        return new(format, bufferRequirements.Write, size, writeState);
    }

    // Bind for writing.
    // Note: this api is not called BindAsObject as the semantics are extended, DBNull is a NULL value for all object values.
    /// When result is null or DBNull, the value was interpreted to be a SQL NULL.
    internal PgValueBinding BindParameterObjectValue(object? value, object? writeState, DataFormat? formatPreference = null)
    {
        // Basically exists to catch cases like object[] resolving a polymorphic read converter, better to fail during binding than writing.
        if (!SupportsWriting)
            ThrowHelper.ThrowNotSupportedException($"Writing {Type} is not supported for this type info.");

        var format = ResolveFormat(Converter, out var bufferRequirements, formatPreference ?? PreferredFormat);

        // Given SQL values are effectively a union of T | NULL we support DBNull.Value to signify a NULL value for all types except DBNull in this api.
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

readonly struct PgFieldBinding
{
    internal PgFieldBinding(DataFormat dataFormat, Size bufferRequirement)
    {
        DataFormat = dataFormat;
        BufferRequirement = bufferRequirement;
    }

    public DataFormat DataFormat { get; }
    public Size BufferRequirement { get; }
}

readonly struct PgValueBinding
{
    public DataFormat DataFormat { get; }
    public Size BufferRequirement { get; }
    public Size? Size { get; }
    public object? WriteState { get; }

    internal PgValueBinding(DataFormat dataFormat, Size bufferRequirement, Size? size, object? writeState)
    {
        DataFormat = dataFormat;
        BufferRequirement = bufferRequirement;
        Size = size;
        WriteState = writeState;
    }

    [MemberNotNullWhen(false, nameof(Size))]
    public bool IsDbNullBinding => Size is null;
}
