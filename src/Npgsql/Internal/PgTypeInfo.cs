using System;
using System.Diagnostics.CodeAnalysis;
using Npgsql.Internal.Postgres;

namespace Npgsql.Internal;

public class PgTypeInfo
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
        SupportsWriting = true;
    }

    public PgTypeInfo(PgSerializerOptions options, PgConverter converter, PgTypeId pgTypeId, Type? unboxedType = null)
        : this(options, converter.TypeToConvert, unboxedType)
    {
        Converter = converter;
        PgTypeId = options.GetCanonicalTypeId(pgTypeId);
        _canBinaryConvert = converter.CanConvert(DataFormat.Binary, out _binaryBufferRequirements);
        _canTextConvert = converter.CanConvert(DataFormat.Text, out _textBufferRequirements);
    }

    private protected PgTypeInfo(PgSerializerOptions options, Type type, PgConverterResolution? resolution, Type? unboxedType = null)
        : this(options, type, unboxedType)
    {
        if (resolution is { } res)
        {
            // Resolutions should always be in canonical form already.
            if (options.PortableTypeIds && res.PgTypeId.IsOid || !options.PortableTypeIds && res.PgTypeId.IsDataTypeName)
                throw new ArgumentException("Given type id is not in canonical form. Make sure ConverterResolver implementations close over canonical ids, e.g. by calling options.GetCanonicalTypeId(pgTypeId) on the constructor arguments.", nameof(PgTypeId));

            PgTypeId = res.PgTypeId;
            Converter = res.Converter;
            _canBinaryConvert = res.Converter.CanConvert(DataFormat.Binary, out _binaryBufferRequirements);
            _canTextConvert = res.Converter.CanConvert(DataFormat.Text, out _textBufferRequirements);
        }
    }

    bool HasCachedInfo(PgConverter converter) => ReferenceEquals(Converter, converter);

    public Type Type { get; }
    public PgSerializerOptions Options { get; }

    public bool SupportsWriting { get; init; }
    public DataFormat? PreferredFormat { get; init; }

    // Doubles as the storage for the converter coming from a default resolution (used to confirm whether we can use cached info).
    PgConverter? Converter { get; }
    [MemberNotNullWhen(false, nameof(Converter))]
    [MemberNotNullWhen(false, nameof(PgTypeId))]
    internal bool IsResolverInfo => GetType() == typeof(PgResolverTypeInfo);

    // TODO pull validate from options + internal exempt for perf?
    internal bool ValidateResolution => true;

    // Used for internal converters to save on binary bloat.
    internal bool IsBoxing { get; }

    public PgTypeId? PgTypeId { get; }

    // Having it here so we can easily extend any behavior.
    internal void DisposeWriteState(object writeState)
    {
        if (writeState is IDisposable disposable)
            disposable.Dispose();
    }

    public PgConverterResolution GetResolution<T>(T? value)
    {
        if (this is not PgResolverTypeInfo resolverInfo)
            return new(Converter!, PgTypeId.GetValueOrDefault());

        var resolution = resolverInfo.GetResolution(value, null);
        return resolution ?? resolverInfo.GetDefaultResolution(null);
    }

    // Note: this api is not called GetResolutionAsObject as the semantics are extended, DBNull is a NULL value for all object values.
    public PgConverterResolution GetObjectResolution(object? value)
    {
        switch (this)
        {
        case { IsResolverInfo: false }:
            return new(Converter, PgTypeId.GetValueOrDefault());
        case PgResolverTypeInfo resolverInfo:
            PgConverterResolution? resolution = null;
            if (value is not DBNull)
                resolution = resolverInfo.GetResolutionAsObject(value, null);
            return resolution ?? resolverInfo.GetDefaultResolution(null);
        default:
            return ThrowNotSupported();
        }

        static PgConverterResolution ThrowNotSupported()
            => throw new NotSupportedException("Should not happen, please file a bug.");
    }

    /// Throws if the instance is a PgResolverTypeInfo.
    internal PgConverterResolution GetResolution()
    {
        if (IsResolverInfo)
            ThrowHelper.ThrowInvalidOperationException("Instance is a PgResolverTypeInfo.");
        return new(Converter, PgTypeId.GetValueOrDefault());
    }

    bool CachedCanConvert(DataFormat format, out BufferRequirements bufferRequirements)
    {
        if (format is DataFormat.Binary)
        {
            bufferRequirements = _binaryBufferRequirements;
            return _canBinaryConvert;
        }

        bufferRequirements = _textBufferRequirements;
        return _canTextConvert;
    }

    public BufferRequirements? GetBufferRequirements(PgConverter converter, DataFormat format)
    {
        var success = HasCachedInfo(converter)
            ? CachedCanConvert(format, out var bufferRequirements)
            : converter.CanConvert(format, out bufferRequirements);

        return success ? bufferRequirements : null;
    }

    // TryBind for reading.
    internal bool TryBind(Field field, DataFormat format, out PgConverterInfo info)
    {
        switch (this)
        {
        case { IsResolverInfo: false }:
            if (!CachedCanConvert(format, out var bufferRequirements))
            {
                info = default;
                return false;
            }
            info = new(this, Converter, bufferRequirements.Read);
            return true;
        case PgResolverTypeInfo resolverInfo:
            var resolution = resolverInfo.GetResolution(field);
            if (HasCachedInfo(resolution.Converter)
                    ? !CachedCanConvert(format, out bufferRequirements)
                    : !resolution.Converter.CanConvert(format, out bufferRequirements))
            {
                info = default;
                return false;
            }
            info = new(this, resolution.Converter, bufferRequirements.Read);
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

    // If we don't have a converter stored we must ask the retrieved one.
    DataFormat ResolveFormat(PgConverter converter, out BufferRequirements bufferRequirements, DataFormat? formatPreference = null)
    {
        switch (formatPreference)
        {
        // The common case, no preference means we default to binary if supported.
        case null or DataFormat.Binary when HasCachedInfo(converter) ? CachedCanConvert(DataFormat.Binary, out bufferRequirements) : converter.CanConvert(DataFormat.Binary, out bufferRequirements):
            return DataFormat.Binary;
        // In this case we either prefer text or we have no preference and our converter doesn't support binary.
        case null or DataFormat.Text:
            var canTextConvert = HasCachedInfo(converter) ? CachedCanConvert(DataFormat.Text, out bufferRequirements) : converter.CanConvert(DataFormat.Text, out bufferRequirements);
            if (!canTextConvert)
            {
                if (formatPreference is null)
                    throw new InvalidOperationException("Converter doesn't support any data format.");
                // Rerun without preference.
                return ResolveFormat(converter, out bufferRequirements);
            }
            return DataFormat.Text;
        default:
            throw new ArgumentOutOfRangeException();
        }
    }
}

public sealed class PgResolverTypeInfo : PgTypeInfo
{
    readonly PgConverterResolver _converterResolver;

    public PgResolverTypeInfo(PgSerializerOptions options, PgConverterResolver converterResolver, PgTypeId? pgTypeId, Type? unboxedType = null)
        : base(options,
            converterResolver.TypeToConvert,
            pgTypeId is { } typeId ? ResolveDefaultId(options, converterResolver, typeId) : null,
            // We always mark resolvers with type object as boxing, as they may freely return converters for any type (see PgConverterResolver.Validate).
            unboxedType ?? (converterResolver.TypeToConvert == typeof(object) ? typeof(object) : null))
        => _converterResolver = converterResolver;

    // We'll always validate the default resolution, the info will be re-used so there is no real downside.
    static PgConverterResolution ResolveDefaultId(PgSerializerOptions options, PgConverterResolver converterResolver, PgTypeId typeId)
        => converterResolver.GetDefaultInternal(validate: true, options.PortableTypeIds, options.GetCanonicalTypeId(typeId));

    public PgConverterResolution? GetResolution<T>(T? value, PgTypeId? expectedPgTypeId)
    {
        return _converterResolver is PgConverterResolver<T> resolverT
            ? resolverT.GetInternal(this, value, expectedPgTypeId ?? PgTypeId)
            : ThrowNotSupportedType(typeof(T));

        PgConverterResolution ThrowNotSupportedType(Type? type)
            => throw new NotSupportedException(IsBoxing
                ? "TypeInfo only supports boxing conversions, call GetResolutionAsObject instead."
                : $"TypeInfo is not of type {type}");
    }

    public PgConverterResolution? GetResolutionAsObject(object? value, PgTypeId? expectedPgTypeId)
        => _converterResolver.GetAsObjectInternal(this, value, expectedPgTypeId ?? PgTypeId);

    public PgConverterResolution GetResolution(Field field)
        => _converterResolver.GetInternal(this, field);

    public PgConverterResolution GetDefaultResolution(PgTypeId? expectedPgTypeId)
        => _converterResolver.GetDefaultInternal(ValidateResolution, Options.PortableTypeIds, expectedPgTypeId ?? PgTypeId);

    public PgConverterResolver GetConverterResolver() => _converterResolver;
}

public readonly struct PgConverterResolution
{
    public PgConverterResolution(PgConverter converter, PgTypeId pgTypeId)
    {
        Converter = converter;
        PgTypeId = pgTypeId;
    }

    public PgConverter Converter { get; }
    public PgTypeId PgTypeId { get; }

    public PgConverter<T> GetConverter<T>() => (PgConverter<T>)Converter;
}

readonly struct PgConverterInfo
{
    readonly PgTypeInfo _typeInfo;

    public PgConverterInfo(PgTypeInfo pgTypeInfo, PgConverter converter, Size bufferRequirement)
    {
        _typeInfo = pgTypeInfo;
        Converter = converter;
        BufferRequirement = bufferRequirement;

        // Object typed resolvers can return any type of converter, so we check the type of the converter instead.
        // We cannot do this in general as we should respect the 'unboxed type' of infos, which can differ from the converter type.
        if (pgTypeInfo.IsResolverInfo && pgTypeInfo.Type == typeof(object))
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
