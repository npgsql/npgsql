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
    public class NpgsqlModificationCommandBatchFactory : ModificationCommandBatchFactory
    {
        /// <summary>
        ///     This constructor is intended only for use when creating test doubles that will override members
        ///     with mocked or faked behavior. Use of this constructor for other purposes may result in unexpected
        ///     behavior including but not limited to throwing <see cref="NullReferenceException" />.
        /// </summary>
        protected NpgsqlModificationCommandBatchFactory()
        {
        }

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
