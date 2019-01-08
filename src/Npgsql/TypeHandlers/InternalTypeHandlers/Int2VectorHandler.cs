using System;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandlers.NumericHandlers;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.InternalTypeHandlers
{
    [TypeMapping("int2vector", NpgsqlDbType.Int2Vector)]
    class Int2VectorHandlerFactory : NpgsqlTypeHandlerFactory
    {
        internal override NpgsqlTypeHandler Create(PostgresType pgType, NpgsqlConnection conn)
            => new Int2VectorHandler(conn.Connector.TypeMapper.DatabaseInfo.ByName["smallint"])
            {
                PostgresType = pgType
            };

        internal override Type DefaultValueType => null;
    }

    /// <summary>
    /// An int2vector is simply a regular array of shorts, with the sole exception that its lower bound must
    /// be 0 (we send 1 for regular arrays).
    /// </summary>
    class Int2VectorHandler : ArrayHandler<short>
    {
        public Int2VectorHandler(PostgresType postgresShortType)
            : base(new Int16Handler { PostgresType = postgresShortType }, 0) { }

        public override ArrayHandler CreateArrayHandler(PostgresType arrayBackendType)
            => new ArrayHandler<ArrayHandler<short>>(this) { PostgresType = arrayBackendType };
    }
}
