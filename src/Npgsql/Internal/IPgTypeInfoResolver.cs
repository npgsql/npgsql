using System;
using Npgsql.Internal.Postgres;

namespace Npgsql.Internal;

public interface IPgTypeInfoResolver
{
    PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options);
}
