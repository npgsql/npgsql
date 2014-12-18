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

namespace Npgsql
{
    internal class TypeHandlerRegistry
    {
        readonly Dictionary<int, TypeHandler> _oidIndex;

        static List<TypeHandler> _scalarTypeHandlers;
        static readonly TypeHandler UnknownTypeHandler = new UnknownTypeHandler();
        static readonly TypeHandlerRegistry EmptyRegistry = new TypeHandlerRegistry();
        static readonly ConcurrentDictionary<string, TypeHandlerRegistry> _registryCache = new ConcurrentDictionary<string, TypeHandlerRegistry>();
        static readonly ILog _log = LogManager.GetCurrentClassLogger();

        TypeHandlerRegistry()
        {
            _oidIndex = new Dictionary<int, TypeHandler>();
        }

        /// <summary>
        /// Looks up a type handler by its Postgresql type's OID.
        /// </summary>
        /// <param name="oid">A Postgresql type OID</param>
        /// <returns>A type handler that can be used to encode and decode values.</returns>
        internal TypeHandler this[int oid]
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

        static internal void Setup(NpgsqlConnector connector)
        {
            TypeHandlerRegistry registry;
            if (_registryCache.TryGetValue(connector.ConnectionString, out registry)) {
                connector.TypeHandlerRegistry = registry;
                return;
            }

            _log.Debug("Loading types for connection string: " + connector.ConnectionString);

            // Below we'll be sending in a query to load OIDs from the backend, but parsing those results will depend
            // on... the OIDs. To solve this chicken and egg problem, set up an empty type handler registry that will
            // enable us to at least read strings via the UnknownTypeHandler
            connector.TypeHandlerRegistry = EmptyRegistry;

            var result = new TypeHandlerRegistry();
            var inList = new StringBuilder();
            var nameIndex = new Dictionary<string, TypeHandler>();

            foreach (var handler in _scalarTypeHandlers)
            {
                foreach (var pgName in handler.PgNames)
                {
                    if (nameIndex.ContainsKey(pgName)) {
                        throw new Exception(String.Format("Two type handlers registered on same Postgresql type name: {0} and {1}", nameIndex[pgName].GetType().Name, handler.GetType().Name));
                    }
                    nameIndex.Add(pgName, handler);
                    inList.AppendFormat("{0}'{1}'", ((inList.Length > 0) ? ", " : ""), pgName);
                }
            }

            using (var command = new NpgsqlCommand(String.Format("SELECT typname, oid, typarray, typdelim FROM pg_type WHERE typname IN ({0})", inList), connector))
            {
                using (var dr = command.GetReader(CommandBehavior.SequentialAccess))
                {
                    while (dr.Read())
                    {
                        var handler = nameIndex[dr.GetString(0)];
                        var oid = Convert.ToInt32(dr.GetString(1));
                        Debug.Assert(handler.Oid == -1);
                        // TODO: Check if we actually need the OID here (we will for write).
                        // If so we need to create instances of handlers per-connector, since OIDs may differ
                        //handler.Oid = oid;
                        result[oid] = handler;

                        var arrayOid = Convert.ToInt32(dr[2]);
                        if (arrayOid != 0)
                        {
                            // The backend has a corresponding array type for this type.
                            // Use reflection to create a constructed type from the generic ArrayHandler<,,>
                            // generic type definition.
                            var arrayHandlerType = typeof(ArrayHandler<,,>).MakeGenericType(handler.FieldType, handler.ProviderSpecificFieldType, handler.GetType());
                            var textDelimiter = dr.GetString(3)[0];
                            var arrayHandler = (TypeHandler)Activator.CreateInstance(arrayHandlerType, handler, textDelimiter);
                            // arrayHandler.Oid = oid;
                            result[arrayOid] = arrayHandler;
                        }
                    }
                }
            }

            /*foreach (var notFound in _typeHandlers.Where(t => t.Oid == -1)) {
                _log.WarnFormat("Could not find type {0} in pg_type", notFound.PgNames[0]);
            }*/

            connector.TypeHandlerRegistry = _registryCache[connector.ConnectionString] = result;
        }

        static void DiscoverTypeHandlers()
        {
            _scalarTypeHandlers = Assembly.GetExecutingAssembly()
                .DefinedTypes
                .Where(t => !t.IsAbstract &&
                            t.IsSubclassOf(typeof (TypeHandler)) &&
                            //t != typeof(UnknownTypeHandler) &&
                            t != typeof(ArrayHandler<,,>))
                .Select(Activator.CreateInstance)
                .Cast<TypeHandler>()
                .ToList();
        }

        static TypeHandlerRegistry()
        {
            DiscoverTypeHandlers();
        }
    }
}
