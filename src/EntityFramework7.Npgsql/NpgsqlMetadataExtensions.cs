// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using EntityFramework7.Npgsql.Metadata;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Utilities;

// ReSharper disable once CheckNamespace

namespace Microsoft.Data.Entity
{
    public static class NpgsqlMetadataExtensions
    {
        public static NpgsqlPropertyAnnotations Npgsql([NotNull] this Property property)
        {
            Check.NotNull(property, nameof(property));

            return new NpgsqlPropertyAnnotations(property);
        }

        public static INpgsqlPropertyAnnotations Npgsql([NotNull] this IProperty property)
        {
            Check.NotNull(property, nameof(property));

            return new ReadOnlyNpgsqlPropertyAnnotations(property);
        }

        public static NpgsqlEntityTypeAnnotations Npgsql([NotNull] this EntityType entityType)
        {
            Check.NotNull(entityType, nameof(entityType));

            return new NpgsqlEntityTypeAnnotations(entityType);
        }

        public static INpgsqlEntityTypeAnnotations Npgsql([NotNull] this IEntityType entityType)
        {
            Check.NotNull(entityType, nameof(entityType));

            return new ReadOnlyNpgsqlEntityTypeAnnotations(entityType);
        }

        public static NpgsqlKeyAnnotations Npgsql([NotNull] this Key key)
        {
            Check.NotNull(key, nameof(key));

            return new NpgsqlKeyAnnotations(key);
        }

        public static INpgsqlKeyAnnotations Npgsql([NotNull] this IKey key)
        {
            Check.NotNull(key, nameof(key));

            return new ReadOnlyNpgsqlKeyAnnotations(key);
        }

        public static NpgsqlIndexAnnotations Npgsql([NotNull] this Index index)
        {
            Check.NotNull(index, nameof(index));

            return new NpgsqlIndexAnnotations(index);
        }

        public static INpgsqlIndexAnnotations Npgsql([NotNull] this IIndex index)
        {
            Check.NotNull(index, nameof(index));

            return new ReadOnlyNpgsqlIndexAnnotations(index);
        }

        public static NpgsqlForeignKeyAnnotations Npgsql([NotNull] this ForeignKey foreignKey)
        {
            Check.NotNull(foreignKey, nameof(foreignKey));

            return new NpgsqlForeignKeyAnnotations(foreignKey);
        }

        public static INpgsqlForeignKeyAnnotations Npgsql([NotNull] this IForeignKey foreignKey)
        {
            Check.NotNull(foreignKey, nameof(foreignKey));

            return new ReadOnlyNpgsqlForeignKeyAnnotations(foreignKey);
        }

        public static NpgsqlModelAnnotations Npgsql([NotNull] this Model model)
        {
            Check.NotNull(model, nameof(model));

            return new NpgsqlModelAnnotations(model);
        }

        public static INpgsqlModelAnnotations Npgsql([NotNull] this IModel model)
        {
            Check.NotNull(model, nameof(model));

            return new ReadOnlyNpgsqlModelAnnotations(model);
        }
    }
}
