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
    internal class Int16Handler : TypeHandler<short>,
        ITypeHandler<byte>, ITypeHandler<int>, ITypeHandler<long>,
        ITypeHandler<float>, ITypeHandler<double>, ITypeHandler<decimal>,
    ITypeHandler<string>
    {
        static readonly string[] _pgNames = { "int2" };
        internal override string[] PgNames { get { return _pgNames; } }
        public override bool SupportsBinaryRead { get { return true; } }

        static readonly NpgsqlDbType?[] _npgsqlDbTypes = { NpgsqlDbType.Smallint };
        internal override NpgsqlDbType?[] NpgsqlDbTypes { get { return _npgsqlDbTypes; } }
        static readonly DbType?[] _dbTypes = { DbType.Int16 };
        internal override DbType?[] DbTypes { get { return _dbTypes; } }

        public override short Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            switch (fieldDescription.FormatCode)
            {
                case FormatCode.Text:
                    return Int16.Parse(buf.ReadString(len), CultureInfo.InvariantCulture);
                case FormatCode.Binary:
                    return buf.ReadInt16();
                default:
                    throw PGUtil.ThrowIfReached("Unknown format code: " + fieldDescription.FormatCode);
            }
        }

        byte ITypeHandler<byte>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return (byte)Read(buf, fieldDescription, len);
        }

        int ITypeHandler<int>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return Read(buf, fieldDescription, len);
        }

        long ITypeHandler<long>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return Read(buf, fieldDescription, len);
        }

        float ITypeHandler<float>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return Read(buf, fieldDescription, len);
        }

        double ITypeHandler<double>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return Read(buf, fieldDescription, len);
        }

        decimal ITypeHandler<decimal>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return Read(buf, fieldDescription, len);
        }

        string ITypeHandler<string>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return Read(buf, fieldDescription, len).ToString();
        }

        public override void WriteText(object value, NpgsqlTextWriter writer)
        {
            var i = GetIConvertibleValue<short>(value);
            writer.WriteString(i.ToString(CultureInfo.InvariantCulture));
        }

        protected override int BinarySize(object value)
        {
            return 6;
        }

        protected override void WriteBinary(object value, NpgsqlBuffer buf)
        {
            var i = GetIConvertibleValue<short>(value);
            buf.WriteInt32(2);
            buf.WriteInt16(i);
        }
    }
}
