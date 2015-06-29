// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata.Builders;
using Microsoft.Data.Entity.Metadata.ModelConventions;
using Microsoft.Data.Entity.Relational;
using Microsoft.Data.Entity.Relational.Metadata;
using Microsoft.Data.Entity.Relational.Migrations.History;
using Microsoft.Data.Entity.Relational.Migrations.Infrastructure;
using Microsoft.Data.Entity.Relational.Migrations.Sql;
using Microsoft.Data.Entity.Relational.Query.Methods;
using Microsoft.Data.Entity.Relational.Update;
using Microsoft.Data.Entity.Storage;
using Microsoft.Data.Entity.ValueGeneration;
using Npgsql.EntityFramework7.Metadata;
using Npgsql.EntityFramework7.Migrations;
using Npgsql.EntityFramework7.Update;
using Npgsql.EntityFramework7.ValueGeneration;

namespace Npgsql.EntityFramework7
{
    public class NpgsqlDatabaseProviderServices : RelationalDatabaseProviderServices
    {
        private readonly IServiceProvider _serviceProvider;

        public NpgsqlDatabaseProviderServices([NotNull] IServiceProvider services)
            : base(services)
        {
        }

        public override IDatabase Database => GetService<NpgsqlDatabase>();
        public override IDatabaseCreator Creator => GetService<NpgsqlDatabaseCreator>();
        public override IDatabaseConnection Connection => GetService<NpgsqlDatabaseConnection>();
        public override IRelationalConnection RelationalConnection => GetService<NpgsqlDatabaseConnection>();
        public override IValueGeneratorSelector ValueGeneratorSelector => GetService<NpgsqlValueGeneratorSelector>();
        public override IRelationalDatabaseCreator RelationalDatabaseCreator => GetService<NpgsqlDatabaseCreator>();
        public override IConventionSetBuilder ConventionSetBuilder => GetService<NpgsqlConventionSetBuilder>();
        public override IMigrationAnnotationProvider MigrationAnnotationProvider => GetService<NpgsqlMigrationAnnotationProvider>();
        public override IHistoryRepository HistoryRepository => GetService<NpgsqlHistoryRepository>();
        public override IMigrationSqlGenerator MigrationSqlGenerator => GetService<NpgsqlMigrationSqlGenerator>();
        public override IModelSource ModelSource => GetService<NpgsqlModelSource>();
        public override ISqlGenerator SqlGenerator => GetService<INpgsqlSqlGenerator>();
        public override IValueGeneratorCache ValueGeneratorCache => GetService<INpgsqlValueGeneratorCache>();
        public override IRelationalTypeMapper TypeMapper => GetService<NpgsqlTypeMapper>();
        public override IModificationCommandBatchFactory ModificationCommandBatchFactory => GetService<NpgsqlModificationCommandBatchFactory>();
        public override IRelationalValueBufferFactoryFactory ValueBufferFactoryFactory => GetService<UntypedValueBufferFactoryFactory>();
        public override IRelationalMetadataExtensionProvider MetadataExtensionProvider => GetService<NpgsqlMetadataExtensionProvider>();
        public override IMethodCallTranslator CompositeMethodCallTranslator => GetService<NpgsqlCompositeMethodCallTranslator>();
        public override IMemberTranslator CompositeMemberTranslator => GetService<NpgsqlCompositeMemberTranslator>();
    }
}
