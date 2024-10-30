using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql.Internal;

[Experimental(NpgsqlDiagnostics.ConvertersExperimental)]
public abstract class PgBufferedConverter<T>(bool customDbNullPredicate = false) : PgConverter<T>(customDbNullPredicate)
{
    protected abstract T ReadCore(PgReader reader);
    protected abstract void WriteCore(PgWriter writer, T value);

    public override Size GetSize(SizeContext context, T value, ref object? writeState)
        => throw new NotSupportedException();

    public sealed override T Read(PgReader reader)
    {
        // We check FieldAtStart to speed up simple value reads, as field level buffering was handled by reader.StartRead() already.
        if (!reader.FieldAtStart && reader.ShouldBufferCurrent())
            ThrowIORequired(reader.CurrentBufferRequirement);

        return ReadCore(reader);
    }

    public sealed override ValueTask<T> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
        => new(Read(reader));

    internal sealed override ValueTask<object> ReadAsObject(bool async, PgReader reader, CancellationToken cancellationToken)
        => new(Read(reader)!);

    public sealed override void Write(PgWriter writer, T value)
    {
        if (!writer.BufferingWrite && writer.ShouldFlush(writer.CurrentBufferRequirement))
            ThrowIORequired(writer.CurrentBufferRequirement);

        WriteCore(writer, value);
    }

    public sealed override ValueTask WriteAsync(PgWriter writer, [DisallowNull] T value, CancellationToken cancellationToken = default)
    {
        Write(writer, value);
        return new();
    }

    internal sealed override ValueTask WriteAsObject(bool async, PgWriter writer, object value, CancellationToken cancellationToken)
    {
        Write(writer, (T)value);
        return new();
    }
}
