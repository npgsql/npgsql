using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using static Npgsql.Internal.Converters.AsyncHelpers;

namespace Npgsql.Internal.Converters;

// We don't inherit from PgComposingConverter<T> to reduce type metadata bloat.

/// A converter to map strongly typed apis onto boxed converter results to produce a strongly typed converter over T.
sealed class CastingConverter<T> : PgConverter<T>
{
    readonly PgConverter _effectiveConverter;
    public CastingConverter(PgConverter effectiveConverter)
        : base(effectiveConverter.DbNullPredicateKind is DbNullPredicate.Custom)
        => _effectiveConverter = effectiveConverter;

    protected override bool IsDbNull(T? value) => _effectiveConverter.IsDbNullValueAsObject(value);

    public override bool CanConvert(DataFormat format, out BufferingRequirement bufferingRequirement)
        => _effectiveConverter.CanConvert(format, out bufferingRequirement);

    public override void GetBufferRequirements(DataFormat format, out Size readRequirement, out Size writeRequirement)
        => _effectiveConverter.GetBufferRequirements(format, out readRequirement, out writeRequirement);

    public override T Read(PgReader reader) => (T)_effectiveConverter.ReadAsObject(reader)!;
    public override unsafe ValueTask<T> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
    {
        // Easy if we have all the data, just go sync and return it.
        if (reader.Remaining >= reader.Current.Size.Value)
            return new((T)_effectiveConverter.ReadAsObject(reader));

        // Otherwise we do one additional allocation, this allow us to share state machine codegen for all Ts.
        var source = new CompletionSource<T>();
        AwaitTask(_effectiveConverter.ReadAsObjectAsync(reader, cancellationToken).AsTask(), source, new(this, &UnboxAndComplete));
        return source.Task;

        static void UnboxAndComplete(Task task, CompletionSource completionSource)
        {
            Debug.Assert(task is Task<object>);
            Debug.Assert(completionSource is CompletionSource<T>);
            Unsafe.As<CompletionSource<T>>(completionSource).SetResult((T)new ValueTask<object>(Unsafe.As<Task<object>>(task)).Result);
        }
    }

    public override Size GetSize(SizeContext context, T value, ref object? writeState)
        => _effectiveConverter.GetSizeAsObject(context, value!, ref writeState);

    public override void Write(PgWriter writer, T value)
        => _effectiveConverter.WriteAsObject(writer, value!);

    public override ValueTask WriteAsync(PgWriter writer, T value, CancellationToken cancellationToken = default)
        => _effectiveConverter.WriteAsObjectAsync(writer, value!, cancellationToken);

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
