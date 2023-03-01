using Npgsql.Internal.TypeHandlers.NumericHandlers;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;

namespace Npgsql.Internal.TypeHandlers.InternalTypeHandlers;

/// <summary>
/// An int2vector is simply a regular array of shorts, with the sole exception that its lower bound must
/// be 0 (we send 1 for regular arrays).
/// </summary>
sealed class Int2VectorHandler : ArrayHandler
{
    public Int2VectorHandler(PostgresType arrayPostgresType, PostgresType postgresShortType)
        : base(arrayPostgresType, new Int16Handler(postgresShortType), ArrayNullabilityMode.Never, 0) { }

    public override NpgsqlTypeHandler CreateArrayHandler(PostgresArrayType pgArrayType, ArrayNullabilityMode arrayNullabilityMode)
        => new ArrayHandler(pgArrayType, this, arrayNullabilityMode);
}
