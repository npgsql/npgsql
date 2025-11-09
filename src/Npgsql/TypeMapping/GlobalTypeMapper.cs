using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Npgsql.Internal;
using Npgsql.Internal.Postgres;
using Npgsql.Internal.ResolverFactories;

namespace Npgsql.TypeMapping;

/// <inheritdoc />
sealed class GlobalTypeMapper : INpgsqlTypeMapper
{
    readonly UserTypeMapper _userTypeMapper = new();
    readonly List<PgTypeInfoResolverFactory> _pluginResolverFactories = [];
    readonly object _sync = new();
    PgTypeInfoResolverFactory[] _typeMappingResolvers = [];

    internal IEnumerable<PgTypeInfoResolverFactory> GetPluginResolverFactories()
    {
        lock (_sync)
            return new List<PgTypeInfoResolverFactory>(_pluginResolverFactories);
    }

    internal PgTypeInfoResolverFactory? GetUserMappingsResolverFactory()
    {
        lock (_sync)
            return _userTypeMapper.Items.Count > 0 ? _userTypeMapper : null;
    }

    internal void AddGlobalTypeMappingResolvers(PgTypeInfoResolverFactory[] factories, Func<PgTypeInfoResolverChainBuilder>? builderFactory = null, bool overwrite = false)
    {
        lock (_sync)
        {
            // Good enough logic to prevent SlimBuilder overriding the normal Builder.
            if (overwrite || factories.Length > _typeMappingResolvers.Length)
            {
                _builderFactory = builderFactory;
                _typeMappingResolvers = factories;
                _typeMappingOptions = null;
            }
        }
    }

    PgSerializerOptions? _typeMappingOptions;
    Func<PgTypeInfoResolverChainBuilder>? _builderFactory;
    JsonSerializerOptions? _jsonSerializerOptions;

    PgSerializerOptions TypeMappingOptions => _typeMappingOptions ?? BuildTypeMappingOptions();

    PgSerializerOptions BuildTypeMappingOptions()
    {
        lock (_sync)
        {
            if (_typeMappingOptions is { } existing)
                return existing;

            var builder = _builderFactory?.Invoke() ?? new();
            builder.AppendResolverFactory(_userTypeMapper);
            foreach (var factory in _pluginResolverFactories)
                builder.AppendResolverFactory(factory);
            foreach (var factory in _typeMappingResolvers)
                builder.AppendResolverFactory(factory);
            var chain = builder.Build();
            var options = new PgSerializerOptions(PostgresMinimalDatabaseInfo.DefaultTypeCatalog, chain)
            {
                // This means we don't ever have a missing oid for a datatypename as our canonical format is datatypenames.
                PortableTypeIds = true,
                // Don't throw if our catalog doesn't know the datatypename.
                IntrospectionMode = true
            };
            _typeMappingOptions = options;
            return options;
        }
    }

    internal DataTypeName? FindDataTypeName(Type type, object? value)
    {
        DataTypeName? dataTypeName;
        try
        {
            var typeInfo = TypeMappingOptions.GetTypeInfoInternal(type, null);
            if (typeInfo is PgProviderTypeInfo providerInfo)
            {
                var concreteTypeInfo = providerInfo.GetObjectConcreteTypeInfo(value, out var state);
                if (state is not null)
                    concreteTypeInfo.DisposeWriteState(state);
                dataTypeName = concreteTypeInfo.PgTypeId.DataTypeName;
            }
            else
            {
                dataTypeName = ((PgConcreteTypeInfo?)typeInfo)?.PgTypeId.DataTypeName;
            }
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
        lock (_sync)
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
            _typeMappingOptions = null;
        }
    }

    public void AddDbTypeResolverFactory(DbTypeResolverFactory factory)
        => throw new NotSupportedException("The global type mapper does not support DbTypeResolverFactories. Call this method on a data source builder instead.");

    /// <inheritdoc />
    public void Reset()
    {
        lock (_sync)
        {
            _pluginResolverFactories.Clear();
            _userTypeMapper.Items.Clear();
            _typeMappingOptions = null;
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
        lock (_sync)
        {
            _jsonSerializerOptions = serializerOptions;

            // If JsonTypeInfoResolverFactory exists we replace it with a configured instance on the same index of the array.
            var factory = new JsonTypeInfoResolverFactory(serializerOptions);
            var type = factory.GetType();

            for (var i = 0; i < _pluginResolverFactories.Count; i++)
            {
                if (_pluginResolverFactories[i].GetType() == type)
                {
                    _pluginResolverFactories[i] = factory;
                    break;
                }
            }

            _typeMappingOptions = null;
        }
        return this;
    }

    /// <inheritdoc />
    [RequiresUnreferencedCode("Json serializer may perform reflection on trimmed types.")]
    [RequiresDynamicCode("Serializing arbitrary types to json can require creating new generic types or methods, which requires creating code at runtime. This may not work when AOT compiling.")]
    public INpgsqlTypeMapper EnableDynamicJson(
        Type[]? jsonbClrTypes = null,
        Type[]? jsonClrTypes = null)
    {
        // Use a re-entered lock to add the read of _jsonSerializerOptions to the total scope.
        lock (_sync)
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
        lock (_sync)
        {
            _userTypeMapper.MapEnum<TEnum>(pgName, nameTranslator);
            _typeMappingOptions = null;
            return this;
        }
    }

    /// <inheritdoc />
    public bool UnmapEnum<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] TEnum>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null) where TEnum : struct, Enum
    {
        lock (_sync)
        {
            var removed = _userTypeMapper.UnmapEnum<TEnum>(pgName, nameTranslator);
            _typeMappingOptions = null;
            return removed;
        }
    }

    /// <inheritdoc />
    [RequiresDynamicCode("Calling MapEnum with a Type can require creating new generic types or methods. This may not work when AOT compiling.")]
    public INpgsqlTypeMapper MapEnum([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
        Type clrType, string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
    {
        lock (_sync)
        {
            _userTypeMapper.MapEnum(clrType, pgName, nameTranslator);
            _typeMappingOptions = null;
            return this;
        }
    }

    /// <inheritdoc />
    public bool UnmapEnum([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
        Type clrType, string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
    {
        lock (_sync)
        {
            var removed = _userTypeMapper.UnmapEnum(clrType, pgName, nameTranslator);
            _typeMappingOptions = null;
            return removed;
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
        lock (_sync)
        {
            _userTypeMapper.MapComposite(clrType, pgName, nameTranslator);
            _typeMappingOptions = null;
            return this;
        }
    }

    /// <inheritdoc />
    [RequiresDynamicCode("Mapping composite types involves serializing arbitrary types which can require creating new generic types or methods. This is currently unsupported with NativeAOT, vote on issue #5303 if this is important to you.")]
    public bool UnmapComposite([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicFields)]
        Type clrType, string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
    {
        lock (_sync)
        {
            var result = _userTypeMapper.UnmapComposite(clrType, pgName, nameTranslator);
            _typeMappingOptions = null;
            return result;
        }
    }
}
