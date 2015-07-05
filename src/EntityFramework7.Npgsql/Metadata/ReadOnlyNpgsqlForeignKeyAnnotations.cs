// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.Data.Entity.Metadata;

namespace EntityFramework7.Npgsql.Metadata
{
    public class ReadOnlyNpgsqlForeignKeyAnnotations : ReadOnlyRelationalForeignKeyAnnotations, INpgsqlForeignKeyAnnotations
    {
        protected const string NpgsqlNameAnnotation = NpgsqlAnnotationNames.Prefix + RelationalAnnotationNames.Name;

        public ReadOnlyNpgsqlForeignKeyAnnotations([NotNull] IForeignKey foreignKey)
            : base(foreignKey)
        {
        }

        public override string Name
            => ForeignKey[NpgsqlNameAnnotation] as string
               ?? base.Name;
    }
}
