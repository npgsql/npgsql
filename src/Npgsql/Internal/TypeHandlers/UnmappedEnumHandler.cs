using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using NpgsqlTypes;

namespace Npgsql.Internal.TypeHandlers
{
    class UnmappedEnumHandler : TextHandler
    {
        readonly INpgsqlNameTranslator _nameTranslator;

        readonly Dictionary<Enum, string> _enumToLabel = new();
        readonly Dictionary<string, Enum> _labelToEnum = new();

        Type? _resolvedType;

        internal UnmappedEnumHandler(PostgresEnumType pgType, INpgsqlNameTranslator nameTranslator, Encoding encoding)
            : base(pgType, encoding)
            => _nameTranslator = nameTranslator;

        #region Read

        protected internal override async ValueTask<TAny> ReadCustom<TAny>(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null)
        {
            var s = await base.Read(buf, len, async, fieldDescription);
            if (typeof(TAny) == typeof(string))
                return (TAny)(object)s;

            if (_resolvedType != typeof(TAny))
                Map(typeof(TAny));

            if (!_labelToEnum.TryGetValue(s, out var value))
                throw new InvalidCastException($"Received enum value '{s}' from database which wasn't found on enum {typeof(TAny)}");

            // TODO: Avoid boxing
            return (TAny)(object)value;
        }

        public override ValueTask<string> Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null)
            => base.Read(buf, len, async, fieldDescription);

        #endregion

        #region Write

        public override int ValidateObjectAndGetLength(object? value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => value is null || value is DBNull
                ? 0
                : ValidateAndGetLength(value, ref lengthCache, parameter);

        protected internal override int ValidateAndGetLengthCustom<TAny>(TAny value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLength(value!, ref lengthCache, parameter);

        [UnconditionalSuppressMessage("Unmapped enums currently aren't trimming-safe.", "IL2072")]
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
        protected override Task WriteWithLengthCustom<TAny>(TAny value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken)
            => WriteObjectWithLength(value!, buf, lengthCache, parameter, async, cancellationToken);

        public override Task WriteObjectWithLength(object? value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
        {
            if (value is null || value is DBNull)
                return WriteWithLength(DBNull.Value, buf, lengthCache, parameter, async, cancellationToken);

            if (buf.WriteSpaceLeft < 4)
                return WriteWithLengthLong(value, buf, lengthCache, parameter, async, cancellationToken);

            buf.WriteInt32(ValidateAndGetLength(value, ref lengthCache, parameter));
            return Write(value, buf, lengthCache, parameter, async, cancellationToken);

            async Task WriteWithLengthLong(object value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken)
            {
                await buf.Flush(async, cancellationToken);
                buf.WriteInt32(ValidateAndGetLength(value, ref lengthCache, parameter));
                await Write(value, buf, lengthCache, parameter, async, cancellationToken);
            }
        }

        [UnconditionalSuppressMessage("Unmapped enums currently aren't trimming-safe.", "IL2072")]
        internal Task Write(object value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
        {
            var type = value.GetType();
            if (type == typeof(string))
                return base.Write((string)value, buf, lengthCache, parameter, async, cancellationToken);
            if (_resolvedType != type)
                Map(type);

            // TODO: Avoid boxing
            if (!_enumToLabel.TryGetValue((Enum)value, out var str))
                throw new InvalidCastException($"Can't write value {value} as enum {type}");
            return base.Write(str, buf, lengthCache, parameter, async, cancellationToken);
        }

        #endregion

        #region Misc

        void Map([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] Type type)
        {
            Debug.Assert(_resolvedType != type);

            _enumToLabel.Clear();
            _labelToEnum.Clear();

            foreach (var field in type.GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                var attribute = (PgNameAttribute?)field.GetCustomAttributes(typeof(PgNameAttribute), false).FirstOrDefault();
                var enumName = attribute?.PgName ?? _nameTranslator.TranslateMemberName(field.Name);
                var enumValue = (Enum)field.GetValue(null)!;

                _enumToLabel[enumValue] = enumName;
                _labelToEnum[enumName] = enumValue;
            }

            _resolvedType = type;
        }

        #endregion
    }
}
