#region License
// The PostgreSQL License
//
// Copyright (C) 2017 The Npgsql Development Team
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
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace Npgsql.PostgresTypes
{
    /// <summary>
    /// Represents a PostgreSQL base data type, which is a simple scalar value.
    /// </summary>
    public class PostgresBaseType : PostgresType
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
        /// Constructs a representation of a PostgreSQL base data type.
        /// </summary>
#pragma warning disable CA2222 // Do not decrease inherited member visibility
        internal PostgresBaseType(string ns, string name, uint oid, Type handlerType, TypeMappingAttribute mapping)
#pragma warning restore CA2222 // Do not decrease inherited member visibility
            : base(ns, name, oid)
        {
            HandlerType = handlerType;
            NpgsqlDbType = mapping.NpgsqlDbType;
            _dbTypes = mapping.DbTypes;
            _clrTypes = mapping.ClrTypes;
        }

        /// <summary>
        /// Constructs an unsupported base type (no handler exists in Npgsql for this type)
        /// </summary>
        protected internal PostgresBaseType(string ns, string name, uint oid) : base(ns, name, oid)
        {
            _dbTypes = new DbType[0];
            _clrTypes = new Type[0];
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
