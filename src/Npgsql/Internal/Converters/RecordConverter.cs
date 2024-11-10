using System;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal.Postgres;

namespace Npgsql.Internal.Converters;

sealed class RecordConverter<T>(PgSerializerOptions options, Func<object[], T>? factory = null) : PgStreamingConverter<T>
{
    static bool IsObjectArrayRecord => typeof(T) == typeof(object[]);

    public override T Read(PgReader reader)
        => Read(async: false, reader, CancellationToken.None).GetAwaiter().GetResult();

    public override ValueTask<T> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
        => Read(async: true, reader, cancellationToken);

    async ValueTask<T> Read(bool async, PgReader reader, CancellationToken cancellationToken)
    {
        if (reader.ShouldBuffer(sizeof(int)))
            await reader.Buffer(async, sizeof(int), cancellationToken).ConfigureAwait(false);
        var fieldCount = reader.ReadInt32();
        var result = new object[fieldCount];
        for (var i = 0; i < fieldCount; i++)
        {
            if (reader.ShouldBuffer(sizeof(uint) + sizeof(int)))
                await reader.Buffer(async, sizeof(uint) + sizeof(int), cancellationToken).ConfigureAwait(false);

            var typeOid = reader.ReadUInt32();
            var length = reader.ReadInt32();

            // Note that we leave .NET nulls in the object array rather than DBNull.
            if (length == -1)
                continue;

            var postgresType =
                options.DatabaseInfo.GetPostgresType(typeOid).GetRepresentationalType()
                ?? throw new NotSupportedException($"Reading isn't supported for record field {i} (unknown type OID {typeOid}");
            var pgTypeId = options.ToCanonicalTypeId(postgresType);

            // TODO resolve based on types expected by _factory (pass in a Type[] during construcion)
            // Only allow object polymorphism for object[] records, valuetuple records are always strongly typed.
            var typeInfo = (IsObjectArrayRecord ? options.GetTypeInfo(typeof(object), pgTypeId) : options.GetDefaultTypeInfo(pgTypeId))
                           ?? throw new NotSupportedException(
                               $"Reading isn't supported for record field {i} (PG type '{postgresType.DisplayName}'");

            var converterInfo = typeInfo.Bind(new Field("?", pgTypeId, -1), DataFormat.Binary);
            var scope = await reader.BeginNestedRead(async, length, converterInfo.BufferRequirement, cancellationToken).ConfigureAwait(false);
            try
            {
                result[i] = await converterInfo.Converter.ReadAsObject(async, reader, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                if (async)
                    await scope.DisposeAsync().ConfigureAwait(false);
                else
                    scope.Dispose();
            }
        }

        return factory is null ? (T)(object)result : factory(result);
    }

    public override Size GetSize(SizeContext context, T value, ref object? writeState)
        => throw new NotSupportedException();

    public override void Write(PgWriter writer, T value)
        => throw new NotSupportedException();

    public override ValueTask WriteAsync(PgWriter writer, T value, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();
}
