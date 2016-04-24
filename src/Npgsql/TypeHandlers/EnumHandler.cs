﻿#region License
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
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;

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
        IEnumHandler Clone();
    }

    internal class EnumHandler<TEnum> : SimpleTypeHandler<TEnum>, IEnumHandler where TEnum : struct
    {
        readonly INpgsqlNameTranslator _nameTranslator;
        readonly Dictionary<TEnum, string> _enumToLabel;
        readonly Dictionary<string, TEnum> _labelToEnum;

        public Type EnumType => typeof(TEnum);

        public EnumHandler(string pgName, INpgsqlNameTranslator nameTranslator)
        {
            Contract.Requires(typeof(TEnum).GetTypeInfo().IsEnum, "EnumHandler instantiated for non-enum type");

            NpgsqlDbType = NpgsqlDbType.Enum;
            PgName = pgName;
            _nameTranslator = nameTranslator;

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

        public override TEnum Read(ReadBuffer buf, int len, FieldDescription fieldDescription)
        {
            var str = buf.ReadString(len);
            TEnum value;
            var success = _labelToEnum.TryGetValue(str, out value);

            if (!success)
                throw new SafeReadException(new InvalidCastException($"Received enum value '{str}' from database which wasn't found on enum {typeof (TEnum)}"));

            return value;
        }

        public override int ValidateAndGetLength(object value, NpgsqlParameter parameter)
        {
            if (!(value is TEnum))
                throw CreateConversionException(value.GetType());

            string str;
            var asEnum = (TEnum)value;
            if (!_enumToLabel.TryGetValue(asEnum, out str))
                throw new InvalidCastException($"Can't write value {asEnum} as enum {typeof (TEnum)}");

            return Encoding.UTF8.GetByteCount(str);
        }

        public override void Write(object value, WriteBuffer buf, NpgsqlParameter parameter)
        {
            string str;
            var asEnum = (TEnum)value;
            if (!_enumToLabel.TryGetValue(asEnum, out str))
                throw new InvalidCastException($"Can't write value {asEnum} as enum {typeof (TEnum)}");

            buf.WriteString(str);
        }

        public IEnumHandler Clone()
        {
            return new EnumHandler<TEnum>(PgName, _nameTranslator);
        }
    }
}
