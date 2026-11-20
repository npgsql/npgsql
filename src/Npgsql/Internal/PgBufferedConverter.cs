using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql.Internal;

[Experimental(NpgsqlDiagnostics.ConvertersExperimental)]
public abstract class PgBufferedConverter<T> : PgConverter<T>
{
    protected PgBufferedConverter()
    {
    }

    [Obsolete("Call the parameterless constructor and set HandleDbNull directly.")]
    protected PgBufferedConverter(bool customDbNullPredicate) => HandleDbNull = customDbNullPredicate;

    [Obsolete("Override Read instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual T ReadCore(PgReader reader) => throw new NotSupportedException("Override Read instead of ReadCore.");

    [Obsolete("Override Write instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual void WriteCore(PgWriter writer, T value) => throw new NotSupportedException("Override Write instead of WriteCore.");

    protected override Size BindValue(in BindContext context, T value, ref object? writeState)
        => throw new NotSupportedException();

#pragma warning disable CS0618 // Type or member is obsolete
    public override T Read(PgReader reader) => ReadCore(reader);
#pragma warning restore CS0618 // Type or member is obsolete

    public sealed override ValueTask<T> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
        => new(Read(reader));

    internal override ValueTask<object?> ReadAsObject(bool async, PgReader reader, CancellationToken cancellationToken)
        => new(Read(reader));

#pragma warning disable CS0618 // Type or member is obsolete
    public override void Write(PgWriter writer, T value) => WriteCore(writer, value);
#pragma warning restore CS0618 // Type or member is obsolete

    public sealed override ValueTask WriteAsync(PgWriter writer, T value, CancellationToken cancellationToken = default)
    {
        Write(writer, value);
        return new();
    }

    internal override ValueTask WriteAsObject(bool async, PgWriter writer, object? value, CancellationToken cancellationToken)
    {
        Write(writer, (T)value!);
        return new();
    }
}
