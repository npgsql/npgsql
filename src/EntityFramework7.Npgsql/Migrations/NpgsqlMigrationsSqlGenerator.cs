// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Metadata.Internal;
using Microsoft.Data.Entity.Migrations.Operations;
using Microsoft.Data.Entity.Storage;
using Microsoft.Data.Entity.Storage.Internal;
using Microsoft.Data.Entity.Update;
using Microsoft.Data.Entity.Utilities;

namespace Microsoft.Data.Entity.Migrations
{
    public class NpgsqlMigrationsSqlGenerator : MigrationsSqlGenerator
    {
        public NpgsqlMigrationsSqlGenerator(
            [NotNull] IRelationalCommandBuilderFactory commandBuilderFactory,
            [NotNull] ISqlGenerator sqlGenerator,
            [NotNull] IRelationalTypeMapper typeMapper,
            [NotNull] IRelationalAnnotationProvider annotations)
            : base(commandBuilderFactory, sqlGenerator, typeMapper, annotations)
        {
        }

        protected override void Generate(MigrationOperation operation, IModel model, RelationalCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            var createDatabaseOperation = operation as NpgsqlCreateDatabaseOperation;
            var dropDatabaseOperation = operation as NpgsqlDropDatabaseOperation;
            if (createDatabaseOperation != null)
            {
                Generate(createDatabaseOperation, model, builder);
            }
            else if (dropDatabaseOperation != null)
            {
                Generate(dropDatabaseOperation, model, builder);
            }
            else
            {
                base.Generate(operation, model, builder);
            }
        }

        protected override void Generate(AlterColumnOperation operation, IModel model, RelationalCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            // TODO: There is probably duplication here with other methods. See ColumnDefinition.

            //TODO: this should provide feature parity with the EF6 provider, check if there's anything missing for EF7

            var type = operation.ColumnType;
            if (operation.ColumnType == null)
            {
                var property = FindProperty(model, operation.Schema, operation.Table, operation.Name);
                type = property != null
                    ? TypeMapper.GetMapping(property).DefaultTypeName
                    : TypeMapper.GetMapping(operation.ClrType).DefaultTypeName;
            }

            var serial = operation.FindAnnotation(NpgsqlAnnotationNames.Prefix + NpgsqlAnnotationNames.Serial);
            var isSerial = serial != null && (bool)serial.Value;

            var identifier = SqlGenerator.DelimitIdentifier(operation.Table, operation.Schema);
            var alterBase = $"ALTER TABLE {identifier} ALTER COLUMN {SqlGenerator.DelimitIdentifier(operation.Name)}";

            // TYPE
            builder.Append(alterBase)
                .Append(" TYPE ")
                .Append(type)
                .AppendLine(SqlGenerator.BatchCommandSeparator);

            // NOT NULL
            builder.Append(alterBase)
                .Append(operation.IsNullable ? " DROP NOT NULL" : " SET NOT NULL")
                .AppendLine(SqlGenerator.BatchCommandSeparator);

            builder.Append(alterBase);

            if (operation.DefaultValue != null)
            {
                builder.Append(" SET DEFAULT ")
                    .Append(SqlGenerator.GenerateLiteral((dynamic)operation.DefaultValue))
                    .AppendLine(SqlGenerator.BatchCommandSeparator);
            }
            else if (!string.IsNullOrWhiteSpace(operation.DefaultValueSql))
            {
                builder.Append(" SET DEFAULT ")
                    .Append(operation.DefaultValueSql)
                    .AppendLine(SqlGenerator.BatchCommandSeparator);
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

        protected override void Generate(CreateSequenceOperation operation, IModel model, RelationalCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            if (operation.ClrType != typeof(long))
            {
                throw new NotSupportedException("PostgreSQL sequences can only be bigint (long)");
            }

            builder
                .Append("CREATE SEQUENCE ")
                .Append(SqlGenerator.DelimitIdentifier(operation.Name, operation.Schema));

            builder
                .Append(" START WITH ")
                .Append(SqlGenerator.GenerateLiteral(operation.StartValue));
            SequenceOptions(operation, model, builder);
        }

        protected override void Generate(RenameIndexOperation operation, IModel model, RelationalCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            if (operation.NewName != null)
            {
                Rename(operation.Schema, operation.Name, operation.NewName, "INDEX", builder);
            }
        }

        protected override void Generate(RenameSequenceOperation operation, IModel model, RelationalCommandListBuilder builder)
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
                    builder.AppendLine(SqlGenerator.BatchCommandSeparator);
                }

                Transfer(operation.NewSchema, operation.Schema, name, "SEQUENCE", builder);
            }
        }

        protected override void Generate(RenameTableOperation operation, IModel model, RelationalCommandListBuilder builder)
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
                    builder.AppendLine(SqlGenerator.BatchCommandSeparator);
                }

                Transfer(operation.NewSchema, operation.Schema, name, "TABLE", builder);
            }
        }

        protected override void Generate(
            [NotNull] CreateIndexOperation operation,
            [CanBeNull] IModel model,
            [NotNull] RelationalCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            var method = (string)operation[NpgsqlAnnotationNames.Prefix + NpgsqlAnnotationNames.IndexMethod];

            builder.Append("CREATE ");

            if (operation.IsUnique)
            {
                builder.Append("UNIQUE ");
            }

            builder
                .Append("INDEX ")
                .Append(SqlGenerator.DelimitIdentifier(operation.Name))
                .Append(" ON ")
                .Append(SqlGenerator.DelimitIdentifier(operation.Table, operation.Schema));

            if (method != null)
            {
                builder
                    .Append(" USING ")
                    .Append(method);
            }

            builder
                .Append(" (")
                .Append(ColumnList(operation.Columns))
                .Append(")");
        }

        protected override void Generate(EnsureSchemaOperation operation, IModel model, RelationalCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            // PostgreSQL 9.3 and above ahve a CREATE SCHEMA IF NOT EXISTS which is perfect for this.
            // But in order to support < 9.3 we create a plpgsql function and call it.
            builder
                .AppendLine("CREATE OR REPLACE FUNCTION pg_temp.__ef_ensure_schema () RETURNS VOID AS $$")
                .Append("  BEGIN IF NOT EXISTS (SELECT 1 FROM pg_namespace WHERE nspname='").Append(operation.Name).AppendLine("') THEN")
                .Append("    CREATE SCHEMA ").Append(operation.Name).AppendLine(";")
                .AppendLine("  END IF; END")
                .AppendLine("$$ LANGUAGE 'plpgsql';")
                .AppendLine(SqlGenerator.BatchCommandSeparator);

            builder.Append("SELECT pg_temp.__ef_ensure_schema()").AppendLine(SqlGenerator.BatchCommandSeparator);
        }

        public virtual void Generate(NpgsqlCreateDatabaseOperation operation, IModel model, RelationalCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            builder
                .Append("CREATE DATABASE ")
                .Append(SqlGenerator.DelimitIdentifier(operation.Name))
                .AppendLine(SqlGenerator.BatchCommandSeparator);
        }

        public virtual void Generate(NpgsqlDropDatabaseOperation operation, IModel model, RelationalCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            var dbName = SqlGenerator.DelimitIdentifier(operation.Name);

            builder
                // TODO: The following revokes connection only for the public role, what about other connecting roles?
                .Append("REVOKE CONNECT ON DATABASE ")
                .Append(dbName)
                .Append(" FROM PUBLIC")
                .AppendLine(SqlGenerator.BatchCommandSeparator)
                // TODO: For PG <= 9.1, the column name is prodpic, not pid (see http://stackoverflow.com/questions/5408156/how-to-drop-a-postgresql-database-if-there-are-active-connections-to-it)
                .Append(
                    "SELECT pg_terminate_backend(pg_stat_activity.pid) FROM pg_stat_activity WHERE pg_stat_activity.datname = '")
                .Append(operation.Name)
                .Append("'")
                .AppendLine(SqlGenerator.BatchCommandSeparator)
                .Append("DROP DATABASE ")
                .Append(dbName);
        }

        protected override void Generate(DropIndexOperation operation, IModel model, RelationalCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            builder
                .Append("DROP INDEX ")
                .Append(SqlGenerator.DelimitIdentifier(operation.Name, operation.Schema));
        }

        protected override void Generate(
            [NotNull] RenameColumnOperation operation,
            [CanBeNull] IModel model,
            [NotNull] RelationalCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            builder.Append("ALTER TABLE ")
                .Append(SqlGenerator.DelimitIdentifier(operation.Table, operation.Schema))
                .Append(" RENAME COLUMN ")
                .Append(SqlGenerator.DelimitIdentifier(operation.Name))
                .Append(" TO ")
                .Append(SqlGenerator.DelimitIdentifier(operation.NewName));
        }

        protected override void ColumnDefinition(
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
            RelationalCommandListBuilder builder)
        {
            Check.NotEmpty(name, nameof(name));
            Check.NotNull(annotatable, nameof(annotatable));
            Check.NotNull(clrType, nameof(clrType));
            Check.NotNull(builder, nameof(builder));

            if (type == null)
            {
                var property = FindProperty(model, schema, table, name);
                type = property != null
                    ? TypeMapper.GetMapping(property).DefaultTypeName
                    : TypeMapper.GetMapping(clrType).DefaultTypeName;
            }

            // TODO: Maybe implement computed columns via functions?
            // http://stackoverflow.com/questions/11165450/store-common-query-as-column/11166268#11166268

            var serial = annotatable[NpgsqlAnnotationNames.Prefix + NpgsqlAnnotationNames.Serial];
            if (serial != null && (bool)serial)
            {
                switch (type)
                {
                case "int":
                case "int4":
                    type = "serial";
                    break;
                case "bigint":
                case "int8":
                    type = "bigserial";
                    break;
                case "smallint":
                case "int2":
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
            [NotNull] RelationalCommandListBuilder builder)
        {
            Check.NotEmpty(name, nameof(name));
            Check.NotEmpty(newName, nameof(newName));
            Check.NotEmpty(type, nameof(type));
            Check.NotNull(builder, nameof(builder));


            builder
                .Append("ALTER ")
                .Append(type)
                .Append(" ")
                .Append(SqlGenerator.DelimitIdentifier(name, schema))
                .Append(" RENAME TO ")
                .Append(SqlGenerator.DelimitIdentifier(newName));
        }

        public virtual void Transfer(
            [NotNull] string newSchema,
            [CanBeNull] string schema,
            [NotNull] string name,
            [NotNull] string type,
            [NotNull] RelationalCommandListBuilder builder)
        {
            Check.NotEmpty(newSchema, nameof(newSchema));
            Check.NotEmpty(name, nameof(name));
            Check.NotNull(type, nameof(type));
            Check.NotNull(builder, nameof(builder));

            builder
                .Append("ALTER ")
                .Append(type)
                .Append(" ")
                .Append(SqlGenerator.DelimitIdentifier(name, schema))
                .Append(" SET SCHEMA ")
                .Append(SqlGenerator.DelimitIdentifier(newSchema));
        }

        protected override void ForeignKeyAction(ReferentialAction referentialAction, RelationalCommandListBuilder builder)
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

        string ColumnList(string[] columns) => string.Join(", ", columns.Select(SqlGenerator.DelimitIdentifier));
    }
}
