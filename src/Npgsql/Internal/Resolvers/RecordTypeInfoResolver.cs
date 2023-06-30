using System;
using Npgsql.Internal;
using Npgsql.PostgresTypes;

namespace Npgsql.TypeMapping.Resolvers;

class RecordTypeInfoResolver : IPgTypeInfoResolver
{
    public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
    {
        return null;
    }
}
