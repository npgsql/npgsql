using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Npgsql.BackendMessages;
using NpgsqlTypes;
using System.Data;

namespace Npgsql.TypeHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-uuid.html
    /// </remarks>
    [TypeMapping("uuid", NpgsqlDbType.Uuid, DbType.Guid, typeof(Guid))]
    internal class UuidHandler : TypeHandler<Guid>,
        ISimpleTypeReader<Guid>, ISimpleTypeWriter,
        ISimpleTypeReader<string>
    {
        public Guid Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            buf.Ensure(16);
            var a = buf.ReadInt32();
            var b = buf.ReadInt16();
            var c = buf.ReadInt16();
            var d = new byte[8];
            buf.ReadBytes(d, 0, 8);
            return new Guid(a, b, c, d);
        }

        string ISimpleTypeReader<string>.Read(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            return Read(buf, len, fieldDescription).ToString();
        }

        #region Write

        public int ValidateAndGetLength(object value, NpgsqlParameter parameter)
        {
            var asString = value as string;
            if (value is string)
            {
                var converted = Guid.Parse(asString);
                if (parameter == null)
                {
                    throw CreateConversionButNoParamException(value.GetType());
                }
                parameter.ConvertedValue = converted;
            }
            else if (!(value is Guid))
            {
                throw CreateConversionException(value.GetType());
            }
            return 16;
        }

        public void Write(object value, NpgsqlBuffer buf, NpgsqlParameter parameter)
        {
            if (parameter != null && parameter.ConvertedValue != null) {
                value = parameter.ConvertedValue;
            }

            var bytes = ((Guid)value).ToByteArray();

            buf.WriteInt32(BitConverter.ToInt32(bytes, 0));
            buf.WriteInt16(BitConverter.ToInt16(bytes, 4));
            buf.WriteInt16(BitConverter.ToInt16(bytes, 6));
            buf.WriteBytes(bytes, 8, 8);
        }

        #endregion
    }
}
