using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Npgsql.BackendMessages;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.NumericHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-oid.html
    /// </remarks>
    [TypeMapping("oid", NpgsqlDbType.Oid)]
    [TypeMapping("xid", NpgsqlDbType.Xid)]
    [TypeMapping("cid", NpgsqlDbType.Cid)]
    internal class UInt32Handler : TypeHandler<uint>,
        ISimpleTypeReader<uint>, ISimpleTypeWriter
    {
        public uint Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            return (uint)buf.ReadInt32();
        }

        public int ValidateAndGetLength(object value, NpgsqlParameter parameter)
        {
            if (!(value is uint))
            {
                var converted = Convert.ToUInt32(value);
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
            buf.WriteInt32((int)(uint)value);
        }
    }
}
