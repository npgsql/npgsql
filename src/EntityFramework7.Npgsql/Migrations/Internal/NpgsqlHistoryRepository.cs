// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Text;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Storage;
using Microsoft.Data.Entity.Storage.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.Data.Entity.Migrations.Internal
{
    public class NpgsqlHistoryRepository : HistoryRepository
    {
        public NpgsqlHistoryRepository(
            [NotNull] IDatabaseCreator databaseCreator,
            [NotNull] ISqlCommandBuilder sqlCommandBuilder,
            [NotNull] NpgsqlRelationalConnection connection,
            [NotNull] IDbContextOptions options,
            [NotNull] IMigrationsModelDiffer modelDiffer,
            [NotNull] NpgsqlMigrationsSqlGenerator migrationsSqlGenerator,
            [NotNull] NpgsqlAnnotationProvider annotations,
            [NotNull] ISqlGenerator sqlGenerator)
            : base(
                  databaseCreator,
                  sqlCommandBuilder,
                  connection,
                  options,
                  modelDiffer,
                  migrationsSqlGenerator,
                  annotations,
                  sqlGenerator)
        {
        }

        protected override string ExistsSql
        {
            get
            {
                var builder = new StringBuilder();

                builder.Append("SELECT 1 FROM pg_catalog.pg_class c JOIN pg_catalog.pg_namespace n ON n.oid=c.relnamespace WHERE ");

                if (TableSchema != null)
                {
                    builder
                        .Append("n.nspname='")
                        .Append(SqlGenerator.EscapeLiteral(TableSchema))
                        .Append("' AND ");
                }

                builder
                    .Append("c.relname='")
                    .Append(SqlGenerator.EscapeLiteral(TableName))
                    .Append("';");

                return builder.ToString();
            }
        }

        protected override bool InterpretExistsResult(object value) => value != DBNull.Value;

        public override string GetCreateIfNotExistsScript()
        {
            return GetCreateScript();
        }

        public override string GetBeginIfNotExistsScript(string migrationId)
        {
            throw new NotSupportedException("Generating idempotent scripts for migration is not currently supported by Npgsql");
        }

        public override string GetBeginIfExistsScript(string migrationId)
        {
            throw new NotSupportedException("Generating idempotent scripts for migration is not currently supported by Npgsql");
        }

        public override string GetEndIfScript()
        {
            throw new NotSupportedException("Generating idempotent scripts for migration is not currently supported by Npgsql");
        }
    }
}
