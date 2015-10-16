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
    public class NpgsqlDatabaseConnection : RelationalConnection
    {
        public NpgsqlDatabaseConnection(
            [NotNull] IDbContextOptions options,
            // ReSharper disable once SuggestBaseTypeForParameter
            [NotNull] ILogger<NpgsqlConnection> logger)
            : base(options, logger)
        {
        }

        private NpgsqlDatabaseConnection(
            [NotNull] IDbContextOptions options, [NotNull] ILogger logger)
            : base(options, logger)
        {
        }

        // TODO: Consider using DbProviderFactory to create connection instance
        // Issue #774
        protected override DbConnection CreateDbConnection() => new NpgsqlConnection(ConnectionString);

        public NpgsqlDatabaseConnection CreateMasterConnection()
        {
            var builder = new NpgsqlConnectionStringBuilder { ConnectionString = ConnectionString };

            // TODO: See #566
            builder.Database = "postgres";
            builder.Pooling = false;

            // TODO use clone connection method once implimented see #1406
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseNpgsql(builder.ConnectionString).CommandTimeout(CommandTimeout);

            return new NpgsqlDatabaseConnection(optionsBuilder.Options, Logger);
        }
    }
}
