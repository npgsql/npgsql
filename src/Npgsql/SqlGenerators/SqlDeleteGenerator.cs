#if ENTITIES
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Common.CommandTrees;

namespace Npgsql.SqlGenerators
{
    internal class SqlDeleteGenerator : SqlBaseGenerator
    {
        private DbDeleteCommandTree _commandTree;

        public SqlDeleteGenerator(DbDeleteCommandTree commandTree)
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
            DeleteExpression delete = new DeleteExpression();
            _projectVarName.Push(_commandTree.Target.VariableName);
            delete.AppendFrom(_commandTree.Target.Expression.Accept(this));
            if (_commandTree.Predicate != null)
            {
                delete.AppendWhere(_commandTree.Predicate.Accept(this));
            }
            _projectVarName.Pop();
            command.CommandText = delete.ToString();
        }
	}
}
#endif