using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Npgsql.Logging;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandlers;
using Npgsql.TypeHandling;
using NpgsqlTypes;

namespace Npgsql.TypeMapping
{
    class ConnectorTypeMapper : TypeMapperBase
    {
        /// <summary>
        /// The connector to which this type mapper belongs.
        /// </summary>
        readonly NpgsqlConnector _connector;

        NpgsqlDatabaseInfo? _databaseInfo;

        /// <summary>
        /// Type information for the database of this mapper.
        /// </summary>
        internal NpgsqlDatabaseInfo DatabaseInfo
            => _databaseInfo ?? throw new InvalidOperationException("Internal error: this type mapper hasn't yet been bound to a database info object");

        internal NpgsqlTypeHandler UnrecognizedTypeHandler { get; }

        readonly Dictionary<uint, NpgsqlTypeHandler> _byOID = new Dictionary<uint, NpgsqlTypeHandler>();
        readonly Dictionary<NpgsqlDbType, NpgsqlTypeHandler> _byNpgsqlDbType = new Dictionary<NpgsqlDbType, NpgsqlTypeHandler>();
        readonly Dictionary<DbType, NpgsqlTypeHandler> _byDbType = new Dictionary<DbType, NpgsqlTypeHandler>();
        readonly Dictionary<string, NpgsqlTypeHandler> _byTypeName = new Dictionary<string, NpgsqlTypeHandler>();

        /// <summary>
        /// Maps CLR types to their type handlers.
        /// </summary>
        readonly Dictionary<Type, NpgsqlTypeHandler> _byClrType= new Dictionary<Type, NpgsqlTypeHandler>();

        /// <summary>
        /// Maps CLR types to their array handlers.
        /// </summary>
        readonly Dictionary<Type, NpgsqlTypeHandler> _arrayHandlerByClrType = new Dictionary<Type, NpgsqlTypeHandler>();

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
            UnrecognizedTypeHandler = new UnknownTypeHandler(_connector.Connection!);
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
            => _byOID.TryGetValue(oid, out handler);

        internal NpgsqlTypeHandler GetByNpgsqlDbType(NpgsqlDbType npgsqlDbType)
            => _byNpgsqlDbType.TryGetValue(npgsqlDbType, out var handler)
                ? handler
                : throw new NpgsqlException($"The NpgsqlDbType '{npgsqlDbType}' isn't present in your database. " +
                                             "You may need to install an extension or upgrade to a newer version.");


        internal NpgsqlTypeHandler GetByDbType(DbType dbType)
            => _byDbType.TryGetValue(dbType, out var handler)
                ? handler
                : throw new NotSupportedException("This DbType is not supported in Npgsql: " + dbType);

        internal NpgsqlTypeHandler GetByDataTypeName(string typeName)
            => _byTypeName.TryGetValue(typeName, out var handler)
                ? handler
                : throw new NotSupportedException("Could not find PostgreSQL type " + typeName);

        internal NpgsqlTypeHandler GetByClrType(Type type)
        {
            if (_byClrType.TryGetValue(type, out var handler))
                return handler;

            if (Nullable.GetUnderlyingType(type) is Type underlyingType && _byClrType.TryGetValue(underlyingType, out handler))
                return handler;

            // Try to see if it is an array type
            var arrayElementType = GetArrayElementType(type);
            if (arrayElementType != null)
            {
                if (_arrayHandlerByClrType.TryGetValue(arrayElementType, out handler))
                    return handler;

                throw new NotSupportedException($"The CLR array type {type} isn't supported by Npgsql or your PostgreSQL. " +
                                                "If you wish to map it to a PostgreSQL composite type array you need to register it before usage, please refer to the documentation.");
            }

            if (type.IsEnum)
            {
                if (_byTypeName.TryGetValue(GetPgName(type, DefaultNameTranslator), out handler))
                    return handler;

                throw new NotSupportedException($"The CLR enum type {type.Name} must be registered with Npgsql before usage, please refer to the documentation.");
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

            base.AddMapping(mapping);
            BindType(mapping, _connector, externalCall: true);
            ChangeCounter = -1;
            return this;
        }

        public override bool RemoveMapping(string pgTypeName)
        {
            CheckReady();

            var removed = base.RemoveMapping(pgTypeName);
            if (!removed)
                return false;

            // Rebind everything. We redo rather than trying to update the
            // existing dictionaries because it's complex to remove arrays, ranges...
            ClearBindings();
            BindTypes();
            ChangeCounter = -1;
            return true;
        }

        void CheckReady()
        {
            if (_connector.State != ConnectorState.Ready)
                throw new InvalidOperationException("Connection must be open and idle to perform registration");
        }

        void ResetMappings()
        {
            var globalMapper = GlobalTypeMapper.Instance;
            globalMapper.Lock.EnterReadLock();
            try
            {
                Mappings.Clear();
                foreach (var kv in globalMapper.Mappings)
                    Mappings.Add(kv.Key, kv.Value);
            }
            finally
            {
                globalMapper.Lock.ExitReadLock();
            }
            ChangeCounter = GlobalTypeMapper.Instance.ChangeCounter;
        }

        void ClearBindings()
        {
            _byOID.Clear();
            _byNpgsqlDbType.Clear();
            _byDbType.Clear();
            _byClrType.Clear();
            _arrayHandlerByClrType.Clear();

            _byNpgsqlDbType[NpgsqlDbType.Unknown] = UnrecognizedTypeHandler;
            _byClrType[typeof(DBNull)] = UnrecognizedTypeHandler;
        }

        public override void Reset()
        {
            ClearBindings();
            ResetMappings();
            BindTypes();
        }

        #endregion Mapping management

        #region Binding

        internal void Bind(NpgsqlDatabaseInfo databaseInfo)
        {
            _databaseInfo = databaseInfo;
            BindTypes();
        }

        void BindTypes()
        {
            foreach (var mapping in Mappings.Values)
                BindType(mapping, _connector, externalCall: false);

            // Enums
            var enumFactory = new UnmappedEnumTypeHandlerFactory(DefaultNameTranslator);
            foreach (var e in DatabaseInfo.EnumTypes.Where(e => !_byOID.ContainsKey(e.OID)))
                BindType(enumFactory.Create(e, _connector.Connection!), e);

            // Wire up any domains we find to their base type mappings, this is important
            // for reading domain fields of composites
            foreach (var domain in DatabaseInfo.DomainTypes)
                if (_byOID.TryGetValue(domain.BaseType.OID, out var baseTypeHandler))
                {
                    _byOID[domain.OID] = baseTypeHandler;
                    if (domain.Array != null)
                        BindType(baseTypeHandler.CreateArrayHandler(domain.Array, _connector.Settings.ArrayNullabilityMode), domain.Array);
                }
        }

        void BindType(NpgsqlTypeMapping mapping, NpgsqlConnector connector, bool externalCall)
        {
            // Binding can occur at two different times:
            // 1. When a user adds a mapping for a specific connection (and exception should bubble up to them)
            // 2. When binding the global mappings, in which case we want to log rather than throw
            // (i.e. missing database type for some unused defined binding shouldn't fail the connection)

            var pgName = mapping.PgTypeName;

            PostgresType? pgType;
            if (pgName.IndexOf('.') > -1)
                DatabaseInfo.ByFullName.TryGetValue(pgName, out pgType);  // Full type name with namespace
            else if (DatabaseInfo.ByName.TryGetValue(pgName, out pgType) && pgType is null) // No dot, partial type name
            {
                // If the name was found but the value is null, that means that there are
                // two db types with the same name (different schemas).
                // Try to fall back to pg_catalog, otherwise fail.
                if (!DatabaseInfo.ByFullName.TryGetValue($"pg_catalog.{pgName}", out pgType))
                {
                    var msg = $"More than one PostgreSQL type was found with the name {mapping.PgTypeName}, please specify a full name including schema";
                    if (externalCall)
                        throw new ArgumentException(msg);
                    Log.Debug(msg);
                    return;
                }
            }

            if (pgType is null)
            {
                var msg = $"A PostgreSQL type with the name {mapping.PgTypeName} was not found in the database";
                if (externalCall)
                    throw new ArgumentException(msg);
                Log.Debug(msg);
                return;
            }
            if (pgType is PostgresDomainType)
            {
                var msg = "Cannot add a mapping to a PostgreSQL domain type";
                if (externalCall)
                    throw new NotSupportedException(msg);
                Log.Debug(msg);
                return;
            }

            var handler = mapping.TypeHandlerFactory.CreateNonGeneric(pgType, connector.Connection!);
            BindType(handler, pgType, mapping.NpgsqlDbType, mapping.DbTypes, mapping.ClrTypes);

            if (!externalCall)
                return;

            foreach (var domain in DatabaseInfo.DomainTypes)
                if (domain.BaseType.OID == pgType.OID)
                {
                    _byOID[domain.OID] = handler;
                    if (domain.Array != null)
                        BindType(handler.CreateArrayHandler(domain.Array, _connector.Settings.ArrayNullabilityMode), domain.Array);
                }
        }

        void BindType(NpgsqlTypeHandler handler, PostgresType pgType, NpgsqlDbType? npgsqlDbType = null, DbType[]? dbTypes = null, Type[]? clrTypes = null)
        {
            _byOID[pgType.OID] = handler;
            _byTypeName[pgType.FullName] = handler;
            _byTypeName[pgType.Name] = handler;

            if (npgsqlDbType.HasValue)
            {
                var value = npgsqlDbType.Value;
                if (_byNpgsqlDbType.ContainsKey(value))
                    throw new InvalidOperationException($"Two type handlers registered on same NpgsqlDbType '{npgsqlDbType}': {_byNpgsqlDbType[value].GetType().Name} and {handler.GetType().Name}");
                _byNpgsqlDbType[npgsqlDbType.Value] = handler;
            }

            if (dbTypes != null)
            {
                foreach (var dbType in dbTypes)
                {
                    if (_byDbType.ContainsKey(dbType))
                        throw new InvalidOperationException($"Two type handlers registered on same DbType {dbType}: {_byDbType[dbType].GetType().Name} and {handler.GetType().Name}");
                    _byDbType[dbType] = handler;
                }
            }

            if (clrTypes != null)
            {
                foreach (var type in clrTypes)
                {
                    if (_byClrType.ContainsKey(type))
                        throw new InvalidOperationException($"Two type handlers registered on same .NET type '{type}': {_byClrType[type].GetType().Name} and {handler.GetType().Name}");
                    _byClrType[type] = handler;
                }
            }

            if (pgType.Array != null)
                BindArrayType(handler, pgType.Array, npgsqlDbType, clrTypes);

            if (pgType.Range != null)
                BindRangeType(handler, pgType.Range, npgsqlDbType, clrTypes);
        }

        void BindArrayType(NpgsqlTypeHandler elementHandler, PostgresArrayType pgArrayType, NpgsqlDbType? elementNpgsqlDbType, Type[]? elementClrTypes)
        {
            var arrayHandler = elementHandler.CreateArrayHandler(pgArrayType, _connector.Settings.ArrayNullabilityMode);

            var arrayNpgsqlDbType = elementNpgsqlDbType.HasValue
                ? NpgsqlDbType.Array | elementNpgsqlDbType.Value
                : (NpgsqlDbType?)null;

            BindType(arrayHandler, pgArrayType, arrayNpgsqlDbType);

            // Note that array handlers aren't registered in ByClrType like base types, because they handle all
            // dimension types and not just one CLR type (e.g. int[], int[,], int[,,]).
            // So the by-type lookup is special and goes via _arrayHandlerByClrType, see this[Type type]
            // TODO: register single-dimensional in _byType as a specific optimization? But do PSV as well...
            if (elementClrTypes != null)
            {
                foreach (var elementType in elementClrTypes)
                {
                    if (_arrayHandlerByClrType.ContainsKey(elementType))
                        throw new Exception(
                            $"Two array type handlers registered on same .NET type {elementType}: {_arrayHandlerByClrType[elementType].GetType().Name} and {arrayHandler.GetType().Name}");
                    _arrayHandlerByClrType[elementType] = arrayHandler;
                }
            }
        }

        void BindRangeType(NpgsqlTypeHandler elementHandler, PostgresRangeType pgRangeType, NpgsqlDbType? elementNpgsqlDbType, Type[]? elementClrTypes)
        {
            var rangeHandler = elementHandler.CreateRangeHandler(pgRangeType);

            var rangeNpgsqlDbType = elementNpgsqlDbType.HasValue
                ? NpgsqlDbType.Range | elementNpgsqlDbType.Value
                : (NpgsqlDbType?)null;

            // We only want to bind supported range CLR types whose element CLR types are being bound as well.
            var clrTypes = elementClrTypes is null
                ? null
                : rangeHandler.SupportedRangeClrTypes
                    .Where(r => elementClrTypes.Contains(r.GenericTypeArguments[0]))
                    .ToArray();

            BindType((NpgsqlTypeHandler)rangeHandler, pgRangeType, rangeNpgsqlDbType, null, clrTypes);
        }

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
            => Mappings.TryGetValue(pgType.Name, out mapping) ||
               Mappings.TryGetValue(pgType.FullName, out mapping) ||
               pgType is PostgresDomainType domain && (
                   Mappings.TryGetValue(domain.BaseType.Name, out mapping) ||
                   Mappings.TryGetValue(domain.BaseType.FullName, out mapping));
    }
}
