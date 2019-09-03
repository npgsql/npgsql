﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers
{
    class UnmappedEnumHandler : TextHandler
    {
        readonly INpgsqlNameTranslator _nameTranslator;

        readonly Dictionary<Enum, string> _enumToLabel = new Dictionary<Enum, string>();
        readonly Dictionary<string, Enum> _labelToEnum = new Dictionary<string, Enum>();

        Type? _resolvedType;

        internal UnmappedEnumHandler(PostgresType pgType, INpgsqlNameTranslator nameTranslator, NpgsqlConnection connection)
            : base(pgType, connection)
        {
            _nameTranslator = nameTranslator;
        }

        #region Read

        protected internal override async ValueTask<TAny> Read<TAny>(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null)
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

        public override ValueTask<string> Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null)
            => base.Read(buf, len, async, fieldDescription);

        #endregion

        #region Write

        protected internal override int ValidateObjectAndGetLength(object value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => value == null || value is DBNull
                ? -1
                : ValidateAndGetLength(value, ref lengthCache, parameter);

        protected internal override int ValidateAndGetLength<TAny>(TAny value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLength(value!, ref lengthCache, parameter);

        int ValidateAndGetLength(object value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
        {
            var type = value.GetType();
            if (type == typeof(string))
                return base.ValidateAndGetLength((string)value, ref lengthCache, parameter);
            if (_resolvedType != type)
                Map(type);

            // TODO: Avoid boxing
            return _enumToLabel.TryGetValue((Enum)value, out var str)
                ? base.ValidateAndGetLength(str, ref lengthCache, parameter)
                : throw new InvalidCastException($"Can't write value {value} as enum {type}");
        }

        // TODO: This boxes the enum (again)
        protected override Task WriteWithLength<TAny>(TAny value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async)
            => WriteObjectWithLength(value!, buf, lengthCache, parameter, async);

        protected internal override Task WriteObjectWithLength(object value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async)
        {
            if (value is DBNull)
                return WriteWithLengthInternal(DBNull.Value, buf, lengthCache, parameter, async);

            if (buf.WriteSpaceLeft < 4)
                return WriteWithLengthLong();

            buf.WriteInt32(ValidateAndGetLength(value, ref lengthCache, parameter));
            return Write(value, buf, lengthCache, parameter, async);

            async Task WriteWithLengthLong()
            {
                await buf.Flush(async);
                buf.WriteInt32(ValidateAndGetLength(value!, ref lengthCache, parameter));
                await Write(value!, buf, lengthCache, parameter, async);
            }
        }

        internal Task Write(object value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async)
        {
            var type = value.GetType();
            if (type == typeof(string))
                return base.Write((string)value, buf, lengthCache, parameter, async);
            if (_resolvedType != type)
                Map(type);

            // TODO: Avoid boxing
            if (!_enumToLabel.TryGetValue((Enum)value, out var str))
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
                var enumName = attribute?.PgName ?? _nameTranslator.TranslateMemberName(field.Name);
                var enumValue = (Enum)field.GetValue(null)!;

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

        public override NpgsqlTypeHandler<string> Create(PostgresType pgType, NpgsqlConnection conn)
            => new UnmappedEnumHandler(pgType, NameTranslator, conn);

        public INpgsqlNameTranslator NameTranslator { get; }
    }
}
