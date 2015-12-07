#region License
// The PostgreSQL License
//
// Copyright (C) 2015 The Npgsql Development Team
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
using Npgsql.Logging;
using Npgsql.TypeHandlers;
using NpgsqlTypes;
using System.Diagnostics.Contracts;
using AsyncRewriter;

namespace Npgsql
{
    internal partial class TypeHandlerRegistry
    {
        #region Members

        internal NpgsqlConnector Connector { get; private set; }
        internal TypeHandler UnrecognizedTypeHandler { get; private set; }

        internal Dictionary<uint, TypeHandler> OIDIndex { get; private set; }
        readonly Dictionary<DbType, TypeHandler> _byDbType;
        readonly Dictionary<NpgsqlDbType, TypeHandler> _byNpgsqlDbType;

        /// <summary>
        /// Maps CLR types to their type handlers.
        /// </summary>
        readonly Dictionary<Type, TypeHandler> _byType;

        /// <summary>
        /// Maps CLR types to their array handlerss.
        /// Used only for enum and composite types.
        /// </summary>
        Dictionary<Type, TypeHandler> _arrayHandlerByType;
        List<BackendType> _backendTypes;

        internal static readonly Dictionary<string, TypeAndMapping> HandlerTypes;
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
        static readonly ConcurrentDictionary<string, List<BackendType>> BackendTypeCache = new ConcurrentDictionary<string, List<BackendType>>();

        static readonly ConcurrentDictionary<string, TypeHandler> _globalEnumMappings;
        static readonly ConcurrentDictionary<string, TypeHandler> _globalCompositeMappings;

        internal static IDictionary<string, TypeHandler> GlobalEnumMappings => _globalEnumMappings;
        internal static IDictionary<string, TypeHandler> GlobalCompositeMappings => _globalCompositeMappings;

        static readonly NpgsqlLogger Log = NpgsqlLogManager.GetCurrentClassLogger();

        #endregion

        #region Initialization and Loading

        [RewriteAsync]
        internal static void Setup(NpgsqlConnector connector, NpgsqlTimeout timeout)
        {
            connector.TypeHandlerRegistry = new TypeHandlerRegistry(connector);

            List<BackendType> types;
            if (!BackendTypeCache.TryGetValue(connector.ConnectionString, out types)) {
                types = BackendTypeCache[connector.ConnectionString] = LoadBackendTypes(connector, timeout);
            }

            connector.TypeHandlerRegistry.RegisterTypes(types);
        }

        TypeHandlerRegistry(NpgsqlConnector connector)
        {
            Connector = connector;
            UnrecognizedTypeHandler = new UnrecognizedTypeHandler();
            OIDIndex = new Dictionary<uint, TypeHandler>();
            _byDbType = new Dictionary<DbType, TypeHandler>();
            _byNpgsqlDbType = new Dictionary<NpgsqlDbType, TypeHandler>();
            _byType = new Dictionary<Type, TypeHandler>();
            _byType[typeof(DBNull)] = UnrecognizedTypeHandler;
            _byNpgsqlDbType[NpgsqlDbType.Unknown] = UnrecognizedTypeHandler;
        }

        [RewriteAsync]
        static List<BackendType> LoadBackendTypes(NpgsqlConnector connector, NpgsqlTimeout timeout)
        {
            var byOID = new Dictionary<uint, BackendType>();

            // Select all types (base, array which is also base, enum, range, composite).
            // Note that arrays are distinguished from primitive types through them having typreceive=array_recv.
            // Order by primitives first, container later.
            // For arrays and ranges, join in the element OID and type (to filter out arrays of unhandled
            // types).
            var query =
                @"SELECT a.typname, a.oid, a.typrelid, " +
                @"CASE WHEN pg_proc.proname='array_recv' THEN 'a' ELSE a.typtype END AS type, " +
                @"CASE " +
                  @"WHEN pg_proc.proname='array_recv' THEN a.typelem " +
                  (connector.SupportsRangeTypes ? @"WHEN a.typtype='r' THEN rngsubtype " : "")+
                  @"ELSE 0 " +
                @"END AS elemoid, " +
                @"CASE " +
                  @"WHEN pg_proc.proname IN ('array_recv','oidvectorrecv') THEN 3 " +  // Arrays last
                  @"WHEN a.typtype='r' THEN 2 " +                                      // Ranges before
                  @"WHEN a.typtype='c' THEN 1 " +                                      // Composite types before
                  @"ELSE 0 " +                                                         // Base types first
                @"END AS ord " +
                @"FROM pg_type AS a " +
                @"JOIN pg_proc ON pg_proc.oid = a.typreceive " +
                @"LEFT OUTER JOIN pg_type AS b ON (b.oid = a.typelem) " +
                (connector.SupportsRangeTypes ? @"LEFT OUTER JOIN pg_range ON (pg_range.rngtypid = a.oid) " : "") +
                @"WHERE " +
                @"  (" +
                @"    a.typtype IN ('b', 'r', 'e', 'c') AND " +
                @"    (b.typtype IS NULL OR b.typtype IN ('b', 'r', 'e', 'c'))" +  // Either non-array or array of supported element type
                @"  ) OR " +
                @"  a.typname = 'record' " +
                @"ORDER BY ord";

            var types = new List<BackendType>();
            using (var command = new NpgsqlCommand(query, connector.Connection))
            {
                command.CommandTimeout = timeout.IsSet ? (int)timeout.TimeLeft.TotalSeconds : 0;
                command.AllResultTypesAreUnknown = true;
                using (var dr = command.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        timeout.Check();
                        var backendType = new BackendType
                        {
                            Name = dr.GetString(0),
                            OID = Convert.ToUInt32(dr[1])
                        };

                        Contract.Assume(backendType.Name != null);
                        Contract.Assume(backendType.OID != 0);

                        uint elementOID;
                        var typeChar = dr.GetString(3)[0];
                        switch (typeChar)
                        {
                        case 'b':  // Normal base type
                            backendType.Type = BackendTypeType.Base;
                            break;
                        case 'a':   // Array
                            backendType.Type = BackendTypeType.Array;
                            elementOID = Convert.ToUInt32(dr[4]);
                            Contract.Assume(elementOID > 0);
                            if (!byOID.TryGetValue(elementOID, out backendType.Element)) {
                                Log.Trace(
                                    $"Array type '{backendType.Name}' refers to unknown element with OID {elementOID}, skipping", connector.Id);
                                continue;
                            }
                            backendType.Element.Array = backendType;
                            break;
                        case 'e':   // Enum
                            backendType.Type = BackendTypeType.Enum;
                            break;
                        case 'r':   // Range
                            backendType.Type = BackendTypeType.Range;
                            elementOID = Convert.ToUInt32(dr[4]);
                            Contract.Assume(elementOID > 0);
                            if (!byOID.TryGetValue(elementOID, out backendType.Element)) {
                                Log.Error(
                                    $"Range type '{backendType.Name}' refers to unknown subtype with OID {elementOID}, skipping", connector.Id);
                                continue;
                            }
                            break;
                        case 'c':   // Composite
                            backendType.Type = BackendTypeType.Composite;
                            backendType.RelationId = Convert.ToUInt32(dr[2]);
                            break;
                        case 'p':   // psuedo-type (record only)
                            Contract.Assume(backendType.Name == "record");
                            // Hack this as a base type
                            backendType.Type = BackendTypeType.Base;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(
                                $"Unknown typtype for type '{backendType.Name}' in pg_type: {typeChar}");
                        }

                        types.Add(backendType);
                        byOID[backendType.OID] = backendType;
                    }
                }
            }

            /*foreach (var notFound in _typeHandlers.Where(t => t.Oid == -1)) {
                _log.WarnFormat("Could not find type {0} in pg_type", notFound.PgNames[0]);
            }*/

            return types;
        }

        void RegisterTypes(List<BackendType> backendTypes)
        {
            foreach (var backendType in backendTypes)
            {
                try
                {
                    switch (backendType.Type)
                    {
                    case BackendTypeType.Base:
                        TypeAndMapping typeAndMapping;
                        // Types whose names aren't in HandlerTypes aren't supported by Npgql, skip them
                        if (HandlerTypes.TryGetValue(backendType.Name, out typeAndMapping)) {
                            RegisterBaseType(backendType.Name, backendType.OID, typeAndMapping.HandlerType, typeAndMapping.Mapping);
                        }
                        continue;

                    case BackendTypeType.Array:
                        RegisterArrayType(backendType);
                        continue;

                    case BackendTypeType.Range:
                        RegisterRangeType(backendType);
                        continue;

                    case BackendTypeType.Enum:
                        TypeHandler handler;
                        if (_globalEnumMappings.TryGetValue(backendType.Name, out handler))
                        {
                            ActivateEnumType(handler, backendType);
                        }
                        else
                        {
                            // Unregistered enum, register as text
                            RegisterBaseType(backendType.Name, backendType.OID, typeof(TextHandler), new TypeMappingAttribute(backendType.Name));
                        }
                        continue;

                    case BackendTypeType.Composite:
                        if (_globalCompositeMappings != null &&
                            _globalCompositeMappings.TryGetValue(backendType.Name, out handler))
                        {
                            ActivateCompositeType(handler, backendType);
                        }
                        continue;

                    default:
                        Log.Error("Unknown type of type encountered, skipping: " + backendType, Connector.Id);
                        continue;
                    }
                }
                catch (Exception e)
                {
                    Log.Error("Exception while registering type " + backendType.Name, e, Connector.Id);
                }
            }

            _backendTypes = backendTypes;
        }

        void RegisterBaseType(string name, uint oid, Type handlerType, TypeMappingAttribute mapping)
        {
            // Instantiate the type handler. If it has a constructor that accepts an NpgsqlConnector, use that to allow
            // the handler to make connector-specific adjustments. Otherwise (the normal case), use the default constructor.
            var handler = (TypeHandler)(
                handlerType.GetConstructor(new[] { typeof(TypeHandlerRegistry) }) != null
                    ? Activator.CreateInstance(handlerType, this)
                    : Activator.CreateInstance(handlerType)
            );

            handler.OID = oid;
            OIDIndex[oid] = handler;
            handler.PgName = name;

            if (mapping.NpgsqlDbType.HasValue)
            {
                var npgsqlDbType = mapping.NpgsqlDbType.Value;
                if (_byNpgsqlDbType.ContainsKey(npgsqlDbType))
                    throw new Exception(
                        $"Two type handlers registered on same NpgsqlDbType {npgsqlDbType}: {_byNpgsqlDbType[npgsqlDbType].GetType().Name} and {handlerType.Name}");
                _byNpgsqlDbType[npgsqlDbType] = handler;
                handler.NpgsqlDbType = npgsqlDbType;
            }

            foreach (var dbType in mapping.DbTypes)
            {
                if (_byDbType.ContainsKey(dbType))
                    throw new Exception(
                        $"Two type handlers registered on same DbType {dbType}: {_byDbType[dbType].GetType().Name} and {handlerType.Name}");
                _byDbType[dbType] = handler;
            }

            foreach (var type in mapping.Types)
            {
                if (_byType.ContainsKey(type))
                    throw new Exception(
                        $"Two type handlers registered on same .NET type {type}: {_byType[type].GetType().Name} and {handlerType.Name}");
                _byType[type] = handler;
            }
        }

        #endregion

        #region Array

        void RegisterArrayType(BackendType backendType)
        {
            Contract.Requires(backendType.Element != null);

            TypeHandler elementHandler;
            if (!OIDIndex.TryGetValue(backendType.Element.OID, out elementHandler)) {
                // Array type referring to an unhandled element type
                return;
            }

            TypeHandler arrayHandler;

            var asBitStringHandler = elementHandler as BitStringHandler;
            if (asBitStringHandler != null) {
                // BitString requires a special array handler which returns bool or BitArray
                arrayHandler = new BitStringArrayHandler(asBitStringHandler);
            } else if (elementHandler is ITypeHandlerWithPsv) {
                var arrayHandlerType = typeof(ArrayHandlerWithPsv<,>).MakeGenericType(elementHandler.GetFieldType(), elementHandler.GetProviderSpecificFieldType());
                arrayHandler = (TypeHandler)Activator.CreateInstance(arrayHandlerType, elementHandler);
            } else {
                var arrayHandlerType = typeof(ArrayHandler<>).MakeGenericType(elementHandler.GetFieldType());
                arrayHandler = (TypeHandler)Activator.CreateInstance(arrayHandlerType, elementHandler);
            }

            arrayHandler.PgName = backendType.Name;
            arrayHandler.OID = backendType.OID;
            OIDIndex[backendType.OID] = arrayHandler;

            var asEnumHandler = elementHandler as IEnumHandler;
            if (asEnumHandler != null)
            {
                if (_arrayHandlerByType == null) {
                    _arrayHandlerByType = new Dictionary<Type, TypeHandler>();
                }
                _arrayHandlerByType[asEnumHandler.EnumType] = arrayHandler;
                return;
            }

            var asCompositeHandler = elementHandler as ICompositeHandler;
            if (asCompositeHandler != null)
            {
                if (_arrayHandlerByType == null) {
                    _arrayHandlerByType = new Dictionary<Type, TypeHandler>();
                }
                _arrayHandlerByType[asCompositeHandler.CompositeType] = arrayHandler;
                return;
            }

            _byNpgsqlDbType[NpgsqlDbType.Array | elementHandler.NpgsqlDbType] = arrayHandler;
        }

        #endregion

        #region Range

        void RegisterRangeType(BackendType backendType)
        {
            Contract.Requires(backendType.Element != null);

            TypeHandler elementHandler;
            if (!OIDIndex.TryGetValue(backendType.Element.OID, out elementHandler))
            {
                // Range type referring to an unhandled element type
                return;
            }

            var rangeHandlerType = typeof(RangeHandler<>).MakeGenericType(elementHandler.GetFieldType());
            var handler = (TypeHandler)Activator.CreateInstance(rangeHandlerType, elementHandler, backendType.Name);

            handler.PgName = backendType.Name;
            handler.NpgsqlDbType = NpgsqlDbType.Range | elementHandler.NpgsqlDbType;
            handler.OID = backendType.OID;
            OIDIndex[backendType.OID] = handler;
            _byNpgsqlDbType.Add(handler.NpgsqlDbType, handler);
        }

        #endregion

        #region Enum

        internal void RegisterEnumType<TEnum>(string pgName) where TEnum : struct
        {
            var backendTypeInfo = _backendTypes.FirstOrDefault(t => t.Name == pgName);
            if (backendTypeInfo == null) {
                throw new Exception($"An enum with the name {pgName} was not found in the database");
            }

            var handler = new EnumHandler<TEnum>();
            ActivateEnumType(handler, backendTypeInfo);
        }

        internal static void MapEnumTypeGlobally<TEnum>(string pgName) where TEnum : struct
        {
            _globalEnumMappings[pgName] = new EnumHandler<TEnum>();
        }

        void ActivateEnumType(TypeHandler handler, BackendType backendType)
        {
            if (backendType.Type != BackendTypeType.Enum)
                throw new InvalidOperationException(
                    $"Trying to activate backend type {backendType.Name} of as enum but is of type {backendType.Type}");

            handler.PgName = backendType.Name;
            handler.OID = backendType.OID;
            handler.NpgsqlDbType = NpgsqlDbType.Enum;
            OIDIndex[backendType.OID] = handler;
            _byType[handler.GetFieldType()] = handler;

            if (backendType.Array != null) {
                RegisterArrayType(backendType.Array);
            }
        }

        internal static void UnregisterEnumTypeGlobally<TEnum>() where TEnum : struct
        {
            var pgName = _globalEnumMappings
                .Single(kv => kv.Value is EnumHandler<TEnum>)
                .Key;
            TypeHandler _;
            _globalEnumMappings.TryRemove(pgName, out _);
        }

        #endregion

        #region Composite

        internal void MapCompositeType<T>(string pgName) where T : new()
        {
            var backendTypeInfo = _backendTypes.FirstOrDefault(t => t.Name == pgName);
            if (backendTypeInfo == null)
            {
                throw new Exception($"A composite with the name {pgName} was not found in the database");
            }

            var handler = new CompositeHandler<T>();
            ActivateCompositeType(handler, backendTypeInfo);
        }

        internal static void MapCompositeTypeGlobally<T>(string pgName) where T : new()
        {
            _globalCompositeMappings[pgName] = new CompositeHandler<T>();
        }

        void ActivateCompositeType(TypeHandler templateHandler, BackendType backendType)
        {
            if (backendType.Type != BackendTypeType.Composite)
                throw new InvalidOperationException(
                    $"Trying to activate backend type {backendType.Name} of as composite but is of type {backendType.Type}");

            // The handler we've received is a global one, effectively serving as a "template".
            // Clone it here to get an instance for our connector
            var asCompositeHandler = ((ICompositeHandler)templateHandler).Clone(this);
            var handler = (TypeHandler)asCompositeHandler;

            handler.PgName = backendType.Name;
            handler.OID = backendType.OID;
            handler.NpgsqlDbType = NpgsqlDbType.Composite;
            OIDIndex[backendType.OID] = handler;
            _byType[handler.GetFieldType()] = handler;
            Contract.Assume(backendType.RelationId != 0);

            var fields = backendType.RawFields;
            if (fields == null)
            {
                fields = new List<Tuple<string, uint>>();
                using (var cmd = new NpgsqlCommand(
                    $"SELECT attname,atttypid FROM pg_attribute WHERE attrelid={backendType.RelationId}", Connector.Connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read()) {
                        fields.Add(new Tuple<string, uint>(reader.GetString(0), reader.GetFieldValue<uint>(1)));
                    }
                }
                backendType.RawFields = fields;
            }

            // Inject the raw field information into the composite handler.
            // At this point the composite handler nows about the fields, but hasn't yet resolved the
            // type OIDs to their type handlers. This is done only very late upon first usage of the handler,
            // allowing composite types to be registered and activated in any order regardless of dependencies.
            asCompositeHandler.RawFields = fields;

            if (backendType.Array != null)
            {
                RegisterArrayType(backendType.Array);
            }
        }

        internal static void UnregisterCompositeTypeGlobally(string pgName)
        {
            TypeHandler _;
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
                if (!OIDIndex.TryGetValue(oid, out result)) {
                    result = UnrecognizedTypeHandler;
                }
                return result;
            }
            set { OIDIndex[oid] = value; }
        }

        internal TypeHandler this[NpgsqlDbType npgsqlDbType, Type specificType = null]
        {
            get
            {
                TypeHandler handler;
                if (_byNpgsqlDbType.TryGetValue(npgsqlDbType, out handler)) {
                    return handler;
                }

                if (specificType == null)
                {
                    if (npgsqlDbType == NpgsqlDbType.Enum || npgsqlDbType == NpgsqlDbType.Composite)
                    {
                        throw new InvalidCastException(string.Format("When specifying NpgsqlDbType.{0}, {0}Type must be specified as well",
                                                       npgsqlDbType == NpgsqlDbType.Enum ? "Enum" : "Composite"));
                    }
                    throw new NotSupportedException("This NpgsqlDbType isn't supported in Npgsql yet: " + npgsqlDbType);
                }

                if ((npgsqlDbType & NpgsqlDbType.Array) != 0)
                {
                    if (_arrayHandlerByType != null && _arrayHandlerByType.TryGetValue(specificType, out handler))
                    {
                        return handler;
                    }
                }
                else if (_byType.TryGetValue(specificType, out handler))
                {
                    return handler;
                }

                if (npgsqlDbType != NpgsqlDbType.Enum && npgsqlDbType != NpgsqlDbType.Composite)
                {
                    throw new ArgumentException("specificType can only be set if NpgsqlDbType is set to Enum or Composite");
                }
                throw new NotSupportedException(
                    $"The CLR type {specificType.Name} must be registered with Npgsql before usage, please refer to the documentation");
            }
        }

        internal TypeHandler this[DbType dbType]
        {
            get
            {
                Contract.Ensures(Contract.Result<TypeHandler>() != null);

                TypeHandler handler;
                if (!_byDbType.TryGetValue(dbType, out handler)) {
                    throw new NotSupportedException("This DbType is not supported in Npgsql: " + dbType);
                }
                return handler;
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
                if (_byType.TryGetValue(type, out handler)) {
                    return handler;
                }

                Type arrayElementType = null;

                // Detect arrays and (generic) ILists
                if (type.IsArray)
                {
                    arrayElementType = type.GetElementType();
                }
                else if (typeof(IList).IsAssignableFrom(type))
                {
                    if (!type.GetTypeInfo().IsGenericType)
                    {
                        throw new NotSupportedException("Non-generic IList is a supported parameter, but the NpgsqlDbType parameter must be set on the parameter");
                    }
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
                    {
                        return handler;
                    }

                    if (arrayElementType.GetTypeInfo().IsEnum)
                    {
                        throw new NotSupportedException(
                            $"The CLR enum type {arrayElementType.Name} must be registered with Npgsql before usage, please refer to the documentation.");
                    }

                    throw new NotSupportedException(
                        $"The CLR type {arrayElementType} isn't supported by Npgsql or your PostgreSQL. " +
                        "If you wish to map it to a PostgreSQL composite type you need to register it before usage, please refer to the documentation.");
                }

                // Only errors from here

                if (type.GetTypeInfo().IsEnum) {
                    throw new NotSupportedException(
                        $"The CLR enum type {type.Name} must be registered with Npgsql before usage, please refer to the documentation.");
                }

                if (typeof (IEnumerable).IsAssignableFrom(type))
                {
                    throw new NotSupportedException($"Npgsql > 3.x removed support for writing a parameter with an IEnumerable value, use .ToList()/.ToArray() instead");
                }

                if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(NpgsqlRange<>))
                {
                    if (!_byType.TryGetValue(type.GetGenericArguments()[0], out handler)) {
                        throw new NotSupportedException("This .NET range type is not supported in your PostgreSQL: " + type);
                    }
                    return this[NpgsqlDbType.Range | handler.NpgsqlDbType];
                }

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
                if (type == typeof(byte[])) {
                    return NpgsqlDbType.Bytea;
                }
                return NpgsqlDbType.Array | ToNpgsqlDbType(type.GetElementType());
            }

            var typeInfo = type.GetTypeInfo();

            if (typeInfo.IsEnum) {
                return NpgsqlDbType.Enum;
            }

            if (typeInfo.IsGenericType && type.GetGenericTypeDefinition() == typeof(NpgsqlRange<>)) {
                return NpgsqlDbType.Range | ToNpgsqlDbType(type.GetGenericArguments()[0]);
            }

            if (type == typeof(DBNull))
            {
                return NpgsqlDbType.Unknown;
            }

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

        #region Type Handler Discovery

        static TypeHandlerRegistry()
        {
            _globalEnumMappings = new ConcurrentDictionary<string, TypeHandler>();
            _globalCompositeMappings = new ConcurrentDictionary<string, TypeHandler>();

            HandlerTypes = new Dictionary<string, TypeAndMapping>();
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
                    HandlerTypes[m.PgName] = new TypeAndMapping { HandlerType=t, Mapping=m };
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
                    foreach (var type in m.Types)
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
        static internal void ClearBackendTypeCache()
        {
            BackendTypeCache.Clear();
        }

        /// <summary>
        /// Clears the internal type cache.
        /// Useful for forcing a reload of the types after loading an extension.
        /// </summary>
        static internal void ClearBackendTypeCache(string connectionString)
        {
            List<BackendType> types;
            BackendTypeCache.TryRemove(connectionString, out types);
        }

        #endregion
    }

    class BackendType
    {
        internal string Name;
        internal uint OID;
        internal BackendTypeType Type;
        internal BackendType Element;
        internal BackendType Array;

        /// <summary>
        /// For composite types, contains the pg_class.oid for the type
        /// </summary>
        internal uint RelationId;

        /// <summary>
        /// For composite types, holds the name and OID for all fields.
        /// </summary>
        internal List<Tuple<string, uint>> RawFields;

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        public override string ToString()
        {
            return Name;
        }
    }

    struct TypeAndMapping
    {
        internal Type HandlerType;
        internal TypeMappingAttribute Mapping;
    }

    /// <summary>
    /// Specifies the type of a type, as represented in the PostgreSQL typtype column of the pg_type table.
    /// See http://www.postgresql.org/docs/current/static/catalog-pg-type.html
    /// </summary>
    enum BackendTypeType
    {
        Base,
        Array,
        Range,
        Enum,
        Composite,
        Pseudo
    }
}
