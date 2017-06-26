#region License
// The PostgreSQL License
//
// Copyright (C) 2017 The Npgsql Development Team
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
#endregion

using Npgsql.BackendMessages;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Npgsql.PostgresTypes;
using Npgsql.TypeMapping;

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

    class EnumHandler<TEnum> : SimpleTypeHandler<TEnum>, IEnumHandler where TEnum : struct
    {
        readonly Dictionary<TEnum, string> _enumToLabel;
        readonly Dictionary<string, TEnum> _labelToEnum;

        public Type EnumType => typeof(TEnum);

        #region Construction

        internal EnumHandler(INpgsqlNameTranslator nameTranslator)
        {
            Debug.Assert(typeof(TEnum).GetTypeInfo().IsEnum, "EnumHandler instantiated for non-enum type");
            _enumToLabel = new Dictionary<TEnum, string>();
            _labelToEnum = new Dictionary<string, TEnum>();

            foreach (var field in typeof(TEnum).GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                var attribute = (PgNameAttribute)field.GetCustomAttributes(typeof(PgNameAttribute), false).FirstOrDefault();
                var enumName = attribute == null
                    ? nameTranslator.TranslateMemberName(field.Name)
                    : attribute.PgName;
                var enumValue = (Enum)field.GetValue(null);
                _enumToLabel[(TEnum)(object)enumValue] = enumName;
                _labelToEnum[enumName] = (TEnum)(object)enumValue;
            }
        }

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

        protected override int ValidateAndGetLength(object value, NpgsqlParameter parameter = null)
        {
            if (!(value is TEnum))
                throw CreateConversionException(value.GetType());

            var asEnum = (TEnum)value;
            if (!_enumToLabel.TryGetValue(asEnum, out var str))
                throw new InvalidCastException($"Can't write value {asEnum} as enum {typeof(TEnum)}");

            return Encoding.UTF8.GetByteCount(str);
        }

        protected override void Write(object value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter = null)
        {
            string str;
            var asEnum = (TEnum)value;
            if (!_enumToLabel.TryGetValue(asEnum, out str))
                throw new InvalidCastException($"Can't write value {asEnum} as enum {typeof(TEnum)}");

            buf.WriteString(str);
        }

        #endregion
    }

    class EnumTypeHandlerFactory<TEnum> : TypeHandlerFactory where TEnum : struct
    {
        readonly Dictionary<TEnum, string> _enumToLabel = new Dictionary<TEnum, string>();
        readonly Dictionary<string, TEnum> _labelToEnum = new Dictionary<string, TEnum>();

        internal EnumTypeHandlerFactory(INpgsqlNameTranslator nameTranslator)
        {
            foreach (var field in typeof(TEnum).GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                var attribute = (PgNameAttribute)field.GetCustomAttributes(typeof(PgNameAttribute), false).FirstOrDefault();
                var enumName = attribute == null
                    ? nameTranslator.TranslateMemberName(field.Name)
                    : attribute.PgName;
                var enumValue = (Enum)field.GetValue(null);
                _enumToLabel[(TEnum)(object)enumValue] = enumName;
                _labelToEnum[enumName] = (TEnum)(object)enumValue;
            }
        }

        protected override TypeHandler Create(NpgsqlConnection conn)
            => new EnumHandler<TEnum>(_enumToLabel, _labelToEnum);
    }
}
