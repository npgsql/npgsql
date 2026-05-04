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

    // Invariance bit on the (Converter.TypeToConvert, Type) pair: true when no requestedType was given or it equalled
    // the converter type, false when Type and the converter type differ (in either direction along their shared
    // subtype chain). False covers two construction cases that are indistinguishable downstream:
    //   - Under-reporting: Type is narrower than the converter type (e.g. ArrayConverter<Array> answering as int[]).
    //   - Polymorphic alias: Type is the converter type returned to a wider query (e.g. typeof(object)).
    // When false the typed fast path is unsafe and dispatch routes through the AsObject APIs with a runtime cast,
    // letting one converter cover multiple advertised types.
    internal bool HasExactType { get; }

    public PgTypeId? PgTypeId { get; }

    // Same predicate used at construction (ResolveType) and at the cache to validate that an info answers a given query.
    // Null requestedType: any info fits (PgTypeId-only queries).
    // requestedType == Type: trivial match.
    // Otherwise: only when this info acknowledges it isn't the exact answer (HasExactType=false) and Type sits below
    // requestedType on the chain, covers polymorphic-alias and under-reporting in one branch.
    internal bool ResolvesAs(Type? requestedType)
        => requestedType is null || requestedType == Type || (!HasExactType && Type.IsAssignableTo(requestedType));

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

        // Always validate the default provider result, the info will be re-used so there is no real downside.
        var result = typeInfoProvider.GetDefault(pgTypeId is { } id ? options.GetCanonicalTypeId(id) : null);
        ValidateResult(nameof(PgConcreteTypeInfoProvider.GetDefault), result, typeInfoProvider.TypeToConvert, options.PortableTypeIds);
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
            ValidateResult(nameof(PgConcreteTypeInfoProvider.GetDefault), result);
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
            ValidateResult(nameof(PgConcreteTypeInfoProvider.GetForField), result);
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
            ValidateResult(nameof(PgConcreteTypeInfoProvider<>.GetForValue), result);
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

        SupportsReading = GetDefaultSupportsReading(converter.TypeToConvert, requestedType);
        SupportsWriting = true;
    }

    public PgConverter Converter { get; }

    public bool SupportsReading { get; init; }
    public bool SupportsWriting { get; init; }

    // Default reads false only when the resolved Type is narrower than the converter type (under-reporting case);
    // caller can opt in to true via the init setter when they know the converter actually produces the narrower type.
    internal static bool GetDefaultSupportsReading(Type type, Type? requestedType)
        => type.IsAssignableTo(ResolveType(type, requestedType));

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
