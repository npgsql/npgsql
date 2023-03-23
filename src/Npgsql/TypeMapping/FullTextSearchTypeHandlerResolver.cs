using System;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandlers.FullTextSearchHandlers;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;

namespace Npgsql.TypeMapping;

sealed class FullTextSearchTypeHandlerResolver : TypeHandlerResolver
{
    readonly NpgsqlDatabaseInfo _databaseInfo;

    public FullTextSearchTypeHandlerResolver(NpgsqlConnector connector)
        => _databaseInfo = connector.DatabaseInfo;

    TsQueryHandler? _tsQueryHandler;
    TsVectorHandler? _tsVectorHandler;

    public override NpgsqlTypeHandler? ResolveByDataTypeName(string typeName) =>
        typeName switch
        {
            "tsquery" => TsQueryHandler(),
            "tsvector" => TsVectorHandler(),
            _ => null
        };

    public override NpgsqlTypeHandler? ResolveByClrType(Type type) => null;

    NpgsqlTypeHandler TsQueryHandler()  => _tsQueryHandler ??= new TsQueryHandler(PgType("tsquery"));
    NpgsqlTypeHandler TsVectorHandler() => _tsVectorHandler ??= new TsVectorHandler(PgType("tsvector"));

    PostgresType PgType(string pgTypeName) => _databaseInfo.GetPostgresTypeByName(pgTypeName);
}
