using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal.Postgres;

namespace Npgsql.Internal;

[Experimental(NpgsqlDiagnostics.ConvertersExperimental)]
public abstract class PgTypeInfo
{
    PgTypeInfo(PgSerializerOptions options, Type type, Type? requestedType)
    {
        Options = options;
        HasExactType = requestedType is null || requestedType == type;
        Type = ResolveType(type, requestedType);
    }

    /// <summary>
    /// Resolves the type the info should advertise. <paramref name="requestedType"/> must be on the same subtype
    /// chain as <paramref name="type"/>: when narrower (or equal) it is returned as-is (the under-reporting case);
    /// when wider <paramref name="type"/> is returned (the polymorphic-alias case, the info advertises the
    /// converter's type back to a wider query). Throws when the two types are not in any subtype relationship.
    /// </summary>
    private protected static Type ResolveType(Type type, Type? requestedType)
    {
        if (requestedType is null || requestedType == type)
            return type;
        if (requestedType.IsAssignableTo(type))
            return requestedType;
        if (type.IsAssignableTo(requestedType))
            return type;
        throw new ArgumentException(
            $"The requested type {requestedType} is not in a subtype relationship with the converter's type {type}.",
            nameof(requestedType));
    }

    private protected PgTypeInfo(PgSerializerOptions options, Type type, PgTypeId? pgTypeId, Type? requestedType = null)
        : this(options, type, requestedType)
        => PgTypeId = pgTypeId is { } id ? options.GetCanonicalTypeId(id) : null;

    private protected PgTypeInfo(PgSerializerOptions options, PgConverter converter, PgTypeId pgTypeId, Type? requestedType = null)
        : this(options, converter.TypeToConvert, pgTypeId, requestedType) {}

    public Type Type { get; }
    public PgSerializerOptions Options { get; }

    // Whether the eventual converter's type is exactly Type. False when what flows out can vary along the shared
    // subtype chain (under-report, polymorphic alias).
    //   - Concrete: false when (Converter.TypeToConvert, Type) differ.
    //   - Provider: false when the underlying provider is polymorphic and dispatches to varied concretes per call,
    //     even though the wrapping pair (Provider.TypeToConvert, Type) is invariant.
    internal bool HasExactType { get; private protected init; }

    public PgTypeId? PgTypeId { get; }

    // Shared validation. Throws if info doesn't belong to options or its Type isn't compatible with expectedType
    // (when allowSubtypes is true, subtypes are accepted). Plugins ship in-tree, so this fires in release. Hot success
    // path is the inlinable predicate. The cold throw path is factored out so callers only pay for the predicate when
    // valid.
    internal static void ValidateInfo(string contextName, PgTypeInfo info, PgSerializerOptions options, Type? expectedType, bool allowSubtypes)
    {
        if (info.Options != options || !IsCompatibleResolution(info.Type, expectedType, allowSubtypes))
            Throw(contextName, info, options, expectedType, allowSubtypes);

        // Strict equality when allowSubtypes is false. Otherwise also admits subtypes of expectedType (covers polymorphic-alias
        // and under-reporting). Null expectedType means "any", used by callers that care only about ownership.
        static bool IsCompatibleResolution(Type? resolvedType, Type? expectedType, bool allowSubtypes)
            => expectedType is null
                || resolvedType is not null && (resolvedType == expectedType || (allowSubtypes && resolvedType.IsAssignableTo(expectedType)));

        static void Throw(string contextName, PgTypeInfo info, PgSerializerOptions options, Type? expectedType, bool allowSubtypes)
        {
            if (info.Options != options)
                throw new InvalidOperationException($"'{contextName}' returned a {nameof(PgTypeInfo)} from a different {nameof(PgSerializerOptions)} instance.");

            if (!IsCompatibleResolution(info.Type, expectedType, allowSubtypes))
                throw new InvalidOperationException($"'{contextName}' returned a {nameof(PgTypeInfo)} advertising type {info.Type} which is incompatible with expected type {expectedType}.");
        }
    }

    /// <summary>
    /// Makes a <see cref="PgConcreteTypeInfo"/> for the given field.
    /// </summary>
    /// <param name="field">The field whose metadata drives the concrete type info selection.</param>
    /// <returns>The <see cref="PgConcreteTypeInfo"/> to use for the field.</returns>
    /// <remarks>
    /// When this instance is already concrete it is returned directly; otherwise the underlying provider is consulted
    /// using the field's metadata (e.g. <see cref="Field.PgTypeId"/>) to select the appropriate concrete type info.
    /// </remarks>
    public PgConcreteTypeInfo MakeConcreteForField(Field field)
    {
        if (this is PgConcreteTypeInfo concrete)
            return concrete;

        // Decided providers skip GetDefault's validation. The prior GetForField call already validated
        // the id. Undecided providers thread it so GetDefaultCore can dispatch on it.
        var providerTypeInfo = (PgProviderTypeInfo)this;
        return providerTypeInfo.GetForField(field)
               ?? providerTypeInfo.GetDefault(providerTypeInfo.PgTypeId is null ? field.PgTypeId : null);
    }

    /// <summary>
    /// Makes a <see cref="PgConcreteTypeInfo"/> for the given value.
    /// </summary>
    /// <param name="value">The value whose content drive the concrete type info selection.</param>
    /// <param name="writeState">Contains any write state that was produced.</param>
    /// <typeparam name="T">The CLR type of the value.</typeparam>
    /// <returns>The <see cref="PgConcreteTypeInfo"/> to use for the value.</returns>
    /// <remarks>
    /// When this instance is already concrete it is returned directly; otherwise the underlying provider is consulted
    /// using the value to select the appropriate concrete type info.
    /// </remarks>
    public PgConcreteTypeInfo MakeConcreteForValue<T>(T? value, out object? writeState)
        => MakeConcreteForValue(default, value, out writeState);

    /// <summary>
    /// Makes a <see cref="PgConcreteTypeInfo"/> for the given value, with an explicit provider context.
    /// </summary>
    /// <param name="context">The context used when this instance is a provider based info.</param>
    /// <param name="value">The value whose content drives the concrete type info selection.</param>
    /// <param name="writeState">Contains any write state that was produced.</param>
    /// <typeparam name="T">The CLR type of the value.</typeparam>
    /// <remarks>
    /// When this instance is already concrete it is returned directly; otherwise the underlying provider is consulted
    /// using the value and the supplied context to select the appropriate concrete type info.
    /// </remarks>
    public PgConcreteTypeInfo MakeConcreteForValue<T>(ProviderValueContext context, T? value, out object? writeState)
    {
        if (this is PgConcreteTypeInfo concrete)
        {
            writeState = null;
            return concrete;
        }

        // Make sure we handle the non-exact typed provider case.
        // This will never cause boxing as non-exact typed infos only happen for subtype relationships, i.e. reference types.
        // We make sure to fall through to GetForValue which has a better error if T is not at all related to this info.
        var providerTypeInfo = (PgProviderTypeInfo)this;
        var concreteTypeInfo = PgProviderTypeInfo.GetProvider(providerTypeInfo) is not PgConcreteTypeInfoProvider<T> && providerTypeInfo.Type == typeof(T)
            ? providerTypeInfo.GetForValueAsObject(context, (object?)value, out writeState)
            : providerTypeInfo.GetForValue(context, value, out writeState);

        // Decided providers skip GetDefault's validation. The prior GetForValue call already validated
        // the id. Undecided providers thread it so GetDefaultCore can dispatch on it.
        return concreteTypeInfo ?? providerTypeInfo.GetDefault(providerTypeInfo.PgTypeId is null ? context.ExpectedPgTypeId : null);
    }

    /// <summary>
    /// Makes a <see cref="PgConcreteTypeInfo"/> for the given object value.
    /// </summary>
    /// <param name="value">The untyped value whose content drives the concrete type info selection.</param>
    /// <param name="writeState">Contains any write state that was produced.</param>
    /// <returns>The <see cref="PgConcreteTypeInfo"/> to use for the value.</returns>
    /// <remarks>
    /// When this instance is already concrete it is returned directly; otherwise the underlying provider is consulted
    /// using the value to select the appropriate concrete type info.
    /// </remarks>
    public PgConcreteTypeInfo MakeConcreteForValueAsObject(object? value, out object? writeState)
        => MakeConcreteForValueAsObject(default, value, out writeState);

    /// <summary>
    /// Makes a <see cref="PgConcreteTypeInfo"/> for the given object value.
    /// </summary>
    /// <param name="context">The context used when this instance is a provider based info.</param>
    /// <param name="value">The untyped value whose content drives the concrete type info selection.</param>
    /// <param name="writeState">Contains any write state that was produced.</param>
    /// <returns>The <see cref="PgConcreteTypeInfo"/> to use for the value.</returns>
    /// <remarks>
    /// When this instance is already concrete it is returned directly; otherwise the underlying provider is consulted
    /// using the value to select the appropriate concrete type info.
    /// </remarks>
    public PgConcreteTypeInfo MakeConcreteForValueAsObject(ProviderValueContext context, object? value, out object? writeState)
    {
        if (this is PgConcreteTypeInfo concrete)
        {
            writeState = null;
            return concrete;
        }

        // Decided providers skip GetDefault's validation. The prior GetForValueAsObject call already validated
        // the id. Undecided providers thread it so GetDefaultCore can dispatch on it.
        var providerTypeInfo = (PgProviderTypeInfo)this;
        return providerTypeInfo.GetForValueAsObject(context, value, out writeState)
               ?? providerTypeInfo.GetDefault(providerTypeInfo.PgTypeId is null ? context.ExpectedPgTypeId : null);
    }

    // Having it here so we can easily extend any behavior.
    internal void DisposeWriteState(object writeState)
    {
        if (writeState is IDisposable disposable)
            disposable.Dispose();
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

        // When the underlying provider permits concrete variance and the resolved Type isn't a leaf (sealed / value
        // type), dispatched concretes may vary along Type's subtype chain — HasExactType=false honestly previews that.
        // requestedType narrowing flows through Type: a variant provider narrowed via requestedType to a sealed
        // wrapper Type is canonical at that leaf, regardless of the underlying floor. Canonical providers over
        // non-sealed floors can opt out via AllowConcreteVariance=false.
        if (typeInfoProvider.AllowConcreteVariance && !Type.IsSealed)
            HasExactType = false;

        // Always validate the default provider result, the info will be re-used so there is no real downside.
        var result = typeInfoProvider.GetDefault(pgTypeId is { } id ? options.GetCanonicalTypeId(id) : null);
        ValidateConcrete(nameof(PgConcreteTypeInfoProvider.GetDefault), result);
        _defaultConcrete = result;
    }

    public PgConcreteTypeInfo GetDefault(PgTypeId? pgTypeId)
    {
        if (PgTypeId is { } decidedId)
        {
            if (pgTypeId is { } id && id != decidedId)
                ThrowUnexpectedPgTypeId(nameof(pgTypeId));
        }
        else if (pgTypeId is not null)
        {
            var result = _typeInfoProvider.GetDefault(pgTypeId);
            ValidateConcrete(nameof(PgConcreteTypeInfoProvider.GetDefault), result);
            return result;
        }

        Debug.Assert(_defaultConcrete is not null);
        return _defaultConcrete;
    }

    public PgConcreteTypeInfo? GetForField(Field field)
    {
        if (PgTypeId is { } decidedId && field.PgTypeId != decidedId)
            ThrowUnexpectedPgTypeId(nameof(field));

        var result = _typeInfoProvider.GetForField(field);
        if (result is not null)
            ValidateConcrete(nameof(PgConcreteTypeInfoProvider.GetForField), result);
        return result;
    }

    public PgConcreteTypeInfo? GetForValue<T>(ProviderValueContext context, T? value, out object? writeState)
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
            ? providerT.GetForValue(context, value, out writeState)
            : ThrowNotSupportedType(typeof(T));

        if (result is not null)
            ValidateConcrete(nameof(PgConcreteTypeInfoProvider<>.GetForValue), result);
        return result;

        PgConcreteTypeInfo ThrowNotSupportedType(Type? type)
            => throw new NotSupportedException(type == Type
                ? $"PgProviderTypeInfo does not exactly match type {type}, call {nameof(GetForValueAsObject)} instead."
                : $"PgProviderTypeInfo is incompatible with type {type}");
    }

    public PgConcreteTypeInfo? GetForValueAsObject(ProviderValueContext context, object? value, out object? writeState)
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

        var result = _typeInfoProvider.GetForValueAsObject(context, value, out writeState);
        if (result is not null)
            ValidateConcrete(nameof(PgConcreteTypeInfoProvider.GetForValueAsObject), result);
        return result;
    }

    internal PgConcreteTypeInfo? GetForValueAsNestedObject(ProviderValueContext context, object? value, out object? writeState)
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

        return _typeInfoProvider.GetForValueAsNestedObject(context, value, out writeState);
    }

    public static PgConcreteTypeInfoProvider GetProvider(PgProviderTypeInfo instance) => instance._typeInfoProvider;

    static void ThrowUnexpectedPgTypeId(string parameterName)
        => throw new ArgumentException($"PgTypeId does not match the decided value on this {nameof(PgProviderTypeInfo)}.", parameterName);

    void ValidateConcrete(string methodName, PgConcreteTypeInfo result)
    {
        // Skip self-validation for framework-internal providers (e.g. composing infrastructure); see PgConcreteTypeInfoProvider.IsInternalProvider.
        if (_typeInfoProvider.IsInternalProvider)
            return;

        ValidateInfo(methodName, result, Options, Type, allowSubtypes: !HasExactType);
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

    bool _supportsReading;
    bool _supportsWriting;

    internal PgConcreteTypeInfo(PgSerializerOptions options, PgConverter converter, PgTypeId pgTypeId, Type? requestedType)
        : base(options, converter, pgTypeId, requestedType)
    {
        Converter = converter;
        _canBinaryConvert = converter.CanConvert(DataFormat.Binary, out _binaryBufferRequirements);
        _canTextConvert = converter.CanConvert(DataFormat.Text, out _textBufferRequirements);

        // Set fields directly to bypass init guards on default values; init props enforce directional widen-to-true.
        _supportsReading = GetDefaultSupportsReading(converter.TypeToConvert, requestedType);
        _supportsWriting = GetDefaultSupportsWriting(converter.TypeToConvert, requestedType);
    }

    public PgConverter Converter { get; }

    // Author widen-to-true is only meaningful in the under-reporting direction (Type narrower than the converter's
    // type): the converter actually returns instances assignable to Type at runtime via author contract.
    public bool SupportsReading
    {
        get => _supportsReading;
        init
        {
            if (value && !_supportsReading && !Type.IsAssignableTo(Converter.TypeToConvert))
                ThrowHelper.ThrowInvalidOperationException(
                    $"Cannot widen {nameof(SupportsReading)} to true; reported type {Type} is not narrower-or-equal to converter type {Converter.TypeToConvert} (under-reporting direction).");
            _supportsReading = value;
        }
    }

    // Author widen-to-true is only meaningful in the polymorphic-alias direction (Type wider than the converter's
    // type), where the AsObject write path can route the wider inbound value through the narrower converter via cast.
    public bool SupportsWriting
    {
        get => _supportsWriting;
        init
        {
            if (value && !_supportsWriting && !Converter.TypeToConvert.IsAssignableTo(Type))
                ThrowHelper.ThrowInvalidOperationException(
                    $"Cannot widen {nameof(SupportsWriting)} to true; converter type {Converter.TypeToConvert} is not narrower-or-equal to reported type {Type} (polymorphic-alias direction).");
            _supportsWriting = value;
        }
    }

    // Defaults compute over the resolved Type (what the info will advertise), not the raw requestedType, so the
    // wider-than-converter case (polymorphic alias) collapses to self-comparison and yields reading=true, writing=true.
    // The exact-type case (Type == converter type) does the same. Under-reporting (Type narrower than converter type)
    // is asymmetric: the converter type is wider so it isn't assignable to Type, yielding reading=false (authors opt
    // in via SupportsReading=true to assert their converter actually returns runtime instances of Type), while Type is
    // narrower than the converter type, which is assignable, yielding writing=true.
    internal static bool GetDefaultSupportsReading(Type type, Type? requestedType)
        => type.IsAssignableTo(ResolveType(type, requestedType));

    internal static bool GetDefaultSupportsWriting(Type type, Type? requestedType)
        => ResolveType(type, requestedType).IsAssignableTo(type);

    public DataFormat? PreferredFormat { get; init; }
    public new PgTypeId PgTypeId => base.PgTypeId.GetValueOrDefault();

    // Cache hits are invariant in advertised type vs requested read type.
    // A different request type means a different cache key, no matter how compatible the type chains are.
    internal bool CanReadTo(Type type) => Type == type;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal T ReadFieldValue<T>(PgReader reader, in PgFieldBinding binding)
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
        var result = typeof(T) != Converter.TypeToConvert
            ? (T)(await Converter.ReadAsObjectAsync(reader, cancellationToken).ConfigureAwait(false))!
            : await Unsafe.As<PgConverter<T>>(Converter).ReadAsync(reader, cancellationToken).ConfigureAwait(false);

        await reader.EndReadAsync().ConfigureAwait(false);
        return result;
    }

    // TryBind for reading.
    internal bool TryBindField(DataFormat format, out PgFieldBinding binding)
    {
        if (!Converter.CanConvert(format, out var bufferRequirements))
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

    internal PgValueBinding BindParameterValue<T>(T? value, object? writeState, NestedObjectDbNullHandling nestedObjectDbNullHandling, DataFormat? formatPreference = null)
    {
        if (typeof(T) != Converter.TypeToConvert)
            return BindParameterValueAsObject(value, writeState, nestedObjectDbNullHandling, formatPreference);

        // Basically exists to catch cases like object[] resolving a polymorphic read converter, better to fail during binding than writing.
        if (!SupportsWriting)
            ThrowHelper.ThrowNotSupportedException($"Writing {Type} is not supported for this type info.");

        // Db nulls are format agnostic, any format will do here, bind can decide to ignore these based on size for overall format handling.
        if (Unsafe.As<PgConverter<T>>(Converter).IsDbNull(value, writeState))
            return new(DataFormat.Binary, Size.Zero, null, writeState);

        var format = ResolveFormat(out var bufferRequirements, formatPreference ?? PreferredFormat);
        var context = new SizeContext(format, bufferRequirements.Write) { NestedObjectDbNullHandling = nestedObjectDbNullHandling };
        var size = Unsafe.As<PgConverter<T>>(Converter).Bind(context, value!, ref writeState);
        return new(format, bufferRequirements.Write, size, writeState);
    }

    internal PgValueBinding BindParameterValueAsObject(object? value, object? writeState, NestedObjectDbNullHandling nestedObjectDbNullHandling, DataFormat? formatPreference = null)
    {
        // Basically exists to catch cases like object[] resolving a polymorphic read converter, better to fail during binding than writing.
        if (!SupportsWriting)
            ThrowHelper.ThrowNotSupportedException($"Writing {Type} is not supported for this type info.");

        // Db nulls are format agnostic, any format will do here, bind can decide to ignore these based on size for overall format handling.
        if (Converter.IsDbNullAsObject(value, writeState))
            return new(DataFormat.Binary, Size.Zero, null, writeState);

        var format = ResolveFormat(out var bufferRequirements, formatPreference ?? PreferredFormat);
        var context = new SizeContext(format, bufferRequirements.Write) { NestedObjectDbNullHandling = nestedObjectDbNullHandling };
        var size = Converter.BindAsObject(context, value, ref writeState);
        return new(format, bufferRequirements.Write, size, writeState);
    }

    DataFormat ResolveFormat(out BufferRequirements bufferRequirements, DataFormat? formatPreference = null)
    {
        // First try to check for preferred support.
        switch (formatPreference)
        {
        case DataFormat.Binary when _canBinaryConvert:
            bufferRequirements = _binaryBufferRequirements;
            return DataFormat.Binary;
        case DataFormat.Text when _canTextConvert:
            bufferRequirements = _textBufferRequirements;
            return DataFormat.Text;
        default:
            // The common case, no preference given (or no match) means we default to binary if supported.
            if (_canBinaryConvert)
            {
                bufferRequirements = _binaryBufferRequirements;
                return DataFormat.Binary;
            }

            if (Converter.CanConvert(DataFormat.Text, out bufferRequirements))
            {
                bufferRequirements = _textBufferRequirements;
                return DataFormat.Text;
            }

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
