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

    // Forward-looking contract bit about the converter consumers will ultimately use through this info: true when the
    // converter's type matches Type canonically, false when consumers should expect variation (the typed fast path is
    // unsafe; dispatch routes through the AsObject APIs with a runtime cast).
    //   - Concrete: false when (Converter.TypeToConvert, Type) differ in either direction along their shared subtype
    //     chain (under-reporting or polymorphic alias).
    //   - Provider: false when the underlying provider is polymorphic and will dispatch to varied concretes per call;
    //     even though the wrapping pair (TypeToConvert, Type) is invariant, what flows out is not.
    internal bool HasExactType { get; private protected init; }

    public PgTypeId? PgTypeId { get; }

    // Same predicate used at construction (ResolveType) and at the cache to validate that an info answers a given query.
    // Null requestedType: any info fits (PgTypeId-only queries).
    // requestedType == Type: trivial match.
    // Otherwise: only when this info acknowledges it isn't the exact answer (HasExactType=false) and Type sits below
    // requestedType on the chain, covers polymorphic-alias and under-reporting in one branch.
    internal bool ResolvesAs(Type? requestedType)
        => requestedType is null || requestedType == Type || (!HasExactType && Type.IsAssignableTo(requestedType));

    // On the outer info (this), check whether a concrete result is admissible per the outer's contract.
    // HasExactType=true requires canonical match (result.Type == Type); HasExactType=false admits narrower-or-equal
    // results on the subtype chain. Used at provider boundaries to validate that concretes flowing out fit.
    internal bool AcceptsResult(PgConcreteTypeInfo result)
        => result.Type == Type || (!HasExactType && result.Type.IsAssignableTo(Type));

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
            ? providerT.GetForValue(context, value, ref writeState)
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

        writeState = null;
        var result = _typeInfoProvider.GetForValueAsObject(context, value, ref writeState);
        if (result is not null)
            ValidateConcrete(nameof(PgConcreteTypeInfoProvider.GetForValueAsObject), result);
        return result;
    }

    public static PgConcreteTypeInfoProvider GetProvider(PgProviderTypeInfo instance) => instance._typeInfoProvider;

    static void ThrowUnexpectedPgTypeId(string parameterName)
        => throw new ArgumentException($"PgTypeId does not match the decided value on this {nameof(PgProviderTypeInfo)}.", parameterName);

    void ValidateConcrete(string methodName, PgConcreteTypeInfo result)
    {
        // Skip self-validation for framework-internal providers (e.g. composing infrastructure); see PgConcreteTypeInfoProvider.IsInternalProvider.
        if (_typeInfoProvider.IsInternalProvider)
            return;

        var expectedTypeToConvert = _typeInfoProvider.TypeToConvert;
        if (expectedTypeToConvert != typeof(object) && result.Converter.TypeToConvert != expectedTypeToConvert)
            throw new InvalidOperationException($"'{methodName}' returned a {nameof(result.Converter)} of type {result.Converter.TypeToConvert} instead of {expectedTypeToConvert} unexpectedly.");

        // Plugins ship in-tree, so this fires in release. The result must fit the outer info's contract:
        // canonical when the outer is HasExactType=true, otherwise narrower-or-equal on the chain.
        if (!AcceptsResult(result))
            throw new InvalidOperationException($"'{methodName}' returned a concrete type info advertising type {result.Type} which is incompatible with this info's contract (Type={Type}, HasExactType={HasExactType}).");

        var expectPortableTypeIds = Options.PortableTypeIds;

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
    // wider-than-converter case naturally collapses to self-comparison.
    //   - Exact (Type == converter type): both checks are self-comparisons → true.
    //   - Under-reporting (Type narrower than converter type): read true (info.Type fits in caller's slot of the same
    //     type), write false-ish (writing the narrower advertised type into the wider converter is safe via cast,
    //     so this also returns true via assignability).
    //   - Polymorphic alias (Type == converter type, requestedType wider): self-comparison → true.
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
            ? (T)await Converter.ReadAsObjectAsync(reader, cancellationToken).ConfigureAwait(false)
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

    // Bind for writing.
    /// When result is null, the value was interpreted to be a SQL NULL.
    internal PgValueBinding BindParameterValue<T>(T? value, object? writeState, DataFormat? formatPreference = null)
    {
        if (typeof(T) != Converter.TypeToConvert)
            return BindParameterObjectValue(value, writeState, formatPreference);

        // Basically exists to catch cases like object[] resolving a polymorphic read converter, better to fail during binding than writing.
        if (!SupportsWriting)
            ThrowHelper.ThrowNotSupportedException($"Writing {Type} is not supported for this type info.");

        var format = ResolveFormat(out var bufferRequirements, formatPreference ?? PreferredFormat);

        Debug.Assert(Converter is PgConverter<T>);
        if (Unsafe.As<PgConverter<T>>(Converter).IsDbNullOrGetSize(format, bufferRequirements.Write, value, ref writeState) is not { } size)
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
        if (value is DBNull && Type != typeof(DBNull) || Converter.IsDbNullOrGetSizeAsObject(format, bufferRequirements.Write, value, ref writeState) is not { } size)
        {
            return new(format, bufferRequirements.Write, null, null);
        }

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
