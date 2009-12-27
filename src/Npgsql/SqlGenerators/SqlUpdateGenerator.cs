#if ENTITIES
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Common.CommandTrees;

namespace Npgsql.SqlGenerators
{
    class SqlUpdateGenerator : SqlBaseGenerator
    {
        private DbUpdateCommandTree _commandTree;

        public SqlUpdateGenerator(DbUpdateCommandTree commandTree)
        {
            _commandTree = commandTree;
        }

        public override VisitedExpression Visit(DbPropertyExpression expression)
        {
            DbVariableReferenceExpression variable = expression.Instance as DbVariableReferenceExpression;
            if (variable == null || variable.VariableName != _projectVarName.Peek())
                throw new NotSupportedException();
            return new PropertyExpression(expression.Property);
        }

        public override void BuildCommand(DbCommand command)
        {
            // TODO: handle _commandTree.Returning and _commandTree.Parameters
            UpdateExpression update = new UpdateExpression();
            _projectVarName.Push(_commandTree.Target.VariableName);
            update.AppendTarget(_commandTree.Target.Expression.Accept(this));
            foreach (DbSetClause clause in _commandTree.SetClauses)
            {
                update.AppendSet(clause.Property.Accept(this), clause.Value.Accept(this));
            }
            if (_commandTree.Predicate != null)
            {
                update.AppendWhere(_commandTree.Predicate.Accept(this));
            }
            _projectVarName.Pop();
            command.CommandText = update.ToString();
        }
	}
}
#endif