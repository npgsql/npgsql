﻿#region License
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers
{
    /// <summary>
    /// Interface implemented by all concrete handlers which handle enums
    /// </summary>
    interface ICompositeHandler
    {
        /// <summary>
        /// The CLR type mapped to the PostgreSQL composite type.
        /// </summary>
        Type CompositeType { get; }
    }

    interface ICompositeHandlerFactory
    {
        ICompositeHandler Create(PostgresType backendType, List<RawCompositeField> rawFields, TypeHandlerRegistry registry);
    }

    /// <summary>
    /// Type handler for PostgreSQL composite types
    /// </summary>
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/rowtypes.html
    ///
    /// Encoding:
    /// A 32-bit integer with the number of columns, then for each column:
    /// * An OID indicating the type of the column
    /// * The length of the column(32-bit integer), or -1 if null
    /// * The column data encoded as binary
    /// </remarks>
    /// <typeparam name="T">the CLR type to map to the PostgreSQL composite type </typeparam>
    class CompositeHandler<T> : ChunkingTypeHandler<T>, ICompositeHandler where T : new()
    {
        readonly TypeHandlerRegistry _registry;
        readonly INpgsqlNameTranslator _nameTranslator;
        List<RawCompositeField> _rawFields;
        [CanBeNull]
        List<MemberDescriptor> _members;

        public Type CompositeType => typeof(T);

        internal CompositeHandler(PostgresType postgresType, INpgsqlNameTranslator nameTranslator, List<RawCompositeField> rawFields, TypeHandlerRegistry registry)
            : base (postgresType)
        {
            _nameTranslator = nameTranslator;

            // At this point the composite handler nows about the fields, but hasn't yet resolved the
            // type OIDs to their type handlers. This is done only very late upon first usage of the handler,
            // allowing composite types to be registered and activated in any order regardless of dependencies.
            _rawFields = rawFields;

            _registry = registry;
        }

        #region Read

        public override async ValueTask<T> Read(ReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null)
        {
            ResolveFieldsIfNeeded();
            Debug.Assert(_members != null);

            await buf.Ensure(4, async);
            var fieldCount = buf.ReadInt32();
            if (fieldCount != _members.Count)
            {
                // PostgreSQL sanity check
                throw new Exception($"pg_attributes contains {_members.Count} rows for type {PgDisplayName}, but {fieldCount} fields were received!");
            }

            // If T is a struct, we have to box it here to properly set its fields below
            object result = new T();

            foreach (var fieldDescriptor in _members)
            {
                await buf.Ensure(8, async);
                buf.ReadInt32();  // read typeOID, not used
                var fieldLen = buf.ReadInt32();
                if (fieldLen == -1)
                {
                    // Null field, simply skip it and leave at default
                    continue;
                }
                fieldDescriptor.SetValue(result, await fieldDescriptor.Handler.ReadAsObject(buf, fieldLen, async));
            }
            return (T)result;
        }

        #endregion

        #region Write

        public override int ValidateAndGetLength(object value, ref LengthCache lengthCache, NpgsqlParameter parameter)
        {
            ResolveFieldsIfNeeded();
            Debug.Assert(_members != null);

            if (lengthCache == null)
                lengthCache = new LengthCache(1);
            if (lengthCache.IsPopulated)
                return lengthCache.Get();

            // Leave empty slot for the entire composite type, and go ahead an populate the element slots
            var pos = lengthCache.Position;
            lengthCache.Set(0);
            var totalLen = 4;  // number of fields
            foreach (var f in _members)
            {
                totalLen += 4 + 4;  // type oid + field length
                var fieldValue = f.GetValue(value);
                if (fieldValue == null)
                    continue;
                totalLen += f.Handler.ValidateAndGetLength(fieldValue, ref lengthCache);
            }
            return lengthCache.Lengths[pos] = totalLen;
        }

        protected override async Task Write(object value, WriteBuffer buf, LengthCache lengthCache, NpgsqlParameter parameter,
            bool async, CancellationToken cancellationToken)
        {
            Debug.Assert(_members != null);

            var composite = (T)value;

            if (buf.WriteSpaceLeft < 4)
                await buf.Flush(async, cancellationToken);
            buf.WriteInt32(_members.Count);

            foreach (var fieldDescriptor in _members)
            {
                var fieldHandler = fieldDescriptor.Handler;
                var fieldValue = fieldDescriptor.GetValue(composite);

                if (buf.WriteSpaceLeft < 4)
                    await buf.Flush(async, cancellationToken);

                buf.WriteUInt32(fieldDescriptor.OID);
                await fieldHandler.WriteWithLength(fieldValue, buf, lengthCache, null, async, cancellationToken);
            }
        }

        #endregion

        #region Misc

        void ResolveFieldsIfNeeded()
        {
            if (_members != null)
                return;

            _members = new List<MemberDescriptor>(_rawFields.Count);
            foreach (var rawField in _rawFields)
            {
                if (!_registry.TryGetByOID(rawField.TypeOID, out var handler))
                    throw new Exception($"PostgreSQL composite type {PgDisplayName}, mapped to CLR type {typeof(T).Name}, has field {rawField.PgName} with an unknown type (TypeOID={rawField.TypeOID})");

                var member = (
                    from m in typeof(T).GetMembers()
                    let attr = m.GetCustomAttribute<PgNameAttribute>()
                    where (attr != null && attr.PgName == rawField.PgName) ||
                          (attr == null && _nameTranslator.TranslateMemberName(m.Name) == rawField.PgName)
                    select m
                ).SingleOrDefault();

                if (member == null)
                    throw new Exception($"PostgreSQL composite type {PgDisplayName} contains field {rawField.PgName} which could not match any on CLR type {typeof(T).Name}");

                var property = member as PropertyInfo;
                if (property != null)
                {
                    _members.Add(new MemberDescriptor(rawField.PgName, rawField.TypeOID, handler, property));
                    continue;
                }

                var field = member as FieldInfo;
                if (field != null)
                {
                    _members.Add(new MemberDescriptor(rawField.PgName, rawField.TypeOID, handler, field));
                    continue;
                }

                throw new Exception($"PostgreSQL composite type {PgDisplayName} contains field {rawField.PgName} which cannot map to CLR type {typeof(T).Name}'s field {member.Name} of type {member.GetType().Name}");
            }

            _rawFields = null;
        }

        struct MemberDescriptor
        {
            // ReSharper disable once NotAccessedField.Local
            // ReSharper disable once MemberCanBePrivate.Local
            internal readonly string PgName;
            internal readonly uint OID;
            internal readonly TypeHandler Handler;
            [CanBeNull]
            readonly PropertyInfo _property;
            [CanBeNull]
            readonly FieldInfo _field;

            internal MemberDescriptor(string pgName, uint oid, TypeHandler handler, PropertyInfo property)
            {
                PgName = pgName;
                OID = oid;
                Handler = handler;
                _property = property;
                _field = null;
            }

            internal MemberDescriptor(string pgName, uint oid, TypeHandler handler, FieldInfo field)
            {
                PgName = pgName;
                OID = oid;
                Handler = handler;
                _property = null;
                _field = field;
            }

            internal void SetValue(object container, [CanBeNull] object fieldValue)
            {
                if (_property != null)
                    _property.SetValue(container, fieldValue);
                else if (_field != null)
                    _field.SetValue(container, fieldValue);
                else throw new InvalidOperationException("Internal Npgsql bug, please report.");
            }

            [CanBeNull]
            internal object GetValue(object container)
            {
                if (_property != null)
                    return _property.GetValue(container);
                if (_field != null)
                    return _field.GetValue(container);
                throw new InvalidOperationException("Internal Npgsql bug, please report.");
            }
        }

        #endregion

        internal class Factory : ICompositeHandlerFactory
        {
            readonly INpgsqlNameTranslator _nameTranslator;

            internal Factory(INpgsqlNameTranslator nameTranslator)
            {
                _nameTranslator = nameTranslator;
            }

            public ICompositeHandler Create(PostgresType backendType, List<RawCompositeField> rawFields, TypeHandlerRegistry registry)
                => new CompositeHandler<T>(backendType, _nameTranslator, rawFields, registry);
        }
    }

    struct RawCompositeField
    {
        internal string PgName;
        internal uint TypeOID;

        public override string ToString() => $"{PgName} => {TypeOID}";
    }
}
