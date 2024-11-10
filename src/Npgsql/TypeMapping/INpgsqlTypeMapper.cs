using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Nodes;
using Npgsql.Internal;
using Npgsql.NameTranslation;
using NpgsqlTypes;

// ReSharper disable UnusedMember.Global
namespace Npgsql.TypeMapping;

/// <summary>
/// A type mapper, managing how to read and write CLR values to PostgreSQL data types.
/// </summary>
/// <remarks>
/// The preferred way to manage type mappings is on <see cref="NpgsqlDataSourceBuilder" />. An alternative, but discouraged, method, is to
/// manage them globally via <see cref="NpgsqlConnection.GlobalTypeMapper"/>).
/// </remarks>
public interface INpgsqlTypeMapper
{
    /// <summary>
    /// The default name translator to convert CLR type names and member names. Defaults to <see cref="NpgsqlSnakeCaseNameTranslator"/>.
    /// </summary>
    INpgsqlNameTranslator DefaultNameTranslator { get; set; }

    /// <summary>
    /// Maps a CLR enum to a PostgreSQL enum type.
    /// </summary>
    /// <remarks>
    /// CLR enum labels are mapped by name to PostgreSQL enum labels.
    /// The translation strategy can be controlled by the <paramref name="nameTranslator"/> parameter,
    /// which defaults to <see cref="NpgsqlSnakeCaseNameTranslator"/>.
    /// You can also use the <see cref="PgNameAttribute"/> on your enum fields to manually specify a PostgreSQL enum label.
    /// If there is a discrepancy between the .NET and database labels while an enum is read or written,
    /// an exception will be raised.
    /// </remarks>
    /// <param name="pgName">
    /// A PostgreSQL type name for the corresponding enum type in the database.
    /// If null, the name translator given in <paramref name="nameTranslator"/> will be used.
    /// </param>
    /// <param name="nameTranslator">
    /// A component which will be used to translate CLR names (e.g. SomeClass) into database names (e.g. some_class).
    /// Defaults to <see cref="DefaultNameTranslator" />.
    /// </param>
    /// <typeparam name="TEnum">The .NET enum type to be mapped</typeparam>
    INpgsqlTypeMapper MapEnum<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] TEnum>(
        string? pgName = null,
        INpgsqlNameTranslator? nameTranslator = null)
        where TEnum : struct, Enum;

    /// <summary>
    /// Removes an existing enum mapping.
    /// </summary>
    /// <param name="pgName">
    /// A PostgreSQL type name for the corresponding enum type in the database.
    /// If null, the name translator given in <paramref name="nameTranslator"/> will be used.
    /// </param>
    /// <param name="nameTranslator">
    /// A component which will be used to translate CLR names (e.g. SomeClass) into database names (e.g. some_class).
    /// Defaults to <see cref="DefaultNameTranslator" />.
    /// </param>
    bool UnmapEnum<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] TEnum>(
        string? pgName = null,
        INpgsqlNameTranslator? nameTranslator = null)
        where TEnum : struct, Enum;

    /// <summary>
    /// Maps a CLR enum to a PostgreSQL enum type.
    /// </summary>
    /// <remarks>
    /// CLR enum labels are mapped by name to PostgreSQL enum labels.
    /// The translation strategy can be controlled by the <paramref name="nameTranslator"/> parameter,
    /// which defaults to <see cref="NpgsqlSnakeCaseNameTranslator"/>.
    /// You can also use the <see cref="PgNameAttribute"/> on your enum fields to manually specify a PostgreSQL enum label.
    /// If there is a discrepancy between the .NET and database labels while an enum is read or written,
    /// an exception will be raised.
    /// </remarks>
    /// <param name="clrType">The .NET enum type to be mapped</param>
    /// <param name="pgName">
    /// A PostgreSQL type name for the corresponding enum type in the database.
    /// If null, the name translator given in <paramref name="nameTranslator"/> will be used.
    /// </param>
    /// <param name="nameTranslator">
    /// A component which will be used to translate CLR names (e.g. SomeClass) into database names (e.g. some_class).
    /// Defaults to <see cref="DefaultNameTranslator" />.
    /// </param>
    [RequiresDynamicCode("Calling MapEnum with a Type can require creating new generic types or methods. This may not work when AOT compiling.")]
    INpgsqlTypeMapper MapEnum(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]Type clrType,
        string? pgName = null,
        INpgsqlNameTranslator? nameTranslator = null);

    /// <summary>
    /// Removes an existing enum mapping.
    /// </summary>
    /// <param name="clrType">The .NET enum type to be mapped</param>
    /// <param name="pgName">
    /// A PostgreSQL type name for the corresponding enum type in the database.
    /// If null, the name translator given in <paramref name="nameTranslator"/> will be used.
    /// </param>
    /// <param name="nameTranslator">
    /// A component which will be used to translate CLR names (e.g. SomeClass) into database names (e.g. some_class).
    /// Defaults to <see cref="DefaultNameTranslator" />.
    /// </param>
    bool UnmapEnum(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]Type clrType,
        string? pgName = null,
        INpgsqlNameTranslator? nameTranslator = null);

    /// <summary>
    /// Maps a CLR type to a PostgreSQL composite type.
    /// </summary>
    /// <remarks>
    /// CLR fields and properties by string to PostgreSQL names.
    /// The translation strategy can be controlled by the <paramref name="nameTranslator"/> parameter,
    /// which defaults to <see cref="NpgsqlSnakeCaseNameTranslator"/>.
    /// You can also use the <see cref="PgNameAttribute"/> on your members to manually specify a PostgreSQL name.
    /// If there is a discrepancy between the .NET type and database type while a composite is read or written,
    /// an exception will be raised.
    /// </remarks>
    /// <param name="pgName">
    /// A PostgreSQL type name for the corresponding composite type in the database.
    /// If null, the name translator given in <paramref name="nameTranslator"/> will be used.
    /// </param>
    /// <param name="nameTranslator">
    /// A component which will be used to translate CLR names (e.g. SomeClass) into database names (e.g. some_class).
    /// Defaults to <see cref="DefaultNameTranslator" />.
    /// </param>
    /// <typeparam name="T">The .NET type to be mapped</typeparam>
    [RequiresDynamicCode("Mapping composite types involves serializing arbitrary types which can require creating new generic types or methods. This is currently unsupported with NativeAOT, vote on issue #5303 if this is important to you.")]
    INpgsqlTypeMapper MapComposite<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicFields)] T>(
        string? pgName = null,
        INpgsqlNameTranslator? nameTranslator = null);

    /// <summary>
    /// Removes an existing composite mapping.
    /// </summary>
    /// <param name="pgName">
    /// A PostgreSQL type name for the corresponding composite type in the database.
    /// If null, the name translator given in <paramref name="nameTranslator"/> will be used.
    /// </param>
    /// <param name="nameTranslator">
    /// A component which will be used to translate CLR names (e.g. SomeClass) into database names (e.g. some_class).
    /// Defaults to <see cref="NpgsqlSnakeCaseNameTranslator"/>
    /// </param>
    [RequiresDynamicCode("Mapping composite types involves serializing arbitrary types which can require creating new generic types or methods. This is currently unsupported with NativeAOT, vote on issue #5303 if this is important to you.")]
    bool UnmapComposite<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicFields)] T>(
        string? pgName = null,
        INpgsqlNameTranslator? nameTranslator = null);

    /// <summary>
    /// Maps a CLR type to a composite type.
    /// </summary>
    /// <remarks>
    /// Maps CLR fields and properties by string to PostgreSQL names.
    /// The translation strategy can be controlled by the <paramref name="nameTranslator"/> parameter,
    /// which defaults to <see cref="DefaultNameTranslator" />.
    /// If there is a discrepancy between the .NET type and database type while a composite is read or written,
    /// an exception will be raised.
    /// </remarks>
    /// <param name="clrType">The .NET type to be mapped.</param>
    /// <param name="pgName">
    /// A PostgreSQL type name for the corresponding composite type in the database.
    /// If null, the name translator given in <paramref name="nameTranslator"/> will be used.
    /// </param>
    /// <param name="nameTranslator">
    /// A component which will be used to translate CLR names (e.g. SomeClass) into database names (e.g. some_class).
    /// Defaults to <see cref="DefaultNameTranslator" />.
    /// </param>
    [RequiresDynamicCode("Mapping composite types involves serializing arbitrary types which can require creating new generic types or methods. This is currently unsupported with NativeAOT, vote on issue #5303 if this is important to you.")]
    INpgsqlTypeMapper MapComposite(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicFields)]  Type clrType,
        string? pgName = null,
        INpgsqlNameTranslator? nameTranslator = null);

    /// <summary>
    /// Removes an existing composite mapping.
    /// </summary>
    /// <param name="clrType">The .NET type to be unmapped.</param>
    /// <param name="pgName">
    /// A PostgreSQL type name for the corresponding composite type in the database.
    /// If null, the name translator given in <paramref name="nameTranslator"/> will be used.
    /// </param>
    /// <param name="nameTranslator">
    /// A component which will be used to translate CLR names (e.g. SomeClass) into database names (e.g. some_class).
    /// Defaults to <see cref="DefaultNameTranslator" />.
    /// </param>
    [RequiresDynamicCode("Mapping composite types involves serializing arbitrary types which can require creating new generic types or methods. This is currently unsupported with NativeAOT, vote on issue #5303 if this is important to you.")]
    bool UnmapComposite(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicFields)] Type clrType,
        string? pgName = null,
        INpgsqlNameTranslator? nameTranslator = null);

    /// <summary>
    /// Adds a type info resolver factory which can add or modify support for PostgreSQL types.
    /// Typically used by plugins.
    /// </summary>
    /// <param name="factory">The type resolver factory to be added.</param>
    void AddTypeInfoResolverFactory(PgTypeInfoResolverFactory factory);

    /// <summary>
    /// Configures the JSON serializer options used when reading and writing all System.Text.Json data.
    /// </summary>
    /// <param name="serializerOptions">Options to customize JSON serialization and deserialization.</param>
    /// <returns></returns>
    INpgsqlTypeMapper ConfigureJsonOptions(JsonSerializerOptions serializerOptions);

    /// <summary>
    /// Sets up dynamic System.Text.Json mappings. This allows mapping arbitrary .NET types to PostgreSQL <c>json</c> and <c>jsonb</c>
    /// types, as well as <see cref="JsonNode" /> and its derived types.
    /// </summary>
    /// <param name="jsonbClrTypes">
    /// A list of CLR types to map to PostgreSQL <c>jsonb</c> (no need to specify <see cref="NpgsqlDbType.Jsonb" />).
    /// </param>
    /// <param name="jsonClrTypes">
    /// A list of CLR types to map to PostgreSQL <c>json</c> (no need to specify <see cref="NpgsqlDbType.Json" />).
    /// </param>
    /// <remarks>
    /// Due to the dynamic nature of these mappings, they are not compatible with NativeAOT or trimming.
    /// </remarks>
    [RequiresUnreferencedCode("Json serializer may perform reflection on trimmed types.")]
    [RequiresDynamicCode(
        "Serializing arbitrary types to json can require creating new generic types or methods, which requires creating code at runtime. This may not work when AOT compiling.")]
    INpgsqlTypeMapper EnableDynamicJson(Type[]? jsonbClrTypes = null, Type[]? jsonClrTypes = null);

    /// <summary>
    /// Sets up mappings for the PostgreSQL <c>record</c> type as a .NET <see cref="ValueTuple" /> or <see cref="Tuple" />.
    /// </summary>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    [RequiresUnreferencedCode(
        "The mapping of PostgreSQL records as .NET tuples requires reflection usage which is incompatible with trimming.")]
    [RequiresDynamicCode(
        "The mapping of PostgreSQL records as .NET tuples requires dynamic code usage which is incompatible with NativeAOT.")]
    INpgsqlTypeMapper EnableRecordsAsTuples();

    /// <summary>
    /// Sets up mappings allowing the use of unmapped enum, range and multirange types.
    /// </summary>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    [RequiresUnreferencedCode(
        "The use of unmapped enums, ranges or multiranges requires reflection usage which is incompatible with trimming.")]
    [RequiresDynamicCode(
        "The use of unmapped enums, ranges or multiranges requires dynamic code usage which is incompatible with NativeAOT.")]
    INpgsqlTypeMapper EnableUnmappedTypes();

    /// <summary>
    /// Resets all mapping changes performed on this type mapper and reverts it to its original, starting state.
    /// </summary>
    void Reset();
}
