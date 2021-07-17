using System.Diagnostics;
using Npgsql.BackendMessages;
using Npgsql.Internal.TypeHandling;
using NpgsqlTypes;

namespace Npgsql.Internal.TypeHandlers.InternalTypeHandlers
{
    partial class PgXid8Handler : NpgsqlSimpleTypeHandler<NpgsqlTransactionId>
    {
        #region Read

        public override NpgsqlTransactionId Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
        {
            Debug.Assert(len == 8);
            return new NpgsqlTransactionId(buf.ReadUInt64());
        }

        #endregion Read

        #region Write

        public override int ValidateAndGetLength(NpgsqlTransactionId value, NpgsqlParameter? parameter) => 8;

        public override void Write(NpgsqlTransactionId value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
            => buf.WriteUInt64((ulong)value);

        #endregion Write
    }
}
