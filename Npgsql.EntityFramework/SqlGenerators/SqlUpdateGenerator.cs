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
    class SqlUpdateGenerator : SqlBaseGenerator
    {
        private DbUpdateCommandTree _commandTree;
        private string _tableName;

        public SqlUpdateGenerator(DbUpdateCommandTree commandTree, NpgsqlProviderManifest providerManifest)
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
            // TODO: handle _commandTree.Parameters
            UpdateExpression update = new UpdateExpression();
            _tableName = _commandTree.Target.VariableName;
            update.AppendTarget(_commandTree.Target.Expression.Accept(this));
            foreach (DbSetClause clause in _commandTree.SetClauses)
            {
                update.AppendSet(clause.Property.Accept(this), clause.Value.Accept(this));
            }
            if (_commandTree.Predicate != null)
            {
                update.AppendWhere(_commandTree.Predicate.Accept(this));
            }
            if (_commandTree.Returning != null)
            {
                update.AppendReturning((DbNewInstanceExpression)_commandTree.Returning);
            }
            _tableName = null;
            command.CommandText = update.ToString();
        }
    }
}
