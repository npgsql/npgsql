using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal.Postgres;
using Npgsql.Util;

namespace Npgsql.Internal;

[Experimental(NpgsqlDiagnostics.ConvertersExperimental)]
public abstract class PgTypeInfo
{
    PgTypeInfo(PgSerializerOptions options, Type type, Type? requestedType)
    {
        Options = options;

        HasExactType = requestedType is null || requestedType == type;
        Type = requestedType is null ? type : GetReportedType(type, requestedType) ?? type;

        SupportsReading = GetDefaultSupportsReading(type, requestedType);
        SupportsWriting = true;
    }

    private protected PgTypeInfo(PgSerializerOptions options, Type type, PgTypeId? pgTypeId, Type? requestedType = null)
        : this(options, type, requestedType)
        => PgTypeId = pgTypeId is { } id ? options.GetCanonicalTypeId(id) : null;

    private protected PgTypeInfo(PgSerializerOptions options, PgConverter converter, PgTypeId pgTypeId, Type? requestedType = null)
        : this(options, converter.TypeToConvert, pgTypeId, requestedType) {}

    public Type Type { get; }
    public PgSerializerOptions Options { get; }

    public bool SupportsReading { get; init; }
    public bool SupportsWriting { get; init; }
    public DataFormat? PreferredFormat { get; init; }

    // True when the reported type matches the converter's type exactly (no reported type given at construction, or
    // the given reported type equals the converter type). When false, the reported type is a widening of the converter
    // type (e.g. Array/Stream base-type reporting, enum-underlying widening) and the caller must dispatch through the
    // info — the info routes reference-variance cases through the object APIs and layout-identity cases (enum) through
    // the typed path with Unsafe.As, as appropriate for the widening kind.
    // Having a single converter cover multiple reported types (Arrays, Streams) reduces the number of generic
    // instantiations that need to be compiled for AOT.
    internal bool HasExactType { get; }

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

        // Make sure we handle the weakly typed provider case.
        // This will never cause boxing as weakly typed infos only happen for subtype relationships, i.e. reference types.
        // We make sure to fall through to ProvideConcreteTypeInfo which has a better error if T is not at all related to this info.
        var concreteTypeInfo = PgProviderTypeInfo.GetProvider(providerTypeInfo) is not PgConcreteTypeInfoProvider<T> && providerTypeInfo.Type == typeof(T)
            ? providerTypeInfo.GetAsObjectConcreteTypeInfo(default, (object?)value, out writeState)
            : providerTypeInfo.GetConcreteTypeInfo(default, value, out writeState);

        return concreteTypeInfo ?? providerTypeInfo.GetDefaultConcreteTypeInfo(null);
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

    // We assume an info without an exact type does not support reading as the converter won't be able to produce the derived type statically.
    // Cases like Array converters reading int[], int[,] etc. are the exception and the reason why SupportsReading is a settable property.
    internal static bool GetDefaultSupportsReading(Type type, Type? requestedType)
        => requestedType is null || GetReportedType(type, requestedType) is not { } reportedType || reportedType == type;

    static Type? GetReportedType(Type converterType, Type requestedType)
    {
        if (!requestedType.IsInSubtypeRelationshipWith(converterType))
            throw new ArgumentException($"The requested type {requestedType} is not in a subtype relationship with the converter's type {converterType}.", nameof(requestedType));

        return requestedType != converterType && requestedType.IsAssignableTo(converterType) ? requestedType : null;
    }
}

public sealed class PgProviderTypeInfo : PgTypeInfo
{
    readonly PgConcreteTypeInfoProvider _typeInfoProvider;
    readonly PgConcreteTypeInfo? _defaultConcrete;

    public PgProviderTypeInfo(PgSerializerOptions options, PgConcreteTypeInfoProvider typeInfoProvider, PgTypeId? pgTypeId)
        : this(options, typeInfoProvider, pgTypeId, requestedType: null)
    {}

    internal PgProviderTypeInfo(PgSerializerOptions options, PgConcreteTypeInfoProvider typeInfoProvider, PgTypeId? pgTypeId, Type? requestedType)
        : base(options, typeInfoProvider.TypeToConvert, pgTypeId, requestedType)
    {
        _typeInfoProvider = typeInfoProvider;

        // Always validate the default provider result, the info will be re-used so there is no real downside.
        var result = typeInfoProvider.GetDefault(pgTypeId is { } id ? options.GetCanonicalTypeId(id) : null);
        ValidateResult(nameof(PgConcreteTypeInfoProvider.GetDefault), result, typeInfoProvider.TypeToConvert, options.PortableTypeIds);
        _defaultConcrete = result;
    }

    public PgConcreteTypeInfo GetDefaultConcreteTypeInfo(PgTypeId? pgTypeId)
    {
        if (pgTypeId is { } id && PgTypeId is { } decidedId)
        {
            if (id != decidedId)
                ThrowUnexpectedPgTypeId(nameof(pgTypeId));

            Debug.Assert(_defaultConcrete is not null);
            return _defaultConcrete;
        }

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
            => throw new NotSupportedException(type == Type
                ? $"PgProviderTypeInfo does not exactly match type {type}, call {nameof(GetAsObjectConcreteTypeInfo)} instead."
                : $"PgProviderTypeInfo is incompatible with type {type}");
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
        : base(options, converter, pgTypeId, requestedType)
    {
        Converter = converter;
        _canBinaryConvert = converter.CanConvert(DataFormat.Binary, out _binaryBufferRequirements);
        _canTextConvert = converter.CanConvert(DataFormat.Text, out _textBufferRequirements);
    }

    Type TypeToConvert
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Converter.TypeToConvert;
    }

    public PgConverter Converter { get; }
    public new PgTypeId PgTypeId => base.PgTypeId.GetValueOrDefault();

    internal bool CanReadTo(Type type) => Type == type || (!HasExactType && Type.IsAssignableTo(type));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal T ReadFieldValue<T>(PgReader reader, PgFieldBinding binding)
    {
        reader.StartRead(binding);
        var result = Converter.Read<T>(reader);
        reader.EndRead();
        return result;
    }

    internal async ValueTask<T> ReadFieldValueAsync<T>(PgReader reader, PgFieldBinding binding, CancellationToken cancellationToken)
    {
        await reader.StartReadAsync(binding, cancellationToken).ConfigureAwait(false);

        // Inline copy of Converter.ReadAsync<T> to keep everything in one async frame.
        var result = typeof(T) != TypeToConvert
            ? (T)await Converter.ReadAsObjectAsync(reader, cancellationToken).ConfigureAwait(false)
            : await Unsafe.As<PgConverter<T>>(Converter).ReadAsync(reader, cancellationToken).ConfigureAwait(false);

        await reader.EndReadAsync().ConfigureAwait(false);
        return result;
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
        if (!CanConvert(format, out var bufferRequirements))
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
        if (typeof(T) != TypeToConvert)
            return BindParameterObjectValue(value, writeState, formatPreference);

        // Basically exists to catch cases like object[] resolving a polymorphic read converter, better to fail during binding than writing.
        if (!SupportsWriting)
            ThrowHelper.ThrowNotSupportedException($"Writing {Type} is not supported for this type info.");

        var format = ResolveFormat(out var bufferRequirements, formatPreference ?? PreferredFormat);

        Debug.Assert(Converter is PgConverter<T>);
        if (Unsafe.As<PgConverter<T>>(Converter).GetSizeOrDbNull(format, bufferRequirements.Write, value, ref writeState) is not { } size)
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

        var format = ResolveFormat(out var bufferRequirements, formatPreference ?? PreferredFormat);

        // Given SQL values are effectively a union of T | NULL we support DBNull.Value to signify a NULL value for all types except DBNull in this api.
        if (value is DBNull && Type != typeof(DBNull) || Converter.GetSizeOrDbNullAsObject(format, bufferRequirements.Write, value, ref writeState) is not { } size)
        {
            return new(format, bufferRequirements.Write, null, null);
        }

        return new(format, bufferRequirements.Write, size, writeState);
    }

    public bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
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

        return Converter.CanConvert(format, out bufferRequirements);
    }

    DataFormat ResolveFormat(out BufferRequirements bufferRequirements, DataFormat? formatPreference = null)
    {
        // First try to check for preferred support.
        switch (formatPreference)
        {
        case DataFormat.Binary when CanConvert(DataFormat.Binary, out bufferRequirements):
            return DataFormat.Binary;
        case DataFormat.Text when CanConvert(DataFormat.Text, out bufferRequirements):
            return DataFormat.Text;
        default:
            // The common case, no preference given (or no match) means we default to binary if supported.
            if (CanConvert(DataFormat.Binary, out bufferRequirements))
                return DataFormat.Binary;
            if (CanConvert(DataFormat.Text, out bufferRequirements))
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
