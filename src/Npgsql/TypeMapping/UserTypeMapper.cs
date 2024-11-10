using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Npgsql.Internal;
using Npgsql.Internal.Composites;
using Npgsql.Internal.Converters;
using Npgsql.Internal.Postgres;
using Npgsql.NameTranslation;
using Npgsql.PostgresTypes;
using NpgsqlTypes;

namespace Npgsql.TypeMapping;

/// <summary>
/// The base class for user type mappings.
/// </summary>
public abstract class UserTypeMapping
{
    /// <summary>
    /// The name of the PostgreSQL type that this mapping is for.
    /// </summary>
    public string PgTypeName { get; }
    /// <summary>
    /// The CLR type that this mapping is for.
    /// </summary>
    public Type ClrType { get; }

    internal UserTypeMapping(string pgTypeName, Type type)
        => (PgTypeName, ClrType) = (pgTypeName, type);

    internal abstract void AddMapping(TypeInfoMappingCollection mappings);
    internal abstract void AddArrayMapping(TypeInfoMappingCollection mappings);
}

sealed class UserTypeMapper : PgTypeInfoResolverFactory
{
    readonly List<UserTypeMapping> _mappings;
    public IList<UserTypeMapping> Items => _mappings;

    INpgsqlNameTranslator _defaultNameTranslator = NpgsqlSnakeCaseNameTranslator.Instance;
    public INpgsqlNameTranslator DefaultNameTranslator
    {
        get => _defaultNameTranslator;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            _defaultNameTranslator = value;
        }
    }

    UserTypeMapper(IEnumerable<UserTypeMapping> mappings) => _mappings = [..mappings];
    public UserTypeMapper() => _mappings = [];

    public UserTypeMapper Clone() => new(_mappings) { DefaultNameTranslator = DefaultNameTranslator };

    public UserTypeMapper MapEnum<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] TEnum>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
        where TEnum : struct, Enum
    {
        Unmap(typeof(TEnum), out var resolvedName, pgName, nameTranslator);
        Items.Add(new EnumMapping<TEnum>(resolvedName, nameTranslator ?? DefaultNameTranslator));
        return this;
    }

    public bool UnmapEnum<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] TEnum>(
        string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
        where TEnum : struct, Enum
        => Unmap(typeof(TEnum), out _, pgName, nameTranslator ?? DefaultNameTranslator);

    [UnconditionalSuppressMessage("Trimming", "IL2111", Justification = "MapEnum<TEnum> TEnum has less DAM annotations than clrType.")]
    [RequiresDynamicCode("Calling MapEnum with a Type can require creating new generic types or methods. This may not work when AOT compiling.")]
    public UserTypeMapper MapEnum([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]Type clrType, string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
    {
        if (!clrType.IsEnum || !clrType.IsValueType)
            throw new ArgumentException("Type must be a concrete Enum", nameof(clrType));

        var openMethod = typeof(UserTypeMapper).GetMethod(nameof(MapEnum), [typeof(string), typeof(INpgsqlNameTranslator)])!;
        var method = openMethod.MakeGenericMethod(clrType);
        method.Invoke(this, [pgName, nameTranslator]);
        return this;
    }

    public bool UnmapEnum([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)]Type clrType,string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
    {
        if (!clrType.IsEnum || !clrType.IsValueType)
            throw new ArgumentException("Type must be a concrete Enum", nameof(clrType));

        return Unmap(clrType, out _, pgName, nameTranslator ?? DefaultNameTranslator);
    }

    [RequiresDynamicCode("Mapping composite types involves serializing arbitrary types which can require creating new generic types or methods. This is currently unsupported with NativeAOT, vote on issue #5303 if this is important to you.")]
    public UserTypeMapper MapComposite<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] T>(
        string? pgName = null, INpgsqlNameTranslator? nameTranslator = null) where T : class
    {
        Unmap(typeof(T), out var resolvedName, pgName, nameTranslator);
        Items.Add(new CompositeMapping<T>(resolvedName, nameTranslator ?? DefaultNameTranslator));
        return this;
    }

    [RequiresDynamicCode("Mapping composite types involves serializing arbitrary types which can require creating new generic types or methods. This is currently unsupported with NativeAOT, vote on issue #5303 if this is important to you.")]
    public UserTypeMapper MapStructComposite<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] T>(
        string? pgName = null, INpgsqlNameTranslator? nameTranslator = null) where T : struct
    {
        Unmap(typeof(T), out var resolvedName, pgName, nameTranslator);
        Items.Add(new StructCompositeMapping<T>(resolvedName, nameTranslator ?? DefaultNameTranslator));
        return this;
    }

    [UnconditionalSuppressMessage("Trimming", "IL2111", Justification = "MapStructComposite and MapComposite have identical DAM annotations to clrType.")]
    [RequiresDynamicCode("Mapping composite types involves serializing arbitrary types which can require creating new generic types or methods. This is currently unsupported with NativeAOT, vote on issue #5303 if this is important to you.")]
    public UserTypeMapper MapComposite([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicFields)]
        Type clrType, string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
    {
        if (clrType.IsConstructedGenericType && clrType.GetGenericTypeDefinition() == typeof(Nullable<>))
            throw new ArgumentException("Cannot map nullable.", nameof(clrType));

        var openMethod = typeof(UserTypeMapper).GetMethod(
            clrType.IsValueType ? nameof(MapStructComposite) : nameof(MapComposite),
            [typeof(string), typeof(INpgsqlNameTranslator)])!;

        var method = openMethod.MakeGenericMethod(clrType);

        method.Invoke(this, [pgName, nameTranslator]);

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

    public override IPgTypeInfoResolver CreateResolver() => new Resolver([.._mappings]);
    public override IPgTypeInfoResolver CreateArrayResolver() => new ArrayResolver([.._mappings]);

    class Resolver(List<UserTypeMapping> userTypeMappings) : IPgTypeInfoResolver
    {
        protected readonly List<UserTypeMapping> _userTypeMappings = userTypeMappings;
        TypeInfoMappingCollection? _mappings;
        protected TypeInfoMappingCollection Mappings => _mappings ??= AddMappings(new());

        PgTypeInfo? IPgTypeInfoResolver.GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
            => Mappings.Find(type, dataTypeName, options);

        TypeInfoMappingCollection AddMappings(TypeInfoMappingCollection mappings)
        {
            foreach (var userTypeMapping in _userTypeMappings)
                userTypeMapping.AddMapping(mappings);

            return mappings;
        }
    }

    sealed class ArrayResolver(List<UserTypeMapping> userTypeMappings) : Resolver(userTypeMappings), IPgTypeInfoResolver
    {
        TypeInfoMappingCollection? _mappings;
        new TypeInfoMappingCollection Mappings => _mappings ??= AddMappings(new(base.Mappings));

        PgTypeInfo? IPgTypeInfoResolver.GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
            => Mappings.Find(type, dataTypeName, options);

        TypeInfoMappingCollection AddMappings(TypeInfoMappingCollection mappings)
        {
            foreach (var userTypeMapping in _userTypeMappings)
                userTypeMapping.AddArrayMapping(mappings);

            return mappings;
        }
    }

    [RequiresDynamicCode("Mapping composite types involves serializing arbitrary types which can require creating new generic types or methods. This is currently unsupported with NativeAOT, vote on issue #5303 if this is important to you.")]
    sealed class CompositeMapping<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields |
                                    DynamicallyAccessedMemberTypes.PublicProperties)]
        T>(string pgTypeName, INpgsqlNameTranslator nameTranslator) : UserTypeMapping(pgTypeName, typeof(T))
        where T : class
    {
        internal override void AddMapping(TypeInfoMappingCollection mappings)
            => mappings.AddType<T>(PgTypeName, (options, mapping, _) =>
            {
                var pgType = mapping.GetPgType(options);
                if (pgType is not PostgresCompositeType compositeType)
                    throw new InvalidOperationException("Composite mapping must be to a composite type");

                return mapping.CreateInfo(options, new CompositeConverter<T>(
                    ReflectionCompositeInfoFactory.CreateCompositeInfo<T>(compositeType, nameTranslator, options)));
            }, isDefault: true);

        internal override void AddArrayMapping(TypeInfoMappingCollection mappings) => mappings.AddArrayType<T>(PgTypeName);
    }

    [RequiresDynamicCode("Mapping composite types involves serializing arbitrary types which can require creating new generic types or methods. This is currently unsupported with NativeAOT, vote on issue #5303 if this is important to you.")]
    sealed class StructCompositeMapping<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields |
                                    DynamicallyAccessedMemberTypes.PublicProperties)]
        T>(string pgTypeName, INpgsqlNameTranslator nameTranslator) : UserTypeMapping(pgTypeName, typeof(T))
        where T : struct
    {
        internal override void AddMapping(TypeInfoMappingCollection mappings)
            => mappings.AddStructType<T>(PgTypeName, (options, mapping, requiresDataTypeName) =>
            {
                var pgType = mapping.GetPgType(options);
                if (pgType is not PostgresCompositeType compositeType)
                    throw new InvalidOperationException("Composite mapping must be to a composite type");

                return mapping.CreateInfo(options, new CompositeConverter<T>(
                    ReflectionCompositeInfoFactory.CreateCompositeInfo<T>(compositeType, nameTranslator, options)));
            }, isDefault: true);

        internal override void AddArrayMapping(TypeInfoMappingCollection mappings) => mappings.AddStructArrayType<T>(PgTypeName);
    }

    internal abstract class EnumMapping(
        string pgTypeName,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] Type enumClrType,
        INpgsqlNameTranslator nameTranslator)
        : UserTypeMapping(pgTypeName, enumClrType)
    {
        internal INpgsqlNameTranslator NameTranslator { get; } = nameTranslator;
    }

    sealed class EnumMapping<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] TEnum> : EnumMapping
        where TEnum : struct, Enum
    {
        readonly Dictionary<TEnum, string> _enumToLabel = new();
        readonly Dictionary<string, TEnum> _labelToEnum = new();

        public EnumMapping(string pgTypeName, INpgsqlNameTranslator nameTranslator)
            : base(pgTypeName, typeof(TEnum), nameTranslator)
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

        internal override void AddMapping(TypeInfoMappingCollection mappings)
            => mappings.AddStructType<TEnum>(PgTypeName, (options, mapping, _) =>
                mapping.CreateInfo(options, new EnumConverter<TEnum>(_enumToLabel, _labelToEnum, options.TextEncoding), preferredFormat: DataFormat.Text), isDefault: true);

        internal override void AddArrayMapping(TypeInfoMappingCollection mappings) => mappings.AddStructArrayType<TEnum>(PgTypeName);
    }
}

