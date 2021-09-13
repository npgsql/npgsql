using System;
using System.Diagnostics.CodeAnalysis;
using Npgsql.Internal.TypeHandling;
using Npgsql.NameTranslation;
using NpgsqlTypes;

// ReSharper disable UnusedMember.Global
namespace Npgsql.TypeMapping
{
    /// <summary>
    /// A type mapper, managing how to read and write CLR values to PostgreSQL data types.
    /// A type mapper exists for each connection, as well as a single global type mapper
    /// (accessible via <see cref="P:NpgsqlConnection.GlobalTypeMapper"/>).
    /// </summary>
    /// <remarks>
    /// </remarks>
    public interface INpgsqlTypeMapper
    {
        /// <summary>
        /// The default name translator to convert CLR type names and member names.
        /// </summary>
        INpgsqlNameTranslator DefaultNameTranslator { get; }

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
        /// Defaults to <see cref="NpgsqlSnakeCaseNameTranslator"/>
        /// </param>
        /// <typeparam name="TEnum">The .NET enum type to be mapped</typeparam>
        INpgsqlTypeMapper MapEnum<TEnum>(
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
        /// Defaults to <see cref="NpgsqlSnakeCaseNameTranslator"/>
        /// </param>
        bool UnmapEnum<TEnum>(
            string? pgName = null,
            INpgsqlNameTranslator? nameTranslator = null)
            where TEnum : struct, Enum;

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
        /// Defaults to <see cref="NpgsqlSnakeCaseNameTranslator"/>
        /// </param>
        /// <typeparam name="T">The .NET type to be mapped</typeparam>
        [RequiresUnreferencedCode("Composite type mapping currently isn't trimming-safe.")]
        INpgsqlTypeMapper MapComposite<T>(
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
        [RequiresUnreferencedCode("Composite type mapping currently isn't trimming-safe.")]
        bool UnmapComposite<T>(
            string? pgName = null,
            INpgsqlNameTranslator? nameTranslator = null);

        /// <summary>
        /// Maps a CLR type to a composite type.
        /// </summary>
        /// <remarks>
        /// Maps CLR fields and properties by string to PostgreSQL names.
        /// The translation strategy can be controlled by the <paramref name="nameTranslator"/> parameter,
        /// which defaults to <see cref="NpgsqlSnakeCaseNameTranslator"/>.
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
        /// Defaults to <see cref="NpgsqlSnakeCaseNameTranslator"/>
        /// </param>
        [RequiresUnreferencedCode("Composite type mapping currently isn't trimming-safe.")]
        INpgsqlTypeMapper MapComposite(
            Type clrType,
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
        /// Defaults to <see cref="NpgsqlSnakeCaseNameTranslator"/>
        /// </param>
        [RequiresUnreferencedCode("Composite type mapping currently isn't trimming-safe.")]
        bool UnmapComposite(
            Type clrType,
            string? pgName = null,
            INpgsqlNameTranslator? nameTranslator = null);

        /// <summary>
        /// Adds a type resolver factory, which produces resolvers that can add or modify support for PostgreSQL types.
        /// Typically used by plugins.
        /// </summary>
        /// <param name="resolverFactory">The type resolver factory to be added.</param>
        void AddTypeResolverFactory(TypeHandlerResolverFactory resolverFactory);

        /// <summary>
        /// Resets all mapping changes performed on this type mapper and reverts it to its original, starting state.
        /// </summary>
        void Reset();
    }
}
