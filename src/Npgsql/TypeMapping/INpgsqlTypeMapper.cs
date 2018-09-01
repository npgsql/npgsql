#region License

// The PostgreSQL License
//
// Copyright (C) 2018 The Npgsql Development Team
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.

#endregion

using System.Collections.Generic;
using JetBrains.Annotations;
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
    [PublicAPI]
    public interface INpgsqlTypeMapper
    {
        /// <summary>
        /// The default name translator to convert CLR type names and member names.
        /// </summary>
        [NotNull]
        INpgsqlNameTranslator DefaultNameTranslator { get; }

        /// <summary>
        /// Enumerates all mappings currently set up on this type mapper.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        IEnumerable<NpgsqlTypeMapping> Mappings { get; }

        /// <summary>
        /// Adds a new type mapping to this mapper, overwriting any existing mapping in the process.
        /// </summary>
        [NotNull]
        INpgsqlTypeMapper AddMapping([NotNull] NpgsqlTypeMapping mapping);

        /// <summary>
        /// Removes an existing mapping from this mapper. Attempts to read or write this type
        /// after removal will result in an exception.
        /// </summary>
        /// <param name="pgTypeName">A PostgreSQL type name for the type in the database.</param>
        bool RemoveMapping([NotNull] string pgTypeName);

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
        /// If null, the name translator given in <paramref name="nameTranslator"/>will be used.
        /// </param>
        /// <param name="nameTranslator">
        /// A component which will be used to translate CLR names (e.g. SomeClass) into database names (e.g. some_class).
        /// Defaults to <see cref="NpgsqlSnakeCaseNameTranslator"/>
        /// </param>
        /// <typeparam name="TEnum">The .NET enum type to be mapped</typeparam>
        INpgsqlTypeMapper MapEnum<TEnum>(string pgName = null, INpgsqlNameTranslator nameTranslator = null) where TEnum : struct;

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
        bool UnmapEnum<TEnum>(string pgName = null, INpgsqlNameTranslator nameTranslator = null) where TEnum : struct;

        /// <summary>
        /// Maps a CLR type to a PostgreSQL composite type.
        /// </summary>
        /// <remarks>
        /// CLR fields and properties by string to PostgreSQL enum labels.
        /// The translation strategy can be controlled by the <paramref name="nameTranslator"/> parameter,
        /// which defaults to <see cref="NpgsqlSnakeCaseNameTranslator"/>.
        /// You can also use the <see cref="PgNameAttribute"/> on your members to manually specify a PostgreSQL enum label.
        /// If there is a discrepancy between the .NET and database labels while a composite is read or written,
        /// an exception will be raised.
        /// </remarks>
        /// <param name="pgName">
        /// A PostgreSQL type name for the corresponding enum type in the database.
        /// If null, the name translator given in <paramref name="nameTranslator"/>will be used.
        /// </param>
        /// <param name="nameTranslator">
        /// A component which will be used to translate CLR names (e.g. SomeClass) into database names (e.g. some_class).
        /// Defaults to <see cref="NpgsqlSnakeCaseNameTranslator"/>
        /// </param>
        /// <typeparam name="T">The .NET type to be mapped</typeparam>
        [NotNull]
        INpgsqlTypeMapper MapComposite<T>(
            [CanBeNull] string pgName = null,
            [CanBeNull] INpgsqlNameTranslator nameTranslator = null) where T : new();

        /// <summary>
        /// Removes an existing enum mapping.
        /// </summary>
        /// <param name="pgName">
        /// A PostgreSQL type name for the corresponding composite type in the database.
        /// If null, the name translator given in <paramref name="nameTranslator"/>will be used.
        /// </param>
        /// <param name="nameTranslator">
        /// A component which will be used to translate CLR names (e.g. SomeClass) into database names (e.g. some_class).
        /// Defaults to <see cref="NpgsqlSnakeCaseNameTranslator"/>
        /// </param>
        bool UnmapComposite<T>(
            [CanBeNull] string pgName = null,
            [CanBeNull] INpgsqlNameTranslator nameTranslator = null) where T : new();

        /// <summary>
        /// Resets all mapping changes performed on this type mapper and reverts it to its original, starting state.
        /// </summary>
        void Reset();
    }
}
