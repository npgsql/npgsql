using System;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandlers;
using Npgsql.Internal.TypeHandling;
using Npgsql.Internal.TypeMapping;
using Npgsql.PostgresTypes;

namespace Npgsql.TypeMapping;

sealed class RecordTypeHandlerResolver : TypeHandlerResolver
{
    readonly TypeMapper _typeMapper;
    readonly NpgsqlDatabaseInfo _databaseInfo;

    RecordHandler? _recordHandler;

    public RecordTypeHandlerResolver(TypeMapper typeMapper, NpgsqlConnector connector)
    {
        _typeMapper = typeMapper;
        _databaseInfo = connector.DatabaseInfo;
    }

    public override NpgsqlTypeHandler? ResolveByDataTypeName(string typeName)
        => typeName == "record" ? GetHandler() : null;

    public override NpgsqlTypeHandler? ResolveByClrType(Type type) => null;

    NpgsqlTypeHandler GetHandler() => _recordHandler ??= new RecordHandler(_databaseInfo.GetPostgresTypeByName("record"), _typeMapper);
}
