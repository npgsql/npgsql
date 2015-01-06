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
    [TypeMapping("bool", NpgsqlDbType.Boolean, DbType.Boolean, typeof(bool))]
    internal class BoolHandler : TypeHandler<bool>
    {
        const byte T = (byte)'T';
        const byte t = (byte)'t';

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

        internal override int Length(object value)
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
