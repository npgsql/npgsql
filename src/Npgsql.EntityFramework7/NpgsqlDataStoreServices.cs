// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Metadata.Builders;
using Microsoft.Data.Entity.Query;
using Microsoft.Data.Entity.Relational;
using Microsoft.Data.Entity.Relational.Migrations.History;
using Microsoft.Data.Entity.Relational.Migrations.Infrastructure;
using Microsoft.Data.Entity.Relational.Migrations.Sql;
using Npgsql.EntityFramework7.Migrations;
using Npgsql.EntityFramework7.Query;
using Microsoft.Data.Entity.Storage;
using Microsoft.Data.Entity.Utilities;
using Microsoft.Data.Entity.ValueGeneration;
using Microsoft.Framework.DependencyInjection;

namespace Npgsql.EntityFramework7
{
    public class NpgsqlDataStoreServices : INpgsqlDataStoreServices
    {
        private readonly IServiceProvider _serviceProvider;

        public NpgsqlDataStoreServices([NotNull] IServiceProvider serviceProvider)
        {
            Check.NotNull(serviceProvider, nameof(serviceProvider));

            _serviceProvider = serviceProvider;
        }

        public virtual IDataStore Store => _serviceProvider.GetRequiredService<INpgsqlDataStore>();

        public virtual IQueryContextFactory QueryContextFactory => _serviceProvider.GetRequiredService<INpgsqlQueryContextFactory>();

        public virtual IDataStoreCreator Creator => _serviceProvider.GetRequiredService<INpgsqlDataStoreCreator>();

        public virtual IDataStoreConnection Connection => _serviceProvider.GetRequiredService<INpgsqlEFConnection>();

        public virtual IRelationalConnection RelationalConnection => _serviceProvider.GetRequiredService<INpgsqlEFConnection>();

        public virtual IValueGeneratorSelector ValueGeneratorSelector => _serviceProvider.GetRequiredService<INpgsqlValueGeneratorSelector>();

        public virtual IDatabaseFactory DatabaseFactory => _serviceProvider.GetRequiredService<INpgsqlDatabaseFactory>();

        public virtual IModelBuilderFactory ModelBuilderFactory => _serviceProvider.GetRequiredService<INpgsqlModelBuilderFactory>();

        public virtual IModelDiffer ModelDiffer => _serviceProvider.GetRequiredService<INpgsqlModelDiffer>();

        public virtual IHistoryRepository HistoryRepository => _serviceProvider.GetRequiredService<INpgsqlHistoryRepository>();

        public virtual IMigrationSqlGenerator MigrationSqlGenerator => _serviceProvider.GetRequiredService<INpgsqlMigrationSqlGenerator>();

        public virtual IModelSource ModelSource => _serviceProvider.GetRequiredService<INpgsqlModelSource>();

        public virtual ISqlGenerator SqlGenerator => _serviceProvider.GetRequiredService<INpgsqlSqlGenerator>();
    }
}
