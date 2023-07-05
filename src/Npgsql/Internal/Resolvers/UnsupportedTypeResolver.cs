using System;
using System.Collections;
using System.Collections.Generic;
using Npgsql.PostgresTypes;
using Npgsql.Properties;

namespace Npgsql.Internal.Resolvers;

sealed class UnsupportedTypeInfoResolver<TBuilder> : IPgTypeInfoResolver
{
    public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
    {
        if (typeof(IEnumerable<>).IsAssignableFrom(type) && !typeof(IList).IsAssignableFrom(type) && type != typeof(string))
            throw new NotSupportedException("Writing is not supported for IEnumerable parameters, use an array or List instead.");

        if (type != typeof(object) && DataTypeNames.Record == dataTypeName)
            throw new NotSupportedException(string.Format(NpgsqlStrings.RecordsNotEnabled, "EnableRecords", typeof(TBuilder).Name));

        return null;
    }
}
