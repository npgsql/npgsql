using System;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;

namespace Npgsql.Internal.TypeHandlers
{
    /// <remarks>
    /// https://www.postgresql.org/docs/current/static/datatype-boolean.html
    /// </remarks>
    class VoidHandler : NpgsqlSimpleTypeHandler<DBNull>
    {
        public VoidHandler(PostgresType pgType) : base(pgType) {}

        public override DBNull Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
            => DBNull.Value;

        public override int ValidateAndGetLength(DBNull value, NpgsqlParameter? parameter)
            => throw new NotSupportedException();

        public override void Write(DBNull value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
            => throw new NotSupportedException();

        public override int ValidateObjectAndGetLength(object? value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => value switch
            {
                DBNull => 0,
                null => 0,
                _ => throw new InvalidCastException($"Can't write CLR type {value.GetType()} with handler type {nameof(VoidHandler)}")
            };

        public override Task WriteObjectWithLength(object? value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => value switch
            {
                DBNull => WriteWithLength(DBNull.Value, buf, lengthCache, parameter, async, cancellationToken),
                null => WriteWithLength(DBNull.Value, buf, lengthCache, parameter, async, cancellationToken),
                _ => throw new InvalidCastException($"Can't write CLR type {value.GetType()} with handler type {nameof(VoidHandler)}")
            };
    }
}
