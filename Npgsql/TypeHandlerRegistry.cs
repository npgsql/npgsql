﻿using System;
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
        readonly Dictionary<Type, TypeHandler> _byEnumTypeAsArray;

        readonly TypeHandler _unrecognizedTypeHandler;

        static internal readonly List<Type> HandlerTypes;
        static readonly Dictionary<NpgsqlDbType, DbType> NpgsqlDbTypeToDbType;
        static readonly Dictionary<DbType, NpgsqlDbType> DbTypeToNpgsqlDbType;
        static readonly Dictionary<Type, NpgsqlDbType> TypeToNpgsqlDbType;
        static readonly Dictionary<Type, DbType> TypeToDbType;
        static readonly ConcurrentDictionary<string, TypeHandlerRegistry> RegistryCache = new ConcurrentDictionary<string, TypeHandlerRegistry>();
        static readonly ILog Log = LogManager.GetCurrentClassLogger();

        static readonly Dictionary<Type, EnumInfo> EnumTypeToEnumInfo;
        static readonly Dictionary<string, EnumInfo> EnumNameToEnumInfo;

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
            _byEnumTypeAsArray = new Dictionary<Type, TypeHandler>();
            _unrecognizedTypeHandler = new UnrecognizedTypeHandler { OID=0 };

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

                    if (attr.NpgsqlDbType.HasValue)
                    {
                        var npgsqlDbType = attr.NpgsqlDbType.Value;
                        if (_byNpgsqlDbType.ContainsKey(npgsqlDbType))
                            throw new Exception(String.Format("Two type handlers registered on same NpgsqlDbType {0}: {1} and {2}",
                                                attr.NpgsqlDbType, _byNpgsqlDbType[npgsqlDbType].GetType().Name, handlerType.Name));
                        _byNpgsqlDbType[npgsqlDbType] = handler;
                        handler.NpgsqlDbType = npgsqlDbType;
                    }

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

            foreach (var enumName in EnumNameToEnumInfo.Keys)
            {
                inList.AppendFormat("{0}'{1}'", ((inList.Length > 0) ? "," : ""), enumName);
            }

            const string query = @"SELECT typname, pg_type.oid, typtype, typarray, typdelim, rngsubtype " +
                                 @"FROM pg_type LEFT OUTER JOIN pg_range ON (pg_type.oid = pg_range.rngtypid) " +
                                 @"WHERE typname in ({0}) OR typtype = 'r' ORDER BY typtype";
            var parameterizedQuery = String.Format(query, inList);
            using (var command = new NpgsqlCommand(parameterizedQuery, connector))
            {
                command.AllResultTypesAreUnknown = true;
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
                            case 'e':
                                RegisterEnumType(name, oid, arrayOID);
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

            connector.TypeHandlerRegistry = RegistryCache[connector.ConnectionString] = this;
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

        void RegisterEnumType(string name, uint oid, uint arrayOID)
        {
            var enumInfo = EnumNameToEnumInfo[name];
            var enumHandlerType = typeof(EnumHandler<>).MakeGenericType(enumInfo.Type);
            var handler = (TypeHandler)Activator.CreateInstance(enumHandlerType, enumInfo);
            handler.OID = oid;
            _oidIndex[oid] = handler;
            _byType[enumInfo.Type] = handler;

            var arrayHandler = CreateArrayHandler(handler, ',');
            arrayHandler.OID = arrayOID;
            _oidIndex[arrayOID] = arrayHandler;
            _byEnumTypeAsArray[enumInfo.Type] = arrayHandler;
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

            arrayHandler.PgName = "array";
            return arrayHandler;
        }

        internal static void AddEnumType(Type enumType, string typeName)
        {
            EnumInfo existingEnumType;
            EnumInfo existingTypeName;
            EnumTypeToEnumInfo.TryGetValue(enumType, out existingEnumType);
            EnumNameToEnumInfo.TryGetValue(typeName, out existingTypeName);

            if (existingEnumType != null && existingEnumType != existingTypeName)
                throw new ArgumentException("Enum type already registered with another name");
            if (existingTypeName != null && existingEnumType != existingTypeName)
                throw new ArgumentException("Enum name is already registered with another enum type");

            var enumToLabel = new Dictionary<Enum, string>();
            var labelToEnum = new Dictionary<string, Enum>();
            var fields = enumType.GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (var field in fields)
            {
                var attribute = (EnumLabelAttribute)field.GetCustomAttributes(typeof(NpgsqlTypes.EnumLabelAttribute)).FirstOrDefault();
                var enumName = attribute == null ? field.Name : attribute.Label;
                var enumValue = (Enum)field.GetValue(null);
                enumToLabel[enumValue] = enumName;
                labelToEnum[enumName] = enumValue;
            }

            var enumInfo = new EnumInfo(enumType, typeName, enumToLabel, labelToEnum);
            EnumTypeToEnumInfo[enumType] = enumInfo;
            EnumNameToEnumInfo[typeName] = enumInfo;
            TypeToNpgsqlDbType[enumType] = NpgsqlDbType.Enum;
        }

        internal class EnumInfo
        {
            public Type Type { get; private set; }
            public string Name { get; private set; }
            Dictionary<Enum, string> _enumToLabel;
            Dictionary<string, Enum> _labelToEnum;

            public EnumInfo(Type type, string name, Dictionary<Enum, string> enumToLabel, Dictionary<string, Enum> labelToEnum)
            {
                Type = type;
                Name = name;
                _enumToLabel = enumToLabel;
                _labelToEnum = labelToEnum;
            }

            public string this[Enum enumValue]
            {
                get
                {
                    string value;
                    if (!_enumToLabel.TryGetValue(enumValue, out value))
                        return null;
                    return value;
                }
            }

            public Enum this[string stringValue]
            {
                get
                {
                    Enum value;
                    if (!_labelToEnum.TryGetValue(stringValue, out value))
                        return null;
                    return value;
                }
            }
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
                    result = _unrecognizedTypeHandler;
                }
                return result;
            }
            set { _oidIndex[oid] = value; }
        }

        internal TypeHandler this[NpgsqlDbType npgsqlDbType, Type enumType = null]
        {
            get
            {
                TypeHandler handler;
                var exists = _byNpgsqlDbType.TryGetValue(npgsqlDbType, out handler);
                if (!exists && enumType != null && (npgsqlDbType & ~NpgsqlDbType.Array) == NpgsqlDbType.Enum)
                {
                    if ((npgsqlDbType & NpgsqlDbType.Array) == NpgsqlDbType.Array)
                        exists = _byEnumTypeAsArray.TryGetValue(enumType, out handler);
                    else
                        exists = _byType.TryGetValue(enumType, out handler);
                    if (!exists)
                    {
                        throw new NotSupportedException("This enum type is not supported. (Have you registered it in Npsql and set the EnumType property of NpgsqlParameter?)");
                    }
                }
                if (!exists)
                {
                    throw new NotSupportedException("This NpgsqlDbType is not supported in Npgsql: " + npgsqlDbType);
                }
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
                    return _unrecognizedTypeHandler;

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
                if (type.IsArray && type != typeof(byte[])) {
                    var arrayHandler = this[NpgsqlDbType.Array | this[type.GetElementType()].NpgsqlDbType, type.GetElementType()];
                    if (arrayHandler != null)
                        return arrayHandler;
                    // if not found fallthrough to the error message below
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
            return DbTypeToNpgsqlDbType[dbType];
        }

        internal static NpgsqlDbType ToNpgsqlDbType(Type type)
        {
            if (type.IsArray) {
                return NpgsqlDbType.Array | ToNpgsqlDbType(type.GetElementType());
            }
            if (type.IsEnum) {
                return NpgsqlDbType.Enum;
            }
            return TypeToNpgsqlDbType[type];
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

        #region Type Discovery

        static TypeHandlerRegistry()
        {
            HandlerTypes = new List<Type>();
            NpgsqlDbTypeToDbType = new Dictionary<NpgsqlDbType, DbType>();
            DbTypeToNpgsqlDbType = new Dictionary<DbType, NpgsqlDbType>();
            TypeToNpgsqlDbType = new Dictionary<Type, NpgsqlDbType>();
            TypeToDbType = new Dictionary<Type, DbType>();

            EnumTypeToEnumInfo = new Dictionary<Type, EnumInfo>();
            EnumNameToEnumInfo = new Dictionary<string, EnumInfo>();

            foreach (var t in Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(TypeHandler))))
            {
                var mappings = t.GetCustomAttributes(typeof(TypeMappingAttribute), false);
                if (!mappings.Any())
                    continue;
                HandlerTypes.Add(t);

                foreach (TypeMappingAttribute m in mappings)
                {
                    if (!m.NpgsqlDbType.HasValue) {
                        continue;
                    }
                    var npgsqlDbType = m.NpgsqlDbType.Value;

                    if (m.DbTypes.Any())
                    {
                        NpgsqlDbTypeToDbType[npgsqlDbType] = m.DbTypes.FirstOrDefault();
                    }
                    foreach (var dbType in m.DbTypes)
                    {
                        DbTypeToNpgsqlDbType[dbType] = npgsqlDbType;
                    }
                    foreach (var type in m.Types)
                    {
                        TypeToNpgsqlDbType[type] = npgsqlDbType;
                        TypeToDbType[type] = m.DbTypes.FirstOrDefault();
                    }
                }
            }
        }

        #endregion
    }
}
