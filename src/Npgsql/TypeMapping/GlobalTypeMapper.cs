using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using Npgsql.Internal;
using Npgsql.Internal.Postgres;
using Npgsql.Internal.Resolvers;
using NpgsqlTypes;

namespace Npgsql.TypeMapping;

/// <inheritdoc />
sealed class GlobalTypeMapper : INpgsqlTypeMapper
{
    readonly UserTypeMapper _userTypeMapper = new();
    readonly List<IPgTypeInfoResolver> _pluginResolvers = new();
    readonly ReaderWriterLockSlim _lock = new();
    IPgTypeInfoResolver[] _typeMappingResolvers = Array.Empty<IPgTypeInfoResolver>();

    internal List<HackyEnumTypeMapping> HackyEnumTypeMappings { get; } = new();

    internal IEnumerable<IPgTypeInfoResolver> GetPluginResolvers()
    {
        var resolvers = new List<IPgTypeInfoResolver>();
        _lock.EnterReadLock();
        try
        {
            resolvers.AddRange(_pluginResolvers);
        }
        finally
        {
            _lock.ExitReadLock();
        }

        return resolvers;
    }

    internal IPgTypeInfoResolver? GetUserMappingsResolver()
    {
        _lock.EnterReadLock();
        try
        {
            return _userTypeMapper.Items.Count > 0 ? _userTypeMapper.Build() : null;
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    internal void AddGlobalTypeMappingResolvers(IPgTypeInfoResolver[] resolvers, bool overwrite = false)
    {
        // Good enough logic to prevent SlimBuilder overriding the normal Builder.
        if (overwrite || resolvers.Length > _typeMappingResolvers.Length)
        {
            _typeMappingResolvers = resolvers;
            ResetTypeMappingCache();
        }
    }

    void ResetTypeMappingCache() => _typeMappingOptions = null;

    PgSerializerOptions? _typeMappingOptions;

    PgSerializerOptions TypeMappingOptions
    {
        get
        {
            if (_typeMappingOptions is not null)
                return _typeMappingOptions;

            _lock.EnterReadLock();
            try
            {
                var resolvers = new List<IPgTypeInfoResolver>();
                resolvers.Add(_userTypeMapper.Build());
                resolvers.AddRange(_pluginResolvers);
                resolvers.AddRange(_typeMappingResolvers);
                return _typeMappingOptions = new(PostgresMinimalDatabaseInfo.DefaultTypeCatalog)
                {
                    // This means we don't ever have a missing oid for a datatypename as our canonical format is datatypenames.
                    PortableTypeIds = true,
                    // Don't throw if our catalog doesn't know the datatypename.
                    IntrospectionMode = true,
                    TypeInfoResolver = new TypeInfoResolverChain(resolvers)
                };
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
    }

    internal DataTypeName? TryGetDataTypeName(Type type, object value)
    {
        DataTypeName? dataTypeName;
        try
        {
            var typeInfo = TypeMappingOptions.GetTypeInfo(type);
            if (typeInfo is PgResolverTypeInfo info)
                dataTypeName = info.GetObjectResolution(value).PgTypeId.DataTypeName;
            else
                dataTypeName = typeInfo?.GetResolution().PgTypeId.DataTypeName;
        }
        catch
        {
            dataTypeName = null;
        }
        return dataTypeName;
    }

    internal static GlobalTypeMapper Instance { get; }

    static GlobalTypeMapper()
        => Instance = new GlobalTypeMapper();

    /// <summary>
    /// Adds a type info resolver which can add or modify support for PostgreSQL types.
    /// Typically used by plugins.
    /// </summary>
    /// <param name="resolver">The type resolver to be added.</param>
    public void AddTypeInfoResolver(IPgTypeInfoResolver resolver)
    {
        _lock.EnterWriteLock();
        try
        {
            var type = resolver.GetType();

            // Since EFCore.PG plugins (and possibly other users) repeatedly call NpgsqlConnection.GlobalTypeMapper.UseNodaTime,
            // we replace an existing resolver of the same CLR type.
            if (_pluginResolvers.Count > 0 && _pluginResolvers[0].GetType() == type)
                _pluginResolvers[0] = resolver;
            for (var i = 0; i < _pluginResolvers.Count; i++)
            {
                if (_pluginResolvers[i].GetType() == type)
                {
                    _pluginResolvers.RemoveAt(i);
                    break;
                }
            }

            _pluginResolvers.Insert(0, resolver);
            ResetTypeMappingCache();
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    /// <inheritdoc />
    public void Reset()
    {
        _lock.EnterWriteLock();
        try
        {
            _pluginResolvers.Clear();
            _userTypeMapper.Items.Clear();
            HackyEnumTypeMappings.Clear();
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    /// <inheritdoc />
    public INpgsqlNameTranslator DefaultNameTranslator
    {
        get => _userTypeMapper.DefaultNameTranslator;
        set => _userTypeMapper.DefaultNameTranslator = value;
    }

    /// <summary>
    /// Sets up dynamic System.Text.Json mappings. This allows mapping arbitrary .NET types to PostgreSQL <c>json</c> and <c>jsonb</c>
    /// types, as well as <see cref="JsonNode" /> and its derived types.
    /// </summary>
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
    public INpgsqlTypeMapper EnableDynamicJsonMappings(
        JsonSerializerOptions? serializerOptions = null,
        Type[]? jsonbClrTypes = null,
        Type[]? jsonClrTypes = null)
    {
        AddTypeInfoResolver(new JsonDynamicTypeInfoResolver(jsonbClrTypes, jsonClrTypes, serializerOptions));
        AddTypeInfoResolver(new JsonDynamicArrayTypeInfoResolver(jsonbClrTypes, jsonClrTypes, serializerOptions));
        return this;
    }

    /// <summary>
    /// Sets up mappings for the PostgreSQL <c>record</c> type as a .NET <see cref="ValueTuple" /> or <see cref="Tuple" />.
    /// </summary>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    [RequiresUnreferencedCode("The mapping of PostgreSQL records as .NET tuples requires reflection usage which is incompatible with trimming.")]
    [RequiresDynamicCode("The mapping of PostgreSQL records as .NET tuples requires dynamic code usage which is incompatible with NativeAOT.")]
    public INpgsqlTypeMapper EnableRecordsAsTuples()
    {
        AddTypeInfoResolver(new TupledRecordTypeInfoResolver());
        AddTypeInfoResolver(new TupledRecordArrayTypeInfoResolver());
        return this;
    }

    /// <summary>
    /// Sets up mappings allowing the use of unmapped enum, range and multirange types.
    /// </summary>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    [RequiresUnreferencedCode("The use of unmapped enums, ranges or multiranges requires reflection usage which is incompatible with trimming.")]
    [RequiresDynamicCode("The use of unmapped enums, ranges or multiranges requires dynamic code usage which is incompatible with NativeAOT.")]
    public INpgsqlTypeMapper EnableUnmappedTypes()
    {
        AddTypeInfoResolver(new UnmappedEnumTypeInfoResolver());
        AddTypeInfoResolver(new UnmappedRangeTypeInfoResolver());
        AddTypeInfoResolver(new UnmappedMultirangeTypeInfoResolver());

        AddTypeInfoResolver(new UnmappedEnumArrayTypeInfoResolver());
        AddTypeInfoResolver(new UnmappedRangeArrayTypeInfoResolver());
        AddTypeInfoResolver(new UnmappedMultirangeArrayTypeInfoResolver());

        return this;
    }

    /// <inheritdoc />
    public INpgsqlTypeMapper MapEnum<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] TEnum>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null) where TEnum : struct, Enum
    {
        _lock.EnterWriteLock();
        try
        {
            _userTypeMapper.MapEnum<TEnum>(pgName, nameTranslator);

            // Temporary hack for EFCore.PG enum mapping compat
            if (_userTypeMapper.Items.FirstOrDefault(i => i.ClrType == typeof(TEnum)) is UserTypeMapping userTypeMapping)
                HackyEnumTypeMappings.Add(new(typeof(TEnum), userTypeMapping.PgTypeName, nameTranslator ?? DefaultNameTranslator));

            ResetTypeMappingCache();

            return this;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    /// <inheritdoc />
    public bool UnmapEnum<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] TEnum>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null) where TEnum : struct, Enum
    {
        _lock.EnterWriteLock();
        try
        {
            var removed = _userTypeMapper.UnmapEnum<TEnum>(pgName, nameTranslator);

            // Temporary hack for EFCore.PG enum mapping compat
            if (removed && ((List<UserTypeMapping>)_userTypeMapper.Items).FindIndex(m => m.ClrType == typeof(TEnum)) is > -1 and var index)
                HackyEnumTypeMappings.RemoveAt(index);

            ResetTypeMappingCache();

            return removed;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    /// <inheritdoc />
    [RequiresDynamicCode("Mapping composite types involves serializing arbitrary types, requiring require creating new generic types or methods. This is currently unsupported with NativeAOT, vote on issue #5303 if this is important to you.")]
    public INpgsqlTypeMapper MapComposite<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicFields)]  T>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
        => MapComposite(typeof(T), pgName, nameTranslator);

    /// <inheritdoc />
    [RequiresDynamicCode("Mapping composite types involves serializing arbitrary types, requiring require creating new generic types or methods. This is currently unsupported with NativeAOT, vote on issue #5303 if this is important to you.")]
    public bool UnmapComposite<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicFields)]  T>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
        => UnmapComposite(typeof(T), pgName, nameTranslator);

    /// <inheritdoc />
    [RequiresDynamicCode("Mapping composite types involves serializing arbitrary types, requiring require creating new generic types or methods. This is currently unsupported with NativeAOT, vote on issue #5303 if this is important to you.")]
    public INpgsqlTypeMapper MapComposite([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicFields)]
        Type clrType, string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
    {
        _lock.EnterWriteLock();
        try
        {
            _userTypeMapper.MapComposite(clrType, pgName, nameTranslator);
            ResetTypeMappingCache();
            return this;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    /// <inheritdoc />
    [RequiresDynamicCode("Mapping composite types involves serializing arbitrary types, requiring require creating new generic types or methods. This is currently unsupported with NativeAOT, vote on issue #5303 if this is important to you.")]
    public bool UnmapComposite([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicFields)]
        Type clrType, string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
    {
        _lock.EnterWriteLock();
        try
        {
            var result = _userTypeMapper.UnmapComposite(clrType, pgName, nameTranslator);
            ResetTypeMappingCache();
            return result;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }
}
