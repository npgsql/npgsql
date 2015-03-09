using System;
using System.Collections.Generic;
using System.Data.Common;
#if ENTITIES6
using System.Data.Entity.Core.Common.CommandTrees;
#else
using System.Data.Common.CommandTrees;
#endif

namespace Npgsql.SqlGenerators
{
    internal class SqlDeleteGenerator : SqlBaseGenerator
    {
        private DbDeleteCommandTree _commandTree;
        private string _tableName;

        public SqlDeleteGenerator(DbDeleteCommandTree commandTree, NpgsqlProviderManifest providerManifest)
            : base(providerManifest)
        {
            _commandTree = commandTree;
        }

        public override VisitedExpression Visit(DbPropertyExpression expression)
        {
            DbVariableReferenceExpression variable = expression.Instance as DbVariableReferenceExpression;
            if (variable == null || variable.VariableName != _tableName)
                throw new NotSupportedException();
            return new PropertyExpression(expression.Property);
        }

        public override void BuildCommand(DbCommand command)
        {
            // TODO: handle _commandTree.Returning and _commandTree.Parameters
            DeleteExpression delete = new DeleteExpression();
            _tableName = _commandTree.Target.VariableName;
            delete.AppendFrom(_commandTree.Target.Expression.Accept(this));
            if (_commandTree.Predicate != null)
            {
                delete.AppendWhere(_commandTree.Predicate.Accept(this));
            }
            _tableName = null;
            command.CommandText = delete.ToString();
        }
    }
}
