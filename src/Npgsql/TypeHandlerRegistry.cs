#region License
// The PostgreSQL License
//
// Copyright (C) 2016 The Npgsql Development Team
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
#endregion

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using AsyncRewriter;
using JetBrains.Annotations;
using Npgsql.Logging;
using Npgsql.TypeHandlers;
using NpgsqlTypes;

namespace Npgsql
{
    partial class TypeHandlerRegistry
    {
        #region Members

        internal NpgsqlConnector Connector { get; private set; }
        internal TypeHandler UnrecognizedTypeHandler { get; private set; }

        internal Dictionary<uint, TypeHandler> ByOID { get; private set; }
        readonly Dictionary<DbType, TypeHandler> _byDbType;
        readonly Dictionary<NpgsqlDbType, TypeHandler> _byNpgsqlDbType;

        /// <summary>
        /// Maps CLR types to their type handlers.
        /// </summary>
        readonly Dictionary<Type, TypeHandler> _byType;

        /// <summary>
        /// Maps CLR types to their array handlers.
        /// </summary>
        Dictionary<Type, TypeHandler> _arrayHandlerByType;

        BackendTypes _backendTypes;

        /// <summary>
        /// A counter that is updated when this registry activates its global mappings.
        /// Tracks <see cref="_globalMappingChangeCounter"/>, allows us to know when a pooled
        /// connection's mappings are no longer up to date because a global mapping change has
        /// occurred.
        /// </summary>
        int _globalMappingActivationCounter = -1;

        /// <summary>
        /// A counter that is incremented whenever a global mapping change occurs (e.g.
        /// <see cref="MapEnumGlobally{T}"/>, <see cref="UnmapCompositeGlobally{T}"/>.
        /// <seealso cref="_globalMappingActivationCounter"/>
        /// </summary>
        static int _globalMappingChangeCounter;

        internal static readonly Dictionary<string, TypeAndMapping> HandlerTypes;
        static readonly Dictionary<NpgsqlDbType, TypeAndMapping> HandlerTypesByNpsgqlDbType;
        static readonly Dictionary<NpgsqlDbType, DbType> NpgsqlDbTypeToDbType;
        static readonly Dictionary<DbType, NpgsqlDbType> DbTypeToNpgsqlDbType;
        static readonly Dictionary<Type, NpgsqlDbType> TypeToNpgsqlDbType;
        static readonly Dictionary<Type, DbType> TypeToDbType;

        /// <summary>
        /// Caches, for each connection string, the results of the backend type query in the form of a list of type
        /// info structs keyed by the PG name.
        /// Repeated connections to the same connection string reuse the query results and avoid an additional
        /// roundtrip at open-time.
        /// </summary>
        static readonly ConcurrentDictionary<string, BackendTypes> BackendTypeCache = new ConcurrentDictionary<string, BackendTypes>();

        static readonly ConcurrentDictionary<string, IEnumHandlerFactory> _globalEnumMappings;
        static readonly ConcurrentDictionary<string, ICompositeHandlerFactory> _globalCompositeMappings;

        internal static IDictionary<string, IEnumHandlerFactory> GlobalEnumMappings => _globalEnumMappings;
        internal static IDictionary<string, ICompositeHandlerFactory> GlobalCompositeMappings => _globalCompositeMappings;

        static readonly INpgsqlNameTranslator DefaultNameTranslator = new NpgsqlSnakeCaseNameTranslator();
        static readonly BackendTypes EmptyBackendTypes = new BackendTypes();
        static readonly NpgsqlLogger Log = NpgsqlLogManager.GetCurrentClassLogger();

        #endregion

        #region Initialization and Loading

        [RewriteAsync]
        internal static void Setup(NpgsqlConnector connector, NpgsqlTimeout timeout)
        {
            // Note that there's a chicken and egg problem here - LoadBackendTypes below needs a functional 
            // connector to load the types, hence the strange initialization code here
            connector.TypeHandlerRegistry = new TypeHandlerRegistry(connector);

            BackendTypes types;
            if (!BackendTypeCache.TryGetValue(connector.ConnectionString, out types))
                types = BackendTypeCache[connector.ConnectionString] = LoadBackendTypes(connector, timeout);

            connector.TypeHandlerRegistry._backendTypes = types;
            connector.TypeHandlerRegistry.ActivateGlobalMappings();
        }

        TypeHandlerRegistry(NpgsqlConnector connector)
        {
            Connector = connector;
            _backendTypes = EmptyBackendTypes;
            UnrecognizedTypeHandler = new UnrecognizedTypeHandler();
            ByOID = new Dictionary<uint, TypeHandler>();
            _byDbType = new Dictionary<DbType, TypeHandler>();
            _byNpgsqlDbType = new Dictionary<NpgsqlDbType, TypeHandler>();
            _byType = new Dictionary<Type, TypeHandler> { [typeof(DBNull)] = UnrecognizedTypeHandler };
            _byNpgsqlDbType[NpgsqlDbType.Unknown] = UnrecognizedTypeHandler;
        }

        internal void ActivateGlobalMappings()
        {
            if (_globalMappingActivationCounter == _globalMappingChangeCounter)
                return;

            foreach (var kv in _globalEnumMappings)
            {
                var backendType = GetBackendTypeByName(kv.Key);
                var backendEnumType = backendType as BackendEnumType;
                if (backendEnumType == null)
                {
                    Log.Warn($"While attempting to activate global enum mappings, PostgreSQL type {kv.Key} was found but is not an enum. Skipping it.", Connector.Id);
                    continue;
                }
                backendEnumType.Activate(this, kv.Value);
            }

            foreach (var kv in _globalCompositeMappings)
            {
                try
                {
                    GetCompositeType(kv.Key).Activate(this, kv.Value);
                }
                catch (Exception e)
                {
                    Log.Warn("Caught an exception while attempting to activate global composite mappings", e, Connector.Id);
                }
            }

            _globalMappingActivationCounter = _globalMappingChangeCounter;
        }

        static readonly string TypesQueryWithRange = GenerateTypesQuery(true);
        static readonly string TypesQueryWithoutRange = GenerateTypesQuery(false);

        static string GenerateTypesQuery(bool withRange)
        {
            // Select all types (base, array which is also base, enum, range, composite).
            // Note that arrays are distinguished from primitive types through them having typreceive=array_recv.
            // Order by primitives first, container later.
            // For arrays and ranges, join in the element OID and type (to filter out arrays of unhandled
            // types).
            return
$@"SELECT ns.nspname, a.typname, a.oid, a.typrelid, a.typbasetype,
CASE WHEN pg_proc.proname='array_recv' THEN 'a' ELSE a.typtype END AS type,
CASE
  WHEN pg_proc.proname='array_recv' THEN a.typelem
  {(withRange ? "WHEN a.typtype='r' THEN rngsubtype" : "")}
  ELSE 0
END AS elemoid,
CASE
  WHEN pg_proc.proname IN ('array_recv','oidvectorrecv') THEN 3    /* Arrays last */
  WHEN a.typtype='r' THEN 2                                        /* Ranges before */
  WHEN a.typtype='d' THEN 1                                        /* Domains before */
  ELSE 0                                                           /* Base types first */
END AS ord
FROM pg_type AS a
JOIN pg_namespace AS ns ON (ns.oid = a.typnamespace)
JOIN pg_proc ON pg_proc.oid = a.typreceive
LEFT OUTER JOIN pg_type AS b ON (b.oid = a.typelem)
{(withRange ? "LEFT OUTER JOIN pg_range ON (pg_range.rngtypid = a.oid) " : "")}
WHERE
  (
    a.typtype IN ('b', 'r', 'e', 'd') AND
    (b.typtype IS NULL OR b.typtype IN ('b', 'r', 'e', 'd'))  /* Either non-array or array of supported element type */
  ) OR
  a.typname IN ('record', 'void')
ORDER BY ord";
        }

        [RewriteAsync]
        static BackendTypes LoadBackendTypes(NpgsqlConnector connector, NpgsqlTimeout timeout)
        {
            var types = new BackendTypes();
            using (var command = new NpgsqlCommand(connector.SupportsRangeTypes ? TypesQueryWithRange : TypesQueryWithoutRange, connector.Connection))
            {
                command.CommandTimeout = timeout.IsSet ? (int)timeout.TimeLeft.TotalSeconds : 0;
                command.AllResultTypesAreUnknown = true;
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        timeout.Check();
                        LoadBackendType(reader, types, connector);
                    }
                }
            }
            return types;
        }

        static void LoadBackendType(NpgsqlDataReader reader, BackendTypes types, NpgsqlConnector connector)
        {
            var ns = reader.GetString(0);
            var name = reader.GetString(1);
            var oid = Convert.ToUInt32(reader[2]);

            Debug.Assert(name != null);
            Debug.Assert(oid != 0);

            uint elementOID;
            var typeChar = reader.GetString(5)[0];
            switch (typeChar)
            {
            case 'b':  // Normal base type
                TypeAndMapping typeAndMapping;
                (
                    HandlerTypes.TryGetValue(name, out typeAndMapping)
                        ? new BackendBaseType(ns, name, oid, typeAndMapping.HandlerType, typeAndMapping.Mapping)
                        : new BackendBaseType(ns, name, oid)  // Unsupported by Npgsql
                ).AddTo(types);
                return;
            case 'a':   // Array
                elementOID = Convert.ToUInt32(reader[6]);
                Debug.Assert(elementOID > 0);
                BackendType elementBackendType;
                if (!types.ByOID.TryGetValue(elementOID, out elementBackendType))
                {
                    Log.Trace($"Array type '{name}' refers to unknown element with OID {elementOID}, skipping", connector.Id);
                    return;
                }
                new BackendArrayType(ns, name, oid, elementBackendType).AddTo(types);
                return;
            case 'r':   // Range
                elementOID = Convert.ToUInt32(reader[6]);
                Debug.Assert(elementOID > 0);
                if (!types.ByOID.TryGetValue(elementOID, out elementBackendType))
                {
                    Log.Trace($"Range type '{name}' refers to unknown subtype with OID {elementOID}, skipping", connector.Id);
                    return;
                }
                new BackendRangeType(ns, name, oid, elementBackendType).AddTo(types);
                return;
            case 'e':   // Enum
                new BackendEnumType(ns, name, oid).AddTo(types);
                return;
            case 'd':   // Domain
                var baseTypeOID = Convert.ToUInt32(reader[4]);
                Debug.Assert(baseTypeOID > 0);
                BackendType baseBackendType;
                if (!types.ByOID.TryGetValue(baseTypeOID, out baseBackendType))
                {
                    Log.Trace($"Domain type '{name}' refers to unknown base type with OID {baseTypeOID}, skipping", connector.Id);
                    return;
                }
                new BackendDomainType(ns, name, oid, baseBackendType).AddTo(types);
                return;
            case 'p':   // pseudo-type (record, void)
                // Hack this as a base type
                goto case 'b';
            default:
                throw new ArgumentOutOfRangeException($"Unknown typtype for type '{name}' in pg_type: {typeChar}");
            }
        }

        #endregion

        #region Enum

        internal void MapEnum<TEnum>([CanBeNull] string pgName, [CanBeNull] INpgsqlNameTranslator nameTranslator) where TEnum : struct
        {
            if (nameTranslator == null)
                nameTranslator = DefaultNameTranslator;
            if (pgName == null)
                pgName = GetPgName<TEnum>(nameTranslator);
            var backendType = GetBackendTypeByName(pgName);

            var asEnumType = backendType as BackendEnumType;
            if (asEnumType == null)
                throw new NpgsqlException($"A PostgreSQL type with the name {pgName} was found in the database but it isn't an enum");

            asEnumType.Activate(this, new EnumHandler<TEnum>(backendType, nameTranslator));
        }

        internal static void MapEnumGlobally<TEnum>([CanBeNull] string pgName, [CanBeNull] INpgsqlNameTranslator nameTranslator) where TEnum : struct
        {
            if (nameTranslator == null)
                nameTranslator = DefaultNameTranslator;
            if (pgName == null)
                pgName = GetPgName<TEnum>(nameTranslator);

            _globalMappingChangeCounter++;
            _globalEnumMappings[pgName] = new EnumHandler<TEnum>.Factory(nameTranslator);
        }

        internal static void UnmapEnumGlobally<TEnum>([CanBeNull] string pgName, [CanBeNull] INpgsqlNameTranslator nameTranslator) where TEnum : struct
        {
            if (nameTranslator == null)
                nameTranslator = DefaultNameTranslator;
            if (pgName == null)
                pgName = GetPgName<TEnum>(nameTranslator);

            _globalMappingChangeCounter++;
            IEnumHandlerFactory _;
            _globalEnumMappings.TryRemove(pgName, out _);
        }

        #endregion

        #region Composite

        internal void MapComposite<T>([CanBeNull] string pgName, [CanBeNull] INpgsqlNameTranslator nameTranslator) where T : new()
        {
            if (nameTranslator == null)
                nameTranslator = DefaultNameTranslator;
            if (pgName == null)
                pgName = GetPgName<T>(nameTranslator);

            // TODO: Check if already mapped dude

            var compositeType = GetCompositeType(pgName);
            compositeType.Activate(this, new CompositeHandler<T>(compositeType, nameTranslator, this));
        }

        internal static void MapCompositeGlobally<T>([CanBeNull] string pgName, [CanBeNull] INpgsqlNameTranslator nameTranslator) where T : new()
        {
            if (nameTranslator == null)
                nameTranslator = DefaultNameTranslator;
            if (pgName == null)
                pgName = GetPgName<T>(nameTranslator);

            _globalMappingChangeCounter++;
            _globalCompositeMappings[pgName] = new CompositeHandler<T>.Factory(nameTranslator);
        }

        internal static void UnmapCompositeGlobally<T>([CanBeNull] string pgName, [CanBeNull] INpgsqlNameTranslator nameTranslator) where T : new()
        {
            if (nameTranslator == null)
                nameTranslator = DefaultNameTranslator;
            if (pgName == null)
                pgName = GetPgName<T>(nameTranslator);

            _globalMappingChangeCounter++;
            ICompositeHandlerFactory _;
            _globalCompositeMappings.TryRemove(pgName, out _);
        }

#if WAT
        const string LoadCompositeQuery =
@"SELECT ns.nspname, a.typname, a.oid,
CASE
  WHEN a.typname = @name THEN 0   /* First we load the composite type */
  ELSE 1                          /* Then we load its array */
END AS ord
FROM pg_type AS a
JOIN pg_namespace AS ns ON (ns.oid = a.typnamespace)
LEFT OUTER JOIN pg_type AS b ON (b.oid = a.typelem)
WHERE
  a.typname = @name OR   /* The composite type */
  b.typname = @name      /* The composite's array type */
ORDER BY ord";

        const string LoadCompositeWithSchemaQuery =
@"SELECT ns_a.nspname, a.typname, a.oid, a.typtype,
CASE
  WHEN a.typname = @name THEN 0   /* First we load the composite type */
  ELSE 1                          /* Then we load its array */
END AS ord
FROM pg_type AS a
JOIN pg_namespace AS ns_a ON (ns_a.oid = a.typnamespace)
LEFT OUTER JOIN pg_type AS b ON (b.oid = a.typelem)
LEFT OUTER JOIN pg_namespace AS ns_b ON (ns_b.oid = b.typnamespace)
WHERE
  (ns_a.nspname = @schema AND a.typname = @name) OR   /* The composite type */
  (ns_b.nspname = @schema AND b.typname = @name)      /* The composite's array type */
ORDER BY ord";
#endif

        static string GenerateLoadCompositeQuery(bool withSchema) =>
$@"SELECT ns.nspname, typ.oid, typ.typtype
FROM pg_type AS typ
JOIN pg_namespace AS ns ON (ns.oid = typ.typnamespace)
WHERE (typ.typname = @name{(withSchema ? " AND a.nspname = @schema" : "")});

SELECT att.attname, att.atttypid
FROM pg_type AS typ
JOIN pg_namespace AS ns ON (ns.oid = typ.typnamespace)
JOIN pg_attribute AS att ON (att.attrelid = typ.typrelid)
WHERE
  typ.typname = @name{(withSchema ? " AND a.nspname = @schema" : "")} AND
  attnum > 0; /* Don't load system attributes */

SELECT ns.nspname, a.typname, a.oid
FROM pg_type AS a
JOIN pg_type AS b ON (b.oid = a.typelem)
JOIN pg_namespace AS ns ON (ns.oid = b.typnamespace)
WHERE a.typtype = 'b' AND b.typname = @name{(withSchema ? " AND b.nspname = @schema" : "")}";

        BackendCompositeType GetCompositeType(string pgName)
        {
            // First check if the composite type definition has already been loaded from the database
            BackendType backendType;
            if (pgName.IndexOf('.') == -1
                ? _backendTypes.ByName.TryGetValue(pgName, out backendType)
                : _backendTypes.ByFullName.TryGetValue(pgName, out backendType))
            {
                var asComposite = backendType as BackendCompositeType;
                if (asComposite == null)
                    throw new NpgsqlException($"Type {pgName} was found but is not a composite");
                return asComposite;
            }

            // This is the first time the composite is mapped, the type definition needs to be loaded
            string name, schema;
            var i = pgName.IndexOf('.');
            if (i == -1)
            {
                schema = null;
                name = pgName;
            }
            else
            {
                schema = pgName.Substring(0, i);
                name = pgName.Substring(i + 1);
            }

            using (var cmd = new NpgsqlCommand(GenerateLoadCompositeQuery(schema != null), Connector.Connection))
            {
                cmd.Parameters.AddWithValue("name", name);
                if (schema != null)
                    cmd.Parameters.AddWithValue("schema", schema);
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        throw new Exception($"An PostgreSQL type with the name {pgName} was not found in the database");

                    // Load some info on the composite type itself, do some checks
                    var ns = reader.GetString(0);
                    Debug.Assert(schema == null || ns == schema);
                    var oid = reader.GetFieldValue<uint>(1);
                    var typeChar = reader.GetChar(2);
                    if (typeChar != 'c')
                        throw new NpgsqlException($"Type {pgName} was found in the database but is not a composite");

                    reader.NextResult();  // Load the fields

                    var fields = new List<RawCompositeField>();
                    while (reader.Read())
                        fields.Add(new RawCompositeField { PgName = reader.GetString(0), TypeOID = reader.GetFieldValue<uint>(1) });

                    var compositeType = new BackendCompositeType(ns, name, oid, fields);
                    compositeType.AddTo(_backendTypes);

                    reader.NextResult();  // Load the array type

                    if (reader.Read())
                    {
                        var arrayNs = reader.GetString(0);
                        var arrayName = reader.GetString(1);
                        var arrayOID = reader.GetFieldValue<uint>(2);

                        new BackendArrayType(arrayNs, arrayName, arrayOID, compositeType).AddTo(_backendTypes);
                    } else
                        Log.Warn($"Could not find array type corresponding to composite {pgName}");

                    return compositeType;
                }
            }
        }

        #endregion

        #region Lookups

        /// <summary>
        /// Looks up a type handler by its PostgreSQL type's OID.
        /// </summary>
        /// <param name="oid">A PostgreSQL type OID</param>
        /// <returns>A type handler that can be used to encode and decode values.</returns>
        internal TypeHandler this[uint oid]
        {
            get
            {
                TypeHandler result;
                return TryGetByOID(oid, out result) ? result : UnrecognizedTypeHandler;
            }
            set { ByOID[oid] = value; }
        }

        internal bool TryGetByOID(uint oid, out TypeHandler handler)
        {
            if (ByOID.TryGetValue(oid, out handler))
                return true;
            BackendType backendType;
            if (!_backendTypes.ByOID.TryGetValue(oid, out backendType))
                return false;

            handler = backendType.Activate(this);
            return true;
        }

        internal TypeHandler this[NpgsqlDbType npgsqlDbType, Type specificType = null]
        {
            get
            {
                if (specificType != null && (npgsqlDbType & NpgsqlDbType.Enum) == 0 && (npgsqlDbType & NpgsqlDbType.Composite) == 0)
                    throw new ArgumentException($"{nameof(specificType)} can only be used with {nameof(NpgsqlDbType.Enum)} or {nameof(NpgsqlDbType.Composite)}");

                TypeHandler handler;
                if (_byNpgsqlDbType.TryGetValue(npgsqlDbType, out handler)) {
                    return handler;
                }

                if (specificType != null)  // Enum/composite
                {
                    // Note that enums and composites are never lazily activated - they're activated at the
                    // moment of mapping (or at connection time when globally-mapped)
                    if ((npgsqlDbType & NpgsqlDbType.Array) != 0)
                    {
                        // Already-activated array of enum/composite
                        if (_arrayHandlerByType != null && _arrayHandlerByType.TryGetValue(specificType, out handler))
                            return handler;
                    }

                    // For non-array enum/composite, simply delegate to type inference
                    return this[specificType];
                }

                // Couldn't find already activated type, attempt to activate

                if (npgsqlDbType == NpgsqlDbType.Enum || npgsqlDbType == NpgsqlDbType.Composite)
                    throw new InvalidCastException(string.Format("When specifying NpgsqlDbType.{0}, {0}Type must be specified as well",
                                                    npgsqlDbType == NpgsqlDbType.Enum ? "Enum" : "Composite"));

                // Base, range or array of base/range
                BackendType backendType;
                if (_backendTypes.ByNpgsqlDbType.TryGetValue(npgsqlDbType, out backendType))
                    return backendType.Activate(this);

                // We don't have a backend type for this NpgsqlDbType. This could be because it's not yet supported by
                // Npgsql, or that the type is missing in the database (old PG, missing extension...)
                TypeAndMapping typeAndMapping;
                if (!HandlerTypesByNpsgqlDbType.TryGetValue(npgsqlDbType, out typeAndMapping))
                    throw new NotSupportedException("This NpgsqlDbType isn't supported in Npgsql yet: " + npgsqlDbType);
                throw new NpgsqlException($"The PostgreSQL type '{typeAndMapping.Mapping.PgName}', mapped to NpgsqlDbType '{npgsqlDbType}' isn't present in your database. " +
                                           "You may need to install an extension or upgrade to a newer version.");
            }
        }

        internal TypeHandler this[DbType dbType]
        {
            get
            {
                TypeHandler handler;
                if (_byDbType.TryGetValue(dbType, out handler))
                    return handler;
                BackendType backendType;
                if (_backendTypes.ByDbType.TryGetValue(dbType, out backendType))
                    return backendType.Activate(this);
                throw new NotSupportedException("This DbType is not supported in Npgsql: " + dbType);
            }
        }

        internal TypeHandler this[object value]
        {
            get
            {
                Debug.Assert(value != null);

                if (value is DateTime)
                {
                    return ((DateTime) value).Kind == DateTimeKind.Utc
                        ? this[NpgsqlDbType.TimestampTZ]
                        : this[NpgsqlDbType.Timestamp];
                }

                if (value is NpgsqlDateTime) {
                    return ((NpgsqlDateTime)value).Kind == DateTimeKind.Utc
                        ? this[NpgsqlDbType.TimestampTZ]
                        : this[NpgsqlDbType.Timestamp];
                }

                return this[value.GetType()];
            }
        }

        internal TypeHandler this[Type type]
        {
            get
            {
                TypeHandler handler;
                if (_byType.TryGetValue(type, out handler))
                    return handler;

                Type arrayElementType = null;

                // Detect arrays and generic ILists
                if (type.IsArray)
                    arrayElementType = type.GetElementType();
                else if (typeof(IList).IsAssignableFrom(type))
                {
                    if (!type.GetTypeInfo().IsGenericType)
                        throw new NotSupportedException("Non-generic IList is a supported parameter, but the NpgsqlDbType parameter must be set on the parameter");
                    arrayElementType = type.GetGenericArguments()[0];
                }

                if (arrayElementType != null)
                {
                    TypeHandler elementHandler;
                    if (_byType.TryGetValue(arrayElementType, out elementHandler) &&
                        elementHandler.BackendType.NpgsqlDbType.HasValue &&
                        _byNpgsqlDbType.TryGetValue(NpgsqlDbType.Array | elementHandler.BackendType.NpgsqlDbType.Value, out handler))
                    {
                        return handler;
                    }

                    // Enum and composite types go through the special _arrayHandlerByType
                    if (_arrayHandlerByType != null && _arrayHandlerByType.TryGetValue(arrayElementType, out handler))
                        return handler;

                    // Unactivated array

                    // Special check for byte[] - bytea not array of int2
                    if (type == typeof(byte[]))
                    {
                        BackendType byteaBackendType;
                        if (!_backendTypes.ByClrType.TryGetValue(typeof(byte[]), out byteaBackendType))
                            throw new NpgsqlException("The PostgreSQL 'bytea' type is missing");
                        return byteaBackendType.Activate(this);
                    }

                    // Get the elements backend type and activate its array backend type
                    BackendType elementBackendType;
                    if (!_backendTypes.ByClrType.TryGetValue(arrayElementType, out elementBackendType))
                    {
                        if (arrayElementType.GetTypeInfo().IsEnum)
                            throw new NotSupportedException($"The CLR enum type {arrayElementType.Name} must be mapped with Npgsql before usage, please refer to the documentation.");
                        throw new NotSupportedException($"The CLR type {arrayElementType} isn't supported by Npgsql or your PostgreSQL. " +
                                                         "If you wish to map it to a PostgreSQL composite type you need to register it before usage, please refer to the documentation.");
                    }

                    if (elementBackendType == null)
                        throw new NotSupportedException($"The PostgreSQL {arrayElementType.Name} does not have an array type in the database");

                    return elementBackendType.Array.Activate(this);
                }

                BackendType backendType;

                // Try to find the backend type by a simple lookup on the given CLR type, this will handle base types.
                if (_backendTypes.ByClrType.TryGetValue(type, out backendType))
                    return backendType.Activate(this);

                // Range type which hasn't yet been set up
                if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(NpgsqlRange<>))
                {
                    BackendType subtypeBackendType;
                    if (!_backendTypes.ByClrType.TryGetValue(type.GetGenericArguments()[0], out subtypeBackendType) ||
                        subtypeBackendType.Range == null)
                    {
                        throw new NpgsqlException($"The .NET range type {type.Name} isn't supported in your PostgreSQL, use CREATE TYPE AS RANGE");
                    }

                    return subtypeBackendType.Range.Activate(this);
                }

                // Nothing worked
                if (type.GetTypeInfo().IsEnum)
                    throw new NotSupportedException($"The CLR enum type {type.Name} must be registered with Npgsql before usage, please refer to the documentation.");

                if (typeof(IEnumerable).IsAssignableFrom(type))
                    throw new NotSupportedException("Npgsql 3.x removed support for writing a parameter with an IEnumerable value, use .ToList()/.ToArray() instead");

                throw new NotSupportedException($"The CLR type {type} isn't supported by Npgsql or your PostgreSQL. " +
                                                 "If you wish to map it to a PostgreSQL composite type you need to register it before usage, please refer to the documentation.");
            }
        }

        internal static NpgsqlDbType ToNpgsqlDbType(DbType dbType)
        {
            return DbTypeToNpgsqlDbType[dbType];
        }

        internal static NpgsqlDbType ToNpgsqlDbType(object value)
        {
            if (value is DateTime)
            {
                return ((DateTime)value).Kind == DateTimeKind.Utc
                    ? NpgsqlDbType.TimestampTZ
                    : NpgsqlDbType.Timestamp;
            }

            if (value is NpgsqlDateTime)
            {
                return ((NpgsqlDateTime)value).Kind == DateTimeKind.Utc
                    ? NpgsqlDbType.TimestampTZ
                    : NpgsqlDbType.Timestamp;
            }

            return ToNpgsqlDbType(value.GetType());
        }

        static NpgsqlDbType ToNpgsqlDbType(Type type)
        {
            NpgsqlDbType npgsqlDbType;
            if (TypeToNpgsqlDbType.TryGetValue(type, out npgsqlDbType)) {
                return npgsqlDbType;
            }

            if (type.IsArray)
            {
                if (type == typeof(byte[]))
                    return NpgsqlDbType.Bytea;
                return NpgsqlDbType.Array | ToNpgsqlDbType(type.GetElementType());
            }

            var typeInfo = type.GetTypeInfo();

            if (typeInfo.IsEnum)
                return NpgsqlDbType.Enum;

            if (typeInfo.IsGenericType && type.GetGenericTypeDefinition() == typeof(NpgsqlRange<>))
                return NpgsqlDbType.Range | ToNpgsqlDbType(type.GetGenericArguments()[0]);

            if (type == typeof(DBNull))
                return NpgsqlDbType.Unknown;

            throw new NotSupportedException("Can't infer NpgsqlDbType for type " + type);
        }

        internal static DbType ToDbType(Type type)
        {
            DbType dbType;
            return TypeToDbType.TryGetValue(type, out dbType) ? dbType : DbType.Object;
        }

        internal static DbType ToDbType(NpgsqlDbType npgsqlDbType)
        {
            DbType dbType;
            return NpgsqlDbTypeToDbType.TryGetValue(npgsqlDbType, out dbType) ? dbType : DbType.Object;
        }

        #endregion

        #region Backend types

        /// <summary>
        /// A structure holding information about all PostgreSQL types found in an actual database.
        /// Only contains <see cref="BackendType"/> instances and not actual <see cref="TypeHandlers"/>, and is shared between
        /// all connections using the same connection string. Consulted when a type handler needs to be created.
        /// </summary>
        class BackendTypes
        {
            internal Dictionary<uint, BackendType> ByOID { get; } = new Dictionary<uint, BackendType>();

#if !__MonoCS__
            /// <summary>
            /// Indexes backend types by their PostgreSQL name, including namespace (e.g. pg_catalog.int4).
            /// Only used for enums and composites.
            /// </summary>
#endif
            internal Dictionary<string, BackendType> ByFullName { get; } = new Dictionary<string, BackendType>();

#if !__MonoCS__
            /// <summary>
            /// Indexes backend types by their PostgreSQL name, not including namespace.
            /// If more than one type exists with the same name (i.e. in different namespaces) this
            /// table will contain an entry with a null value.
            /// Only used for enums and composites.
            /// </summary>
#endif
            internal Dictionary<string, BackendType> ByName { get; } = new Dictionary<string, BackendType>();
            internal Dictionary<NpgsqlDbType, BackendType> ByNpgsqlDbType { get; } = new Dictionary<NpgsqlDbType, BackendType>();
            internal Dictionary<DbType, BackendType> ByDbType { get; } = new Dictionary<DbType, BackendType>();
            internal Dictionary<Type, BackendType> ByClrType { get; } = new Dictionary<Type, BackendType>();
        }

        /// <summary>
        /// Represents a single PostgreSQL data type, as discovered from pg_type.
        /// Note that this simply a data structure describing a type and not a handler, and is shared between connectors
        /// having the same connection string.
        /// </summary>
        abstract class BackendType : IBackendType
        {
            protected BackendType(string ns, string name, uint oid)
            {
                Namespace = ns;
                Name = name;
                FullName = Namespace + '.' + Name;
                DisplayName = Namespace == "pg_catalog"
                    ? Name
                    : FullName;
                OID = oid;
            }

            public string Namespace { get; }
            public string Name { get; }
            public uint OID { get; }
            public NpgsqlDbType? NpgsqlDbType { get; protected set; }
            public string FullName { get; }
            public string DisplayName { get; }
            internal BackendArrayType Array;
            internal BackendRangeType Range;

            /// <summary>
            /// For base types, contains the handler type.
            /// If null, this backend type isn't supported by Npgsql.
            /// </summary>
            [CanBeNull]
            internal Type HandlerType;

            internal virtual void AddTo(BackendTypes types)
            {
                types.ByOID[OID] = this;
                if (NpgsqlDbType != null)
                    types.ByNpgsqlDbType[NpgsqlDbType.Value] = this;
            }

            internal abstract TypeHandler Activate(TypeHandlerRegistry registry);

            /// <summary>
            /// Returns a string that represents the current object.
            /// </summary>
            public override string ToString() => DisplayName;
        }

        class BackendBaseType : BackendType
        {
            readonly DbType[] _dbTypes;
            readonly Type[] _clrTypes;

            [CanBeNull] ConstructorInfo _ctorWithRegistry;
            [CanBeNull] ConstructorInfo _ctorWithoutRegistry;

            /// <summary>
            /// Constructs an unsupported base type (no handler exists in Npgsql for this type)
            /// </summary>
            internal BackendBaseType(string ns, string name, uint oid) : base(ns, name, oid)
            {
                _dbTypes = new DbType[0];
                _clrTypes = new Type[0];
            }

            internal BackendBaseType(string ns, string name, uint oid, Type handlerType, TypeMappingAttribute mapping)
                : base(ns, name, oid)
            {
                HandlerType = handlerType;
                NpgsqlDbType = mapping.NpgsqlDbType;
                _dbTypes = mapping.DbTypes;
                _clrTypes = mapping.ClrTypes;
            }

            internal override void AddTo(BackendTypes types)
            {
                base.AddTo(types);
                foreach (var dbType in _dbTypes)
                    types.ByDbType[dbType] = this;
                foreach (var type in _clrTypes)
                    types.ByClrType[type] = this;
            }

            internal override TypeHandler Activate(TypeHandlerRegistry registry)
            {
                if (HandlerType == null)
                {
                    registry.ByOID[OID] = registry.UnrecognizedTypeHandler;
                    return registry.UnrecognizedTypeHandler;
                }

                var handler = InstantiateHandler(registry);

                registry.ByOID[OID] = handler;

                if (NpgsqlDbType.HasValue)
                {
                    var value = NpgsqlDbType.Value;
                    if (registry._byNpgsqlDbType.ContainsKey(value))
                        throw new Exception($"Two type handlers registered on same NpgsqlDbType {NpgsqlDbType}: {registry._byNpgsqlDbType[value].GetType().Name} and {HandlerType.Name}");
                    registry._byNpgsqlDbType[NpgsqlDbType.Value] = handler;
                }

                if (_dbTypes != null)
                {
                    foreach (var dbType in _dbTypes)
                    {
                        if (registry._byDbType.ContainsKey(dbType))
                            throw new Exception($"Two type handlers registered on same DbType {dbType}: {registry._byDbType[dbType].GetType().Name} and {HandlerType.Name}");
                        registry._byDbType[dbType] = handler;
                    }
                }

                if (_clrTypes != null)
                {
                    foreach (var type in _clrTypes)
                    {
                        if (registry._byType.ContainsKey(type))
                            throw new Exception($"Two type handlers registered on same .NET type {type}: {registry._byType[type].GetType().Name} and {HandlerType.Name}");
                        registry._byType[type] = handler;
                    }
                }

                return handler;
            }

            /// <summary>
            /// Instantiate the type handler. If it has a constructor that accepts a TypeHandlerRegistry, use that to allow
            /// the handler to make connector-specific adjustments. Otherwise (the normal case), use the default constructor.
            /// </summary>
            /// <param name="registry"></param>
            /// <returns></returns>
            TypeHandler InstantiateHandler(TypeHandlerRegistry registry)
            {
                Debug.Assert(HandlerType != null);

                if (_ctorWithRegistry == null && _ctorWithoutRegistry == null)
                {
                    var ctors = HandlerType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    _ctorWithRegistry = (
                        from c in ctors
                        let p = c.GetParameters()
                        where p.Length == 2 && p[0].ParameterType == typeof(IBackendType) && p[1].ParameterType == typeof(TypeHandlerRegistry)
                        select c
                    ).FirstOrDefault();

                    if (_ctorWithRegistry == null)
                    {
                        _ctorWithoutRegistry = (
                            from c in ctors
                            let p = c.GetParameters()
                            where p.Length == 1 && p[0].ParameterType == typeof(IBackendType)
                            select c
                        ).FirstOrDefault();
                        if (_ctorWithoutRegistry == null)
                            throw new Exception($"Type handler type {HandlerType.Name} does not have an appropriate constructor");
                    }
                }

                if (_ctorWithRegistry != null)
                    return (TypeHandler)_ctorWithRegistry.Invoke(new object[] { this, registry });
                Debug.Assert(_ctorWithoutRegistry != null);
                return (TypeHandler)_ctorWithoutRegistry.Invoke(new object[] { this });
            }
        }

        class BackendArrayType : BackendType
        {
            readonly BackendType _element;

            internal BackendArrayType(string ns, string name, uint oid, BackendType elementBackendType)
                : base(ns, name, oid)
            {
                _element = elementBackendType;
                if (elementBackendType.NpgsqlDbType.HasValue)
                    NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Array | elementBackendType.NpgsqlDbType;
                _element.Array = this;
            }

            internal override TypeHandler Activate(TypeHandlerRegistry registry)
            {
                TypeHandler elementHandler;
                if (!registry.TryGetByOID(_element.OID, out elementHandler))
                {
                    // Element type hasn't been set up yet, do it now
                    elementHandler = _element.Activate(registry);
                }

                var arrayHandler = elementHandler.CreateArrayHandler(this);
                registry.ByOID[OID] = arrayHandler;

                var asEnumHandler = elementHandler as IEnumHandler;
                if (asEnumHandler != null)
                {
                    if (registry._arrayHandlerByType == null)
                        registry._arrayHandlerByType = new Dictionary<Type, TypeHandler>();
                    registry._arrayHandlerByType[asEnumHandler.EnumType] = arrayHandler;
                    return arrayHandler;
                }

                var asCompositeHandler = elementHandler as ICompositeHandler;
                if (asCompositeHandler != null)
                {
                    if (registry._arrayHandlerByType == null)
                        registry._arrayHandlerByType = new Dictionary<Type, TypeHandler>();
                    registry._arrayHandlerByType[asCompositeHandler.CompositeType] = arrayHandler;
                    return arrayHandler;
                }

                if (NpgsqlDbType.HasValue)
                    registry._byNpgsqlDbType[NpgsqlDbType.Value] = arrayHandler;

                // Note that array handlers aren't registered in _byType, because they handle all dimension types and not just one CLR type
                // (e.g. int[], int[,], int[,,]). So the by-type lookup is special, see this[Type type]
                // TODO: register single-dimensional in _byType as a specific optimization? But do PSV as well...

                return arrayHandler;
            }
        }

        class BackendRangeType : BackendType
        {
            readonly BackendType _subtype;

            internal BackendRangeType(string ns, string name, uint oid, BackendType subtypeBackendType)
                : base(ns, name, oid)
            {
                _subtype = subtypeBackendType;
                if (subtypeBackendType.NpgsqlDbType.HasValue)
                    NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Range | subtypeBackendType.NpgsqlDbType;
                _subtype.Range = this;
            }

            internal override TypeHandler Activate(TypeHandlerRegistry registry)
            {
                TypeHandler subtypeHandler;
                if (!registry.TryGetByOID(_subtype.OID, out subtypeHandler))
                {
                    // Subtype hasn't been set up yet, do it now
                    subtypeHandler = _subtype.Activate(registry);
                }

                var handler = subtypeHandler.CreateRangeHandler(this);
                registry.ByOID[OID] = handler;
                if (NpgsqlDbType.HasValue)
                    registry._byNpgsqlDbType.Add(NpgsqlDbType.Value, handler);
                registry._byType[handler.GetFieldType()] = handler;
                return handler;
            }
        }

        class BackendEnumType : BackendType
        {
            internal BackendEnumType(string ns, string name, uint oid) : base(ns, name, oid)
            {
                NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Enum;
            }

            internal override void AddTo(BackendTypes types)
            {
                base.AddTo(types);
                types.ByFullName[FullName] = this;
                types.ByName[Name] = types.ByName.ContainsKey(Name)
                    ? null
                    : this;
            }

            internal override TypeHandler Activate(TypeHandlerRegistry registry)
            {
                // Enums need to be mapped by the user with an explicit mapping call (MapComposite or MapCompositeGlobally).
                // If we're here the enum hasn't been mapped to a CLR type and we should activate it as text.
                return new BackendBaseType(Namespace, Name, OID, typeof(TextHandler), new TypeMappingAttribute(Name))
                    .Activate(registry);
            }

            internal void Activate(TypeHandlerRegistry registry, IEnumHandlerFactory handlerFactory)
                => Activate(registry, handlerFactory.Create(this));

            internal void Activate(TypeHandlerRegistry registry, IEnumHandler enumHandler)
            {
                var handler = (TypeHandler)enumHandler;
                registry.ByOID[OID] = handler;
                registry._byType[enumHandler.EnumType] = handler;

                Array?.Activate(registry);
            }
        }

        class BackendCompositeType : BackendType
        {
            /// <summary>
            /// Holds the name and OID for all fields.
            /// Populated on the first activation of the composite.
            /// </summary>
            List<RawCompositeField> _rawFields;

            internal BackendCompositeType(string ns, string name, uint oid, List<RawCompositeField> rawFields)
                : base(ns, name, oid)
            {
                NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Composite;
                _rawFields = rawFields;
            }

            internal override void AddTo(BackendTypes types)
            {
                base.AddTo(types);
                types.ByFullName[FullName] = this;
                types.ByName[Name] = types.ByName.ContainsKey(Name)
                    ? null
                    : this;
            }

            internal override TypeHandler Activate(TypeHandlerRegistry registry)
            {
                // Composites need to be mapped by the user with an explicit mapping call (MapComposite or MapCompositeGlobally).
                // If we're here the enum hasn't been mapped to a CLR type and we should activate it as text.
                throw new Exception($"Composite PostgreSQL type {Name} must be mapped before use");
            }

            internal void Activate(TypeHandlerRegistry registry, ICompositeHandlerFactory factory)
                => Activate(registry, factory.Create(this, registry));

            internal void Activate(TypeHandlerRegistry registry, ICompositeHandler compositeHandler)
            {
                var handler = (TypeHandler)compositeHandler;

                registry.ByOID[OID] = handler;
                registry._byType[compositeHandler.CompositeType] = handler;

                // Inject the raw field information into the composite handler.
                // At this point the composite handler nows about the fields, but hasn't yet resolved the
                // type OIDs to their type handlers. This is done only very late upon first usage of the handler,
                // allowing composite types to be registered and activated in any order regardless of dependencies.
                compositeHandler.RawFields = _rawFields;

                Array?.Activate(registry);
            }
        }

        /// <summary>
        /// Represents a PostgreSQL domain type.
        /// </summary>
        /// <remarks>
        /// When PostgreSQL returns a RowDescription for a domain type, the type OID is the base type's
        /// (so fetching a domain type over text returns a RowDescription for text).
        /// However, when a composite type is returned, the type OID there is that of the domain,
        /// so we provide "clean" support for domain types.
        /// </remarks>
        class BackendDomainType : BackendType
        {
            readonly BackendType _baseBackendType;

            public BackendDomainType(string ns, string name, uint oid, BackendType baseBackendType)
                : base(ns, name, oid)
            {
                _baseBackendType = baseBackendType;
            }

            internal override TypeHandler Activate(TypeHandlerRegistry registry)
            {
                TypeHandler baseTypeHandler;
                if (!registry.TryGetByOID(_baseBackendType.OID, out baseTypeHandler))
                {
                    // Base type hasn't been set up yet, do it now
                    baseTypeHandler = _baseBackendType.Activate(registry);
                }

                // Make the domain type OID point to the base type's type handler, the wire encoding
                // is the same
                registry.ByOID[OID] = baseTypeHandler;

                return baseTypeHandler;
            }
        }

        #endregion

        #region Type Handler Discovery

        static TypeHandlerRegistry()
        {
            _globalEnumMappings = new ConcurrentDictionary<string, IEnumHandlerFactory>();
            _globalCompositeMappings = new ConcurrentDictionary<string, ICompositeHandlerFactory>();

            HandlerTypes = new Dictionary<string, TypeAndMapping>();
            HandlerTypesByNpsgqlDbType = new Dictionary<NpgsqlDbType, TypeAndMapping>();
            NpgsqlDbTypeToDbType = new Dictionary<NpgsqlDbType, DbType>();
            DbTypeToNpgsqlDbType = new Dictionary<DbType, NpgsqlDbType>();
            TypeToNpgsqlDbType = new Dictionary<Type, NpgsqlDbType>();
            TypeToDbType = new Dictionary<Type, DbType>();

            foreach (var t in typeof(TypeHandlerRegistry).GetTypeInfo().Assembly.GetTypes().Where(t => t.GetTypeInfo().IsSubclassOf(typeof(TypeHandler))))
            {
                var mappings = t.GetTypeInfo().GetCustomAttributes(typeof(TypeMappingAttribute), false);
                if (!mappings.Any())
                    continue;

                foreach (TypeMappingAttribute m in mappings)
                {
                    if (HandlerTypes.ContainsKey(m.PgName)) {
                        throw new Exception("Two type handlers registered on same PostgreSQL type name: " + m.PgName);
                    }
                    var typeAndMapping = new TypeAndMapping { HandlerType = t, Mapping = m };
                    HandlerTypes[m.PgName] = typeAndMapping;

                    if (m.NpgsqlDbType.HasValue)
                    {
                        if (HandlerTypesByNpsgqlDbType.ContainsKey(m.NpgsqlDbType.Value)) {
                            throw new Exception("Two type handlers registered on same NpgsqlDbType: " + m.NpgsqlDbType);
                        }
                        HandlerTypesByNpsgqlDbType[m.NpgsqlDbType.Value] = typeAndMapping;
                    }

                    if (!m.NpgsqlDbType.HasValue) {
                        continue;
                    }
                    var npgsqlDbType = m.NpgsqlDbType.Value;

                    var inferredDbType = m.InferredDbType;

                    if (inferredDbType != null) {
                        NpgsqlDbTypeToDbType[npgsqlDbType] = inferredDbType.Value;
                    }
                    foreach (var dbType in m.DbTypes) {
                        DbTypeToNpgsqlDbType[dbType] = npgsqlDbType;
                    }
                    foreach (var type in m.ClrTypes)
                    {
                        TypeToNpgsqlDbType[type] = npgsqlDbType;
                        if (inferredDbType != null) {
                            TypeToDbType[type] = inferredDbType.Value;
                        }
                    }
                }
            }
        }

        #endregion

        #region Misc

        /// <summary>
        /// Clears the internal type cache.
        /// Useful for forcing a reload of the types after loading an extension.
        /// </summary>
        internal static void ClearBackendTypeCache()
        {
            BackendTypeCache.Clear();
        }

        /// <summary>
        /// Clears the internal type cache.
        /// Useful for forcing a reload of the types after loading an extension.
        /// </summary>
        internal static void ClearBackendTypeCache(string connectionString)
        {
            BackendTypes types;
            BackendTypeCache.TryRemove(connectionString, out types);
        }

        static string GetPgName<T>(INpgsqlNameTranslator nameTranslator)
        {
            var attr = typeof(T).GetTypeInfo().GetCustomAttribute<PgNameAttribute>();
            return attr == null
                ? nameTranslator.TranslateTypeName(typeof(T).Name)
                : attr.PgName;
        }

        BackendType GetBackendTypeByName(string pgName)
        {
            BackendType backendType;
            var i = pgName.IndexOf('.');
            if (i == -1)
            {
                // No dot, this is a partial type name
                if (!_backendTypes.ByName.TryGetValue(pgName, out backendType))
                    throw new NpgsqlException($"An PostgreSQL type with the name {pgName} was not found in the database");
                if (backendType == null)
                    throw new NpgsqlException($"More than one PostgreSQL type was found with the name {pgName}, please specify a full name including schema");
                return backendType;
            }

            // Full type name with namespace
            if (!_backendTypes.ByFullName.TryGetValue(pgName, out backendType))
                throw new Exception($"An PostgreSQL type with the name {pgName} was not found in the database");
            return backendType;
        }

        #endregion
    }

    interface IBackendType
    {
        string Namespace { get; }
        string Name { get; }
        uint OID { get; }
        NpgsqlDbType? NpgsqlDbType { get; }
        string FullName { get; }
        string DisplayName { get; }
    }

    struct TypeAndMapping
    {
        internal Type HandlerType;
        internal TypeMappingAttribute Mapping;
    }
}
