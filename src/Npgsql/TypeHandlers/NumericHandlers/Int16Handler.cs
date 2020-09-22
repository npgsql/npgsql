using System.Data;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.NumericHandlers
{
    /// <summary>
    /// A type handler for the PostgreSQL smallint data type.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/datatype-numeric.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and  may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    [TypeMapping("smallint", NpgsqlDbType.Smallint, new[] { DbType.Int16, DbType.Byte, DbType.SByte }, new[] { typeof(short), typeof(byte), typeof(sbyte) }, DbType.Int16)]
    public class Int16Handler : NpgsqlSimpleTypeHandler<short>,
        INpgsqlSimpleTypeHandler<byte>, INpgsqlSimpleTypeHandler<sbyte>, INpgsqlSimpleTypeHandler<int>, INpgsqlSimpleTypeHandler<long>,
        INpgsqlSimpleTypeHandler<float>, INpgsqlSimpleTypeHandler<double>, INpgsqlSimpleTypeHandler<decimal>
    {
        /// <inheritdoc />
        public Int16Handler(PostgresType postgresType) : base(postgresType) {}

        #region Read

        /// <inheritdoc />
        public override short Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
            => buf.ReadInt16();

        byte INpgsqlSimpleTypeHandler<byte>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
            => checked((byte)Read(buf, len, fieldDescription));

        sbyte INpgsqlSimpleTypeHandler<sbyte>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
            => checked((sbyte)Read(buf, len, fieldDescription));

        int INpgsqlSimpleTypeHandler<int>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
            => Read(buf, len, fieldDescription);

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
        public override int ValidateAndGetLength(short value, NpgsqlParameter? parameter) => 2;
        /// <inheritdoc />
        public int ValidateAndGetLength(byte value, NpgsqlParameter? parameter)           => 2;
        /// <inheritdoc />
        public int ValidateAndGetLength(sbyte value, NpgsqlParameter? parameter)          => 2;
        /// <inheritdoc />
        public int ValidateAndGetLength(decimal value, NpgsqlParameter? parameter)        => 2;

        /// <inheritdoc />
        public int ValidateAndGetLength(int value, NpgsqlParameter? parameter)
        {
            _ = checked((short)value);
            return 2;
        }

        /// <inheritdoc />
        public int ValidateAndGetLength(long value, NpgsqlParameter? parameter)
        {
            _ = checked((short)value);
            return 2;
        }

        /// <inheritdoc />
        public int ValidateAndGetLength(float value, NpgsqlParameter? parameter)
        {
            _ = checked((short)value);
            return 2;
        }

        /// <inheritdoc />
        public int ValidateAndGetLength(double value, NpgsqlParameter? parameter)
        {
            _ = checked((short)value);
            return 2;
        }

        /// <inheritdoc />
        public override void Write(short value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter) => buf.WriteInt16(value);
        /// <inheritdoc />
        public void Write(int value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)            => buf.WriteInt16((short)value);
        /// <inheritdoc />
        public void Write(long value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)           => buf.WriteInt16((short)value);
        /// <inheritdoc />
        public void Write(byte value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)           => buf.WriteInt16(value);
        /// <inheritdoc />
        public void Write(sbyte value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)          => buf.WriteInt16(value);
        /// <inheritdoc />
        public void Write(decimal value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)        => buf.WriteInt16((short)value);
        /// <inheritdoc />
        public void Write(double value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)         => buf.WriteInt16((short)value);
        /// <inheritdoc />
        public void Write(float value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)          => buf.WriteInt16((short)value);

        #endregion Write
    }
}
