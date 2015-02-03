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
    /// http://www.postgresql.org/docs/9.3/static/datatype-numeric.html
    /// </remarks>
    [TypeMapping("float4", NpgsqlDbType.Real, DbType.Single, typeof(float))]
    internal class SingleHandler : TypeHandler<float>,
        ISimpleTypeReader<float>, ISimpleTypeWriter,
        ISimpleTypeReader<double>
    {
        public float Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return buf.ReadSingle();
        }

        double ISimpleTypeReader<double>.Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return Read(buf, fieldDescription, len);
        }

        public int ValidateAndGetLength(object value) { return 4; }

        public void Write(object value, NpgsqlBuffer buf)
        {
            var f = GetIConvertibleValue<float>(value);
            buf.WriteSingle(f);
        }
    }
}
