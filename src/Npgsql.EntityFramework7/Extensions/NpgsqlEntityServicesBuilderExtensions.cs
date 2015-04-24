// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Relational;
using Npgsql.EntityFramework7;
using Npgsql.EntityFramework7.Migrations;
using Npgsql.EntityFramework7.Query;
using Npgsql.EntityFramework7.Update;
using Microsoft.Data.Entity.Storage;
using Microsoft.Data.Entity.Utilities;

// ReSharper disable once CheckNamespace

namespace Microsoft.Framework.DependencyInjection
{
    public static class NpgsqlEntityServicesBuilderExtensions
    {
        public static EntityFrameworkServicesBuilder AddNpgsql([NotNull] this EntityFrameworkServicesBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            ((IAccessor<IServiceCollection>)builder.AddRelational()).Service
                .AddSingleton<IDataStoreSource, NpgsqlDataStoreSource>()
                .TryAdd(new ServiceCollection()
                    .AddSingleton<INpgsqlModelBuilderFactory, NpgsqlModelBuilderFactory>()
                    .AddSingleton<INpgsqlValueGeneratorCache, NpgsqlValueGeneratorCache>()
                    .AddSingleton<NpgsqlSequenceValueGeneratorFactory>()
                    .AddSingleton<INpgsqlSqlGenerator, NpgsqlSqlGenerator>()
                    .AddSingleton<SqlStatementExecutor>()
                    .AddSingleton<NpgsqlTypeMapper>()
                    .AddSingleton<NpgsqlModificationCommandBatchFactory>()
                    .AddSingleton<NpgsqlCommandBatchPreparer>()
                    .AddSingleton<INpgsqlModelSource, NpgsqlModelSource>()
                    .AddScoped<INpgsqlQueryContextFactory, NpgsqlQueryContextFactory>()
                    .AddScoped<INpgsqlValueGeneratorSelector, NpgsqlValueGeneratorSelector>()
                    .AddScoped<NpgsqlBatchExecutor>()
                    .AddScoped<INpgsqlDataStoreServices, NpgsqlDataStoreServices>()
                    .AddScoped<INpgsqlDataStore, NpgsqlDataStore>()
                    .AddScoped<INpgsqlEFConnection, NpgsqlRelationalConnection>()
                    .AddScoped<INpgsqlModelDiffer, NpgsqlModelDiffer>()
                    .AddScoped<INpgsqlDatabaseFactory, NpgsqlDatabaseFactory>()
                    .AddScoped<INpgsqlMigrationSqlGenerator, NpgsqlMigrationSqlGenerator>()
                    .AddScoped<INpgsqlDataStoreCreator, NpgsqlDataStoreCreator>()
                    .AddScoped<INpgsqlHistoryRepository, NpgsqlHistoryRepository>());

            return builder;
        }
    }
}
