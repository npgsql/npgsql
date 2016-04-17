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
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Npgsql.BackendMessages;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.GeometricHandlers
{
    /// <summary>
    /// Type handler for the PostgreSQL geometric path segment type (open or closed).
    /// </summary>
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-geometric.html
    /// </remarks>
    [TypeMapping("path", NpgsqlDbType.Path, typeof(NpgsqlPath))]
    internal class PathHandler : ChunkingTypeHandler<NpgsqlPath>
    {
        #region State

        NpgsqlPath _value;
        ReadBuffer _readBuf;
        WriteBuffer _writeBuf;
        int _index;

        #endregion

        #region Read

        public override void PrepareRead(ReadBuffer buf, int len, FieldDescription fieldDescription)
        {
            _readBuf = buf;
            _index = -1;
        }

        public override bool Read(out NpgsqlPath result)
        {
            result = default(NpgsqlPath);

            if (_index == -1)
            {
                if (_readBuf.ReadBytesLeft < 5) { return false; }

                bool open;
                var openByte = _readBuf.ReadByte();
                switch (openByte) {
                    case 1:
                        open = false;
                        break;
                    case 0:
                        open = true;
                        break;
                    default:
                        throw new Exception("Error decoding binary geometric path: bad open byte");
                }
                var numPoints = _readBuf.ReadInt32();
                _value = new NpgsqlPath(numPoints, open);
                _index = 0;
            }

            for (; _index < _value.Capacity; _index++)
            {
                if (_readBuf.ReadBytesLeft < 16) { return false; }
                _value.Add(new NpgsqlPoint(_readBuf.ReadDouble(), _readBuf.ReadDouble()));
            }
            result = _value;
            _value = default(NpgsqlPath);
            _readBuf = null;
            return true;
        }

        #endregion

        #region Write

        public override int ValidateAndGetLength(object value, ref LengthCache lengthCache, NpgsqlParameter parameter=null)
        {
            if (!(value is NpgsqlPath))
                    throw CreateConversionException(value.GetType());
            return 5 + ((NpgsqlPath)value).Count * 16;
        }

        public override void PrepareWrite(object value, WriteBuffer buf, LengthCache lengthCache, NpgsqlParameter parameter=null)
        {
            _writeBuf = buf;
            _value = (NpgsqlPath)value;
            _index = -1;
        }

        public override bool Write(ref DirectBuffer directBuf)
        {
            if (_index == -1)
            {
                if (_writeBuf.WriteSpaceLeft < 5) { return false; }
                _writeBuf.WriteByte((byte)(_value.Open ? 0 : 1));
                _writeBuf.WriteInt32(_value.Count);
                _index = 0;
            }

            for (; _index < _value.Count; _index++)
            {
                if (_writeBuf.WriteSpaceLeft < 16) { return false; }
                var p = _value[_index];
                _writeBuf.WriteDouble(p.X);
                _writeBuf.WriteDouble(p.Y);
            }
            _writeBuf = null;
            _value = default(NpgsqlPath);
            return true;
        }

        #endregion
    }
}
