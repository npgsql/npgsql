using System;
using System.Diagnostics.CodeAnalysis;
using Npgsql.Internal.ResolverFactories;
using Npgsql.TypeMapping;

// ReSharper disable once CheckNamespace
namespace Npgsql;

/// <summary>
/// Extension methods over <see cref="INpgsqlTypeMapper" />.
/// </summary>
public static class INpgsqlTypeMapperExtensions
{
    /// <summary>
    /// Sets up mappings for the PostgreSQL <c>record</c> type as a .NET <see cref="ValueTuple" /> or <see cref="Tuple" />.
    /// </summary>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    [RequiresUnreferencedCode("The mapping of PostgreSQL records as .NET tuples requires reflection usage which is incompatible with trimming.")]
    [RequiresDynamicCode("The mapping of PostgreSQL records as .NET tuples requires dynamic code usage which is incompatible with NativeAOT.")]
    public static T EnableRecordsAsTuples<T>(this T mapper) where T : INpgsqlTypeMapper
    {
        mapper.AddTypeInfoResolverFactory(new TupledRecordTypeInfoResolverFactory());
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
        mapper.AddTypeInfoResolverFactory(new UnmappedTypeInfoResolverFactory());
        return mapper;
    }
}
