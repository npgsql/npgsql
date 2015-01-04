using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql.Messages;
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
    internal class InternalCharHandler : TypeHandler<char>
    {
        static readonly string[] _pgNames = { "char" };
        internal override string[] PgNames { get { return _pgNames; } }
        public override bool SupportsBinaryRead { get { return true; } }

        static readonly NpgsqlDbType?[] _npgsqlDbTypes = { NpgsqlDbType.SingleChar };
        internal override NpgsqlDbType?[] NpgsqlDbTypes { get { return _npgsqlDbTypes; } }
        static readonly DbType?[] _dbTypes = { null };
        internal override DbType?[] DbTypes { get { return _dbTypes; } }

        public override char Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return buf.ReadString(1)[0];
        }

        public override void WriteText(object value, NpgsqlTextWriter writer)
        {
            var ch = (char)value;
            writer.WriteSingleChar(ch);
        }

        protected override int BinarySize(object value)
        {
            return 5;
        }

        protected override void WriteBinary(object value, NpgsqlBuffer buf)
        {
            buf.WriteInt32(1);
            buf.WriteByte((byte)(char)value);
        }
    }
}
