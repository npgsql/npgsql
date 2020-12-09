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
        public override NpgsqlTypeHandler CreateNonGeneric(PostgresType pgType, NpgsqlConnection conn)
            => new Int2VectorHandler(pgType, conn.Connector!.TypeMapper.DatabaseInfo.ByName["smallint"]
                                             ?? throw new NpgsqlException("Two types called 'smallint' defined in the database"));

        public override Type DefaultValueType => typeof(short[]);
    }

    /// <summary>
    /// An int2vector is simply a regular array of shorts, with the sole exception that its lower bound must
    /// be 0 (we send 1 for regular arrays).
    /// </summary>
    class Int2VectorHandler : ArrayHandler<short>
    {
        public Int2VectorHandler(PostgresType arrayPostgresType, PostgresType postgresShortType)
            : base(arrayPostgresType, new Int16Handler(postgresShortType), ArrayNullabilityMode.Never, 0) { }

        public override ArrayHandler CreateArrayHandler(PostgresArrayType arrayBackendType, ArrayNullabilityMode arrayNullabilityMode)
            => new ArrayHandler<ArrayHandler<short>>(arrayBackendType, this, arrayNullabilityMode);
    }
}
