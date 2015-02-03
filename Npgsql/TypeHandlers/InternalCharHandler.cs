using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql.BackendMessages;
using NpgsqlTypes;
using System.Data;

namespace Npgsql.TypeHandlers
{
    /// <summary>
    /// Type handler for the Postgresql "char" type, used only internally
    /// </summary>
    /// <remarks>
    /// http://www.postgresql.org/docs/9.3/static/datatype-character.html
    /// </remarks>
    [TypeMapping("char", NpgsqlDbType.SingleChar)]
    internal class InternalCharHandler : TypeHandler<char>,
        ISimpleTypeReader<char>, ISimpleTypeWriter
    {
        public char Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return buf.ReadString(1)[0];
        }

        public int ValidateAndGetLength(object value) { return 1; }

        public void Write(object value, NpgsqlBuffer buf)
        {
            buf.WriteByte((byte)(char)value);
        }
    }
}
