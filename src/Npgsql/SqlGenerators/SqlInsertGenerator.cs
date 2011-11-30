#if ENTITIES
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Common.CommandTrees;

namespace Npgsql.SqlGenerators
{
    internal class SqlInsertGenerator : SqlBaseGenerator
	{
        private DbInsertCommandTree _commandTree;
        private bool _processingReturning;

        public SqlInsertGenerator(DbInsertCommandTree commandTree)
        {
            _commandTree = commandTree;
        }

        public override VisitedExpression Visit(DbPropertyExpression expression)
        {
            DbVariableReferenceExpression variable = expression.Instance as DbVariableReferenceExpression;
            if (variable == null || variable.VariableName != _projectVarName.Peek())
                throw new NotSupportedException();
            if (!_processingReturning)
            {
                return new PropertyExpression(expression.Property);
            }
            else
            {
                // the table name needs to be quoted, the column name does not.
                // http://archives.postgresql.org/pgsql-bugs/2007-01/msg00102.php
                string tableName = QuoteIdentifier(_variableSubstitution[variable.VariableName]);
                if (variable.VariableName == _commandTree.Target.VariableName)
                {
                    // try to get the table name schema qualified.
                    DbScanExpression scan = _commandTree.Target.Expression as DbScanExpression;
                    if (scan != null)
                    {
                        System.Data.Metadata.Edm.MetadataProperty metadata;
                        string overrideSchema = "http://schemas.microsoft.com/ado/2007/12/edm/EntityStoreSchemaGenerator:Schema";
                        if (scan.Target.MetadataProperties.TryGetValue(overrideSchema, false, out metadata) && metadata.Value != null)
                        {
                            tableName = QuoteIdentifier(metadata.Value.ToString()) + "." + tableName;
                        }
                        else if (scan.Target.MetadataProperties.TryGetValue("Schema", false, out metadata) && metadata.Value != null)
                        {
                            tableName = QuoteIdentifier(metadata.Value.ToString()) + "." + tableName;
                        }
                        else
                        {
                            tableName = QuoteIdentifier(scan.Target.EntityContainer.Name) + "." + tableName;
                        }
                    }
                }
                return new LiteralExpression("currval(pg_get_serial_sequence('" + tableName + "', '" + expression.Property.Name + "'))");
            }
        }

        public override void BuildCommand(DbCommand command)
        {
            // TODO: handle_commandTree.Parameters
            InsertExpression insert = new InsertExpression();
            _projectVarName.Push(_commandTree.Target.VariableName);
            insert.AppendTarget(_commandTree.Target.Expression.Accept(this));
            List<VisitedExpression> columns = new List<VisitedExpression>();
            List<VisitedExpression> values = new List<VisitedExpression>();
            foreach (DbSetClause clause in _commandTree.SetClauses)
            {
                columns.Add(clause.Property.Accept(this));
                values.Add(clause.Value.Accept(this));
            }
            insert.AppendColumns(columns);
            insert.AppendValues(values);
            if (_commandTree.Returning != null)
            {
                _processingReturning = true;
                insert.ReturningExpression = _commandTree.Returning.Accept(this);
            }
            _projectVarName.Pop();
            command.CommandText = insert.ToString();
        }
	}
}
#endif