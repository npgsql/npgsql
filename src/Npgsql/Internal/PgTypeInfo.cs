using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Npgsql.Internal.Converters;
using Npgsql.Internal.Descriptors;
using Npgsql.PostgresTypes;

namespace Npgsql.Internal;

public class PgTypeInfo
{
    readonly bool _canBinaryConvert;
    readonly BufferingRequirement _binaryBufferingRequirement;

    readonly bool _canTextConvert;
    readonly BufferingRequirement _textBufferingRequirement;

    PgTypeInfo(PgSerializerOptions options, Type type, Type? unboxedType)
    {
        if (unboxedType is not null && type != typeof(object))
            throw new ArgumentException("Cannot supply unboxed type information for converters that don't convert to object.", nameof(unboxedType));

        Options = options;
        IsBoxing = unboxedType is not null;
        Type = unboxedType ?? type;
    }

    PgTypeInfo(PgSerializerOptions options, PgConverter converter, PgTypeId pgTypeId, Type? unboxedType = null)
        : this(options, converter.TypeToConvert, unboxedType)
    {
        Converter = converter;
        PgTypeId = options.GetCanonicalTypeId(pgTypeId);
        _canBinaryConvert = converter.CanConvert(DataFormat.Binary, out _binaryBufferingRequirement, out _);
        _canTextConvert = converter.CanConvert(DataFormat.Text, out _textBufferingRequirement, out _);
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
            _canBinaryConvert = res.Converter.CanConvert(DataFormat.Binary, out _binaryBufferingRequirement, out _);
            _canTextConvert = res.Converter.CanConvert(DataFormat.Text, out _textBufferingRequirement, out _);
        }
    }

    bool HasCachedInfo => PgTypeId is not null;

    public Type Type { get; }
    public PgSerializerOptions Options { get; }

    // Whether this TypeInfo maps to the default CLR Type for the DataTypeName given to IPgTypeInfoResolver.GetTypeInfo.
    public bool IsDefault { get; private set; }
    public DataFormat? PreferredFormat { get; private set; }

    PgConverter? Converter { get; }
    [MemberNotNullWhen(false, nameof(Converter))]
    bool IsResolverInfo => Converter is null;

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

    // Bind for writing, optimized for code bloat.
    internal PgConverterInfo Bind<T>(DataFormat format, T? value, PgTypeId? expectedPgTypeId = null)
    {
        if (this is not PgTypeResolverInfo resolverInfo)
            return BindAsObject(format, null, expectedPgTypeId);

        var resolution = resolverInfo.GetResolution(value, expectedPgTypeId);
        return ValidateFormat(isRead: false, format, resolution.Converter, resolution.PgTypeId, resolution.Converter.TypeToConvert);
    }

    // Bind for writing.
    internal PgConverterInfo BindAsObject(DataFormat format, object? value, PgTypeId? expectedPgTypeId = null)
    {
        switch (this)
        {
        case { Converter: { } converter }:
            // Type lies when IsBoxing is true.
            var typeToConvert = IsBoxing ? typeof(object) : Type;
            return ValidateFormat(isRead: false, format, converter, PgTypeId.GetValueOrDefault(), typeToConvert);
        case PgTypeResolverInfo resolverInfo:
            var resolution = resolverInfo.GetResolutionAsObject(value, expectedPgTypeId);
            return ValidateFormat(isRead: false, format, resolution.Converter, resolution.PgTypeId, resolution.Converter.TypeToConvert);
        default:
            throw new NotSupportedException("Should not happen, please file a bug.");
        }
    }

    // Bind for reading.
    internal PgConverterInfo Bind(Field field, DataFormat format)
    {
        switch (this)
        {
        case { Converter: { } converter }:
            // Type lies when IsBoxing is true.
            var typeToConvert = IsBoxing ? typeof(object) : Type;
            return ValidateFormat(isRead: true, format, converter, PgTypeId.GetValueOrDefault(), typeToConvert);
        case PgTypeResolverInfo resolverInfo:
            var resolution = resolverInfo.GetResolution(field);
            return ValidateFormat(isRead: true, format, resolution.Converter, resolution.PgTypeId, resolution.Converter.TypeToConvert);
        default:
            throw new NotSupportedException("Should not happen, please file a bug.");
        }
    }

    // Get the default resolution, if there is no PgTypeId we throw.
    internal PgConverterResolution GetResolutionOrThrow()
    {
        return this switch
        {
            { Converter: { } converter } => new(converter, PgTypeId.GetValueOrDefault()),
            PgTypeResolverInfo resolverInfo => resolverInfo.GetDefaultResolutionOrThrow(),
            _ => throw new NotSupportedException("Should not happen, please file a bug.")
        };
    }

    PgConverterInfo ValidateFormat(bool isRead, DataFormat format, PgConverter converter, PgTypeId pgTypeId, Type typeToConvert)
    {
        if (HasCachedInfo ? !CachedCanConvert(format, out var bufferingRequirement) : !converter.CanConvert(format, out bufferingRequirement, out _))
            throw new InvalidOperationException($"Converter {converter.GetType()} does not support {format} format.");

        return new()
        {
            Converter = converter,
            AsObject = Type != typeToConvert,
            BufferRequirement = GetRequirement(isRead),
            PgTypeId = pgTypeId,
        };

        Size GetRequirement(bool isRead)
        {
            var (readRequirement, writeRequirement) = bufferingRequirement.ToBufferRequirements(converter);
            return isRead ? readRequirement : writeRequirement;
        }
    }

    bool CachedCanConvert(DataFormat format, out BufferingRequirement bufferingRequirement)
    {
        if (format is DataFormat.Binary)
        {
            bufferingRequirement = _binaryBufferingRequirement;
            return _canBinaryConvert;
        }

        bufferingRequirement = _textBufferingRequirement;
        return _canTextConvert;
    }

    /// When result is null, the value was interpreted to be a SQL NULL.
    public Size? GetSize<T>(PgConverterInfo info, T? value, out object? writeState, out DataFormat format, DataFormat? formatPreference = null)
    {
        format = ResolveFormat(info.Converter, out _, formatPreference ?? PreferredFormat);
        var context = new SizeContext(format);
        var converter = info.GetConverter<T>();
        if (converter.IsDbNullValue(value))
        {
            writeState = null;
            return null;
        }
        writeState = null;
        var size = converter.GetSize(context, value, ref writeState);
        return size;
    }

    // Note: this api is not called GetSizeAsObject as the semantics are extended, DBNull is a NULL value for all object values.
    /// When result is null, the value was interpreted to be a SQL NULL.
    public Size? GetObjectSize(PgConverterInfo info, object? value, out object? writeState, out DataFormat format, DataFormat? formatPreference = null)
    {
        format = ResolveFormat(info.Converter, out _, formatPreference ?? PreferredFormat);
        var context = new SizeContext(format);

        // Given SQL values are effectively a union of T | NULL we support DBNull.Value to signify a NULL value for all types in this api.
        if (value is DBNull || info.Converter.IsDbNullValueAsObject(value))
        {
            writeState = null;
            return null;
        }
        writeState = null;
        var size = info.Converter.GetSizeAsObject(context, value, ref writeState);
        return size;
    }

    internal PgTypeInfo ToObjectConverterInfo(bool? isDefault = null)
    {
        if (IsResolverInfo)
        {
            // TODO should have a CastingConverterResolver.
            throw new NotImplementedException();
        }

        return new(Options, new CastingConverter<object>(Converter), PgTypeId.GetValueOrDefault())
        {
            IsDefault = isDefault ?? IsDefault,
            PreferredFormat = PreferredFormat
        };
    }

    internal PgTypeInfo ToComposedConverterInfo(PgConverter converter, PgTypeId pgTypeId, Type? unboxedType = null, bool? isDefault = null)
    {
        if (IsResolverInfo)
            throw new InvalidOperationException("Cannot compose a normal converter info on top of a resolver based converter info.");

        return new(Options, converter, pgTypeId, unboxedType)
        {
            IsDefault = isDefault ?? IsDefault,
            PreferredFormat = PreferredFormat
        };
    }

    internal PgTypeInfo ToComposedConverterInfo(PgConverterResolver resolver, PgTypeId? expectedPgTypeId, Type? unboxedType = null, bool? isDefault = null)
        => new PgTypeResolverInfo(Options, resolver, expectedPgTypeId, unboxedType)
        {
            IsDefault = isDefault ?? IsDefault,
            PreferredFormat = PreferredFormat,
        };

    public static PgTypeInfo Create(PgSerializerOptions options, PgConverter converter, PgTypeId pgTypeId, DataFormat? preferredFormat = null)
        => new(options, converter, pgTypeId) { PreferredFormat = preferredFormat };

    public static PgTypeInfo CreateDefault(PgSerializerOptions options, PgConverter converter, PgTypeId pgTypeId, DataFormat? preferredFormat = null)
        => new(options, converter, pgTypeId) { IsDefault = true, PreferredFormat = preferredFormat };

    internal static PgTypeInfo CreateBoxing(PgSerializerOptions options, Type unboxedType, PgConverter converter, PgTypeId pgTypeId, bool isDefault = false, DataFormat? preferredFormat = null)
        => new(options, converter, pgTypeId, unboxedType) { IsDefault = isDefault, PreferredFormat = preferredFormat };

    internal static PgTypeInfo CreateBoxing(PgSerializerOptions options, Type unboxedType, PgConverterResolver resolver, PgTypeId? expectedPgTypeId, bool isDefault = false, DataFormat? preferredFormat = null)
        => new PgTypeResolverInfo(options, resolver, expectedPgTypeId, unboxedType) { IsDefault = isDefault, PreferredFormat = preferredFormat };

    // It should not be possible to create a resolver based info that has IsDefault false without a PgTypeId.
    public static PgTypeInfo Create(PgSerializerOptions options, PgConverterResolver resolver, PgTypeId expectedPgTypeId, DataFormat? preferredFormat = null)
        => new PgTypeResolverInfo(options, resolver, expectedPgTypeId) { PreferredFormat = preferredFormat };

    public static PgTypeInfo CreateDefault(PgSerializerOptions options, PgConverterResolver resolver, PgTypeId? expectedPgTypeId = null, DataFormat? preferredFormat = null)
        => new PgTypeResolverInfo(options, resolver, expectedPgTypeId) { IsDefault = true, PreferredFormat = preferredFormat };

    // If we don't have a converter stored we must ask the retrieved one through virtual calls.
    DataFormat ResolveFormat(PgConverter converter, out BufferingRequirement bufferingRequirement, DataFormat? formatPreference = null)
    {
        switch (formatPreference)
        {
        // The common case, no preference means we default to binary if supported.
        case null or DataFormat.Binary when HasCachedInfo ? CachedCanConvert(DataFormat.Binary, out bufferingRequirement) : converter.CanConvert(DataFormat.Binary, out bufferingRequirement, out _):
            return DataFormat.Binary;
        // In this case we either prefer text or we have no preference and our converter doesn't support binary.
        case null or DataFormat.Text:
            var canTextConvert = HasCachedInfo ? CachedCanConvert(DataFormat.Text, out bufferingRequirement) : converter.CanConvert(DataFormat.Text, out bufferingRequirement, out _);
            if (!canTextConvert)
                throw new InvalidOperationException("Converter doesn't support any data format.");
            return DataFormat.Text;
        default:
            throw new ArgumentOutOfRangeException();
        }
    }
}

public sealed class PgTypeResolverInfo : PgTypeInfo
{
    internal PgTypeResolverInfo(PgSerializerOptions options, PgConverterResolver converterResolver, PgTypeId? pgTypeId, Type? unboxedType = null)
        : base(options,
            converterResolver.TypeToConvert,
            // We'll always validate the default resolution, the info will be re-used so there is no real downside.
            pgTypeId is { } typeId ? converterResolver.GetDefaultInternal(validate: true, options.PortableTypeIds, typeId) : null,
            unboxedType)
    {
        ConverterResolver = converterResolver;
        // It should not be possible to create a resolver based info that has IsDefault false without a PgTypeId.
        Debug.Assert(PgTypeId is not null || IsDefault);
    }

    internal PgConverterResolver ConverterResolver { get; }

    public PgConverterResolution GetResolution<T>(T? value, PgTypeId? expectedPgTypeId)
    {
        return ConverterResolver is PgConverterResolver<T> resolverT
            ? resolverT.GetInternal(this, value, expectedPgTypeId)
            : ThrowNotSupportedType(typeof(T));

        PgConverterResolution ThrowNotSupportedType(Type? type)
            => throw new NotSupportedException(IsBoxing
                ? $"TypeInfo only supports boxing conversions, call GetResolution<T> with {typeof(object)} instead of {type} or call GetResolutionAsObject instead."
                : $"TypeInfo is not of type {type}");
    }

    public PgConverterResolution GetResolutionAsObject(object? value, PgTypeId? expectedPgTypeId)
        => ConverterResolver.GetAsObjectInternal(this, value, expectedPgTypeId);

    public PgConverterResolution GetResolution(Field field)
        => ConverterResolver.GetInternal(this, field);

    public PgConverterResolution GetDefaultResolutionOrThrow()
        => ConverterResolver.GetDefaultInternal(ValidateResolution, Options.PortableTypeIds, PgTypeId ?? throw new InvalidOperationException("Cannot get default resolution for undecided type info."));
}

public readonly struct PgConverterInfo
{
    public required PgConverter Converter { get; init; }
    public required Size BufferRequirement { get; init; }
    // Whether Converter.TypeToConvert matches the PgTypeInfo.Type, if it doesn't object apis and a downcast should be used.
    public required bool AsObject { get; init; }
    public required PgTypeId PgTypeId { get; init; }

    public PgConverter<T> GetConverter<T>() => (PgConverter<T>)Converter;
}
