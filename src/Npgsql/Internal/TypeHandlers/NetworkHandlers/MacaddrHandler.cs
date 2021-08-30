using System.Diagnostics;
using System.Net.NetworkInformation;
using Npgsql.BackendMessages;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;

namespace Npgsql.Internal.TypeHandlers.NetworkHandlers
{
    /// <summary>
    /// A type handler for the PostgreSQL macaddr and macaddr8 data types.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/datatype-net-types.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    public partial class MacaddrHandler : NpgsqlSimpleTypeHandler<PhysicalAddress>
    {
        public MacaddrHandler(PostgresType pgType) : base(pgType) {}

        #region Read

        /// <inheritdoc />
        public override PhysicalAddress Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
        {
            Debug.Assert(len == 6 || len == 8);

            var bytes = new byte[len];

            buf.ReadBytes(bytes, 0, len);
            return new PhysicalAddress(bytes);
        }

        #endregion Read

        #region Write

        /// <inheritdoc />
        public override int ValidateAndGetLength(PhysicalAddress value, NpgsqlParameter? parameter)
            => value.GetAddressBytes().Length;

        /// <inheritdoc />
        public override void Write(PhysicalAddress value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
        {
            var bytes = value.GetAddressBytes();
            buf.WriteBytes(bytes, 0, bytes.Length);
        }

        #endregion Write
    }
}
