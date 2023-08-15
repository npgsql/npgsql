using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Npgsql.Internal.Converters;
using Npgsql.Internal.Postgres;
using Npgsql.PostgresTypes;
using NpgsqlTypes;

namespace Npgsql.Internal.Resolvers;

[RequiresUnreferencedCode("Unmapped enum resolver may perform reflection on types with fields that were trimmed if not referenced directly.")]
[RequiresDynamicCode("Unmapped enums need to construct a generic converter for a statically unknown enum type.")]
class UnmappedEnumTypeInfoResolver : IPgTypeInfoResolver
{
    public PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
    {
        if (type is null || !IsEnum(type, out type))
            return null;

        if (dataTypeName is not { } pgName || options.TypeCatalog.GetPostgresTypeByName(pgName) is not PostgresEnumType)
            return null;

        // We have a valid unmapped enum lookup.
        var mappings = new TypeInfoMappingCollection();
        CreateEnumMapping(mappings, type, pgName);

        return mappings.Find(type, dataTypeName, options);
    }

    protected void CreateEnumMapping(TypeInfoMappingCollection mappings, Type type, DataTypeName dataTypeName)
        => AddStructTypeMethodInfo.MakeGenericMethod(type).Invoke(mappings, new object?[] {
            (string)dataTypeName,
            new TypeInfoFactory((options, mapping, _) =>
            {
                var enumToLabel = new Dictionary<Enum, string>();
                var labelToEnum = new Dictionary<string, Enum>();
                foreach (var field in mapping.Type.GetFields(BindingFlags.Static | BindingFlags.Public))
                {
                    var attribute = (PgNameAttribute?)field.GetCustomAttributes(typeof(PgNameAttribute), false).FirstOrDefault();
                    var enumName = attribute?.PgName ?? options.DefaultNameTranslator.TranslateMemberName(field.Name);
                    var enumValue = (Enum)field.GetValue(null)!;

                    enumToLabel[enumValue] = enumName;
                    labelToEnum[enumName] = enumValue;
                }

                return new PgTypeInfo(options, (PgConverter)Activator.CreateInstance(typeof(EnumConverter<>).MakeGenericType(mapping.Type),
                    enumToLabel, labelToEnum,
                    options.TextEncoding)!, new DataTypeName(mapping.DataTypeName));
            }),
            null});

    public static bool IsEnum(Type type, [NotNullWhen(true)]out Type? enumType)
        => (enumType = type).IsEnum || (enumType = Nullable.GetUnderlyingType(type!))?.IsEnum == true;

    static readonly MethodInfo AddStructTypeMethodInfo = typeof(TypeInfoMappingCollection).GetMethod(nameof(TypeInfoMappingCollection.AddStructType),
        new[] { typeof(string), typeof(TypeInfoFactory), typeof(Func<TypeInfoMapping, TypeInfoMapping>) }) ?? throw new NullReferenceException();
}

[RequiresUnreferencedCode("Unmapped enum resolver may perform reflection on types with fields that were trimmed if not referenced directly.")]
[RequiresDynamicCode("Unmapped enums need to construct a generic converter for a statically unknown enum type")]
sealed class UnmappedEnumTypeInfoArrayResolver : UnmappedEnumTypeInfoResolver, IPgTypeInfoResolver
{
    public new PgTypeInfo? GetTypeInfo(Type? type, DataTypeName? dataTypeName, PgSerializerOptions options)
    {
        if (type is null || !IsEnumArray(type, out var elementType))
            return null;

        if (dataTypeName is not { IsArray: true } pgName
            || options.TypeCatalog.GetPostgresTypeByName(pgName) is not PostgresArrayType { Element: PostgresEnumType pgEnumType })
            return null;

        // We have a valid unmapped enum array lookup.

        var mappings = new TypeInfoMappingCollection();
        CreateEnumMapping(mappings, elementType, pgEnumType.DataTypeName);
        AddStructArrayTypeMethodInfo.MakeGenericMethod(elementType).Invoke(mappings, new []{ (object)pgEnumType.DataTypeName.Value });

        return mappings.Find(type, dataTypeName, options);
    }

    static bool IsEnumArray(Type type, [NotNullWhen(true)]out Type? enumType)
        => TypeInfoMappingCollection.IsArrayType(type, out enumType) && IsEnum(enumType, out enumType);

    static readonly MethodInfo AddStructArrayTypeMethodInfo = typeof(TypeInfoMappingCollection)
        .GetMethod(nameof(TypeInfoMappingCollection.AddStructArrayType), new[] { typeof(string) }) ?? throw new NullReferenceException();
}
