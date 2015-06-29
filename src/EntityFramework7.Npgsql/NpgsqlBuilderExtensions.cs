// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using EntityFramework7.Npgsql.Metadata;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Metadata.Builders;
using Microsoft.Data.Entity.Utilities;

// ReSharper disable once CheckNamespace

namespace Microsoft.Data.Entity
{
    public static class NpgsqlBuilderExtensions
    {
        public static NpgsqlPropertyBuilder ForNpgsql(
            [NotNull] this PropertyBuilder propertyBuilder)
            => new NpgsqlPropertyBuilder(Check.NotNull(propertyBuilder, nameof(propertyBuilder)).Metadata);

        public static PropertyBuilder ForNpgsql(
            [NotNull] this PropertyBuilder propertyBuilder,
            [NotNull] Action<NpgsqlPropertyBuilder> builderAction)
        {
            Check.NotNull(propertyBuilder, nameof(propertyBuilder));
            Check.NotNull(builderAction, nameof(builderAction));

            builderAction(ForNpgsql(propertyBuilder));

            return propertyBuilder;
        }

        public static PropertyBuilder<TProperty> ForNpgsql<TProperty>(
            [NotNull] this PropertyBuilder<TProperty> propertyBuilder,
            [NotNull] Action<NpgsqlPropertyBuilder> builderAction)
        {
            Check.NotNull(propertyBuilder, nameof(propertyBuilder));
            Check.NotNull(builderAction, nameof(builderAction));

            builderAction(ForNpgsql(propertyBuilder));

            return propertyBuilder;
        }

        public static NpgsqlEntityTypeBuilder ForNpgsql(
            [NotNull] this EntityTypeBuilder entityTypeBuilder)
            => new NpgsqlEntityTypeBuilder(Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder)).Metadata);

        public static EntityTypeBuilder ForNpgsql(
            [NotNull] this EntityTypeBuilder entityTypeBuilder,
            [NotNull] Action<NpgsqlEntityTypeBuilder> builderAction)
        {
            Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));

            builderAction(ForNpgsql(entityTypeBuilder));

            return entityTypeBuilder;
        }

        public static EntityTypeBuilder<TEntity> ForNpgsql<TEntity>(
            [NotNull] this EntityTypeBuilder<TEntity> entityTypeBuilder,
            [NotNull] Action<NpgsqlEntityTypeBuilder> builderAction)
            where TEntity : class
        {
            Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));

            builderAction(ForNpgsql(entityTypeBuilder));

            return entityTypeBuilder;
        }

        public static NpgsqlKeyBuilder ForNpgsql(
            [NotNull] this KeyBuilder keyBuilder)
            => new NpgsqlKeyBuilder(Check.NotNull(keyBuilder, nameof(keyBuilder)).Metadata);

        public static KeyBuilder ForNpgsql(
            [NotNull] this KeyBuilder keyBuilder,
            [NotNull] Action<NpgsqlKeyBuilder> builderAction)
        {
            Check.NotNull(keyBuilder, nameof(keyBuilder));
            Check.NotNull(builderAction, nameof(builderAction));

            builderAction(ForNpgsql(keyBuilder));

            return keyBuilder;
        }

        public static NpgsqlIndexBuilder ForNpgsql(
            [NotNull] this IndexBuilder indexBuilder)
            => new NpgsqlIndexBuilder(Check.NotNull(indexBuilder, nameof(indexBuilder)).Metadata);

        public static IndexBuilder ForNpgsql(
            [NotNull] this IndexBuilder indexBuilder,
            [NotNull] Action<NpgsqlIndexBuilder> builderAction)
        {
            Check.NotNull(indexBuilder, nameof(indexBuilder));
            Check.NotNull(builderAction, nameof(builderAction));

            builderAction(ForNpgsql(indexBuilder));

            return indexBuilder;
        }

        public static NpgsqlForeignKeyBuilder ForNpgsql(
            [NotNull] this ReferenceCollectionBuilder referenceCollectionBuilder)
            => new NpgsqlForeignKeyBuilder(
                Check.NotNull(referenceCollectionBuilder, nameof(referenceCollectionBuilder)).Metadata);

        public static ReferenceCollectionBuilder ForNpgsql(
            [NotNull] this ReferenceCollectionBuilder referenceCollectionBuilder,
            [NotNull] Action<NpgsqlForeignKeyBuilder> builderAction)
        {
            Check.NotNull(referenceCollectionBuilder, nameof(referenceCollectionBuilder));
            Check.NotNull(builderAction, nameof(builderAction));

            builderAction(ForNpgsql(referenceCollectionBuilder));

            return referenceCollectionBuilder;
        }

        public static ReferenceCollectionBuilder<TEntity, TRelatedEntity> ForNpgsql<TEntity, TRelatedEntity>(
            [NotNull] this ReferenceCollectionBuilder<TEntity, TRelatedEntity> referenceCollectionBuilder,
            [NotNull] Action<NpgsqlForeignKeyBuilder> builderAction)
            where TEntity : class
            where TRelatedEntity : class
        {
            Check.NotNull(referenceCollectionBuilder, nameof(referenceCollectionBuilder));
            Check.NotNull(builderAction, nameof(builderAction));

            builderAction(ForNpgsql(referenceCollectionBuilder));

            return referenceCollectionBuilder;
        }

        public static NpgsqlForeignKeyBuilder ForNpgsql(
            [NotNull] this ReferenceReferenceBuilder referenceReferenceBuilder)
            => new NpgsqlForeignKeyBuilder(
                Check.NotNull(referenceReferenceBuilder, nameof(referenceReferenceBuilder)).Metadata);

        public static ReferenceReferenceBuilder ForNpgsql(
            [NotNull] this ReferenceReferenceBuilder referenceReferenceBuilder,
            [NotNull] Action<NpgsqlForeignKeyBuilder> builderAction)
        {
            Check.NotNull(referenceReferenceBuilder, nameof(referenceReferenceBuilder));
            Check.NotNull(builderAction, nameof(builderAction));

            builderAction(ForNpgsql(referenceReferenceBuilder));

            return referenceReferenceBuilder;
        }

        public static ReferenceReferenceBuilder<TEntity, TRelatedEntity> ForNpgsql<TEntity, TRelatedEntity>(
            [NotNull] this ReferenceReferenceBuilder<TEntity, TRelatedEntity> referenceReferenceBuilder,
            [NotNull] Action<NpgsqlForeignKeyBuilder> builderAction)
            where TEntity : class
            where TRelatedEntity : class
        {
            Check.NotNull(referenceReferenceBuilder, nameof(referenceReferenceBuilder));
            Check.NotNull(builderAction, nameof(builderAction));

            builderAction(ForNpgsql(referenceReferenceBuilder));

            return referenceReferenceBuilder;
        }

        public static NpgsqlModelBuilder ForNpgsql(
            [NotNull] this ModelBuilder modelBuilder)
            => new NpgsqlModelBuilder(Check.NotNull(modelBuilder, nameof(modelBuilder)).Model);

        public static ModelBuilder ForNpgsql(
            [NotNull] this ModelBuilder modelBuilder,
            [NotNull] Action<NpgsqlModelBuilder> builderAction)
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder));
            Check.NotNull(builderAction, nameof(builderAction));

            builderAction(ForNpgsql(modelBuilder));

            return modelBuilder;
        }
    }
}
