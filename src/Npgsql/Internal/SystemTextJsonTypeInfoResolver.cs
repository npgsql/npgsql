using System;
using System.Text.Json;
using Npgsql.Internal;
using Npgsql.PostgresTypes;

namespace Npgsql.TypeMapping;

class SystemTextJsonTypeInfoResolver : IPgTypeInfoResolver
{
    public SystemTextJsonTypeInfoResolver(Type[]? jsonbClrTypes = null, Type[]? jsonClrTypes = null, JsonSerializerOptions? serializerOptions = null)
    {
    }

    public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
    {
        return null;
    }
}
