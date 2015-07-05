// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.Data.Entity.Metadata;

namespace EntityFramework7.Npgsql.Metadata
{
    public class ReadOnlyNpgsqlEntityTypeAnnotations : ReadOnlyRelationalEntityTypeAnnotations, INpgsqlEntityTypeAnnotations
    {
        protected const string NpgsqlTableAnnotation = NpgsqlAnnotationNames.Prefix + RelationalAnnotationNames.TableName;
        protected const string NpgsqlSchemaAnnotation = NpgsqlAnnotationNames.Prefix + RelationalAnnotationNames.Schema;

        public ReadOnlyNpgsqlEntityTypeAnnotations([NotNull] IEntityType entityType)
            : base(entityType)
        {
        }

        public override string Table
            => EntityType[NpgsqlTableAnnotation] as string
               ?? base.Table;

        public override string Schema
            => EntityType[NpgsqlSchemaAnnotation] as string
               ?? base.Schema;
    }
}
