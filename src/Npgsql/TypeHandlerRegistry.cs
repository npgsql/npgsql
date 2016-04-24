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
using System.Linq;
using System.Reflection;
using System.Diagnostics.Contracts;
using AsyncRewriter;
using JetBrains.Annotations;
using Npgsql.Logging;
using Npgsql.TypeHandlers;
using NpgsqlTypes;

namespace Npgsql
{
    internal partial class TypeHandlerRegistry
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

        static readonly ConcurrentDictionary<string, IEnumHandler> _globalEnumMappings;
        static readonly ConcurrentDictionary<string, ICompositeHandler> _globalCompositeMappings;

        internal static IDictionary<string, IEnumHandler> GlobalEnumMappings => _globalEnumMappings;
        internal static IDictionary<string, ICompositeHandler> GlobalCompositeMappings => _globalCompositeMappings;

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

        void ActivateGlobalMappings()
        {
            foreach (var kv in _globalEnumMappings)
            {
                BackendType backendType;
                if (!_backendTypes.ByName.TryGetValue(kv.Key, out backendType))
                {
                    Log.Warn($"While attempting to activate global enum mappings, PostgreSQL type {kv.Key} wasn't found in the database. Skipping it.", Connector.Id);
                    continue;
                }
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
                BackendType backendType;
                if (!_backendTypes.ByName.TryGetValue(kv.Key, out backendType))
                {
                    Log.Warn($"While attempting to activate global composite mappings, PostgreSQL type {kv.Key} wasn't found in the database. Skipping it.", Connector.Id);
                    continue;
                }
                var backendCompositeType = backendType as BackendCompositeType;
                if (backendCompositeType == null)
                {
                    Log.Warn($"While attempting to activate global composite mappings, PostgreSQL type {kv.Key} was found but is not a composite. Skipping it.", Connector.Id);
                    continue;
                }
                backendCompositeType.Activate(this, kv.Value);
            }
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
"SELECT a.typname, a.oid, a.typrelid, " +
"CASE WHEN pg_proc.proname='array_recv' THEN 'a' ELSE a.typtype END AS type, " +
"CASE " +
  "WHEN pg_proc.proname='array_recv' THEN a.typelem " +
  (withRange ? "WHEN a.typtype='r' THEN rngsubtype " : "") +
  "ELSE 0 " +
"END AS elemoid, " +
"CASE " +
  "WHEN pg_proc.proname IN ('array_recv','oidvectorrecv') THEN 3 " +  // Arrays last
  "WHEN a.typtype='r' THEN 2 " +                                      // Ranges before
  "WHEN a.typtype='c' THEN 1 " +                                      // Composite types before
  "ELSE 0 " +                                                         // Base types first
"END AS ord " +
"FROM pg_type AS a " +
"JOIN pg_proc ON pg_proc.oid = a.typreceive " +
"LEFT OUTER JOIN pg_type AS b ON (b.oid = a.typelem) " +
(withRange ? "LEFT OUTER JOIN pg_range ON (pg_range.rngtypid = a.oid) " : "") +
"WHERE " +
"  (" +
"    a.typtype IN ('b', 'r', 'e', 'c') AND " +
"    (b.typtype IS NULL OR b.typtype IN ('b', 'r', 'e', 'c'))" +  // Either non-array or array of supported element type
"  ) OR " +
"  a.typname = 'record' " +
"ORDER BY ord";
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
            var name = reader.GetString(0);
            var oid = Convert.ToUInt32(reader[1]);

            Contract.Assume(name != null);
            Contract.Assume(oid != 0);

            uint elementOID;
            var typeChar = reader.GetString(3)[0];
            switch (typeChar)
            {
            case 'b':  // Normal base type
                TypeAndMapping typeAndMapping;
                (
                    HandlerTypes.TryGetValue(name, out typeAndMapping)
                        ? new BackendBaseType(name, oid, typeAndMapping.HandlerType, typeAndMapping.Mapping)
                        : new BackendBaseType(name, oid)  // Unsupported by Npgsql
                ).AddTo(types);
                return;
            case 'a':   // Array
                elementOID = Convert.ToUInt32(reader[4]);
                Contract.Assume(elementOID > 0);
                BackendType elementBackendType;
                if (!types.ByOID.TryGetValue(elementOID, out elementBackendType))
                {
                    Log.Trace($"Array type '{name}' refers to unknown element with OID {elementOID}, skipping", connector.Id);
                    return;
                }
                new BackendArrayType(name, oid, elementBackendType).AddTo(types);
                return;
            case 'r':   // Range
                elementOID = Convert.ToUInt32(reader[4]);
                Contract.Assume(elementOID > 0);
                if (!types.ByOID.TryGetValue(elementOID, out elementBackendType))
                {
                    Log.Trace($"Range type '{name}' refers to unknown subtype with OID {elementOID}, skipping", connector.Id);
                    return;
                }
                new BackendRangeType(name, oid, elementBackendType).AddTo(types);
                return;
            case 'e':   // Enum
                new BackendEnumType(name, oid).AddTo(types);
                return;
            case 'c':   // Composite
                var relationId = Convert.ToUInt32(reader[2]);
                new BackendCompositeType(name, oid, relationId).AddTo(types);
                return;
            case 'p':   // psuedo-type (record only)
                Contract.Assume(name == "record");
                // Hack this as a base type
                goto case 'b';
            default:
                throw new ArgumentOutOfRangeException($"Unknown typtype for type '{name}' in pg_type: {typeChar}");
            }
        }

        #endregion

        #region Enum

        internal void MapEnum<TEnum>(string pgName) where TEnum : struct
        {
            BackendType backendType;
            if (!_backendTypes.ByName.TryGetValue(pgName, out backendType))
                throw new Exception($"An enum with the name {pgName} was not found in the database");
            var asEnumType = backendType as BackendEnumType;
            if (asEnumType == null)
                throw new Exception($"A PostgreSQL type with the name {pgName} was found in the database but it isn't an enum");

            asEnumType.Activate(this, new EnumHandler<TEnum>(pgName));
        }

        internal static void MapEnumGlobally<TEnum>(string pgName) where TEnum : struct
        {
            _globalEnumMappings[pgName] = new EnumHandler<TEnum>(pgName);
        }

        internal static void UnmapEnumGlobally(string pgName)
        {
            IEnumHandler _;
            _globalEnumMappings.TryRemove(pgName, out _);
        }

        #endregion

        #region Composite

        internal void MapComposite<T>(string pgName) where T : new()
        {
            BackendType backendType;
            if (!_backendTypes.ByName.TryGetValue(pgName, out backendType))
                throw new Exception($"A composite with the name {pgName} was not found in the database");
            var asComposite = backendType as BackendCompositeType;
            if (asComposite == null)
                throw new Exception($"Type {pgName} was found in the database but is not a composite");

            asComposite.Activate(this, new CompositeHandler<T>(pgName, this));
        }

        internal static void MapCompositeGlobally<T>(string pgName) where T : new()
        {
            _globalCompositeMappings[pgName] = new CompositeHandler<T>(pgName);
        }

        internal static void UnmapCompositeGlobally(string pgName)
        {
            ICompositeHandler _;
            _globalCompositeMappings.TryRemove(pgName, out _);
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
                Contract.EndContractBlock();

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
                throw new NotSupportedException($"The PostgreSQL type '{typeAndMapping.Mapping.PgName}', mapped to NpgsqlDbType '{npgsqlDbType}' isn't present in your database. " +
                                                    "You may need to install an extension or upgrade to a newer version.");
            }
        }

        internal TypeHandler this[DbType dbType]
        {
            get
            {
                Contract.Ensures(Contract.Result<TypeHandler>() != null);

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
                Contract.Requires(value != null);
                Contract.Ensures(Contract.Result<TypeHandler>() != null);

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
                Contract.Ensures(Contract.Result<TypeHandler>() != null);

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
                        _byNpgsqlDbType.TryGetValue(NpgsqlDbType.Array | elementHandler.NpgsqlDbType, out handler))
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
                            throw new NotSupportedException("The PostgreSQL 'bytea' type is missing");
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
                        throw new NotSupportedException($"The .NET range type {type.Name} isn't supported in your PostgreSQL, use CREATE TYPE AS RANGE");
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
        abstract class BackendType
        {
            protected BackendType(string name, uint oid)
            {
                Name = name;
                OID = oid;
            }

            internal readonly string Name;
            internal readonly uint OID;
            internal BackendArrayType Array;
            internal BackendRangeType Range;

            /// <summary>
            /// For base types, contains the handler type.
            /// If null, this backend type isn't supported by Npgsql.
            /// </summary>
            [CanBeNull]
            internal Type HandlerType;

            internal NpgsqlDbType? NpgsqlDbType;

            internal virtual void AddTo(BackendTypes types)
            {
                types.ByOID[OID] = this;
                types.ByName[Name] = this;
                if (NpgsqlDbType != null)
                    types.ByNpgsqlDbType[NpgsqlDbType.Value] = this;
            }

            internal abstract TypeHandler Activate(TypeHandlerRegistry registry);

            /// <summary>
            /// Returns a string that represents the current object.
            /// </summary>
            public override string ToString() { return Name; }
        }

        class BackendBaseType : BackendType
        {
            readonly DbType[] _dbTypes;
            readonly Type[] _clrTypes;

            /// <summary>
            /// Constructs an unsupported base type (no handler exists in Npgsql for this type)
            /// </summary>
            internal BackendBaseType(string name, uint oid) : base(name, oid)
            {
                _dbTypes = new DbType[0];
                _clrTypes = new Type[0];
            }

            internal BackendBaseType(string name, uint oid, Type handlerType, TypeMappingAttribute mapping) : base(name, oid)
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
                var handlerType = HandlerType;
                if (handlerType == null)
                {
                    registry.ByOID[OID] = registry.UnrecognizedTypeHandler;
                    return registry.UnrecognizedTypeHandler;
                }

                // Instantiate the type handler. If it has a constructor that accepts an NpgsqlConnector, use that to allow
                // the handler to make connector-specific adjustments. Otherwise (the normal case), use the default constructor.
                var handler = (TypeHandler)(
                    handlerType.GetConstructor(new[] { typeof(TypeHandlerRegistry) }) != null
                        ? Activator.CreateInstance(handlerType, registry)
                        : Activator.CreateInstance(handlerType)
                );

                handler.OID = OID;
                handler.PgName = Name;
                registry.ByOID[OID] = handler;

                if (NpgsqlDbType.HasValue)
                {
                    var value = NpgsqlDbType.Value;
                    if (registry._byNpgsqlDbType.ContainsKey(value))
                        throw new Exception($"Two type handlers registered on same NpgsqlDbType {NpgsqlDbType}: {registry._byNpgsqlDbType[value].GetType().Name} and {handlerType.Name}");
                    registry._byNpgsqlDbType[NpgsqlDbType.Value] = handler;
                    handler.NpgsqlDbType = value;
                }

                if (_dbTypes != null)
                {
                    foreach (var dbType in _dbTypes)
                    {
                        if (registry._byDbType.ContainsKey(dbType))
                            throw new Exception($"Two type handlers registered on same DbType {dbType}: {registry._byDbType[dbType].GetType().Name} and {handlerType.Name}");
                        registry._byDbType[dbType] = handler;
                    }
                }

                if (_clrTypes != null)
                {
                    foreach (var type in _clrTypes)
                    {
                        if (registry._byType.ContainsKey(type))
                            throw new Exception($"Two type handlers registered on same .NET type {type}: {registry._byType[type].GetType().Name} and {handlerType.Name}");
                        registry._byType[type] = handler;
                    }
                }

                return handler;
            }
        }

        class BackendArrayType : BackendType
        {
            readonly BackendType _element;

            internal BackendArrayType(string name, uint oid, BackendType elementBackendType) : base(name, oid)
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

                var arrayHandler = elementHandler.CreateArrayHandler(Name, OID);
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

                registry._byNpgsqlDbType[arrayHandler.NpgsqlDbType] = arrayHandler;

                // Note that array handlers aren't registered in _byType, because they handle all dimension types and not just one CLR type
                // (e.g. int[], int[,], int[,,]). So the by-type lookup is special, see this[Type type]
                // TODO: register single-dimensional in _byType as a specific optimization? But do PSV as well...

                return arrayHandler;
            }
        }

        class BackendRangeType : BackendType
        {
            readonly BackendType _subtype;

            internal BackendRangeType(string name, uint oid, BackendType subtypeBackendType) : base(name, oid)
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

                var handler = subtypeHandler.CreateRangeHandler(Name, OID);
                registry.ByOID[OID] = handler;
                registry._byNpgsqlDbType.Add(handler.NpgsqlDbType, handler);
                registry._byType[handler.GetFieldType()] = handler;
                return handler;
            }
        }

        class BackendEnumType : BackendType
        {
            internal BackendEnumType(string name, uint oid) : base(name, oid) { }

            internal override TypeHandler Activate(TypeHandlerRegistry registry)
            {
                // Enums need to be mapped by the user with an explicit mapping call (MapComposite or MapCompositeGlobally).
                // If we're here the enum hasn't been mapped to a CLR type and we should activate it as text.
                return new BackendBaseType(Name, OID, typeof(TextHandler), new TypeMappingAttribute(Name))
                    .Activate(registry);
            }

            internal void Activate(TypeHandlerRegistry registry, IEnumHandler templateHandler)
            {
                // The handler we've received is a global one, effectively serving as a "template".
                // Clone it here to get an instance for our connector
                var enumHandler = templateHandler.Clone();
                var handler = (TypeHandler)enumHandler;
                handler.OID = OID;
                registry.ByOID[OID] = handler;
                registry._byType[templateHandler.EnumType] = handler;

                Array?.Activate(registry);
            }
        }

        class BackendCompositeType : BackendType
        {
            /// <summary>
            /// Contains the pg_class.oid for the type
            /// </summary>
            readonly uint _relationId;

            /// <summary>
            /// Holds the name and OID for all fields.
            /// Populated on the first activation of the composite.
            /// </summary>
            [CanBeNull]
            List<RawCompositeField> _rawFields;

            internal BackendCompositeType(string name, uint oid, uint relationId) : base(name, oid)
            {
                _relationId = relationId;
            }

            internal override TypeHandler Activate(TypeHandlerRegistry registry)
            {
                // Composites need to be mapped by the user with an explicit mapping call (MapComposite or MapCompositeGlobally).
                // If we're here the enum hasn't been mapped to a CLR type and we should activate it as text.
                throw new Exception($"Composite PostgreSQL type {Name} must be mapped before use");
            }

            internal void Activate(TypeHandlerRegistry registry, ICompositeHandler templateHandler)
            {
                // The handler we've received is a global one, effectively serving as a "template".
                // Clone it here to get an instance for our connector
                var compositeHandler = templateHandler.Clone(registry);
                var handler = (TypeHandler)compositeHandler;

                handler.OID = OID;
                registry.ByOID[OID] = handler;
                registry._byType[compositeHandler.CompositeType] = handler;
                Contract.Assume(_relationId != 0);

                var fields = _rawFields;
                if (fields == null)
                {
                    fields = new List<RawCompositeField>();
                    using (var cmd = new NpgsqlCommand($"SELECT attname,atttypid FROM pg_attribute WHERE attrelid={_relationId}", registry.Connector.Connection))
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read())
                            fields.Add(new RawCompositeField { PgName = reader.GetString(0), TypeOID = reader.GetFieldValue<uint>(1) });
                    _rawFields = fields;
                }

                // Inject the raw field information into the composite handler.
                // At this point the composite handler nows about the fields, but hasn't yet resolved the
                // type OIDs to their type handlers. This is done only very late upon first usage of the handler,
                // allowing composite types to be registered and activated in any order regardless of dependencies.
                compositeHandler.RawFields = fields;

                Array?.Activate(registry);
            }
        }

        class BackendPseudoType : BackendType
        {
            internal BackendPseudoType(string name, uint oid) : base(name, oid) { }

            internal override TypeHandler Activate(TypeHandlerRegistry registry)
            {
                throw new NotImplementedException();
            }
        }

#endregion

        #region Type Handler Discovery

        static TypeHandlerRegistry()
        {
            _globalEnumMappings = new ConcurrentDictionary<string, IEnumHandler>();
            _globalCompositeMappings = new ConcurrentDictionary<string, ICompositeHandler>();

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

        #endregion
    }

    struct TypeAndMapping
    {
        internal Type HandlerType;
        internal TypeMappingAttribute Mapping;
    }
}
