// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Metadata.Builders;
using Microsoft.Data.Entity.Utilities;

// ReSharper disable once CheckNamespace

namespace EntityFranework7.Npgsql
{
    public static class NpgsqlPropertyBuilderExtensions
    {
        public static PropertyBuilder HasNpgsqlColumnName(
            [NotNull] this PropertyBuilder propertyBuilder,
            [CanBeNull] string name)
        {
            Check.NotNull(propertyBuilder, nameof(propertyBuilder));
            Check.NullButNotEmpty(name, nameof(name));

            propertyBuilder.Metadata.Npgsql().Column = name;

            return propertyBuilder;
        }

        public static PropertyBuilder<TProperty> HasNpgsqlColumnName<TProperty>(
            [NotNull] this PropertyBuilder<TProperty> propertyBuilder,
            [CanBeNull] string name)
            => (PropertyBuilder<TProperty>)HasNpgsqlColumnName((PropertyBuilder)propertyBuilder, name);

        public static PropertyBuilder HasNpgsqlColumnType(
            [NotNull] this PropertyBuilder propertyBuilder,
            [CanBeNull] string typeName)
        {
            Check.NotNull(propertyBuilder, nameof(propertyBuilder));
            Check.NullButNotEmpty(typeName, nameof(typeName));

            propertyBuilder.Metadata.Npgsql().ColumnType = typeName;

            return propertyBuilder;
        }

        public static PropertyBuilder<TProperty> HasNpgsqlColumnType<TProperty>(
            [NotNull] this PropertyBuilder<TProperty> propertyBuilder,
            [CanBeNull] string typeName)
            => (PropertyBuilder<TProperty>)HasNpgsqlColumnType((PropertyBuilder)propertyBuilder, typeName);

        public static PropertyBuilder NpgsqlDefaultValueSql(
            [NotNull] this PropertyBuilder propertyBuilder,
            [CanBeNull] string sql)
        {
            Check.NotNull(propertyBuilder, nameof(propertyBuilder));
            Check.NullButNotEmpty(sql, nameof(sql));

            propertyBuilder.Metadata.Npgsql().DefaultValueSql = sql;

            return propertyBuilder;
        }

        public static PropertyBuilder<TProperty> NpgsqlDefaultValueSql<TProperty>(
            [NotNull] this PropertyBuilder<TProperty> propertyBuilder,
            [CanBeNull] string sql)
            => (PropertyBuilder<TProperty>)NpgsqlDefaultValueSql((PropertyBuilder)propertyBuilder, sql);

        public static PropertyBuilder NpgsqlDefaultValue(
            [NotNull] this PropertyBuilder propertyBuilder,
            [CanBeNull] object value)
        {
            Check.NotNull(propertyBuilder, nameof(propertyBuilder));

            propertyBuilder.Metadata.Npgsql().DefaultValue = value;

            return propertyBuilder;
        }

        public static PropertyBuilder<TProperty> NpgsqlDefaultValue<TProperty>(
            [NotNull] this PropertyBuilder<TProperty> propertyBuilder,
            [CanBeNull] object value)
            => (PropertyBuilder<TProperty>)NpgsqlDefaultValue((PropertyBuilder)propertyBuilder, value);

        public static PropertyBuilder NpgsqlComputedExpression(
            [NotNull] this PropertyBuilder propertyBuilder,
            [CanBeNull] string sql)
        {
            Check.NotNull(propertyBuilder, nameof(propertyBuilder));
            Check.NullButNotEmpty(sql, nameof(sql));

            propertyBuilder.Metadata.Npgsql().ComputedExpression = sql;

            return propertyBuilder;
        }

        public static PropertyBuilder<TProperty> NpgsqlComputedExpression<TProperty>(
            [NotNull] this PropertyBuilder<TProperty> propertyBuilder,
            [CanBeNull] string sql)
            => (PropertyBuilder<TProperty>)NpgsqlComputedExpression((PropertyBuilder)propertyBuilder, sql);

        public static PropertyBuilder UseNpgsqlSequence(
            [NotNull] this PropertyBuilder propertyBuilder,
            [CanBeNull] string name = null,
            [CanBeNull] string schema = null)
        {
            Check.NotNull(propertyBuilder, nameof(propertyBuilder));
            Check.NullButNotEmpty(name, nameof(name));
            Check.NullButNotEmpty(schema, nameof(schema));

            var property = propertyBuilder.Metadata;
            var sequence = property.EntityType.Model.Npgsql().GetOrAddSequence(name, schema);

            property.StoreGeneratedPattern = StoreGeneratedPattern.Identity;
            property.Npgsql().SequenceName = sequence.Name;
            property.Npgsql().SequenceSchema = sequence.Schema;

            return propertyBuilder;
        }

        public static PropertyBuilder<TProperty> UseNpgsqlSequence<TProperty>(
            [NotNull] this PropertyBuilder<TProperty> propertyBuilder,
            [CanBeNull] string name = null,
            [CanBeNull] string schema = null)
            => (PropertyBuilder<TProperty>)UseNpgsqlSequence((PropertyBuilder)propertyBuilder, name, schema);
    }
}
