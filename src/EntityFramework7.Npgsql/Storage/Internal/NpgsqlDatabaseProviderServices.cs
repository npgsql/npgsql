// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Internal;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Metadata.Conventions.Internal;
using Microsoft.Data.Entity.Migrations;
using Microsoft.Data.Entity.Migrations.Internal;
using Microsoft.Data.Entity.Query;
using Microsoft.Data.Entity.Query.ExpressionTranslators;
using Microsoft.Data.Entity.Query.ExpressionTranslators.Internal;
using Microsoft.Data.Entity.Query.Internal;
using Microsoft.Data.Entity.Query.Sql;
using Microsoft.Data.Entity.Query.Sql.Internal;
using Microsoft.Data.Entity.Update;
using Microsoft.Data.Entity.Update.Internal;
using Microsoft.Data.Entity.ValueGeneration;
using Microsoft.Data.Entity.ValueGeneration.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.Data.Entity.Storage.Internal
{
    public class NpgsqlDatabaseProviderServices : RelationalDatabaseProviderServices
    {
        public NpgsqlDatabaseProviderServices([NotNull] IServiceProvider services)
            : base(services)
        {
        }

        public override string InvariantName => GetType().GetTypeInfo().Assembly.GetName().Name;
        public override IDatabaseCreator Creator => GetService<NpgsqlDatabaseCreator>();
        public override IRelationalConnection RelationalConnection => GetService<NpgsqlRelationalConnection>();
        public override ISqlGenerator SqlGenerator => GetService<NpgsqlSqlGenerator>();
        public override IRelationalDatabaseCreator RelationalDatabaseCreator => GetService<NpgsqlDatabaseCreator>();
        public override IConventionSetBuilder ConventionSetBuilder => GetService<NpgsqlConventionSetBuilder>();
        public override IMigrationsAnnotationProvider MigrationsAnnotationProvider => GetService<NpgsqlMigrationsAnnotationProvider>();
        public override IHistoryRepository HistoryRepository => GetService<NpgsqlHistoryRepository>();
        public override IMigrationsSqlGenerator MigrationsSqlGenerator => GetService<NpgsqlMigrationsSqlGenerator>();
        public override IModelSource ModelSource => GetService<NpgsqlModelSource>();
        public override IUpdateSqlGenerator UpdateSqlGenerator => GetService<NpgsqlUpdateSqlGenerator>();
        public override IValueGeneratorCache ValueGeneratorCache => GetService<NpgsqlValueGeneratorCache>();
        public override IRelationalTypeMapper TypeMapper => GetService<NpgsqlTypeMapper>();
        public override IModificationCommandBatchFactory ModificationCommandBatchFactory => GetService<NpgsqlModificationCommandBatchFactory>();
        public override IRelationalValueBufferFactoryFactory ValueBufferFactoryFactory => GetService<TypedRelationalValueBufferFactoryFactory>();
        public override IRelationalAnnotationProvider AnnotationProvider => GetService<NpgsqlAnnotationProvider>();
        public override IMethodCallTranslator CompositeMethodCallTranslator => GetService<NpgsqlCompositeMethodCallTranslator>();
        public override IMemberTranslator CompositeMemberTranslator => GetService<NpgsqlCompositeMemberTranslator>();
        public override IQueryCompilationContextFactory QueryCompilationContextFactory => GetService<NpgsqlQueryCompilationContextFactory>();
        public override ISqlQueryGeneratorFactory SqlQueryGeneratorFactory => GetService<NpgsqlQuerySqlGeneratorFactory>();
    }
}
