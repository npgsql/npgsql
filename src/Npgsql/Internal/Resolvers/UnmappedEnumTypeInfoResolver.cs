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
class UnmappedEnumTypeInfoResolver : DynamicTypeInfoResolver
{
    protected override DynamicMappingCollection? GetMappings(Type? type, DataTypeName dataTypeName, PgSerializerOptions options)
    {
        if (type is null || !IsTypeOrNullableOfType(type, static type => type.IsEnum, out var matchedType) || options.DatabaseInfo.GetPostgresTypeByName(dataTypeName) is not PostgresEnumType)
            return null;

        return CreateCollection().AddMapping(matchedType, dataTypeName, static (options, mapping, _) =>
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

                return mapping.CreateInfo(options, (PgConverter)Activator.CreateInstance(typeof(EnumConverter<>).MakeGenericType(mapping.Type),
                    enumToLabel, labelToEnum,
                    options.TextEncoding)!);
            });
    }
}

[RequiresUnreferencedCode("Unmapped enum resolver may perform reflection on types with fields that were trimmed if not referenced directly.")]
[RequiresDynamicCode("Unmapped enums need to construct a generic converter for a statically unknown enum type")]
sealed class UnmappedEnumArrayTypeInfoResolver : UnmappedEnumTypeInfoResolver
{
    protected override DynamicMappingCollection? GetMappings(Type? type, DataTypeName dataTypeName, PgSerializerOptions options)
        => type is not null && IsArrayLikeType(type, out var elementType) && IsArrayDataTypeName(dataTypeName, options, out var elementDataTypeName)
            ? base.GetMappings(elementType, elementDataTypeName, options)?.AddArrayMapping(elementType, elementDataTypeName)
            : null;
}
