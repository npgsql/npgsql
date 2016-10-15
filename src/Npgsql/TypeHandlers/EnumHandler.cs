#region License
// The PostgreSQL License
//
// Copyright (C) 2016 The Npgsql Development Team
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

    interface IEnumHandlerFactory
    {
        IEnumHandler Create(PostgresType backendType);
    }

    class EnumHandler<TEnum> : SimpleTypeHandler<TEnum>, IEnumHandler where TEnum : struct
    {
        readonly Dictionary<TEnum, string> _enumToLabel;
        readonly Dictionary<string, TEnum> _labelToEnum;

        public Type EnumType => typeof(TEnum);

        #region Construction

        internal EnumHandler(PostgresType postgresType, INpgsqlNameTranslator nameTranslator)
            : base(postgresType)
        {
            Debug.Assert(typeof(TEnum).GetTypeInfo().IsEnum, "EnumHandler instantiated for non-enum type");
            _enumToLabel = new Dictionary<TEnum, string>();
            _labelToEnum = new Dictionary<string, TEnum>();
            GenerateMappings(nameTranslator, _enumToLabel, _labelToEnum);
        }

        internal EnumHandler(PostgresType postgresType, Dictionary<TEnum, string> enumToLabel, Dictionary<string, TEnum> labelToEnum)
            : base(postgresType)
        {
            Debug.Assert(typeof(TEnum).GetTypeInfo().IsEnum, "EnumHandler instantiated for non-enum type");
            _enumToLabel = enumToLabel;
            _labelToEnum = labelToEnum;
        }

        static void GenerateMappings(INpgsqlNameTranslator nameTranslator, Dictionary<TEnum, string> enumToLabel, Dictionary<string, TEnum> labelToEnum)
        {
            foreach (var field in typeof(TEnum).GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                var attribute = (PgNameAttribute)field.GetCustomAttributes(typeof(PgNameAttribute), false).FirstOrDefault();
                var enumName = attribute == null
                    ? nameTranslator.TranslateMemberName(field.Name)
                    : attribute.PgName;
                var enumValue = (Enum)field.GetValue(null);
                enumToLabel[(TEnum)(object)enumValue] = enumName;
                labelToEnum[enumName] = (TEnum)(object)enumValue;
            }
        }

        #endregion

        #region Read

        public override TEnum Read(ReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            var str = buf.ReadString(len);
            TEnum value;
            var success = _labelToEnum.TryGetValue(str, out value);

            if (!success)
                throw new SafeReadException(new InvalidCastException($"Received enum value '{str}' from database which wasn't found on enum {typeof(TEnum)}"));

            return value;
        }

        #endregion

        #region Write

        public override int ValidateAndGetLength(object value, NpgsqlParameter parameter = null)
        {
            if (!(value is TEnum))
                throw CreateConversionException(value.GetType());

            string str;
            var asEnum = (TEnum)value;
            if (!_enumToLabel.TryGetValue(asEnum, out str))
                throw new InvalidCastException($"Can't write value {asEnum} as enum {typeof(TEnum)}");

            return Encoding.UTF8.GetByteCount(str);
        }

        protected override void Write(object value, WriteBuffer buf, NpgsqlParameter parameter = null)
        {
            string str;
            var asEnum = (TEnum)value;
            if (!_enumToLabel.TryGetValue(asEnum, out str))
                throw new InvalidCastException($"Can't write value {asEnum} as enum {typeof(TEnum)}");

            buf.WriteString(str);
        }

        #endregion

        internal class Factory : IEnumHandlerFactory
        {
            readonly Dictionary<TEnum, string> _enumToLabel;
            readonly Dictionary<string, TEnum> _labelToEnum;

            internal Factory(INpgsqlNameTranslator nameTranslator)
            {
                _enumToLabel = new Dictionary<TEnum, string>();
                _labelToEnum = new Dictionary<string, TEnum>();
                GenerateMappings(nameTranslator, _enumToLabel, _labelToEnum);
            }

            public IEnumHandler Create(PostgresType backendType)
                => new EnumHandler<TEnum>(backendType, _enumToLabel, _labelToEnum);
        }
    }
}
