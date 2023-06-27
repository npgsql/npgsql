using System;
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

    public PgTypeInfo(PgSerializerOptions options, PgConverter converter, PgTypeId pgTypeId, Type? unboxedType = null)
        : this(options, converter.TypeToConvert, unboxedType)
    {
        Converter = converter;
        PgTypeId = options.GetCanonicalTypeId(pgTypeId);
        _canBinaryConvert = converter.CanConvert(DataFormat.Binary, out _binaryBufferingRequirement);
        _canTextConvert = converter.CanConvert(DataFormat.Text, out _textBufferingRequirement);
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
            _canBinaryConvert = res.Converter.CanConvert(DataFormat.Binary, out _binaryBufferingRequirement);
            _canTextConvert = res.Converter.CanConvert(DataFormat.Text, out _textBufferingRequirement);
        }
    }

    bool HasCachedInfo(PgConverter converter) => ReferenceEquals(Converter, converter);

    public Type Type { get; }
    public PgSerializerOptions Options { get; }

    public bool SupportsWriting { get; init; }
    public DataFormat? PreferredFormat { get; init; }

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

    // Bind for reading.
    internal PgConverterInfo Bind(Field field, DataFormat format)
    {
        switch (this)
        {
        case { IsResolverInfo: false }:
            // Type lies when IsBoxing is true.
            var typeToConvert = IsBoxing ? typeof(object) : Type;
            return ValidateFormat(isRead: true, format, Converter, typeToConvert);
        case PgTypeResolverInfo resolverInfo:
            var resolution = resolverInfo.GetResolution(field);
            return ValidateFormat(isRead: true, format, resolution.Converter, resolution.Converter.TypeToConvert);
        default:
            throw new NotSupportedException("Should not happen, please file a bug.");
        }
    }

    public PgConverterResolution GetResolution<T>(T? value, PgTypeId? expectedPgTypeId = null)
        => this is PgTypeResolverInfo resolverInfo
            ? resolverInfo.GetResolution(value, expectedPgTypeId)
            : GetResolutionAsObject(null); // Other cases, to keep binary bloat minimal.

    public PgConverterResolution GetResolutionAsObject(object? value, PgTypeId? expectedPgTypeId = null)
    {
        switch (this)
        {
        case { IsResolverInfo: false }:
            return new(Converter, PgTypeId.GetValueOrDefault());
        case PgTypeResolverInfo resolverInfo:
            return resolverInfo.GetResolutionAsObject(value, expectedPgTypeId);
        default:
            throw new NotSupportedException("Should not happen, please file a bug.");
        }
    }

    // Get the default resolution, if there is no PgTypeId we throw.
    internal PgConverterResolution GetResolutionOrThrow()
        => this switch
        {
            { IsResolverInfo: false } => new(Converter, PgTypeId.GetValueOrDefault()),
            PgTypeResolverInfo resolverInfo => resolverInfo.GetDefaultResolutionOrThrow(),
            _ => throw new NotSupportedException("Should not happen, please file a bug.")
        };

    PgConverterInfo ValidateFormat(bool isRead, DataFormat format, PgConverter converter, Type typeToConvert)
    {
        if (HasCachedInfo(converter) ? !CachedCanConvert(format, out var bufferingRequirement) : !converter.CanConvert(format, out bufferingRequirement))
            throw new InvalidOperationException($"Converter {converter.GetType()} does not support {format} format.");

        return new()
        {
            Converter = converter,
            AsObject = Type != typeToConvert,
            BufferRequirement = GetRequirement(isRead, format),
        };

        Size GetRequirement(bool isRead, DataFormat format)
        {
            var (readRequirement, writeRequirement) = bufferingRequirement.ToBufferRequirements(format, converter);
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

    // Bind for writing.
    /// When result is null, the value was interpreted to be a SQL NULL.
    public PgConverterInfo? Bind<T>(PgConverterResolution resolution, T? value, out object? writeState, out DataFormat format, DataFormat? formatPreference = null)
    {
        // Basically exists to catch cases like object[] resolving a polymorphic read converter, better to fail during binding than writing.
        if (!SupportsWriting)
            throw new NotSupportedException($"Writing {Type} is not supported for this type info.");

        var converter = resolution.GetConverter<T>();
        format = ResolveFormat(converter, out _, formatPreference ?? PreferredFormat);
        if (converter.IsDbNullValue(value))
        {
            writeState = null;
            return null;
        }
        writeState = null;
        var context = new SizeContext(format);
        var size = converter.GetSize(context, value, ref writeState);
        return new()
        {
            Converter = converter,
            AsObject = Type != typeof(T),
            BufferRequirement = size,
        };
    }

    // Bind for writing.
    // Note: this api is not called BindAsObject as the semantics are extended, DBNull is a NULL value for all object values.
    /// When result is null, the value was interpreted to be a SQL NULL.
    public PgConverterInfo? BindObject(PgConverterResolution resolution, object? value, out object? writeState, out DataFormat format, DataFormat? formatPreference = null)
    {
        // Basically exists to catch cases like object[] resolving a polymorphic read converter, better to fail during binding than writing.
        if (!SupportsWriting)
            throw new NotSupportedException($"Writing {Type} is not supported for this type info.");

        var converter = resolution.Converter;
        format = ResolveFormat(converter, out _, formatPreference ?? PreferredFormat);

        // Given SQL values are effectively a union of T | NULL we support DBNull.Value to signify a NULL value for all types in this api.
        if (value is DBNull || converter.IsDbNullValueAsObject(value))
        {
            writeState = null;
            return null;
        }
        writeState = null;
        var context = new SizeContext(format);
        var size = converter.GetSizeAsObject(context, value, ref writeState);
        return new()
        {
            Converter = converter,
            AsObject = !IsBoxing && Type != typeof(object),
            BufferRequirement = size,
        };
    }

    internal PgTypeInfo ToObjectTypeInfo()
    {
        if (IsResolverInfo)
        {
            // TODO should have a CastingConverterResolver.
            throw new NotImplementedException();
        }

        return new(Options, new CastingConverter<object>(Converter), PgTypeId.GetValueOrDefault())
        {
            PreferredFormat = PreferredFormat
        };
    }

    internal PgTypeInfo ToComposedTypeInfo(PgConverter converter, PgTypeId pgTypeId, Type? unboxedType = null)
    {
        if (IsResolverInfo && PgTypeId is null)
            throw new InvalidOperationException("Cannot compose a normal converter info on top of an undecided resolver based type info.");

        return new(Options, converter, pgTypeId, unboxedType)
        {
            PreferredFormat = PreferredFormat
        };
    }

    internal PgTypeInfo ToComposedTypeInfo(PgConverterResolver resolver, PgTypeId? expectedPgTypeId, Type? unboxedType = null)
        => new PgTypeResolverInfo(Options, resolver, expectedPgTypeId, unboxedType)
        {
            PreferredFormat = PreferredFormat,
        };
    //
    // public static PgTypeInfo Create(PgSerializerOptions options, PgConverter converter, PgTypeId pgTypeId, DataFormat? preferredFormat = null, Type? unboxedType = null)
    //     => new(options, converter, pgTypeId, unboxedType) { PreferredFormat = preferredFormat };
    //
    // public static PgTypeResolverInfo Create(PgSerializerOptions options, PgConverterResolver resolver, PgTypeId? expectedPgTypeId = null, DataFormat? preferredFormat = null, Type? unboxedType = null)
    //     => new(options, resolver, expectedPgTypeId, unboxedType) { PreferredFormat = preferredFormat };

    // If we don't have a converter stored we must ask the retrieved one through virtual calls.
    DataFormat ResolveFormat(PgConverter converter, out BufferingRequirement bufferingRequirement, DataFormat? formatPreference = null)
    {
        switch (formatPreference)
        {
        // The common case, no preference means we default to binary if supported.
        case null or DataFormat.Binary when HasCachedInfo(converter) ? CachedCanConvert(DataFormat.Binary, out bufferingRequirement) : converter.CanConvert(DataFormat.Binary, out bufferingRequirement):
            return DataFormat.Binary;
        // In this case we either prefer text or we have no preference and our converter doesn't support binary.
        case null or DataFormat.Text:
            var canTextConvert = HasCachedInfo(converter) ? CachedCanConvert(DataFormat.Text, out bufferingRequirement) : converter.CanConvert(DataFormat.Text, out bufferingRequirement);
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
    readonly PgConverterResolver _converterResolver;

    public PgTypeResolverInfo(PgSerializerOptions options, PgConverterResolver converterResolver, PgTypeId? pgTypeId, Type? unboxedType = null)
        : base(options,
            converterResolver.TypeToConvert,
            // We'll always validate the default resolution, the info will be re-used so there is no real downside.
            pgTypeId is { } typeId ? converterResolver.GetDefaultInternal(validate: true, options.PortableTypeIds, options.GetCanonicalTypeId(typeId)) : null,
            unboxedType)
        => _converterResolver = converterResolver;

    public new PgConverterResolution GetResolution<T>(T? value, PgTypeId? expectedPgTypeId)
    {
        return _converterResolver is PgConverterResolver<T> resolverT
            ? resolverT.GetInternal(this, value, expectedPgTypeId ?? PgTypeId)
            : ThrowNotSupportedType(typeof(T));

        PgConverterResolution ThrowNotSupportedType(Type? type)
            => throw new NotSupportedException(IsBoxing
                ? $"TypeInfo only supports boxing conversions, call GetResolution<T> with {typeof(object)} instead of {type} or call GetResolutionAsObject instead."
                : $"TypeInfo is not of type {type}");
    }

    public new PgConverterResolution GetResolutionAsObject(object? value, PgTypeId? expectedPgTypeId)
        => _converterResolver.GetAsObjectInternal(this, value, expectedPgTypeId ?? PgTypeId);

    public PgConverterResolution GetResolution(Field field)
        => _converterResolver.GetInternal(this, field);

    public PgConverterResolution GetDefaultResolution(PgTypeId pgTypeId)
        => _converterResolver.GetDefaultInternal(ValidateResolution, Options.PortableTypeIds, pgTypeId);

    public PgConverterResolution GetDefaultResolutionOrThrow()
        => GetDefaultResolution(PgTypeId ?? throw new InvalidOperationException("Cannot get default resolution for undecided type info."));
}

public readonly struct PgConverterInfo
{
    public required PgConverter Converter { get; init; }
    public required Size BufferRequirement { get; init; }
    // Whether Converter.TypeToConvert matches the PgTypeInfo.Type, if it doesn't object apis and a downcast should be used.
    public required bool AsObject { get; init; }

    public PgConverter<T> GetConverter<T>() => (PgConverter<T>)Converter;
}
