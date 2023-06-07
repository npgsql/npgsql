using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using Npgsql.BackendMessages;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;

namespace Npgsql.Internal.TypeHandlers;

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

sealed partial class EnumHandler<TEnum> : NpgsqlSimpleTypeHandler<TEnum>, IEnumHandler where TEnum : struct, Enum
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

        if (_labelToEnum.TryGetValue(str, out var value) is false)
        {
            ThrowHelper.ThrowInvalidCastException("Received enum value '{0}' from database which wasn't found on enum {1}", str, typeof(TEnum));
        }
        return value;
    }

    #endregion

    #region Write

    public override int ValidateAndGetLength(TEnum value, NpgsqlParameter? parameter)
    {
        if (_enumToLabel.TryGetValue(value, out var str) is false)
        {
            ThrowHelper.ThrowInvalidCastException("Can't write value {0} as enum {1}", value, typeof(TEnum));
        }

        return Encoding.UTF8.GetByteCount(str);
    }

    public override void Write(TEnum value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
    {
        if (_enumToLabel.TryGetValue(value, out var str) is false)
        {
            ThrowHelper.ThrowInvalidCastException("Can't write value {0} as enum {1}", value, typeof(TEnum));
        }

        buf.WriteString(str);
    }

    #endregion
}