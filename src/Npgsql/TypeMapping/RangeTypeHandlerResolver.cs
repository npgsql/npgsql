using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandlers.DateTimeHandlers;
using Npgsql.Internal.TypeHandling;
using Npgsql.Internal.TypeMapping;
using Npgsql.PostgresTypes;
using Npgsql.Properties;
using Npgsql.Util;
using NpgsqlTypes;
using static Npgsql.Util.Statics;

namespace Npgsql.TypeMapping;

sealed class RangeTypeHandlerResolver : TypeHandlerResolver
{
    readonly TypeMapper _typeMapper;
    readonly NpgsqlDatabaseInfo _databaseInfo;

    readonly TimestampHandler _timestampHandler;
    readonly TimestampTzHandler _timestampTzHandler;

    NpgsqlTypeHandler? _timestampRangeHandler;
    NpgsqlTypeHandler? _timestampTzRangeHandler;
    NpgsqlTypeHandler? _timestampMultirangeHandler;
    NpgsqlTypeHandler? _timestampTzMultirangeHandler;

    internal RangeTypeHandlerResolver(TypeMapper typeMapper, NpgsqlConnector connector)
    {
        _typeMapper = typeMapper;
        _databaseInfo = connector.DatabaseInfo;

        _timestampHandler = new TimestampHandler(PgType("timestamp without time zone"));
        _timestampTzHandler = new TimestampTzHandler(PgType("timestamp with time zone"));
    }

    public override NpgsqlTypeHandler? ResolveByDataTypeName(string typeName)
    {
        if (!_databaseInfo.TryGetPostgresTypeByName(typeName, out var pgType))
            return null;

        return pgType switch
        {
            PostgresRangeType pgRangeType
                => _typeMapper.ResolveByOID(pgRangeType.Subtype.OID).CreateRangeHandler(pgRangeType),
            PostgresMultirangeType pgMultirangeType
                => _typeMapper.ResolveByOID(pgMultirangeType.Subrange.Subtype.OID).CreateMultirangeHandler(pgMultirangeType),
            _ => null
        };
    }

    public override NpgsqlTypeHandler? ResolveByNpgsqlDbType(NpgsqlDbType npgsqlDbType)
    {
        if (npgsqlDbType.HasFlag(NpgsqlDbType.Range))
        {
            var subtypeHandler = _typeMapper.ResolveByNpgsqlDbType(npgsqlDbType & ~NpgsqlDbType.Range);

            if (subtypeHandler.PostgresType.Range is not { } pgRangeType)
                throw new ArgumentException(
                    $"No range type could be found in the database for subtype {subtypeHandler.PostgresType}");

            return subtypeHandler.CreateRangeHandler(pgRangeType);
        }

        if (npgsqlDbType.HasFlag(NpgsqlDbType.Multirange))
        {
            var subtypeHandler = _typeMapper.ResolveByNpgsqlDbType(npgsqlDbType & ~NpgsqlDbType.Multirange);

            if (subtypeHandler.PostgresType.Range?.Multirange is not { } pgMultirangeType)
                throw new ArgumentException(string.Format(NpgsqlStrings.NoMultirangeTypeFound, subtypeHandler.PostgresType));

            return subtypeHandler.CreateMultirangeHandler(pgMultirangeType);
        }

        // Not a range or multirange
        return null;
    }

    public override NpgsqlTypeHandler? ResolveByClrType(Type type)
    {
        // Try to see if it is an array type
        var arrayElementType = GetArrayListElementType(type);
        if (arrayElementType is not null)
        {
            // With PG14, we map arrays over range types to PG multiranges by default, not to regular arrays over ranges.
            if (arrayElementType.IsGenericType &&
                arrayElementType.GetGenericTypeDefinition() == typeof(NpgsqlRange<>) &&
                _databaseInfo.Version.IsGreaterOrEqual(14))
            {
                var arraySubtypeType = arrayElementType.GetGenericArguments()[0];

                return _typeMapper.ResolveByClrType(arraySubtypeType) is
                    { PostgresType : { Range : { Multirange: { } pgMultirangeType } } } arraySubtypeHandler
                    ? arraySubtypeHandler.CreateMultirangeHandler(pgMultirangeType)
                    : throw new NotSupportedException($"The CLR range type {type} isn't supported by Npgsql or your PostgreSQL.");
            }
        }

        // TODO: We can make the following compatible with reflection-free mode by having NpgsqlRange implement some interface, and
        // check for that.
        if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(NpgsqlRange<>))
            return null;

        var subtypeType = type.GetGenericArguments()[0];

        return _typeMapper.ResolveByClrType(subtypeType) is { PostgresType : { Range : { } pgRangeType } } subtypeHandler
            ? subtypeHandler.CreateRangeHandler(pgRangeType)
            : throw new NotSupportedException($"The CLR range type {type} isn't supported by Npgsql or your PostgreSQL.");

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
    }

    public override NpgsqlTypeHandler? ResolveValueDependentValue(object value)
    {
        // In LegacyTimestampBehavior, DateTime isn't value-dependent, and handled above in ClrTypeToDataTypeNameTable like other types
        if (LegacyTimestampBehavior)
            return null;

        return value switch
        {
            NpgsqlRange<DateTime> range => RangeHandler(!range.LowerBoundInfinite ? range.LowerBound.Kind :
                !range.UpperBoundInfinite ? range.UpperBound.Kind : DateTimeKind.Unspecified),

            NpgsqlRange<DateTime>[] multirange => MultirangeHandler(GetMultirangeKind(multirange)),
            List<NpgsqlRange<DateTime>> multirange => MultirangeHandler(GetMultirangeKind(multirange)),

            _ => null
        };

        NpgsqlTypeHandler RangeHandler(DateTimeKind kind)
            => kind == DateTimeKind.Utc
                ? _timestampTzRangeHandler ??= _timestampTzHandler.CreateRangeHandler((PostgresRangeType)PgType("tstzrange"))
                : _timestampRangeHandler ??= _timestampHandler.CreateRangeHandler((PostgresRangeType)PgType("tsrange"));

        NpgsqlTypeHandler MultirangeHandler(DateTimeKind kind)
            => kind == DateTimeKind.Utc
                ? _timestampTzMultirangeHandler ??= _timestampTzHandler.CreateMultirangeHandler((PostgresMultirangeType)PgType("tstzmultirange"))
                : _timestampMultirangeHandler ??= _timestampHandler.CreateMultirangeHandler((PostgresMultirangeType)PgType("tsmultirange"));
    }

    static DateTimeKind GetRangeKind(NpgsqlRange<DateTime> range)
        => !range.LowerBoundInfinite
            ? range.LowerBound.Kind
            : !range.UpperBoundInfinite
                ? range.UpperBound.Kind
                : DateTimeKind.Unspecified;

    static DateTimeKind GetMultirangeKind(IList<NpgsqlRange<DateTime>> multirange)
    {
        for (var i = 0; i < multirange.Count; i++)
            if (!multirange[i].IsEmpty)
                return GetRangeKind(multirange[i]);

        return DateTimeKind.Unspecified;
    }

    PostgresType PgType(string pgTypeName) => _databaseInfo.GetPostgresTypeByName(pgTypeName);
}
