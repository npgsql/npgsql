using System.Data;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.NumericHandlers
{
    /// <summary>
    /// A type handler for the PostgreSQL real data type.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/datatype-numeric.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and  may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    [TypeMapping("real", NpgsqlDbType.Real, DbType.Single, typeof(float))]
    public class SingleHandler : NpgsqlSimpleTypeHandler<float>, INpgsqlSimpleTypeHandler<double>
    {
        /// <inheritdoc />
        public SingleHandler(PostgresType postgresType) : base(postgresType) {}

        #region Read

        /// <inheritdoc />
        public override float Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
            => buf.ReadSingle();

        double INpgsqlSimpleTypeHandler<double>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
            => Read(buf, len, fieldDescription);

        #endregion Read

        #region Write

        /// <inheritdoc />
        public int ValidateAndGetLength(double value, NpgsqlParameter? parameter)         => 4;
        /// <inheritdoc />
        public override int ValidateAndGetLength(float value, NpgsqlParameter? parameter) => 4;

        /// <inheritdoc />
        public void Write(double value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)         => buf.WriteSingle((float)value);
        /// <inheritdoc />
        public override void Write(float value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter) => buf.WriteSingle(value);

        #endregion Write
    }
}
