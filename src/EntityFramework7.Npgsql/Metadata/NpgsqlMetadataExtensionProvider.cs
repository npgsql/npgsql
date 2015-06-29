// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational.Metadata;

namespace EntityFramework7.Npgsql.Metadata
{
    public class NpgsqlMetadataExtensionProvider : IRelationalMetadataExtensionProvider
    {
        public virtual IRelationalEntityTypeExtensions Extensions(IEntityType entityType) => entityType.Npgsql();
        public virtual IRelationalForeignKeyExtensions Extensions(IForeignKey foreignKey) => foreignKey.Npgsql();
        public virtual IRelationalIndexExtensions Extensions(IIndex index) => index.Npgsql();
        public virtual IRelationalKeyExtensions Extensions(IKey key) => key.Npgsql();
        public virtual IRelationalPropertyExtensions Extensions(IProperty property) => property.Npgsql();
        public virtual IRelationalModelExtensions Extensions(IModel model) => model.Npgsql();
    }
}
