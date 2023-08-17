using System;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal.Postgres;

namespace Npgsql.Internal.Converters;

sealed class ObjectArrayRecordConverter<T> : PgStreamingConverter<T>
{
    readonly PgSerializerOptions _serializerOptions;
    readonly Func<object[], T>? _factory;

    public ObjectArrayRecordConverter(PgSerializerOptions serializerOptions, Func<object[], T>? factory = null)
    {
        _serializerOptions = serializerOptions;
        _factory = factory;
    }

    public override T Read(PgReader reader)
        => Read(async: false, reader, CancellationToken.None).GetAwaiter().GetResult();

    public override ValueTask<T> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
        => Read(async: true, reader, cancellationToken);

    async ValueTask<T> Read(bool async, PgReader reader, CancellationToken cancellationToken)
    {
        if (reader.ShouldBuffer(sizeof(int)))
            await reader.BufferData(async, sizeof(int), cancellationToken).ConfigureAwait(false);
        var fieldCount = reader.ReadInt32();
        var result = new object[fieldCount];
        for (var i = 0; i < fieldCount; i++)
        {
            if (reader.ShouldBuffer(sizeof(uint) + sizeof(int)))
                await reader.BufferData(async, sizeof(uint) + sizeof(int), cancellationToken).ConfigureAwait(false);

            var typeOid = reader.ReadUInt32();
            var length = reader.ReadInt32();

            // Note that we leave .NET nulls in the object array rather than DBNull.
            if (length == -1)
                continue;

            var postgresType =
                _serializerOptions.TypeCatalog.GetPgType((Oid)typeOid).GetRepresentationalType()
                ?? throw new NotSupportedException($"Reading isn't supported for record field {i} (unknown type OID {typeOid}");

            var typeInfo = _serializerOptions.GetObjectOrDefaultTypeInfo(postgresType)
                           ?? throw new NotSupportedException(
                               $"Reading isn't supported for record field {i} (PG type '{postgresType.DisplayName}'");
            var resolution = typeInfo.GetConcreteResolution();
            if (typeInfo.GetBufferRequirements(resolution.Converter, DataFormat.Binary) is not { } bufferRequirements)
                throw new NotSupportedException($"Resolved record field converter '{resolution.Converter.GetType()}' has to support the binary format to be compatible.");

            var scope = await reader.BeginNestedRead(async, length, bufferRequirements.Read, cancellationToken).ConfigureAwait(false);
            try
            {
                result[i] = await resolution.Converter.ReadAsObject(async, reader, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                if (async)
                    await scope.DisposeAsync().ConfigureAwait(false);
                else
                    scope.Dispose();
            }
        }

        return _factory is null ? (T)(object)result : _factory(result);
    }

    public override Size GetSize(SizeContext context, T value, ref object? writeState)
        => throw new NotSupportedException();

    public override void Write(PgWriter writer, T value)
        => throw new NotSupportedException();

    public override ValueTask WriteAsync(PgWriter writer, T value, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();
}
