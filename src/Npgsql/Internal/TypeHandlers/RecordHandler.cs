using System;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using Npgsql.TypeMapping;

namespace Npgsql.Internal.TypeHandlers
{
    /// <summary>
    /// Type handler for PostgreSQL record types.
    /// </summary>
    /// <remarks>
    /// https://www.postgresql.org/docs/current/static/datatype-pseudo.html
    ///
    /// Encoding (identical to composite):
    /// A 32-bit integer with the number of columns, then for each column:
    /// * An OID indicating the type of the column
    /// * The length of the column(32-bit integer), or -1 if null
    /// * The column data encoded as binary
    /// </remarks>
    partial class RecordHandler : NpgsqlTypeHandler<object[]>
    {
        readonly ConnectorTypeMapper _typeMapper;

        public RecordHandler(PostgresType postgresType, ConnectorTypeMapper typeMapper)
            : base(postgresType)
            => _typeMapper = typeMapper;

        #region Read

        public override async ValueTask<object[]> Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null)
        {
            await buf.Ensure(4, async);
            var fieldCount = buf.ReadInt32();
            var result = new object[fieldCount];

            for (var i = 0; i < fieldCount; i++)
            {
                await buf.Ensure(8, async);
                var typeOID = buf.ReadUInt32();
                var fieldLen = buf.ReadInt32();
                if (fieldLen == -1)  // Null field, simply skip it and leave at default
                    continue;
                result[i] = await _typeMapper.ResolveByOID(typeOID).ReadAsObject(buf, fieldLen, async);
            }

            return result;
        }

        #endregion

        #region Write (unsupported)

        public override int ValidateAndGetLength(object[] value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => throw new NotSupportedException("Can't write record types");

        public override Task Write(object[] value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => throw new NotSupportedException("Can't write record types");

        #endregion
    }
}
