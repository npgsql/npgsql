// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Data.Common;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Utilities;
using Microsoft.Extensions.Logging;
using Npgsql;

// ReSharper disable once CheckNamespace
namespace Microsoft.Data.Entity.Storage.Internal
{
    public class NpgsqlRelationalConnection : RelationalConnection
    {
        public NpgsqlRelationalConnection(
            [NotNull] IDbContextOptions options,
            // ReSharper disable once SuggestBaseTypeForParameter
            [NotNull] ILogger<NpgsqlConnection> logger)
            : base(options, logger)
        {
        }

        private NpgsqlRelationalConnection(
            [NotNull] IDbContextOptions options, [NotNull] ILogger logger)
            : base(options, logger)
        {
        }

        // TODO: Consider using DbProviderFactory to create connection instance
        // Issue #774
        protected override DbConnection CreateDbConnection() => new NpgsqlConnection(ConnectionString);

        public NpgsqlRelationalConnection CreateMasterConnection()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString) {
                Database = "postgres",
                Pooling = false
            };
            var masterConn = ((NpgsqlConnection)DbConnection).CloneWith(csb.ToString());
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseNpgsql(masterConn);
            return new NpgsqlRelationalConnection(optionsBuilder.Options, Logger);
        }
    }
}
