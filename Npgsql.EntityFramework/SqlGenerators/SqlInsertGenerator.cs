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
    internal class SqlInsertGenerator : SqlBaseGenerator
    {
        private DbInsertCommandTree _commandTree;
        private string _tableName;

        public SqlInsertGenerator(DbInsertCommandTree commandTree, NpgsqlProviderManifest providerManifest)
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
            // TODO: handle_commandTree.Parameters
            InsertExpression insert = new InsertExpression();
            _tableName = _commandTree.Target.VariableName;
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
                insert.AppendReturning(_commandTree.Returning as DbNewInstanceExpression);
            }
            _tableName = null;
            command.CommandText = insert.ToString();
        }
    }
}
