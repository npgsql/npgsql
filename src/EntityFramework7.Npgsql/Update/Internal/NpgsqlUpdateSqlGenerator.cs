// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Storage;
using Microsoft.Data.Entity.Utilities;

namespace Microsoft.Data.Entity.Update.Internal
{
    public class NpgsqlUpdateSqlGenerator : UpdateSqlGenerator
    {
        public NpgsqlUpdateSqlGenerator([NotNull] ISqlGenerator sqlGenerator)
            : base(sqlGenerator)
        {
        }

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
            var schemaName = modificationCommands[0].Schema;

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
                commandStringBuilder.Append(SqlGenerator.BatchCommandSeparator).AppendLine();
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
            var schemaName = command.Schema;
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
            commandStringBuilder.Append(SqlGenerator.BatchCommandSeparator).AppendLine();
        }

        // ReSharper disable once ParameterTypeCanBeEnumerable.Local
        private void AppendReturningClause(
            StringBuilder commandStringBuilder,
            IReadOnlyList<ColumnModification> operations)
        {
            commandStringBuilder
                .AppendLine()
                .Append("RETURNING ")
                .AppendJoin(operations.Select(c => SqlGenerator.DelimitIdentifier(c.ColumnName)));
        }

        // This function is a temporary workaround for
        // https://github.com/aspnet/EntityFramework/issues/3023
        protected override void AppendWhereCondition(
            [NotNull] StringBuilder commandStringBuilder,
            [NotNull] ColumnModification columnModification,
            bool useOriginalValue)
        {
            Check.NotNull(commandStringBuilder, nameof(commandStringBuilder));
            Check.NotNull(columnModification, nameof(columnModification));

            // In PostgreSQL, doing WHERE x = @p doesn't work if @p is null.
            // IS NOT DISTINCT FROM does the same thing as equality but also returns true for null comparison.
            // http://www.postgresql.org/docs/current/static/functions-comparison.html
            commandStringBuilder
                .Append(SqlGenerator.DelimitIdentifier(columnModification.ColumnName))
                .Append(" IS NOT DISTINCT FROM ")
                .Append(
                    SqlGenerator.GenerateParameterName(
                        useOriginalValue
                            ? columnModification.OriginalParameterName
                            : columnModification.ParameterName));
        }

        public override void AppendBatchHeader(StringBuilder commandStringBuilder)
        {
            Check.NotNull(commandStringBuilder, nameof(commandStringBuilder));

            // TODO: Npgsql
        }

        protected override void AppendIdentityWhereCondition(StringBuilder commandStringBuilder, ColumnModification columnModification)
        {
            throw new NotImplementedException();
        }

        protected override void AppendRowsAffectedWhereCondition(StringBuilder commandStringBuilder, int expectedRowsAffected)
        {
            throw new NotImplementedException();
        }

        public enum ResultsGrouping
        {
            OneResultSet,
            OneCommandPerResultSet
        }
    }
}
