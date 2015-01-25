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
    [TypeMapping("numeric", NpgsqlDbType.Numeric, new[] { DbType.Decimal, DbType.VarNumeric }, typeof(decimal))]
    internal class NumericHandler : TypeHandler<decimal>,
        ISimpleTypeReader<decimal>, ISimpleTypeWriter,
        ISimpleTypeReader<byte>, ISimpleTypeReader<short>, ISimpleTypeReader<int>, ISimpleTypeReader<long>,
        ISimpleTypeReader<float>, ISimpleTypeReader<double>,
        ISimpleTypeReader<string>
    {
        public override bool SupportsBinaryWrite { get { return false; } }

        public decimal Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            throw new NotSupportedException();
        }

        byte ISimpleTypeReader<byte>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return (byte)Read(buf, fieldDescription, len);
        }

        short ISimpleTypeReader<short>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return (short)Read(buf, fieldDescription, len);
        }

        int ISimpleTypeReader<int>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return (int)Read(buf, fieldDescription, len);
        }

        long ISimpleTypeReader<long>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return (long)Read(buf, fieldDescription, len);
        }

        float ISimpleTypeReader<float>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return (float)Read(buf, fieldDescription, len);
        }

        double ISimpleTypeReader<double>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return (double)Read(buf, fieldDescription, len);
        }

        string ISimpleTypeReader<string>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return Read(buf, fieldDescription, len).ToString();
        }

        public int ValidateAndGetLength(object value)
        {
            throw new NotImplementedException();
        }

        public void Write(object value, NpgsqlBuffer buf)
        {
            throw new NotImplementedException();
        }
    }
}
