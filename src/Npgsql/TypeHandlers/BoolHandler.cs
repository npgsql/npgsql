using System;
using Npgsql.BackendMessages;
using NpgsqlTypes;
using System.Data;

namespace Npgsql.TypeHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-boolean.html
    /// </remarks>
    [TypeMapping("bool", NpgsqlDbType.Boolean, DbType.Boolean, typeof(bool))]
    internal class BoolHandler : TypeHandler<bool>,
        ISimpleTypeReader<bool>, ISimpleTypeWriter
    {
        public bool Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            return buf.ReadByte() != 0;
        }

        public int ValidateAndGetLength(object value, NpgsqlParameter parameter)
        {
            if (!(value is bool))
            {
                var converted = Convert.ToBoolean(value);
                if (parameter == null)
                {
                    throw CreateConversionButNoParamException(value.GetType());
                }
                parameter.ConvertedValue = converted;
            }
            return 1;
        }

        public void Write(object value, NpgsqlBuffer buf, NpgsqlParameter parameter)
        {
            if (parameter != null && parameter.ConvertedValue != null) {
                value = parameter.ConvertedValue;
            }
            buf.WriteByte(((bool)value) ? (byte)1 : (byte)0);
        }
    }
}
