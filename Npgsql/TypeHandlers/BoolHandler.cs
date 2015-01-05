using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql.Messages;
using NpgsqlTypes;
using System.Data;

namespace Npgsql.TypeHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/9.3/static/datatype-boolean.html
    /// </remarks>
    internal class BoolHandler : TypeHandler<bool>
    {
        static readonly string[] _pgNames = { "bool" };
        internal override string[] PgNames { get { return _pgNames; } }
        public override bool SupportsBinaryRead { get { return true; } }

        static readonly NpgsqlDbType?[] _npgsqlDbTypes = { NpgsqlDbType.Boolean };
        internal override NpgsqlDbType?[] NpgsqlDbTypes { get { return _npgsqlDbTypes; } }
        static readonly DbType?[] _dbTypes = { DbType.Boolean };
        internal override DbType?[] DbTypes { get { return _dbTypes; } }

        const byte T = (byte)'T';
        const byte t = (byte)'t';
        const byte f = (byte)'f';

        public override bool Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            switch (fieldDescription.FormatCode)
            {
                case FormatCode.Text:
                    var b = buf.ReadByte();
                    return b == T || b == t;
                case FormatCode.Binary:
                    return buf.ReadByte() != 0;
                default:
                    throw PGUtil.ThrowIfReached("Unknown format code: " + fieldDescription.FormatCode);
            }
        }

        public override void WriteText(object value, NpgsqlTextWriter writer)
        {
            var v = ((IConvertible)value).ToBoolean(null);
            writer.WriteSingleChar(v ? (char)t : (char)f);
        }

        internal override int BinarySize(object value)
        {
            return 1;
        }

        internal override void WriteBinary(object value, NpgsqlBuffer buf)
        {
            var v = ((IConvertible)value).ToBoolean(null);
            buf.WriteByte(v ? (byte)1 : (byte)0);
        }
    }
}
