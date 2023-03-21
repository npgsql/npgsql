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
    {
        if (typeName == "int2vector")
            return Int2VectorHandler();
        if (typeName == "oidvector")
            return OidVectorHandler();

        if (_databaseInfo.TryGetPostgresTypeByName(typeName, out var pgType) && pgType is PostgresArrayType pgArrayType)
            return _typeMapper.ResolveByOID(pgArrayType.Element.OID).CreateArrayHandler(pgArrayType, _connector.Settings.ArrayNullabilityMode);

        return null;
    }

    public override NpgsqlTypeHandler? ResolveByNpgsqlDbType(NpgsqlDbType npgsqlDbType)
    {
        if (!npgsqlDbType.HasFlag(NpgsqlDbType.Array))
            return null;

        var elementHandler = _typeMapper.ResolveByNpgsqlDbType(npgsqlDbType & ~NpgsqlDbType.Array);

        // TODO: return an unsupported handler???
        if (elementHandler.PostgresType.Array is not { } pgArrayType)
            throw new ArgumentException(
                $"No array type could be found in the database for element {elementHandler.PostgresType}");

        return elementHandler.CreateArrayHandler(pgArrayType, _connector.Settings.ArrayNullabilityMode);
    }

    public override NpgsqlTypeHandler? ResolveValueDependentValue(object value)
    {
        // For arrays/lists, return timestamp or timestamptz based on the kind of the first DateTime; if the user attempts to
        // mix incompatible Kinds, that will fail during validation. For empty arrays it doesn't matter.
        if (value is IList<DateTime> array)
            return ArrayHandler(array.Count == 0 ? DateTimeKind.Unspecified : array[0].Kind);

        return null;

        NpgsqlTypeHandler ArrayHandler(DateTimeKind kind)
            => kind == DateTimeKind.Utc
                ? _timestampTzArrayHandler ??= _typeMapper.ResolveByNpgsqlDbType(NpgsqlDbType.TimestampTz).CreateArrayHandler(
                    (PostgresArrayType)PgType("timestamp with time zone[]"), _connector.Settings.ArrayNullabilityMode)
                : _timestampArrayHandler ??= _typeMapper.ResolveByNpgsqlDbType(NpgsqlDbType.Timestamp).CreateArrayHandler(
                    (PostgresArrayType)PgType("timestamp without time zone[]"), _connector.Settings.ArrayNullabilityMode);
    }

    public override NpgsqlTypeHandler? ResolveByClrType(Type type)
    {
        // Try to see if it is an array type
        var arrayElementType = GetArrayListElementType(type);
        if (arrayElementType is null)
            return null;

        // TODO: return an unsupported handler???
        if (_typeMapper.ResolveByClrType(arrayElementType) is not { } elementHandler)
            throw new ArgumentException($"Array type over CLR type {arrayElementType.Name} isn't supported by Npgsql");

        if (elementHandler.PostgresType.Array is not { } pgArrayType)
            throw new ArgumentException(
                $"No array type could be found in the database for element {elementHandler.PostgresType}");

        return elementHandler.CreateArrayHandler(pgArrayType, _connector.Settings.ArrayNullabilityMode);
    }

    static Type? GetArrayListElementType(Type type)
    {
        var typeInfo = type.GetTypeInfo();
        if (typeInfo.IsArray)
            return GetUnderlyingType(type.GetElementType()!); // The use of bang operator is justified here as Type.GetElementType() only returns null for the Array base class which can't be mapped in a useful way.

        var ilist = typeInfo.ImplementedInterfaces.FirstOrDefault(x => x.GetTypeInfo().IsGenericType && x.GetGenericTypeDefinition() == typeof(IList<>));
        if (ilist != null)
            return GetUnderlyingType(ilist.GetGenericArguments()[0]);

        if (typeof(IList).IsAssignableFrom(type))
            throw new NotSupportedException("Non-generic IList is a supported parameter, but the NpgsqlDbType parameter must be set on the parameter");

        return null;

        Type GetUnderlyingType(Type t)
            => Nullable.GetUnderlyingType(t) ?? t;
    }

    NpgsqlTypeHandler Int2VectorHandler() => _int2VectorHandler ??= new Int2VectorHandler(PgType("int2vector"), PgType("smallint"));
    NpgsqlTypeHandler OidVectorHandler() => _oidVectorHandler ??= new OIDVectorHandler(PgType("oidvector"), PgType("oid"));

    PostgresType PgType(string pgTypeName) => _databaseInfo.GetPostgresTypeByName(pgTypeName);
}
