using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Relational;
using Microsoft.Data.Entity.Relational.Update;
using Microsoft.Data.Entity.Utilities;

namespace EntityFramework.Npgsql.Extensions
{
    public class NpgsqlSqlGenerator : SqlGenerator
    {
		public override void AppendInsertOperation(
			StringBuilder commandStringBuilder,
			ModificationCommand command)
		{
			Check.NotNull(command, "command");

			AppendBulkInsertOperation(commandStringBuilder, new[] { command });
		}

		public virtual ResultsGrouping AppendBulkInsertOperation(
			[NotNull] StringBuilder commandStringBuilder, 
			[NotNull] IReadOnlyList<ModificationCommand> modificationCommands)
        {
            Check.NotNull(commandStringBuilder, "commandStringBuilder");
            Check.NotEmpty(modificationCommands, "modificationCommands");

			var tableName = modificationCommands[0].TableName;
			var schemaName = modificationCommands[0].SchemaName;

			// TODO: Support TPH
			var defaultValuesOnly = !modificationCommands.First().ColumnModifications.Any(o => o.IsWrite);
            var statementCount = defaultValuesOnly ? modificationCommands.Count : 1;
            var valueSetCount = defaultValuesOnly ? 1 : modificationCommands.Count;

            for ( var i = 0; i < statementCount; i++ )
            {
                var operations = modificationCommands[i].ColumnModifications;
                var writeOperations = operations.Where(o => o.IsWrite).ToArray();
                var readOperations = operations.Where(o => o.IsRead).ToArray();

                AppendInsertCommandHeader(commandStringBuilder, tableName, schemaName, writeOperations);

                AppendValuesHeader(commandStringBuilder, writeOperations);
                AppendValues(commandStringBuilder, writeOperations);
                for ( var j = 1; j < valueSetCount; j++ )
                {
                    commandStringBuilder.Append(",").AppendLine();
                    AppendValues(commandStringBuilder, modificationCommands[j].ColumnModifications.Where(o => o.IsWrite).ToArray());
                }

                if ( readOperations.Length > 0 )
                {
                    AppendInsertReturningClause(commandStringBuilder, readOperations);
                }

                commandStringBuilder.Append(BatchCommandSeparator).AppendLine();

                if ( readOperations.Length == 0 )
                {
                    AppendSelectAffectedCountCommand(commandStringBuilder, tableName, schemaName);
                }
            }

            return defaultValuesOnly ? ResultsGrouping.OneCommandPerResultSet : ResultsGrouping.OneResultSet;
        }

        public override void AppendUpdateOperation(StringBuilder commandStringBuilder,  ModificationCommand command)
        {
            Check.NotNull(commandStringBuilder, "commandStringBuilder");
            Check.NotNull(command, "command");

			var tableName = command.TableName;
			var schemaName = command.SchemaName;
			var operations = command.ColumnModifications;

			var writeOperations = operations.Where(o => o.IsWrite).ToArray();
            var conditionOperations = operations.Where(o => o.IsCondition).ToArray();
            var readOperations = operations.Where(o => o.IsRead).ToArray();

            AppendUpdateCommandHeader(commandStringBuilder, tableName, schemaName, writeOperations);

            AppendWhereClause(commandStringBuilder, conditionOperations);

            if ( writeOperations.Length > 0 )
            {
                AppendUpdateReturningClause(commandStringBuilder, writeOperations);
            }

            commandStringBuilder.Append(BatchCommandSeparator).AppendLine();
        }

        public override void AppendDeleteOperation([NotNull] StringBuilder commandStringBuilder, [NotNull] ModificationCommand command)
        {
            Check.NotNull(command, "command");

			var tableName = command.TableName;
            var schemaName = command.SchemaName;
            var conditionOperations = command.ColumnModifications.Where(o => o.IsCondition).ToArray();

            AppendDeleteCommand(commandStringBuilder, tableName, schemaName, conditionOperations);
        }

        public override void AppendDeleteCommand([NotNull] StringBuilder commandStringBuilder, string tableName, string schemaName, [NotNull] IReadOnlyList<ColumnModification> conditionOperations)
        {
            Check.NotNull(commandStringBuilder, "commandStringBuilder");
            Check.NotNull(conditionOperations, "conditionOperations");

            AppendDeleteCommandHeader(commandStringBuilder, tableName, schemaName);
            AppendWhereClause(commandStringBuilder, conditionOperations);
            AppendDeleteReturningClause(commandStringBuilder, conditionOperations);
            commandStringBuilder.Append(BatchCommandSeparator).AppendLine();
        }

        public override void AppendSelectAffectedCountCommand(StringBuilder commandStringBuilder, string tableName, string schemaName)
        {
            // No @@ROWCOUNT in Npgsql

            // ToDo : Npgsql ?
        }

        public override void AppendBatchHeader(StringBuilder commandStringBuilder)
        {
            // No NOCOUNT in Npgsql

            // ToDo : Npgsql ?
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
            // No @@ROWCOUNT in Npgsql

            // ToDo : Npgsql ?
        }

        public enum ResultsGrouping
        {
            OneResultSet,
            OneCommandPerResultSet
        }

        private void AppendInsertReturningClause(StringBuilder commandStringBuilder, IReadOnlyList<ColumnModification> operations)
        {
            commandStringBuilder.AppendLine().Append(" RETURNING ");
            bool first = true;
            foreach ( var operation in operations )
            {
                if ( !first )
                    commandStringBuilder.Append(",");
                commandStringBuilder.Append(DelimitIdentifier(operation.ColumnName));
                first = false;
            }
        }

        private void AppendUpdateReturningClause(StringBuilder commandStringBuilder, IReadOnlyList<ColumnModification> operations)
        {
            commandStringBuilder.AppendLine().Append(" RETURNING 1 ");

            // For now append 'RETURNING 1' but shouldn't the changed colums be returned here (code below) ?

            //commandStringBuilder.AppendLine().Append(" RETURNING ");
            //bool first = true;
            //foreach ( var operation in operations )
            //{
            //    if ( !first )
            //        commandStringBuilder.Append(",");
            //    commandStringBuilder.Append(DelimitIdentifier(operation.ColumnName));
            //    first = false;
            //}
        }

        private void AppendDeleteReturningClause(StringBuilder commandStringBuilder, IReadOnlyList<ColumnModification> operations)
        {
            commandStringBuilder.AppendLine().Append(" RETURNING 1 ");
        }
        
        //public override void AppendInsertOperation([NotNull] StringBuilder commandStringBuilder, [NotNull] ModificationCommand command)
        //{
        //    Check.NotNull(command, "command");

        //    var schemaQualifiedName = command.SchemaQualifiedName;
        //    var operations = command.ColumnModifications;

        //    var writeOperations = operations.Where(o => o.IsWrite).ToArray();
        //    var readOperations = operations.Where(o => o.IsRead).ToArray();

        //    AppendInsertCommand(commandStringBuilder, schemaQualifiedName, writeOperations);

        //    if ( readOperations.Length > 0 )
        //    {
        //        var keyOperations = operations.Where(o => o.IsKey).ToArray();

        //        AppendSelectAffectedCommand(commandStringBuilder, schemaQualifiedName, readOperations, keyOperations);
        //    }
        //    else
        //    {
        //        AppendSelectAffectedCountCommand(commandStringBuilder, schemaQualifiedName);
        //    }
        //}
    }
}