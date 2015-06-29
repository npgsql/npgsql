#region License
// The PostgreSQL License
//
// Copyright (C) 2015 The Npgsql Development Team
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
#endregion

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

        public SqlUpdateGenerator(DbUpdateCommandTree commandTree)
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
