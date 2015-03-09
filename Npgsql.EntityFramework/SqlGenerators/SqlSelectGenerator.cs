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
    internal class SqlSelectGenerator : SqlBaseGenerator
    {
        private DbQueryCommandTree _commandTree;

        public SqlSelectGenerator(DbQueryCommandTree commandTree, NpgsqlProviderManifest providerManifest)
            : base(providerManifest)
        {
            _commandTree = commandTree;
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
            List<Type> expectedTypes = new List<Type>();
            foreach (ColumnExpression column in pe.Projection.Arguments)
            {
                expectedTypes.Add(column.CLRType);
            }
            ((NpgsqlCommand)command).ExpectedTypes = expectedTypes.ToArray();
        }
    }
}
