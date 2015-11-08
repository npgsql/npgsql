#region License
// The PostgreSQL License
//
// Copyright (C) 2015 The Npgsql Development Team
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
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
#pragma warning disable 1591
        List<Tuple<string, uint>> RawFields { get; set; }
        ICompositeHandler Clone(TypeHandlerRegistry registry);
#pragma warning restore 1591
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
        public List<Tuple<string, uint>> RawFields { get; set; }
        List<FieldDescriptor> _fields;

        NpgsqlBuffer _buf;
        LengthCache _lengthCache;

        int _fieldIndex;
        int _len;
        object _value;
        bool _wroteFieldHeader;

        public Type CompositeType => typeof (T);

        internal CompositeHandler() {}

        CompositeHandler(TypeHandlerRegistry registry)
        {
            _registry = registry;
        }

        #region Read

        public override void PrepareRead(NpgsqlBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            ResolveFieldsIfNeeded();
            _buf = buf;
            _fieldIndex = -1;
            _len = -1;
            _value = new T();
        }

        public override bool Read(out T result)
        {
            result = default(T);

            if (_fieldIndex == -1)
            {
                if (_buf.ReadBytesLeft < 4) { return false; }
                var fieldCount = _buf.ReadInt32();
                if (fieldCount != _fields.Count) {
                    // PostgreSQL sanity check
                    throw new Exception(
                        $"pg_attributes contains {_fields.Count} rows for type {PgName}, but {fieldCount} fields were received!");
                }
                _fieldIndex = 0;
            }

            for (; _fieldIndex < _fields.Count; _fieldIndex++)
            {
                var fieldDescriptor = _fields[_fieldIndex];
                // Not yet started reading the field.
                // Read the type OID (not really needed), then the length.
                if (_len == -1)
                {
                    if (_buf.ReadBytesLeft < 8) { return false; }
                    var typeOID = _buf.ReadInt32();
                    Contract.Assume(typeOID == fieldDescriptor.Handler.OID);
                    _len = _buf.ReadInt32();
                    if (_len == -1)
                    {
                        // Null field, simply skip it and leave at default
                        continue;
                    }
                }

                // Get the field's type handler and read the value
                var handler = fieldDescriptor.Handler;
                object fieldValue = null;

                if (handler is ISimpleTypeHandler)
                {
                    var asSimpleReader = (ISimpleTypeHandler)handler;
                    if (_buf.ReadBytesLeft < _len) { return false; }
                    fieldValue = asSimpleReader.ReadAsObject(_buf, _len);
                }
                else if (handler is IChunkingTypeHandler)
                {
                    var asChunkingReader = (IChunkingTypeHandler)handler;
                    asChunkingReader.PrepareRead(_buf, _len);
                    if (!asChunkingReader.ReadAsObject(out fieldValue)) {
                        return false;
                    }
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

            if (lengthCache == null)
            {
                lengthCache = new LengthCache(1);
            }
            if (lengthCache.IsPopulated)
            {
                return lengthCache.Get();
            }

            // Leave empty slot for the entire composite type, and go ahead an populate the element slots
            var pos = lengthCache.Position;
            lengthCache.Set(0);
            var totalLen = 4;  // number of fields
            foreach (var f in _fields)
            {
                totalLen += 4 + 4;  // type oid + field length
                var fieldValue = f.GetValue(value);
                var asChunkingWriter = f.Handler as IChunkingTypeHandler;
                totalLen += asChunkingWriter?.ValidateAndGetLength(fieldValue, ref lengthCache, parameter) ??
                    ((ISimpleTypeHandler)f.Handler).ValidateAndGetLength(fieldValue, null);
            }
            return lengthCache.Lengths[pos] = totalLen;
        }

        public override void PrepareWrite(object value, NpgsqlBuffer buf, LengthCache lengthCache, NpgsqlParameter parameter)
        {
            _value = (T)value;
            _buf = buf;
            _lengthCache = lengthCache;
            _fieldIndex = -1;
            _wroteFieldHeader = false;
        }

        public override bool Write(ref DirectBuffer directBuf)
        {
            if (_fieldIndex == -1)
            {
                if (_buf.WriteSpaceLeft < 4) { return false; }
                _buf.WriteInt32(_fields.Count);
                _fieldIndex = 0;
            }

            for (; _fieldIndex < _fields.Count; _fieldIndex++)
            {
                var fieldDescriptor = _fields[_fieldIndex];
                var fieldHandler = fieldDescriptor.Handler;
                var fieldValue = fieldDescriptor.GetValue(_value);

                var asSimpleWriter = fieldHandler as ISimpleTypeHandler;
                if (asSimpleWriter != null)
                {
                    var elementLen = asSimpleWriter.ValidateAndGetLength(fieldValue, null);
                    if (_buf.WriteSpaceLeft < 8 + elementLen) { return false; }
                    _buf.WriteUInt32(fieldDescriptor.Handler.OID);
                    _buf.WriteInt32(elementLen);
                    asSimpleWriter.Write(fieldValue, _buf, null);
                    continue;
                }

                var asChunkedWriter = fieldHandler as IChunkingTypeHandler;
                if (asChunkedWriter != null)
                {
                    if (!_wroteFieldHeader)
                    {
                        if (_buf.WriteSpaceLeft < 8) { return false; }
                        _buf.WriteUInt32(fieldDescriptor.Handler.OID);
                        _buf.WriteInt32(asChunkedWriter.ValidateAndGetLength(fieldValue, ref _lengthCache, null));
                        asChunkedWriter.PrepareWrite(fieldValue, _buf, _lengthCache, null);
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
            if (_fields != null) { return; }

            _fields = new List<FieldDescriptor>(RawFields.Count);
            foreach (var rawField in RawFields)
            {
                TypeHandler fieldHandler;
                if (!_registry.OIDIndex.TryGetValue(rawField.Item2, out fieldHandler))
                {
                    throw new Exception(
                        $"PostgreSQL composite type {PgName}, mapped to CLR type {typeof (T).Name}, has field {rawField.Item1} with a type that hasn't been registered (OID={rawField.Item2})");
                }

                var member = (
                    from m in typeof (T).GetMembers()
                    let attr = m.GetCustomAttribute<PgNameAttribute>()
                    where (attr != null && attr.PgName == rawField.Item1) ||
                          (attr == null && m.Name == rawField.Item1)
                    select m
                ).SingleOrDefault();

                if (member == null)
                {
                    throw new Exception(
                        $"PostgreSQL composite type {PgName} contains field {rawField.Item1} which could not match any on CLR type {typeof (T).Name}");
                }

                var property = member as PropertyInfo;
                if (property != null)
                {
                    _fields.Add(new FieldDescriptor(rawField.Item1, fieldHandler, property));
                    continue;
                }

                var field = member as FieldInfo;
                if (field != null)
                {
                    _fields.Add(new FieldDescriptor(rawField.Item1, fieldHandler, field));
                    continue;
                }

                throw new Exception(
                    $"PostgreSQL composite type {PgName} contains field {rawField.Item1} which cannot map to CLR type {typeof (T).Name}'s field {member.Name} of type {member.GetType().Name}");
            }

            RawFields = null;
        }

        public ICompositeHandler Clone(TypeHandlerRegistry registry)
        {
            return new CompositeHandler<T>(registry);
        }

        struct FieldDescriptor
        {
            internal readonly string Name;
            internal readonly TypeHandler Handler;
            readonly PropertyInfo _property;
            readonly FieldInfo _field;

            internal FieldDescriptor(string name, TypeHandler handler, PropertyInfo property)
            {
                Name = name;
                Handler = handler;
                _property = property;
                _field = null;
            }

            internal FieldDescriptor(string name, TypeHandler handler, FieldInfo field)
            {
                Name = name;
                Handler = handler;
                _property = null;
                _field = field;
            }

            internal void SetValue(object container, object fieldValue)
            {
                if (_property != null)
                {
                    _property.SetValue(container, fieldValue);
                }
                else if (_field != null)
                {
                    _field.SetValue(container, fieldValue);
                }
                else throw PGUtil.ThrowIfReached();
            }

            internal object GetValue(object container)
            {
                if (_property != null)
                {
                    return _property.GetValue(container);
                }
                if (_field != null)
                {
                    return _field.GetValue(container);
                }
                throw PGUtil.ThrowIfReached();
            }
        }

        #endregion
    }
}
