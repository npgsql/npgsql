using System.Net;
using JetBrains.Annotations;
using Npgsql.BackendMessages;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

#pragma warning disable 618

namespace Npgsql.TypeHandlers.NetworkHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-net-types.html
    /// </remarks>
    [TypeMapping("cidr", NpgsqlDbType.Cidr)]
    class CidrHandler : NpgsqlSimpleTypeHandler<(IPAddress Address, int Subnet)>, INpgsqlSimpleTypeHandler<NpgsqlInet>
    {
        public override (IPAddress Address, int Subnet) Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
            => InetHandler.DoRead(buf, len, fieldDescription, true);

        NpgsqlInet INpgsqlSimpleTypeHandler<NpgsqlInet>.Read(NpgsqlReadBuffer buf, int len, [CanBeNull] FieldDescription fieldDescription)
        {
            var (address, subnet) = Read(buf, len, fieldDescription);
            return new NpgsqlInet(address, subnet);
        }

        public override int ValidateAndGetLength((IPAddress Address, int Subnet) value, NpgsqlParameter parameter)
            => InetHandler.GetLength(value.Address);

        public int ValidateAndGetLength(NpgsqlInet value, NpgsqlParameter parameter)
            => InetHandler.GetLength(value.Address);

        public override void Write((IPAddress Address, int Subnet) value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => InetHandler.DoWrite(value.Address, value.Subnet, buf, true);

        public void Write(NpgsqlInet value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => InetHandler.DoWrite(value.Address, value.Netmask, buf, true);
    }
}
