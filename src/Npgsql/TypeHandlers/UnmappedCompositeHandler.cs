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
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;
#if !NETSTANDARD1_3
using System.Dynamic;
#endif

namespace Npgsql.TypeHandlers
{
    /// <summary>
    /// Type handler for PostgreSQL composite types, mapping them to C# dynamic.
    /// This is the default handler used for composites.
    /// </summary>
    /// <seealso cref="MappedCompositeHandler{T}"/>.
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/rowtypes.html
    ///
    /// Encoding:
    /// A 32-bit integer with the number of columns, then for each column:
    /// * An OID indicating the type of the column
    /// * The length of the column(32-bit integer), or -1 if null
    /// * The column data encoded as binary
    /// </remarks>
    class UnmappedCompositeHandler : NpgsqlTypeHandler<object>
    {
        readonly ConnectorTypeMapper _typeMapper;
        readonly INpgsqlNameTranslator _nameTranslator;

        [CanBeNull]
        List<MemberDescriptor> _members;

        [CanBeNull]
        Type _resolvedType;

        internal UnmappedCompositeHandler(INpgsqlNameTranslator nameTranslator, ConnectorTypeMapper typeMapper)
        {
            _nameTranslator = nameTranslator;

            // After construction the composite handler will have a reference to its PostgresCompositeType,
            // which contains information about the fields. But the actual binding of their type OIDs
            // to their type handlers is done only very late upon first usage of the handler,
            // allowing composite types to be activated in any order regardless of dependencies.

            _typeMapper = typeMapper;
        }

        #region Read

        protected internal override async ValueTask<TAny> Read<TAny>(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null)
        {
            if (_resolvedType != typeof(TAny))
                Map(typeof(TAny));

            await buf.Ensure(4, async);
            var fieldCount = buf.ReadInt32();
            if (fieldCount != _members.Count)  // PostgreSQL sanity check
                throw new Exception($"pg_attributes contains {_members.Count} rows for type {PgDisplayName}, but {fieldCount} fields were received!");

            // If TAny is a struct, we have to box it here to properly set its fields below
            object result = Activator.CreateInstance<TAny>();
            foreach (var member in _members)
            {
                await buf.Ensure(8, async);
                buf.ReadInt32();  // read typeOID, not used
                var fieldLen = buf.ReadInt32();
                if (fieldLen == -1)
                    continue;  // Null field, simply skip it and leave at default
                member.Setter(result, await member.Handler.ReadAsObject(buf, fieldLen, async));
            }
            return (TAny)result;
        }

        internal override ValueTask<object> ReadAsObject(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null)
            => Read(buf, len, async, fieldDescription);

        internal override object ReadAsObject(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
            => Read(buf, len, false, fieldDescription).Result;

#pragma warning disable CS1998 // Needless async (for netstandard1.3)
        public override async ValueTask<object> Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null)
#pragma warning restore CS1998 // Needless async (for netstandard1.3)
        {
#if NETSTANDARD1_3
            throw new NotSupportedException("Not support in .NET Standard 1.3");
#else
            if (_members == null)
                ResolveFields();
            Debug.Assert(_members != null);

            await buf.Ensure(4, async);
            var fieldCount = buf.ReadInt32();
            if (fieldCount != _members.Count)  // PostgreSQL sanity check
                throw new Exception($"pg_attributes contains {_members.Count} rows for type {PgDisplayName}, but {fieldCount} fields were received!");

            var result = (IDictionary<string, object>)new ExpandoObject();

            foreach (var member in _members)
            {
                await buf.Ensure(8, async);
                buf.ReadInt32();  // read typeOID, not used
                var fieldLen = buf.ReadInt32();
                if (fieldLen == -1)
                {
                    // Null field, simply skip it and leave at default
                    continue;
                }
                // TODO: We need name translation
                result[member.PgName] = await member.Handler.ReadAsObject(buf, fieldLen, async);
            }
            return result;
#endif
        }

        #endregion

        #region Write

        protected internal override int ValidateObjectAndGetLength(object value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => value == null || value is DBNull
                ? -1
                : ValidateAndGetLength(value, ref lengthCache, parameter);

        protected internal override int ValidateAndGetLength<TAny>(TAny value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => ValidateAndGetLength(value, ref lengthCache, parameter);

        public override int ValidateAndGetLength(object value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
        {
            var type = value.GetType();
            if (_resolvedType != type)
            {
                if (value is IDictionary<string, object> asDict)
                    MapDynamic(asDict);
                else
                    Map(type);
            }
            Debug.Assert(_members != null);

            if (lengthCache == null)
                lengthCache = new NpgsqlLengthCache(1);
            if (lengthCache.IsPopulated)
                return lengthCache.Get();

            // Leave empty slot for the entire composite type, and go ahead an populate the element slots
            var pos = lengthCache.Position;
            lengthCache.Set(0);
            var totalLen = 4;  // number of fields
            foreach (var f in _members)
            {
                totalLen += 4 + 4;  // type oid + field length
                var fieldValue = f.Getter(value);
                if (fieldValue == null)
                    continue;
                totalLen += f.Handler.ValidateObjectAndGetLength(fieldValue, ref lengthCache, null);
            }
            return lengthCache.Lengths[pos] = totalLen;
        }

        protected internal override Task WriteObjectWithLength(object value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => value == null || value is DBNull
                ? WriteWithLengthInternal<DBNull>(null, buf, lengthCache, parameter, async)
                : WriteWithLengthInternal(value, buf, lengthCache, parameter, async);

        protected override Task WriteWithLength<T2>(T2 value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
        {
            buf.WriteInt32(ValidateAndGetLength(value, ref lengthCache, parameter));
            return Write(value, buf, lengthCache, parameter, async);
        }

        public override async Task Write(object value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
        {
            Debug.Assert(_resolvedType != null);
            Debug.Assert(_members != null);

            if (buf.WriteSpaceLeft < 4)
                await buf.Flush(async);
            buf.WriteInt32(_members.Count);

            foreach (var fieldDescriptor in _members)
            {
                var fieldHandler = fieldDescriptor.Handler;
                var fieldValue = fieldDescriptor.Getter(value);

                if (buf.WriteSpaceLeft < 4)
                    await buf.Flush(async);

                buf.WriteUInt32(fieldDescriptor.OID);
                await fieldHandler.WriteObjectWithLength(fieldValue, buf, lengthCache, null, async);
            }
        }

        #endregion

        #region Misc

        void ResolveFields()
        {
            Debug.Assert(_members == null);
            Debug.Assert(PostgresType is PostgresCompositeType, "CompositeHandler initialized with a non-composite type");

            var rawFields = ((PostgresCompositeType)PostgresType).Fields;
            _members = new List<MemberDescriptor>(rawFields.Count);
            foreach (var rawField in rawFields)
            {
                var member = new MemberDescriptor { PgName = rawField.Name, OID = rawField.Type.OID };
                if (!_typeMapper.TryGetByOID(rawField.Type.OID, out member.Handler))
                    throw new Exception($"PostgreSQL composite type {PgDisplayName} has field {rawField.Name} with an unknown type (TypeOID={rawField.Type.OID})");
                _members.Add(member);
            }
        }

        void Map(Type type)
        {
            Debug.Assert(_resolvedType != type);
            if (_members == null)
                ResolveFields();
            Debug.Assert(_members != null);

            foreach (var member in _members)
            {
                var typeMember = (
                    from m in type.GetMembers()
                    let attr = m.GetCustomAttribute<PgNameAttribute>()
                    where attr != null && attr.PgName == member.PgName ||
                          attr == null && _nameTranslator.TranslateMemberName(m.Name) == member.PgName
                    select m
                ).SingleOrDefault();

                if (typeMember == null)
                    throw new Exception($"PostgreSQL composite type {PgDisplayName} contains field {member.PgName} which could not match any on CLR type {type.Name}");

                switch (typeMember)
                {
                case PropertyInfo p:
                    member.Getter = composite => p.GetValue(composite);
                    member.Setter = (composite, v) => p.SetValue(composite, v);
                    break;
                case FieldInfo f:
                    member.Getter = composite => f.GetValue(composite);
                    member.Setter = (composite, v) => f.SetValue(composite, v);
                    break;
                default:
                    throw new Exception($"PostgreSQL composite type {PgDisplayName} contains field {member.PgName} which cannot map to CLR type {type.Name}'s field {typeMember.Name} of type {member.GetType().Name}");
                }
            }

            _resolvedType = type;
        }

        void MapDynamic(IDictionary<string, object> dict)
        {
            Debug.Assert(_resolvedType != typeof(object));
            if (_members == null)
                ResolveFields();
            Debug.Assert(_members != null);

            foreach (var member in _members)
            {
                var translatedName = dict.Keys.SingleOrDefault(k => _nameTranslator.TranslateMemberName(k) == member.PgName);
                if (translatedName == null)
                    throw new Exception($"PostgreSQL composite type {PgDisplayName} contains field {member.PgName} which could not match any on provided dictionary");
                member.Getter = composite => ((IDictionary<string, object>)composite)[translatedName];
            }

            _resolvedType = dict.GetType();
        }

        delegate object MemberValueGetter(object composite);
        delegate void MemberValueSetter(object composite, object value);

        class MemberDescriptor
        {
            // ReSharper disable once NotAccessedField.Local
            // ReSharper disable once MemberCanBePrivate.Local
            internal string PgName;
            internal uint OID;
            internal NpgsqlTypeHandler Handler;
            internal MemberValueGetter Getter;
            internal MemberValueSetter Setter;
        }

        #endregion
    }

#pragma warning disable CA1040    // Avoid empty interfaces
    interface IDynamicCompositeTypeHandlerFactory { }
#pragma warning restore CA1040    // Avoid empty interfaces

    class UnmappedCompositeTypeHandlerFactory : NpgsqlTypeHandlerFactory<object>
    {
        readonly INpgsqlNameTranslator _nameTranslator;

        internal UnmappedCompositeTypeHandlerFactory(INpgsqlNameTranslator nameTranslator)
        {
            _nameTranslator = nameTranslator;
        }

        protected override NpgsqlTypeHandler<object> Create(NpgsqlConnection conn)
            => new UnmappedCompositeHandler(_nameTranslator, conn.Connector.TypeMapper);
    }
}
