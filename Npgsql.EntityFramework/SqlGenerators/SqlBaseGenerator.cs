using System;
using System.Collections.Generic;
using System.Data.Common;
#if ENTITIES6
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Metadata.Edm;
#else
using System.Data.Common.CommandTrees;
using System.Data.Metadata.Edm;
#endif
using System.Linq;

namespace Npgsql.SqlGenerators
{
    internal abstract class SqlBaseGenerator : DbExpressionVisitor<VisitedExpression>
    {

        protected Dictionary<string, PendingProjectsNode> _refToNode = new Dictionary<string, PendingProjectsNode>();
        protected HashSet<InputExpression> _currentExpressions = new HashSet<InputExpression>();
        protected uint _aliasCounter = 0;

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
                                input.ColumnsToProject.Add(new Tuple<string, string>(prevName, col.Name), col.Name);
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
                        PendingProjectsNode l = VisitInputWithBinding(exp.Left, bindingName + "_1");
                        PendingProjectsNode r = VisitInputWithBinding(exp.Right, bindingName + "_2");
                        InputExpression input = new InputExpression(new CombinedProjectionExpression(l.Last.Exp, expression.ExpressionKind, r.Last.Exp), bindingName);
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
            return new BooleanExpression("OR", expression.Left.Accept(this), expression.Right.Accept(this));
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
            NegatableExpression negatable = argument as NegatableExpression;
            if (negatable != null)
            {
                negatable.Negate();
                return negatable;
            }
            else
            {
                return new NegateExpression(argument);
            }
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
            // in which case the child of this expression is (probably) a DbProjectExpression,
            // so the correct columns will be projected since Limit is compatible with the result of a DbProjectExpression
            return VisitInputWithBinding(expression, NextAlias()).Last.Exp;
        }

        public override VisitedExpression Visit(DbLikeExpression expression)
        {
            // LIKE keyword
            return new NegatableBooleanExpression(DbExpressionKind.Like, expression.Argument.Accept(this), expression.Pattern.Accept(this));
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
            return new IsNullExpression(expression.Argument.Accept(this));
        }

        public override VisitedExpression Visit(DbIsEmptyExpression expression)
        {
            // NOT EXISTS
            return new ExistsExpression(expression.Argument.Accept(this)).Negate();
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
            // literals to be inserted into the sql
            // may require some formatting depending on the type
            //throw new NotImplementedException();
            // TODO: this is just for testing
            return new ConstantExpression(expression.Value, expression.ResultType);
        }

        public override VisitedExpression Visit(DbComparisonExpression expression)
        {
            DbExpressionKind comparisonOperator;
            switch (expression.ExpressionKind)
            {
                case DbExpressionKind.Equals:
                case DbExpressionKind.GreaterThan:
                case DbExpressionKind.GreaterThanOrEquals:
                case DbExpressionKind.LessThan:
                case DbExpressionKind.LessThanOrEquals:
                case DbExpressionKind.Like:
                case DbExpressionKind.NotEquals:
                    comparisonOperator = expression.ExpressionKind;
                    break;
                default:
                    throw new NotSupportedException();
            }
            return new NegatableBooleanExpression(comparisonOperator, expression.Left.Accept(this), expression.Right.Accept(this));
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
                    return "varchar";
                case PrimitiveTypeKind.Decimal:
                    return "numeric";
                case PrimitiveTypeKind.Single:
                    return "float4";
                case PrimitiveTypeKind.Double:
                    return "float8";
                case PrimitiveTypeKind.DateTime:
                    return "timestamp";
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
            LiteralExpression arithmeticOperator;

            switch (expression.ExpressionKind)
            {
                case DbExpressionKind.Divide:
                    arithmeticOperator = new LiteralExpression("/");
                    break;
                case DbExpressionKind.Minus:
                    arithmeticOperator = new LiteralExpression("-");
                    break;
                case DbExpressionKind.Modulo:
                    arithmeticOperator = new LiteralExpression("%");
                    break;
                case DbExpressionKind.Multiply:
                    arithmeticOperator = new LiteralExpression("*");
                    break;
                case DbExpressionKind.Plus:
                    arithmeticOperator = new LiteralExpression("+");
                    break;
                case DbExpressionKind.UnaryMinus:
                    arithmeticOperator = new LiteralExpression("-");
                    break;
                default:
                    throw new NotSupportedException();
            }

            if (expression.ExpressionKind == DbExpressionKind.UnaryMinus)
            {
                System.Diagnostics.Debug.Assert(expression.Arguments.Count == 1);
                arithmeticOperator.Append("(");
                arithmeticOperator.Append(expression.Arguments[0].Accept(this));
                arithmeticOperator.Append(")");
                return arithmeticOperator;
            }
            else
            {
                LiteralExpression math = new LiteralExpression("");
                bool first = true;
                foreach (DbExpression arg in expression.Arguments)
                {
                    if (!first)
                        math.Append(arithmeticOperator);
                    math.Append("(");
                    math.Append(arg.Accept(this));
                    math.Append(")");
                    first = false;
                }
                return math;
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
            return new BooleanExpression("AND", expression.Left.Accept(this), expression.Right.Accept(this));
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
                        arg = args[0].Accept(this);
                        arg.Append(" || ");
                        arg.Append(args[1].Accept(this));
                        return arg;
                    case "Contains":
                        System.Diagnostics.Debug.Assert(args.Count == 2);
                        FunctionExpression contains = new FunctionExpression("position");
                        arg = args[1].Accept(this);
                        arg.Append(" in ");
                        arg.Append(args[0].Accept(this));
                        contains.AddArgument(arg);
                        // if position returns zero, then contains is false
                        return new NegatableBooleanExpression(DbExpressionKind.GreaterThan, contains, new LiteralExpression("0"));
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
                            start.Append("+1-");
                            start.Append(arg1);
                            return Substring(arg0, start);
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
                        return new NegatableBooleanExpression(DbExpressionKind.Equals, startsWith, new LiteralExpression("1"));
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
                    case "DiffDays":
                    case "DiffHours":
                    case "DiffMicroseconds":
                    case "DiffMilliseconds":
                    case "DiffMinutes":
                    case "DiffMonths":
                    case "DiffNanoseconds":
                    case "DiffSeconds":
                    case "DiffYears":
                        return DateAdd(function.Name, args);
                    //    return
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
                        timezone.Append("/60");
                        return timezone;
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
                        return BitwiseOperator(args, " & ");
                    case "BitwiseOr":
                        return BitwiseOperator(args, " | ");
                    case "BitwiseXor":
                        return BitwiseOperator(args, " # ");
                    case "BitwiseNot":
                        System.Diagnostics.Debug.Assert(args.Count == 1);
                        LiteralExpression not = new LiteralExpression("~ ");
                        not.Append(args[0].Accept(this));
                        return not;

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

                    default:
                        throw new NotSupportedException("NotSupported " + function.Name);
                }
            }
            throw new NotSupportedException();
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
            string operation = "";
            string part = "";
            bool nano = false;
            if (functionName.Contains("Add"))
            {
                operation = "+";
                part = functionName.Substring(3);
            }
            else if (functionName.Contains("Diff"))
            {
                operation = "-";
                part = functionName.Substring(4);
            }
            else throw new NotSupportedException();

            if (part == "Nanoseconds")
            {
                nano = true;
                part = "Microseconds";
            }

            System.Diagnostics.Debug.Assert(args.Count == 2);
            VisitedExpression dateAddDiff = new LiteralExpression("");
            dateAddDiff.Append(args[0].Accept(this));
            dateAddDiff.Append(operation);
            dateAddDiff.Append(args[1].Accept(this));
            dateAddDiff.Append(nano
                                   ? String.Format("/ 1000 * INTERVAL '1 {0}'", part)
                                   : String.Format(" * INTERVAL '1 {0}'", part));

            return dateAddDiff;
        }

        private VisitedExpression BitwiseOperator(IList<DbExpression> args, string oper)
        {
            System.Diagnostics.Debug.Assert(args.Count == 2);
            VisitedExpression arg = args[0].Accept(this);
            arg.Append(oper);
            arg.Append(args[1].Accept(this));
            return arg;
        }

#if ENTITIES6
        public override VisitedExpression Visit(DbInExpression expression)
        {
            return new InExpression(expression.Item.Accept(this), expression.List.Select(e => (ConstantExpression)e.Accept(this)).ToList());
        }

        public override VisitedExpression Visit(DbPropertyExpression expression)
        {
            // This is overridden in the other visitors
            throw new NotImplementedException("New in Entity Framework 6");
        }
#endif
    }
}
