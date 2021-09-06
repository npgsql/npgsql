using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandlers;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using NpgsqlTypes;

namespace Npgsql.TypeMapping
{
    sealed class ConnectorTypeMapper : TypeMapperBase
    {
        internal NpgsqlConnector Connector { get; }

        NpgsqlDatabaseInfo? _databaseInfo;

        internal NpgsqlDatabaseInfo DatabaseInfo
        {
            get => _databaseInfo ?? throw new InvalidOperationException("Internal error: this type mapper hasn't yet been bound to a database info object");
            set
            {
                _databaseInfo = value;
                Reset();
            }
        }

        ITypeHandlerResolver[] _resolvers;
        internal NpgsqlTypeHandler UnrecognizedTypeHandler { get; }

        readonly ConcurrentDictionary<uint, NpgsqlTypeHandler> _handlersByOID = new();
        readonly ConcurrentDictionary<NpgsqlDbType, NpgsqlTypeHandler> _handlersByNpgsqlDbType = new();
        readonly ConcurrentDictionary<Type, NpgsqlTypeHandler> _handlersByClrType = new();
        readonly ConcurrentDictionary<string, NpgsqlTypeHandler> _handlersByDataTypeName = new();

        readonly Dictionary<uint, TypeMappingInfo> _userTypeMappings = new();

        /// <summary>
        /// Copy of <see cref="GlobalTypeMapper.ChangeCounter"/> at the time when this
        /// mapper was created, to detect mapping changes. If changes are made to this connection's
        /// mapper, the change counter is set to -1.
        /// </summary>
        internal int ChangeCounter { get; private set; }

        #region Construction

        internal ConnectorTypeMapper(NpgsqlConnector connector) : base(GlobalTypeMapper.Instance.DefaultNameTranslator)
        {
            Connector = connector;
            UnrecognizedTypeHandler = new UnknownTypeHandler(Connector);
            _resolvers = Array.Empty<ITypeHandlerResolver>();
        }

        #endregion Constructors

        #region Type handler lookup

        /// <summary>
        /// Looks up a type handler by its PostgreSQL type's OID.
        /// </summary>
        /// <param name="oid">A PostgreSQL type OID</param>
        /// <returns>A type handler that can be used to encode and decode values.</returns>
        internal NpgsqlTypeHandler ResolveByOID(uint oid)
            => TryResolveByOID(oid, out var result) ? result : UnrecognizedTypeHandler;

        internal bool TryResolveByOID(uint oid, [NotNullWhen(true)] out NpgsqlTypeHandler? handler)
        {
            if (_handlersByOID.TryGetValue(oid, out handler))
                return true;

            foreach (var resolver in _resolvers)
            {
                if ((handler = resolver.ResolveByOID(oid)) is not null)
                {
                    _handlersByOID[oid] = handler;
                    return true;
                }
            }

            if (!DatabaseInfo.ByOID.TryGetValue(oid, out var pgType))
                return false;

            switch (pgType)
            {
            case PostgresArrayType pgArrayType:
            {
                // TODO: This will throw, but we're in TryResolve. Figure out the whole Try/non-Try strategy here.
                var elementHandler = ResolveByOID(pgArrayType.Element.OID);
                handler = _handlersByOID[oid] =
                    elementHandler.CreateArrayHandler(pgArrayType, Connector.Settings.ArrayNullabilityMode);
                return true;
            }

            case PostgresRangeType pgRangeType:
            {
                // TODO: This will throw, but we're in TryResolve. Figure out the whole Try/non-Try strategy here.
                var subtypeHandler = ResolveByOID(pgRangeType.Subtype.OID);
                handler = _handlersByOID[oid] = subtypeHandler.CreateRangeHandler(pgRangeType);
                return true;
            }

            case PostgresMultirangeType pgMultirangeType:
            {
                // TODO: This will throw, but we're in TryResolve. Figure out the whole Try/non-Try strategy here.
                var subtypeHandler = ResolveByOID(pgMultirangeType.Subrange.Subtype.OID);
                handler = _handlersByOID[oid] = subtypeHandler.CreateMultirangeHandler(pgMultirangeType);
                return true;
            }

            case PostgresEnumType pgEnumType:
            {
                // A mapped enum would have been registered in _extraHandlersByOID and returned above - this is unmapped.
                handler = _handlersByOID[oid] = new UnmappedEnumHandler(pgEnumType, DefaultNameTranslator, Connector.TextEncoding);
                return true;
            }

            case PostgresDomainType pgDomainType:
            {
                // Note that when when sending back domain types, PG sends back the type OID of their base type - so in regular
                // circumstances we never need to resolve domains from a type OID.
                // However, when a domain is part of a composite type, the domain's type OID is sent, so we support this here.
                // TODO: This will throw, but we're in TryResolve. Figure out the whole Try/non-Try strategy here.
                handler = _handlersByOID[oid] = ResolveByOID(pgDomainType.BaseType.OID);
                return true;
            }

            default:
                handler = null;
                return false;
            }
        }

        internal NpgsqlTypeHandler ResolveByNpgsqlDbType(NpgsqlDbType npgsqlDbType)
        {
            if (_handlersByNpgsqlDbType.TryGetValue(npgsqlDbType, out var handler))
                return handler;

            if (TryResolve(npgsqlDbType, out handler))
                return _handlersByNpgsqlDbType[npgsqlDbType] = handler;

            if (npgsqlDbType.HasFlag(NpgsqlDbType.Array))
            {
                if (!TryResolve(npgsqlDbType & ~NpgsqlDbType.Array, out var elementHandler))
                    throw new ArgumentException($"Array type over NpgsqlDbType {npgsqlDbType} isn't supported by Npgsql");

                if (elementHandler.PostgresType.Array is not { } pgArrayType)
                    throw new ArgumentException($"No array type could be found in the database for element {elementHandler.PostgresType}");

                return _handlersByNpgsqlDbType[npgsqlDbType] =
                    elementHandler.CreateArrayHandler(pgArrayType, Connector.Settings.ArrayNullabilityMode);
            }

            if (npgsqlDbType.HasFlag(NpgsqlDbType.Range))
            {
                if (!TryResolve(npgsqlDbType & ~NpgsqlDbType.Range, out var subtypeHandler))
                    throw new ArgumentException($"Range type over NpgsqlDbType {npgsqlDbType} isn't supported by Npgsql");

                if (subtypeHandler.PostgresType.Range is not { } pgRangeType)
                    throw new ArgumentException($"No range type could be found in the database for subtype {subtypeHandler.PostgresType}");

                return _handlersByNpgsqlDbType[npgsqlDbType] = subtypeHandler.CreateRangeHandler(pgRangeType);
            }

            if (npgsqlDbType.HasFlag(NpgsqlDbType.Multirange))
            {
                if (!TryResolve(npgsqlDbType & ~NpgsqlDbType.Multirange, out var subtypeHandler))
                    throw new ArgumentException($"Multirange type over NpgsqlDbType {npgsqlDbType} isn't supported by Npgsql");

                if (subtypeHandler.PostgresType.Range?.Multirange is not { } pgMultirangeType)
                    throw new ArgumentException($"No multirange type could be found in the database for subtype {subtypeHandler.PostgresType}");

                return _handlersByNpgsqlDbType[npgsqlDbType] = subtypeHandler.CreateMultirangeHandler(pgMultirangeType);
            }

            throw new NpgsqlException($"The NpgsqlDbType '{npgsqlDbType}' isn't present in your database. " +
                                      "You may need to install an extension or upgrade to a newer version.");

            bool TryResolve(NpgsqlDbType npgsqlDbType, [NotNullWhen(true)] out NpgsqlTypeHandler? handler)
            {
                if (GlobalTypeMapper.NpgsqlDbTypeToDataTypeName(npgsqlDbType) is { } dataTypeName)
                    foreach (var resolver in _resolvers)
                        if ((handler = resolver.ResolveByDataTypeName(dataTypeName)) is not null)
                            return true;

                handler = null;
                return false;
            }
        }

        internal NpgsqlTypeHandler ResolveByDataTypeName(string typeName)
        {
            if (_handlersByDataTypeName.TryGetValue(typeName, out var handler))
                return handler;

            foreach (var resolver in _resolvers)
                if ((handler = resolver.ResolveByDataTypeName(typeName)) is not null)
                    return _handlersByDataTypeName[typeName] = handler;

            if (DatabaseInfo.GetPostgresTypeByName(typeName) is not { } pgType)
                throw new NotSupportedException("Could not find PostgreSQL type " + typeName);

            switch (pgType)
            {
            case PostgresArrayType pgArrayType:
            {
                var elementHandler = ResolveByOID(pgArrayType.Element.OID);
                return _handlersByDataTypeName[typeName] =
                    elementHandler.CreateArrayHandler(pgArrayType, Connector.Settings.ArrayNullabilityMode);
            }

            case PostgresRangeType pgRangeType:
            {
                var subtypeHandler = ResolveByOID(pgRangeType.Subtype.OID);
                return _handlersByDataTypeName[typeName] = subtypeHandler.CreateRangeHandler(pgRangeType);
            }

            case PostgresMultirangeType pgMultirangeType:
            {
                var subtypeHandler = ResolveByOID(pgMultirangeType.Subrange.Subtype.OID);
                return _handlersByDataTypeName[typeName] = subtypeHandler.CreateMultirangeHandler(pgMultirangeType);
            }

            case PostgresEnumType pgEnumType:
            {
                // A mapped enum would have been registered in _extraHandlersByDataTypeName and bound above - this is unmapped.
                return _handlersByDataTypeName[typeName] =
                    new UnmappedEnumHandler(pgEnumType, DefaultNameTranslator, Connector.TextEncoding);
            }

            case PostgresDomainType pgDomainType:
                return _handlersByDataTypeName[typeName] = ResolveByOID(pgDomainType.BaseType.OID);

            case PostgresBaseType pgBaseType:
                throw new NotSupportedException($"PostgreSQL type '{pgBaseType}' isn't supported by Npgsql");

            case PostgresCompositeType pgCompositeType:
                throw new NotSupportedException(
                    $"Composite type '{pgCompositeType}' must be mapped with Npgsql before being used, see the docs.");

            default:
                throw new ArgumentOutOfRangeException($"Unhandled PostgreSQL type type: {pgType.GetType()}");
            }
        }

        internal NpgsqlTypeHandler ResolveByClrType(Type type)
        {
            if (_handlersByClrType.TryGetValue(type, out var handler))
                return handler;

            foreach (var resolver in _resolvers)
                if ((handler = resolver.ResolveByClrType(type)) is not null)
                    return _handlersByClrType[type] = handler;

            // Try to see if it is an array type
            var arrayElementType = GetArrayListElementType(type);
            if (arrayElementType is not null)
            {
                // Arrays over range types are multiranges, not regular arrays.
                if (arrayElementType.IsGenericType && arrayElementType.GetGenericTypeDefinition() == typeof(NpgsqlRange<>))
                {
                    var subtypeType = arrayElementType.GetGenericArguments()[0];

                    return ResolveByClrType(subtypeType) is { PostgresType : { Range : { Multirange: { } pgMultirangeType } } } subtypeHandler
                        ? _handlersByClrType[type] = subtypeHandler.CreateMultirangeHandler(pgMultirangeType)
                        : throw new NotSupportedException($"The CLR range type {type} isn't supported by Npgsql or your PostgreSQL.");
                }

                if (ResolveByClrType(arrayElementType) is not { } elementHandler)
                    throw new ArgumentException($"Array type over CLR type {arrayElementType.Name} isn't supported by Npgsql");

                if (elementHandler.PostgresType.Array is not { } pgArrayType)
                    throw new ArgumentException($"No array type could be found in the database for element {elementHandler.PostgresType}");

                return _handlersByClrType[type] =
                    elementHandler.CreateArrayHandler(pgArrayType, Connector.Settings.ArrayNullabilityMode);
            }

            if (Nullable.GetUnderlyingType(type) is { } underlyingType && ResolveByClrType(underlyingType) is { } underlyingHandler)
                return _handlersByClrType[type] = underlyingHandler;

            if (type.IsEnum)
            {
                return DatabaseInfo.GetPostgresTypeByName(GetPgName(type, DefaultNameTranslator)) is PostgresEnumType pgEnumType
                    ? _handlersByClrType[type] = new UnmappedEnumHandler(pgEnumType, DefaultNameTranslator, Connector.TextEncoding)
                    : throw new NotSupportedException(
                        $"Could not find a PostgreSQL enum type corresponding to {type.Name}. " +
                        "Consider mapping the enum before usage, refer to the documentation for more details.");
            }

            // TODO: We can make the following compatible with reflection-free mode by having NpgsqlRange implement some interface, and
            // check for that.
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(NpgsqlRange<>))
            {
                var subtypeType = type.GetGenericArguments()[0];

                return ResolveByClrType(subtypeType) is { PostgresType : { Range : { } pgRangeType } } subtypeHandler
                    ? _handlersByClrType[type] = subtypeHandler.CreateRangeHandler(pgRangeType)
                    : throw new NotSupportedException($"The CLR range type {type} isn't supported by Npgsql or your PostgreSQL.");
            }

            if (typeof(IEnumerable).IsAssignableFrom(type))
                throw new NotSupportedException("IEnumerable parameters are not supported, pass an array or List instead");

            throw new NotSupportedException($"The CLR type {type} isn't natively supported by Npgsql or your PostgreSQL. " +
                                            $"To use it with a PostgreSQL composite you need to specify {nameof(NpgsqlParameter.DataTypeName)} or to map it, please refer to the documentation.");

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

        internal bool TryGetMapping(PostgresType pgType, [NotNullWhen(true)] out TypeMappingInfo? mapping)
        {
            foreach (var resolver in _resolvers)
                if (resolver.GetDataTypeNameByOID(pgType.OID) is { } dataTypeName && (mapping = resolver.GetMappingByDataTypeName(dataTypeName)) is not null)
                    return true;

            switch (pgType)
            {
            case PostgresArrayType pgArrayType:
                if (TryGetMapping(pgArrayType.Element, out var elementMapping))
                {
                    mapping = new(elementMapping.NpgsqlDbType | NpgsqlDbType.Array, DbType.Object, pgType.DisplayName);
                    return true;
                }

                break;

            case PostgresRangeType pgRangeType:
            {
                if (TryGetMapping(pgRangeType.Subtype, out var subtypeMapping))
                {
                    mapping = new(subtypeMapping.NpgsqlDbType | NpgsqlDbType.Range, DbType.Object, pgType.DisplayName);
                    return true;
                }

                break;
            }

            case PostgresMultirangeType pgMultirangeType:
            {
                if (TryGetMapping(pgMultirangeType.Subrange.Subtype, out var subtypeMapping))
                {
                    mapping = new(subtypeMapping.NpgsqlDbType | NpgsqlDbType.Multirange, DbType.Object, pgType.DisplayName);
                    return true;
                }

                break;
            }

            case PostgresDomainType pgDomainType:
                if (TryGetMapping(pgDomainType.BaseType, out var baseMapping))
                {
                    mapping = new(baseMapping.NpgsqlDbType, baseMapping.DbType, pgType.DisplayName, baseMapping.ClrTypes);
                    return true;
                }

                break;

            case PostgresEnumType or PostgresCompositeType:
                return _userTypeMappings.TryGetValue(pgType.OID, out mapping);
            }

            mapping = null;
            return false;
        }

        #endregion Type handler lookup

        #region Mapping management

        public override INpgsqlTypeMapper MapEnum<TEnum>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
        {
            if (pgName != null && pgName.Trim() == "")
                throw new ArgumentException("pgName can't be empty", nameof(pgName));

            nameTranslator ??= DefaultNameTranslator;
            pgName ??= GetPgName(typeof(TEnum), nameTranslator);

            if (DatabaseInfo.GetPostgresTypeByName(pgName) is not PostgresEnumType pgEnumType)
                throw new InvalidCastException($"Cannot map enum type {typeof(TEnum).Name} to PostgreSQL type {pgName} which isn't an enum");

            var handler = new UserEnumTypeMapping<TEnum>(pgName, nameTranslator).CreateHandler(pgEnumType, Connector);

            ApplyUserMapping(pgEnumType, typeof(TEnum), handler);

            return this;
        }

        public override bool UnmapEnum<TEnum>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
        {
            if (pgName != null && pgName.Trim() == "")
                throw new ArgumentException("pgName can't be empty", nameof(pgName));

            nameTranslator ??= DefaultNameTranslator;
            pgName ??= GetPgName(typeof(TEnum), nameTranslator);

            var userEnumMapping = new UserEnumTypeMapping<TEnum>(pgName, nameTranslator);

            if (DatabaseInfo.GetPostgresTypeByName(pgName) is not PostgresEnumType pgEnumType)
                throw new InvalidCastException($"Could not find {pgName}");

            var found = _handlersByOID.TryRemove(pgEnumType.OID, out _);
            found |= _handlersByClrType.TryRemove(userEnumMapping.ClrType, out _);
            found |= _handlersByDataTypeName.TryRemove(userEnumMapping.PgTypeName, out _);
            return found;
        }

        public override INpgsqlTypeMapper MapComposite<T>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
        {
            if (pgName != null && pgName.Trim() == "")
                throw new ArgumentException("pgName can't be empty", nameof(pgName));

            nameTranslator ??= DefaultNameTranslator;
            pgName ??= GetPgName(typeof(T), nameTranslator);

            if (DatabaseInfo.GetPostgresTypeByName(pgName) is not PostgresCompositeType pgCompositeType)
            {
                throw new InvalidCastException(
                    $"Cannot map composite type {typeof(T).Name} to PostgreSQL type {pgName} which isn't a composite");
            }

            var handler = new UserCompositeTypeMapping<T>(pgName, nameTranslator).CreateHandler(pgCompositeType, Connector);

            ApplyUserMapping(pgCompositeType, typeof(T), handler);

            return this;
        }

        public override INpgsqlTypeMapper MapComposite(Type clrType, string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
        {
            if (pgName != null && pgName.Trim() == "")
                throw new ArgumentException("pgName can't be empty", nameof(pgName));

            nameTranslator ??= DefaultNameTranslator;
            pgName ??= GetPgName(clrType, nameTranslator);

            if (DatabaseInfo.GetPostgresTypeByName(pgName) is not PostgresCompositeType pgCompositeType)
            {
                throw new InvalidCastException(
                    $"Cannot map composite type {clrType.Name} to PostgreSQL type {pgName} which isn't a composite");
            }

            var userCompositeMapping =
                (IUserTypeMapping)Activator.CreateInstance(typeof(UserCompositeTypeMapping<>).MakeGenericType(clrType),
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null,
                new object[] { clrType, nameTranslator }, null)!;

            var handler = userCompositeMapping.CreateHandler(pgCompositeType, Connector);

            ApplyUserMapping(pgCompositeType, clrType, handler);

            return this;
        }

        public override bool UnmapComposite<T>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
            => UnmapComposite(typeof(T), pgName, nameTranslator);

        public override bool UnmapComposite(Type clrType, string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
        {
            if (pgName != null && pgName.Trim() == "")
                throw new ArgumentException("pgName can't be empty", nameof(pgName));

            nameTranslator ??= DefaultNameTranslator;
            pgName ??= GetPgName(clrType, nameTranslator);

            if (DatabaseInfo.GetPostgresTypeByName(pgName) is not PostgresCompositeType pgCompositeType)
                throw new InvalidCastException($"Could not find {pgName}");

            var found = _handlersByOID.TryRemove(pgCompositeType.OID, out _);
            found |= _handlersByClrType.TryRemove(clrType, out _);
            found |= _handlersByDataTypeName.TryRemove(pgName, out _);
            return found;
        }

        void ApplyUserMapping(PostgresType pgType, Type clrType, NpgsqlTypeHandler handler)
        {
            _handlersByOID[pgType.OID] =
                _handlersByDataTypeName[pgType.FullName] =
                    _handlersByDataTypeName[pgType.Name] =
                        _handlersByClrType[clrType] = handler;

            _userTypeMappings[pgType.OID] = new(npgsqlDbType: null, DbType.Object, pgType.Name, clrType);
        }

        public override void AddTypeResolverFactory(ITypeHandlerResolverFactory resolverFactory)
        {
            var sw = new SpinWait();
            while (true)
            {
                var oldResolvers = _resolvers;
                var newResolvers = new ITypeHandlerResolver[_resolvers.Length + 1];
                Array.Copy(oldResolvers, 0, newResolvers, 1, _resolvers.Length);
                newResolvers[0] = resolverFactory.Create(Connector);

                if (Interlocked.CompareExchange(ref _resolvers, newResolvers, oldResolvers) == oldResolvers)
                {
                    _handlersByOID.Clear();
                    _handlersByNpgsqlDbType.Clear();
                    _handlersByClrType.Clear();
                    _handlersByDataTypeName.Clear();

                    ChangeCounter = -1;
                    return;
                }

                sw.SpinOnce();
            }
        }

        [MemberNotNull(nameof(_resolvers))]
        public override void Reset()
        {
            var globalMapper = GlobalTypeMapper.Instance;
            globalMapper.Lock.EnterReadLock();
            try
            {
                _handlersByOID.Clear();
                _handlersByNpgsqlDbType.Clear();
                _handlersByClrType.Clear();
                _handlersByDataTypeName.Clear();

                _resolvers = new ITypeHandlerResolver[globalMapper.ResolverFactories.Count];
                for (var i = 0; i < _resolvers.Length; i++)
                    _resolvers[i] = globalMapper.ResolverFactories[i].Create(Connector);

                _userTypeMappings.Clear();

                foreach (var userTypeMapping in globalMapper.UserTypeMappings.Values)
                {
                    if (DatabaseInfo.TryGetPostgresTypeByName(userTypeMapping.PgTypeName, out var pgType))
                    {
                        ApplyUserMapping(pgType, userTypeMapping.ClrType, userTypeMapping.CreateHandler(pgType, Connector));
                    }
                }

                ChangeCounter = GlobalTypeMapper.Instance.ChangeCounter;
            }
            finally
            {
                globalMapper.Lock.ExitReadLock();
            }
        }

        #endregion Mapping management

        internal (NpgsqlDbType? npgsqlDbType, PostgresType postgresType) GetTypeInfoByOid(uint oid)
        {
            if (!DatabaseInfo.ByOID.TryGetValue(oid, out var postgresType))
                throw new InvalidOperationException($"Couldn't find PostgreSQL type with OID {oid}");

            if (TryGetMapping(postgresType, out var mapping))
                return (mapping.NpgsqlDbType, postgresType);

            return postgresType switch
            {
                PostgresArrayType pgArrayType when TryGetMapping(pgArrayType.Element, out var elementMapping)
                    => new(elementMapping.NpgsqlDbType | NpgsqlDbType.Array, postgresType),

                PostgresDomainType { BaseType: PostgresArrayType baseType }
                    when TryGetMapping(baseType.Element, out var baseTypeElementNpgsqlTypeMapping)
                    => (baseTypeElementNpgsqlTypeMapping.NpgsqlDbType | NpgsqlDbType.Array, postgresType),

                _ => (null, postgresType)
            };

            bool TryGetMapping(PostgresType pgType, [NotNullWhen(true)] out TypeMappingInfo? mapping)
            {
                foreach (var resolver in _resolvers)
                {
                    if (resolver.GetDataTypeNameByOID(pgType.OID) is { } dataTypeName &&
                        (mapping = resolver.GetMappingByDataTypeName(dataTypeName)) is not null)
                    {
                        return true;
                    }
                }

                mapping = null;
                return false;
            }
        }
    }
}
