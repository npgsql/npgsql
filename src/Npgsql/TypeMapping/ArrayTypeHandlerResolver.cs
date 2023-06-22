using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandlers.InternalTypeHandlers;
using Npgsql.Internal.TypeHandling;
using Npgsql.Internal.TypeMapping;
using Npgsql.PostgresTypes;
using NpgsqlTypes;

namespace Npgsql.TypeMapping;

sealed class ArrayTypeHandlerResolver : TypeHandlerResolver
{
    readonly TypeMapper _typeMapper;
    readonly NpgsqlConnector _connector;
    readonly NpgsqlDatabaseInfo _databaseInfo;

    // Internal types
    Int2VectorHandler? _int2VectorHandler;
    OIDVectorHandler? _oidVectorHandler;

    // Complex type handlers over timestamp/timestamptz (because DateTime is value-dependent)
    NpgsqlTypeHandler? _timestampArrayHandler;
    NpgsqlTypeHandler? _timestampTzArrayHandler;

    public ArrayTypeHandlerResolver(TypeMapper typeMapper, NpgsqlConnector connector)
    {
        _typeMapper = typeMapper;
        _connector = connector;
        _databaseInfo = connector.DatabaseInfo;
    }

    public override NpgsqlTypeHandler? ResolveByDataTypeName(string typeName)
        => typeName switch
        {
            "int2vector" => Int2VectorHandler(),
            "oidvector" => OidVectorHandler(),

            _ => _databaseInfo.TryGetPostgresTypeByName(typeName, out var pgType) && pgType is PostgresArrayType pgArrayType
                ? _typeMapper.ResolveByOID(pgArrayType.Element.OID)
                    .CreateArrayHandler(pgArrayType, _connector.Settings.ArrayNullabilityMode)
                : null
        };

    public override NpgsqlTypeHandler? ResolveByNpgsqlDbType(NpgsqlDbType npgsqlDbType)
    {
        if (!npgsqlDbType.HasFlag(NpgsqlDbType.Array))
            return null;

        var elementHandler = _typeMapper.ResolveByNpgsqlDbType(npgsqlDbType & ~NpgsqlDbType.Array);

        return elementHandler.PostgresType.Array is { } pgArrayType
            ? elementHandler.CreateArrayHandler(pgArrayType, _connector.Settings.ArrayNullabilityMode)
            : null;
    }

    public override NpgsqlTypeHandler? ResolveValueDependentValue(object value)
    {
        // For arrays/lists, return timestamp or timestamptz based on the kind of the first DateTime; if the user attempts to
        // mix incompatible Kinds, that will fail during validation. For empty arrays it doesn't matter.
        return value is IList<DateTime> array
            ? ArrayHandler(array.Count == 0 ? DateTimeKind.Unspecified : array[0].Kind)
            : null;

        NpgsqlTypeHandler ArrayHandler(DateTimeKind kind)
            => kind == DateTimeKind.Utc
                ? _timestampTzArrayHandler ??= _typeMapper.ResolveByNpgsqlDbType(NpgsqlDbType.TimestampTz).CreateArrayHandler(
                    (PostgresArrayType)PgType("timestamp with time zone[]"), _connector.Settings.ArrayNullabilityMode)
                : _timestampArrayHandler ??= _typeMapper.ResolveByNpgsqlDbType(NpgsqlDbType.Timestamp).CreateArrayHandler(
                    (PostgresArrayType)PgType("timestamp without time zone[]"), _connector.Settings.ArrayNullabilityMode);
    }

    public override NpgsqlTypeHandler? ResolveByClrType(Type type)
        => type != typeof(byte[]) // We do support mapping .NET byte to PG smallint, but we want byte[][] to resolve to bytea[] by default.
           && GetArrayListElementType(type) is { } arrayElementType
           && _typeMapper.ResolveByClrType(arrayElementType) is { PostgresType.Array: { } pgArrayType } elementHandler
            ? elementHandler.CreateArrayHandler(pgArrayType, _connector.Settings.ArrayNullabilityMode)
            : null;

    static Type? GetArrayListElementType(Type type)
    {
        var typeInfo = type.GetTypeInfo();
        if (typeInfo.IsArray)
            return GetUnderlyingType(type.GetElementType()!); // The use of bang operator is justified here as Type.GetElementType() only returns null for the Array base class which can't be mapped in a useful way.

        var ilist = typeInfo.ImplementedInterfaces.FirstOrDefault(x => x.GetTypeInfo().IsGenericType && x.GetGenericTypeDefinition() == typeof(IList<>));
        return ilist != null ? GetUnderlyingType(ilist.GetGenericArguments()[0]) : null;

        Type GetUnderlyingType(Type t)
            => Nullable.GetUnderlyingType(t) ?? t;
    }

    NpgsqlTypeHandler Int2VectorHandler() => _int2VectorHandler ??= new Int2VectorHandler(PgType("int2vector"), PgType("smallint"));
    NpgsqlTypeHandler OidVectorHandler() => _oidVectorHandler ??= new OIDVectorHandler(PgType("oidvector"), PgType("oid"));

    PostgresType PgType(string pgTypeName) => _databaseInfo.GetPostgresTypeByName(pgTypeName);
}
