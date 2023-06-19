using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Npgsql.Internal;
using Npgsql.NameTranslation;
using NpgsqlTypes;

namespace Npgsql.TypeMapping;

public abstract class UserTypeMapping
{
    public string PgTypeName { get; }
    public Type ClrType { get; }

    internal UserTypeMapping(string pgTypeName, Type type)
        => (PgTypeName, ClrType) = (pgTypeName, type);

    internal abstract PgTypeInfo Create(PgSerializerOptions options);
}

class UserTypeMapper
{
    readonly List<UserTypeMapping> _mappings = new();
    public IList<UserTypeMapping> Items => _mappings;

    public INpgsqlNameTranslator DefaultNameTranslator { get; set; } = new NpgsqlSnakeCaseNameTranslator();

    sealed class CompositeMapping<T> : UserTypeMapping
    {
        readonly INpgsqlNameTranslator _nameTranslator;

        public CompositeMapping(string pgTypeName, INpgsqlNameTranslator nameTranslator)
            : base(pgTypeName, typeof(T))
        {
            _nameTranslator = nameTranslator;
        }

        internal override PgTypeInfo Create(PgSerializerOptions options) => throw new NotImplementedException();
    }

    sealed class EnumMapping<TEnum> : UserTypeMapping
        where TEnum : struct, Enum
    {
        readonly Dictionary<TEnum, string> _enumToLabel = new();
        readonly Dictionary<string, TEnum> _labelToEnum = new();

        public EnumMapping(string pgTypeName, INpgsqlNameTranslator nameTranslator)
            : base(pgTypeName, typeof(TEnum))
        {
            foreach (var field in typeof(TEnum).GetFields(BindingFlags.Static | BindingFlags.Public))
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

        internal override PgTypeInfo Create(PgSerializerOptions options) => throw new NotImplementedException();
    }

    public UserTypeMapper MapEnum<TEnum>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
        where TEnum : struct, Enum
    {
        if (pgName != null && pgName.Trim() == "")
            throw new ArgumentException("pgName can't be empty", nameof(pgName));

        nameTranslator ??= DefaultNameTranslator;
        pgName ??= GetPgName(typeof(TEnum), nameTranslator);

        if (Items.FirstOrDefault(x => x.PgTypeName == pgName) is { } item)
            Items.Remove(item);

        Items.Add(new EnumMapping<TEnum>(pgName, nameTranslator));
        return this;
    }

    public bool UnmapEnum<TEnum>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
        where TEnum : struct, Enum
        => Unmap(typeof(TEnum), pgName, nameTranslator);

    [RequiresUnreferencedCode("Composite type mapping currently isn't trimming-safe.")]
    public UserTypeMapper MapComposite<T>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
    {
        if (pgName != null && pgName.Trim() == "")
            throw new ArgumentException("pgName can't be empty", nameof(pgName));

        nameTranslator ??= DefaultNameTranslator;
        pgName ??= GetPgName(typeof(T), nameTranslator);

        if (Items.FirstOrDefault(x => x.PgTypeName == pgName) is { } item)
            Items.Remove(item);

        Items.Add(new CompositeMapping<T>(pgName, nameTranslator));
        return this;
    }

    [RequiresUnreferencedCode("Composite type mapping currently isn't trimming-safe.")]
    public UserTypeMapper MapComposite(Type clrType, string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
    {
        var openMethod = typeof(UserTypeMapper).GetMethod(nameof(MapComposite), new[] { typeof(string), typeof(INpgsqlNameTranslator) })!;
        var method = openMethod.MakeGenericMethod(clrType);
        method.Invoke(this, new object?[] { pgName, nameTranslator });

        return this;
    }

    public bool UnmapComposite<T>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
        => UnmapComposite(typeof(T), pgName, nameTranslator);

    public bool UnmapComposite(Type clrType, string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
        => Unmap(clrType, pgName, nameTranslator);

    bool Unmap(Type type, string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
    {
        if (pgName != null && pgName.Trim() == "")
            throw new ArgumentException("pgName can't be empty", nameof(pgName));

        nameTranslator ??= DefaultNameTranslator;
        pgName ??= GetPgName(type, nameTranslator);

        return Items.Remove(Items.FirstOrDefault(x => x.PgTypeName == pgName)!);
    }

    static string GetPgName(Type type, INpgsqlNameTranslator nameTranslator)
        => type.GetCustomAttribute<PgNameAttribute>()?.PgName
           ?? nameTranslator.TranslateTypeName(type.Name);
}
