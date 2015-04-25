// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Relational.Migrations;
using Microsoft.Data.Entity.Utilities;
using Microsoft.Framework.Logging;

namespace Npgsql.EntityFramework7
{
    public class NpgsqlDatabaseFactory : INpgsqlDatabaseFactory
    {
        private readonly DbContext _context;
        private readonly INpgsqlDataStoreCreator _dataStoreCreator;
        private readonly INpgsqlEFConnection _connection;
        private readonly IMigrator _migrator;
        private readonly ILoggerFactory _loggerFactory;

        public NpgsqlDatabaseFactory(
            [NotNull] DbContext context,
            [NotNull] INpgsqlDataStoreCreator dataStoreCreator,
            [NotNull] INpgsqlEFConnection connection,
            [NotNull] IMigrator migrator,
            [NotNull] ILoggerFactory loggerFactory)
        {
            Check.NotNull(context, nameof(context));
            Check.NotNull(dataStoreCreator, nameof(dataStoreCreator));
            Check.NotNull(connection, nameof(connection));
            Check.NotNull(migrator, nameof(migrator));
            Check.NotNull(loggerFactory, nameof(loggerFactory));

            _context = context;
            _dataStoreCreator = dataStoreCreator;
            _connection = connection;
            _migrator = migrator;
            _loggerFactory = loggerFactory;
        }

        public virtual Database CreateDatabase() 
            => new NpgsqlDatabase(_context, _dataStoreCreator, _connection, _migrator, _loggerFactory);
    }
}
