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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Npgsql.BackendMessages;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.GeometricHandlers
{
    /// <summary>
    /// Type handler for the PostgreSQL geometric polygon type.
    /// </summary>
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-geometric.html
    /// </remarks>
    [TypeMapping("polygon", NpgsqlDbType.Polygon, typeof(NpgsqlPolygon))]
    internal class PolygonHandler : ChunkingTypeHandler<NpgsqlPolygon>
    {
        #region State

        NpgsqlPolygon _value;
        NpgsqlBuffer _buf;
        int _index;

        #endregion

        #region Read

        public override void PrepareRead(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            _buf = buf;
            _index = -1;
        }

        public override bool Read(out NpgsqlPolygon result)
        {
            result = default(NpgsqlPolygon);

            if (_index == -1)
            {
                if (_buf.ReadBytesLeft < 4) { return false; }
                var numPoints = _buf.ReadInt32();
                _value = new NpgsqlPolygon(numPoints);
                _index = 0;
            }

            for (; _index < _value.Capacity; _index++) {
                if (_buf.ReadBytesLeft < 16) { return false; }
                _value.Add(new NpgsqlPoint(_buf.ReadDouble(), _buf.ReadDouble()));
            }
            result = _value;
            _value = default(NpgsqlPolygon);
            _buf = null;
            return true;
        }

        #endregion

        #region Write

        public override int ValidateAndGetLength(object value, ref LengthCache lengthCache, NpgsqlParameter parameter=null)
        {
            if (!(value is NpgsqlPolygon))
                throw CreateConversionException(value.GetType());
            return 4 + ((NpgsqlPolygon)value).Count * 16;
        }

        public override void PrepareWrite(object value, NpgsqlBuffer buf, LengthCache lengthCache, NpgsqlParameter parameter=null)
        {
            _buf = buf;
            _value = (NpgsqlPolygon)value;
            _index = -1;
        }

        public override bool Write(ref DirectBuffer directBuf)
        {
            if (_index == -1)
            {
                if (_buf.WriteSpaceLeft < 4) { return false; }
                _buf.WriteInt32(_value.Count);
                _index = 0;
            }

            for (; _index < _value.Count; _index++)
            {
                if (_buf.WriteSpaceLeft < 16) { return false; }
                var p = _value[_index];
                _buf.WriteDouble(p.X);
                _buf.WriteDouble(p.Y);
            }
            _buf = null;
            _value = default(NpgsqlPolygon);
            return true;
        }

        #endregion
    }
}
