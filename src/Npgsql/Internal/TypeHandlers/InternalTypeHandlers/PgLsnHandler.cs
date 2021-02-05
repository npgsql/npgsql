using System.Diagnostics;
using Npgsql.BackendMessages;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.Internal.TypeHandlers.InternalTypeHandlers
{
    [TypeMapping("pg_lsn", NpgsqlDbType.PgLsn, typeof(NpgsqlLogSequenceNumber))]
    class PgLsnHandler : NpgsqlSimpleTypeHandler<NpgsqlLogSequenceNumber>
    {
        public PgLsnHandler(PostgresType postgresType) : base(postgresType) {}

        #region Read

        public override NpgsqlLogSequenceNumber Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
        {
            Debug.Assert(len == 8);
            return new NpgsqlLogSequenceNumber(buf.ReadUInt64());
        }

        #endregion Read

        #region Write

        public override int ValidateAndGetLength(NpgsqlLogSequenceNumber value, NpgsqlParameter? parameter) => 8;

        public override void Write(NpgsqlLogSequenceNumber value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
            => buf.WriteUInt64((ulong)value);

        #endregion Write
    }
}
