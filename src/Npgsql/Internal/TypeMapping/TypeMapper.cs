using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Npgsql.Internal.TypeHandlers;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.Internal.TypeMapping;

/// <summary>
/// Type mapper used to map types to type handlers.
/// </summary>
public sealed class TypeMapper
{
    internal NpgsqlConnector Connector { get; }
    readonly object _writeLock = new();

    NpgsqlDatabaseInfo? _databaseInfo;

    internal NpgsqlDatabaseInfo DatabaseInfo
    {
        get
        {
            var databaseInfo = _databaseInfo;
            if (databaseInfo is null)
                ThrowHelper.ThrowInvalidOperationException("Internal error: this type mapper hasn't yet been bound to a database info object");
            return databaseInfo;
        }
    }

    volatile TypeHandlerResolver[] _handlerResolvers;
    volatile TypeMappingResolver[] _mappingResolvers;
    internal NpgsqlTypeHandler UnrecognizedTypeHandler { get; }

    readonly ConcurrentDictionary<uint, NpgsqlTypeHandler> _handlersByOID = new();
    readonly ConcurrentDictionary<NpgsqlDbType, NpgsqlTypeHandler> _handlersByNpgsqlDbType = new();
    readonly ConcurrentDictionary<Type, NpgsqlTypeHandler> _handlersByClrType = new();
    readonly ConcurrentDictionary<string, NpgsqlTypeHandler> _handlersByDataTypeName = new();

    readonly Dictionary<uint, TypeMappingInfo> _userTypeMappings = new();
    readonly INpgsqlNameTranslator _defaultNameTranslator;

    readonly ILogger _commandLogger;

    #region Construction

    internal TypeMapper(NpgsqlConnector connector, INpgsqlNameTranslator defaultNameTranslator)
    {
        Connector = connector;
        _defaultNameTranslator = defaultNameTranslator;
        UnrecognizedTypeHandler = new UnknownTypeHandler(Connector.TextEncoding);
        _handlerResolvers = Array.Empty<TypeHandlerResolver>();
        _mappingResolvers = Array.Empty<TypeMappingResolver>();
        _commandLogger = connector.LoggingConfiguration.CommandLogger;
    }

    #endregion Constructors

    internal void Initialize(
        NpgsqlDatabaseInfo databaseInfo,
        List<TypeHandlerResolverFactory> resolverFactories,
        Dictionary<string, IUserTypeMapping> userTypeMappings)
    {
        _databaseInfo = databaseInfo;

        var handlerResolvers = new TypeHandlerResolver[resolverFactories.Count];
        var mappingResolvers = new List<TypeMappingResolver>(resolverFactories.Count);
        for (var i = 0; i < resolverFactories.Count; i++)
        {
            handlerResolvers[i] = resolverFactories[i].Create(this, Connector);
            var mappingResolver = resolverFactories[i].CreateMappingResolver();
            if (mappingResolver is not null)
                mappingResolvers.Add(mappingResolver);
        }

        // Add global mapper resolvers in backwards because they're inserted in the beginning
        for (var i = resolverFactories.Count - 1; i >= 0; i--)
        {
            var globalMappingResolver = resolverFactories[i].CreateGlobalMappingResolver();
            if (globalMappingResolver is not null)
                GlobalTypeMapper.Instance.TryAddMappingResolver(globalMappingResolver);
        }

        _handlerResolvers = handlerResolvers;
        _mappingResolvers = mappingResolvers.ToArray();

        foreach (var userTypeMapping in userTypeMappings.Values)
        {
            if (DatabaseInfo.TryGetPostgresTypeByName(userTypeMapping.PgTypeName, out var pgType))
            {
                _handlersByOID[pgType.OID] =
                    _handlersByDataTypeName[pgType.FullName] =
                        _handlersByDataTypeName[pgType.Name] =
                            _handlersByClrType[userTypeMapping.ClrType] = userTypeMapping.CreateHandler(pgType, Connector);

                _userTypeMappings[pgType.OID] = new(npgsqlDbType: null, pgType.Name, userTypeMapping.ClrType);
            }
        }
    }

    #region Type handler lookup

    /// <summary>
    /// Looks up a type handler by its PostgreSQL type's OID.
    /// </summary>
    /// <param name="oid">A PostgreSQL type OID</param>
    /// <returns>A type handler that can be used to encode and decode values.</returns>
    public NpgsqlTypeHandler ResolveByOID(uint oid)
        => TryResolveByOID(oid, out var result) ? result : UnrecognizedTypeHandler;

    internal bool TryResolveByOID(uint oid, [NotNullWhen(true)] out NpgsqlTypeHandler? handler)
    {
        if (_handlersByOID.TryGetValue(oid, out handler))
            return true;

        return TryResolveLong(oid, out handler);

        bool TryResolveLong(uint oid, [NotNullWhen(true)] out NpgsqlTypeHandler? handler)
        {
            if (!DatabaseInfo.ByOID.TryGetValue(oid, out var pgType))
            {
                handler = null;
                return false;
            }

            lock (_writeLock)
            {
                if ((handler = ResolveByPostgresType(pgType)) is not null)
                {
                    _handlersByOID[oid] = handler;
                    return true;
                }

                if ((handler = ResolveComplexTypeByDataTypeName(pgType.FullName, throwOnError: false)) is not null)
                {
                    _handlersByOID[oid] = handler;
                    return true;
                }

                handler = null;
                return false;
            }
        }
    }

    /// <summary>
    /// Looks up a type handler by NpgsqlDbType.
    /// </summary>
    /// <param name="npgsqlDbType">Parameter's NpgsqlDbType</param>
    /// <returns>A type handler that can be used to encode and decode values.</returns>
    public NpgsqlTypeHandler ResolveByNpgsqlDbType(NpgsqlDbType npgsqlDbType)
    {
        if (_handlersByNpgsqlDbType.TryGetValue(npgsqlDbType, out var handler))
            return handler;

        return ResolveLong(npgsqlDbType);

        NpgsqlTypeHandler ResolveLong(NpgsqlDbType npgsqlDbType)
        {
            lock (_writeLock)
            {
                // First, try to resolve as a base type; translate the NpgsqlDbType to a PG data type name and look that up.
                if (GlobalTypeMapper.NpgsqlDbTypeToDataTypeName(npgsqlDbType) is { } dataTypeName)
                {
                    foreach (var resolver in _handlerResolvers)
                    {
                        try
                        {
                            if (resolver.ResolveByDataTypeName(dataTypeName) is { } handler)
                                return _handlersByNpgsqlDbType[npgsqlDbType] = handler;
                        }
                        catch (Exception e)
                        {
                            _commandLogger.LogError(e,
                                $"Type resolver {resolver.GetType().Name} threw exception while resolving NpgsqlDbType {npgsqlDbType}");
                        }
                    }
                }

                // Can't find (or translate) PG data type name by NpgsqlDbType.
                // This might happen because of flags (like Array, Range or Multirange).
                foreach (var resolver in _handlerResolvers)
                {
                    try
                    {
                        if (resolver.ResolveByNpgsqlDbType(npgsqlDbType) is { } handler)
                            return _handlersByNpgsqlDbType[npgsqlDbType] = handler;
                    }
                    catch (Exception e)
                    {
                        _commandLogger.LogError(e,
                            $"Type resolver {resolver.GetType().Name} threw exception while resolving NpgsqlDbType {npgsqlDbType}");
                    }
                }

                if (npgsqlDbType.HasFlag(NpgsqlDbType.Array))
                {
                    var elementHandler = ResolveByNpgsqlDbType(npgsqlDbType & ~NpgsqlDbType.Array);

                    if (elementHandler.PostgresType.Array is not { } pgArrayType)
                        throw new ArgumentException(
                            $"No array type could be found in the database for element {elementHandler.PostgresType}");

                    return _handlersByNpgsqlDbType[npgsqlDbType] =
                        elementHandler.CreateArrayHandler(pgArrayType, Connector.Settings.ArrayNullabilityMode);
                }

                throw new NpgsqlException($"The NpgsqlDbType '{npgsqlDbType}' isn't present in your database. " +
                                          "You may need to install an extension or upgrade to a newer version.");
            }
        }
    }

    internal NpgsqlTypeHandler ResolveByDataTypeName(string typeName)
        => ResolveByDataTypeNameCore(typeName) ?? ResolveComplexTypeByDataTypeName(typeName, throwOnError: true)!;

    NpgsqlTypeHandler? ResolveByDataTypeNameCore(string typeName)
    {
        if (_handlersByDataTypeName.TryGetValue(typeName, out var handler))
            return handler;

        return ResolveLong(typeName);

        NpgsqlTypeHandler? ResolveLong(string typeName)
        {
            lock (_writeLock)
            {
                foreach (var resolver in _handlerResolvers)
                {
                    try
                    {
                        if (resolver.ResolveByDataTypeName(typeName) is { } handler)
                            return _handlersByDataTypeName[typeName] = handler;
                    }
                    catch (Exception e)
                    {
                        _commandLogger.LogError(e, $"Type resolver {resolver.GetType().Name} threw exception while resolving data type name {typeName}");
                    }
                }

                return null;
            }
        }
    }

    NpgsqlTypeHandler? ResolveByPostgresType(PostgresType type)
    {
        if (_handlersByDataTypeName.TryGetValue(type.FullName, out var handler))
            return handler;

        return ResolveLong(type);

        NpgsqlTypeHandler? ResolveLong(PostgresType type)
        {
            lock (_writeLock)
            {
                foreach (var resolver in _handlerResolvers)
                {
                    try
                    {
                        if (resolver.ResolveByPostgresType(type) is { } handler)
                            return _handlersByDataTypeName[type.FullName] = handler;
                    }
                    catch (Exception e)
                    {
                        _commandLogger.LogError(e, $"Type resolver {resolver.GetType().Name} threw exception while resolving data type name {type.FullName}");
                    }
                }

                return null;
            }
        }
    }

    NpgsqlTypeHandler? ResolveComplexTypeByDataTypeName(string typeName, bool throwOnError)
    {
        lock (_writeLock)
        {
            var pgType = DatabaseInfo.GetPostgresTypeByName(typeName);

            switch (pgType)
            {
            case PostgresArrayType pgArrayType:
            {
                var elementHandler = ResolveByOID(pgArrayType.Element.OID);
                return _handlersByDataTypeName[typeName] =
                    elementHandler.CreateArrayHandler(pgArrayType, Connector.Settings.ArrayNullabilityMode);
            }

            case PostgresEnumType pgEnumType:
            {
                // A mapped enum would have been registered in _extraHandlersByDataTypeName and bound above - this is unmapped.
                return _handlersByDataTypeName[typeName] =
                    new UnmappedEnumHandler(pgEnumType, _defaultNameTranslator, Connector.TextEncoding);
            }

            case PostgresDomainType pgDomainType:
                return _handlersByDataTypeName[typeName] = ResolveByOID(pgDomainType.BaseType.OID);

            case PostgresBaseType pgBaseType:
                return throwOnError
                    ? throw new NotSupportedException($"PostgreSQL type '{pgBaseType}' isn't supported by Npgsql")
                    : null;

            case PostgresCompositeType pgCompositeType:
                // We don't support writing unmapped composite types, but we do support reading unmapped composite types.
                // So when we're invoked from ResolveOID (which is the read path), we don't want to raise an exception.
                return throwOnError
                    ? throw new NotSupportedException(
                        $"Composite type '{pgCompositeType}' must be mapped with Npgsql before being used, see the docs.")
                    : null;

#pragma warning disable CS0618
            case PostgresRangeType:
            case PostgresMultirangeType:
                return throwOnError
                    ? throw new NotSupportedException(
                        $"'{pgType}' is a range type; please call {nameof(NpgsqlSlimDataSourceBuilder.EnableRanges)} on {nameof(NpgsqlSlimDataSourceBuilder)} to enable ranges. " +
                        "See https://www.npgsql.org/doc/types/ranges.html for more information.")
                    : null;
#pragma warning restore CS0618

            default:
                throw new ArgumentOutOfRangeException($"Unhandled PostgreSQL type type: {pgType.GetType()}");
            }
        }
    }

    internal NpgsqlTypeHandler ResolveByValue<T>(T value)
    {
        if (value is null)
            return ResolveByClrType(typeof(T));

        if (typeof(T).IsValueType)
        {
            // Attempt to resolve value types generically via the resolver. This is the efficient fast-path, where we don't even need to
            // do a dictionary lookup (the JIT elides type checks in generic methods for value types)
            NpgsqlTypeHandler? handler;

            foreach (var resolver in _handlerResolvers)
            {
                try
                {
                    if ((handler = resolver.ResolveValueTypeGenerically(value)) is not null)
                        return handler;
                }
                catch (Exception e)
                {
                    _commandLogger.LogError(e, $"Type resolver {resolver.GetType().Name} threw exception while resolving value with type {typeof(T)}");
                }
            }

            // There may still be some value types not resolved by the above, e.g. NpgsqlRange
        }

        // Value types would have been resolved above, so this is a reference type - no JIT optimizations.
        // We go through the regular logic (and there's no boxing).
        return ResolveByValue((object)value);
    }

    internal NpgsqlTypeHandler ResolveByValue(object value)
    {
        // We resolve as follows:
        // 1. Cached by-type lookup (fast path). This will work for almost all types after the very first resolution.
        // 2. Value-dependent type lookup (e.g. DateTime by Kind) via the resolvers. This includes complex types (e.g. array/range
        //    over DateTime), and the results cannot be cached.
        // 3. Uncached by-type lookup (for the very first resolution of a given type)

        var type = value.GetType();
        if (_handlersByClrType.TryGetValue(type, out var handler))
            return handler;

        return ResolveLong(value, type);

        NpgsqlTypeHandler ResolveLong(object value, Type type)
        {
            foreach (var resolver in _handlerResolvers)
            {
                try
                {
                    if (resolver.ResolveValueDependentValue(value) is { } handler)
                        return handler;
                }
                catch (Exception e)
                {
                    _commandLogger.LogError(e, $"Type resolver {resolver.GetType().Name} threw exception while resolving value with type {type}");
                }
            }

            // ResolveByClrType either throws, or resolves a handler and caches it in _handlersByClrType (where it would be found above the
            // next time we resolve this type)
            return ResolveByClrType(type);
        }
    }

    // TODO: This is needed as a separate method only because of binary COPY, see #3957
    /// <summary>
    /// Looks up a type handler by CLR Type.
    /// </summary>
    /// <param name="type">Parameter's CLR type</param>
    /// <returns>A type handler that can be used to encode and decode values.</returns>
    public NpgsqlTypeHandler ResolveByClrType(Type type)
    {
        if (_handlersByClrType.TryGetValue(type, out var handler))
            return handler;

        return ResolveLong(type);

        NpgsqlTypeHandler ResolveLong(Type type)
        {
            lock (_writeLock)
            {
                foreach (var resolver in _handlerResolvers)
                {
                    try
                    {
                        if (resolver.ResolveByClrType(type) is { } handler)
                            return _handlersByClrType[type] = handler;
                    }
                    catch (Exception e)
                    {
                        _commandLogger.LogError(e, $"Type resolver {resolver.GetType().Name} threw exception while resolving value with type {type}");
                    }
                }

                // Try to see if it is an array type
                var arrayElementType = GetArrayListElementType(type);
                if (arrayElementType is not null)
                {
                    if (ResolveByClrType(arrayElementType) is not { } elementHandler)
                        throw new ArgumentException($"Array type over CLR type {arrayElementType.Name} isn't supported by Npgsql");

                    if (elementHandler.PostgresType.Array is not { } pgArrayType)
                        throw new ArgumentException(
                            $"No array type could be found in the database for element {elementHandler.PostgresType}");

                    return _handlersByClrType[type] =
                        elementHandler.CreateArrayHandler(pgArrayType, Connector.Settings.ArrayNullabilityMode);
                }

                if (Nullable.GetUnderlyingType(type) is { } underlyingType && ResolveByClrType(underlyingType) is { } underlyingHandler)
                    return _handlersByClrType[type] = underlyingHandler;

                if (type.IsEnum)
                {
                    return DatabaseInfo.TryGetPostgresTypeByName(GetPgName(type, _defaultNameTranslator), out var pgType)
                           && pgType is PostgresEnumType pgEnumType
                        ? _handlersByClrType[type] = new UnmappedEnumHandler(pgEnumType, _defaultNameTranslator, Connector.TextEncoding)
                        : throw new NotSupportedException(
                            $"Could not find a PostgreSQL enum type corresponding to {type.Name}. " +
                            "Consider mapping the enum before usage, refer to the documentation for more details.");
                }

                if (typeof(IEnumerable).IsAssignableFrom(type))
                    throw new NotSupportedException("IEnumerable parameters are not supported, pass an array or List instead");

                throw new NotSupportedException($"The CLR type {type} isn't natively supported by Npgsql or your PostgreSQL. " +
                                                $"To use it with a PostgreSQL composite you need to specify {nameof(NpgsqlParameter.DataTypeName)} or to map it, please refer to the documentation.");
            }

            static Type? GetArrayListElementType(Type type)
            {
                var typeInfo = type.GetTypeInfo();
                if (typeInfo.IsArray)
                    return GetUnderlyingType(type.GetElementType()!); // The use of bang operator is justified here as Type.GetElementType() only returns null for the Array base class which can't be mapped in a useful way.

                var ilist = typeInfo.ImplementedInterfaces.FirstOrDefault(x => x.GetTypeInfo().IsGenericType && x.GetGenericTypeDefinition() == typeof(IList<>));
                if (ilist != null)
                    return GetUnderlyingType(ilist.GetGenericArguments()[0]);

                if (typeof(IList).IsAssignableFrom(type))
                    throw new NotSupportedException("Non-generic IList is a supported parameter, but the NpgsqlDbType parameter must be set on the parameter");

                return null;

                Type GetUnderlyingType(Type t)
                    => Nullable.GetUnderlyingType(t) ?? t;
            }   
        }
    }

    #endregion Type handler lookup

    internal bool TryGetMapping(PostgresType pgType, [NotNullWhen(true)] out TypeMappingInfo? mapping)
    {
        foreach (var resolver in _mappingResolvers)
            if ((mapping = resolver.GetMappingByPostgresType(this, pgType)) is not null)
                return true;

        switch (pgType)
        {
        case PostgresArrayType pgArrayType:
            if (TryGetMapping(pgArrayType.Element, out var elementMapping))
            {
                mapping = new(elementMapping.NpgsqlDbType | NpgsqlDbType.Array, pgType.DisplayName);
                return true;
            }

            break;

        case PostgresDomainType pgDomainType:
            if (TryGetMapping(pgDomainType.BaseType, out var baseMapping))
            {
                mapping = new(baseMapping.NpgsqlDbType, pgType.DisplayName, baseMapping.ClrTypes);
                return true;
            }

            break;

        case PostgresEnumType or PostgresCompositeType:
            return _userTypeMappings.TryGetValue(pgType.OID, out mapping);
        }

        mapping = null;
        return false;
    }

    internal (NpgsqlDbType? npgsqlDbType, PostgresType postgresType) GetTypeInfoByOid(uint oid)
    {
        if (!DatabaseInfo.ByOID.TryGetValue(oid, out var pgType))
            ThrowHelper.ThrowInvalidOperationException($"Couldn't find PostgreSQL type with OID {oid}");

        if (TryGetMapping(pgType, out var mapping))
            return (mapping.NpgsqlDbType, pgType);

        return (null, pgType);
    }

    static string GetPgName(Type clrType, INpgsqlNameTranslator nameTranslator)
        => clrType.GetCustomAttribute<PgNameAttribute>()?.PgName
           ?? nameTranslator.TranslateTypeName(clrType.Name);
}
