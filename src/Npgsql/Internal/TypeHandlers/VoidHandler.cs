using System;
using Npgsql.BackendMessages;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using Npgsql.TypeMapping;

namespace Npgsql.Internal.TypeHandlers
{
    /// <remarks>
    /// https://www.postgresql.org/docs/current/static/datatype-boolean.html
    /// </remarks>
    class VoidHandler : NpgsqlSimpleTypeHandler<DBNull>
    {
        public VoidHandler(PostgresType postgresType) : base(postgresType) {}

        public override DBNull Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
            => DBNull.Value;

        public override int ValidateAndGetLength(DBNull value, NpgsqlParameter? parameter)
            => throw new NotSupportedException();

        public override void Write(DBNull value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
            => throw new NotSupportedException();
    }
}
