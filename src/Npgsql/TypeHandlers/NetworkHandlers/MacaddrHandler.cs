using System.Diagnostics;
using System.Net.NetworkInformation;
using Npgsql.BackendMessages;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.NetworkHandlers
{
    /// <summary>
    /// Defines the type handler for macaddr and macaddr8.
    /// </summary>
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-net-types.html
    /// </remarks>
    [TypeMapping("macaddr8", NpgsqlDbType.MacAddr8)]
    [TypeMapping("macaddr", NpgsqlDbType.MacAddr, typeof(PhysicalAddress))]
    class MacaddrHandler : NpgsqlSimpleTypeHandler<PhysicalAddress>
    {
        #region Read

        /// <inheritdoc />
        public override PhysicalAddress Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            Debug.Assert(len == 6 || len == 8);

            var bytes = new byte[len];

            buf.ReadBytes(bytes, 0, len);
            return new PhysicalAddress(bytes);
        }

        #endregion Read

        #region Write

        /// <inheritdoc />
        public override int ValidateAndGetLength(PhysicalAddress value, NpgsqlParameter parameter)
            => value.GetAddressBytes().Length;

        /// <inheritdoc />
        public override void Write(PhysicalAddress value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
        {
            var bytes = value.GetAddressBytes();
            buf.WriteBytes(bytes, 0, bytes.Length);
        }

        #endregion Write
    }
}
