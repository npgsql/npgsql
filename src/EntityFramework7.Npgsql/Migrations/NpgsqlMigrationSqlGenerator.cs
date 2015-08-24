// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityFramework7.Npgsql.Metadata;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations.Operations;
using Microsoft.Data.Entity.Migrations.Sql;
using Microsoft.Data.Entity.Utilities;

namespace EntityFramework7.Npgsql.Migrations
{
    public class NpgsqlMigrationSqlGenerator : MigrationSqlGenerator
    {
        private readonly NpgsqlUpdateSqlGenerator _sql;
        private readonly NpgsqlTypeMapper _typeMapper;

        public NpgsqlMigrationSqlGenerator(
            [NotNull] NpgsqlUpdateSqlGenerator sqlGenerator,
            [NotNull] NpgsqlTypeMapper typeMapper,
            [NotNull] NpgsqlMetadataExtensionProvider annotations)
            : base(sqlGenerator, typeMapper, annotations)
        {
            _sql = sqlGenerator;
            _typeMapper = typeMapper;
        }

        public override void Generate(
            [NotNull] AlterColumnOperation operation,
            [CanBeNull] IModel model,
            [NotNull] SqlBatchBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            //TODO: this should provide feature parity with the EF6 provider, check if there's anything missing for EF7

            var type = operation.ColumnType;
            if (operation.ColumnType == null)
            {
                var property = FindProperty(model, operation.Schema, operation.Table, operation.Name);
                type = property != null
                    ? _typeMapper.MapPropertyType(property).DefaultTypeName
                    : _typeMapper.GetDefaultMapping(operation.ClrType).DefaultTypeName;
            }

            var isSerial = false;
            var serial = operation.Annotations.Where(r => r.Name == NpgsqlAnnotationNames.Prefix + NpgsqlAnnotationNames.Serial).FirstOrDefault();
            isSerial = serial != null && (bool) serial.Value;

            var identifier = _sql.DelimitIdentifier(operation.Table, operation.Schema);
            var alterBase = $"ALTER TABLE {identifier} ALTER COLUMN {_sql.DelimitIdentifier(operation.Name)}";

            // TYPE
            builder.Append(alterBase)
                .Append(" TYPE ")
                .Append(type)
                .EndBatch();

            // NOT NULL
            builder.Append(alterBase)
                .Append(operation.IsNullable ? " DROP NOT NULL" : " SET NOT NULL")
                .EndBatch();

            builder.Append(alterBase);

            if (operation.DefaultValue != null)
            {
                builder.Append(" SET DEFAULT ")
                    .Append(_sql.GenerateLiteral((dynamic)operation.DefaultValue))
                    .EndBatch();
            }
            else if (!string.IsNullOrWhiteSpace(operation.DefaultValueSql))
            {
                builder.Append(" SET DEFAULT ")
                    .Append(operation.DefaultValueSql)
                    .EndBatch();
            }
            else if (isSerial)
            {
                builder.Append(" SET DEFAULT ");
                switch (type)
                {
                    case "smallint":
                    case "int":
                    case "bigint":
                    case "real":
                    case "double precision":
                    case "numeric":
                        //TODO: need function CREATE SEQUENCE IF NOT EXISTS and set to it...
                        //Until this is resolved changing IsIdentity from false to true
                        //on types int2, int4 and int8 won't switch to type serial2, serial4 and serial8
                        throw new NotImplementedException("Not supporting creating sequence for integer types");
                    case "uuid":
                        builder.Append("uuid_generate_v4()");
                        break;
                    default:
                        throw new NotImplementedException($"Not supporting creating IsIdentity for {type}");

                }
            }
            else
            {
                builder.Append(" DROP DEFAULT ");
            }
        }

        public override void Generate(
            [NotNull] RenameIndexOperation operation,
            [CanBeNull] IModel model,
            [NotNull] SqlBatchBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            if (operation.NewName != null)
            {
                Rename(operation.Schema, operation.Name, operation.NewName, "INDEX", builder);
            }
        }

        public override void Generate(RenameSequenceOperation operation, IModel model, SqlBatchBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            var separate = false;
            var name = operation.Name;
            if (operation.NewName != null)
            {
                Rename(operation.Schema, operation.Name, operation.NewName, "SEQUENCE", builder);

                separate = true;
                name = operation.NewName;
            }

            if (operation.NewSchema != null)
            {
                if (separate)
                {
                    builder.AppendLine(_sql.BatchCommandSeparator);
                }

                Transfer(operation.NewSchema, operation.Schema, name, "SEQUENCE", builder);
            }
        }

        public override void Generate(
            [NotNull] RenameTableOperation operation,
            [CanBeNull] IModel model,
            [NotNull] SqlBatchBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            var separate = false;
            var name = operation.Name;
            if (operation.NewName != null)
            {
                Rename(operation.Schema, operation.Name, operation.NewName, "TABLE", builder);

                separate = true;
                name = operation.NewName;
            }

            if (operation.NewSchema != null)
            {
                if (separate)
                {
                    builder.AppendLine(_sql.BatchCommandSeparator);
                }

                Transfer(operation.NewSchema, operation.Schema, name, "TABLE", builder);
            }
        }

        public override void Generate(CreateSchemaOperation operation, IModel model, SqlBatchBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            builder.Append("CREATE SCHEMA ")
                .Append(_sql.DelimitIdentifier(operation.Name))
                .EndBatch();
        }

        public virtual void Generate(
            [NotNull] CreateDatabaseOperation operation,
            [CanBeNull] IModel model,
            [NotNull] SqlBatchBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            builder
                .Append("CREATE DATABASE ")
                .Append(_sql.DelimitIdentifier(operation.Name))
                .EndBatch();
        }

        public virtual void Generate(
            [NotNull] DropDatabaseOperation operation,
            [CanBeNull] IModel model,
            [NotNull] SqlBatchBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            var dbName = _sql.DelimitIdentifier(operation.Name);

            builder
                // TODO: The following revokes connection only for the public role, what about other connecting roles?
                .Append("REVOKE CONNECT ON DATABASE ")
                .Append(dbName)
                .Append(" FROM PUBLIC")
                .EndBatch()
                // TODO: For PG <= 9.1, the column name is prodpic, not pid (see http://stackoverflow.com/questions/5408156/how-to-drop-a-postgresql-database-if-there-are-active-connections-to-it)
                .Append(
                    "SELECT pg_terminate_backend(pg_stat_activity.pid) FROM pg_stat_activity WHERE pg_stat_activity.datname = '")
                .Append(operation.Name)
                .Append("'")
                .EndBatch()
                .Append("DROP DATABASE ")
                .Append(dbName);
        }

        public override void Generate(
            [NotNull] DropIndexOperation operation,
            [CanBeNull] IModel model,
            [NotNull] SqlBatchBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            builder
                .Append("DROP INDEX ")
                .Append(_sql.DelimitIdentifier(operation.Name, operation.Schema));
        }

        public override void Generate(
            [NotNull] RenameColumnOperation operation,
            [CanBeNull] IModel model,
            [NotNull] SqlBatchBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            builder.Append("ALTER TABLE ")
                .Append(_sql.DelimitIdentifier(operation.Table, operation.Schema))
                .Append(" RENAME COLUMN ")
                .Append(_sql.DelimitIdentifier(operation.Name))
                .Append(" TO ")
                .Append(_sql.DelimitIdentifier(operation.NewName));
        }

        public override void ColumnDefinition(
            string schema,
            string table,
            string name,
            Type clrType,
            string type,
            bool nullable,
            object defaultValue,
            string defaultValueSql,
            string computedColumnSql,
            IAnnotatable annotatable,
            IModel model,
            SqlBatchBuilder builder)
        {
            Check.NotEmpty(name, nameof(name));
            Check.NotNull(annotatable, nameof(annotatable));
            Check.NotNull(clrType, nameof(clrType));
            Check.NotNull(builder, nameof(builder));

            if (type == null)
            {
                var property = FindProperty(model, schema, table, name);
                type = property != null
                    ? _typeMapper.MapPropertyType(property).DefaultTypeName
                    : _typeMapper.GetDefaultMapping(clrType).DefaultTypeName;
            }

            // TODO: Maybe implement computed columns via functions?
            // http://stackoverflow.com/questions/11165450/store-common-query-as-column/11166268#11166268

            var serial = annotatable[NpgsqlAnnotationNames.Prefix + NpgsqlAnnotationNames.Serial];
            if (serial != null && (bool)serial)
            {
                switch (type)
                {
                case "int":
                    type = "serial";
                    break;
                case "bigint":
                    type = "bigserial";
                    break;
                case "smallint":
                    type = "smallserial";
                    break;
                default:
                    throw new InvalidOperationException($"Column {name} of type {type} can't be Identity");
                }
            }

            base.ColumnDefinition(
                schema,
                table,
                name,
                clrType,
                type,
                nullable,
                defaultValue,
                defaultValueSql,
                computedColumnSql,
                annotatable,
                model,
                builder);
        }

        public virtual void Rename(
            [CanBeNull] string schema,
            [NotNull] string name,
            [NotNull] string newName,
            [NotNull] string type,
            [NotNull] SqlBatchBuilder builder)
        {
            Check.NotEmpty(name, nameof(name));
            Check.NotEmpty(newName, nameof(newName));
            Check.NotEmpty(type, nameof(type));
            Check.NotNull(builder, nameof(builder));


            builder
                .Append("ALTER ")
                .Append(type)
                .Append(" ")
                .Append(_sql.DelimitIdentifier(name, schema))
                .Append(" RENAME TO ")
                .Append(_sql.DelimitIdentifier(newName));
        }

        public virtual void Transfer(
            [NotNull] string newSchema,
            [CanBeNull] string schema,
            [NotNull] string name,
            [NotNull] string type,
            [NotNull] SqlBatchBuilder builder)
        {
            Check.NotEmpty(newSchema, nameof(newSchema));
            Check.NotEmpty(name, nameof(name));
            Check.NotNull(type, nameof(type));
            Check.NotNull(builder, nameof(builder));

            builder
                .Append("ALTER ")
                .Append(type)
                .Append(" ")
                .Append(_sql.DelimitIdentifier(name, schema))
                .Append(" SET SCHEMA ")
                .Append(_sql.DelimitIdentifier(newSchema));
        }

        public override void ForeignKeyAction(ReferentialAction referentialAction, SqlBatchBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            if (referentialAction == ReferentialAction.Restrict)
            {
                builder.Append("NO ACTION");
            }
            else
            {
                base.ForeignKeyAction(referentialAction, builder);
            }
        }

        #region Npgsql additions

        public override void Generate(
            [NotNull] CreateSequenceOperation operation,
            [CanBeNull] IModel model,
            [NotNull] SqlBatchBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            builder
                .Append("CREATE SEQUENCE ")
                .Append(_sql.DelimitIdentifier(operation.Name, operation.Schema));

            builder
                .Append(" START WITH ")
                .Append(_sql.GenerateLiteral(operation.StartWith));
            SequenceOptions(operation, model, builder);
        }

        #endregion
    }
}
