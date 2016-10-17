#region License
// The PostgreSQL License
//
// Copyright (C) 2016 The Npgsql Development Team
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
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Npgsql.BackendMessages;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers
{
    /// <summary>
    /// Interface implemented by all concrete handlers which handle enums
    /// </summary>
    internal interface ICompositeHandler
    {
        /// <summary>
        /// The CLR type mapped to the PostgreSQL composite type.
        /// </summary>
        Type CompositeType { get; }
        List<RawCompositeField> RawFields { get; set; }
    }

    interface ICompositeHandlerFactory
    {
        ICompositeHandler Create(IBackendType backendType, TypeHandlerRegistry registry);
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
    internal class CompositeHandler<T> : ChunkingTypeHandler<T>, ICompositeHandler where T : new()
    {
        readonly TypeHandlerRegistry _registry;
        readonly INpgsqlNameTranslator _nameTranslator;
        public List<RawCompositeField> RawFields { get; set; }
        [CanBeNull]
        List<MemberDescriptor> _members;

        ReadBuffer _readBuf;
        WriteBuffer _writeBuf;
        LengthCache _lengthCache;
        bool _preparedRead;

        int _fieldIndex;
        int _len;
        object _value;
        bool _wroteFieldHeader;

        public Type CompositeType => typeof (T);

        internal CompositeHandler(IBackendType backendType, INpgsqlNameTranslator nameTranslator, TypeHandlerRegistry registry)
            : base (backendType)
        {
            _nameTranslator = nameTranslator;
            _registry = registry;
        }

        #region Read

        public override void PrepareRead(ReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            ResolveFieldsIfNeeded();
            _readBuf = buf;
            _fieldIndex = -1;
            _len = -1;
            _preparedRead = false;
            _value = new T();
        }

        public override bool Read(out T result)
        {
            Contract.Assert(_members != null);

            result = default(T);

            if (_fieldIndex == -1)
            {
                if (_readBuf.ReadBytesLeft < 4) { return false; }
                var fieldCount = _readBuf.ReadInt32();
                if (fieldCount != _members.Count) {
                    // PostgreSQL sanity check
                    throw new Exception($"pg_attributes contains {_members.Count} rows for type {PgDisplayName}, but {fieldCount} fields were received!");
                }
                _fieldIndex = 0;
            }

            for (; _fieldIndex < _members.Count; _fieldIndex++)
            {
                var fieldDescriptor = _members[_fieldIndex];
                // Not yet started reading the field.
                // Read the type OID (not really needed), then the length.
                if (_len == -1)
                {
                    if (_readBuf.ReadBytesLeft < 8) { return false; }
                    _readBuf.ReadInt32();  // read typeOID, not used
                    _len = _readBuf.ReadInt32();
                    if (_len == -1)
                    {
                        // Null field, simply skip it and leave at default
                        continue;
                    }
                }

                // Get the field's type handler and read the value
                var handler = fieldDescriptor.Handler;
                object fieldValue;

                if (handler is ISimpleTypeHandler)
                {
                    var asSimpleReader = (ISimpleTypeHandler)handler;
                    if (_readBuf.ReadBytesLeft < _len)
                        return false;
                    fieldValue = asSimpleReader.ReadAsObject(_readBuf, _len);
                }
                else if (handler is IChunkingTypeHandler)
                {
                    var asChunkingReader = (IChunkingTypeHandler)handler;
                    if (!_preparedRead)
                    {
                        asChunkingReader.PrepareRead(_readBuf, _len);
                        _preparedRead = true;
                    }

                    if (!asChunkingReader.ReadAsObject(out fieldValue))
                        return false;
                    _preparedRead = false;
                }
                else throw PGUtil.ThrowIfReached();

                fieldDescriptor.SetValue(_value, fieldValue);
                _len = -1;
            }

            result = (T)_value;
            return true;
        }

        #endregion

        #region Write

        public override int ValidateAndGetLength(object value, ref LengthCache lengthCache, NpgsqlParameter parameter)
        {
            ResolveFieldsIfNeeded();
            Contract.Assert(_members != null);

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

                var asChunkingWriter = f.Handler as IChunkingTypeHandler;
                totalLen += asChunkingWriter?.ValidateAndGetLength(fieldValue, ref lengthCache, parameter) ??
                    ((ISimpleTypeHandler)f.Handler).ValidateAndGetLength(fieldValue, null);
            }
            return lengthCache.Lengths[pos] = totalLen;
        }

        public override void PrepareWrite(object value, WriteBuffer buf, LengthCache lengthCache, NpgsqlParameter parameter)
        {
            _value = (T)value;
            _writeBuf = buf;
            _lengthCache = lengthCache;
            _fieldIndex = -1;
            _wroteFieldHeader = false;
        }

        public override bool Write(ref DirectBuffer directBuf)
        {
            Contract.Assert(_members != null);
            if (_fieldIndex == -1)
            {
                if (_writeBuf.WriteSpaceLeft < 4) { return false; }
                _writeBuf.WriteInt32(_members.Count);
                _fieldIndex = 0;
            }

            for (; _fieldIndex < _members.Count; _fieldIndex++)
            {
                var fieldDescriptor = _members[_fieldIndex];
                var fieldHandler = fieldDescriptor.Handler;
                var fieldValue = fieldDescriptor.GetValue(_value);

                if (fieldValue == null)
                {
                    if (_writeBuf.WriteSpaceLeft < 4)
                        return false;
                    _writeBuf.WriteUInt32(fieldHandler.BackendType.OID);
                    _writeBuf.WriteInt32(-1);
                    continue;
                }

                var asSimpleWriter = fieldHandler as ISimpleTypeHandler;
                if (asSimpleWriter != null)
                {
                    var elementLen = asSimpleWriter.ValidateAndGetLength(fieldValue, null);
                    if (_writeBuf.WriteSpaceLeft < 8 + elementLen) { return false; }
                    _writeBuf.WriteUInt32(fieldDescriptor.OID);
                    _writeBuf.WriteInt32(elementLen);
                    asSimpleWriter.Write(fieldValue, _writeBuf, null);
                    continue;
                }

                var asChunkedWriter = fieldHandler as IChunkingTypeHandler;
                if (asChunkedWriter != null)
                {
                    if (!_wroteFieldHeader)
                    {
                        if (_writeBuf.WriteSpaceLeft < 8) { return false; }
                        _writeBuf.WriteUInt32(fieldDescriptor.OID);
                        _writeBuf.WriteInt32(asChunkedWriter.ValidateAndGetLength(fieldValue, ref _lengthCache, null));
                        asChunkedWriter.PrepareWrite(fieldValue, _writeBuf, _lengthCache, null);
                        _wroteFieldHeader = true;
                    }
                    if (!asChunkedWriter.Write(ref directBuf))
                    {
                        return false;
                    }
                    _wroteFieldHeader = false;
                    continue;
                }

                throw PGUtil.ThrowIfReached();
            }

            return true;
        }

        #endregion

        #region Misc

        void ResolveFieldsIfNeeded()
        {
            if (_members != null)
                return;

            _members = new List<MemberDescriptor>(RawFields.Count);
            foreach (var rawField in RawFields)
            {
                TypeHandler handler;
                if (!_registry.TryGetByOID(rawField.TypeOID, out handler))
                    throw new Exception($"PostgreSQL composite type {PgDisplayName}, mapped to CLR type {typeof (T).Name}, has field {rawField.PgName} with an unknown type (TypeOID={rawField.TypeOID})");

                var member = (
                    from m in typeof (T).GetMembers()
                    let attr = m.GetCustomAttribute<PgNameAttribute>()
                    where (attr != null && attr.PgName == rawField.PgName) ||
                          (attr == null && _nameTranslator.TranslateMemberName(m.Name) == rawField.PgName)
                    select m
                ).SingleOrDefault();

                if (member == null)
                    throw new Exception($"PostgreSQL composite type {PgDisplayName} contains field {rawField.PgName} which could not match any on CLR type {typeof (T).Name}");

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

                throw new Exception($"PostgreSQL composite type {PgDisplayName} contains field {rawField.PgName} which cannot map to CLR type {typeof (T).Name}'s field {member.Name} of type {member.GetType().Name}");
            }

            RawFields = null;
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
                else throw PGUtil.ThrowIfReached();
            }

            [CanBeNull]
            internal object GetValue(object container)
            {
                if (_property != null)
                    return _property.GetValue(container);
                if (_field != null)
                    return _field.GetValue(container);
                throw PGUtil.ThrowIfReached();
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

            public ICompositeHandler Create(IBackendType backendType, TypeHandlerRegistry registry)
                => new CompositeHandler<T>(backendType, _nameTranslator, registry);
        }
    }

    internal struct RawCompositeField
    {
        internal string PgName;
        internal uint TypeOID;

        public override string ToString() => $"{PgName} => {TypeOID}";
    }
}
