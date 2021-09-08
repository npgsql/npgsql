using Npgsql.BackendMessages;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;

namespace Npgsql.Internal.TypeHandlers.NumericHandlers
{
    /// <summary>
    /// A type handler for PostgreSQL unsigned 64-bit data types. This is only used for internal types.
    /// </summary>
    /// <remarks>
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    public partial class UInt64Handler : NpgsqlSimpleTypeHandler<ulong>
    {
        public UInt64Handler(PostgresType pgType) : base(pgType) {}

        /// <inheritdoc />
        public override ulong Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
            => buf.ReadUInt64();

        /// <inheritdoc />
        public override int ValidateAndGetLength(ulong value, NpgsqlParameter? parameter) => 8;

        /// <inheritdoc />
        public override void Write(ulong value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
            => buf.WriteUInt64(value);
    }
}
