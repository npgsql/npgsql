using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql.Messages;

namespace Npgsql.TypeHandlers
{
    internal class Int32Handler : TypeHandler<int>,
        ITypeHandler<byte>, ITypeHandler<short>, ITypeHandler<long>,
        ITypeHandler<float>, ITypeHandler<double>, ITypeHandler<decimal>,
    ITypeHandler<string>
    {
        static readonly string[] _pgNames = { "int4" };
        internal override string[] PgNames { get { return _pgNames; } }
        public override bool SupportsBinaryRead { get { return true; } }

        public override int Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            switch (fieldDescription.FormatCode)
            {
                case FormatCode.Text:
                    return Int32.Parse(buf.ReadString(len));
                case FormatCode.Binary:
                    return buf.ReadInt32();
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
    }
}
