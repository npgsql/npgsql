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
            return new PropertyExpression(variable, expression.Property);
        }

        public override VisitedExpression Visit(DbNullExpression expression)
        {
            // must provide a NULL of the correct type
            // this is necessary for certain types of union queries.
            return new CastExpression(new LiteralExpression("NULL"), GetDbType(expression.ResultType.EdmType));
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