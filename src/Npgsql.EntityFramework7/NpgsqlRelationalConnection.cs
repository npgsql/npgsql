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
    public class NpgsqlRelationalConnection : RelationalConnection, INpgsqlEFConnection
    {
        private readonly ILoggerFactory _loggerFactory;

        public NpgsqlRelationalConnection([NotNull] IDbContextOptions options, [NotNull] ILoggerFactory loggerFactory)
            : base(options, loggerFactory)
        {
            Check.NotNull(loggerFactory, nameof(loggerFactory));

            _loggerFactory = loggerFactory;
        }

        // TODO: Consider using DbProviderFactory to create connection instance
        // Issue #774
        protected override DbConnection CreateDbConnection() => new NpgsqlConnection(ConnectionString);

        public virtual INpgsqlEFConnection CreateMasterConnection()
        {
            var builder = new NpgsqlConnectionStringBuilder { ConnectionString = ConnectionString };

            // TODO use clone connection method once implimented see #1406
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseNpgsql(builder.ConnectionString).CommandTimeout(CommandTimeout);

            return new NpgsqlRelationalConnection(optionsBuilder.Options, _loggerFactory);
        }
    }
}
