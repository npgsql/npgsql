using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Npgsql.Messages;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.NumericHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/9.3/static/datatype-oid.html
    /// </remarks>
    internal class UInt32Handler : TypeHandler<uint>
    {
        static readonly string[] _pgNames = { "oid", "xid", "cid" };
        internal override string[] PgNames { get { return _pgNames; } }
        public override bool SupportsBinaryRead { get { return true; } }

        static readonly NpgsqlDbType?[] _npgsqlDbTypes = { NpgsqlDbType.Oid, NpgsqlDbType.Xid, NpgsqlDbType.Cid };
        internal override NpgsqlDbType?[] NpgsqlDbTypes { get { return _npgsqlDbTypes; } }

        public override uint Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            switch (fieldDescription.FormatCode)
            {
                case FormatCode.Text:
                    return UInt32.Parse(buf.ReadString(len), CultureInfo.InvariantCulture);
                case FormatCode.Binary:
                    return (uint)buf.ReadInt32();
                default:
                    throw PGUtil.ThrowIfReached("Unknown format code: " + fieldDescription.FormatCode);
            }
        }

        public override void WriteText(object value, NpgsqlTextWriter writer)
        {
            var i = (uint)value;
            writer.WriteString(i.ToString(CultureInfo.InvariantCulture));
        }

        protected override int BinarySize(object value)
        {
            return 8;
        }

        protected override void WriteBinary(object value, NpgsqlBuffer buf)
        {
            buf.WriteInt32(4);
            buf.WriteInt32((int)(uint)value);
        }
    }
}
