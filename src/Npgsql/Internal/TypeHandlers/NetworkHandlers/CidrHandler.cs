using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using Npgsql.TypeMapping;
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
    /// should be considered somewhat unstable, and  may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    public class CidrHandler : NpgsqlSimpleTypeHandler<(IPAddress Address, int Subnet)>, INpgsqlSimpleTypeHandler<NpgsqlInet>
    {
        /// <inheritdoc />
        public CidrHandler(PostgresType postgresType) : base(postgresType) {}

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

        protected override int ValidateObjectAndGetLength(object value, NpgsqlParameter? parameter)
            => value switch
            {
                ValueTuple<IPAddress, int> converted => ((INpgsqlSimpleTypeHandler<(IPAddress, int)>)this).ValidateAndGetLength(converted, parameter),
                NpgsqlInet converted => ((INpgsqlSimpleTypeHandler<NpgsqlInet>)this).ValidateAndGetLength(converted, parameter),

                DBNull => -1,
                null => -1,
                _ => throw new InvalidCastException($"Can't write CLR type {value.GetType()} with handler type CidrHandler")
            };

        public override Task WriteObjectWithLength(object value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => value switch
            {
                ValueTuple<IPAddress, int> converted => WriteWithLengthInternal(converted, buf, lengthCache, parameter, async, cancellationToken),
                NpgsqlInet converted => WriteWithLengthInternal(converted, buf, lengthCache, parameter, async, cancellationToken),

                DBNull => WriteWithLengthInternal(DBNull.Value, buf, lengthCache, parameter, async, cancellationToken),
                null => WriteWithLengthInternal(DBNull.Value, buf, lengthCache, parameter, async, cancellationToken),
                _ => throw new InvalidCastException($"Can't write CLR type {value.GetType()} with handler type CidrHandler")
            };
    }
}
