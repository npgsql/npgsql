using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Npgsql.PostgresTypes
{
    class PostgresBaseType : PostgresType
    {
        [CanBeNull]
        readonly DbType[] _dbTypes;
        [CanBeNull]
        readonly Type[] _clrTypes;

        [CanBeNull]
        ConstructorInfo _ctorWithRegistry;
        [CanBeNull]
        ConstructorInfo _ctorWithoutRegistry;

        /// <summary>
        /// Constructs an unsupported base type (no handler exists in Npgsql for this type)
        /// </summary>
        internal PostgresBaseType(string ns, string name, uint oid) : base(ns, name, oid)
        {
            _dbTypes = new DbType[0];
            _clrTypes = new Type[0];
        }

        internal PostgresBaseType(string ns, string name, uint oid, Type handlerType, TypeMappingAttribute mapping)
            : base(ns, name, oid)
        {
            HandlerType = handlerType;
            NpgsqlDbType = mapping.NpgsqlDbType;
            _dbTypes = mapping.DbTypes;
            _clrTypes = mapping.ClrTypes;
        }

        internal override void AddTo(TypeHandlerRegistry.AvailablePostgresTypes types)
        {
            base.AddTo(types);

            if (_dbTypes != null)
                foreach (var dbType in _dbTypes)
                    types.ByDbType[dbType] = this;
            if (_clrTypes != null)
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
                if (registry.ByNpgsqlDbType.ContainsKey(value))
                    throw new Exception($"Two type handlers registered on same NpgsqlDbType {NpgsqlDbType}: {registry.ByNpgsqlDbType[value].GetType().Name} and {HandlerType.Name}");
                registry.ByNpgsqlDbType[NpgsqlDbType.Value] = handler;
            }

            if (_dbTypes != null)
            {
                foreach (var dbType in _dbTypes)
                {
                    if (registry.ByDbType.ContainsKey(dbType))
                        throw new Exception($"Two type handlers registered on same DbType {dbType}: {registry.ByDbType[dbType].GetType().Name} and {HandlerType.Name}");
                    registry.ByDbType[dbType] = handler;
                }
            }

            if (_clrTypes != null)
            {
                foreach (var type in _clrTypes)
                {
                    if (registry.ByType.ContainsKey(type))
                        throw new Exception($"Two type handlers registered on same .NET type {type}: {registry.ByType[type].GetType().Name} and {HandlerType.Name}");
                    registry.ByType[type] = handler;
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
                    where p.Length == 2 && p[0].ParameterType == typeof(PostgresType) && p[1].ParameterType == typeof(TypeHandlerRegistry)
                    select c
                ).FirstOrDefault();

                if (_ctorWithRegistry == null)
                {
                    _ctorWithoutRegistry = (
                        from c in ctors
                        let p = c.GetParameters()
                        where p.Length == 1 && p[0].ParameterType == typeof(PostgresType)
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
}
