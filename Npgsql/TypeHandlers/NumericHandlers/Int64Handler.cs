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
    internal class Int64Handler : TypeHandler<long>,
        ITypeHandler<byte>, ITypeHandler<short>, ITypeHandler<int>,
        ITypeHandler<float>, ITypeHandler<double>, ITypeHandler<decimal>,
    ITypeHandler<string>
    {
        static readonly string[] _pgNames = { "int8" };
        internal override string[] PgNames { get { return _pgNames; } }
        public override bool SupportsBinaryRead { get { return true; } }

        static readonly NpgsqlDbType?[] _npgsqlDbTypes = { NpgsqlDbType.Bigint };
        internal override NpgsqlDbType?[] NpgsqlDbTypes { get { return _npgsqlDbTypes; } }
        static readonly DbType?[] _dbTypes = { DbType.Int64 };
        internal override DbType?[] DbTypes { get { return _dbTypes; } }

        public override long Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            switch (fieldDescription.FormatCode)
            {
                case FormatCode.Text:
                    return Int64.Parse(buf.ReadString(len), CultureInfo.InvariantCulture);
                case FormatCode.Binary:
                    return buf.ReadInt64();
                default:
                    throw PGUtil.ThrowIfReached("Unknown format code: " + fieldDescription.FormatCode);
            }
        }

        byte ITypeHandler<byte>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return (byte)Read(buf, fieldDescription, len);
        }

        short ITypeHandler<short>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return (short)Read(buf, fieldDescription, len);
        }

        int ITypeHandler<int>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return (int)Read(buf, fieldDescription, len);
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
            var i = GetIConvertibleValue<long>(value);
            writer.WriteString(i.ToString(CultureInfo.InvariantCulture));
        }

        protected override int BinarySize(object value)
        {
            return 12;
        }

        protected override void WriteBinary(object value, NpgsqlBuffer buf)
        {
            var i = GetIConvertibleValue<long>(value);
            buf.WriteInt32(8);
            buf.WriteInt64(i);
        }
    }
}
