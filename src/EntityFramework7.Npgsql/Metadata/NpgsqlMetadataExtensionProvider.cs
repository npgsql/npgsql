// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;

namespace EntityFramework7.Npgsql.Metadata
{
    public class NpgsqlMetadataExtensionProvider : IRelationalMetadataExtensionProvider
    {
        public virtual IRelationalEntityTypeAnnotations For(IEntityType entityType) => entityType.Npgsql();
        public virtual IRelationalForeignKeyAnnotations For(IForeignKey foreignKey) => foreignKey.Npgsql();
        public virtual IRelationalIndexAnnotations For(IIndex index) => index.Npgsql();
        public virtual IRelationalKeyAnnotations For(IKey key) => key.Npgsql();
        public virtual IRelationalModelAnnotations For(IModel model) => model.Npgsql();
        public virtual IRelationalPropertyAnnotations For(IProperty property) => property.Npgsql();
    }
}
