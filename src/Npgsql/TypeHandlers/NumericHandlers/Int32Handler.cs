using System.Data;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.NumericHandlers
{
    /// <summary>
    /// A type handler for the PostgreSQL integer data type.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/datatype-numeric.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and  may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    [TypeMapping("integer", NpgsqlDbType.Integer, DbType.Int32, typeof(int))]
    public class Int32Handler : NpgsqlSimpleTypeHandler<int>,
        INpgsqlSimpleTypeHandler<byte>, INpgsqlSimpleTypeHandler<short>, INpgsqlSimpleTypeHandler<long>,
        INpgsqlSimpleTypeHandler<float>, INpgsqlSimpleTypeHandler<double>, INpgsqlSimpleTypeHandler<decimal>
    {
        /// <inheritdoc />
        public Int32Handler(PostgresType postgresType) : base(postgresType) {}

        #region Read

        /// <inheritdoc />
        public override int Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
            => buf.ReadInt32();

        byte INpgsqlSimpleTypeHandler<byte>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
            => checked((byte)Read(buf, len, fieldDescription));

        short INpgsqlSimpleTypeHandler<short>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
            => checked((short)Read(buf, len, fieldDescription));

        long INpgsqlSimpleTypeHandler<long>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
            => Read(buf, len, fieldDescription);

        float INpgsqlSimpleTypeHandler<float>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
            => Read(buf, len, fieldDescription);

        double INpgsqlSimpleTypeHandler<double>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
            => Read(buf, len, fieldDescription);

        decimal INpgsqlSimpleTypeHandler<decimal>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
            => Read(buf, len, fieldDescription);

        #endregion Read

        #region Write

        /// <inheritdoc />
        public override int ValidateAndGetLength(int value, NpgsqlParameter? parameter) => 4;
        /// <inheritdoc />
        public int ValidateAndGetLength(short value, NpgsqlParameter? parameter)        => 4;
        /// <inheritdoc />
        public int ValidateAndGetLength(byte value, NpgsqlParameter? parameter)         => 4;
        /// <inheritdoc />
        public int ValidateAndGetLength(decimal value, NpgsqlParameter? parameter)      => 4;

        /// <inheritdoc />
        public int ValidateAndGetLength(long value, NpgsqlParameter? parameter)
        {
            _ = checked((int)value);
            return 4;
        }

        /// <inheritdoc />
        public int ValidateAndGetLength(float value, NpgsqlParameter? parameter)
        {
            _ = checked((int)value);
            return 4;
        }

        /// <inheritdoc />
        public int ValidateAndGetLength(double value, NpgsqlParameter? parameter)
        {
            _ = checked((int)value);
            return 4;
        }

        /// <inheritdoc />
        public override void Write(int value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter) => buf.WriteInt32(value);
        /// <inheritdoc />
        public void Write(short value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)        => buf.WriteInt32(value);
        /// <inheritdoc />
        public void Write(long value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)         => buf.WriteInt32((int)value);
        /// <inheritdoc />
        public void Write(byte value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)         => buf.WriteInt32(value);
        /// <inheritdoc />
        public void Write(float value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)        => buf.WriteInt32((int)value);
        /// <inheritdoc />
        public void Write(double value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)       => buf.WriteInt32((int)value);
        /// <inheritdoc />
        public void Write(decimal value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)      => buf.WriteInt32((int)value);

        #endregion Write
    }
}
