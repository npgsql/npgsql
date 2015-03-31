// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.Data.Entity.Metadata;
using Npgsql.EntityFramework7.Metadata;
using Microsoft.Data.Entity.Utilities;

// ReSharper disable once CheckNamespace

namespace Microsoft.Data.Entity
{
    public static class NpgsqlMetadataExtensions
    {
        public static NpgsqlPropertyExtensions Npgsql([NotNull] this Property property)
        {
            Check.NotNull(property, nameof(property));

            return new NpgsqlPropertyExtensions(property);
        }

        public static INpgsqlPropertyExtensions Npgsql([NotNull] this IProperty property)
        {
            Check.NotNull(property, nameof(property));

            return new ReadOnlyNpgsqlPropertyExtensions(property);
        }

        public static NpgsqlEntityTypeExtensions Npgsql([NotNull] this EntityType entityType)
        {
            Check.NotNull(entityType, nameof(entityType));

            return new NpgsqlEntityTypeExtensions(entityType);
        }

        public static INpgsqlEntityTypeExtensions Npgsql([NotNull] this IEntityType entityType)
        {
            Check.NotNull(entityType, nameof(entityType));

            return new ReadOnlyNpgsqlEntityTypeExtensions(entityType);
        }

        public static NpgsqlKeyExtensions Npgsql([NotNull] this Key key)
        {
            Check.NotNull(key, nameof(key));

            return new NpgsqlKeyExtensions(key);
        }

        public static INpgsqlKeyExtensions Npgsql([NotNull] this IKey key)
        {
            Check.NotNull(key, nameof(key));

            return new ReadOnlyNpgsqlKeyExtensions(key);
        }

        public static NpgsqlIndexExtensions Npgsql([NotNull] this Index index)
        {
            Check.NotNull(index, nameof(index));

            return new NpgsqlIndexExtensions(index);
        }

        public static INpgsqlIndexExtensions Npgsql([NotNull] this IIndex index)
        {
            Check.NotNull(index, nameof(index));

            return new ReadOnlyNpgsqlIndexExtensions(index);
        }

        public static NpgsqlForeignKeyExtensions Npgsql([NotNull] this ForeignKey foreignKey)
        {
            Check.NotNull(foreignKey, nameof(foreignKey));

            return new NpgsqlForeignKeyExtensions(foreignKey);
        }

        public static INpgsqlForeignKeyExtensions Npgsql([NotNull] this IForeignKey foreignKey)
        {
            Check.NotNull(foreignKey, nameof(foreignKey));

            return new ReadOnlyNpgsqlForeignKeyExtensions(foreignKey);
        }

        public static NpgsqlModelExtensions Npgsql([NotNull] this Model model)
        {
            Check.NotNull(model, nameof(model));

            return new NpgsqlModelExtensions(model);
        }

        public static INpgsqlModelExtensions Npgsql([NotNull] this IModel model)
        {
            Check.NotNull(model, nameof(model));

            return new ReadOnlyNpgsqlModelExtensions(model);
        }
    }
}
