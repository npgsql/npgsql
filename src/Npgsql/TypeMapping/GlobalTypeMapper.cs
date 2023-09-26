﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Npgsql.Internal;
using Npgsql.Internal.Postgres;

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
                dataTypeName = typeInfo?.GetConcreteResolution().PgTypeId.DataTypeName;
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

    /// <inheritdoc />
    public INpgsqlTypeMapper MapEnum<TEnum>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null) where TEnum : struct, Enum
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
    public bool UnmapEnum<TEnum>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null) where TEnum : struct, Enum
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
    [RequiresUnreferencedCode("Composite type mapping currently isn't trimming-safe.")]
    public INpgsqlTypeMapper MapComposite<T>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
        => MapComposite(typeof(T), pgName, nameTranslator);

    /// <inheritdoc />
    [RequiresUnreferencedCode("Composite type mapping currently isn't trimming-safe.")]
    public bool UnmapComposite<T>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
        => UnmapComposite(typeof(T), pgName, nameTranslator);

    /// <inheritdoc />
    [RequiresUnreferencedCode("Composite type mapping currently isn't trimming-safe.")]
    public INpgsqlTypeMapper MapComposite(Type clrType, string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
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
    [RequiresUnreferencedCode("Composite type mapping currently isn't trimming-safe.")]
    public bool UnmapComposite(Type clrType, string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
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
