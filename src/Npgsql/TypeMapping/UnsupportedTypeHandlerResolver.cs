using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Reflection;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandlers;
using Npgsql.Internal.TypeHandlers.DateTimeHandlers;
using Npgsql.Internal.TypeHandlers.FullTextSearchHandlers;
using Npgsql.Internal.TypeHandlers.GeometricHandlers;
using Npgsql.Internal.TypeHandlers.InternalTypeHandlers;
using Npgsql.Internal.TypeHandlers.LTreeHandlers;
using Npgsql.Internal.TypeHandlers.NetworkHandlers;
using Npgsql.Internal.TypeHandlers.NumericHandlers;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using Npgsql.Properties;
using NpgsqlTypes;
using static Npgsql.Util.Statics;

namespace Npgsql.TypeMapping;

sealed class UnsupportedTypeHandlerResolver : TypeHandlerResolver
{
    readonly NpgsqlDatabaseInfo _databaseInfo;

    internal UnsupportedTypeHandlerResolver(NpgsqlConnector connector)
        => _databaseInfo = connector.DatabaseInfo;

    public override NpgsqlTypeHandler? ResolveByDataTypeName(string typeName)
        => PgType(typeName) switch
        {
            PostgresBaseType { Name: "record" } => UnsupportedRecordHandler(),
            PostgresBaseType { Name: "tsvector" } => UnsupportedTsVectorHandler(),
            PostgresBaseType { Name: "tsquery" } => UnsupportedTsQueryHandler(),
            PostgresRangeType pgRangeType => UnsupportedRangeHandler(pgRangeType),
            PostgresArrayType pgArrayType => UnsupportedArrayHandler(pgArrayType),

            _ => null
        };

    public override NpgsqlTypeHandler? ResolveByClrType(Type type)
    {
        switch (type.FullName)
        {
        case "NpgsqlTypes.NpgsqlTsVector":
        case "NpgsqlTypes.NpgsqlTsQueryLexeme":
        case "NpgsqlTypes.NpgsqlTsQueryAnd":
        case "NpgsqlTypes.NpgsqlTsQueryOr":
        case "NpgsqlTypes.NpgsqlTsQueryNot":
        case "NpgsqlTypes.NpgsqlTsQueryEmpty":
        case "NpgsqlTypes.NpgsqlTsQueryFollowedBy":
            return UnsupportedTsQueryHandler();
        default:
            return null;
        }
    }

    PostgresType PgType(string pgTypeName) => _databaseInfo.GetPostgresTypeByName(pgTypeName);

    NpgsqlTypeHandler UnsupportedRecordHandler() => new UnsupportedHandler(PgType("record"), string.Format(
        NpgsqlStrings.RecordsNotEnabled, nameof(NpgsqlSlimDataSourceBuilder.EnableRecords), nameof(NpgsqlSlimDataSourceBuilder)));

    NpgsqlTypeHandler UnsupportedTsVectorHandler() => new UnsupportedHandler(PgType("tsvector"), string.Format(
        NpgsqlStrings.FullTextSearchNotEnabled, nameof(NpgsqlSlimDataSourceBuilder.EnableFullTextSearch),
        nameof(NpgsqlSlimDataSourceBuilder)));

    NpgsqlTypeHandler UnsupportedTsQueryHandler() => new UnsupportedHandler(PgType("tsquery"), string.Format(
        NpgsqlStrings.FullTextSearchNotEnabled, nameof(NpgsqlSlimDataSourceBuilder.EnableFullTextSearch),
        nameof(NpgsqlSlimDataSourceBuilder)));

    NpgsqlTypeHandler UnsupportedRangeHandler(PostgresRangeType pgType) => new UnsupportedHandler(pgType, string.Format(
        NpgsqlStrings.RangesNotEnabled, nameof(NpgsqlSlimDataSourceBuilder.EnableRanges),
        nameof(NpgsqlSlimDataSourceBuilder)));

    NpgsqlTypeHandler UnsupportedArrayHandler(PostgresArrayType pgType) => new UnsupportedHandler(pgType, string.Format(
        NpgsqlStrings.ArraysNotEnabled, nameof(NpgsqlSlimDataSourceBuilder.EnableArrays),
        nameof(NpgsqlSlimDataSourceBuilder)));
}
