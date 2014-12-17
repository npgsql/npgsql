using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql.Messages;

namespace Npgsql.TypeHandlers
{
    internal class DecimalHandler : TypeHandler<decimal>,
        ITypeHandler<byte>, ITypeHandler<short>, ITypeHandler<int>, ITypeHandler<long>,
        ITypeHandler<float>, ITypeHandler<double>,
        ITypeHandler<string>
    {
        static readonly string[] _pgNames = { "numeric" };
        internal override string[] PgNames { get { return _pgNames; } }

        public override decimal Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            switch (fieldDescription.FormatCode)
            {
                case FormatCode.Text:
                    return Decimal.Parse(buf.ReadString(len));
                case FormatCode.Binary:
                    throw new NotSupportedException();
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

        long ITypeHandler<long>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return (long)Read(buf, fieldDescription, len);
        }

        float ITypeHandler<float>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return (float)Read(buf, fieldDescription, len);
        }

        double ITypeHandler<double>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return (double)Read(buf, fieldDescription, len);
        }

        string ITypeHandler<string>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return Read(buf, fieldDescription, len).ToString();
        }
    }
}
