using System.Data;
using JetBrains.Annotations;
using Npgsql.BackendMessages;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.NumericHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-numeric.html
    /// </remarks>
    [TypeMapping("bigint", NpgsqlDbType.Bigint, DbType.Int64, typeof(long))]
    class Int64Handler : NpgsqlSimpleTypeHandler<long>,
        INpgsqlSimpleTypeHandler<byte>, INpgsqlSimpleTypeHandler<short>, INpgsqlSimpleTypeHandler<int>,
        INpgsqlSimpleTypeHandler<float>, INpgsqlSimpleTypeHandler<double>, INpgsqlSimpleTypeHandler<decimal>
    {
        #region Read

        public override long Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
            => buf.ReadInt64();

        byte INpgsqlSimpleTypeHandler<byte>.Read(NpgsqlReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => checked((byte)Read(buf, len, fieldDescription));

        short INpgsqlSimpleTypeHandler<short>.Read(NpgsqlReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => checked((short)Read(buf, len, fieldDescription));

        int INpgsqlSimpleTypeHandler<int>.Read(NpgsqlReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => checked((int)Read(buf, len, fieldDescription));

        float INpgsqlSimpleTypeHandler<float>.Read(NpgsqlReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => Read(buf, len, fieldDescription);

        double INpgsqlSimpleTypeHandler<double>.Read(NpgsqlReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => Read(buf, len, fieldDescription);

        decimal INpgsqlSimpleTypeHandler<decimal>.Read(NpgsqlReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => Read(buf, len, fieldDescription);

        #endregion Read

        #region Write

        public override int ValidateAndGetLength(long value, NpgsqlParameter parameter) => 8;
        public int ValidateAndGetLength(short value, NpgsqlParameter parameter)         => 8;
        public int ValidateAndGetLength(int value, NpgsqlParameter parameter)           => 8;
        public int ValidateAndGetLength(float value, NpgsqlParameter parameter)         => 8;
        public int ValidateAndGetLength(double value, NpgsqlParameter parameter)        => 8;
        public int ValidateAndGetLength(decimal value, NpgsqlParameter parameter)       => 8;
        public int ValidateAndGetLength(byte value, NpgsqlParameter parameter)          => 8;

        public override void Write(long value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => buf.WriteInt64(value);
        public void Write(short value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => buf.WriteInt64(value);
        public void Write(int value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => buf.WriteInt64(value);
        public void Write(byte value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => buf.WriteInt64(value);
        public void Write(float value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => buf.WriteInt64(checked((long)value));
        public void Write(double value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => buf.WriteInt64(checked((long)value));
        public void Write(decimal value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => buf.WriteInt64((long)value);

        #endregion Write
    }
}
