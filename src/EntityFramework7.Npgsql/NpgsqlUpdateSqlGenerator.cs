// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Update;
using Microsoft.Data.Entity.Utilities;

namespace EntityFramework7.Npgsql
{
    public class NpgsqlUpdateSqlGenerator : UpdateSqlGenerator
    {
        public override void AppendInsertOperation(
            StringBuilder commandStringBuilder,
            ModificationCommand command)
        {
            Check.NotNull(command, nameof(command));

            AppendBulkInsertOperation(commandStringBuilder, new[] { command });
        }

        public virtual ResultsGrouping AppendBulkInsertOperation(
            StringBuilder commandStringBuilder,
            IReadOnlyList<ModificationCommand> modificationCommands)
        {
            Check.NotNull(commandStringBuilder, nameof(commandStringBuilder));
            Check.NotEmpty(modificationCommands, nameof(modificationCommands));

            var tableName = modificationCommands[0].TableName;
            var schemaName = modificationCommands[0].SchemaName;

            // TODO: Support TPH
            var defaultValuesOnly = !modificationCommands.First().ColumnModifications.Any(o => o.IsWrite);
            var statementCount = defaultValuesOnly
                ? modificationCommands.Count
                : 1;
            var valueSetCount = defaultValuesOnly
                ? 1
                : modificationCommands.Count;

            for (var i = 0; i < statementCount; i++)
            {
                var operations = modificationCommands[i].ColumnModifications;
                var writeOperations = operations.Where(o => o.IsWrite).ToArray();
                var readOperations = operations.Where(o => o.IsRead).ToArray();

                AppendInsertCommandHeader(commandStringBuilder, tableName, schemaName, writeOperations);
                AppendValuesHeader(commandStringBuilder, writeOperations);
                AppendValues(commandStringBuilder, writeOperations);
                for (var j = 1; j < valueSetCount; j++)
                {
                    commandStringBuilder.Append(",").AppendLine();
                    AppendValues(commandStringBuilder, modificationCommands[j].ColumnModifications.Where(o => o.IsWrite).ToArray());
                }
                if (readOperations.Length > 0)
                {
                    AppendReturningClause(commandStringBuilder, readOperations);
                }
                commandStringBuilder.Append(BatchCommandSeparator).AppendLine();
            }

            return defaultValuesOnly
                ? ResultsGrouping.OneCommandPerResultSet
                : ResultsGrouping.OneResultSet;
        }

        public override void AppendUpdateOperation(
            StringBuilder commandStringBuilder,
            ModificationCommand command)
        {
            Check.NotNull(commandStringBuilder, nameof(commandStringBuilder));
            Check.NotNull(command, nameof(command));

            var tableName = command.TableName;
            var schemaName = command.SchemaName;
            var operations = command.ColumnModifications;

            var writeOperations = operations.Where(o => o.IsWrite).ToArray();
            var conditionOperations = operations.Where(o => o.IsCondition).ToArray();
            var readOperations = operations.Where(o => o.IsRead).ToArray();

            AppendUpdateCommandHeader(commandStringBuilder, tableName, schemaName, writeOperations);
            AppendWhereClause(commandStringBuilder, conditionOperations);
            if (readOperations.Length > 0)
            {
                AppendReturningClause(commandStringBuilder, readOperations);
            }
        }

        // ReSharper disable once ParameterTypeCanBeEnumerable.Local
        private void AppendReturningClause(
            StringBuilder commandStringBuilder,
            IReadOnlyList<ColumnModification> operations)
        {
            commandStringBuilder
                .AppendLine()
                .Append("RETURNING ")
                .AppendJoin(operations.Select(c => DelimitIdentifier(c.ColumnName)));
        }

        public override void AppendBatchHeader(StringBuilder commandStringBuilder)
        {
            Check.NotNull(commandStringBuilder, nameof(commandStringBuilder));

            // TODO: Npgsql
        }

        protected override void AppendIdentityWhereCondition(StringBuilder commandStringBuilder, ColumnModification columnModification)
        {
            commandStringBuilder
                .Append(DelimitIdentifier(columnModification.ColumnName))
                .Append(" = ")
                .Append("scope_identity()");
        }

        protected override void AppendRowsAffectedWhereCondition(StringBuilder commandStringBuilder, int expectedRowsAffected)
        {
            Check.NotNull(commandStringBuilder, nameof(commandStringBuilder));

            commandStringBuilder
                .Append("@@ROWCOUNT = " + expectedRowsAffected);
        }

        public override string BatchSeparator => "GO";

        public override string DelimitIdentifier(string identifier)
        {
            Check.NotEmpty(identifier, nameof(identifier));

            return '"' + EscapeIdentifier(identifier) + '"';
        }

        public override string EscapeIdentifier(string identifier)
        {
            Check.NotEmpty(identifier, nameof(identifier));

            return identifier.Replace("\"", "\"\"");
        }

        public override string GenerateLiteral(byte[] literal)
        {
            Check.NotNull(literal, nameof(literal));

            var builder = new StringBuilder();

            builder.Append("0x");

            var parts = literal.Select(b => b.ToString("X2", CultureInfo.InvariantCulture));
            foreach (var part in parts)
            {
                builder.Append(part);
            }

            return builder.ToString();
        }

        public override string GenerateLiteral(bool literal) => literal ? "1" : "0";
        public override string GenerateLiteral(DateTime literal) => "'" + literal.ToString(@"yyyy-MM-dd HH\:mm\:ss.fffffff") + "'";
        public override string GenerateLiteral(DateTimeOffset literal) => "'" + literal.ToString(@"yyyy-MM-dd HH\:mm\:ss.fffffffzzz") + "'";
        public virtual string GenerateLiteral(Guid literal) => "'" + literal + "'";

        public enum ResultsGrouping
        {
            OneResultSet,
            OneCommandPerResultSet
        }

        #region Npsql-specific

        public override string GenerateNextSequenceValueOperation(string sequenceName)
        {
            Check.NotNull(sequenceName, nameof(sequenceName));

            //TODO:Verify implementation

            //split string using '.' to extract schema and name parts
            var parts = sequenceName.Split(new char[] {'.'});

            //TODO: should we throw exceptions if parts is empty or has more than 2 components?

            //synthesize the sequence name
            //The name itself must be delimited, but the schema should not
            var actualSequenceName = (parts.Length == 1) ? DelimitIdentifier(parts[0]) : $"{parts[0]}.{DelimitIdentifier(parts[1])}";

            return string.Format("SELECT nextval('{0}')", actualSequenceName);
        }

        #endregion
    }
}
