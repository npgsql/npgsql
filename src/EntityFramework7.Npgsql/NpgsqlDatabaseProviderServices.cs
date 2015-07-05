// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Reflection;
using EntityFramework7.Npgsql.Metadata;
using EntityFramework7.Npgsql.Migrations;
using EntityFramework7.Npgsql.Update;
using EntityFramework7.Npgsql.ValueGeneration;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Metadata.Conventions.Internal;
using Microsoft.Data.Entity.Migrations.History;
using Microsoft.Data.Entity.Migrations.Infrastructure;
using Microsoft.Data.Entity.Migrations.Sql;
using Microsoft.Data.Entity.Query.Methods;
using Microsoft.Data.Entity.Storage;
using Microsoft.Data.Entity.Update;
using Microsoft.Data.Entity.ValueGeneration;

namespace EntityFramework7.Npgsql
{
    public class NpgsqlDatabaseProviderServices : RelationalDatabaseProviderServices
    {
        public NpgsqlDatabaseProviderServices([NotNull] IServiceProvider services)
            : base(services)
        {
        }

        public override string InvariantName => GetType().GetTypeInfo().Assembly.GetName().Name;
        public override IDatabase Database => GetService<NpgsqlDatabase>();
        public override IDatabaseCreator Creator => GetService<NpgsqlDatabaseCreator>();
        public override IRelationalConnection RelationalConnection => GetService<NpgsqlDatabaseConnection>();
        public override IRelationalDatabaseCreator RelationalDatabaseCreator => GetService<NpgsqlDatabaseCreator>();
        public override IConventionSetBuilder ConventionSetBuilder => GetService<NpgsqlConventionSetBuilder>();
        public override IMigrationAnnotationProvider MigrationAnnotationProvider => GetService<NpgsqlMigrationAnnotationProvider>();
        public override IHistoryRepository HistoryRepository => GetService<NpgsqlHistoryRepository>();
        public override IMigrationSqlGenerator MigrationSqlGenerator => GetService<NpgsqlMigrationSqlGenerator>();
        public override IModelSource ModelSource => GetService<NpgsqlModelSource>();
        public override IUpdateSqlGenerator UpdateSqlGenerator => GetService<NpgsqlUpdateSqlGenerator>();
        public override IValueGeneratorCache ValueGeneratorCache => GetService<NpgsqlValueGeneratorCache>();
        public override IRelationalTypeMapper TypeMapper => GetService<NpgsqlTypeMapper>();
        public override IModificationCommandBatchFactory ModificationCommandBatchFactory => GetService<NpgsqlModificationCommandBatchFactory>();
        public override IRelationalValueBufferFactoryFactory ValueBufferFactoryFactory => GetService<UntypedValueBufferFactoryFactory>();
        public override IRelationalMetadataExtensionProvider MetadataExtensionProvider => GetService<NpgsqlMetadataExtensionProvider>();
        public override IMethodCallTranslator CompositeMethodCallTranslator => GetService<NpgsqlCompositeMethodCallTranslator>();
        public override IMemberTranslator CompositeMemberTranslator => GetService<NpgsqlCompositeMemberTranslator>();
    }
}
