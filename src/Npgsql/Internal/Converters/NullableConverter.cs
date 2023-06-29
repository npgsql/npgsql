using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using static Npgsql.Internal.Converters.AsyncHelpers;

namespace Npgsql.Internal.Converters;

// We don't inherit from PgComposingConverter<T> to reduce type metadata bloat.

// NULL writing is always responsibility of the caller writing the length, so there is not much we do here.
/// Special value converter to be able to use struct converters as System.Nullable converters, it delegates all behavior to the effective converter.
sealed class NullableConverter<T> : PgConverter<T?> where T : struct
{
    readonly PgConverter<T> _effectiveConverter;
    public NullableConverter(PgConverter<T> effectiveConverter)
        : base(effectiveConverter.DbNullPredicateKind is DbNullPredicate.Custom)
        => _effectiveConverter = effectiveConverter;

    protected override bool IsDbNull(T? value)
        => value is null || _effectiveConverter.IsDbNullValue(value.GetValueOrDefault());

    public override bool CanConvert(DataFormat format, out BufferingRequirement bufferingRequirement)
        => _effectiveConverter.CanConvert(format, out bufferingRequirement);

    public override void GetBufferRequirements(DataFormat format, out Size readRequirement, out Size writeRequirement)
        => _effectiveConverter.GetBufferRequirements(format, out readRequirement, out writeRequirement);

    public override T? Read(PgReader reader)
        => _effectiveConverter.Read(reader);

    public override unsafe ValueTask<T?> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
    {
        // Easy if we have all the data, just go sync and return it.
        if (reader.Remaining >= reader.CurrentSize)
            return new(_effectiveConverter.Read(reader));

        // Otherwise we do one additional allocation, this allow us to share state machine codegen for all Ts.
        var source = new CompletionSource<T?>();
        AwaitTask(_effectiveConverter.ReadAsync(reader, cancellationToken).AsTask(), source, new(this, &UnboxAndComplete));
        return source.Task;

        static void UnboxAndComplete(Task task, CompletionSource completionSource)
        {
            Debug.Assert(task is Task<T>);
            Debug.Assert(completionSource is CompletionSource<T?>);
            Unsafe.As<CompletionSource<T?>>(completionSource).SetResult(new ValueTask<T>(Unsafe.As<Task<T>>(task)).Result);
        }
    }

    public override Size GetSize(SizeContext context, [DisallowNull] T? value, ref object? writeState)
        => _effectiveConverter.GetSize(context, value.GetValueOrDefault(), ref writeState);

    public override void Write(PgWriter writer, T? value)
        => _effectiveConverter.Write(writer, value.GetValueOrDefault());

    public override ValueTask WriteAsync(PgWriter writer, T? value, CancellationToken cancellationToken = default)
        => _effectiveConverter.WriteAsync(writer, value.GetValueOrDefault(), cancellationToken);

    private protected override ValueTask<object> ReadAsObject(bool async, PgReader reader, CancellationToken cancellationToken)
        => async
            ? _effectiveConverter.ReadAsObjectAsync(reader, cancellationToken)
            : new(_effectiveConverter.ReadAsObject(reader));

    private protected override ValueTask WriteAsObject(bool async, PgWriter writer, object value, CancellationToken cancellationToken)
    {
        if (async)
            return _effectiveConverter.WriteAsObjectAsync(writer, value, cancellationToken);

        _effectiveConverter.WriteAsObject(writer, value);
        return new();
    }
}


