using System;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandlers.NumericHandlers;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.InternalTypeHandlers
{
    [TypeMapping("oidvector", NpgsqlDbType.Oidvector)]
    class OIDVectorHandlerFactory : NpgsqlTypeHandlerFactory
    {
        internal override NpgsqlTypeHandler Create(PostgresType pgType, NpgsqlConnection conn)
            => new OIDVectorHandler(conn.Connector.TypeMapper.DatabaseInfo.ByName["oid"])
            {
                PostgresType = pgType
            };

        internal override Type DefaultValueType => null;
    }

    /// <summary>
    /// An OIDVector is simply a regular array of uints, with the sole exception that its lower bound must
    /// be 0 (we send 1 for regular arrays).
    /// </summary>
    class OIDVectorHandler : ArrayHandler<uint>
    {
        public OIDVectorHandler(PostgresType postgresOIDType)
            : base(new UInt32Handler { PostgresType = postgresOIDType }, 0) { }

        public override ArrayHandler CreateArrayHandler(PostgresType arrayBackendType)
            => new ArrayHandler<ArrayHandler<uint>>(this) { PostgresType = arrayBackendType };
    }
}
