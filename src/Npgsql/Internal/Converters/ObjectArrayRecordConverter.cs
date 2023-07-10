using System;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal.Postgres;

namespace Npgsql.Internal.Converters;

sealed class ObjectArrayRecordConverter : PgStreamingConverter<object[]>
{
    readonly PgSerializerOptions _serializerOptions;

    public ObjectArrayRecordConverter(PgSerializerOptions serializerOptions)
        => _serializerOptions = serializerOptions;

    public override object[] Read(PgReader reader)
        => Read(async: false, reader, CancellationToken.None).GetAwaiter().GetResult();

    public override ValueTask<object[]> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
        => Read(async: true, reader, cancellationToken);

    async ValueTask<object[]> Read(bool async, PgReader reader, CancellationToken cancellationToken)
    {
        if (reader.ShouldBuffer(sizeof(int)))
            await reader.BufferData(async, sizeof(int), cancellationToken).ConfigureAwait(false);
        var fieldCount = reader.ReadInt32();
        var result = new object[fieldCount];
        var recordSize = reader.CurrentSize;

        for (var i = 0; i < fieldCount; i++)
        {
            reader.Current.Size = recordSize;
            if (reader.ShouldBuffer(sizeof(uint) + sizeof(int)))
                await reader.BufferData(async, sizeof(uint) + sizeof(int), cancellationToken).ConfigureAwait(false);

            var typeOid = reader.ReadUInt32();
            var fieldLen = reader.ReadInt32();

            // Note that we leave .NET nulls in the object array rather than DBNull.
            if (fieldLen == -1)
                continue;

            var postgresType =
                _serializerOptions.TypeCatalog.GetPgType((Oid)typeOid).GetRepresentationalType()
                ?? throw new NotSupportedException($"Reading isn't supported for record field {i} (unknown type OID {typeOid}");

            var typeInfo = _serializerOptions.GetObjectOrDefaultTypeInfo(postgresType)
                           ?? throw new NotSupportedException(
                               $"Reading isn't supported for record field {i} (PG type '{postgresType.DisplayName}'");
            var converterInfo = typeInfo.Bind(new Field("?", typeInfo.PgTypeId!.Value, -1), DataFormat.Binary);

            reader.Current.Size = fieldLen;
            if (typeInfo.GetBufferRequirements(converterInfo.Converter, DataFormat.Binary) is not { } bufferRequirements)
                throw new NotSupportedException("Record field converter has to support the binary format to be compatible.");

            if (reader.ShouldBuffer(bufferRequirements.Read))
                await reader.BufferData(async, bufferRequirements.Read, cancellationToken).ConfigureAwait(false);

            result[i] = await converterInfo.Converter.ReadAsObject(async, reader, cancellationToken).ConfigureAwait(false);
        }

        return result;
    }

    public override Size GetSize(SizeContext context, object[] value, ref object? writeState)
        => throw new NotSupportedException();

    public override void Write(PgWriter writer, object[] value)
        => throw new NotSupportedException();

    public override ValueTask WriteAsync(PgWriter writer, object[] value, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();
}
