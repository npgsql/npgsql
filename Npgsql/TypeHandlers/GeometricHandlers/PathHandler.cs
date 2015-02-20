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
    /// http://www.postgresql.org/docs/9.4/static/datatype-geometric.html
    /// </remarks>
    [TypeMapping("path", NpgsqlDbType.Path, typeof(NpgsqlPath))]
    internal class PathHandler : TypeHandler<NpgsqlPath>,
        IChunkingTypeReader<NpgsqlPath>, IChunkingTypeWriter
    {
        #region State

        NpgsqlPath _value;
        NpgsqlBuffer _buf;
        int _index;

        #endregion

        #region Read

        public void PrepareRead(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            _buf = buf;
            _index = -1;
        }

        public bool Read(out NpgsqlPath result)
        {
            result = default(NpgsqlPath);

            if (_index == -1)
            {
                if (_buf.ReadBytesLeft < 5) { return false; }

                bool open;
                var openByte = _buf.ReadByte();
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
                var numPoints = _buf.ReadInt32();
                _value = new NpgsqlPath(numPoints, open);
                _index = 0;
            }

            for (; _index < _value.Capacity; _index++)
            {
                if (_buf.ReadBytesLeft < 16) { return false; }
                _value.Add(new NpgsqlPoint(_buf.ReadDouble(), _buf.ReadDouble()));
            }
            result = _value;
            _value = default(NpgsqlPath);
            _buf = null;
            return true;
        }

        #endregion

        #region Write

        public int ValidateAndGetLength(object value, NpgsqlParameter parameter)
        {
            if (!(value is NpgsqlPath)) {
                throw new InvalidCastException("Expected an NpgsqlPath");
            }
            return 5 + ((NpgsqlPath)value).Count * 16;
        }

        public void PrepareWrite(object value, NpgsqlBuffer buf, NpgsqlParameter parameter)
        {
            _buf = buf;
            _value = (NpgsqlPath)value;
            _index = -1;
        }

        public bool Write(ref DirectBuffer directBuf)
        {
            if (_index == -1)
            {
                if (_buf.WriteSpaceLeft < 5) { return false; }
                _buf.WriteByte((byte)(_value.Open ? 0 : 1));
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
            _value = default(NpgsqlPath);
            return true;
        }

        #endregion
    }
}
