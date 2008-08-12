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