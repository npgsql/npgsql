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

        public SqlInsertGenerator(DbInsertCommandTree commandTree)
        {
            commandTree.Validate();

            _commandTree = commandTree;
        }

        public override void BuildCommand(DbCommand command)
        {
            VisitedExpression ve = _commandTree.Target.Expression.Accept(this);
            foreach (DbModificationClause clause in _commandTree.SetClauses)
            {
            }
            command.CommandText = ve.ToString();
        }
	}
}
#endif
