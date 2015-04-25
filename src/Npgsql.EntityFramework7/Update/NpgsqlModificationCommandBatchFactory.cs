// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Relational.Update;
using Microsoft.Data.Entity.Utilities;

namespace Npgsql.EntityFramework7.Update
{
    public class NpgsqlModificationCommandBatchFactory : ModificationCommandBatchFactory, INpgsqlModificationCommandBatchFactory
    {
        public NpgsqlModificationCommandBatchFactory(
            [NotNull] INpgsqlSqlGenerator sqlGenerator)
            : base(sqlGenerator)
        {
        }

        public override ModificationCommandBatch Create([NotNull] IDbContextOptions options)
        {
            Check.NotNull(options, nameof(options));

            var optionsExtension = options.Extensions.OfType<NpgsqlOptionsExtension>().FirstOrDefault();

            var maxBatchSize = optionsExtension?.MaxBatchSize;

            // TODO: Batch size disabled for now
            return new NpgsqlModificationCommandBatch((INpgsqlSqlGenerator)SqlGenerator);
            //return new NpgsqlModificationCommandBatch((INpgsqlSqlGenerator)SqlGenerator, maxBatchSize);
        }
    }
}
