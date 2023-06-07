using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Npgsql.Internal.TypeHandlers;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using NpgsqlTypes;

namespace Npgsql.Internal.TypeMapping;

public interface IUserEnumTypeMapping : IUserTypeMapping
{
    INpgsqlNameTranslator NameTranslator { get; }
}

sealed class UserEnumTypeMapping<TEnum> : IUserEnumTypeMapping
    where TEnum : struct, Enum
{
    public string PgTypeName { get; }
    public Type ClrType => typeof(TEnum);
    public INpgsqlNameTranslator NameTranslator { get; }

    readonly Dictionary<TEnum, string> _enumToLabel;
    readonly Dictionary<string, TEnum> _labelToEnum;

    public UserEnumTypeMapping(string pgTypeName, INpgsqlNameTranslator nameTranslator)
    {
        (PgTypeName, NameTranslator) = (pgTypeName, nameTranslator);
        var fields = typeof(TEnum).GetFields(BindingFlags.Static | BindingFlags.Public);
        _enumToLabel = new(fields.Length, new EnumEqualityComparer());
        _labelToEnum = new(fields.Length);
        
        foreach (var field in fields)
        {
            var attribute = (PgNameAttribute?)field.GetCustomAttribute(typeof(PgNameAttribute), false);
            var enumName = attribute is null
                ? nameTranslator.TranslateMemberName(field.Name)
                : attribute.PgName;
            var enumValue = (TEnum)field.GetValue(null)!;

            _enumToLabel[enumValue] = enumName;
            _labelToEnum[enumName] = enumValue;
        }
    }

    public NpgsqlTypeHandler CreateHandler(PostgresType postgresType, NpgsqlConnector connector)
        => new EnumHandler<TEnum>((PostgresEnumType)postgresType, _enumToLabel, _labelToEnum);

    sealed class EnumEqualityComparer : IEqualityComparer<TEnum>
    {
        readonly Func<TEnum, int> _getHashCode;
        readonly Func<TEnum, TEnum, bool> _equals;
        public EnumEqualityComparer()
        {
            var enumUnderlyingType = typeof(TEnum).GetEnumUnderlyingType();

            if (enumUnderlyingType == typeof(byte))
            {
                _getHashCode = static e => Unsafe.As<TEnum, byte>(ref e).GetHashCode();
                _equals = static (x, y) => Unsafe.As<TEnum, byte>(ref x) == Unsafe.As<TEnum, byte>(ref y);
            }
            else if (enumUnderlyingType == typeof(sbyte))
            {
                _getHashCode = static e => Unsafe.As<TEnum, sbyte>(ref e).GetHashCode();
                _equals = static (x, y) => Unsafe.As<TEnum, sbyte>(ref x) == Unsafe.As<TEnum, sbyte>(ref y);
            }
            else if (enumUnderlyingType == typeof(short))
            {
                _getHashCode = static e => Unsafe.As<TEnum, short>(ref e).GetHashCode();
                _equals = static (x, y) => Unsafe.As<TEnum, short>(ref x) == Unsafe.As<TEnum, short>(ref y);
            }
            else if (enumUnderlyingType == typeof(ushort))
            {
                _getHashCode = static e => Unsafe.As<TEnum, ushort>(ref e).GetHashCode();
                _equals = static (x, y) => Unsafe.As<TEnum, ushort>(ref x) == Unsafe.As<TEnum, ushort>(ref y);
            }
            else if (enumUnderlyingType == typeof(int))
            {
                _getHashCode = static e => Unsafe.As<TEnum, int>(ref e).GetHashCode();
                _equals = static (x, y) => Unsafe.As<TEnum, int>(ref x) == Unsafe.As<TEnum, int>(ref y);
            }
            else if (enumUnderlyingType == typeof(uint))
            {
                _getHashCode = static e => Unsafe.As<TEnum, uint>(ref e).GetHashCode();
                _equals = static (x, y) => Unsafe.As<TEnum, uint>(ref x) == Unsafe.As<TEnum, uint>(ref y);
            }
            else if (enumUnderlyingType == typeof(long))
            {
                _getHashCode = static e => Unsafe.As<TEnum, long>(ref e).GetHashCode();
                _equals = static (x, y) => Unsafe.As<TEnum, long>(ref x) == Unsafe.As<TEnum, long>(ref y);
            }
            else if (enumUnderlyingType == typeof(ulong))
            {
                _getHashCode = static e => Unsafe.As<TEnum, ulong>(ref e).GetHashCode();
                _equals = static (x, y) => Unsafe.As<TEnum, ulong>(ref x) == Unsafe.As<TEnum, ulong>(ref y);
            }
            else
            {
                _getHashCode = static e => e.GetHashCode();
                _equals = static (x, y) => EqualityComparer<TEnum>.Default.Equals(x, y);
            }
        }

        public bool Equals(TEnum x, TEnum y) => _equals(x, y);

        public int GetHashCode(TEnum obj) => _getHashCode(obj);
    }
}