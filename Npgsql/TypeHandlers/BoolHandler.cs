using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql.BackendMessages;
using NpgsqlTypes;
using System.Data;

namespace Npgsql.TypeHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/9.3/static/datatype-boolean.html
    /// </remarks>
    [TypeMapping("bool", NpgsqlDbType.Boolean, DbType.Boolean, typeof(bool))]
    internal class BoolHandler : TypeHandler<bool>,
        ISimpleTypeReader<bool>, ISimpleTypeWriter
    {
        const byte T = (byte)'T';
        const byte t = (byte)'t';

        public bool Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return buf.ReadByte() != 0;
        }

        public int ValidateAndGetLength(object value) { return 1; }

        public void Write(object value, NpgsqlBuffer buf)
        {
            var v = ((IConvertible)value).ToBoolean(null);
            buf.WriteByte(v ? (byte)1 : (byte)0);
        }
    }
}
