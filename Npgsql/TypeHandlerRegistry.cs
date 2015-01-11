using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Common.Logging;
using Npgsql.TypeHandlers;
using NpgsqlTypes;
using System.Diagnostics.Contracts;

namespace Npgsql
{
    internal class TypeHandlerRegistry
    {
        #region Members

        readonly Dictionary<string, TypeHandler> _nameIndex;
        readonly Dictionary<uint, TypeHandler> _oidIndex;

        readonly Dictionary<DbType, TypeHandler> _byDbType;
        readonly Dictionary<NpgsqlDbType, TypeHandler> _byNpgsqlDbType;
        readonly Dictionary<Type, TypeHandler> _byType;

        public TypeHandler UnknownTypeHandler { get; private set; }

        static readonly List<Type> HandlerTypes;
        static readonly Dictionary<DbType, NpgsqlDbType> _npgsqlDbTypeToDbType;
        static readonly ConcurrentDictionary<string, TypeHandlerRegistry> _registryCache = new ConcurrentDictionary<string, TypeHandlerRegistry>();
        static readonly ILog Log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Initialization and Loading

        static internal void Setup(NpgsqlConnector connector)
        {
            // TODO: Implement caching again
            /*
            TypeHandlerRegistry registry;
            if (_registryCache.TryGetValue(connector.ConnectionString, out registry))
            {
                connector.TypeHandlerRegistry = registry;
            }

            connector.TypeHandlerRegistry = _registryCache[connector.ConnectionString] = new TypeHandlerRegistry(connector);
             */
            connector.TypeHandlerRegistry = new TypeHandlerRegistry(connector);
        }

        TypeHandlerRegistry(NpgsqlConnector connector)
        {
            Log.Debug("Loading types for connection string: " + connector.ConnectionString);

            _oidIndex = new Dictionary<uint, TypeHandler>();
            _byDbType = new Dictionary<DbType, TypeHandler>();
            _byNpgsqlDbType = new Dictionary<NpgsqlDbType, TypeHandler>();
            _byType = new Dictionary<Type, TypeHandler>();
            UnknownTypeHandler = new UnknownTypeHandler { OID=0 };

            // Below we'll be sending in a query to load OIDs from the backend, but parsing those results will depend
            // on... the OIDs. To solve this chicken and egg problem, set up an empty type handler registry that will
            // enable us to at least read strings via the UnknownTypeHandler
            //connector.TypeHandlerRegistry = EmptyRegistry;
            connector.TypeHandlerRegistry = this;

            var inList = new StringBuilder();
            _nameIndex = new Dictionary<string, TypeHandler>();

            foreach (var handlerType in HandlerTypes)
            {
                foreach (TypeMappingAttribute attr in handlerType.GetCustomAttributes(typeof(TypeMappingAttribute), false))
                {
                    // TODO: Pass the connector to the type handler constructor for optional customizations
                    var handler = (TypeHandler)Activator.CreateInstance(handlerType);

                    if (_nameIndex.ContainsKey(attr.PgName))
                        throw new Exception(String.Format("Two type handlers registered on same PostgreSQL type name {0}: {1} and {2}", attr.PgName, _nameIndex[attr.PgName].GetType().Name, handlerType.Name));
                    handler.PgName = attr.PgName;
                    _nameIndex[attr.PgName] = handler;

                    if (_byNpgsqlDbType.ContainsKey(attr.NpgsqlDbType))
                        throw new Exception(String.Format("Two type handlers registered on same NpgsqlDbType {0}: {1} and {2}",
                                            attr.NpgsqlDbType, _byNpgsqlDbType[attr.NpgsqlDbType].GetType().Name, handlerType.Name));
                    _byNpgsqlDbType[attr.NpgsqlDbType] = handler;
                    handler.NpgsqlDbType = attr.NpgsqlDbType;

                    foreach (var dbType in attr.DbTypes)
                    {
                        if (_byDbType.ContainsKey(dbType))
                            throw new Exception(String.Format("Two type handlers registered on same DbType {0}: {1} and {2}",
                                                dbType, _byDbType[dbType].GetType().Name, handlerType.Name));
                        _byDbType[dbType] = handler;
                    }

                    foreach (var type in attr.Types)
                    {
                        if (_byType.ContainsKey(type))
                            throw new Exception(String.Format("Two type handlers registered on same .NET type {0}: {1} and {2}",
                                                type, _byType[type].GetType().Name, handlerType.Name));
                        _byType[type] = handler;
                    }

                    inList.AppendFormat("{0}'{1}'", ((inList.Length > 0) ? "," : ""), attr.PgName);
                }
            }

            const string query = @"SELECT typname, pg_type.oid, typtype, typarray, typdelim, rngsubtype " +
                                 @"FROM pg_type LEFT OUTER JOIN pg_range ON (pg_type.oid = pg_range.rngtypid) " +
                                 @"WHERE typname in ({0}) OR typtype = 'r' ORDER BY typtype";
            var parameterizedQuery = String.Format(query, inList);
            using (var command = new NpgsqlCommand(parameterizedQuery, connector))
            {
                using (var dr = command.GetReader(CommandBehavior.SequentialAccess))
                {
                    while (dr.Read())
                    {
                        var name = dr.GetString(0);
                        Contract.Assume(name != null);
                        var oid = Convert.ToUInt32(dr[1]);
                        Contract.Assume(oid != 0);
                        var typtype = dr.GetString(2)[0];
                        var arrayOID = Convert.ToUInt32(dr[3]);
                        var textDelimiter = dr.GetString(4)[0];

                        switch (typtype)
                        {
                            case 'b':
                                RegisterBaseType(name, oid, arrayOID, textDelimiter);
                                continue;
                            case 'r':
                                var rangeSubtypeOID = UInt32.Parse(dr.GetString(5));
                                Contract.Assume(rangeSubtypeOID != 0);
                                RegisterRangeType(name, oid, arrayOID, textDelimiter, rangeSubtypeOID);
                                continue;
                            default:
                                Log.Error("Unknown type of type encountered, skipping: " + typtype);
                                continue;
                        }
                    }
                }
            }

            /*foreach (var notFound in _typeHandlers.Where(t => t.Oid == -1)) {
                _log.WarnFormat("Could not find type {0} in pg_type", notFound.PgNames[0]);
            }*/

            connector.TypeHandlerRegistry = _registryCache[connector.ConnectionString] = this;
        }

        void RegisterBaseType(string name, uint oid, uint arrayOID, char textDelimiter)
        {
            var handler = _nameIndex[name];
            Contract.Assume(handler.OID == 0);
            handler.OID = oid;
            _oidIndex[oid] = handler;

            if (arrayOID == 0) {
                return;
            }

            // The backend has a corresponding array type for this type.
            // Use reflection to create a constructed type from the relevant ArrayHandler
            // generic type definition.
            var arrayHandler = CreateArrayHandler(handler, textDelimiter);
             arrayHandler.OID = arrayOID;
            _oidIndex[arrayOID] = arrayHandler;
            _byNpgsqlDbType.Add(NpgsqlDbType.Array | handler.NpgsqlDbType, arrayHandler);
        }

        void RegisterRangeType(string name, uint oid, uint arrayOID, char textDelimiter, uint elementOid)
        {
            TypeHandler elementHandler;
            if (!_oidIndex.TryGetValue(elementOid, out elementHandler))
            {
                Log.ErrorFormat("Range type '{0}' refers to unknown subtype with OID {1}, skipping", name, elementOid);
                return;
            }

            var rangeHandlerType = typeof(RangeHandler<>).MakeGenericType(elementHandler.GetFieldType());
            var handler = (TypeHandler)Activator.CreateInstance(rangeHandlerType, elementHandler, name);

            handler.OID = oid;
            _oidIndex[oid] = handler;
            _byNpgsqlDbType.Add(NpgsqlDbType.Range | elementHandler.NpgsqlDbType, handler);

            // Array of ranges
            var arrayHandler = CreateArrayHandler(handler, ',');
             arrayHandler.OID = arrayOID;
            _oidIndex[arrayOID] = arrayHandler;
            _byNpgsqlDbType.Add(NpgsqlDbType.Range | NpgsqlDbType.Array | elementHandler.NpgsqlDbType, handler);
        }

        static ArrayHandler CreateArrayHandler(TypeHandler elementHandler, char textDelimiter)
        {
            ArrayHandler arrayHandler;

            var asBitStringHandler = elementHandler as BitStringHandler;
            if (asBitStringHandler != null)
            {
                // BitString requires a special array handler which returns bool or BitArray
                arrayHandler = new BitStringArrayHandler(asBitStringHandler, textDelimiter);
            }
            else if (elementHandler is ITypeHandlerWithPsv)
            {
                var arrayHandlerType = typeof(ArrayHandlerWithPsv<,>).MakeGenericType(elementHandler.GetFieldType(), elementHandler.GetProviderSpecificFieldType());
                arrayHandler = (ArrayHandler)Activator.CreateInstance(arrayHandlerType, elementHandler, textDelimiter);
            }
            else
            {
                var arrayHandlerType = typeof(ArrayHandler<>).MakeGenericType(elementHandler.GetFieldType());
                arrayHandler = (ArrayHandler)Activator.CreateInstance(arrayHandlerType, elementHandler, textDelimiter);
            }

            return arrayHandler;
        }

        #endregion

        #region Lookups

        /// <summary>
        /// Looks up a type handler by its Postgresql type's OID.
        /// </summary>
        /// <param name="oid">A Postgresql type OID</param>
        /// <returns>A type handler that can be used to encode and decode values.</returns>
        internal TypeHandler this[uint oid]
        {
            get
            {
                TypeHandler result;
                if (!_oidIndex.TryGetValue(oid, out result)) {
                    result = UnknownTypeHandler;
                }
                return result;
            }
            set { _oidIndex[oid] = value; }
        }

        internal TypeHandler this[NpgsqlDbType npgsqlDbType]
        {
            get
            {
                TypeHandler handler;
                var exists = _byNpgsqlDbType.TryGetValue(npgsqlDbType, out handler);
                Contract.Assume(exists);
                return handler;
            }
        }

        internal TypeHandler this[DbType dbType]
        {
            get
            {
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
                if (value == null || value is DBNull)
                    return UnknownTypeHandler;

                if (value is DateTime)
                {
                    return ((DateTime) value).Kind == DateTimeKind.Utc
                        ? this[NpgsqlDbType.TimestampTZ]
                        : this[NpgsqlDbType.Timestamp];
                }

                return this[value.GetType()];
            }
        }

        TypeHandler this[Type type]
        {
            get
            {
                if (type.IsArray) {
                    return type == typeof(byte[])
                        ? this[NpgsqlDbType.Bytea]
                        : this[NpgsqlDbType.Array | this[type.GetElementType()].NpgsqlDbType];
                }

                TypeHandler handler;
                if (!_byType.TryGetValue(type, out handler)) {
                    throw new NotSupportedException("This .NET type is not supported in Npgsql: " + type);
                }
                return handler;
            }
        }

        internal static NpgsqlDbType ToNpgsqlDbType(DbType dbType)
        {
            return _npgsqlDbTypeToDbType[dbType];
        }

        #endregion

        #region Type Discovery

        static TypeHandlerRegistry()
        {
            HandlerTypes = new List<Type>();
            _npgsqlDbTypeToDbType = new Dictionary<DbType, NpgsqlDbType>();

            foreach (var t in Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof (TypeHandler))))
            {
                var mappings = t.GetCustomAttributes(typeof (TypeMappingAttribute), false);
                if (!mappings.Any())
                    continue;
                HandlerTypes.Add(t);

                foreach (TypeMappingAttribute m in mappings)
                {
                    foreach (var dbType in m.DbTypes)
                    {
                        _npgsqlDbTypeToDbType[dbType] = m.NpgsqlDbType;
                    }
                }
            }
        }

        #endregion
    }
}
