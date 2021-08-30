using Npgsql.BackendMessages;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;

namespace Npgsql.Internal.TypeHandlers.NumericHandlers
{
    /// <summary>
    /// A type handler for the PostgreSQL bigint data type.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/datatype-numeric.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    public partial class Int64Handler : NpgsqlSimpleTypeHandler<long>,
        INpgsqlSimpleTypeHandler<byte>, INpgsqlSimpleTypeHandler<short>, INpgsqlSimpleTypeHandler<int>,
        INpgsqlSimpleTypeHandler<float>, INpgsqlSimpleTypeHandler<double>, INpgsqlSimpleTypeHandler<decimal>
    {
        public Int64Handler(PostgresType pgType) : base(pgType) {}

        #region Read

        /// <inheritdoc />
        public override long Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
            => buf.ReadInt64();

        byte INpgsqlSimpleTypeHandler<byte>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
            => checked((byte)Read(buf, len, fieldDescription));

        short INpgsqlSimpleTypeHandler<short>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
            => checked((short)Read(buf, len, fieldDescription));

        int INpgsqlSimpleTypeHandler<int>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
            => checked((int)Read(buf, len, fieldDescription));

        float INpgsqlSimpleTypeHandler<float>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
            => Read(buf, len, fieldDescription);

        double INpgsqlSimpleTypeHandler<double>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
            => Read(buf, len, fieldDescription);

        decimal INpgsqlSimpleTypeHandler<decimal>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
            => Read(buf, len, fieldDescription);

        #endregion Read

        #region Write

        /// <inheritdoc />
        public override int ValidateAndGetLength(long value, NpgsqlParameter? parameter) => 8;
        /// <inheritdoc />
        public int ValidateAndGetLength(int value, NpgsqlParameter? parameter)           => 8;
        /// <inheritdoc />
        public int ValidateAndGetLength(short value, NpgsqlParameter? parameter)         => 8;
        /// <inheritdoc />
        public int ValidateAndGetLength(byte value, NpgsqlParameter? parameter)          => 8;
        /// <inheritdoc />
        public int ValidateAndGetLength(decimal value, NpgsqlParameter? parameter)       => 8;

        /// <inheritdoc />
        public int ValidateAndGetLength(float value, NpgsqlParameter? parameter)
        {
            _ = checked((long)value);
            return 8;
        }

        /// <inheritdoc />
        public int ValidateAndGetLength(double value, NpgsqlParameter? parameter)
        {
            _ = checked((long)value);
            return 8;
        }

        /// <inheritdoc />
        public override void Write(long value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter) => buf.WriteInt64(value);
        /// <inheritdoc />
        public void Write(short value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)         => buf.WriteInt64(value);
        /// <inheritdoc />
        public void Write(int value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)           => buf.WriteInt64(value);
        /// <inheritdoc />
        public void Write(byte value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)          => buf.WriteInt64(value);
        /// <inheritdoc />
        public void Write(float value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)         => buf.WriteInt64((long)value);
        /// <inheritdoc />
        public void Write(double value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)        => buf.WriteInt64((long)value);
        /// <inheritdoc />
        public void Write(decimal value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)       => buf.WriteInt64((long)value);

        #endregion Write
    }
}
