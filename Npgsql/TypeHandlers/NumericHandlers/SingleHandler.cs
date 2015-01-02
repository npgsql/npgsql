using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Npgsql.Messages;
using NpgsqlTypes;
using System.Data;

namespace Npgsql.TypeHandlers.NumericHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/9.3/static/datatype-numeric.html
    /// </remarks>
    internal class SingleHandler : TypeHandler<float>,
        ITypeHandler<double>
    {
        static readonly string[] _pgNames = { "float4" };
        internal override string[] PgNames { get { return _pgNames; } }
        public override bool SupportsBinaryRead { get { return true; } }

        static readonly NpgsqlDbType?[] _npgsqlDbTypes = { NpgsqlDbType.Real };
        internal override NpgsqlDbType?[] NpgsqlDbTypes { get { return _npgsqlDbTypes; } }
        static readonly DbType?[] _dbTypes = { DbType.Single };
        internal override DbType?[] DbTypes { get { return _dbTypes; } }

        public override float Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            switch (fieldDescription.FormatCode)
            {
                case FormatCode.Text:
                    return Single.Parse(buf.ReadString(len), CultureInfo.InvariantCulture);
                case FormatCode.Binary:
                    return buf.ReadSingle();
                default:
                    throw PGUtil.ThrowIfReached("Unknown format code: " + fieldDescription.FormatCode);
            }
        }

        double ITypeHandler<double>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return Read(buf, fieldDescription, len);
        }

        public override void WriteText(object value, NpgsqlTextWriter writer)
        {
            var f = GetIConvertibleValue<float>(value);
            writer.WriteString(f.ToString(CultureInfo.InvariantCulture));
        }

        protected override int BinarySize(object value)
        {
            return 8;
        }

        protected override void WriteBinary(object value, NpgsqlBuffer buf)
        {
            var f = GetIConvertibleValue<float>(value);
            buf.WriteInt32(4);
            buf.WriteSingle(f);
        }
    }
}
