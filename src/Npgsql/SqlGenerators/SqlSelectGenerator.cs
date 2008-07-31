#if ENTITIES
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Common.CommandTrees;

namespace Npgsql.SqlGenerators
{
	internal class SqlSelectGenerator : SqlBaseGenerator
	{
        private DbQueryCommandTree _commandTree;

        public SqlSelectGenerator(DbQueryCommandTree commandTree)
        {
            commandTree.Validate();

            _commandTree = commandTree;
        }

        protected SqlSelectGenerator()
        {
            // used only for other generators such as returning
        }

        public override VisitedExpression Visit(DbPropertyExpression expression)
        {
            // not quite sure what this does
            // may be . notation for seperating
            // scopes (such as schema.table.column)
            //VisitedExpression variable = expression.Instance.Accept(this);
            VariableReferenceExpression variable = new VariableReferenceExpression(expression.Instance.Accept(this).ToString(), _variableSubstitution);
            return new PropertyExpression(variable, expression.Property.Name);
        }

        public override void BuildCommand(DbCommand command)
        {
            System.Diagnostics.Debug.Assert(_commandTree.Query is DbProjectExpression);
            VisitedExpression ve = _commandTree.Query.Accept(this);
            command.CommandText = ve.ToString();
        }
	}
}
#endif