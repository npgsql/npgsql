using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Npgsql.Internal.Converters;
using Npgsql.Internal.Postgres;
using Npgsql.PostgresTypes;
using NpgsqlTypes;

namespace Npgsql.Internal.ResolverFactories;

[RequiresUnreferencedCode("The use of unmapped enums, ranges or multiranges requires reflection usage which is incompatible with trimming.")]
[RequiresDynamicCode("The use of unmapped enums, ranges or multiranges requires dynamic code usage which is incompatible with NativeAOT.")]
sealed partial class UnmappedTypeInfoResolverFactory : PgTypeInfoResolverFactory
{
    public override IPgTypeInfoResolver CreateResolver() => new EnumResolver();
    public override IPgTypeInfoResolver CreateArrayResolver() => new EnumArrayResolver();

    [RequiresUnreferencedCode("The use of unmapped enums, ranges or multiranges requires reflection usage which is incompatible with trimming.")]
    [RequiresDynamicCode("The use of unmapped enums, ranges or multiranges requires dynamic code usage which is incompatible with NativeAOT.")]
    class EnumResolver : DynamicTypeInfoResolver
    {
        protected override DynamicMappingCollection? GetMappings(Type? type, DataTypeName dataTypeName, PgSerializerOptions options)
        {
            if (type is null || !IsTypeOrNullableOfType(type, static type => type.IsEnum, out var matchedType) || options.DatabaseInfo.GetPostgresType(dataTypeName) is not PostgresEnumType)
                return null;

            return CreateCollection().AddMapping(matchedType, dataTypeName, static (options, mapping, _) =>
                {
                    var enumToLabel = new Dictionary<Enum, string>();
                    var labelToEnum = new Dictionary<string, Enum>();
                    foreach (var field in mapping.Type.GetFields(BindingFlags.Static | BindingFlags.Public))
                    {
                        var attribute = (PgNameAttribute?)field.GetCustomAttribute(typeof(PgNameAttribute), false);
                        var enumName = attribute?.PgName ?? options.DefaultNameTranslator.TranslateMemberName(field.Name);
                        var enumValue = (Enum)field.GetValue(null)!;

                        enumToLabel[enumValue] = enumName;
                        labelToEnum[enumName] = enumValue;
                    }

                    return mapping.CreateInfo(options, (PgConverter)Activator.CreateInstance(typeof(EnumConverter<>).MakeGenericType(mapping.Type),
                        enumToLabel, labelToEnum,
                        options.TextEncoding)!);
                });
        }
    }

    [RequiresUnreferencedCode("The use of unmapped enums, ranges or multiranges requires reflection usage which is incompatible with trimming.")]
    [RequiresDynamicCode("The use of unmapped enums, ranges or multiranges requires dynamic code usage which is incompatible with NativeAOT.")]
    sealed class EnumArrayResolver : EnumResolver
    {
        protected override DynamicMappingCollection? GetMappings(Type? type, DataTypeName dataTypeName, PgSerializerOptions options)
            => type is not null && IsArrayLikeType(type, out var elementType) && IsArrayDataTypeName(dataTypeName, options, out var elementDataTypeName)
                ? base.GetMappings(elementType, elementDataTypeName, options)?.AddArrayMapping(elementType, elementDataTypeName)
                : null;
    }
}
