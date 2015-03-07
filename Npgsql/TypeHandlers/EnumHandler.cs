using Npgsql.BackendMessages;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Npgsql.TypeHandlers
{
    internal interface IEnumHandler { }
    internal class EnumHandler<TEnum> : TypeHandler<TEnum>, IEnumHandler,
        ISimpleTypeReader<TEnum>, ISimpleTypeWriter
        where TEnum : struct
    {
        readonly Dictionary<TEnum, string> _enumToLabel;
        readonly Dictionary<string, TEnum> _labelToEnum;

        public EnumHandler()
        {
            Contract.Requires(typeof(TEnum).IsEnum, "EnumHandler instantiated for non-enum type");

            // Reflect on our enum type to find any explicit mappings
            if (!typeof(TEnum).GetFields(BindingFlags.Static | BindingFlags.Public).Any(t => t.GetCustomAttributes(typeof(EnumLabelAttribute), false).Any())) {
                return;
            }

            _enumToLabel = new Dictionary<TEnum, string>();
            _labelToEnum = new Dictionary<string, TEnum>();
            foreach (var field in typeof(TEnum).GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                var attribute = (EnumLabelAttribute)field.GetCustomAttributes(typeof(EnumLabelAttribute), false).FirstOrDefault();
                var enumName = attribute == null ? field.Name : attribute.Label;
                var enumValue = (Enum)field.GetValue(null);
                _enumToLabel[(TEnum)(object)enumValue] = enumName;
                _labelToEnum[enumName] = (TEnum)(object)enumValue;
            }
        }

        public TEnum Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            var str = buf.ReadStringSimple(len);
            TEnum value;
            var success = _labelToEnum == null
                ? Enum.TryParse(str, out value)
                : _labelToEnum.TryGetValue(str, out value);

            if (!success)
                throw new SafeReadException(new InvalidCastException(String.Format("Received enum value '{0}' from database which wasn't found on enum {1}", str, typeof(TEnum))));

            return value;
        }

        public int ValidateAndGetLength(object value)
        {
            if (!(value is TEnum))
                throw new InvalidCastException(String.Format("Can't write type {0} as enum {1}", value.GetType(), typeof(TEnum)));

            string str;
            if (_enumToLabel == null)
            {
                str = value.ToString();
            }
            else
            {
                var asEnum = (TEnum)value;
                if (!_enumToLabel.TryGetValue(asEnum, out str)) {
                    throw new InvalidCastException(String.Format("Can't write value {0} as enum {1}", asEnum, typeof(TEnum)));
                }
            }

            return Encoding.UTF8.GetByteCount(str);
        }

        public void Write(object value, NpgsqlBuffer buf)
        {
            string str;
            if (_enumToLabel == null) {
                str = value.ToString();
            } else {
                var asEnum = (TEnum)value;
                if (!_enumToLabel.TryGetValue(asEnum, out str)) {
                    throw new InvalidCastException(String.Format("Can't write value {0} as enum {1}", asEnum, typeof(TEnum)));
                }
            }

            buf.WriteStringSimple(str);
        }

        internal EnumHandler<TEnum> Clone()
        {
            return new EnumHandler<TEnum>();
        }
    }
}
