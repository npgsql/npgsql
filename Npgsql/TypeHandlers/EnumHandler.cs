using Npgsql.Messages;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Npgsql.TypeHandlers
{
    internal class EnumHandler<TEnum> : TypeHandler<TEnum>, ISimpleTypeReader<TEnum>, ISimpleTypeWriter
    {
        TypeHandlerRegistry.EnumInfo EnumInfo { get; set; }

        public EnumHandler(TypeHandlerRegistry.EnumInfo enumInfo)
        {
            NpgsqlDbType = NpgsqlDbType.Enum;
            EnumInfo = enumInfo;
        }

        public TEnum Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            var str = buf.ReadStringSimple(len);
            var enumValue = EnumInfo[str];
            if (enumValue == null)
                return default(TEnum); // TODO: Should we throw an exception?
            return (TEnum)(object)enumValue;
        }

        public int GetLength(object value)
        {
            var str = EnumInfo[(Enum)value];
            if (str == null)
                throw new InvalidCastException("Invalid enum value (" + value + ") of type " + typeof(TEnum));
            return Encoding.UTF8.GetByteCount(str);
        }

        public void Write(object value, NpgsqlBuffer buf)
        {
            var str = EnumInfo[(Enum)value];
            buf.WriteStringSimple(str);
        }
    }
}
