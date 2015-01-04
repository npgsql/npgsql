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
        readonly Dictionary<uint, uint> _arrayOidToElementOid;
        readonly Dictionary<uint, uint> _rangeOidToElementOid;
        readonly Dictionary<NpgsqlDbType, uint> _npgsqlDbTypeToOid;
        readonly Dictionary<DbType, uint> _dbTypeToOid;
        readonly Dictionary<uint, NpgsqlDbType> _oidToNpgsqlDbType;
        readonly Dictionary<Type, uint> _typeToOid;

        static List<TypeHandler> _scalarTypeHandlers;
        static Dictionary<DbType, NpgsqlDbType> _dbTypeToNpgsqlDbType;
        static Dictionary<NpgsqlDbType, DbType> _npgsqlDbTypeToDbType;
        static Dictionary<Type, NpgsqlDbType> _typeToNpgsqlDbType;
        static readonly TypeHandler UnknownTypeHandler = new UnknownTypeHandler();
        static readonly TypeHandlerRegistry EmptyRegistry = new TypeHandlerRegistry();
        static readonly ConcurrentDictionary<string, TypeHandlerRegistry> _registryCache = new ConcurrentDictionary<string, TypeHandlerRegistry>();
        static readonly ILog Log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Initialization and Loading

        static internal void Setup(NpgsqlConnector connector)
        {
            TypeHandlerRegistry registry;
            if (_registryCache.TryGetValue(connector.ConnectionString, out registry))
            {
                connector.TypeHandlerRegistry = registry;
            }

            connector.TypeHandlerRegistry = _registryCache[connector.ConnectionString] = new TypeHandlerRegistry(connector);
        }

        TypeHandlerRegistry()
        {
            _oidIndex = new Dictionary<uint, TypeHandler>();
            _arrayOidToElementOid = new Dictionary<uint, uint>();
            _rangeOidToElementOid = new Dictionary<uint, uint>();
            _npgsqlDbTypeToOid = new Dictionary<NpgsqlDbType, uint>();
            _dbTypeToOid = new Dictionary<DbType, uint>();
            _oidToNpgsqlDbType = new Dictionary<uint, NpgsqlDbType>();
            _typeToOid = new Dictionary<Type, uint>();
        }

        TypeHandlerRegistry(NpgsqlConnector connector) : this()
        {
            Log.Debug("Loading types for connection string: " + connector.ConnectionString);

            // Below we'll be sending in a query to load OIDs from the backend, but parsing those results will depend
            // on... the OIDs. To solve this chicken and egg problem, set up an empty type handler registry that will
            // enable us to at least read strings via the UnknownTypeHandler
            connector.TypeHandlerRegistry = EmptyRegistry;

            var inList = new StringBuilder();
            _nameIndex = new Dictionary<string, TypeHandler>();

            foreach (var handler in _scalarTypeHandlers)
            {
                foreach (var pgName in handler.PgNames)
                {
                    if (_nameIndex.ContainsKey(pgName))
                    {
                        throw new Exception(String.Format("Two type handlers registered on same Postgresql type name: {0} and {1}", _nameIndex[pgName].GetType().Name, handler.GetType().Name));
                    }
                    _nameIndex.Add(pgName, handler);
                    inList.AppendFormat("{0}'{1}'", ((inList.Length > 0) ? "," : ""), pgName);
                }
            }

            const string query = @"SELECT typname, pg_type.oid, typtype, typarray, typdelim, rngsubtype " +
                                 @"FROM pg_type LEFT OUTER JOIN pg_range ON (pg_type.oid = pg_range.rngtypid)" +
                                 @"WHERE typname in ({0}) OR typtype = 'r' ORDER BY typtype";
            using (var command = new NpgsqlCommand(String.Format(query, inList), connector))
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

            UnknownOid = GetOidFromNpgsqlDbType(NpgsqlDbType.Unknown);
        }

        void RegisterBaseType(string name, uint oid, uint arrayOID, char textDelimiter)
        {
            var handler = _nameIndex[name];
            Contract.Assert(handler.Oid == 0);

            // TODO: Check if we actually need the OID here (we will for write).
            // If so we need to create instances of handlers per-connector, since OIDs may differ
            //handler.Oid = oid;
            _oidIndex[oid] = handler;

            NpgsqlDbType npgsqlDbType = 0;

            // Map NpgsqlDbType and DbType
            for (var i = 0; i < handler.PgNames.Length && i < handler.NpgsqlDbTypes.Length; i++)
            {
                if (handler.PgNames[i] == name && handler.NpgsqlDbTypes[i].HasValue)
                {
                    _npgsqlDbTypeToOid[handler.NpgsqlDbTypes[i].Value] = oid;
                    npgsqlDbType = handler.NpgsqlDbTypes[i].Value;
                    _oidToNpgsqlDbType[oid] = npgsqlDbType;

                    // Take the first occurance of the type
                    if (handler.AllowAutoInferring && !_typeToOid.ContainsKey(handler.GetFieldType()))
                    {
                        _typeToOid[handler.GetFieldType()] = oid;
                    }
                }
            }
            for (var i = 0; i < handler.PgNames.Length && i < handler.DbTypes.Length; i++)
            {
                if (handler.PgNames[i] == name && handler.DbTypes[i].HasValue)
                    _dbTypeToOid[handler.DbTypes[i].Value] = oid;
            }
            for (var i = 0; i < handler.PgNames.Length && i < handler.DbTypeAliases.Length; i++)
            {
                if (handler.PgNames[i] == name && handler.DbTypeAliases[i] != null)
                {
                    foreach (var dbType in handler.DbTypeAliases[i])
                        _dbTypeToOid[handler.DbTypes[i].Value] = oid;
                }
            }

            if (arrayOID == 0) {
                return;
            }

            // The backend has a corresponding array type for this type.
            // Use reflection to create a constructed type from the relevant ArrayHandler
            // generic type definition.
            var arrayHandler = CreateArrayHandler(handler, textDelimiter);

            _npgsqlDbTypeToOid[npgsqlDbType | NpgsqlDbType.Array] = arrayOID;
            _oidToNpgsqlDbType[arrayOID] = npgsqlDbType | NpgsqlDbType.Array;

            if (arrayHandler.AllowAutoInferring)
            {
                var elementFieldType = handler.GetFieldType();
                _typeToOid[elementFieldType.MakeArrayType()] = arrayOID;
                for (var i = 2; i <= ArrayHandler.MaxDimensions; i++)
                    _typeToOid[elementFieldType.MakeArrayType(i)] = arrayOID;
            }

            // arrayHandler.Oid = oid;
            _oidIndex[arrayOID] = arrayHandler;
            _arrayOidToElementOid[arrayOID] = oid;
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
            var rangeHandler = (TypeHandler)Activator.CreateInstance(rangeHandlerType, elementHandler, name);

            // Array of ranges
            var arrayRangeHandler = CreateArrayHandler(rangeHandler, ',');

            _npgsqlDbTypeToOid[NpgsqlDbType.Range | _oidToNpgsqlDbType[elementOid]] = oid;
            _npgsqlDbTypeToOid[NpgsqlDbType.Array | NpgsqlDbType.Range | _oidToNpgsqlDbType[elementOid]] = arrayOID;
            _oidToNpgsqlDbType[oid] = NpgsqlDbType.Range | _oidToNpgsqlDbType[elementOid];
            _oidToNpgsqlDbType[arrayOID] = NpgsqlDbType.Array | NpgsqlDbType.Range | _oidToNpgsqlDbType[elementOid];
            if (rangeHandler.AllowAutoInferring)
            {
                _typeToOid[rangeHandler.GetFieldType()] = oid;
                _typeToOid[arrayRangeHandler.GetFieldType(null)] = oid;
            }

            if (arrayRangeHandler.AllowAutoInferring)
            {
                var rangeFieldType = rangeHandler.GetFieldType();
                _typeToOid[rangeFieldType.MakeArrayType()] = arrayOID;
                for (var i = 2; i <= ArrayHandler.MaxDimensions; i++)
                    _typeToOid[rangeFieldType.MakeArrayType(i)] = arrayOID;
            }

            _oidIndex[oid] = rangeHandler;
            _rangeOidToElementOid[oid] = elementOid;
            _oidIndex[arrayOID] = arrayRangeHandler;
            _arrayOidToElementOid[arrayOID] = oid;
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
                return this[_npgsqlDbTypeToOid[npgsqlDbType]];
            }
        }

        internal TypeHandler this[DbType dbType]
        {
            get
            {
                uint oid = 0;
                if (!_dbTypeToOid.TryGetValue(dbType, out oid))
                {
                    throw new NotSupportedException("This DbType is not supported in Npgsql: " + dbType);
                }
                return this[oid];
            }
        }

        internal uint UnknownOid { get; private set; }

        internal uint GetOidFromNpgsqlDbType(NpgsqlDbType npgsqlDbType)
        {
            uint oid;
            if (_npgsqlDbTypeToOid.TryGetValue(npgsqlDbType, out oid))
                return oid;
            else
                throw new NotSupportedException("Your PostgreSQL version does not support the NpgsqlDbType \"" + npgsqlDbType + "\".");
        }

        internal uint GetElementOidFromArrayOid(uint arrayOid)
        {
            return _arrayOidToElementOid[arrayOid];
        }

        internal uint GetElementOidFromRangeOid(uint rangeOid)
        {
            return _rangeOidToElementOid[rangeOid];
        }

        internal NpgsqlDbType GetNpgsqlDbTypeFromOid(uint oid)
        {
            NpgsqlDbType npgsqlDbType;
            if (_oidToNpgsqlDbType.TryGetValue(oid, out npgsqlDbType))
                return npgsqlDbType;
            else
                return NpgsqlDbType.Unknown;
        }

        internal uint InferTypeOidFromValue(object value)
        {
            // First check the special cases and common types
            var npgsqlDbType = InferNpgsqlDbTypeFromValue(value);
            uint oid = 0;
            if (npgsqlDbType != NpgsqlDbType.Unknown && _npgsqlDbTypeToOid.TryGetValue(npgsqlDbType, out oid))
                return oid;

            // If db instance specific types are used, such as composite types or enum, this one will find it
            if (value != null && _typeToOid.TryGetValue(value.GetType(), out oid))
                return oid;

            return 0;
        }

        static internal NpgsqlDbType InferNpgsqlDbTypeFromValue(object value)
        {
            if (value == null || value is DBNull)
                return NpgsqlDbType.Unknown;

            if (value is DateTime) {
                if (((DateTime)value).Kind == DateTimeKind.Utc)
                    return NpgsqlDbType.TimestampTZ;
                else
                    return NpgsqlDbType.Timestamp;
            }

            if (value is string || value is char || value is char[])
                return NpgsqlDbType.Text;

            NpgsqlDbType type;
            if (_typeToNpgsqlDbType.TryGetValue(value.GetType(), out type))
                return type;

            return NpgsqlDbType.Unknown;
        }

        public static NpgsqlDbType GetNpgsqlDbTypeFromDbType(DbType dbType)
        {
            NpgsqlDbType t;
            if (!_dbTypeToNpgsqlDbType.TryGetValue(dbType, out t))
                throw new NotSupportedException("The DbType " + dbType + " is not supported for Npgsql");
            return t;
        }

        public static DbType GetDbTypeFromNpgsqlDbType(NpgsqlDbType npgsqlDbType)
        {
            DbType t;
            if (!_npgsqlDbTypeToDbType.TryGetValue(npgsqlDbType, out t))
                return DbType.Object;
            return t;
        }

        #endregion

        #region Static Initialization

        static TypeHandlerRegistry()
        {
            DiscoverTypeHandlers();
        }

        static void DiscoverTypeHandlers()
        {
            _scalarTypeHandlers = Assembly.GetExecutingAssembly()
                .DefinedTypes
                .Where(t => t.IsSubclassOf(typeof (TypeHandler)) &&
                            !t.IsAbstract &&
                            !typeof(ArrayHandler).IsAssignableFrom(t) &&  // Arrays are taken care of later
                            typeof(RangeHandler<>) != t
                )
                .Select(Activator.CreateInstance)
                .Cast<TypeHandler>()
                .ToList();

            _dbTypeToNpgsqlDbType = new Dictionary<DbType, NpgsqlDbType>();
            _npgsqlDbTypeToDbType = new Dictionary<NpgsqlDbType, DbType>();
            _typeToNpgsqlDbType = new Dictionary<Type, NpgsqlDbType>();

            // Set up mapping DbType <-> NpgsqlDbType. Used by NpgsqlParameter to be able to link between these.
            // Also set up a mapping .NET Type -> DbType/NpgsqlDbType
            foreach (var handler in _scalarTypeHandlers)
            {
                // Read backwards, so default types (the left-most in the lists) are updated last
                for (var i = Math.Min(handler.NpgsqlDbTypes.Length, handler.DbTypeAliases.Length) - 1; i >= 0; i--)
                {
                    if (handler.NpgsqlDbTypes[i].HasValue && handler.DbTypeAliases[i] != null)
                    {
                        var npgsqlDbType = handler.NpgsqlDbTypes[i].Value;
                        foreach (var dbType in handler.DbTypeAliases[i])
                        {
                            _dbTypeToNpgsqlDbType[dbType] = npgsqlDbType;
                        }
                    }
                }
                for (var i = Math.Min(handler.NpgsqlDbTypes.Length, handler.DbTypes.Length) - 1; i >= 0; i--)
                {
                    if (handler.NpgsqlDbTypes[i].HasValue && handler.DbTypes[i].HasValue)
                    {
                        var npgsqlDbType = handler.NpgsqlDbTypes[i].Value;
                        var dbType = handler.DbTypes[i].Value;
                        _dbTypeToNpgsqlDbType[dbType] = npgsqlDbType;
                        _npgsqlDbTypeToDbType[npgsqlDbType] = dbType;
                    }
                }
                if (handler.NpgsqlDbTypes.Length != 0 && handler.NpgsqlDbTypes[0].HasValue)
                {
                    // The default NpgsqlDbType is the first one in the list
                    var npgsqlDbType = handler.NpgsqlDbTypes[0].Value;

                    // Scalar
                    _typeToNpgsqlDbType[handler.GetFieldType()] = npgsqlDbType;

                    // Array
                    var elementFieldType = handler.GetFieldType();
                    _typeToNpgsqlDbType[elementFieldType.MakeArrayType()] = npgsqlDbType | NpgsqlDbType.Array;
                    for (var i = 2; i <= ArrayHandler.MaxDimensions; i++)
                        _typeToNpgsqlDbType[elementFieldType.MakeArrayType(i)] = npgsqlDbType | NpgsqlDbType.Array;

                    // Range
                    var rangeType = typeof(NpgsqlRange<>).MakeGenericType(handler.GetFieldType());
                    _typeToNpgsqlDbType[rangeType] = npgsqlDbType | NpgsqlDbType.Range;

                    // Array of range
                    _typeToNpgsqlDbType[rangeType.MakeArrayType()] = npgsqlDbType | NpgsqlDbType.Array | NpgsqlDbType.Range;
                    for (var i = 2; i <= ArrayHandler.MaxDimensions; i++)
                        _typeToNpgsqlDbType[rangeType.MakeArrayType(i)] = npgsqlDbType | NpgsqlDbType.Array | NpgsqlDbType.Range;
                }

            }
        }

        #endregion
    }
}
