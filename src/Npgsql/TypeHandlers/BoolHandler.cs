using System.Data;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers
{
    /// <summary>
    /// A type handler for the PostgreSQL bool data type.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/datatype-boolean.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and  may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    [TypeMapping("boolean", NpgsqlDbType.Boolean, DbType.Boolean, typeof(bool))]
    public class BoolHandler : NpgsqlSimpleTypeHandler<bool>
    {
        /// <inheritdoc />
        public BoolHandler(PostgresType postgresType) : base(postgresType) {}

        /// <inheritdoc />
        public override bool Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
            => buf.ReadByte() != 0;

        /// <inheritdoc />
        public override int ValidateAndGetLength(bool value, NpgsqlParameter? parameter)
            => 1;

        /// <inheritdoc />
        public override void Write(bool value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
            => buf.WriteByte(value ? (byte)1 : (byte)0);
    }
}
