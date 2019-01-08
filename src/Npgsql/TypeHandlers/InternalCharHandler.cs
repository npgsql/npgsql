using JetBrains.Annotations;
using Npgsql.BackendMessages;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers
{
    /// <summary>
    /// Type handler for the Postgresql "char" type, used only internally
    /// </summary>
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-character.html
    /// </remarks>
    [TypeMapping("char", NpgsqlDbType.InternalChar)]
    class InternalCharHandler : NpgsqlSimpleTypeHandler<char>,
        INpgsqlSimpleTypeHandler<byte>, INpgsqlSimpleTypeHandler<short>, INpgsqlSimpleTypeHandler<int>, INpgsqlSimpleTypeHandler<long>
    {
        #region Read

        public override char Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
            => (char)buf.ReadByte();

        byte INpgsqlSimpleTypeHandler<byte>.Read(NpgsqlReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => buf.ReadByte();

        short INpgsqlSimpleTypeHandler<short>.Read(NpgsqlReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => buf.ReadByte();

        int INpgsqlSimpleTypeHandler<int>.Read(NpgsqlReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => buf.ReadByte();

        long INpgsqlSimpleTypeHandler<long>.Read(NpgsqlReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
            => buf.ReadByte();

        #endregion

        #region Write

        public override int ValidateAndGetLength(char value, NpgsqlParameter parameter) => 1;
        public int ValidateAndGetLength(byte value, NpgsqlParameter parameter)          => 1;
        public int ValidateAndGetLength(short value, NpgsqlParameter parameter)         => 1;
        public int ValidateAndGetLength(int value, NpgsqlParameter parameter)           => 1;
        public int ValidateAndGetLength(long value, NpgsqlParameter parameter)          => 1;

        public override void Write(char value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => buf.WriteByte(checked((byte)value));

        public void Write(byte value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => buf.WriteByte(value);

        public void Write(short value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => buf.WriteByte(checked((byte)value));

        public void Write(int value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => buf.WriteByte(checked((byte)value));

        public void Write(long value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => buf.WriteByte(checked((byte)value));

        #endregion
    }
}
