// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Data.Common;
using System.Data.SqlClient;
using JetBrains.Annotations;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Relational;
using Microsoft.Data.Entity.Utilities;
using Microsoft.Framework.Logging;
using Npgsql;

namespace Npgsql.EntityFramework7
{
    public class NpgsqlDataStoreConnection : RelationalConnection
    {
        private readonly ILoggerFactory _loggerFactory;

        public NpgsqlDataStoreConnection([NotNull] IDbContextOptions options, [NotNull] ILoggerFactory loggerFactory)
            : base(options, loggerFactory)
        {
            Check.NotNull(loggerFactory, nameof(loggerFactory));

            _loggerFactory = loggerFactory;
        }

        // TODO: Consider using DbProviderFactory to create connection instance
        // Issue #774
        protected override DbConnection CreateDbConnection() => new NpgsqlConnection(ConnectionString);

        public NpgsqlDataStoreConnection CreateMasterConnection()
        {
            var builder = new NpgsqlConnectionStringBuilder { ConnectionString = ConnectionString };

            // TODO: See #566
            builder.Database = "postgres";
            builder.Pooling = false;

            // TODO use clone connection method once implimented see #1406
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseNpgsql(builder.ConnectionString).CommandTimeout(CommandTimeout);

            return new NpgsqlDataStoreConnection(optionsBuilder.Options, _loggerFactory);
        }
    }
}
