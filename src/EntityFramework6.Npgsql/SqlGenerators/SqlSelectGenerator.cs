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
using System.Linq;
using System.Collections.Generic;
using System.Data.Common;
#if ENTITIES6
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Metadata.Edm;
#else
using System.Data.Common.CommandTrees;
using System.Data.Metadata.Edm;
#endif

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
            /*
             * Algorithm for finding the correct reference expression: "Collection"."Name"
             * The collection is always a leaf InputExpression, found by lookup in _refToNode.
             * The name for the collection is found using node.TopName.
             *
             * We must now follow the path from the leaf down to the root,
             * and make sure the column is projected all the way down.
             *
             * We need not project columns at a current InputExpression.
             * For example, in
             *  SELECT ? FROM <from> AS "X" WHERE "X"."field" = <value>
             * we use the property "field" but it should not be projected.
             * Current expressions are stored in _currentExpressions.
             * There can be many of these, for example if we are in a WHERE EXISTS (SELECT ...) or in the right hand side of an Apply expression.
             *
             * At join nodes, column names might have to be renamed, if a name collision occurs.
             * For example, the following would be illegal,
             *  SELECT "X"."A" AS "A", "Y"."A" AS "A" FROM (SELECT 1 AS "A") AS "X" CROSS JOIN (SELECT 1 AS "A") AS "Y"
             * so we write
             *  SELECT "X"."A" AS "A", "Y"."A" AS "A_Alias<N>" FROM (SELECT 1 AS "A") AS "X" CROSS JOIN (SELECT 1 AS "A") AS "Y"
             * The new name is then propagated down to the root.
             */

            string name = expression.Property.Name;
            string from = (expression.Instance.ExpressionKind == DbExpressionKind.Property)
                ? ((DbPropertyExpression)expression.Instance).Property.Name
                : ((DbVariableReferenceExpression)expression.Instance).VariableName;

            PendingProjectsNode node = _refToNode[from];
            from = node.TopName;
            while (node != null)
            {
                foreach (var item in node.Selects)
                {
                    if (_currentExpressions.Contains(item.Exp))
                        continue;

                    var use = new StringPair(from, name);

                    if (!item.Exp.ColumnsToProject.ContainsKey(use))
                    {
                        var oldName = name;
                        while (item.Exp.ProjectNewNames.Contains(name))
                            name = oldName + "_" + NextAlias();
                        item.Exp.ColumnsToProject[use] = name;
                        item.Exp.ProjectNewNames.Add(name);
                    }
                    else
                    {
                        name = item.Exp.ColumnsToProject[use];
                    }
                    from = item.AsName;
                }
                node = node.JoinParent;
            }
            return new ColumnReferenceExpression { Variable = from, Name = name };
        }

        public override VisitedExpression Visit(DbNullExpression expression)
        {
            // must provide a NULL of the correct type
            // this is necessary for certain types of union queries.
            return new CastExpression(new LiteralExpression("NULL"), GetDbType(expression.ResultType.EdmType));
        }

        public override void BuildCommand(DbCommand command)
        {
            System.Diagnostics.Debug.Assert(command is NpgsqlCommand);
            System.Diagnostics.Debug.Assert(_commandTree.Query is DbProjectExpression);
            VisitedExpression ve = _commandTree.Query.Accept(this);
            System.Diagnostics.Debug.Assert(ve is InputExpression);
            InputExpression pe = (InputExpression)ve;
            command.CommandText = pe.ToString();

            // We retrieve all strings as unknowns in text format in the case the data types aren't really texts
            ((NpgsqlCommand)command).UnknownResultTypeList = pe.Projection.Arguments.Select(a => ((PrimitiveType)((ColumnExpression)a).ColumnType.EdmType).PrimitiveTypeKind == PrimitiveTypeKind.String).ToArray();

            // We must treat sbyte and DateTimeOffset specially so the value is read correctly
            if (pe.Projection.Arguments.Any(a => {
                var kind = ((PrimitiveType)((ColumnExpression)a).ColumnType.EdmType).PrimitiveTypeKind;
                return kind == PrimitiveTypeKind.SByte || kind == PrimitiveTypeKind.DateTimeOffset;
            }))
            {
                ((NpgsqlCommand)command).ObjectResultTypes = pe.Projection.Arguments.Select(a => {
                    var kind = ((PrimitiveType)((ColumnExpression)a).ColumnType.EdmType).PrimitiveTypeKind;
                    if (kind == PrimitiveTypeKind.SByte)
                    {
                        return typeof(sbyte);
                    }
                    else if (kind == PrimitiveTypeKind.DateTimeOffset)
                    {
                        return typeof(DateTimeOffset);
                    }
                    return null;
                }).ToArray();
            }
        }
    }
}
