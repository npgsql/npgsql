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
        public override NpgsqlTypeHandler CreateNonGeneric(PostgresType pgType, NpgsqlConnection conn)
            => new OIDVectorHandler(pgType, conn.Connector!.TypeMapper.DatabaseInfo.ByName["oid"]
                                    ?? throw new NpgsqlException("Two types called 'oid' defined in the database"));

        public override Type DefaultValueType => typeof(uint[]);
    }

    /// <summary>
    /// An OIDVector is simply a regular array of uints, with the sole exception that its lower bound must
    /// be 0 (we send 1 for regular arrays).
    /// </summary>
    class OIDVectorHandler : ArrayHandler<uint>
    {
        public OIDVectorHandler(PostgresType oidvectorType, PostgresType oidType)
            : base(oidvectorType, new UInt32Handler(oidType), ArrayNullabilityMode.Never, 0) { }

        public override ArrayHandler CreateArrayHandler(PostgresArrayType arrayBackendType, ArrayNullabilityMode arrayNullabilityMode)
            => new ArrayHandler<ArrayHandler<uint>>(arrayBackendType, this, arrayNullabilityMode);
    }
}
