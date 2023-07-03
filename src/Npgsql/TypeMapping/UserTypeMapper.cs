using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Npgsql.Internal;
using Npgsql.Internal.Converters;
using Npgsql.NameTranslation;
using Npgsql.PostgresTypes;
using NpgsqlTypes;

namespace Npgsql.TypeMapping;

public abstract class UserTypeMapping
{
    public string PgTypeName { get; }
    public Type ClrType { get; }

    internal UserTypeMapping(string pgTypeName, Type type)
        => (PgTypeName, ClrType) = (pgTypeName, type);

    internal abstract void Build(TypeInfoMappingCollection mappings);
}

sealed class UserTypeMapper
{
    readonly List<UserTypeMapping> _mappings;
    public IList<UserTypeMapping> Items => _mappings;

    public INpgsqlNameTranslator DefaultNameTranslator { get; set; } = new NpgsqlSnakeCaseNameTranslator();

    UserTypeMapper(IEnumerable<UserTypeMapping> mappings) => _mappings = new List<UserTypeMapping>(mappings);
    public UserTypeMapper() => _mappings = new();

    public UserTypeMapper Clone() => new(_mappings) { DefaultNameTranslator = DefaultNameTranslator };

    public UserTypeMapper MapEnum<TEnum>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
        where TEnum : struct, Enum
    {
        Unmap(typeof(TEnum), out var resolvedName, pgName, nameTranslator);
        Items.Add(new EnumMapping<TEnum>(resolvedName, nameTranslator ?? DefaultNameTranslator));
        return this;
    }

    public bool UnmapEnum<TEnum>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
        where TEnum : struct, Enum
        => Unmap(typeof(TEnum), out _, pgName, nameTranslator ?? DefaultNameTranslator);

    // TODO these overloads will accept some form of context that houses the converter for the composite.
    public UserTypeMapper MapComposite<T>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null) where T : class
    {
        Unmap(typeof(T), out var resolvedName, pgName, nameTranslator);
        Items.Add(new CompositeMapping<T>(resolvedName, nameTranslator ?? DefaultNameTranslator));
        return this;
    }

    public UserTypeMapper MapStructComposite<T>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null) where T : struct
    {
        Unmap(typeof(T), out var resolvedName, pgName, nameTranslator);
        Items.Add(new StructCompositeMapping<T>(resolvedName, nameTranslator ?? DefaultNameTranslator));
        return this;
    }

    [RequiresUnreferencedCode("Composite type mapping currently isn't trimming-safe.")]
    public UserTypeMapper MapComposite(Type clrType, string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
    {
        if (clrType.IsConstructedGenericType && clrType.GetGenericTypeDefinition() == typeof(Nullable<>))
            throw new ArgumentException("Cannot map nullable.", nameof(clrType));

        var openMethod = typeof(UserTypeMapper).GetMethod(
            clrType.IsValueType ? nameof(MapStructComposite) : nameof(MapComposite),
            new[] { typeof(string), typeof(INpgsqlNameTranslator) })!;

        var method = openMethod.MakeGenericMethod(clrType);

        // TODO build a reflection based composite converter.

        method.Invoke(this, new object?[] { pgName, nameTranslator });

        return this;
    }

    public bool UnmapComposite<T>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null) where T : class
        => UnmapComposite(typeof(T), pgName, nameTranslator);

    public bool UnmapStructComposite<T>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null) where T : struct
        => UnmapComposite(typeof(T), pgName, nameTranslator);

    public bool UnmapComposite(Type clrType, string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
        => Unmap(clrType, out _, pgName, nameTranslator);

    bool Unmap(Type type, out string resolvedName, string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
    {
        if (pgName != null && pgName.Trim() == "")
            throw new ArgumentException("pgName can't be empty", nameof(pgName));

        nameTranslator ??= DefaultNameTranslator;
        resolvedName = pgName ??= GetPgName(type, nameTranslator);

        UserTypeMapping? toRemove = null;
        foreach (var item in _mappings)
            if (item.PgTypeName == pgName)
                toRemove = item;

        return toRemove is not null && _mappings.Remove(toRemove);
    }

    static string GetPgName(Type type, INpgsqlNameTranslator nameTranslator)
        => type.GetCustomAttribute<PgNameAttribute>()?.PgName
           ?? nameTranslator.TranslateTypeName(type.Name);

    public IPgTypeInfoResolver Build()
    {
        var infoMappings = new TypeInfoMappingCollection();
        foreach (var mapping in _mappings)
            mapping.Build(infoMappings);

        return new UserMappingResolver(infoMappings);
    }

    sealed class UserMappingResolver : IPgTypeInfoResolver
    {
        readonly TypeInfoMappingCollection _mappings;
        public UserMappingResolver(TypeInfoMappingCollection mappings) => _mappings = mappings;
        PgTypeInfo? IPgTypeInfoResolver.GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
            => _mappings.Find(type, dataTypeName, options);
    }

    sealed class CompositeMapping<T> : UserTypeMapping where T : class
    {
        readonly INpgsqlNameTranslator _nameTranslator;

        public CompositeMapping(string pgTypeName, INpgsqlNameTranslator nameTranslator)
            : base(pgTypeName, typeof(T))
        {
            _nameTranslator = nameTranslator;
        }

        internal override void Build(TypeInfoMappingCollection mappings)
        {
            mappings.AddType<T>(PgTypeName, static (options, mapping, resolvedDataTypeName) =>
            {
                throw new NotImplementedException();
            });
            mappings.AddArrayType<T>(PgTypeName);
        }
    }

    sealed class StructCompositeMapping<T> : UserTypeMapping where T : struct
    {
        readonly INpgsqlNameTranslator _nameTranslator;

        public StructCompositeMapping(string pgTypeName, INpgsqlNameTranslator nameTranslator)
            : base(pgTypeName, typeof(T))
        {
            _nameTranslator = nameTranslator;
        }

        internal override void Build(TypeInfoMappingCollection mappings)
        {
            mappings.AddStructType<T>(PgTypeName, static (options, mapping, resolvedDataTypeName) =>
            {
                throw new NotImplementedException();
            });
            mappings.AddStructArrayType<T>(PgTypeName);
        }
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

        internal override void Build(TypeInfoMappingCollection mappings)
        {
            mappings.AddStructType<TEnum>(PgTypeName, (options, mapping, _) =>
                mapping.CreateInfo(options, new EnumConverter<TEnum>(_enumToLabel, _labelToEnum, options.TextEncoding), preferredFormat: DataFormat.Text), isDefault: true);

            mappings.AddStructArrayType<TEnum>(PgTypeName);
        }
    }
}
