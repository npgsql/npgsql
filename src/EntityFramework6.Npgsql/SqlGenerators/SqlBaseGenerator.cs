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
using System.Globalization;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Metadata.Edm;
#else
using System.Data.Common.CommandTrees;
using System.Data.Metadata.Edm;
#endif
using System.Linq;
using NpgsqlTypes;

namespace Npgsql.SqlGenerators
{
    internal abstract class SqlBaseGenerator : DbExpressionVisitor<VisitedExpression>
    {
        internal NpgsqlCommand _command;
        internal bool _createParametersForConstants;
        private Version _version;
        internal Version Version { get { return _version; } set { _version = value; _useNewPrecedences = value >= new Version(9, 5); } }
        private bool _useNewPrecedences;

        protected Dictionary<string, PendingProjectsNode> _refToNode = new Dictionary<string, PendingProjectsNode>();
        protected HashSet<InputExpression> _currentExpressions = new HashSet<InputExpression>();
        protected uint _aliasCounter = 0;
        protected uint _parameterCount = 0;

        private static Dictionary<string, string> AggregateFunctionNames = new Dictionary<string, string>()
        {
            {"Avg","avg"},
            {"Count","count"},
            {"Min","min"},
            {"Max","max"},
            {"Sum","sum"},
            {"BigCount","count"},
            {"StDev","stddev_samp"},
            {"StDevP","stddev_pop"},
            {"Var","var_samp"},
            {"VarP","var_pop"},
        };

#if ENTITIES6
        private static readonly Dictionary<string, Operator> BinaryOperatorFunctionNames = new Dictionary<string, Operator>()
        {
            {"@@",Operator.QueryMatch},
            {"operator_tsquery_and",Operator.QueryAnd},
            {"operator_tsquery_or",Operator.QueryOr},
            {"operator_tsquery_contains",Operator.QueryContains},
            {"operator_tsquery_is_contained",Operator.QueryIsContained}
        };
#endif

        protected SqlBaseGenerator()
        {
        }

        private void EnterExpression(PendingProjectsNode n)
        {
            _currentExpressions.Add(n.Last.Exp);
        }
        private void LeaveExpression(PendingProjectsNode n)
        {
            _currentExpressions.Remove(n.Last.Exp);
        }

        protected string NextAlias()
        {
            return "Alias" + _aliasCounter++;
        }

        private bool IsCompatible(InputExpression child, DbExpressionKind parentKind)
        {
            switch (parentKind)
            {
                case DbExpressionKind.Filter:
                    return
                        child.Projection == null &&
                        child.GroupBy == null &&
                        child.Skip == null &&
                        child.Limit == null;
                case DbExpressionKind.GroupBy:
                    return
                        child.Projection == null &&
                        child.GroupBy == null &&
                        child.Distinct == false &&
                        child.OrderBy == null &&
                        child.Skip == null &&
                        child.Limit == null;
                case DbExpressionKind.Distinct:
                    return
                        child.OrderBy == null &&
                        child.Skip == null &&
                        child.Limit == null;
                case DbExpressionKind.Sort:
                    return
                        child.Projection == null &&
                        child.GroupBy == null &&
                        child.Skip == null &&
                        child.Limit == null;
                case DbExpressionKind.Skip:
                    return
                        child.Projection == null &&
                        child.Skip == null &&
                        child.Limit == null;
                case DbExpressionKind.Project:
                    return
                        child.Projection == null &&
                        child.Distinct == false;
                // Limit and NewInstance are always true
                default:
                    throw new ArgumentException("Unexpected parent expression kind");
            }
        }

        private PendingProjectsNode GetInput(DbExpression expression, string childBindingName, string parentBindingName, DbExpressionKind parentKind)
        {
            PendingProjectsNode n = VisitInputWithBinding(expression, childBindingName);
            if (!IsCompatible(n.Last.Exp, parentKind))
            {
                n.Selects.Add(new NameAndInputExpression(parentBindingName, new InputExpression(n.Last.Exp, n.Last.AsName)));
            }
            return n;
        }

        private PendingProjectsNode VisitInputWithBinding(DbExpression expression, string bindingName)
        {
            PendingProjectsNode n;
            switch (expression.ExpressionKind)
            {
                case DbExpressionKind.Scan:
                    {
                        ScanExpression scan = (ScanExpression)expression.Accept(this);
                        InputExpression input = new InputExpression(scan, bindingName);
                        n = new PendingProjectsNode(bindingName, input);

                        break;
                    }
                case DbExpressionKind.Filter:
                    {
                        DbFilterExpression exp = (DbFilterExpression)expression;
                        n = GetInput(exp.Input.Expression, exp.Input.VariableName, bindingName, expression.ExpressionKind);
                        EnterExpression(n);
                        VisitedExpression pred = exp.Predicate.Accept(this);
                        if (n.Last.Exp.Where == null)
                            n.Last.Exp.Where = new WhereExpression(pred);
                        else
                            n.Last.Exp.Where.And(pred);
                        LeaveExpression(n);

                        break;
                    }
                case DbExpressionKind.Sort:
                    {
                        DbSortExpression exp = (DbSortExpression)expression;
                        n = GetInput(exp.Input.Expression, exp.Input.VariableName, bindingName, expression.ExpressionKind);
                        EnterExpression(n);
                        n.Last.Exp.OrderBy = new OrderByExpression();
                        foreach (var order in exp.SortOrder)
                        {
                            n.Last.Exp.OrderBy.AppendSort(order.Expression.Accept(this), order.Ascending);
                        }
                        LeaveExpression(n);

                        break;
                    }
                case DbExpressionKind.Skip:
                    {
                        DbSkipExpression exp = (DbSkipExpression)expression;
                        n = GetInput(exp.Input.Expression, exp.Input.VariableName, bindingName, expression.ExpressionKind);
                        EnterExpression(n);
                        n.Last.Exp.OrderBy = new OrderByExpression();
                        foreach (var order in exp.SortOrder)
                        {
                            n.Last.Exp.OrderBy.AppendSort(order.Expression.Accept(this), order.Ascending);
                        }
                        n.Last.Exp.Skip = new SkipExpression(exp.Count.Accept(this));
                        LeaveExpression(n);
                        break;
                    }
                case DbExpressionKind.Distinct:
                    {
                        DbDistinctExpression exp = (DbDistinctExpression)expression;
                        string childBindingName = NextAlias();

                        n = VisitInputWithBinding(exp.Argument, childBindingName);
                        if (!IsCompatible(n.Last.Exp, expression.ExpressionKind))
                        {
                            InputExpression prev = n.Last.Exp;
                            string prevName = n.Last.AsName;
                            InputExpression input = new InputExpression(prev, prevName);
                            n.Selects.Add(new NameAndInputExpression(bindingName, input));

                            // We need to copy all the projected columns so the DISTINCT keyword will work on the correct columns
                            // A parent project expression is never compatible with this new expression,
                            // so these are the columns that finally will be projected, as wanted
                            foreach (ColumnExpression col in prev.Projection.Arguments)
                            {
                                input.ColumnsToProject.Add(new StringPair(prevName, col.Name), col.Name);
                                input.ProjectNewNames.Add(col.Name);
                            }
                        }
                        n.Last.Exp.Distinct = true;
                        break;
                    }
                case DbExpressionKind.Limit:
                    {
                        DbLimitExpression exp = (DbLimitExpression)expression;
                        n = VisitInputWithBinding(exp.Argument, NextAlias());
                        if (n.Last.Exp.Limit != null)
                        {
                            FunctionExpression least = new FunctionExpression("LEAST");
                            least.AddArgument(n.Last.Exp.Limit.Arg);
                            least.AddArgument(exp.Limit.Accept(this));
                            n.Last.Exp.Limit.Arg = least;
                        }
                        else
                        {
                            n.Last.Exp.Limit = new LimitExpression(exp.Limit.Accept(this));
                        }
                        break;
                    }
                case DbExpressionKind.NewInstance:
                    {
                        DbNewInstanceExpression exp = (DbNewInstanceExpression)expression;
                        if (exp.Arguments.Count == 1 && exp.Arguments[0].ExpressionKind == DbExpressionKind.Element)
                        {
                            n = VisitInputWithBinding(((DbElementExpression)exp.Arguments[0]).Argument, NextAlias());
                            if (n.Last.Exp.Limit != null)
                            {
                                FunctionExpression least = new FunctionExpression("LEAST");
                                least.AddArgument(n.Last.Exp.Limit.Arg);
                                least.AddArgument(new LiteralExpression("1"));
                                n.Last.Exp.Limit.Arg = least;
                            }
                            else
                            {
                                n.Last.Exp.Limit = new LimitExpression(new LiteralExpression("1"));
                            }
                        }
                        else if (exp.Arguments.Count >= 1)
                        {
                            LiteralExpression result = new LiteralExpression("(");
                            for (int i = 0; i < exp.Arguments.Count; ++i)
                            {
                                DbExpression arg = exp.Arguments[i];
                                var visitedColumn = arg.Accept(this);
                                if (!(visitedColumn is ColumnExpression))
                                    visitedColumn = new ColumnExpression(visitedColumn, "C", arg.ResultType);

                                result.Append(i == 0 ? "SELECT " : " UNION ALL SELECT ");
                                result.Append(visitedColumn);
                            }
                            result.Append(")");
                            n = new PendingProjectsNode(bindingName, new InputExpression(result, bindingName));
                        }
                        else
                        {
                            TypeUsage type = ((CollectionType)exp.ResultType.EdmType).TypeUsage;
                            LiteralExpression result = new LiteralExpression("(SELECT ");
                            result.Append(new CastExpression(new LiteralExpression("NULL"), GetDbType(type.EdmType)));
                            result.Append(" LIMIT 0)");
                            n = new PendingProjectsNode(bindingName, new InputExpression(result, bindingName));
                        }
                        break;
                    }
                case DbExpressionKind.UnionAll:
                case DbExpressionKind.Intersect:
                case DbExpressionKind.Except:
                    {
                        DbBinaryExpression exp = (DbBinaryExpression)expression;
                        DbExpressionKind expKind = exp.ExpressionKind;
                        List<VisitedExpression> list = new List<VisitedExpression>();
                        Action<DbExpression> func = null;
                        func = e =>
                        {
                            if (e.ExpressionKind == expKind && e.ExpressionKind != DbExpressionKind.Except)
                            {
                                DbBinaryExpression binaryExp = (DbBinaryExpression)e;
                                func(binaryExp.Left);
                                func(binaryExp.Right);
                            }
                            else
                            {
                                list.Add(VisitInputWithBinding(e, bindingName + "_" + list.Count).Last.Exp);
                            }
                        };
                        func(exp.Left);
                        func(exp.Right);
                        InputExpression input = new InputExpression(new CombinedProjectionExpression(expression.ExpressionKind, list), bindingName);
                        n = new PendingProjectsNode(bindingName, input);
                        break;
                    }
                case DbExpressionKind.Project:
                    {
                        DbProjectExpression exp = (DbProjectExpression)expression;
                        PendingProjectsNode child = VisitInputWithBinding(exp.Input.Expression, exp.Input.VariableName);
                        InputExpression input = child.Last.Exp;
                        bool enterScope = false;
                        if (!IsCompatible(input, expression.ExpressionKind))
                        {
                            input = new InputExpression(input, child.Last.AsName);
                        }
                        else enterScope = true;

                        if (enterScope) EnterExpression(child);

                        input.Projection = new CommaSeparatedExpression();

                        DbNewInstanceExpression projection = (DbNewInstanceExpression)exp.Projection;
                        RowType rowType = projection.ResultType.EdmType as RowType;
                        for (int i = 0; i < rowType.Properties.Count && i < projection.Arguments.Count; ++i)
                        {
                            EdmProperty prop = rowType.Properties[i];
                            input.Projection.Arguments.Add(new ColumnExpression(projection.Arguments[i].Accept(this), prop.Name, prop.TypeUsage));
                        }

                        if (enterScope) LeaveExpression(child);

                        n = new PendingProjectsNode(bindingName, input);
                        break;
                    }
                case DbExpressionKind.GroupBy:
                    {
                        DbGroupByExpression exp = (DbGroupByExpression)expression;
                        PendingProjectsNode child = VisitInputWithBinding(exp.Input.Expression, exp.Input.VariableName);

                        // I don't know why the input for GroupBy in EF have two names
                        _refToNode[exp.Input.GroupVariableName] = child;

                        InputExpression input = child.Last.Exp;
                        bool enterScope = false;
                        if (!IsCompatible(input, expression.ExpressionKind))
                        {
                            input = new InputExpression(input, child.Last.AsName);
                        }
                        else enterScope = true;

                        if (enterScope) EnterExpression(child);

                        input.Projection = new CommaSeparatedExpression();

                        input.GroupBy = new GroupByExpression();
                        RowType rowType = ((CollectionType)(exp.ResultType.EdmType)).TypeUsage.EdmType as RowType;
                        int columnIndex = 0;
                        foreach (var key in exp.Keys)
                        {
                            VisitedExpression keyColumnExpression = key.Accept(this);
                            var prop = rowType.Properties[columnIndex];
                            input.Projection.Arguments.Add(new ColumnExpression(keyColumnExpression, prop.Name, prop.TypeUsage));
                            // have no idea why EF is generating a group by with a constant expression,
                            // but postgresql doesn't need it.
                            if (!(key is DbConstantExpression))
                            {
                                input.GroupBy.AppendGroupingKey(keyColumnExpression);
                            }
                            ++columnIndex;
                        }
                        foreach (var ag in exp.Aggregates)
                        {
                            DbFunctionAggregate function = (DbFunctionAggregate)ag;
                            VisitedExpression functionExpression = VisitFunction(function);
                            var prop = rowType.Properties[columnIndex];
                            input.Projection.Arguments.Add(new ColumnExpression(functionExpression, prop.Name, prop.TypeUsage));
                            ++columnIndex;
                        }

                        if (enterScope) LeaveExpression(child);

                        n = new PendingProjectsNode(bindingName, input);
                        break;
                    }
                case DbExpressionKind.CrossJoin:
                case DbExpressionKind.FullOuterJoin:
                case DbExpressionKind.InnerJoin:
                case DbExpressionKind.LeftOuterJoin:
                case DbExpressionKind.CrossApply:
                case DbExpressionKind.OuterApply:
                    {
                        InputExpression input = new InputExpression();
                        n = new PendingProjectsNode(bindingName, input);

                        JoinExpression from = VisitJoinChildren(expression, input, n);

                        input.From = from;

                        break;
                    }
                default: throw new NotImplementedException();
            }
            _refToNode[bindingName] = n;
            return n;
        }

        private bool IsJoin(DbExpressionKind kind)
        {
            switch (kind)
            {
                case DbExpressionKind.CrossJoin:
                case DbExpressionKind.FullOuterJoin:
                case DbExpressionKind.InnerJoin:
                case DbExpressionKind.LeftOuterJoin:
                case DbExpressionKind.CrossApply:
                case DbExpressionKind.OuterApply:
                    return true;
            }
            return false;
        }

        private JoinExpression VisitJoinChildren(DbExpression expression, InputExpression input, PendingProjectsNode n)
        {
            DbExpressionBinding left, right;
            DbExpression condition = null;
            if (expression.ExpressionKind == DbExpressionKind.CrossJoin)
            {
                left = ((DbCrossJoinExpression)expression).Inputs[0];
                right = ((DbCrossJoinExpression)expression).Inputs[1];
                if (((DbCrossJoinExpression)expression).Inputs.Count > 2)
                {
                    // I have never seen more than 2 inputs in CrossJoin
                    throw new NotImplementedException();
                }
            }
            else if (expression.ExpressionKind == DbExpressionKind.CrossApply || expression.ExpressionKind == DbExpressionKind.OuterApply)
            {
                left = ((DbApplyExpression)expression).Input;
                right = ((DbApplyExpression)expression).Apply;
            }
            else
            {
                left = ((DbJoinExpression)expression).Left;
                right = ((DbJoinExpression)expression).Right;
                condition = ((DbJoinExpression)expression).JoinCondition;
            }

            return VisitJoinChildren(left.Expression, left.VariableName, right.Expression, right.VariableName, expression.ExpressionKind, condition, input, n);
        }
        private JoinExpression VisitJoinChildren(DbExpression left, string leftName, DbExpression right, string rightName, DbExpressionKind joinType, DbExpression condition, InputExpression input, PendingProjectsNode n)
        {
            JoinExpression join = new JoinExpression();
            join.JoinType = joinType;

            if (IsJoin(left.ExpressionKind))
            {
                join.Left = VisitJoinChildren(left, input, n);
            }
            else
            {
                PendingProjectsNode l = VisitInputWithBinding(left, leftName);
                l.JoinParent = n;
                join.Left = new FromExpression(l.Last.Exp, l.Last.AsName);
            }

            if (joinType == DbExpressionKind.OuterApply || joinType == DbExpressionKind.CrossApply)
            {
                EnterExpression(n);
                PendingProjectsNode r = VisitInputWithBinding(right, rightName);
                LeaveExpression(n);
                r.JoinParent = n;
                join.Right = new FromExpression(r.Last.Exp, r.Last.AsName) { ForceSubquery = true };
            }
            else
            {
                if (IsJoin(right.ExpressionKind))
                {
                    join.Right = VisitJoinChildren(right, input, n);
                }
                else
                {
                    PendingProjectsNode r = VisitInputWithBinding(right, rightName);
                    r.JoinParent = n;
                    join.Right = new FromExpression(r.Last.Exp, r.Last.AsName);
                }
            }

            if (condition != null)
            {
                EnterExpression(n);
                join.Condition = condition.Accept(this);
                LeaveExpression(n);
            }
            return join;
        }

        public override VisitedExpression Visit(DbVariableReferenceExpression expression)
        {
            //return new VariableReferenceExpression(expression.VariableName, _variableSubstitution);
            throw new NotImplementedException();
        }

        public override VisitedExpression Visit(DbUnionAllExpression expression)
        {
            // Handled by VisitInputWithBinding
            throw new NotImplementedException();
        }

        public override VisitedExpression Visit(DbTreatExpression expression)
        {
            throw new NotImplementedException();
        }

        public override VisitedExpression Visit(DbSkipExpression expression)
        {
            // Handled by VisitInputWithBinding
            throw new NotImplementedException();
        }

        public override VisitedExpression Visit(DbSortExpression expression)
        {
            // Handled by VisitInputWithBinding
            throw new NotImplementedException();
        }

        public override VisitedExpression Visit(DbScanExpression expression)
        {
            MetadataProperty metadata;
            string tableName;
            string overrideTable = "http://schemas.microsoft.com/ado/2007/12/edm/EntityStoreSchemaGenerator:Name";
            if (expression.Target.MetadataProperties.TryGetValue(overrideTable, false, out metadata) && metadata.Value != null)
            {
                tableName = metadata.Value.ToString();
            }
            else if (expression.Target.MetadataProperties.TryGetValue("Table", false, out metadata) && metadata.Value != null)
            {
                tableName = metadata.Value.ToString();
            }
            else
            {
                tableName = expression.Target.Name;
            }

            if (expression.Target.MetadataProperties.Contains("DefiningQuery"))
            {
                MetadataProperty definingQuery = expression.Target.MetadataProperties.GetValue("DefiningQuery", false);
                if (definingQuery.Value != null)
                {
                    return new ScanExpression("(" + definingQuery.Value + ")", expression.Target);
                }
            }

            ScanExpression scan;
            string overrideSchema = "http://schemas.microsoft.com/ado/2007/12/edm/EntityStoreSchemaGenerator:Schema";
            if (expression.Target.MetadataProperties.TryGetValue(overrideSchema, false, out metadata) && metadata.Value != null)
            {
                scan = new ScanExpression(QuoteIdentifier(metadata.Value.ToString()) + "." + QuoteIdentifier(tableName), expression.Target);
            }
            else if (expression.Target.MetadataProperties.TryGetValue("Schema", false, out metadata) && metadata.Value != null)
            {
                scan = new ScanExpression(QuoteIdentifier(metadata.Value.ToString()) + "." + QuoteIdentifier(tableName), expression.Target);
            }
            else
            {
                scan = new ScanExpression(QuoteIdentifier(expression.Target.EntityContainer.Name) + "." + QuoteIdentifier(tableName), expression.Target);
            }

            return scan;
        }

        public override VisitedExpression Visit(DbRelationshipNavigationExpression expression)
        {
            throw new NotImplementedException();
        }

        public override VisitedExpression Visit(DbRefExpression expression)
        {
            throw new NotImplementedException();
        }

        public override VisitedExpression Visit(DbQuantifierExpression expression)
        {
            // TODO: EXISTS or NOT EXISTS depending on expression.ExpressionKind
            // comes with it's built in test (subselect for EXISTS)
            // This kind of expression is never even created in the EF6 code base
            throw new NotImplementedException();
        }

        public override VisitedExpression Visit(DbProjectExpression expression)
        {
            return VisitInputWithBinding(expression, NextAlias()).Last.Exp;
        }

        public override VisitedExpression Visit(DbParameterReferenceExpression expression)
        {
            // use parameter in sql
            return new LiteralExpression("@" + expression.ParameterName);
        }

        public override VisitedExpression Visit(DbOrExpression expression)
        {
            return OperatorExpression.Build(Operator.Or, _useNewPrecedences, expression.Left.Accept(this), expression.Right.Accept(this));
        }

        public override VisitedExpression Visit(DbOfTypeExpression expression)
        {
            throw new NotImplementedException();
        }

        public override VisitedExpression Visit(DbNullExpression expression)
        {
            // select does something different here.  But insert, update, delete, and functions can just use
            // a NULL literal.
            return new LiteralExpression("NULL");
        }

        public override VisitedExpression Visit(DbNotExpression expression)
        {
            // argument can be a "NOT EXISTS" or similar operator that can be negated.
            // Convert the not if that's the case
            VisitedExpression argument = expression.Argument.Accept(this);
            return OperatorExpression.Negate(argument, _useNewPrecedences);
        }

        public override VisitedExpression Visit(DbNewInstanceExpression expression)
        {
            // Handled by VisitInputWithBinding
            throw new NotImplementedException();
        }

        public override VisitedExpression Visit(DbLimitExpression expression)
        {
            // Normally handled by VisitInputWithBinding

            // Otherwise, it is (probably) a child of a DbElementExpression,
            // in which case the child of this expression might be a DbProjectExpression,
            // then the correct columns will be projected since Limit is compatible with the result of a DbProjectExpression,
            // which will result in having a Projection on the node after visiting it.
            PendingProjectsNode node = VisitInputWithBinding(expression, NextAlias());
            if (node.Last.Exp.Projection == null)
            {
                // This DbLimitExpression is (probably) a child of DbElementExpression
                // and this expression's child is not a DbProjectExpression, but we should
                // find a DbProjectExpression if we look deeper in the command tree.
                // The child of this expression is (probably) a DbSortExpression or something else
                // that will (probably) be an ancestor to a DbProjectExpression.

                // Since this is (probably) a child of DbElementExpression, we want the first column,
                // so make sure it is propagated from the nearest explicit projection.

                CommaSeparatedExpression projection = node.Selects[0].Exp.Projection;
                for (int i = 1; i < node.Selects.Count; i++)
                {
                    ColumnExpression column = (ColumnExpression)projection.Arguments[0];

                    node.Selects[i].Exp.ColumnsToProject[new StringPair(node.Selects[i - 1].AsName, column.Name)] = column.Name;
                }
            }
            return node.Last.Exp;
        }

        public override VisitedExpression Visit(DbLikeExpression expression)
        {
            // LIKE keyword
            return OperatorExpression.Build(Operator.Like, _useNewPrecedences, expression.Argument.Accept(this), expression.Pattern.Accept(this));
        }

        public override VisitedExpression Visit(DbJoinExpression expression)
        {
            // Handled by VisitInputWithBinding
            throw new NotImplementedException();
        }

        public override VisitedExpression Visit(DbIsOfExpression expression)
        {
            throw new NotImplementedException();
        }

        public override VisitedExpression Visit(DbIsNullExpression expression)
        {
            return OperatorExpression.Build(Operator.IsNull, _useNewPrecedences, expression.Argument.Accept(this));
        }

        public override VisitedExpression Visit(DbIsEmptyExpression expression)
        {
            // NOT EXISTS
            return OperatorExpression.Negate(new ExistsExpression(expression.Argument.Accept(this)), _useNewPrecedences);
        }

        public override VisitedExpression Visit(DbIntersectExpression expression)
        {
            // INTERSECT keyword
            // Handled by VisitInputWithBinding
            throw new NotImplementedException();
        }

        public override VisitedExpression Visit(DbGroupByExpression expression)
        {
            // Normally handled by VisitInputWithBinding

            // Otherwise, it is (probably) a child of a DbElementExpression.
            // Group by always projects the correct columns.
            return VisitInputWithBinding(expression, NextAlias()).Last.Exp;
        }

        public override VisitedExpression Visit(DbRefKeyExpression expression)
        {
            throw new NotImplementedException();
        }

        public override VisitedExpression Visit(DbEntityRefExpression expression)
        {
            throw new NotImplementedException();
        }

        public override VisitedExpression Visit(DbFunctionExpression expression)
        {
            // a function call
            // may be built in, canonical, or user defined
            return VisitFunction(expression.Function, expression.Arguments, expression.ResultType);
        }

        public override VisitedExpression Visit(DbFilterExpression expression)
        {
            // Handled by VisitInputWithBinding
            throw new NotImplementedException();
        }

        public override VisitedExpression Visit(DbExceptExpression expression)
        {
            // EXCEPT keyword
            // Handled by VisitInputWithBinding
            throw new NotImplementedException();
        }

        public override VisitedExpression Visit(DbElementExpression expression)
        {
            // If child of DbNewInstanceExpression, this is handled in VisitInputWithBinding

            // a scalar expression (ie ExecuteScalar)
            // so it will likely be translated into a select
            //throw new NotImplementedException();
            LiteralExpression scalar = new LiteralExpression("(");
            scalar.Append(expression.Argument.Accept(this));
            scalar.Append(")");
            return scalar;
        }

        public override VisitedExpression Visit(DbDistinctExpression expression)
        {
            // Handled by VisitInputWithBinding
            throw new NotImplementedException();
        }

        public override VisitedExpression Visit(DbDerefExpression expression)
        {
            throw new NotImplementedException();
        }

        public override VisitedExpression Visit(DbCrossJoinExpression expression)
        {
            // join without ON
            // Handled by VisitInputWithBinding
            throw new NotImplementedException();
        }

        public override VisitedExpression Visit(DbConstantExpression expression)
        {
            if (_createParametersForConstants)
            {
                NpgsqlParameter parameter = new NpgsqlParameter();
                parameter.ParameterName = "p_" + _parameterCount++;
                parameter.NpgsqlDbType = NpgsqlProviderManifest.GetNpgsqlDbType(((PrimitiveType)expression.ResultType.EdmType).PrimitiveTypeKind);
                parameter.Value = expression.Value;
                _command.Parameters.Add(parameter);
                return new LiteralExpression("@" + parameter.ParameterName);
            }
            else
            {
                return new ConstantExpression(expression.Value, expression.ResultType);
            }
        }

        public override VisitedExpression Visit(DbComparisonExpression expression)
        {
            Operator comparisonOperator;
            switch (expression.ExpressionKind)
            {
                case DbExpressionKind.Equals: comparisonOperator = Operator.Equals; break;
                case DbExpressionKind.GreaterThan: comparisonOperator = Operator.GreaterThan; break;
                case DbExpressionKind.GreaterThanOrEquals: comparisonOperator = Operator.GreaterThanOrEquals; break;
                case DbExpressionKind.LessThan: comparisonOperator = Operator.LessThan; break;
                case DbExpressionKind.LessThanOrEquals: comparisonOperator = Operator.LessThanOrEquals; break;
                case DbExpressionKind.Like: comparisonOperator = Operator.Like; break;
                case DbExpressionKind.NotEquals: comparisonOperator = Operator.NotEquals; break;
                default: throw new NotSupportedException();
            }
            return OperatorExpression.Build(comparisonOperator, _useNewPrecedences, expression.Left.Accept(this), expression.Right.Accept(this));
        }

        public override VisitedExpression Visit(DbCastExpression expression)
        {
            return new CastExpression(expression.Argument.Accept(this), GetDbType(expression.ResultType.EdmType));
        }

        protected string GetDbType(EdmType edmType)
        {
            PrimitiveType primitiveType = edmType as PrimitiveType;
            if (primitiveType == null)
                throw new NotSupportedException();
            switch (primitiveType.PrimitiveTypeKind)
            {
                case PrimitiveTypeKind.Boolean:
                    return "bool";
                case PrimitiveTypeKind.SByte:
                case PrimitiveTypeKind.Byte:
                case PrimitiveTypeKind.Int16:
                    return "int2";
                case PrimitiveTypeKind.Int32:
                    return "int4";
                case PrimitiveTypeKind.Int64:
                    return "int8";
                case PrimitiveTypeKind.String:
                    return "text";
                case PrimitiveTypeKind.Decimal:
                    return "numeric";
                case PrimitiveTypeKind.Single:
                    return "float4";
                case PrimitiveTypeKind.Double:
                    return "float8";
                case PrimitiveTypeKind.DateTime:
                    return "timestamp";
                case PrimitiveTypeKind.DateTimeOffset:
                    return "timestamptz";
                case PrimitiveTypeKind.Time:
                    return "interval";
                case PrimitiveTypeKind.Binary:
                    return "bytea";
                case PrimitiveTypeKind.Guid:
                    return "uuid";
            }
            throw new NotSupportedException();
        }

        public override VisitedExpression Visit(DbCaseExpression expression)
        {
            LiteralExpression caseExpression = new LiteralExpression(" CASE ");
            for (int i = 0; i < expression.When.Count && i < expression.Then.Count; ++i)
            {
                caseExpression.Append(" WHEN (");
                caseExpression.Append(expression.When[i].Accept(this));
                caseExpression.Append(") THEN (");
                caseExpression.Append(expression.Then[i].Accept(this));
                caseExpression.Append(")");
            }
            if (expression.Else is DbNullExpression)
            {
                caseExpression.Append(" END ");
            }
            else
            {
                caseExpression.Append(" ELSE (");
                caseExpression.Append(expression.Else.Accept(this));
                caseExpression.Append(") END ");
            }
            return caseExpression;
        }

        public override VisitedExpression Visit(DbArithmeticExpression expression)
        {
            Operator arithmeticOperator;

            switch (expression.ExpressionKind)
            {
                case DbExpressionKind.Divide:
                    arithmeticOperator = Operator.Div;
                    break;
                case DbExpressionKind.Minus:
                    arithmeticOperator = Operator.Sub;
                    break;
                case DbExpressionKind.Modulo:
                    arithmeticOperator = Operator.Mod;
                    break;
                case DbExpressionKind.Multiply:
                    arithmeticOperator = Operator.Mul;
                    break;
                case DbExpressionKind.Plus:
                    arithmeticOperator = Operator.Add;
                    break;
                case DbExpressionKind.UnaryMinus:
                    arithmeticOperator = Operator.UnaryMinus;
                    break;
                default:
                    throw new NotSupportedException();
            }

            if (expression.ExpressionKind == DbExpressionKind.UnaryMinus)
            {
                System.Diagnostics.Debug.Assert(expression.Arguments.Count == 1);
                return OperatorExpression.Build(arithmeticOperator, _useNewPrecedences, expression.Arguments[0].Accept(this));
            }
            else
            {
                System.Diagnostics.Debug.Assert(expression.Arguments.Count == 2);
                return OperatorExpression.Build(arithmeticOperator, _useNewPrecedences, expression.Arguments[0].Accept(this), expression.Arguments[1].Accept(this));
            }
        }

        public override VisitedExpression Visit(DbApplyExpression expression)
        {
            // like a join, but used when the right hand side (the Apply part) is a function.
            // it lets you return the results of a function call given values from the
            // left hand side (the Input part).
            // sql standard is lateral join

            // Handled by VisitInputWithBinding
            throw new NotImplementedException();
        }

        public override VisitedExpression Visit(DbAndExpression expression)
        {
            return OperatorExpression.Build(Operator.And, _useNewPrecedences, expression.Left.Accept(this), expression.Right.Accept(this));
        }

        public override VisitedExpression Visit(DbExpression expression)
        {
            // only concrete types visited
            throw new NotSupportedException();
        }

        public abstract void BuildCommand(DbCommand command);

        internal static string QuoteIdentifier(string identifier)
        {
            return "\"" + identifier.Replace("\"", "\"\"") + "\"";
        }

        private VisitedExpression VisitFunction(DbFunctionAggregate functionAggregate)
        {
            if (functionAggregate.Function.NamespaceName == "Edm")
            {
                FunctionExpression aggregate;
                try
                {
                    aggregate = new FunctionExpression(AggregateFunctionNames[functionAggregate.Function.Name]);
                } catch (KeyNotFoundException)
                {
                    throw new NotSupportedException();
                }
                System.Diagnostics.Debug.Assert(functionAggregate.Arguments.Count == 1);
                VisitedExpression aggregateArg;
                if (functionAggregate.Distinct)
                {
                    aggregateArg = new LiteralExpression("DISTINCT ");
                    ((LiteralExpression)aggregateArg).Append(functionAggregate.Arguments[0].Accept(this));
                }
                else
                {
                    aggregateArg = functionAggregate.Arguments[0].Accept(this);
                }
                aggregate.AddArgument(aggregateArg);
                return new CastExpression(aggregate, GetDbType(functionAggregate.ResultType.EdmType));
            }
            throw new NotSupportedException();
        }

        private VisitedExpression VisitFunction(EdmFunction function, IList<DbExpression> args, TypeUsage resultType)
        {
            if (function.NamespaceName == "Edm")
            {
                VisitedExpression arg;
                switch (function.Name)
                {
                    // string functions
                    case "Concat":
                        System.Diagnostics.Debug.Assert(args.Count == 2);
                        return OperatorExpression.Build(Operator.Concat, _useNewPrecedences, args[0].Accept(this), args[1].Accept(this));
                    case "Contains":
                        System.Diagnostics.Debug.Assert(args.Count == 2);
                        FunctionExpression contains = new FunctionExpression("position");
                        arg = args[1].Accept(this);
                        arg.Append(" in ");
                        arg.Append(args[0].Accept(this));
                        contains.AddArgument(arg);
                        // if position returns zero, then contains is false
                        return OperatorExpression.Build(Operator.GreaterThan, _useNewPrecedences, contains, new LiteralExpression("0"));
                    // case "EndsWith": - depends on a reverse function to be able to implement with parameterized queries
                    case "IndexOf":
                        System.Diagnostics.Debug.Assert(args.Count == 2);
                        FunctionExpression indexOf = new FunctionExpression("position");
                        arg = args[0].Accept(this);
                        arg.Append(" in ");
                        arg.Append(args[1].Accept(this));
                        indexOf.AddArgument(arg);
                        return indexOf;
                    case "Left":
                        System.Diagnostics.Debug.Assert(args.Count == 2);
                        return Substring(args[0].Accept(this), new LiteralExpression(" 1 "), args[1].Accept(this));
                    case "Length":
                        FunctionExpression length = new FunctionExpression("char_length");
                        System.Diagnostics.Debug.Assert(args.Count == 1);
                        length.AddArgument(args[0].Accept(this));
                        return new CastExpression(length, GetDbType(resultType.EdmType));
                    case "LTrim":
                        return StringModifier("ltrim", args);
                    case "Replace":
                        FunctionExpression replace = new FunctionExpression("replace");
                        System.Diagnostics.Debug.Assert(args.Count == 3);
                        replace.AddArgument(args[0].Accept(this));
                        replace.AddArgument(args[1].Accept(this));
                        replace.AddArgument(args[2].Accept(this));
                        return replace;
                    // case "Reverse":
                    case "Right":
                        System.Diagnostics.Debug.Assert(args.Count == 2);
                        {
                            var arg0 = args[0].Accept(this);
                            var arg1 = args[1].Accept(this);
                            var start = new FunctionExpression("char_length");
                            start.AddArgument(arg0);
                            // add one before subtracting count since strings are 1 based in postgresql
                            return Substring(arg0, OperatorExpression.Build(Operator.Sub, _useNewPrecedences, OperatorExpression.Build(Operator.Add, _useNewPrecedences, start, new LiteralExpression("1")), arg1));
                        }
                    case "RTrim":
                        return StringModifier("rtrim", args);
                    case "Substring":
                        System.Diagnostics.Debug.Assert(args.Count == 3);
                        return Substring(args[0].Accept(this), args[1].Accept(this), args[2].Accept(this));
                    case "StartsWith":
                        System.Diagnostics.Debug.Assert(args.Count == 2);
                        FunctionExpression startsWith = new FunctionExpression("position");
                        arg = args[1].Accept(this);
                        arg.Append(" in ");
                        arg.Append(args[0].Accept(this));
                        startsWith.AddArgument(arg);
                        return OperatorExpression.Build(Operator.Equals, _useNewPrecedences, startsWith, new LiteralExpression("1"));
                    case "ToLower":
                        return StringModifier("lower", args);
                    case "ToUpper":
                        return StringModifier("upper", args);
                    case "Trim":
                        return StringModifier("btrim", args);

                    // date functions
                    // date functions
                    case "AddDays":
                    case "AddHours":
                    case "AddMicroseconds":
                    case "AddMilliseconds":
                    case "AddMinutes":
                    case "AddMonths":
                    case "AddNanoseconds":
                    case "AddSeconds":
                    case "AddYears":
                        return DateAdd(function.Name, args);
                    case "DiffDays":
                    case "DiffHours":
                    case "DiffMicroseconds":
                    case "DiffMilliseconds":
                    case "DiffMinutes":
                    case "DiffMonths":
                    case "DiffNanoseconds":
                    case "DiffSeconds":
                    case "DiffYears":
                        System.Diagnostics.Debug.Assert(args.Count == 2);
                        return DateDiff(function.Name, args[0].Accept(this), args[1].Accept(this));
                    case "Day":
                    case "Hour":
                    case "Minute":
                    case "Month":
                    case "Second":
                    case "Year":
                        return DatePart(function.Name, args);
                    case "Millisecond":
                        return DatePart("milliseconds", args);
                    case "GetTotalOffsetMinutes":
                        VisitedExpression timezone = DatePart("timezone", args);
                        return OperatorExpression.Build(Operator.Div, _useNewPrecedences, timezone, new LiteralExpression("60"));
                    case "CurrentDateTime":
                        return new LiteralExpression("LOCALTIMESTAMP");
                    case "CurrentUtcDateTime":
                        LiteralExpression utcNow = new LiteralExpression("CURRENT_TIMESTAMP");
                        utcNow.Append(" AT TIME ZONE 'UTC'");
                        return utcNow;
                    case "CurrentDateTimeOffset":
                        // TODO: this doesn't work yet because the reader
                        // doesn't return DateTimeOffset.
                        return new LiteralExpression("CURRENT_TIMESTAMP");

                    // bitwise operators
                    case "BitwiseAnd":
                        return BitwiseOperator(args, Operator.BitwiseAnd);
                    case "BitwiseOr":
                        return BitwiseOperator(args, Operator.BitwiseOr);
                    case "BitwiseXor":
                        return BitwiseOperator(args, Operator.BitwiseXor);
                    case "BitwiseNot":
                        System.Diagnostics.Debug.Assert(args.Count == 1);
                        return OperatorExpression.Build(Operator.BitwiseNot, _useNewPrecedences, args[0].Accept(this));

                    // math operators
                    case "Abs":
                    case "Ceiling":
                    case "Floor":
                        return UnaryMath(function.Name, args);
                    case "Round":
                        return (args.Count == 1) ? UnaryMath(function.Name, args) : BinaryMath(function.Name, args);
                    case "Power":
                        return BinaryMath(function.Name, args);
                    case "Truncate":
                        return BinaryMath("trunc", args);

                    case "NewGuid":
                        return new FunctionExpression("uuid_generate_v4");
                    case "TruncateTime":
                        return new TruncateTimeExpression("day", args[0].Accept(this));

                    default:
                        throw new NotSupportedException("NotSupported " + function.Name);
                }
            }

#if ENTITIES6
            var functionName = function.StoreFunctionNameAttribute ?? function.Name;
            if (function.NamespaceName == "Npgsql")
            {
                Operator binaryOperator;
                if (BinaryOperatorFunctionNames.TryGetValue(functionName, out binaryOperator))
                {
                    if (args.Count != 2)
                        throw new ArgumentException(string.Format("Invalid number of {0} arguments. Expected 2.", function.Name), "args");

                    return OperatorExpression.Build(
                        binaryOperator,
                        _useNewPrecedences,
                        args[0].Accept(this),
                        args[1].Accept(this));
                }

                if (functionName == "operator_tsquery_negate")
                {
                    if (args.Count != 1)
                        throw new ArgumentException("Invalid number of operator_tsquery_not arguments. Expected 1.", "args");

                    return OperatorExpression.Build(Operator.QueryNegate, _useNewPrecedences, args[0].Accept(this));
                }

                if (functionName == "ts_rank" || functionName == "ts_rank_cd")
                {
                    if (args.Count > 4)
                    {
                        var weightD = args[0] as DbConstantExpression;
                        var weightC = args[1] as DbConstantExpression;
                        var weightB = args[2] as DbConstantExpression;
                        var weightA = args[3] as DbConstantExpression;

                        if (weightD == null || weightC == null || weightB == null || weightA == null)
                            throw new NotSupportedException("All weight values must be constant expressions.");

                        var newValue = string.Format(
                            CultureInfo.InvariantCulture,
                            "{{ {0:r}, {1:r}, {2:r}, {3:r} }}",
                            weightD.Value,
                            weightC.Value,
                            weightB.Value,
                            weightA.Value);

                        args = new[] { DbExpression.FromString(newValue) }.Concat(args.Skip(4)).ToList();
                    }
                }
                else if (functionName == "setweight")
                {
                    if (args.Count != 2)
                        throw new ArgumentException("Invalid number of setweight arguments. Expected 2.", "args");

                    var weightLabelExpression = args[1] as DbConstantExpression;
                    if (weightLabelExpression == null)
                        throw new NotSupportedException("setweight label argument must be a constant expression.");

                    var weightLabel = (NpgsqlWeightLabel)weightLabelExpression.Value;
                    if (!Enum.IsDefined(typeof(NpgsqlWeightLabel), weightLabelExpression.Value))
                        throw new NotSupportedException("Unsupported weight label value: " + weightLabel);

                    args = new[] { args[0], DbExpression.FromString(weightLabel.ToString()) };
                }
                else if (functionName == "as_tsvector")
                {
                    if (args.Count != 1)
                        throw new ArgumentException("Invalid number of arguments. Expected 1.", "args");

                    return new CastExpression(args[0].Accept(this), "tsvector");
                }
                else if (functionName == "as_tsquery")
                {
                    if (args.Count != 1)
                        throw new ArgumentException("Invalid number of arguments. Expected 1.", "args");

                    return new CastExpression(args[0].Accept(this), "tsquery");
                }
            }

            var customFuncCall = new FunctionExpression(
                string.IsNullOrEmpty(function.Schema)
                    ? QuoteIdentifier(functionName)
                    : QuoteIdentifier(function.Schema) + "." + QuoteIdentifier(functionName)
            );

            foreach (var a in args)
                customFuncCall.AddArgument(a.Accept(this));
            return customFuncCall;
#else
            throw new NotSupportedException();
#endif
        }

        private VisitedExpression Substring(VisitedExpression source, VisitedExpression start, VisitedExpression count)
        {
            FunctionExpression substring = new FunctionExpression("substr");
            substring.AddArgument(source);
            substring.AddArgument(start);
            substring.AddArgument(count);
            return substring;
        }

        private VisitedExpression Substring(VisitedExpression source, VisitedExpression start)
        {
            FunctionExpression substring = new FunctionExpression("substr");
            substring.AddArgument(source);
            substring.AddArgument(start);
            return substring;
        }

        private VisitedExpression UnaryMath(string funcName, IList<DbExpression> args)
        {
            FunctionExpression mathFunction = new FunctionExpression(funcName);
            System.Diagnostics.Debug.Assert(args.Count == 1);
            mathFunction.AddArgument(args[0].Accept(this));
            return mathFunction;
        }

        private VisitedExpression BinaryMath(string funcName, IList<DbExpression> args)
        {
            FunctionExpression mathFunction = new FunctionExpression(funcName);
            System.Diagnostics.Debug.Assert(args.Count == 2);
            mathFunction.AddArgument(args[0].Accept(this));
            mathFunction.AddArgument(args[1].Accept(this));
            return mathFunction;
        }

        private VisitedExpression StringModifier(string modifier, IList<DbExpression> args)
        {
            FunctionExpression modifierFunction = new FunctionExpression(modifier);
            System.Diagnostics.Debug.Assert(args.Count == 1);
            modifierFunction.AddArgument(args[0].Accept(this));
            return modifierFunction;
        }

        private VisitedExpression DatePart(string partName, IList<DbExpression> args)
        {

            FunctionExpression extract_date = new FunctionExpression("cast(extract");
            System.Diagnostics.Debug.Assert(args.Count == 1);
            VisitedExpression arg = new LiteralExpression(partName + " FROM ");
            arg.Append(args[0].Accept(this));
            extract_date.AddArgument(arg);
            // need to convert to Int32 to match cononical function
            extract_date.Append(" as int4)");
            return extract_date;
        }

        /// <summary>
        /// PostgreSQL has no direct functions to implements DateTime canonical functions
        /// http://msdn.microsoft.com/en-us/library/bb738626.aspx
        /// http://msdn.microsoft.com/en-us/library/bb738626.aspx
        /// but we can use workaround:
        /// expression + number * INTERVAL '1 number_type'
        /// where number_type is the number type (days, years and etc)
        /// </summary>
        /// <param name="functionName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private VisitedExpression DateAdd(string functionName, IList<DbExpression> args)
        {
            bool nano = false;
            string part = functionName.Substring(3);

            if (part == "Nanoseconds")
            {
                nano = true;
                part = "Microseconds";
            }

            System.Diagnostics.Debug.Assert(args.Count == 2);
            VisitedExpression time = args[0].Accept(this);
            VisitedExpression mulLeft = args[1].Accept(this);
            if (nano)
                mulLeft = OperatorExpression.Build(Operator.Div, _useNewPrecedences, mulLeft, new LiteralExpression("1000"));
            LiteralExpression mulRight = new LiteralExpression(String.Format("INTERVAL '1 {0}'", part));
            return OperatorExpression.Build(Operator.Add, _useNewPrecedences, time, OperatorExpression.Build(Operator.Mul, _useNewPrecedences, mulLeft, mulRight));
        }

        private VisitedExpression DateDiff(string functionName, VisitedExpression start, VisitedExpression end)
        {
            switch (functionName)
            {
                case "DiffDays":
                    start = new FunctionExpression("date_trunc").AddArgument("'day'").AddArgument(start);
                    end = new FunctionExpression("date_trunc").AddArgument("'day'").AddArgument(end);
                    return new FunctionExpression("date_part").AddArgument("'day'").AddArgument(
                        OperatorExpression.Build(Operator.Sub, _useNewPrecedences, end, start)
                    ).Append("::int4");
                case "DiffHours":
                    {
                        start = new FunctionExpression("date_trunc").AddArgument("'hour'").AddArgument(start);
                        end = new FunctionExpression("date_trunc").AddArgument("'hour'").AddArgument(end);
                        LiteralExpression epoch = new LiteralExpression("epoch from ");
                        OperatorExpression diff = OperatorExpression.Build(Operator.Sub, _useNewPrecedences, end, start);
                        epoch.Append(diff);
                        return OperatorExpression.Build(Operator.Div, _useNewPrecedences, new FunctionExpression("extract").AddArgument(epoch).Append("::int4"), new LiteralExpression("3600"));
                    }
                case "DiffMicroseconds":
                    {
                        start = new FunctionExpression("date_trunc").AddArgument("'microseconds'").AddArgument(start);
                        end = new FunctionExpression("date_trunc").AddArgument("'microseconds'").AddArgument(end);
                        LiteralExpression epoch = new LiteralExpression("epoch from ");
                        OperatorExpression diff = OperatorExpression.Build(Operator.Sub, _useNewPrecedences, end, start);
                        epoch.Append(diff);
                        return new CastExpression(OperatorExpression.Build(Operator.Mul, _useNewPrecedences, new FunctionExpression("extract").AddArgument(epoch), new LiteralExpression("1000000")), "int4");
                    }
                case "DiffMilliseconds":
                    {
                        start = new FunctionExpression("date_trunc").AddArgument("'milliseconds'").AddArgument(start);
                        end = new FunctionExpression("date_trunc").AddArgument("'milliseconds'").AddArgument(end);
                        LiteralExpression epoch = new LiteralExpression("epoch from ");
                        OperatorExpression diff = OperatorExpression.Build(Operator.Sub, _useNewPrecedences, end, start);
                        epoch.Append(diff);
                        return new CastExpression(OperatorExpression.Build(Operator.Mul, _useNewPrecedences, new FunctionExpression("extract").AddArgument(epoch), new LiteralExpression("1000")), "int4");
                    }
                case "DiffMinutes":
                    {
                        start = new FunctionExpression("date_trunc").AddArgument("'minute'").AddArgument(start);
                        end = new FunctionExpression("date_trunc").AddArgument("'minute'").AddArgument(end);
                        LiteralExpression epoch = new LiteralExpression("epoch from ");
                        OperatorExpression diff = OperatorExpression.Build(Operator.Sub, _useNewPrecedences, end, start);
                        epoch.Append(diff);
                        return OperatorExpression.Build(Operator.Div, _useNewPrecedences, new FunctionExpression("extract").AddArgument(epoch).Append("::int4"), new LiteralExpression("60"));
                    }
                case "DiffMonths":
                    {
                        start = new FunctionExpression("date_trunc").AddArgument("'month'").AddArgument(start);
                        end = new FunctionExpression("date_trunc").AddArgument("'month'").AddArgument(end);
                        VisitedExpression age = new FunctionExpression("age").AddArgument(end).AddArgument(start);

                        // A month is 30 days and a year is 365.25 days after conversion from interval to seconds.
                        // After rounding and casting, the result will contain the correct number of months as an int4.
                        FunctionExpression seconds = new FunctionExpression("extract").AddArgument(new LiteralExpression("epoch from ").Append(age));
                        VisitedExpression months = OperatorExpression.Build(Operator.Div, _useNewPrecedences, seconds, new LiteralExpression("2629800.0"));
                        return new FunctionExpression("round").AddArgument(months).Append("::int4");
                    }
                case "DiffNanoseconds":
                    {
                        // PostgreSQL only supports microseconds precision, so the value will be a multiple of 1000
                        // This date_trunc will make sure start and end are of type timestamp, e.g. if the arguments is of type date
                        start = new FunctionExpression("date_trunc").AddArgument("'microseconds'").AddArgument(start);
                        end = new FunctionExpression("date_trunc").AddArgument("'microseconds'").AddArgument(end);
                        LiteralExpression epoch = new LiteralExpression("epoch from ");
                        OperatorExpression diff = OperatorExpression.Build(Operator.Sub, _useNewPrecedences, end, start);
                        epoch.Append(diff);
                        return new CastExpression(OperatorExpression.Build(Operator.Mul, _useNewPrecedences, new FunctionExpression("extract").AddArgument(epoch), new LiteralExpression("1000000000")), "int4");
                    }
                case "DiffSeconds":
                    {
                        start = new FunctionExpression("date_trunc").AddArgument("'second'").AddArgument(start);
                        end = new FunctionExpression("date_trunc").AddArgument("'second'").AddArgument(end);
                        LiteralExpression epoch = new LiteralExpression("epoch from ");
                        OperatorExpression diff = OperatorExpression.Build(Operator.Sub, _useNewPrecedences, end, start);
                        epoch.Append(diff);
                        return new FunctionExpression("extract").AddArgument(epoch).Append("::int4");
                    }
                case "DiffYears":
                    {
                        start = new FunctionExpression("date_trunc").AddArgument("'year'").AddArgument(start);
                        end = new FunctionExpression("date_trunc").AddArgument("'year'").AddArgument(end);
                        VisitedExpression age = new FunctionExpression("age").AddArgument(end).AddArgument(start);
                        return new FunctionExpression("date_part").AddArgument("'year'").AddArgument(age).Append("::int4");
                    }
                default: throw new NotSupportedException("Internal error: unknown function name " + functionName);
            }
        }

        private VisitedExpression BitwiseOperator(IList<DbExpression> args, Operator oper)
        {
            System.Diagnostics.Debug.Assert(args.Count == 2);
            return OperatorExpression.Build(oper, _useNewPrecedences, args[0].Accept(this), args[1].Accept(this));
        }

#if ENTITIES6
        public override VisitedExpression Visit(DbInExpression expression)
        {
            VisitedExpression item = expression.Item.Accept(this);

            ConstantExpression[] elements = new ConstantExpression[expression.List.Count];
            for (int i = 0; i < expression.List.Count; i++)
            {
                elements[i] = (ConstantExpression)expression.List[i].Accept(this);
            }

            return OperatorExpression.Build(Operator.In, _useNewPrecedences, item, new ConstantListExpression(elements));
        }

        public override VisitedExpression Visit(DbPropertyExpression expression)
        {
            // This is overridden in the other visitors
            throw new NotImplementedException("New in Entity Framework 6");
        }
#endif
    }
}
