using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Npgsql.BackendMessages;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using NpgsqlTypes;

namespace Npgsql.Internal.TypeHandlers
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

    partial class EnumHandler<TEnum> : NpgsqlSimpleTypeHandler<TEnum>, IEnumHandler where TEnum : struct, Enum
    {
        readonly Dictionary<TEnum, string> _enumToLabel;
        readonly Dictionary<string, TEnum> _labelToEnum;

        public Type EnumType => typeof(TEnum);

        #region Construction

        internal EnumHandler(PostgresEnumType postgresType, Dictionary<TEnum, string> enumToLabel, Dictionary<string, TEnum> labelToEnum)
            : base(postgresType)
        {
            Debug.Assert(typeof(TEnum).GetTypeInfo().IsEnum, "EnumHandler instantiated for non-enum type");
            _enumToLabel = enumToLabel;
            _labelToEnum = labelToEnum;
        }

        #endregion

        #region Read

        public override TEnum Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
        {
            var str = buf.ReadString(len);
            var success = _labelToEnum.TryGetValue(str, out var value);

            if (!success)
                throw new InvalidCastException($"Received enum value '{str}' from database which wasn't found on enum {typeof(TEnum)}");

            return value;
        }

        #endregion

        #region Write

        public override int ValidateAndGetLength(TEnum value, NpgsqlParameter? parameter)
            => _enumToLabel.TryGetValue(value, out var str)
                ? Encoding.UTF8.GetByteCount(str)
                : throw new InvalidCastException($"Can't write value {value} as enum {typeof(TEnum)}");

        public override void Write(TEnum value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
        {
            if (!_enumToLabel.TryGetValue(value, out var str))
                throw new InvalidCastException($"Can't write value {value} as enum {typeof(TEnum)}");
            buf.WriteString(str);
        }

        #endregion
    }
}
