using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Npgsql.Internal.Postgres;
using NpgsqlTypes;

namespace Npgsql.Internal;

static class AdoSerializerHelpers
{
    public static PgTypeInfo GetTypeInfoForReading(Type type, PgTypeId pgTypeId, PgSerializerOptions options)
    {
        PgTypeInfo? typeInfo = null;
        Exception? inner = null;
        try
        {
            typeInfo = options.GetTypeInfoInternal(type, pgTypeId);
        }
        catch (Exception ex)
        {
            inner = ex;
        }
        return typeInfo ?? ThrowReadingNotSupported(type, options, pgTypeId, inner);
    }

    public static PgTypeInfo GetTypeInfoForWriting(Type? type, PgTypeId? pgTypeId, PgSerializerOptions options, NpgsqlDbType? npgsqlDbType = null)
    {
        Debug.Assert(type != typeof(object), "Parameters of type object are not supported.");

        PgTypeInfo? typeInfo = null;
        Exception? inner = null;
        try
        {
            typeInfo = options.GetTypeInfoInternal(type, pgTypeId);
        }
        catch (Exception ex)
        {
            inner = ex;
        }
        return typeInfo ?? ThrowWritingNotSupported(type, options, pgTypeId, npgsqlDbType, inner: inner);
    }

    // InvalidCastException thrown to align with ADO.NET convention.
    // resolved=true distinguishes the "resolution succeeded but the resolved converter opted out of this
    // direction" case (e.g. read-only converters) from the "no converter could be found / resolution threw"
    // case — important for diagnosing user reports.
    [DoesNotReturn]
    internal static PgTypeInfo ThrowReadingNotSupported(Type? type, PgSerializerOptions options, PgTypeId pgTypeId, Exception? inner = null, bool resolved = false)
    {
        var typeFragment = type is null ? "" : $" as '{type.FullName}'{(resolved ? " (resolved)" : "")}";
        var dataTypeNameFragment = $"DataTypeName '{options.DatabaseInfo.FindPostgresType(pgTypeId)?.DisplayName ?? "unknown"}'";
        var innerHint = inner is null ? "" : " See the inner exception for details.";

        throw new InvalidCastException($"Reading{typeFragment} is not supported for fields having {dataTypeNameFragment}.{innerHint}", inner);
    }

    [DoesNotReturn]
    internal static PgTypeInfo ThrowWritingNotSupported(Type? type, PgSerializerOptions options, PgTypeId? pgTypeId, NpgsqlDbType? npgsqlDbType = null, string? parameterName = null, Exception? inner = null, bool resolved = false)
    {
        var pgTypeFragment = pgTypeId is null
            ? "no NpgsqlDbType or DataTypeName. Try setting one of these values to the expected database type."
            : npgsqlDbType is null
                ? $"DataTypeName '{options.DatabaseInfo.FindPostgresType(pgTypeId.GetValueOrDefault())?.DisplayName ?? "unknown"}'"
                : $"NpgsqlDbType '{npgsqlDbType}'";
        var parameterFragment = parameterName is null ? "parameters" : $"parameter '{parameterName}'";
        var typeFragment = type is null ? "" : $" values of type '{type.FullName}'{(resolved ? " (resolved)" : "")}";
        var innerHint = inner is null ? "" : " See the inner exception for details.";

        throw new InvalidCastException($"Writing{typeFragment} is not supported for {parameterFragment} having {pgTypeFragment}.{innerHint}", inner);
    }
}
