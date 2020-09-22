using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers
{
    /// <summary>
    /// A type handler for the PostgreSQL "char" type, used only internally.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/datatype-character.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and  may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    [TypeMapping("char", NpgsqlDbType.InternalChar)]
    public class InternalCharHandler : NpgsqlSimpleTypeHandler<char>,
        INpgsqlSimpleTypeHandler<byte>, INpgsqlSimpleTypeHandler<short>, INpgsqlSimpleTypeHandler<int>, INpgsqlSimpleTypeHandler<long>
    {
        /// <inheritdoc />
        public InternalCharHandler(PostgresType postgresType) : base(postgresType) {}

        #region Read

        /// <inheritdoc />
        public override char Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
            => (char)buf.ReadByte();

        byte INpgsqlSimpleTypeHandler<byte>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
            => buf.ReadByte();

        short INpgsqlSimpleTypeHandler<short>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
            => buf.ReadByte();

        int INpgsqlSimpleTypeHandler<int>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
            => buf.ReadByte();

        long INpgsqlSimpleTypeHandler<long>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
            => buf.ReadByte();

        #endregion

        #region Write

        /// <inheritdoc />
        public int ValidateAndGetLength(byte value, NpgsqlParameter? parameter)          => 1;

        /// <inheritdoc />
        public override int ValidateAndGetLength(char value, NpgsqlParameter? parameter)
        {
            _ = checked((byte)value);
            return 1;
        }

        /// <inheritdoc />
        public int ValidateAndGetLength(short value, NpgsqlParameter? parameter)
        {
            _ = checked((byte)value);
            return 1;
        }

        /// <inheritdoc />
        public int ValidateAndGetLength(int value, NpgsqlParameter? parameter)
        {
            _ = checked((byte)value);
            return 1;
        }

        /// <inheritdoc />
        public int ValidateAndGetLength(long value, NpgsqlParameter? parameter)
        {
            _ = checked((byte)value);
            return 1;
        }

        /// <inheritdoc />
        public override void Write(char value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter) => buf.WriteByte((byte)value);
        /// <inheritdoc />
        public void Write(byte value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter) => buf.WriteByte(value);
        /// <inheritdoc />
        public void Write(short value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter) => buf.WriteByte((byte)value);
        /// <inheritdoc />
        public void Write(int value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter) => buf.WriteByte((byte)value);
        /// <inheritdoc />
        public void Write(long value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter) => buf.WriteByte((byte)value);

        #endregion
    }
}
