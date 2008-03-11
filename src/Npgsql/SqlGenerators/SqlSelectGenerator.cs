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

        public override void BuildCommand(DbCommand command)
        {
            System.Diagnostics.Debug.Assert(_commandTree.Query is DbProjectExpression);
            VisitedExpression ve = _commandTree.Query.Accept(this);
            command.CommandText = ve.ToString();
        }
	}
}
#endif