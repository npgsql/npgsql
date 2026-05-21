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
        object? writeState = null;
        ValidateInfo(contextName, info, options, expectedType, allowSubtypes, ref writeState);
    }

    // writeState-bearing overload: disposes-and-nulls writeState before the throw so callers get
    // "out param is null on throw" semantics without needing to wrap in try/catch.
    private protected static void ValidateInfo(string contextName, PgTypeInfo info, PgSerializerOptions options, Type? expectedType, bool allowSubtypes, ref object? writeState)
    {
        if (info.Options == options && IsCompatibleResolution(info.Type, expectedType, allowSubtypes))
            return;

        (var toDispose, writeState) = (writeState, null);
        (toDispose as IDisposable)?.Dispose();
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
        // the id. Undecided providers thread it so GetDefaultCore can dispatch on it. The provider gate
        // enforces "null result ⇒ null state," so the GetDefault fallback can't observe orphaned state.
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

        // Decided providers skip GetDefault's validation. The prior GetForValueAsObject call already
        // validated the id. Undecided providers thread it so GetDefaultCore can dispatch on it. The
        // provider gate enforces "null result ⇒ null state," so the GetDefault fallback can't observe
        // orphaned state.
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

    private PgProviderTypeInfo(PgSerializerOptions options, PgConcreteTypeInfoProvider typeInfoProvider, PgTypeId? pgTypeId, Type? requestedType = null)
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
        object? writeState = null;
        ValidateConcrete(nameof(PgConcreteTypeInfoProvider.GetDefault), result, ref writeState);
        _defaultConcrete = result;
    }

    /// <summary>Creates a provider-backed type info.</summary>
    public static PgProviderTypeInfo Create(PgSerializerOptions options, PgConcreteTypeInfoProvider provider, PgTypeId? pgTypeId, Type? requestedType = null)
        => new(options, provider, pgTypeId, requestedType);

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
            object? writeState = null;
            ValidateConcrete(nameof(PgConcreteTypeInfoProvider.GetDefault), result, ref writeState);
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
        {
            object? writeState = null;
            ValidateConcrete(nameof(PgConcreteTypeInfoProvider.GetForField), result, ref writeState);
        }
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
            ValidateConcrete(nameof(PgConcreteTypeInfoProvider<>.GetForValue), result, ref writeState);
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
            ValidateConcrete(nameof(PgConcreteTypeInfoProvider.GetForValueAsObject), result, ref writeState);
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

    void ValidateConcrete(string methodName, PgConcreteTypeInfo result, ref object? writeState)
    {
        // Skip self-validation for framework-internal providers (e.g. composing infrastructure); see PgConcreteTypeInfoProvider.IsInternalProvider.
        if (_typeInfoProvider.IsInternalProvider)
            return;

        ValidateInfo(methodName, result, Options, Type, allowSubtypes: !HasExactType, ref writeState);
    }
}

public sealed class PgConcreteTypeInfo : PgTypeInfo
{
    readonly bool _supportsReading;
    readonly bool _supportsWriting;

    readonly BufferRequirements _binaryBufferRequirements;
    readonly BufferRequirements _textBufferRequirements;

    // <paramref name="binary"/> fills the binary slot, <paramref name="text"/> fills the text slot.
    // Both slots may carry the same instance (multi-format converter) or different instances (single-format per slot).
    internal PgConcreteTypeInfo(PgSerializerOptions options, PgConverter binary, PgConverter? text, PgTypeId pgTypeId, Type? requestedType = null)
        : base(options, binary, pgTypeId, requestedType)
    {
        if (text is not null && binary.TypeToConvert != text.TypeToConvert)
            throw new ArgumentException($"Binary converter type {binary.TypeToConvert} and text converter type {text.TypeToConvert} must match.", nameof(text));

        Converter = binary;
        BinaryConverter = binary;
        _binaryBufferRequirements = binary.GetDescriptor(new ConversionContext()).BufferRequirements;
        if (text is not null)
        {
            TextConverter = text;
            _textBufferRequirements = text.GetDescriptor(new ConversionContext { TextEncoding = options.TextEncoding }).BufferRequirements;
        }

        _supportsReading = GetDefaultSupportsReading(binary.TypeToConvert, requestedType);
        _supportsWriting = GetDefaultSupportsWriting(binary.TypeToConvert, requestedType);
    }

    /// <summary>
    /// Creates a concrete type info with a binary-only converter. Use the dual overload for
    /// converters that also handle the text wire format.
    /// </summary>
    public static PgConcreteTypeInfo Create(
        PgSerializerOptions options,
        PgConverter binary,
        PgTypeId pgTypeId,
        Type? requestedType = null,
        DataFormat? preferredFormat = null,
        bool? supportsReading = null,
        bool? supportsWriting = null)
        => new(options, binary, null, pgTypeId, requestedType)
        {
            PreferredFormat = preferredFormat,
            SupportsReading = supportsReading ?? GetDefaultSupportsReading(binary.TypeToConvert, requestedType),
            SupportsWriting = supportsWriting ?? GetDefaultSupportsWriting(binary.TypeToConvert, requestedType),
        };

    /// <summary>
    /// Creates a concrete type info with explicit dual converters: <paramref name="binary"/> fills
    /// the binary wire-format slot, <paramref name="text"/> fills the text wire-format slot.
    /// </summary>
    public static PgConcreteTypeInfo Create(
        PgSerializerOptions options,
        PgConverter binary,
        PgConverter text,
        PgTypeId pgTypeId,
        Type? requestedType = null,
        DataFormat? preferredFormat = null,
        bool? supportsReading = null,
        bool? supportsWriting = null)
        => new(options, binary, text, pgTypeId, requestedType)
        {
            PreferredFormat = preferredFormat,
            SupportsReading = supportsReading ?? GetDefaultSupportsReading(binary.TypeToConvert, requestedType),
            SupportsWriting = supportsWriting ?? GetDefaultSupportsWriting(binary.TypeToConvert, requestedType),
        };

    /// <summary>
    /// Creates a wrapping concrete type info with explicit dual converters, AND-propagating
    /// <see cref="SupportsReading"/> and <see cref="SupportsWriting"/> from this info into the wrapper.
    /// </summary>
    public PgConcreteTypeInfo CreateComposition(
        PgConverter binary,
        PgConverter? text,
        PgTypeId pgTypeId,
        Type? requestedType = null,
        bool? supportsReadingOverride = null,
        bool? supportsWritingOverride = null,
        DataFormat? preferredFormat = null)
    {
        var readingSupported = SupportsReading
                               && (supportsReadingOverride ?? GetDefaultSupportsReading(binary.TypeToConvert, requestedType));
        var writingSupported = SupportsWriting
                               && (supportsWritingOverride ?? GetDefaultSupportsWriting(binary.TypeToConvert, requestedType));
        return new PgConcreteTypeInfo(Options, binary, text, pgTypeId, requestedType)
        {
            PreferredFormat = preferredFormat,
            SupportsReading = readingSupported,
            SupportsWriting = writingSupported
        };
    }

    public PgConverter Converter { get; }

    /// <summary>The converter that handles the binary wire format, or null when binary is unsupported.</summary>
    public PgConverter? BinaryConverter { get; }

    /// <summary>The converter that handles the text wire format, or null when text is unsupported.</summary>
    public PgConverter? TextConverter { get; }

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
        var result = typeof(T) == Converter.TypeToConvert
            ? await Unsafe.As<PgConverter<T>>(Converter).ReadAsync(reader, cancellationToken).ConfigureAwait(false)
            : (T)(await Converter.ReadAsObjectAsync(reader, cancellationToken).ConfigureAwait(false))!;

        await reader.EndReadAsync().ConfigureAwait(false);
        return result;
    }

    // TryBind for reading.
    internal bool TryBindField(DataFormat format, out PgFieldBinding binding)
    {
        switch (format)
        {
        case DataFormat.Binary when BinaryConverter is not null:
            binding = new(format, _binaryBufferRequirements.Read);
            return true;
        case DataFormat.Text when TextConverter is not null:
            binding = new(format, _textBufferRequirements.Read);
            return true;
        default:
            binding = default;
            return false;
        }
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

        try
        {
            // Basically exists to catch cases like object[] resolving a polymorphic read converter, better to fail during binding than writing.
            if (!SupportsWriting)
                ThrowHelper.ThrowNotSupportedException($"Writing {Type} is not supported for this type info.");

            // Db nulls are format agnostic, any format will do here, bind can decide to ignore these based on size for overall format handling.
            if (Unsafe.As<PgConverter<T>>(Converter).IsDbNull(value, writeState))
                return new(DataFormat.Binary, Size.Zero, null, writeState);

            var format = ResolveFormat(out var bufferRequirements, formatPreference ?? PreferredFormat);
            var context = BindContext.CreateUnchecked(format, bufferRequirements.Write, bufferRequirements.IsBindOptional)
                with { NestedObjectDbNullHandling = nestedObjectDbNullHandling };
            var size = Unsafe.As<PgConverter<T>>(Converter).Bind(context, value!, ref writeState);
            return new(format, bufferRequirements.Write, size, writeState);
        }
        catch
        {
            // Pre-Bind throws (SupportsWriting, IsDbNull, ResolveFormat) bypass PgConverter.Bind's safety
            // net so we dispose here. Bind throws null writeState via the safety net first, leaving this
            // a no-op.
            if (writeState is not null)
                DisposeWriteState(writeState);
            throw;
        }
    }

    /// Object route with parameter-policy null detection. Wraps <see cref="BindParameterValueAsObject"/>
    /// with an upfront <see cref="PgConverterExtensions.IsDbNullAsNestedObject"/> check so callers (the
    /// parameter layer) don't have to thread the policy themselves and can't leak writeState across the
    /// pre-check.
    internal PgValueBinding BindParameterValueAsNestedObject(object? value, object? writeState, NestedObjectDbNullHandling nestedObjectDbNullHandling, DataFormat? formatPreference = null)
    {
        bool isDbNull;
        try
        {
            isDbNull = Converter.IsDbNullAsNestedObject(value, writeState, nestedObjectDbNullHandling);
        }
        catch
        {
            if (writeState is not null)
                DisposeWriteState(writeState);
            throw;
        }

        return isDbNull
            ? new(DataFormat.Binary, Size.Zero, null, writeState)
            : BindParameterValueAsObject(value, writeState, nestedObjectDbNullHandling, formatPreference);
    }

    PgValueBinding BindParameterValueAsObject(object? value, object? writeState, NestedObjectDbNullHandling nestedObjectDbNullHandling, DataFormat? formatPreference = null)
    {
        try
        {
            // Basically exists to catch cases like object[] resolving a polymorphic read converter, better to fail during binding than writing.
            if (!SupportsWriting)
                ThrowHelper.ThrowNotSupportedException($"Writing {Type} is not supported for this type info.");

            // Db nulls are format agnostic, any format will do here, bind can decide to ignore these based on size for overall format handling.
            if (Converter.IsDbNullAsObject(value, writeState))
                return new(DataFormat.Binary, Size.Zero, null, writeState);

            var format = ResolveFormat(out var bufferRequirements, formatPreference ?? PreferredFormat);
            var context = BindContext.CreateUnchecked(format, bufferRequirements.Write, bufferRequirements.IsBindOptional)
                with { NestedObjectDbNullHandling = nestedObjectDbNullHandling };
            var size = Converter.BindAsObject(context, value, ref writeState);
            return new(format, bufferRequirements.Write, size, writeState);
        }
        catch
        {
            // Pre-Bind throws (SupportsWriting, IsDbNull, ResolveFormat) bypass PgConverter.Bind's safety
            // net so we dispose here. Bind throws null writeState via the safety net first, leaving this
            // a no-op.
            if (writeState is not null)
                DisposeWriteState(writeState);
            throw;
        }
    }

    DataFormat ResolveFormat(out BufferRequirements bufferRequirements, DataFormat? formatPreference = null)
    {
        // First try to check for preferred support.
        switch (formatPreference)
        {
        case DataFormat.Binary when BinaryConverter is not null:
            bufferRequirements = _binaryBufferRequirements;
            return DataFormat.Binary;
        case DataFormat.Text when TextConverter is not null:
            bufferRequirements = _textBufferRequirements;
            return DataFormat.Text;
        default:
            // The common case, no preference given (or no match) means we default to binary if supported.
            if (BinaryConverter is not null)
            {
                bufferRequirements = _binaryBufferRequirements;
                return DataFormat.Binary;
            }

            if (TextConverter is not null)
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
