// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Text;
using EntityFramework7.Npgsql;
using EntityFramework7.Npgsql.Metadata;
using EntityFramework7.Npgsql.Migrations;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Internal;
using Microsoft.Data.Entity.Migrations;
using Microsoft.Data.Entity.Storage;
using Microsoft.Data.Entity.Update;
using Microsoft.Data.Entity.Utilities;

namespace EntityFramework7.Npgsql.Migrations
{
    public class NpgsqlHistoryRepository : HistoryRepository
    {
        private readonly NpgsqlUpdateSqlGenerator _sql;

        public NpgsqlHistoryRepository(
            [NotNull] IDatabaseCreator databaseCreator,
            [NotNull] ISqlStatementExecutor executor,
            [NotNull] NpgsqlDatabaseConnection connection,
            [NotNull] IDbContextOptions options,
            [NotNull] IMigrationsModelDiffer modelDiffer,
            [NotNull] NpgsqlMigrationsSqlGenerator sqlGenerator,
            [NotNull] NpgsqlMetadataExtensionProvider annotations,
            [NotNull] NpgsqlUpdateSqlGenerator sql)
            : base(
                  databaseCreator,
                  executor,
                  connection,
                  options,
                  modelDiffer,
                  sqlGenerator,
                  annotations,
                  sql)
        {
            Check.NotNull(sql, nameof(sql));

            _sql = sql;
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
                        .Append(_sql.EscapeLiteral(TableSchema))
                        .Append("' AND ");
                }

                builder
                    .Append("c.relname='")
                    .Append(_sql.EscapeLiteral(TableName))
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
