using System;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal.Postgres;

namespace Npgsql.Internal;

sealed class ObjectConverter() : PgStreamingConverter<object>(customDbNullPredicate: true)
{
    protected override bool IsDbNullValue(object? value, ref object? writeState)
    {
        var concreteTypeInfo = writeState switch
        {
            PgConcreteTypeInfo info => info,
            WriteState ws => ws.ConcreteTypeInfo,
            _ => throw new InvalidOperationException("writeState cannot be null, ObjectTypeInfoProvider is expected to pre-populate it with concrete type info.")
        };

        object? effectiveState = null;
        var isDbNull = concreteTypeInfo.Converter.IsDbNullAsObject(value, ref effectiveState);
        if (writeState is WriteState s && !ReferenceEquals(s.EffectiveState, effectiveState))
            s.EffectiveState = effectiveState;
        else if (writeState is not null)
            writeState = new WriteState { ConcreteTypeInfo = concreteTypeInfo, EffectiveState = effectiveState };

        return isDbNull;
    }

    public override object Read(PgReader reader) => throw new NotSupportedException();
    public override ValueTask<object> ReadAsync(PgReader reader, CancellationToken cancellationToken = default) => throw new NotSupportedException();

    public override Size GetSize(SizeContext context, object value, ref object? writeState)
    {
        var (concreteTypeInfo, effectiveState) = writeState switch
        {
            PgConcreteTypeInfo info => (info, (object?)null),
            WriteState state => (state.ConcreteTypeInfo, state.EffectiveState),
            _ => throw new InvalidOperationException("Invalid state")
        };

        if (concreteTypeInfo.GetBufferRequirements(concreteTypeInfo.Converter, context.Format) is not { } bufferRequirements)
        {
            ThrowHelper.ThrowNotSupportedException($"Resolved converter '{concreteTypeInfo.Converter.GetType()}' has to support the {context.Format} format to be compatible.");
            return default;
        }

        // Fixed size converters won't have a GetSize implementation.
        if (bufferRequirements.Write.Kind is SizeKind.Exact)
            return bufferRequirements.Write;

        var result = concreteTypeInfo.Converter.GetSizeAsObject(context, value, ref effectiveState);
        if (effectiveState is not null)
        {
            if (writeState is WriteState s && !ReferenceEquals(s.EffectiveState, effectiveState))
                s.EffectiveState = effectiveState;
            else
                writeState = new WriteState { ConcreteTypeInfo = concreteTypeInfo, EffectiveState = effectiveState };
        }

        return result;
    }

    public override void Write(PgWriter writer, object value)
        => Write(async: false, writer, value, CancellationToken.None).GetAwaiter().GetResult();

    public override ValueTask WriteAsync(PgWriter writer, object value, CancellationToken cancellationToken = default)
        => Write(async: true, writer, value, cancellationToken);

    async ValueTask Write(bool async, PgWriter writer, object value, CancellationToken cancellationToken)
    {
        var (concreteTypeInfo, effectiveState) = writer.Current.WriteState switch
        {
            PgConcreteTypeInfo info => (info, (object?)null),
            WriteState state => (state.ConcreteTypeInfo, state.EffectiveState),
            _ => throw new InvalidOperationException("Invalid state")
        };

        var writeRequirement = concreteTypeInfo.GetBufferRequirements(concreteTypeInfo.Converter, DataFormat.Binary)!.Value.Write;
        using var _ = await writer.BeginNestedWrite(async, writeRequirement, writer.Current.Size.Value, effectiveState, cancellationToken).ConfigureAwait(false);
        await concreteTypeInfo.Converter.WriteAsObject(async, writer, value, cancellationToken).ConfigureAwait(false);
    }

    sealed class WriteState
    {
        public required PgConcreteTypeInfo ConcreteTypeInfo { get; init; }
        public required object? EffectiveState { get; set; }
    }
}

// TODO the goal is to allow this provider to return the underlying converter type info, but we're not there yet.
// At that point we don't need the ObjectConverter any longer.
sealed class LateBoundTypeInfoProvider(PgSerializerOptions options, PgTypeId typeId) : PgConcreteTypeInfoProvider<object>
{
    PgConcreteTypeInfo? _defaultConcreteTypeInfo;

    protected override PgConcreteTypeInfo GetDefaultCore(PgTypeId? pgTypeId)
        => pgTypeId is { } expectedId && expectedId != typeId
            ? new(options, new ObjectConverter(), expectedId)
            : _defaultConcreteTypeInfo ??= new(options, new ObjectConverter(), typeId);

    protected override PgConcreteTypeInfo? GetForValueCore(ProviderValueContext context, object? value, ref object? writeState)
    {
        if (value is null or DBNull)
        {
            writeState = options.UnspecifiedDBNullTypeInfo;
            return GetDefaultCore(context.ExpectedPgTypeId);
        }

        var typeInfo = AdoSerializerHelpers.GetTypeInfoForWriting(value.GetType(), context.ExpectedPgTypeId ?? typeId, options);
        writeState = typeInfo.GetObjectConcreteTypeInfo(value, out _);

        return GetDefault(context.ExpectedPgTypeId);
    }
}
