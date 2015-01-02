using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Npgsql.Messages;
using NpgsqlTypes;
using System.Data;

namespace Npgsql.TypeHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/9.3/static/datatype-uuid.html
    /// </remarks>
    internal class UuidHandler : TypeHandler<Guid>, ITypeHandler<string>
    {
        static readonly string[] _pgNames = { "uuid" };
        internal override string[] PgNames { get { return _pgNames; } }
        public override bool SupportsBinaryRead { get { return true; } }

        static readonly NpgsqlDbType?[] _npgsqlDbTypes = { NpgsqlDbType.Uuid };
        internal override NpgsqlDbType?[] NpgsqlDbTypes { get { return _npgsqlDbTypes; } }
        static readonly DbType?[] _dbTypes = { DbType.Guid };
        internal override DbType?[] DbTypes { get { return _dbTypes; } }

        public override Guid Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            switch (fieldDescription.FormatCode)
            {
                case FormatCode.Text:
                    return new Guid(buf.ReadString(len));
                case FormatCode.Binary:
                    return ReadBinary(buf);
                default:
                    throw PGUtil.ThrowIfReached("Unknown format code: " + fieldDescription.FormatCode);
            }
        }

        Guid ReadBinary(NpgsqlBuffer buf)
        {
            buf.Ensure(16);
            var a = buf.ReadInt32();
            var b = buf.ReadInt16();
            var c = buf.ReadInt16();
            var d = new byte[8];
            buf.ReadBytes(d, 0, 8, true);
            return new Guid(a, b, c, d);
        }

        string ITypeHandler<string>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return Read(buf, fieldDescription, len).ToString();
        }

        public override void WriteText(object value, NpgsqlTextWriter writer)
        {
            var guid = value is Guid ? (Guid)value : Guid.Parse(value.ToString());
            writer.WriteString(guid.ToString());
        }
    }
}
