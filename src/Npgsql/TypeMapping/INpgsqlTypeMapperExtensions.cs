using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Nodes;
using Npgsql.Internal.Resolvers;
using Npgsql.TypeMapping;
using NpgsqlTypes;

// ReSharper disable once CheckNamespace
namespace Npgsql;

/// <summary>
/// Extension methods over <see cref="INpgsqlTypeMapper" />.
/// </summary>
public static class INpgsqlTypeMapperExtensions
{
    /// <summary>
    /// Sets up dynamic System.Text.Json mappings. This allows mapping arbitrary .NET types to PostgreSQL <c>json</c> and <c>jsonb</c>
    /// types, as well as <see cref="JsonNode" /> and its derived types.
    /// </summary>
    /// <param name="mapper">The type mapper.</param>
    /// <param name="serializerOptions">Options to customize JSON serialization and deserialization.</param>
    /// <param name="jsonbClrTypes">
    /// A list of CLR types to map to PostgreSQL <c>jsonb</c> (no need to specify <see cref="NpgsqlDbType.Jsonb" />).
    /// </param>
    /// <param name="jsonClrTypes">
    /// A list of CLR types to map to PostgreSQL <c>json</c> (no need to specify <see cref="NpgsqlDbType.Json" />).
    /// </param>
    /// <remarks>
    /// Due to the dynamic nature of these mappings, they are not compatible with NativeAOT or trimming.
    /// </remarks>
    [RequiresUnreferencedCode("Json serializer may perform reflection on trimmed types.")]
    [RequiresDynamicCode("Serializing arbitrary types to json can require creating new generic types or methods, which requires creating code at runtime. This may not work when AOT compiling.")]
    public static T EnableDynamicJsonMappings<T>(
        this T mapper,
        JsonSerializerOptions? serializerOptions = null,
        Type[]? jsonbClrTypes = null,
        Type[]? jsonClrTypes = null)
        where T : INpgsqlTypeMapper
    {
        mapper.AddTypeInfoResolver(new JsonDynamicTypeInfoResolver(jsonbClrTypes, jsonClrTypes, serializerOptions));
        mapper.AddTypeInfoResolver(new JsonDynamicArrayTypeInfoResolver(jsonbClrTypes, jsonClrTypes, serializerOptions));
        return mapper;
    }

    /// <summary>
    /// Sets up mappings for the PostgreSQL <c>record</c> type as a .NET <see cref="ValueTuple" /> or <see cref="Tuple" />.
    /// </summary>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    [RequiresUnreferencedCode("The mapping of PostgreSQL records as .NET tuples requires reflection usage which is incompatible with trimming.")]
    [RequiresDynamicCode("The mapping of PostgreSQL records as .NET tuples requires dynamic code usage which is incompatible with NativeAOT.")]
    public static T EnableRecordsAsTuples<T>(this T mapper) where T : INpgsqlTypeMapper
    {
        mapper.AddTypeInfoResolver(new TupledRecordTypeInfoResolver());
        mapper.AddTypeInfoResolver(new TupledRecordArrayTypeInfoResolver());
        return mapper;
    }

    /// <summary>
    /// Sets up mappings allowing the use of unmapped enum, range and multirange types.
    /// </summary>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    [RequiresUnreferencedCode("The use of unmapped enums, ranges or multiranges requires reflection usage which is incompatible with trimming.")]
    [RequiresDynamicCode("The use of unmapped enums, ranges or multiranges requires dynamic code usage which is incompatible with NativeAOT.")]
    public static T EnableUnmappedTypes<T>(this T mapper) where T : INpgsqlTypeMapper
    {
        mapper.AddTypeInfoResolver(new UnmappedEnumTypeInfoResolver());
        mapper.AddTypeInfoResolver(new UnmappedRangeTypeInfoResolver());
        mapper.AddTypeInfoResolver(new UnmappedMultirangeTypeInfoResolver());

        mapper.AddTypeInfoResolver(new UnmappedEnumArrayTypeInfoResolver());
        mapper.AddTypeInfoResolver(new UnmappedRangeArrayTypeInfoResolver());
        mapper.AddTypeInfoResolver(new UnmappedMultirangeArrayTypeInfoResolver());

        return mapper;
    }
}
