using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Npgsql.BackendMessages;
using NpgsqlTypes;
using System.Data;

namespace Npgsql.TypeHandlers.NumericHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-numeric.html
    /// </remarks>
    [TypeMapping("int8", NpgsqlDbType.Bigint, DbType.Int64, typeof(long))]
    internal class Int64Handler : TypeHandler<long>,
        ISimpleTypeReader<long>, ISimpleTypeWriter,
        ISimpleTypeReader<byte>, ISimpleTypeReader<short>, ISimpleTypeReader<int>,
        ISimpleTypeReader<float>, ISimpleTypeReader<double>, ISimpleTypeReader<decimal>,
    ISimpleTypeReader<string>
    {
        public long Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            return buf.ReadInt64();
        }

        byte ISimpleTypeReader<byte>.Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            return (byte)Read(buf, len, fieldDescription);
        }

        short ISimpleTypeReader<short>.Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            return (short)Read(buf, len, fieldDescription);
        }

        int ISimpleTypeReader<int>.Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            return (int)Read(buf, len, fieldDescription);
        }

        float ISimpleTypeReader<float>.Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            return Read(buf, len, fieldDescription);
        }

        double ISimpleTypeReader<double>.Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            return Read(buf, len, fieldDescription);
        }

        decimal ISimpleTypeReader<decimal>.Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            return Read(buf, len, fieldDescription);
        }

        string ISimpleTypeReader<string>.Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            return Read(buf, len, fieldDescription).ToString();
        }

        public int ValidateAndGetLength(object value) { return 8; }

        public void Write(object value, NpgsqlBuffer buf)
        {
            var i = GetIConvertibleValue<long>(value);
            buf.WriteInt64(i);
        }
    }
}
