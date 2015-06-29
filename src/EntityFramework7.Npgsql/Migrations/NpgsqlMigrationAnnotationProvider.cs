// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational.Migrations.Infrastructure;
using Npgsql.EntityFramework7.Metadata;

namespace Npgsql.EntityFramework7.Migrations
{
    public class NpgsqlMigrationAnnotationProvider : MigrationAnnotationProvider
    {
        public override IEnumerable<IAnnotation> For(IKey key)
            => key.Annotations.Where(a => a.Name == NpgsqlAnnotationNames.Prefix + NpgsqlAnnotationNames.Clustered);

        public override IEnumerable<IAnnotation> For(IIndex index)
            => index.Annotations.Where(a => a.Name == NpgsqlAnnotationNames.Prefix + NpgsqlAnnotationNames.Clustered);

        public override IEnumerable<IAnnotation> For(IProperty property)
        {
            var annotations = property.Annotations.Where(
                a => a.Name == NpgsqlAnnotationNames.Prefix + NpgsqlAnnotationNames.ColumnComputedExpression);
            foreach (var annotation in annotations)
            {
                yield return annotation;
            }

            if (GetValueGenerationStrategy(property) == NpgsqlValueGenerationStrategy.Identity)
            {
                yield return new Annotation(
                    NpgsqlAnnotationNames.Prefix + NpgsqlAnnotationNames.ValueGeneration,
                    NpgsqlValueGenerationStrategy.Identity.ToString());
            }
        }

        // TODO: Move to metadata API?
        private static NpgsqlValueGenerationStrategy? GetValueGenerationStrategy(IProperty property)
            => property.StoreGeneratedPattern == StoreGeneratedPattern.Identity
                    && property.Npgsql().DefaultExpression == null
                    && property.Npgsql().DefaultValue == null
                    && property.Npgsql().ComputedExpression == null
                ? property.Npgsql().ValueGenerationStrategy
                : null;
    }
}
