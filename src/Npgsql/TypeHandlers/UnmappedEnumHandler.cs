#region License
// The PostgreSQL License
//
// Copyright (C) 2018 The Npgsql Development Team
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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Npgsql.BackendMessages;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers
{
    class UnmappedEnumHandler : TextHandler
    {
        readonly INpgsqlNameTranslator _nameTranslator;

        readonly Dictionary<Enum, string> _enumToLabel = new Dictionary<Enum, string>();
        readonly Dictionary<string, Enum> _labelToEnum = new Dictionary<string, Enum>();

        [CanBeNull]
        Type _resolvedType;

        internal UnmappedEnumHandler(INpgsqlNameTranslator nameTranslator, NpgsqlConnection connection)
            : base(connection)
        {
            _nameTranslator = nameTranslator;
        }

        #region Read

        protected internal override async ValueTask<TAny> Read<TAny>(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null)
        {
            var s = await base.Read(buf, len, async, fieldDescription);
            if (typeof(TAny) == typeof(string))
                return (TAny)(object)s;

            if (_resolvedType != typeof(TAny))
                Map(typeof(TAny));

            if (!_labelToEnum.TryGetValue(s, out var value))
                throw new NpgsqlSafeReadException(new InvalidCastException($"Received enum value '{s}' from database which wasn't found on enum {typeof(TAny)}"));

            // TODO: Avoid boxing
            return (TAny)(object)value;
        }

        public override ValueTask<string> Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null)
            => base.Read(buf, len, async, fieldDescription);

        #endregion

        #region Write

        protected internal override int ValidateObjectAndGetLength(object value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => value == null || value is DBNull
                ? -1
                : ValidateAndGetLength(value, ref lengthCache, parameter);

        protected internal override int ValidateAndGetLength<TAny>(TAny value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => ValidateAndGetLength(value, ref lengthCache, parameter);

        int ValidateAndGetLength(object value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
        {
            var type = value.GetType();
            if (type == typeof(string))
                return base.ValidateAndGetLength((string)(object)value, ref lengthCache, parameter);
            if (_resolvedType != type)
                Map(type);

            // TODO: Avoid boxing
            return _enumToLabel.TryGetValue((Enum)(object)value, out var str)
                ? base.ValidateAndGetLength(str, ref lengthCache, parameter)
                : throw new InvalidCastException($"Can't write value {value} as enum {type}");
        }

        // TODO: This boxes the enum (again)
        protected override Task WriteWithLength<TAny>(TAny value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => WriteObjectWithLength(value, buf, lengthCache, parameter, async);

        protected internal override Task WriteObjectWithLength(object value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
        {
            if (value == null || value is DBNull)
                return WriteWithLengthInternal<DBNull>(null, buf, lengthCache, parameter, async);

            if (buf.WriteSpaceLeft < 4)
                return WriteWithLengthLong();

            buf.WriteInt32(ValidateAndGetLength(value, ref lengthCache, parameter));
            return Write(value, buf, lengthCache, parameter, async);

            async Task WriteWithLengthLong()
            {
                await buf.Flush(async);
                buf.WriteInt32(ValidateAndGetLength(value, ref lengthCache, parameter));
                await Write(value, buf, lengthCache, parameter, async);
            }
        }

        internal Task Write(object value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
        {
            var type = value.GetType();
            if (type == typeof(string))
                return base.Write((string)(object)value, buf, lengthCache, parameter, async);
            if (_resolvedType != type)
                Map(type);

            // TODO: Avoid boxing
            if (!_enumToLabel.TryGetValue((Enum)(object)value, out var str))
                throw new InvalidCastException($"Can't write value {value} as enum {type}");
            return base.Write(str, buf, lengthCache, parameter, async);
        }

        #endregion

        #region Misc

        void Map(Type type)
        {
            Debug.Assert(_resolvedType != type);

            _enumToLabel.Clear();
            _labelToEnum.Clear();

            foreach (var field in type.GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                var attribute = (PgNameAttribute)field.GetCustomAttributes(typeof(PgNameAttribute), false).FirstOrDefault();
                var enumName = attribute == null
                    ? _nameTranslator.TranslateMemberName(field.Name)
                    : attribute.PgName;
                var enumValue = (Enum)field.GetValue(null);
                _enumToLabel[enumValue] = enumName;
                _labelToEnum[enumName] = enumValue;
            }

            _resolvedType = type;
        }

        #endregion
    }

    class UnmappedEnumTypeHandlerFactory : NpgsqlTypeHandlerFactory<string>, IEnumTypeHandlerFactory
    {
        internal UnmappedEnumTypeHandlerFactory(INpgsqlNameTranslator nameTranslator)
        {
            NameTranslator = nameTranslator;
        }

        protected override NpgsqlTypeHandler<string> Create(NpgsqlConnection conn)
            => new UnmappedEnumHandler(NameTranslator, conn);

        public INpgsqlNameTranslator NameTranslator { get; }
    }
}
