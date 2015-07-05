// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using EntityFramework7.Npgsql;
using EntityFramework7.Npgsql.Metadata;
using EntityFramework7.Npgsql.Migrations;
using EntityFramework7.Npgsql.Update;
using EntityFramework7.Npgsql.ValueGeneration;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Infrastructure;
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

            builder.AddRelational().GetService()
                .AddSingleton<IDatabaseProvider, DatabaseProvider<NpgsqlDatabaseProviderServices, NpgsqlOptionsExtension>>()
                .TryAdd(new ServiceCollection()
                    .AddSingleton<NpgsqlConventionSetBuilder>()
                    .AddSingleton<NpgsqlValueGeneratorCache>()
                    .AddSingleton<NpgsqlUpdateSqlGenerator>()
                    .AddSingleton<NpgsqlTypeMapper>()
                    .AddSingleton<NpgsqlModelSource>()
                    .AddSingleton<NpgsqlMetadataExtensionProvider>()
                    .AddSingleton<NpgsqlMigrationAnnotationProvider>()
                    .AddScoped<NpgsqlModificationCommandBatchFactory>()
                    .AddScoped<NpgsqlDatabaseProviderServices>()
                    .AddScoped<NpgsqlDatabase>()
                    .AddScoped<NpgsqlDatabaseConnection>()
                    .AddScoped<NpgsqlMigrationSqlGenerator>()
                    .AddScoped<NpgsqlDatabaseCreator>()
                    .AddScoped<NpgsqlHistoryRepository>()
                    .AddScoped<NpgsqlCompositeMethodCallTranslator>()
                    .AddScoped<NpgsqlCompositeMemberTranslator>());

            return builder;
        }
    }
}
