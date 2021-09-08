using System.Net;
using Npgsql.BackendMessages;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using NpgsqlTypes;

#pragma warning disable 618

namespace Npgsql.Internal.TypeHandlers.NetworkHandlers
{
    /// <summary>
    /// A type handler for the PostgreSQL cidr data type.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/datatype-net-types.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    public partial class CidrHandler : NpgsqlSimpleTypeHandler<(IPAddress Address, int Subnet)>, INpgsqlSimpleTypeHandler<NpgsqlInet>
    {
        public CidrHandler(PostgresType pgType) : base(pgType) {}

        /// <inheritdoc />
        public override (IPAddress Address, int Subnet) Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
            => InetHandler.DoRead(buf, len, fieldDescription, true);

        NpgsqlInet INpgsqlSimpleTypeHandler<NpgsqlInet>.Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription)
        {
            var (address, subnet) = Read(buf, len, fieldDescription);
            return new NpgsqlInet(address, subnet);
        }

        /// <inheritdoc />
        public override int ValidateAndGetLength((IPAddress Address, int Subnet) value, NpgsqlParameter? parameter)
            => InetHandler.GetLength(value.Address);

        /// <inheritdoc />
        public int ValidateAndGetLength(NpgsqlInet value, NpgsqlParameter? parameter)
            => InetHandler.GetLength(value.Address);

        /// <inheritdoc />
        public override void Write((IPAddress Address, int Subnet) value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
            => InetHandler.DoWrite(value.Address, value.Subnet, buf, true);

        /// <inheritdoc />
        public void Write(NpgsqlInet value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
            => InetHandler.DoWrite(value.Address, value.Netmask, buf, true);
    }
}
