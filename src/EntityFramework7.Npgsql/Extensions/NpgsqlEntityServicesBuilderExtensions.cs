// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using EntityFramework7.Npgsql;
using EntityFramework7.Npgsql.Metadata;
using EntityFramework7.Npgsql.Migrations;
using EntityFramework7.Npgsql.Query.Sql;
using EntityFramework7.Npgsql.Update;
using EntityFramework7.Npgsql.ValueGeneration;
using EntityFramework7.Npgsql.Query.ExpressionTranslators;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Storage;
using Microsoft.Data.Entity.Utilities;
using Microsoft.Framework.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace

namespace Microsoft.Framework.DependencyInjection
{
    public static class NpgsqlEntityServicesBuilderExtensions
    {
        public static EntityFrameworkServicesBuilder AddNpgsql([NotNull] this EntityFrameworkServicesBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            var service = builder.AddRelational().GetService();

            service.TryAddEnumerable(ServiceDescriptor
                .Singleton<IDatabaseProvider, DatabaseProvider<NpgsqlDatabaseProviderServices, NpgsqlOptionsExtension>>());

            service.TryAdd(new ServiceCollection()
                .AddSingleton<NpgsqlConventionSetBuilder>()
                .AddSingleton<NpgsqlValueGeneratorCache>()
                .AddSingleton<NpgsqlUpdateSqlGenerator>()
                .AddSingleton<NpgsqlTypeMapper>()
                .AddSingleton<NpgsqlModelSource>()
                .AddSingleton<NpgsqlMetadataExtensionProvider>()
                .AddSingleton<NpgsqlMigrationsAnnotationProvider>()
                .AddScoped<NpgsqlModificationCommandBatchFactory>()
                .AddScoped<NpgsqlDatabaseProviderServices>()
                .AddScoped<NpgsqlDatabaseConnection>()
                .AddScoped<NpgsqlMigrationsSqlGenerator>()
                .AddScoped<NpgsqlDatabaseCreator>()
                .AddScoped<NpgsqlHistoryRepository>()
                .AddQuery());

            return builder;
        }

        private static IServiceCollection AddQuery(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddScoped<NpgsqlCompositeExpressionFragmentTranslator>()
                .AddScoped<NpgsqlCompositeMemberTranslator>()
                .AddScoped<NpgsqlCompositeMethodCallTranslator>()
                .AddScoped<NpgsqlQuerySqlGeneratorFactory>();
        }
    }
}
