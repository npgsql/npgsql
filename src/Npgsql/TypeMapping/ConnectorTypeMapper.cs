using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandlers;
using Npgsql.Internal.TypeHandling;
using Npgsql.Logging;
using Npgsql.PostgresTypes;
using NpgsqlTypes;

namespace Npgsql.TypeMapping
{
    sealed class ConnectorTypeMapper : TypeMapperBase
    {
        readonly NpgsqlConnector _connector;

        NpgsqlDatabaseInfo? _databaseInfo;

        internal NpgsqlDatabaseInfo DatabaseInfo
        {
            get => _databaseInfo ?? throw new InvalidOperationException("Internal error: this type mapper hasn't yet been bound to a database info object");
            set => _databaseInfo = value;
        }

        internal NpgsqlTypeHandler UnrecognizedTypeHandler { get; }

        internal IDictionary<string, NpgsqlTypeMapping> MappingsByName { get; private set; }
        internal IDictionary<NpgsqlDbType, NpgsqlTypeMapping> MappingsByNpgsqlDbType { get; private set; }
        internal IDictionary<Type, NpgsqlTypeMapping> MappingsByClrType { get; private set; }

        readonly Dictionary<uint, NpgsqlTypeHandler> _handlersByOID = new();
        readonly Dictionary<NpgsqlDbType, NpgsqlTypeHandler> _handlersByNpgsqlDbType = new();
        readonly Dictionary<string, NpgsqlTypeHandler> _handlersByTypeName = new();
        readonly Dictionary<Type, NpgsqlTypeHandler> _handlersByClrType = new();

        /// <summary>
        /// Maps CLR types to their array handlers.
        /// </summary>
        readonly Dictionary<Type, NpgsqlTypeHandler> _arrayHandlerByClrType = new();

        /// <summary>
        /// Copy of <see cref="GlobalTypeMapper.ChangeCounter"/> at the time when this
        /// mapper was created, to detect mapping changes. If changes are made to this connection's
        /// mapper, the change counter is set to -1.
        /// </summary>
        internal int ChangeCounter { get; private set; }

        static readonly NpgsqlLogger Log = NpgsqlLogManager.CreateLogger(nameof(ConnectorTypeMapper));

        #region Construction

        internal ConnectorTypeMapper(NpgsqlConnector connector) : base(GlobalTypeMapper.Instance.DefaultNameTranslator)
        {
            _connector = connector;
            UnrecognizedTypeHandler = new UnknownTypeHandler(_connector);
            ClearBindings();
            ResetMappings();
        }

        #endregion Constructors

        #region Type handler lookup

        /// <summary>
        /// Looks up a type handler by its PostgreSQL type's OID.
        /// </summary>
        /// <param name="oid">A PostgreSQL type OID</param>
        /// <returns>A type handler that can be used to encode and decode values.</returns>
        internal NpgsqlTypeHandler GetByOID(uint oid)
            => TryGetByOID(oid, out var result) ? result : UnrecognizedTypeHandler;

        internal bool TryGetByOID(uint oid, [NotNullWhen(true)] out NpgsqlTypeHandler? handler)
        {
            if (_handlersByOID.TryGetValue(oid, out handler))
                return true;

            if (DatabaseInfo.ByOID.TryGetValue(oid, out var pgType))
            {
                if (MappingsByName.TryGetValue(pgType.Name, out var mapping))
                {
                    handler = Bind(mapping, pgType);
                    return true;
                }

                switch (pgType)
                {
                case PostgresArrayType pgArrayType when GetMapping(pgArrayType.Element) is { } elementMapping:
                    handler = BindArray(elementMapping);
                    return true;

                case PostgresRangeType pgRangeType when GetMapping(pgRangeType.Subtype) is { } subtypeMapping:
                    handler = BindRange(subtypeMapping);
                    return true;

                case PostgresEnumType pgEnumType:
                    // A mapped enum would have been registered in InternalMappings and bound above - this is unmapped.
                    handler = BindUnmappedEnum(pgEnumType);
                    return true;

                case PostgresArrayType { Element: PostgresEnumType pgEnumElementType } pgArrayType:
                    // Array over unmapped enum
                    var elementHandler = BindUnmappedEnum(pgEnumElementType);
                    handler = BindArray(elementHandler, pgArrayType);
                    return true;

                case PostgresDomainType pgDomainType:
                    // Note that when when sending back domain types, PG sends back the type OID of their base type - so in regular
                    // circumstances we never need to resolve domains from a type OID.
                    // However, when a domain is part of a composite type, the domain's type OID is sent, so we support this here.
                    if (TryGetByOID(pgDomainType.BaseType.OID, out handler))
                    {
                        _handlersByOID[oid] = handler;
                        return true;
                    }
                    return false;
                }
            }

            return false;
        }

        internal NpgsqlTypeHandler GetByNpgsqlDbType(NpgsqlDbType npgsqlDbType)
        {
            if (_handlersByNpgsqlDbType.TryGetValue(npgsqlDbType, out var handler))
                return handler;

            // TODO: revisit externalCall - things are changing. No more "binding at global time" which only needs to log - always throw?
            if (MappingsByNpgsqlDbType.TryGetValue(npgsqlDbType, out var mapping))
                return Bind(mapping);

            if (npgsqlDbType.HasFlag(NpgsqlDbType.Array))
            {
                var elementNpgsqlDbType = npgsqlDbType & ~NpgsqlDbType.Array;

                return MappingsByNpgsqlDbType.TryGetValue(elementNpgsqlDbType, out var elementMapping)
                    ? BindArray(elementMapping)
                    : throw new ArgumentException($"Could not find a mapping for array element NpgsqlDbType {elementNpgsqlDbType}");
            }

            if (npgsqlDbType.HasFlag(NpgsqlDbType.Range))
            {
                var subtypeNpgsqlDbType = npgsqlDbType & ~NpgsqlDbType.Range;

                return MappingsByNpgsqlDbType.TryGetValue(subtypeNpgsqlDbType, out var subtypeMapping)
                    ? BindRange(subtypeMapping)
                    : throw new ArgumentException($"Could not find a mapping for range subtype NpgsqlDbType {subtypeNpgsqlDbType}");
            }

            throw new NpgsqlException($"The NpgsqlDbType '{npgsqlDbType}' isn't present in your database. " +
                                      "You may need to install an extension or upgrade to a newer version.");
        }

        internal NpgsqlTypeHandler GetByDataTypeName(string typeName)
        {
            if (_handlersByTypeName.TryGetValue(typeName, out var handler))
                return handler;

            if (MappingsByName.TryGetValue(typeName, out var mapping))
                return Bind(mapping);

            if (DatabaseInfo.GetPostgresTypeByName(typeName) is { } pgType)
            {
                switch (pgType)
                {
                case PostgresArrayType pgArrayType when GetMapping(pgArrayType.Element) is { } elementMapping:
                    return BindArray(elementMapping);

                case PostgresRangeType pgRangeType when GetMapping(pgRangeType.Subtype) is { } subtypeMapping:
                    return BindRange(subtypeMapping);

                case PostgresEnumType pgEnumType:
                    // A mapped enum would have been registered in InternalMappings and bound above - this is unmapped.
                    return BindUnmappedEnum(pgEnumType);

                case PostgresArrayType { Element: PostgresEnumType pgEnumElementType } pgArrayType:
                    // Array over unmapped enum
                    var elementHandler = BindUnmappedEnum(pgEnumElementType);
                    return BindArray(elementHandler, pgArrayType);

                case PostgresDomainType pgDomainType:
                    return _handlersByTypeName[typeName] = GetByDataTypeName(pgDomainType.BaseType.Name);
                }
            }

            throw new NotSupportedException("Could not find PostgreSQL type " + typeName);
        }

        internal NpgsqlTypeHandler GetByClrType(Type type)
        {
            if (_handlersByClrType.TryGetValue(type, out var handler))
                return handler;

            if (MappingsByClrType.TryGetValue(type, out var mapping))
                return Bind(mapping);

            // Try to see if it is an array type
            var arrayElementType = GetArrayElementType(type);
            if (arrayElementType is not null)
            {
                if (_arrayHandlerByClrType.TryGetValue(arrayElementType, out handler))
                    return handler;

                return MappingsByClrType.TryGetValue(arrayElementType, out var elementMapping)
                    ? BindArray(elementMapping)
                    : throw new NotSupportedException($"The CLR array type {type} isn't supported by Npgsql or your PostgreSQL. " +
                                                      "If you wish to map it to a PostgreSQL composite type array you need to register " +
                                                      "it before usage, please refer to the documentation.");
            }

            if (Nullable.GetUnderlyingType(type) is { } underlyingType && GetByClrType(underlyingType) is { } underlyingHandler)
                return _handlersByClrType[type] = underlyingHandler;

            if (type.IsEnum)
            {
                return DatabaseInfo.GetPostgresTypeByName(GetPgName(type, DefaultNameTranslator)) is PostgresEnumType pgEnumType
                    ? BindUnmappedEnum(pgEnumType)
                    : throw new NotSupportedException(
                        $"Could not find a PostgreSQL enum type corresponding to {type.Name}. " +
                        "Consider mapping the enum before usage, refer to the documentation for more details.");
            }

            // TODO: We can make the following compatible with reflection-free mode by having NpgsqlRange implement some interface, and
            // check for that.
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(NpgsqlRange<>))
            {
                var subtypeType = type.GetGenericArguments()[0];

                return MappingsByClrType.TryGetValue(subtypeType, out var subtypeMapping)
                    ? BindRange(subtypeMapping)
                    : throw new NotSupportedException($"The CLR range type {type} isn't supported by Npgsql or your PostgreSQL.");
            }

            if (typeof(IEnumerable).IsAssignableFrom(type))
                throw new NotSupportedException("Npgsql 3.x removed support for writing a parameter with an IEnumerable value, use .ToList()/.ToArray() instead");

            throw new NotSupportedException($"The CLR type {type} isn't natively supported by Npgsql or your PostgreSQL. " +
                                            $"To use it with a PostgreSQL composite you need to specify {nameof(NpgsqlParameter.DataTypeName)} or to map it, please refer to the documentation.");
        }

        static Type? GetArrayElementType(Type type)
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

        #endregion Type handler lookup

        #region Mapping management

        public override INpgsqlTypeMapper AddMapping(NpgsqlTypeMapping mapping)
        {
            CheckReady();

            if (MappingsByName.ContainsKey(mapping.PgTypeName))
                RemoveMapping(mapping.PgTypeName);

            MappingsByName[mapping.PgTypeName] = mapping;
            if (mapping.NpgsqlDbType is not null)
                MappingsByNpgsqlDbType[mapping.NpgsqlDbType.Value] = mapping;
            foreach (var clrType in mapping.ClrTypes)
                MappingsByClrType[clrType] = mapping;

            Bind(mapping);

            ChangeCounter = -1;
            return this;
        }

        public override bool RemoveMapping(string pgTypeName)
        {
            CheckReady();

            if (!MappingsByName.TryGetValue(pgTypeName, out var mapping))
                return false;

            MappingsByName.Remove(pgTypeName);
            if (mapping.NpgsqlDbType is not null)
                MappingsByNpgsqlDbType.Remove(mapping.NpgsqlDbType.Value);
            foreach (var clrType in mapping.ClrTypes)
                MappingsByClrType.Remove(clrType);

            // Clear all bindings. We do this rather than trying to update the existing dictionaries because it's complex to remove arrays,
            // ranges...
            ClearBindings();
            ChangeCounter = -1;
            return true;
        }

        public override IEnumerable<NpgsqlTypeMapping> Mappings => MappingsByName.Values;

        void CheckReady()
        {
            if (_connector.State != ConnectorState.Ready)
                throw new InvalidOperationException("Connection must be open and idle to perform registration");
        }

        [MemberNotNull(nameof(MappingsByName), nameof(MappingsByNpgsqlDbType), nameof(MappingsByClrType))]
        void ResetMappings()
        {
            var globalMapper = GlobalTypeMapper.Instance;
            globalMapper.Lock.EnterReadLock();
            try
            {
                MappingsByName = new Dictionary<string, NpgsqlTypeMapping>(globalMapper.MappingsByName);
                MappingsByNpgsqlDbType = new Dictionary<NpgsqlDbType, NpgsqlTypeMapping>(globalMapper.MappingsByNpgsqlDbType);
                MappingsByClrType = new Dictionary<Type, NpgsqlTypeMapping>(globalMapper.MappingsByClrType);
            }
            finally
            {
                globalMapper.Lock.ExitReadLock();
            }
            ChangeCounter = GlobalTypeMapper.Instance.ChangeCounter;
        }

        void ClearBindings()
        {
            _handlersByOID.Clear();
            _handlersByNpgsqlDbType.Clear();
            _handlersByClrType.Clear();
            _arrayHandlerByClrType.Clear();

            _handlersByNpgsqlDbType[NpgsqlDbType.Unknown] = UnrecognizedTypeHandler;
            _handlersByClrType[typeof(DBNull)] = UnrecognizedTypeHandler;
        }

        public override void Reset()
        {
            ClearBindings();
            ResetMappings();
        }

        #endregion Mapping management

        #region Binding

        NpgsqlTypeHandler Bind(NpgsqlTypeMapping mapping, PostgresType? pgType = null)
        {
            pgType ??= GetPostgresType(mapping);
            var handler = mapping.TypeHandlerFactory.CreateNonGeneric(pgType, _connector);
            Bind(handler, pgType, mapping.NpgsqlDbType, mapping.ClrTypes);

            return handler;
        }

        void Bind(NpgsqlTypeHandler handler, PostgresType pgType, NpgsqlDbType? npgsqlDbType = null, Type[]? clrTypes = null)
        {
            if (_handlersByOID.TryGetValue(pgType.OID, out var existingHandler))
            {
                if (handler.GetType() != existingHandler.GetType())
                {
                    throw new InvalidOperationException($"Two type handlers registered on same type OID '{pgType.OID}': " +
                                                        $"{existingHandler.GetType().Name} and {handler.GetType().Name}");
                }

                return;
            }

            _handlersByOID[pgType.OID] = handler;
            _handlersByTypeName[pgType.FullName] = handler;
            _handlersByTypeName[pgType.Name] = handler;

            if (npgsqlDbType.HasValue)
            {
                var value = npgsqlDbType.Value;
                if (_handlersByNpgsqlDbType.ContainsKey(npgsqlDbType.Value))
                {
                    throw new InvalidOperationException($"Two type handlers registered on same NpgsqlDbType '{npgsqlDbType.Value}': " +
                                                        $"{_handlersByNpgsqlDbType[value].GetType().Name} and {handler.GetType().Name}");
                }

                _handlersByNpgsqlDbType[npgsqlDbType.Value] = handler;
            }

            if (clrTypes != null)
            {
                foreach (var type in clrTypes)
                {
                    if (_handlersByClrType.ContainsKey(type))
                    {
                        throw new InvalidOperationException($"Two type handlers registered on same .NET type '{type}': " +
                                                            $"{_handlersByClrType[type].GetType().Name} and {handler.GetType().Name}");
                    }

                    _handlersByClrType[type] = handler;
                }
            }
        }

        ArrayHandler BindArray(NpgsqlTypeMapping elementMapping)
        {
            if (GetPostgresType(elementMapping).Array is not { } arrayPgType)
                throw new ArgumentException($"No array type could be found in the database for element {elementMapping.PgTypeName}");

            var elementHandler = Bind(elementMapping);

            var arrayNpgsqlDbType = elementMapping.NpgsqlDbType.HasValue
                ? NpgsqlDbType.Array | elementMapping.NpgsqlDbType.Value
                : (NpgsqlDbType?)null;

            return BindArray(elementHandler, arrayPgType, arrayNpgsqlDbType, elementMapping.ClrTypes);
        }

        ArrayHandler BindArray(
            NpgsqlTypeHandler elementHandler,
            PostgresArrayType arrayPgType,
            NpgsqlDbType? arrayNpgsqlDbType = null,
            Type[]? elementClrTypes = null)
        {
            var arrayHandler = elementHandler.CreateArrayHandler(arrayPgType, _connector.Settings.ArrayNullabilityMode);

            Bind(arrayHandler, arrayPgType, arrayNpgsqlDbType);

            // Note that array handlers aren't registered in ByClrType like base types, because they handle all
            // dimension types and not just one CLR type (e.g. int[], int[,], int[,,]).
            // So the by-type lookup is special and goes via _arrayHandlerByClrType, see this[Type type]
            // TODO: register single-dimensional in _byType as a specific optimization? But avoid MakeArrayType for reflection-free mode?
            if (elementClrTypes is not null)
            {
                foreach (var elementType in elementClrTypes)
                {
                    if (_arrayHandlerByClrType.TryGetValue(elementType, out var existingArrayHandler))
                    {
                        if (arrayHandler.GetType() != existingArrayHandler.GetType())
                        {
                            throw new Exception(
                                $"Two array type handlers registered on same .NET type {elementType}: " +
                                $"{existingArrayHandler.GetType().Name} and {arrayHandler.GetType().Name}");
                        }
                    }
                    else
                        _arrayHandlerByClrType[elementType] = arrayHandler;
                }
            }

            return arrayHandler;
        }

        NpgsqlTypeHandler BindRange(NpgsqlTypeMapping subtypeMapping)
        {
            if (GetPostgresType(subtypeMapping).Range is not { } rangePgType)
                throw new ArgumentException($"No range type could be found in the database for subtype {subtypeMapping.PgTypeName}");

            var subtypeHandler = Bind(subtypeMapping);
            var rangeHandler = subtypeHandler.CreateRangeHandler(rangePgType);

            var rangeNpgsqlDbType = subtypeMapping.NpgsqlDbType.HasValue
                ? NpgsqlDbType.Range | subtypeMapping.NpgsqlDbType.Value
                : (NpgsqlDbType?)null;

            // We only want to bind supported range CLR types whose element CLR types are being bound as well.
            var clrTypes = rangeHandler.SupportedRangeClrTypes
                .Where(r => subtypeMapping.ClrTypes.Contains(r.GenericTypeArguments[0]))
                .ToArray();

            var asTypeHandler = (NpgsqlTypeHandler)rangeHandler;
            Bind(asTypeHandler, rangePgType, rangeNpgsqlDbType, clrTypes: clrTypes);

            return asTypeHandler;
        }

        NpgsqlTypeHandler BindUnmappedEnum(PostgresEnumType pgEnumType)
        {
            var unmappedEnumFactory = new UnmappedEnumTypeHandlerFactory(DefaultNameTranslator);
            var handler = unmappedEnumFactory.Create(pgEnumType, _connector);
            // TODO: Can map the enum's CLR type to prevent future lookups
            Bind(handler, pgEnumType);
            return handler;
        }

        PostgresType GetPostgresType(NpgsqlTypeMapping mapping)
        {
            var pgName = mapping.PgTypeName;

            var pgType = DatabaseInfo.GetPostgresTypeByName(pgName);

            // TODO: Revisit this
            if (pgType is PostgresDomainType)
                throw new NotSupportedException("Cannot add a mapping to a PostgreSQL domain type");

            return pgType;
        }

        NpgsqlTypeMapping? GetMapping(PostgresType pgType)
            => MappingsByName.TryGetValue(
                pgType is PostgresDomainType pgDomainType ? pgDomainType.BaseType.Name : pgType.Name,
                out var mapping)
                ? mapping
                : null;

        #endregion Binding

        internal (NpgsqlDbType? npgsqlDbType, PostgresType postgresType) GetTypeInfoByOid(uint oid)
        {
            if (!DatabaseInfo.ByOID.TryGetValue(oid, out var postgresType))
                throw new InvalidOperationException($"Couldn't find PostgreSQL type with OID {oid}");

            // Try to find the postgresType in the mappings
            if (TryGetMapping(postgresType, out var npgsqlTypeMapping))
                return (npgsqlTypeMapping.NpgsqlDbType, postgresType);

            // Try to find the elements' postgresType in the mappings
            if (postgresType is PostgresArrayType arrayType &&
                TryGetMapping(arrayType.Element, out var elementNpgsqlTypeMapping))
                return (elementNpgsqlTypeMapping.NpgsqlDbType | NpgsqlDbType.Array, postgresType);

            // Try to find the elements' postgresType of the base type in the mappings
            // this happens with domains over arrays
            if (postgresType is PostgresDomainType domainType && domainType.BaseType is PostgresArrayType baseType &&
                TryGetMapping(baseType.Element, out var baseTypeElementNpgsqlTypeMapping))
                return (baseTypeElementNpgsqlTypeMapping.NpgsqlDbType | NpgsqlDbType.Array, postgresType);

            // It might be an unmapped enum/composite type, or some other unmapped type
            return (null, postgresType);
        }

        bool TryGetMapping(PostgresType pgType, [NotNullWhen(true)] out NpgsqlTypeMapping? mapping)
            => MappingsByName.TryGetValue(pgType.Name, out mapping) ||
               MappingsByName.TryGetValue(pgType.FullName, out mapping) ||
               pgType is PostgresDomainType domain && (
                   MappingsByName.TryGetValue(domain.BaseType.Name, out mapping) ||
                   MappingsByName.TryGetValue(domain.BaseType.FullName, out mapping));
    }
}
