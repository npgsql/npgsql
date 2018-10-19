#region License

// The PostgreSQL License
//
// Copyright (C) 2018 The Npgsql Development Team
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
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading;
using Npgsql.NameTranslation;
using Npgsql.TypeHandling;
using NpgsqlTypes;

namespace Npgsql.TypeMapping
{
    class GlobalTypeMapper : TypeMapperBase
    {
        public static GlobalTypeMapper Instance { get; }

        /// <summary>
        /// A counter that is incremented whenever a global mapping change occurs.
        /// Used to invalidate bound type mappers.
        /// </summary>
        internal int ChangeCounter => _changeCounter;

        internal ReaderWriterLockSlim Lock { get; }
            = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        int _changeCounter;

        static GlobalTypeMapper()
        {
            var instance = new GlobalTypeMapper();
            instance.SetupGlobalTypeMapper();
            Instance = instance;
        }

        internal GlobalTypeMapper() : base(new NpgsqlSnakeCaseNameTranslator())
            => Mappings = new Dictionary<string, NpgsqlTypeMapping>();

        #region Mapping management

        public override INpgsqlTypeMapper AddMapping(NpgsqlTypeMapping mapping)
        {
            Lock.EnterWriteLock();
            try
            {
                base.AddMapping(mapping);
                RecordChange();

                if (mapping.NpgsqlDbType.HasValue)
                {
                    foreach (var dbType in mapping.DbTypes)
                        _dbTypeToNpgsqlDbType[dbType] = mapping.NpgsqlDbType.Value;

                    if (mapping.InferredDbType.HasValue)
                        _npgsqlDbTypeToDbType[mapping.NpgsqlDbType.Value] = mapping.InferredDbType.Value;

                    foreach (var clrType in mapping.ClrTypes)
                        _typeToNpgsqlDbType[clrType] = mapping.NpgsqlDbType.Value;
                }

                if (mapping.InferredDbType.HasValue)
                    foreach (var clrType in mapping.ClrTypes)
                        _typeToDbType[clrType] = mapping.InferredDbType.Value;

                return this;
            }
            finally
            {
                Lock.ExitWriteLock();
            }
        }

        public override bool RemoveMapping(string pgTypeName)
        {
            Lock.EnterWriteLock();
            try
            {
                var result = base.RemoveMapping(pgTypeName);
                RecordChange();
                return result;
            }
            finally
            {
                Lock.ExitWriteLock();
            }
        }

        public override void Reset()
        {
            Lock.EnterWriteLock();
            try
            {
                Mappings.Clear();
                SetupGlobalTypeMapper();
                RecordChange();
            }
            finally
            {
                Lock.ExitWriteLock();
            }
        }

        internal void RecordChange() => Interlocked.Increment(ref _changeCounter);

        #endregion Mapping management

        #region NpgsqlDbType/DbType inference for NpgsqlParameter

        readonly Dictionary<NpgsqlDbType, DbType> _npgsqlDbTypeToDbType = new Dictionary<NpgsqlDbType, DbType>();
        readonly Dictionary<DbType, NpgsqlDbType> _dbTypeToNpgsqlDbType = new Dictionary<DbType, NpgsqlDbType>();
        readonly Dictionary<Type, NpgsqlDbType> _typeToNpgsqlDbType = new Dictionary<Type, NpgsqlDbType>();
        readonly Dictionary<Type, DbType> _typeToDbType = new Dictionary<Type, DbType>();

        internal DbType ToDbType(NpgsqlDbType npgsqlDbType)
            => _npgsqlDbTypeToDbType.TryGetValue(npgsqlDbType, out var dbType) ? dbType : DbType.Object;

        internal NpgsqlDbType ToNpgsqlDbType(DbType dbType)
        {
            if (!_dbTypeToNpgsqlDbType.TryGetValue(dbType, out var npgsqlDbType))
                throw new NotSupportedException($"The parameter type DbType.{dbType} isn't supported by PostgreSQL or Npgsql");
            return npgsqlDbType;
        }

        internal DbType ToDbType(Type type)
            => _typeToDbType.TryGetValue(type, out var dbType) ? dbType : DbType.Object;

        internal NpgsqlDbType ToNpgsqlDbType(Type type)
        {
            if (_typeToNpgsqlDbType.TryGetValue(type, out var npgsqlDbType))
                return npgsqlDbType;

            if (type.IsArray)
            {
                if (type == typeof(byte[]))
                    return NpgsqlDbType.Bytea;
                return NpgsqlDbType.Array | ToNpgsqlDbType(type.GetElementType());
            }

            var typeInfo = type.GetTypeInfo();

            var ilist = typeInfo.ImplementedInterfaces.FirstOrDefault(x => x.GetTypeInfo().IsGenericType && x.GetGenericTypeDefinition() == typeof(IList<>));
            if (ilist != null)
                return NpgsqlDbType.Array | ToNpgsqlDbType(ilist.GetGenericArguments()[0]);

            if (typeInfo.IsGenericType && type.GetGenericTypeDefinition() == typeof(NpgsqlRange<>))
                return NpgsqlDbType.Range | ToNpgsqlDbType(type.GetGenericArguments()[0]);

            if (type == typeof(DBNull))
                return NpgsqlDbType.Unknown;

            throw new NotSupportedException("Can't infer NpgsqlDbType for type " + type);
        }


        #endregion NpgsqlDbType/DbType inference for NpgsqlParameter

        #region Setup for built-in handlers

        void SetupGlobalTypeMapper()
        {
            // Look for TypeHandlerFactories with mappings in our assembly, set them up
            foreach (var t in typeof(TypeMapperBase).GetTypeInfo().Assembly.GetTypes().Where(t => t.GetTypeInfo().IsSubclassOf(typeof(NpgsqlTypeHandlerFactory))))
            {
                var mappingAttributes = t.GetTypeInfo().GetCustomAttributes(typeof(TypeMappingAttribute), false);
                if (!mappingAttributes.Any())
                    continue;

                var factory = (NpgsqlTypeHandlerFactory)Activator.CreateInstance(t);

                foreach (TypeMappingAttribute m in mappingAttributes)
                {
                    // TODO: Duplication between TypeMappingAttribute and TypeMapping. Look at this later.
                    AddMapping(new NpgsqlTypeMappingBuilder
                    {
                        PgTypeName = m.PgName,
                        NpgsqlDbType = m.NpgsqlDbType,
                        DbTypes = m.DbTypes,
                        ClrTypes = m.ClrTypes,
                        InferredDbType = m.InferredDbType,
                        TypeHandlerFactory = factory,
                    }.Build());
                }
            }

            // Look for NpgsqlTypeHandler classes with mappings in our assembly, set them up with the DefaultTypeHandlerFactory.
            // This is a shortcut that allows us to not specify a factory for each and every type handler
            foreach (var t in typeof(TypeMapperBase).GetTypeInfo().Assembly.GetTypes().Where(t => t.GetTypeInfo().IsSubclassOf(typeof(NpgsqlTypeHandler))))
            {
                var mappingAttributes = t.GetTypeInfo().GetCustomAttributes(typeof(TypeMappingAttribute), false);
                if (!mappingAttributes.Any())
                    continue;

                var factory = new DefaultTypeHandlerFactory(t);

                foreach (TypeMappingAttribute m in mappingAttributes)
                {
                    // TODO: Duplication between TypeMappingAttribute and TypeMapping. Look at this later.
                    AddMapping(new NpgsqlTypeMappingBuilder
                    {
                        PgTypeName = m.PgName,
                        NpgsqlDbType = m.NpgsqlDbType,
                        DbTypes = m.DbTypes,
                        ClrTypes = m.ClrTypes,
                        InferredDbType = m.InferredDbType,
                        TypeHandlerFactory = factory
                    }.Build());
                }
            }
        }

        #endregion Setup for built-in handlers
    }
}
