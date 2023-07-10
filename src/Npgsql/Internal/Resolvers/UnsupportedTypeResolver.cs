using System;
using System.Collections;
using Npgsql.Internal.Postgres;

namespace Npgsql.Internal.Resolvers;

sealed class UnsupportedTypeInfoResolver<TBuilder> : IPgTypeInfoResolver
{
    public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
    {
        if (PgSerializerOptions.IntrospectionMode)
            return null;

        RecordTypeInfoResolver.CheckUnsupported<TBuilder>(type, dataTypeName, options);
        RangeTypeInfoResolver.ThrowIfUnsupported<TBuilder>(type, dataTypeName, options);
        FullTextSearchTypeInfoResolver.CheckUnsupported<TBuilder>(type, dataTypeName, options);

        if (typeof(IEnumerable).IsAssignableFrom(type) && !typeof(IList).IsAssignableFrom(type) && type != typeof(string) && (dataTypeName is null || dataTypeName.Value.IsArray))
            throw new NotSupportedException("Writing is not supported for IEnumerable parameters, use an array or List instead.");
        //
        // $"Can't write CLR type {value.GetType()}. " +
        //     "You may need to use the System.Text.Json or Json.NET plugins, see the docs for more information."

        return null;
    }
}
