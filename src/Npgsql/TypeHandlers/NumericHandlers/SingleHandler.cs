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
    [TypeMapping("float4", NpgsqlDbType.Real, DbType.Single, typeof(float))]
    internal class SingleHandler : TypeHandler<float>,
        ISimpleTypeReader<float>, ISimpleTypeWriter,
        ISimpleTypeReader<double>
    {
        public float Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            return buf.ReadSingle();
        }

        double ISimpleTypeReader<double>.Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            return Read(buf, len, fieldDescription);
        }

        public int ValidateAndGetLength(object value, NpgsqlParameter parameter)
        {
            if (!(value is float))
            {
                var converted = Convert.ToSingle(value);
                if (parameter == null)
                {
                    throw CreateConversionButNoParamException(value.GetType());
                }
                parameter.ConvertedValue = converted;
            }
            return 4;
        }

        public void Write(object value, NpgsqlBuffer buf, NpgsqlParameter parameter)
        {
            if (parameter != null && parameter.ConvertedValue != null) {
                value = parameter.ConvertedValue;
            }
            buf.WriteSingle((float)value);
        }
    }
}
