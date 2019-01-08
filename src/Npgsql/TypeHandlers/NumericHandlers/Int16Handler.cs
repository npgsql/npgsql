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
    [TypeMapping("smallint", NpgsqlDbType.Smallint, new[] { DbType.Int16, DbType.Byte, DbType.SByte }, new[] { typeof(short), typeof(byte), typeof(sbyte) }, DbType.Int16)]
    class Int16Handler : NpgsqlSimpleTypeHandler<short>,
        INpgsqlSimpleTypeHandler<byte>, INpgsqlSimpleTypeHandler<sbyte>, INpgsqlSimpleTypeHandler<int>, INpgsqlSimpleTypeHandler<long>,
        INpgsqlSimpleTypeHandler<float>, INpgsqlSimpleTypeHandler<double>, INpgsqlSimpleTypeHandler<decimal>
    {
        #region Read

        public override short Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
            => buf.ReadInt16();

        byte INpgsqlSimpleTypeHandler<byte>.Read(NpgsqlReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => checked((byte)Read(buf, len, fieldDescription));

        sbyte INpgsqlSimpleTypeHandler<sbyte>.Read(NpgsqlReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => checked((sbyte)Read(buf, len, fieldDescription));

        int INpgsqlSimpleTypeHandler<int>.Read(NpgsqlReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => Read(buf, len, fieldDescription);

        long INpgsqlSimpleTypeHandler<long>.Read(NpgsqlReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => Read(buf, len, fieldDescription);

        float INpgsqlSimpleTypeHandler<float>.Read(NpgsqlReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => Read(buf, len, fieldDescription);

        double INpgsqlSimpleTypeHandler<double>.Read(NpgsqlReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => Read(buf, len, fieldDescription);

        decimal INpgsqlSimpleTypeHandler<decimal>.Read(NpgsqlReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => Read(buf, len, fieldDescription);

        #endregion Read

        #region Write

        public override int ValidateAndGetLength(short value, NpgsqlParameter parameter) => 2;
        public int ValidateAndGetLength(int value, NpgsqlParameter parameter)            => 2;
        public int ValidateAndGetLength(long value, NpgsqlParameter parameter)           => 2;
        public int ValidateAndGetLength(byte value, NpgsqlParameter parameter)           => 2;
        public int ValidateAndGetLength(sbyte value, NpgsqlParameter parameter)          => 2;
        public int ValidateAndGetLength(float value, NpgsqlParameter parameter)          => 2;
        public int ValidateAndGetLength(double value, NpgsqlParameter parameter)         => 2;
        public int ValidateAndGetLength(decimal value, NpgsqlParameter parameter)        => 2;

        public override void Write(short value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => buf.WriteInt16(value);
        public void Write(int value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => buf.WriteInt16(checked((short)value));
        public void Write(long value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => buf.WriteInt16(checked((short)value));
        public void Write(byte value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => buf.WriteInt16(value);
        public void Write(sbyte value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => buf.WriteInt16(value);
        public void Write(decimal value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => buf.WriteInt16((short)value);
        public void Write(double value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => buf.WriteInt16(checked((short)value));
        public void Write(float value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => buf.WriteInt16(checked((short)value));

        #endregion Write
    }
}
