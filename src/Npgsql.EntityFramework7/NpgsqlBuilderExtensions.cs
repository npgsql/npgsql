// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Metadata.Builders;
using Npgsql.EntityFramework7.Metadata;
using Microsoft.Data.Entity.Utilities;

// ReSharper disable once CheckNamespace

namespace Microsoft.Data.Entity
{
    public static class NpgsqlBuilderExtensions
    {
        public static NpgsqlPropertyBuilder ForNpgsql(
            [NotNull] this PropertyBuilder propertyBuilder)
        {
            Check.NotNull(propertyBuilder, nameof(propertyBuilder));

            return new NpgsqlPropertyBuilder(propertyBuilder.Metadata);
        }

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
        {
            Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));

            return new NpgsqlEntityTypeBuilder(entityTypeBuilder.Metadata);
        }

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
        {
            Check.NotNull(keyBuilder, nameof(keyBuilder));

            return new NpgsqlKeyBuilder(keyBuilder.Metadata);
        }

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
        {
            Check.NotNull(indexBuilder, nameof(indexBuilder));

            return new NpgsqlIndexBuilder(indexBuilder.Metadata);
        }

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
        {
            Check.NotNull(referenceCollectionBuilder, nameof(referenceCollectionBuilder));

            return new NpgsqlForeignKeyBuilder(referenceCollectionBuilder.Metadata);
        }

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
        {
            Check.NotNull(referenceCollectionBuilder, nameof(referenceCollectionBuilder));
            Check.NotNull(builderAction, nameof(builderAction));

            builderAction(ForNpgsql(referenceCollectionBuilder));

            return referenceCollectionBuilder;
        }

        public static NpgsqlForeignKeyBuilder ForNpgsql(
            [NotNull] this CollectionReferenceBuilder collectionReferenceBuilder)
        {
            Check.NotNull(collectionReferenceBuilder, nameof(collectionReferenceBuilder));

            return new NpgsqlForeignKeyBuilder(collectionReferenceBuilder.Metadata);
        }

        public static CollectionReferenceBuilder ForNpgsql(
            [NotNull] this CollectionReferenceBuilder collectionReferenceBuilder,
            [NotNull] Action<NpgsqlForeignKeyBuilder> builderAction)
        {
            Check.NotNull(collectionReferenceBuilder, nameof(collectionReferenceBuilder));
            Check.NotNull(builderAction, nameof(builderAction));

            builderAction(ForNpgsql(collectionReferenceBuilder));

            return collectionReferenceBuilder;
        }

        public static CollectionReferenceBuilder<TEntity, TRelatedEntity> ForNpgsql<TEntity, TRelatedEntity>(
            [NotNull] this CollectionReferenceBuilder<TEntity, TRelatedEntity> collectionReferenceBuilder,
            [NotNull] Action<NpgsqlForeignKeyBuilder> builderAction)
            where TEntity : class
        {
            Check.NotNull(collectionReferenceBuilder, nameof(collectionReferenceBuilder));
            Check.NotNull(builderAction, nameof(builderAction));

            builderAction(ForNpgsql(collectionReferenceBuilder));

            return collectionReferenceBuilder;
        }

        public static NpgsqlForeignKeyBuilder ForNpgsql(
            [NotNull] this ReferenceReferenceBuilder referenceReferenceBuilder)
        {
            Check.NotNull(referenceReferenceBuilder, nameof(referenceReferenceBuilder));

            return new NpgsqlForeignKeyBuilder(referenceReferenceBuilder.Metadata);
        }

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
        {
            Check.NotNull(referenceReferenceBuilder, nameof(referenceReferenceBuilder));
            Check.NotNull(builderAction, nameof(builderAction));

            builderAction(ForNpgsql(referenceReferenceBuilder));

            return referenceReferenceBuilder;
        }

        public static NpgsqlModelBuilder ForNpgsql(
            [NotNull] this ModelBuilder modelBuilder)
        {
            Check.NotNull(modelBuilder, nameof(modelBuilder));

            return new NpgsqlModelBuilder(modelBuilder.Model);
        }

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
