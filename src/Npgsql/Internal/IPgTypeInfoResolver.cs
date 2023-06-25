using System;
using Npgsql.PostgresTypes;

namespace Npgsql.Internal;

public interface IPgTypeInfoResolver
{
    PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options);
}
