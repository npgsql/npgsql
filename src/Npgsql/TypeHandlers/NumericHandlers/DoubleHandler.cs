using System.Data;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.NumericHandlers
{
    /// <summary>
    /// A type handler for the PostgreSQL double precision data type.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/datatype-numeric.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and  may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    [TypeMapping("double precision", NpgsqlDbType.Double, DbType.Double, typeof(double))]
    public class DoubleHandler : NpgsqlSimpleTypeHandler<double>
    {
        /// <inheritdoc />
        public DoubleHandler(PostgresType postgresType) : base(postgresType) {}

        /// <inheritdoc />
        public override double Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
            => buf.ReadDouble();

        /// <inheritdoc />
        public override int ValidateAndGetLength(double value, NpgsqlParameter? parameter)
            => 8;

        /// <inheritdoc />
        public override void Write(double value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
            => buf.WriteDouble(value);
    }
}
