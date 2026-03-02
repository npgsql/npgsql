using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Npgsql.Internal.Postgres;

namespace Npgsql.Internal;

[Experimental(NpgsqlDiagnostics.ConvertersExperimental)]
public abstract class PgTypeInfo
{
    readonly bool _canBinaryConvert;
    readonly BufferRequirements _binaryBufferRequirements;

    readonly bool _canTextConvert;
    readonly BufferRequirements _textBufferRequirements;

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

    private protected PgTypeInfo(PgSerializerOptions options, PgConverter converter, PgTypeId pgTypeId, Type? unboxedType = null)
        : this(options, converter.TypeToConvert, unboxedType)
    {
        Converter = converter;
        PgTypeId = options.GetCanonicalTypeId(pgTypeId);
        _canBinaryConvert = converter.CanConvert(DataFormat.Binary, out _binaryBufferRequirements);
        _canTextConvert = converter.CanConvert(DataFormat.Text, out _textBufferRequirements);
    }

    private protected PgTypeInfo(PgSerializerOptions options, Type type, PgConcreteTypeInfo? defaultConcrete, Type? unboxedType = null)
        : this(options, type, unboxedType)
    {
        if (defaultConcrete is not null)
        {
            Debug.Assert(options.PortableTypeIds && defaultConcrete.PgTypeId.IsDataTypeName || !options.PortableTypeIds && defaultConcrete.PgTypeId.IsOid);
            PgTypeId = defaultConcrete.PgTypeId;
            Converter = defaultConcrete.Converter;
            _canBinaryConvert = defaultConcrete.Converter.CanConvert(DataFormat.Binary, out _binaryBufferRequirements);
            _canTextConvert = defaultConcrete.Converter.CanConvert(DataFormat.Text, out _textBufferRequirements);
        }
    }

    bool HasCachedInfo(PgConverter converter) => ReferenceEquals(Converter, converter);

    public Type Type { get; }
    public PgSerializerOptions Options { get; }

    public bool SupportsReading { get; init; }
    public bool SupportsWriting { get; init; }
    public DataFormat? PreferredFormat { get; init; }

    // Doubles as the storage for the converter coming from a default provider result (used to confirm whether we can use cached info).
    protected PgConverter? Converter { get; }

    // TODO pull validate from options + internal exempt for perf?
    internal bool ValidateProviderResults => true;

    // Used for internal converters to save on binary bloat.
    internal bool IsBoxing { get; }

    public PgTypeId? PgTypeId { get; }

    // Having it here so we can easily extend any behavior.
    internal void DisposeWriteState(object writeState)
    {
        if (writeState is IDisposable disposable)
            disposable.Dispose();
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

    bool CanConvert(PgConverter converter, DataFormat format, out BufferRequirements bufferRequirements)
    {
        if (HasCachedInfo(converter))
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
        }

        return converter.CanConvert(format, out bufferRequirements);
    }

    public BufferRequirements? GetBufferRequirements(PgConverter converter, DataFormat format)
    {
        var success = CanConvert(converter, format, out var bufferRequirements);
        return success ? bufferRequirements : null;
    }

    // TryBind for reading.
    internal bool TryBind(Field field, DataFormat format, out PgConverterInfo info)
    {
        switch (this)
        {
        case PgConcreteTypeInfo v:
            if (!CanConvert(v.Converter, format, out var bufferRequirements))
            {
                info = default;
                return false;
            }
            info = new(this, v.Converter, bufferRequirements.Read);
            return true;
        case PgProviderTypeInfo providerTypeInfo:
            var concreteTypeInfo = providerTypeInfo.GetConcreteTypeInfo(field) ?? providerTypeInfo.GetDefaultConcreteTypeInfo(field.PgTypeId);
            if (!CanConvert(concreteTypeInfo.Converter, format, out bufferRequirements))
            {
                info = default;
                return false;
            }
            info = new(this, concreteTypeInfo.Converter, bufferRequirements.Read);
            return true;
        default:
            throw new NotSupportedException("Should not happen, please file a bug.");
        }
    }

    // Bind for reading.
    internal PgConverterInfo Bind(Field field, DataFormat format)
    {
        if (!TryBind(field, format, out var info))
            ThrowHelper.ThrowInvalidOperationException($"Resolved converter does not support {format} format.");

        return info;
    }

    // Bind for writing.
    /// When result is null, the value was interpreted to be a SQL NULL.
    internal PgConverterInfo? Bind<T>(PgConverter<T> converter, T? value, out Size size, out object? writeState, out DataFormat format, DataFormat? formatPreference = null)
    {
        // Basically exists to catch cases like object[] resolving a polymorphic read converter, better to fail during binding than writing.
        if (!SupportsWriting)
            ThrowHelper.ThrowNotSupportedException($"Writing {Type} is not supported for this type info.");

        format = ResolveFormat(converter, out var bufferRequirements, formatPreference ?? PreferredFormat);

        writeState = null;
        if (converter.GetSizeOrDbNull(format, bufferRequirements.Write, value, ref writeState) is not { } sizeOrDbNull)
        {
            size = default;
            return null;
        }

        size = sizeOrDbNull;
        return new(this, converter, bufferRequirements.Write);
    }

    // Bind for writing.
    // Note: this api is not called BindAsObject as the semantics are extended, DBNull is a NULL value for all object values.
    /// When result is null or DBNull, the value was interpreted to be a SQL NULL.
    internal PgConverterInfo? BindObject(PgConverter converter, object? value, out Size size, out object? writeState, out DataFormat format, DataFormat? formatPreference = null)
    {
        // Basically exists to catch cases like object[] resolving a polymorphic read converter, better to fail during binding than writing.
        if (!SupportsWriting)
            throw new NotSupportedException($"Writing {Type} is not supported for this type info.");

        format = ResolveFormat(converter, out var bufferRequirements, formatPreference ?? PreferredFormat);

        // Given SQL values are effectively a union of T | NULL we support DBNull.Value to signify a NULL value for all types except DBNull in this api.
        writeState = null;
        if (value is DBNull && Type != typeof(DBNull) || converter.GetSizeOrDbNullAsObject(format, bufferRequirements.Write, value, ref writeState) is not { } sizeOrDbNull)
        {
            size = default;
            return null;
        }

        size = sizeOrDbNull;
        return new(this, converter, bufferRequirements.Write);
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

    // We assume a boxing type info does not support reading as the converter won't be able to produce the derived type statically.
    // Cases like Array converters unboxing to int[], int[,] etc. are the exception and the reason why SupportsReading is a settable property.
    internal static bool GetDefaultSupportsReading(Type type, Type? unboxedType)
        => unboxedType is null || unboxedType == type;
}

// We always mark providers with type object as boxing, as they may freely return converters for any type (see PgConcreteTypeInfoProvider.Validate).
public sealed class PgProviderTypeInfo(
    PgSerializerOptions options,
    PgConcreteTypeInfoProvider typeInfoProvider,
    PgTypeId? pgTypeId,
    Type? unboxedType = null)
    : PgTypeInfo(options,
        typeInfoProvider.TypeToConvert,
        pgTypeId is { } typeId ? GetDefault(options, typeInfoProvider, typeId) : null,
        unboxedType ?? (typeInfoProvider.TypeToConvert == typeof(object) ? typeof(object) : null))
{

    // We'll always validate the default provider result, the info will be re-used so there is no real downside.
    static PgConcreteTypeInfo GetDefault(PgSerializerOptions options, PgConcreteTypeInfoProvider concreteTypeInfoProvider, PgTypeId typeId)
        => concreteTypeInfoProvider.GetDefaultInternal(validate: true, options.PortableTypeIds, options.GetCanonicalTypeId(typeId));

    public PgConcreteTypeInfo? GetConcreteTypeInfo<T>(T? value, PgTypeId? expectedPgTypeId)
    {
        if (expectedPgTypeId is { } id && PgTypeId is { } decidedId && id != decidedId)
            ThrowUnexpectedPgTypeId(nameof(expectedPgTypeId));

        return typeInfoProvider is PgConcreteTypeInfoProvider<T> providerT
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

        return typeInfoProvider.GetAsObjectInternal(this, value, expectedPgTypeId ?? PgTypeId);
    }

    public PgConcreteTypeInfo? GetConcreteTypeInfo(Field field)
    {
        if (PgTypeId is { } decidedId && field.PgTypeId != decidedId)
            ThrowUnexpectedPgTypeId(nameof(field));

        return typeInfoProvider.GetInternal(this, field);
    }

    public PgConcreteTypeInfo GetDefaultConcreteTypeInfo(PgTypeId? pgTypeId)
    {
        if (pgTypeId is { } id && PgTypeId is { } decidedId && id != decidedId)
            ThrowUnexpectedPgTypeId(nameof(pgTypeId));

        return typeInfoProvider.GetDefaultInternal(ValidateProviderResults, Options.PortableTypeIds, pgTypeId ?? PgTypeId);
    }

    public PgConcreteTypeInfoProvider GetTypeInfoProvider() => typeInfoProvider;

    static void ThrowUnexpectedPgTypeId(string parameterName)
        => throw new ArgumentException($"PgTypeId does not match the decided value on this {nameof(PgProviderTypeInfo)}.", parameterName);
}

public sealed class PgConcreteTypeInfo(PgSerializerOptions options, PgConverter converter, PgTypeId pgTypeId, Type? unboxedType = null)
    : PgTypeInfo(options, converter, pgTypeId, unboxedType)
{
    public new PgConverter Converter => base.Converter!;
    public new PgTypeId PgTypeId => base.PgTypeId.GetValueOrDefault();
}

readonly struct PgConverterInfo
{
    readonly PgTypeInfo _typeInfo;

    public PgConverterInfo(PgTypeInfo pgTypeInfo, PgConverter converter, Size bufferRequirement)
    {
        _typeInfo = pgTypeInfo;
        Converter = converter;
        BufferRequirement = bufferRequirement;

        // Object typed providers can return any type of converter, so we check the type of the converter instead.
        // We cannot do this in general as we should respect the 'unboxed type' of infos, which can differ from the converter type.
        if (pgTypeInfo is PgProviderTypeInfo && pgTypeInfo.Type == typeof(object))
            TypeToConvert = Converter.TypeToConvert;
        else
            TypeToConvert = pgTypeInfo.Type;
    }

    public bool IsDefault => _typeInfo is null;

    public Type TypeToConvert { get; }

    public PgTypeInfo TypeInfo => _typeInfo;

    public PgConverter Converter { get; }
    public Size BufferRequirement { get; }

    /// Whether Converter.TypeToConvert matches PgTypeInfo.Type, if it doesn't object apis should be used.
    public bool IsBoxingConverter => _typeInfo.IsBoxing;
}
