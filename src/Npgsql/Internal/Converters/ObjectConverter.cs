using System;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal.Postgres;

namespace Npgsql.Internal;

sealed class ObjectConverter(PgSerializerOptions options, PgTypeId pgTypeId) : PgStreamingConverter<object>(customDbNullPredicate: true)
{
    protected override bool IsDbNullValue(object? value, ref object? writeState)
    {
        if (value is null or DBNull)
            return true;

        var typeInfo = GetTypeInfo(value.GetType());

        object? effectiveState = null;
        var converter = typeInfo.GetObjectResolution(value).Converter;
        if (converter.IsDbNullAsObject(value, ref effectiveState))
            return true;

        writeState = effectiveState is not null ? new WriteState { TypeInfo = typeInfo, EffectiveState = effectiveState } : typeInfo;
        return false;
    }

    public override object Read(PgReader reader) => throw new NotSupportedException();
    public override ValueTask<object> ReadAsync(PgReader reader, CancellationToken cancellationToken = default) => throw new NotSupportedException();

    public override Size GetSize(SizeContext context, object value, ref object? writeState)
    {
        var (typeInfo, effectiveState) = writeState switch
        {
            PgTypeInfo info => (info, null),
            WriteState state => (state.TypeInfo, state.EffectiveState),
            _ => throw new InvalidOperationException("Invalid state")
        };

        // We can call GetDefaultResolution here as validation has already happened in IsDbNullValue.
        // And we know it was called due to the writeState being filled.
        var converter = typeInfo is PgResolverTypeInfo resolverTypeInfo
            ? resolverTypeInfo.GetDefaultResolution(null).Converter
            : typeInfo.GetResolution().Converter;
        if (typeInfo.GetBufferRequirements(converter, context.Format) is not { } bufferRequirements)
        {
            ThrowHelper.ThrowNotSupportedException($"Resolved converter '{converter.GetType()}' has to support the {context.Format} format to be compatible.");
            return default;
        }

        // Fixed size converters won't have a GetSize implementation.
        if (bufferRequirements.Write.Kind is SizeKind.Exact)
            return bufferRequirements.Write;

        var result = converter.GetSizeAsObject(context, value, ref effectiveState);
        if (effectiveState is not null)
        {
            if (writeState is WriteState state && !ReferenceEquals(state.EffectiveState, effectiveState))
                state.EffectiveState = effectiveState;
            else
                writeState = new WriteState { TypeInfo = typeInfo, EffectiveState = effectiveState };
        }

        return result;
    }

    public override void Write(PgWriter writer, object value)
        => Write(async: false, writer, value, CancellationToken.None).GetAwaiter().GetResult();

    public override ValueTask WriteAsync(PgWriter writer, object value, CancellationToken cancellationToken = default)
        => Write(async: true, writer, value, cancellationToken);

    async ValueTask Write(bool async, PgWriter writer, object value, CancellationToken cancellationToken)
    {
        var (typeInfo, effectiveState) = writer.Current.WriteState switch
        {
            PgTypeInfo info => (info, null),
            WriteState state => (state.TypeInfo, state.EffectiveState),
            _ => throw new InvalidOperationException("Invalid state")
        };

        // We can call GetDefaultResolution here as validation has already happened in IsDbNullValue.
        // And we know it was called due to the writeState being filled.
        var converter = typeInfo is PgResolverTypeInfo resolverTypeInfo
            ? resolverTypeInfo.GetDefaultResolution(null).Converter
            : typeInfo.GetResolution().Converter;
        var writeRequirement = typeInfo.GetBufferRequirements(converter, DataFormat.Binary)!.Value.Write;
        using var _ = await writer.BeginNestedWrite(async, writeRequirement, writer.Current.Size.Value, effectiveState, cancellationToken).ConfigureAwait(false);
        await converter.WriteAsObject(async, writer, value, cancellationToken).ConfigureAwait(false);
    }

    PgTypeInfo GetTypeInfo(Type type)
        => options.GetTypeInfoInternal(type, pgTypeId)
           ?? throw new NotSupportedException($"Writing values of '{type.FullName}' having DataTypeName '{options.DatabaseInfo.GetPostgresType(pgTypeId).DisplayName}' is not supported.");

    sealed class WriteState
    {
        public required PgTypeInfo TypeInfo { get; init; }
        public required object EffectiveState { get; set; }
    }
}
