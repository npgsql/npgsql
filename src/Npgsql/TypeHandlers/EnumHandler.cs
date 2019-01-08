using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Npgsql.BackendMessages;
using Npgsql.TypeHandling;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers
{
    /// <summary>
    /// Interface implemented by all concrete handlers which handle enums
    /// </summary>
    interface IEnumHandler
    {
        /// <summary>
        /// The CLR enum type mapped to the PostgreSQL enum
        /// </summary>
        Type EnumType { get; }
    }

    class EnumHandler<TEnum> : NpgsqlSimpleTypeHandler<TEnum>, IEnumHandler where TEnum : struct, Enum
    {
        readonly Dictionary<TEnum, string> _enumToLabel;
        readonly Dictionary<string, TEnum> _labelToEnum;

        public Type EnumType => typeof(TEnum);

        #region Construction

        internal EnumHandler(Dictionary<TEnum, string> enumToLabel, Dictionary<string, TEnum> labelToEnum)
        {
            Debug.Assert(typeof(TEnum).GetTypeInfo().IsEnum, "EnumHandler instantiated for non-enum type");
            _enumToLabel = enumToLabel;
            _labelToEnum = labelToEnum;
        }

        #endregion

        #region Read

        public override TEnum Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            var str = buf.ReadString(len);
            var success = _labelToEnum.TryGetValue(str, out var value);

            if (!success)
                throw new NpgsqlSafeReadException(new InvalidCastException($"Received enum value '{str}' from database which wasn't found on enum {typeof(TEnum)}"));

            return value;
        }

        #endregion

        #region Write

        public override int ValidateAndGetLength(TEnum value, NpgsqlParameter parameter = null)
            => _enumToLabel.TryGetValue(value, out var str)
                ? Encoding.UTF8.GetByteCount(str)
                : throw new InvalidCastException($"Can't write value {value} as enum {typeof(TEnum)}");

        public override void Write(TEnum value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter = null)
        {
            if (!_enumToLabel.TryGetValue(value, out var str))
                throw new InvalidCastException($"Can't write value {value} as enum {typeof(TEnum)}");
            buf.WriteString(str);
        }

        #endregion
    }


    /// <summary>
    /// Interface implemented by all enum handler factories.
    /// Used to expose the name translator for those reflecting enum mappings (e.g. EF Core).
    /// </summary>
    public interface IEnumTypeHandlerFactory
    {
        /// <summary>
        /// The name translator used for this enum.
        /// </summary>
        INpgsqlNameTranslator NameTranslator { get; }
    }

    class EnumTypeHandlerFactory<TEnum> : NpgsqlTypeHandlerFactory<TEnum>, IEnumTypeHandlerFactory
        where TEnum : struct, Enum
    {
        readonly Dictionary<TEnum, string> _enumToLabel = new Dictionary<TEnum, string>();
        readonly Dictionary<string, TEnum> _labelToEnum = new Dictionary<string, TEnum>();

        internal EnumTypeHandlerFactory(INpgsqlNameTranslator nameTranslator)
        {
            foreach (var field in typeof(TEnum).GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                NameTranslator = nameTranslator;

                var attribute = (PgNameAttribute)field.GetCustomAttributes(typeof(PgNameAttribute), false).FirstOrDefault();
                var enumName = attribute == null
                    ? nameTranslator.TranslateMemberName(field.Name)
                    : attribute.PgName;
                var enumValue = (Enum)field.GetValue(null);
                _enumToLabel[(TEnum)(object)enumValue] = enumName;
                _labelToEnum[enumName] = (TEnum)(object)enumValue;
            }
        }

        protected override NpgsqlTypeHandler<TEnum> Create(NpgsqlConnection conn)
            => new EnumHandler<TEnum>(_enumToLabel, _labelToEnum);

        public INpgsqlNameTranslator NameTranslator { get; }
    }
}
