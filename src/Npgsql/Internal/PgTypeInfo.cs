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

        IsStronglyTyped = requestedType is null || requestedType == type;
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

    // Whether a reported type was given during construction of this instance that is different from the converter type.
    // This means the converter cannot be used in a strongly typed fashion and will need to use the object apis for reading and writing.
    // This is generally used to provide one, or fewer, converter(s) to deal with some base type, e.g. Arrays or Streams.
    // In turn this impacts the number of types (or more commonly, generic instantiations) that need to be compiled for AOT as well.
    internal bool IsStronglyTyped { get; }

    public PgTypeId? PgTypeId { get; }

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
        if (this is not PgProviderTypeInfo providerTypeInfo)
            return (PgConcreteTypeInfo)this;

        // Decided providers skip GetDefault's validation. The prior GetForField call already validated
        // the id. Undecided providers thread it so GetDefaultCore can dispatch on it.
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
        if (this is not PgProviderTypeInfo providerTypeInfo)
        {
            writeState = null;
            return (PgConcreteTypeInfo)this;
        }

        // Make sure we handle the weakly typed provider case.
        // This will never cause boxing as weakly typed infos only happen for subtype relationships, i.e. reference types.
        // We make sure to fall through to GetForValue which has a better error if T is not at all related to this info.
        var concreteTypeInfo = PgProviderTypeInfo.GetProvider(providerTypeInfo) is not PgConcreteTypeInfoProvider<T> && providerTypeInfo.Type == typeof(T)
            ? providerTypeInfo.GetForValueAsObject(context, (object?)value, out writeState)
            : providerTypeInfo.GetForValue(context, value, out writeState);

        // Decided providers skip GetDefault's validation. The prior GetForValue call already validated
        // the id. Undecided providers thread it so GetDefaultCore can dispatch on it.
        return concreteTypeInfo
            ?? providerTypeInfo.GetDefault(providerTypeInfo.PgTypeId is null ? context.ExpectedPgTypeId : null);
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
        writeState = null;
        if (this is not PgProviderTypeInfo providerTypeInfo)
            return (PgConcreteTypeInfo)this;

        // Decided providers skip GetDefault's validation. The prior GetForValueAsObject call already
        // validated the id. Undecided providers thread it so GetDefaultCore can dispatch on it.
        return providerTypeInfo.GetForValueAsObject(context, value, out writeState)
            ?? providerTypeInfo.GetDefault(providerTypeInfo.PgTypeId is null ? context.ExpectedPgTypeId : null);
    }

    // We assume a weakly typed info does not support reading as the converter won't be able to produce the derived type statically.
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

    public PgConcreteTypeInfo GetDefault(PgTypeId? pgTypeId)
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
                ? $"PgProviderTypeInfo is weakly compatible with type {type}, call {nameof(GetForValueAsObject)} instead."
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

    public PgConcreteTypeInfo? GetForNestedObjectValue(ProviderValueContext context, object? value, out object? writeState)
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
        switch (context.NestedObjectDbNullHandling)
        {
        case NestedObjectDbNullHandling.ExtendedThrowOnNull:
            if (value is null)
                ThrowHelper.ThrowArgumentNullException(nameof(value), "Object-typed value cannot be null, a db null value must be used instead.");
            goto case NestedObjectDbNullHandling.Extended;
        case NestedObjectDbNullHandling.Extended:
            if (value is DBNull)
                return null;
            goto case NestedObjectDbNullHandling.Default;
        case NestedObjectDbNullHandling.Default:
            return value is null ? null : _typeInfoProvider.GetForValueAsObject(context, value, ref writeState);
        default:
            ThrowHelper.ThrowUnreachableException();
            return default;
        }
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

    readonly Type _typeToConvert;

    public PgConcreteTypeInfo(PgSerializerOptions options, PgConverter converter, PgTypeId pgTypeId)
        : this(options, converter, pgTypeId, requestedType: null)
    {}

    internal PgConcreteTypeInfo(PgSerializerOptions options, PgConverter converter, PgTypeId pgTypeId, Type? requestedType)
        : base(options, converter, pgTypeId, requestedType)
    {
        Converter = converter;
        _typeToConvert = converter.TypeToConvert;
        _canBinaryConvert = converter.CanConvert(DataFormat.Binary, out _binaryBufferRequirements);
        _canTextConvert = converter.CanConvert(DataFormat.Text, out _textBufferRequirements);
    }

    public PgConverter Converter { get; }
    public new PgTypeId PgTypeId => base.PgTypeId.GetValueOrDefault();

    public bool IsNestedObjectDbNull([NotNullWhen(false)] object? value, object? writeState, NestedObjectDbNullHandling nestedObjectDbNullHandling)
    {
        switch (nestedObjectDbNullHandling)
        {
        case NestedObjectDbNullHandling.ExtendedThrowOnNull:
            if (value is null)
                ThrowHelper.ThrowArgumentNullException(nameof(value), "Object-typed value cannot be null, a db null value must be used instead.");
            goto case NestedObjectDbNullHandling.Extended;
        case NestedObjectDbNullHandling.Extended:
            if (value is DBNull)
                return true;
            goto case NestedObjectDbNullHandling.Default;
        case NestedObjectDbNullHandling.Default:
            return value is null || Converter.IsDbNullAsObject(value, writeState);
        default:
            ThrowHelper.ThrowUnreachableException();
            return default;
        }
    }

    public Size GetSize<T>(SizeContext context, [DisallowNull] T value, [NotNullIfNotNull(nameof(writeState))] ref object? writeState)
    {
        if (context.BufferRequirement is { Kind: SizeKind.Exact, Value: var byteCount })
            return byteCount;

        var size = GetConverter<T>().GetSize(context, value, ref writeState);

        switch (size.Kind)
        {
        case SizeKind.UpperBound:
            ThrowHelper.ThrowInvalidOperationException($"{nameof(SizeKind.UpperBound)} is not a valid return value for GetSize.");
            break;
        case SizeKind.Unknown:
            // Not valid yet.
            ThrowHelper.ThrowInvalidOperationException($"{nameof(SizeKind.Unknown)} is not a valid return value for GetSize.");
            break;
        }

        return size;
    }

    public Size GetSizeAsObject(SizeContext context, object value, [NotNullIfNotNull(nameof(writeState))] ref object? writeState)
    {
        if (context.BufferRequirement is { Kind: SizeKind.Exact, Value: var byteCount })
            return byteCount;

        var size = Converter.GetSizeAsObject(context, value, ref writeState);

        switch (size.Kind)
        {
        case SizeKind.UpperBound:
            ThrowHelper.ThrowInvalidOperationException($"{nameof(SizeKind.UpperBound)} is not a valid return value for GetSize.");
            break;
        case SizeKind.Unknown:
            // Not valid yet.
            ThrowHelper.ThrowInvalidOperationException($"{nameof(SizeKind.Unknown)} is not a valid return value for GetSize.");
            break;
        }

        return size;
    }

    public Size? IsDbNullOrGetSize<T>(SizeContext context, T? value, [NotNullIfNotNull(nameof(writeState))] ref object? writeState)
        => GetConverter<T>().IsDbNull(value, writeState) ? null : GetSize(context, value, ref writeState);

    public Size? IsDbNullOrGetSizeAsObject(SizeContext context, object? value, [NotNullIfNotNull(nameof(writeState))] ref object? writeState)
        => Converter.IsDbNullAsObject(value, writeState) ? null : GetSizeAsObject(context, value, ref writeState);

    public Size? IsNestedObjectDbNullOrGetSize(SizeContext context, object? value, [NotNullIfNotNull(nameof(writeState))] ref object? writeState)
    {
        if (IsNestedObjectDbNull(value, writeState, context.NestedObjectDbNullHandling))
            return null;

        return GetSizeAsObject(context, value, ref writeState);
    }

    public bool CanReadTo(Type type) => Type == type || (!IsStronglyTyped && Type.IsAssignableTo(type));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool ShouldReadAsObject<T>() => typeof(T) != _typeToConvert;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    PgConverter<T> GetConverter<T>()
    {
        if (ShouldReadAsObject<T>())
            ThrowHelper.ThrowInvalidCastException("Invalid type for converter.");

        // Justification: avoid perf cost of casting to a known base class type per field read.
        Debug.Assert(Converter is PgConverter<T>);
        return Unsafe.As<PgConverter<T>>(Converter);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal T ConverterRead<T>(PgReader reader)
        => ShouldReadAsObject<T>()
            ? (T)Converter.ReadAsObject(reader)
            : GetConverter<T>().Read(reader);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ValueTask<T> ConverterReadAsync<T>(PgReader reader, CancellationToken cancellationToken)
    {
        if (ShouldReadAsObject<T>())
        {
            var task = Converter.ReadAsObjectAsync(reader, cancellationToken);
            return task.IsCompletedSuccessfully ? new((T)task.Result) : ReadAndUnboxAsync(task);
        }

        return GetConverter<T>().ReadAsync(reader, cancellationToken);

        [MethodImpl(MethodImplOptions.NoInlining)]
        async ValueTask<T> ReadAndUnboxAsync(ValueTask<object> task)
            => (T)await task.ConfigureAwait(false);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void ConverterWrite<T>(PgWriter writer, [DisallowNull]T value)
    {
        if (!IsStronglyTyped)
        {
            Converter.WriteAsObject(writer, value);
            return;
        }
        GetConverter<T>().Write(writer, value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ValueTask ConverterWriteAsync<T>(PgWriter writer, [DisallowNull]T value, CancellationToken cancellationToken)
        => !IsStronglyTyped
            ? Converter.WriteAsObjectAsync(writer, value, cancellationToken)
            : GetConverter<T>().WriteAsync(writer, value, cancellationToken);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal T ReadFieldValue<T>(PgReader reader, PgFieldBinding binding)
    {
        reader.StartRead(binding);
        var result = ConverterRead<T>(reader);
        reader.EndRead();
        return result;
    }

    internal async ValueTask<T> ReadFieldValueAsync<T>(PgReader reader, PgFieldBinding binding, CancellationToken cancellationToken)
    {
        await reader.StartReadAsync(binding, cancellationToken).ConfigureAwait(false);

        // Copy of ConverterReadAsync to keep everything in one async frame.
        T result;
        if (ShouldReadAsObject<T>())
            result = (T)await Converter.ReadAsObjectAsync(reader, cancellationToken).ConfigureAwait(false);
        else
            result = await GetConverter<T>().ReadAsync(reader, cancellationToken).ConfigureAwait(false);

        await reader.EndReadAsync().ConfigureAwait(false);
        return result;
    }

    public BufferRequirements? GetBufferRequirements(DataFormat format)
    {
        var success = CanConvert(format, out var bufferRequirements);
        return success ? bufferRequirements : null;
    }

    // Having it here so we can easily extend any behavior.
    internal void DisposeWriteState(object writeState)
    {
        if (writeState is IDisposable disposable)
            disposable.Dispose();
    }

    // TryBind for reading.
    internal bool TryBindField(DataFormat format, out PgFieldBinding context)
    {
        if (!CanConvert(format, out var bufferRequirements))
        {
            context = default;
            return false;
        }
        context = new(format, bufferRequirements.Read);
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
        if (!IsStronglyTyped)
            return BindParameterValueAsObject(value, writeState, nestedObjectDbNullHandling, formatPreference);

        // Basically exists to catch cases like object[] resolving a polymorphic read converter, better to fail during binding than writing.
        if (!SupportsWriting)
            ThrowHelper.ThrowNotSupportedException($"Writing {Type} is not supported for this type info.");

        if (GetConverter<T>().IsDbNull(value, writeState))
            return new(DataFormat.Binary, Size.Zero, null, writeState);

        var format = ResolveFormat(out var bufferRequirements, formatPreference ?? PreferredFormat);
        var context = new SizeContext(format, bufferRequirements.Write) { NestedObjectDbNullHandling = nestedObjectDbNullHandling };
        var size = GetSize(context, value, ref writeState);
        return new(format, bufferRequirements.Write, size, writeState);
    }

    internal PgValueBinding BindParameterValueAsObject(object? value, object? writeState, NestedObjectDbNullHandling nestedObjectDbNullHandling, DataFormat? formatPreference = null)
    {
        // Basically exists to catch cases like object[] resolving a polymorphic read converter, better to fail during binding than writing.
        if (!SupportsWriting)
            throw new NotSupportedException($"Writing {Type} is not supported for this type info.");

        if (Converter.IsDbNullAsObject(value, writeState))
            return new(DataFormat.Binary, Size.Zero, null, writeState);

        var format = ResolveFormat(out var bufferRequirements, formatPreference ?? PreferredFormat);
        var context = new SizeContext(format, bufferRequirements.Write) { NestedObjectDbNullHandling = nestedObjectDbNullHandling };
        var size = GetSizeAsObject(context, value, ref writeState);
        return new(format, bufferRequirements.Write, size, writeState);
    }

    bool CanConvert(DataFormat format, out BufferRequirements bufferRequirements)
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

    // DataFormat can differ from the actual field format if data will be reinterpreted for this binding (e.g. UnknownResultType)
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
