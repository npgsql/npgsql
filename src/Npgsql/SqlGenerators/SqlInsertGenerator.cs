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
            commandTree.Validate();

            _commandTree = commandTree;
        }

        public override VisitedExpression Visit(DbPropertyExpression expression)
        {
            DbVariableReferenceExpression variable = expression.Instance as DbVariableReferenceExpression;
            if (variable == null || variable.VariableName != _projectVarName.Peek())
                throw new NotSupportedException();
            if (!_processingReturning)
            {
                return new LiteralExpression(expression.Property.Name);
            }
            else
            {
                return new LiteralExpression("currval(pg_get_serial_sequence('" + _variableSubstitution[variable.VariableName] + "', '" + expression.Property.Name + "'))");
            }
        }

        public override void BuildCommand(DbCommand command)
        {
            // TODO: handle _commandTree.Returning and _commandTree.Parameters
            InsertExpression insert = new InsertExpression();
            _projectVarName.Push(_commandTree.Target.VariableName);
            insert.Append(_commandTree.Target.Expression.Accept(this));
            VisitedExpression columnList = new LiteralExpression("(");
            VisitedExpression valueList = new LiteralExpression("(");
            bool first = true;
            foreach (DbSetClause clause in _commandTree.SetClauses)
            {
                if (!first)
                {
                    columnList.Append(",");
                    valueList.Append(",");
                }
                columnList.Append(clause.Property.Accept(this));
                valueList.Append(clause.Value.Accept(this));
                first = false;
            }
            columnList.Append(")");
            valueList.Append(")");
            insert.Append(columnList);
            insert.Append(" VALUES ");
            insert.Append(valueList);
            if (_commandTree.Returning != null)
            {
                insert.Append(";");
                _processingReturning = true;
                insert.Append(_commandTree.Returning.Accept(this));
            }
            _projectVarName.Pop();
            command.CommandText = insert.ToString();
        }
	}
}
#endif