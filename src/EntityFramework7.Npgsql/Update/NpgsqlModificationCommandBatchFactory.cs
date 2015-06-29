// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Relational;
using Microsoft.Data.Entity.Relational.Metadata;
using Microsoft.Data.Entity.Relational.Update;

namespace EntityFramework7.Npgsql.Update
{
    // TODO: This is very temporary: command batching is disabled for now until properly implemented
    public class NpgsqlModificationCommandBatchFactory : ModificationCommandBatchFactory
    {
        public NpgsqlModificationCommandBatchFactory([NotNull] ISqlGenerator sqlGenerator)
            : base(sqlGenerator)
        {
        }

        public override ModificationCommandBatch Create(
            IDbContextOptions options,
            IRelationalMetadataExtensionProvider metadataExtensionProvider)
            => new NpgsqlModificationCommandBatch(SqlGenerator);
    }
}
