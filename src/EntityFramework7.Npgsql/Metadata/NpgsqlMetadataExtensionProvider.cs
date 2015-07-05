// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;

namespace EntityFramework7.Npgsql.Metadata
{
    public class NpgsqlMetadataExtensionProvider : IRelationalMetadataExtensionProvider
    {
        public virtual IRelationalEntityTypeAnnotations Extensions(IEntityType entityType) => entityType.Npgsql();
        public virtual IRelationalForeignKeyAnnotations Extensions(IForeignKey foreignKey) => foreignKey.Npgsql();
        public virtual IRelationalIndexAnnotations Extensions(IIndex index) => index.Npgsql();
        public virtual IRelationalKeyAnnotations Extensions(IKey key) => key.Npgsql();
        public virtual IRelationalPropertyAnnotations Extensions(IProperty property) => property.Npgsql();
        public virtual IRelationalModelAnnotations Extensions(IModel model) => model.Npgsql();
    }
}
