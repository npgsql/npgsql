using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Npgsql.Internal.Postgres;
using Npgsql.PostgresTypes;
using NpgsqlTypes;

namespace Npgsql.Internal;

static class AdoSerializerHelpers
{
    public static PgTypeInfo GetTypeInfoForReading(Type type, PostgresType postgresType, PgSerializerOptions options)
    {
        PgTypeInfo? typeInfo = null;
        Exception? inner = null;
        try
        {
            typeInfo = type == typeof(object) ? options.GetObjectOrDefaultTypeInfo(postgresType) : options.GetTypeInfo(type, postgresType);
        }
        catch (Exception ex)
        {
            inner = ex;
        }
        return typeInfo ?? ThrowReadingNotSupported(type, postgresType.DisplayName, inner);

        // InvalidCastException thrown to align with ADO.NET convention.
        [DoesNotReturn]
        static PgTypeInfo ThrowReadingNotSupported(Type? type, string displayName, Exception? inner = null)
            => throw new InvalidCastException($"Reading{(type is null ? "" : $" as '{type.FullName}'")} is not supported for field having DataTypeName '{displayName}'", inner);
    }

    public static PgTypeInfo GetTypeInfoForWriting(Type? type, PgTypeId? pgTypeId, PgSerializerOptions options, NpgsqlDbType? npgsqlDbType = null)
    {
        Debug.Assert(type != typeof(object), "Parameters of type object are not supported.");

        PgTypeInfo? typeInfo = null;
        Exception? inner = null;
        try
        {
            typeInfo = type is null ? options.GetDefaultTypeInfo(pgTypeId!.Value) : options.GetTypeInfo(type, pgTypeId);
        }
        catch (Exception ex)
        {
            inner = ex;
        }
        return typeInfo ?? ThrowWritingNotSupported(type,
            pgTypeString:
                pgTypeId is null ? "no NpgsqlDbType or DataTypeName. Try setting one of these values to the expected database type." :
                npgsqlDbType is null
                ? $"DataTypeName '{options.DatabaseInfo.FindPostgresType(pgTypeId.GetValueOrDefault())?.DisplayName ?? "unknown"}'"
                : $"NpgsqlDbType '{npgsqlDbType}'", inner);

        // InvalidCastException thrown to align with ADO.NET convention.
        [DoesNotReturn]
        static PgTypeInfo ThrowWritingNotSupported(Type? type, string pgTypeString, Exception? inner = null)
            => throw new InvalidCastException($"Writing{(type is null ? "" : $" values of '{type.FullName}'")} is not supported for parameter having {pgTypeString}.", inner);
    }
}
