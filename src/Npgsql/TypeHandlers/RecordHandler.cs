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
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;

namespace Npgsql.TypeHandlers
{
    /// <summary>
    /// Type handler for PostgreSQL record types.
    /// </summary>
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-pseudo.html
    ///
    /// Encoding (identical to composite):
    /// A 32-bit integer with the number of columns, then for each column:
    /// * An OID indicating the type of the column
    /// * The length of the column(32-bit integer), or -1 if null
    /// * The column data encoded as binary
    /// </remarks>
    [TypeMapping("record")]
    class RecordHandler : ChunkingTypeHandler<object[]>
    {
        readonly TypeHandlerRegistry _registry;
        ReadBuffer _readBuf;

        int _fieldIndex, _fieldCount, _fieldLen;
        TypeHandler _fieldHandler;
        object[] _value;

        public RecordHandler(PostgresType postgresType, TypeHandlerRegistry registry)
            : base(postgresType)
        {
            _registry = registry;
        }

        #region Read

        public override void PrepareRead(ReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            _readBuf = buf;
            _fieldIndex = _fieldCount = - 1;
            _fieldLen = -1;
        }

        public override bool Read([CanBeNull] out object[] result)
        {
            result = null;

            if (_fieldIndex == -1)
            {
                if (_readBuf.ReadBytesLeft < 4) { return false; }
                _fieldCount = _readBuf.ReadInt32();
                _value = new object[_fieldCount];
                _fieldIndex = 0;
            }

            for (; _fieldIndex < _fieldCount; _fieldIndex++)
            {
                // Not yet started reading the field.
                // Read the type OID, then the length.
                if (_fieldLen == -1)
                {
                    if (_readBuf.ReadBytesLeft < 8) { return false; }
                    var typeOID = _readBuf.ReadUInt32();
                    _fieldLen = _readBuf.ReadInt32();
                    if (_fieldLen == -1)  // Null field, simply skip it and leave at default
                        continue;
                    _fieldHandler = _registry[typeOID];
                }

                // Get the field's type handler and read the value
                object fieldValue;
                var asSimpleHandler = _fieldHandler as ISimpleTypeHandler;
                if (asSimpleHandler != null)
                {
                    if (_readBuf.ReadBytesLeft < _fieldLen) { return false; }
                    fieldValue = asSimpleHandler.ReadAsObject(_readBuf, _fieldLen);
                }
                else if (_fieldHandler is IChunkingTypeHandler)
                {
                    var asChunkingHandler = (IChunkingTypeHandler)_fieldHandler;
                    asChunkingHandler.PrepareRead(_readBuf, _fieldLen);
                    if (!asChunkingHandler.ReadAsObject(out fieldValue))
                        return false;
                }
                else throw new InvalidOperationException("Internal Npgsql bug, please report.");

                _value[_fieldIndex] = fieldValue;
                _fieldLen = -1;
            }

            result = _value;
            _fieldHandler = null;
            return true;
        }

        #endregion

        #region Write (unsupported)

        public override int ValidateAndGetLength(object value, ref LengthCache lengthCache, NpgsqlParameter parameter)
        {
            throw new NotSupportedException("Can't write record types");
        }

        protected override Task Write(object value, WriteBuffer buf, LengthCache lengthCache, NpgsqlParameter parameter,
            bool async, CancellationToken cancellationToken)
        {
            throw new NotSupportedException("Can't write record types");
        }

        #endregion
    }
}
