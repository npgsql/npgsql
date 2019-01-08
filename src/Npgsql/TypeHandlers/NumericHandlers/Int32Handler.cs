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
    [TypeMapping("integer", NpgsqlDbType.Integer, DbType.Int32, typeof(int))]
    class Int32Handler : NpgsqlSimpleTypeHandler<int>,
        INpgsqlSimpleTypeHandler<byte>, INpgsqlSimpleTypeHandler<short>, INpgsqlSimpleTypeHandler<long>,
        INpgsqlSimpleTypeHandler<float>, INpgsqlSimpleTypeHandler<double>, INpgsqlSimpleTypeHandler<decimal>
    {
        #region Read

        public override int Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
            => buf.ReadInt32();

        byte INpgsqlSimpleTypeHandler<byte>.Read(NpgsqlReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => checked((byte)Read(buf, len, fieldDescription));

        short INpgsqlSimpleTypeHandler<short>.Read(NpgsqlReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => checked((short)Read(buf, len, fieldDescription));

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

        public override int ValidateAndGetLength(int value, NpgsqlParameter parameter) => 4;
        public int ValidateAndGetLength(short value, NpgsqlParameter parameter)        => 4;
        public int ValidateAndGetLength(long value, NpgsqlParameter parameter)         => 4;
        public int ValidateAndGetLength(float value, NpgsqlParameter parameter)        => 4;
        public int ValidateAndGetLength(double value, NpgsqlParameter parameter)       => 4;
        public int ValidateAndGetLength(decimal value, NpgsqlParameter parameter)      => 4;
        public int ValidateAndGetLength(byte value, NpgsqlParameter parameter)         => 4;

        public override void Write(int value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => buf.WriteInt32(value);
        public void Write(short value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => buf.WriteInt32(value);
        public void Write(long value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => buf.WriteInt32(checked((int)value));
        public void Write(byte value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => buf.WriteInt32(value);
        public void Write(float value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => buf.WriteInt32(checked((int)value));
        public void Write(double value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => buf.WriteInt32(checked((int)value));
        public void Write(decimal value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => buf.WriteInt32((int)value);

        #endregion Write
    }
}
