using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Threading;
using Npgsql.Internal;
using Npgsql.Internal.Postgres;
using Npgsql.Internal.ResolverFactories;

namespace Npgsql.TypeMapping;

/// <inheritdoc />
sealed class GlobalTypeMapper : INpgsqlTypeMapper
{
    readonly UserTypeMapper _userTypeMapper = new();
    readonly List<PgTypeInfoResolverFactory> _pluginResolverFactories = [];
    readonly ReaderWriterLockSlim _lock = new();
    PgTypeInfoResolverFactory[] _typeMappingResolvers = [];

    internal IEnumerable<PgTypeInfoResolverFactory> GetPluginResolverFactories()
    {
        var resolvers = new List<PgTypeInfoResolverFactory>();
        _lock.EnterReadLock();
        try
        {
            resolvers.AddRange(_pluginResolverFactories);
        }
        finally
        {
            _lock.ExitReadLock();
        }

        return resolvers;
    }

    internal PgTypeInfoResolverFactory? GetUserMappingsResolverFactory()
    {
        _lock.EnterReadLock();
        try
        {
            return _userTypeMapper.Items.Count > 0 ? _userTypeMapper : null;
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    internal void AddGlobalTypeMappingResolvers(PgTypeInfoResolverFactory[] factories, Func<PgTypeInfoResolverChainBuilder>? builderFactory = null, bool overwrite = false)
    {
        // Good enough logic to prevent SlimBuilder overriding the normal Builder.
        if (overwrite || factories.Length > _typeMappingResolvers.Length)
        {
            _builderFactory = builderFactory;
            _typeMappingResolvers = factories;
            ResetTypeMappingCache();
        }
    }

    void ResetTypeMappingCache() => _typeMappingOptions = null;

    PgSerializerOptions? _typeMappingOptions;
    Func<PgTypeInfoResolverChainBuilder>? _builderFactory;
    JsonSerializerOptions? _jsonSerializerOptions;

    PgSerializerOptions TypeMappingOptions
    {
        get
        {
            if (_typeMappingOptions is not null)
                return _typeMappingOptions;

            _lock.EnterReadLock();
            try
            {
                var builder = _builderFactory?.Invoke() ?? new();
                builder.AppendResolverFactory(_userTypeMapper);
                foreach (var factory in _pluginResolverFactories)
                    builder.AppendResolverFactory(factory);
                foreach (var factory in _typeMappingResolvers)
                    builder.AppendResolverFactory(factory);
                var chain = builder.Build();
                return _typeMappingOptions = new(PostgresMinimalDatabaseInfo.DefaultTypeCatalog, chain)
                {
                    // This means we don't ever have a missing oid for a datatypename as our canonical format is datatypenames.
                    PortableTypeIds = true,
                    // Don't throw if our catalog doesn't know the datatypename.
                    IntrospectionMode = true
                };
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
    }

    internal DataTypeName? FindDataTypeName(Type type, object value)
    {
        DataTypeName? dataTypeName;
        try
        {
            var typeInfo = TypeMappingOptions.GetTypeInfoInternal(type, null);
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

    /// <inheritdoc />
    public void AddTypeInfoResolverFactory(PgTypeInfoResolverFactory factory)
    {
        _lock.EnterWriteLock();
        try
        {
            var type = factory.GetType();

            // Since EFCore.PG plugins (and possibly other users) repeatedly call NpgsqlConnection.GlobalTypeMapper.UseNodaTime,
            // we replace an existing resolver of the same CLR type.
            if (_pluginResolverFactories.Count > 0 && _pluginResolverFactories[0].GetType() == type)
                _pluginResolverFactories[0] = factory;
            for (var i = 0; i < _pluginResolverFactories.Count; i++)
            {
                if (_pluginResolverFactories[i].GetType() == type)
                {
                    _pluginResolverFactories.RemoveAt(i);
                    break;
                }
            }

            _pluginResolverFactories.Insert(0, factory);
            ResetTypeMappingCache();
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    void ReplaceTypeInfoResolverFactory(PgTypeInfoResolverFactory factory)
    {
        _lock.EnterWriteLock();
        try
        {
            var type = factory.GetType();

            for (var i = 0; i < _pluginResolverFactories.Count; i++)
            {
                if (_pluginResolverFactories[i].GetType() == type)
                {
                    _pluginResolverFactories[i] = factory;
                    break;
                }
            }

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
            _pluginResolverFactories.Clear();
            _userTypeMapper.Items.Clear();
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

    /// <inheritdoc />
    public INpgsqlTypeMapper ConfigureJsonOptions(JsonSerializerOptions serializerOptions)
    {
        _jsonSerializerOptions = serializerOptions;
        // If JsonTypeInfoResolverFactory exists we replace it with a configured instance on the same index of the array.
        ReplaceTypeInfoResolverFactory(new JsonTypeInfoResolverFactory(serializerOptions));
        return this;
    }

    /// <inheritdoc />
    [RequiresUnreferencedCode("Json serializer may perform reflection on trimmed types.")]
    [RequiresDynamicCode("Serializing arbitrary types to json can require creating new generic types or methods, which requires creating code at runtime. This may not work when AOT compiling.")]
    public INpgsqlTypeMapper EnableDynamicJson(
        Type[]? jsonbClrTypes = null,
        Type[]? jsonClrTypes = null)
    {
        AddTypeInfoResolverFactory(new JsonDynamicTypeInfoResolverFactory(jsonbClrTypes, jsonClrTypes, _jsonSerializerOptions));
        return this;
    }

    /// <inheritdoc />
    [RequiresUnreferencedCode("The mapping of PostgreSQL records as .NET tuples requires reflection usage which is incompatible with trimming.")]
    [RequiresDynamicCode("The mapping of PostgreSQL records as .NET tuples requires dynamic code usage which is incompatible with NativeAOT.")]
    public INpgsqlTypeMapper EnableRecordsAsTuples()
    {
        AddTypeInfoResolverFactory(new TupledRecordTypeInfoResolverFactory());
        return this;
    }

    /// <inheritdoc />
    [RequiresUnreferencedCode("The use of unmapped enums, ranges or multiranges requires reflection usage which is incompatible with trimming.")]
    [RequiresDynamicCode("The use of unmapped enums, ranges or multiranges requires dynamic code usage which is incompatible with NativeAOT.")]
    public INpgsqlTypeMapper EnableUnmappedTypes()
    {
        AddTypeInfoResolverFactory(new UnmappedTypeInfoResolverFactory());
        return this;
    }

    /// <inheritdoc />
    public INpgsqlTypeMapper MapEnum<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] TEnum>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null) where TEnum : struct, Enum
    {
        _lock.EnterWriteLock();
        try
        {
            _userTypeMapper.MapEnum<TEnum>(pgName, nameTranslator);
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
            ResetTypeMappingCache();
            return removed;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    /// <inheritdoc />
    [RequiresDynamicCode("Calling MapEnum with a Type can require creating new generic types or methods. This may not work when AOT compiling.")]
    public INpgsqlTypeMapper MapEnum([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
        Type clrType, string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
    {
        _lock.EnterWriteLock();
        try
        {
            _userTypeMapper.MapEnum(clrType, pgName, nameTranslator);
            ResetTypeMappingCache();
            return this;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    /// <inheritdoc />
    public bool UnmapEnum([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
        Type clrType, string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
    {
        _lock.EnterWriteLock();
        try
        {
            var removed = _userTypeMapper.UnmapEnum(clrType, pgName, nameTranslator);
            ResetTypeMappingCache();
            return removed;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    /// <inheritdoc />
    [RequiresDynamicCode("Mapping composite types involves serializing arbitrary types which can require creating new generic types or methods. This is currently unsupported with NativeAOT, vote on issue #5303 if this is important to you.")]
    public INpgsqlTypeMapper MapComposite<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicFields)]  T>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
        => MapComposite(typeof(T), pgName, nameTranslator);

    /// <inheritdoc />
    [RequiresDynamicCode("Mapping composite types involves serializing arbitrary types which can require creating new generic types or methods. This is currently unsupported with NativeAOT, vote on issue #5303 if this is important to you.")]
    public bool UnmapComposite<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicFields)]  T>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
        => UnmapComposite(typeof(T), pgName, nameTranslator);

    /// <inheritdoc />
    [RequiresDynamicCode("Mapping composite types involves serializing arbitrary types which can require creating new generic types or methods. This is currently unsupported with NativeAOT, vote on issue #5303 if this is important to you.")]
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
    [RequiresDynamicCode("Mapping composite types involves serializing arbitrary types which can require creating new generic types or methods. This is currently unsupported with NativeAOT, vote on issue #5303 if this is important to you.")]
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
