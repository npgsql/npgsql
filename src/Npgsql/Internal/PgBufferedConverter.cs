using System;
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

    protected abstract T ReadCore(PgReader reader);
    protected abstract void WriteCore(PgWriter writer, T value);

    protected override Size BindValue(in BindContext context, T value, ref object? writeState)
        => throw new NotSupportedException();

    public sealed override T Read(PgReader reader) => ReadCore(reader);

    public sealed override ValueTask<T> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
        => new(Read(reader));

    internal sealed override ValueTask<object?> ReadAsObject(bool async, PgReader reader, CancellationToken cancellationToken)
        => new(Read(reader));

    public sealed override void Write(PgWriter writer, T value) => WriteCore(writer, value);

    public sealed override ValueTask WriteAsync(PgWriter writer, T value, CancellationToken cancellationToken = default)
    {
        Write(writer, value);
        return new();
    }

    internal sealed override ValueTask WriteAsObject(bool async, PgWriter writer, object? value, CancellationToken cancellationToken)
    {
        Write(writer, (T)value!);
        return new();
    }
}
