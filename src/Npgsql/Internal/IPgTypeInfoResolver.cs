using System;
using Npgsql.PostgresTypes;

namespace Npgsql.Internal;

// TODO decide whether to keep the two methods merged or split them.
public interface IPgTypeInfoResolver
{
    // /// Called when just a DataTypeName is known, this should return the most appropriate/default clr type to convert with.
    // PgTypeInfo? GetDefaultTypeInfo(DataTypeName dataTypeName, PgConverterOptions options);

    PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options);
}
