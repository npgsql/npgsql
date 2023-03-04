using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandling;
using Npgsql.Internal.TypeMapping;
using Npgsql.PostgresTypes;

namespace Npgsql.TypeMapping;

sealed class UserMappedTypeHandlerResolver : TypeHandlerResolver
{
    readonly NpgsqlConnector _connector;
    readonly IUserTypeMapping _mapping;
    readonly uint? _typeOID;
    readonly TypeMappingInfo? _typeMapping;

    NpgsqlTypeHandler? _handler;

    public UserMappedTypeHandlerResolver(NpgsqlConnector connector, IUserTypeMapping mapping)
    {
        _connector = connector;
        _mapping = mapping;

        if (connector.DatabaseInfo.TryGetPostgresTypeByName(mapping.PgTypeName, out var pgType))
        {
            _typeOID = pgType.OID;
            _typeMapping = new(npgsqlDbType: null, pgType.Name, mapping.ClrType);
        }
    }

    public override NpgsqlTypeHandler? ResolveByDataTypeName(string typeName)
    {
        if (_connector.DatabaseInfo.TryGetPostgresTypeByName(_mapping.PgTypeName, out var pgType)
            && (pgType.FullName == typeName || pgType.Name == typeName))
            return GetHandler(pgType);

        return null;
    }

    public override NpgsqlTypeHandler? ResolveByClrType(Type type)
    {
        if (_connector.DatabaseInfo.TryGetPostgresTypeByName(_mapping.PgTypeName, out var pgType) && _mapping.ClrType == type)
            return GetHandler(pgType);

        return null;
    }

    NpgsqlTypeHandler GetHandler(PostgresType pgType) => _handler ??= _mapping.CreateHandler(pgType, _connector);

    public override NpgsqlTypeHandler? ResolveByPostgresType(PostgresType type)
        => ResolveByDataTypeName(type.FullName);

    public override TypeMappingInfo? GetMappingByPostgresType(PostgresType type)
    {
        if (_typeOID != type.OID) 
            return null;
        Debug.Assert(_typeMapping is not null);
        return _typeMapping;
    }
}
