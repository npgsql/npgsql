#if ENTITIES
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Common.CommandTrees;
using System.Data.Metadata.Edm;

namespace Npgsql.SqlGenerators
{
    internal abstract class SqlBaseGenerator : DbExpressionVisitor<VisitedExpression>
	{
        protected Dictionary<string, string> _variableSubstitution = new Dictionary<string, string>();
        protected Stack<string> _projectVarName = new Stack<string>();
        protected Stack<string> _filterVarName = new Stack<string>();

        protected SqlBaseGenerator()
        {
        }

        private void SubstituteFilterVar(string value)
        {
            if (_filterVarName.Count != 0)
                _variableSubstitution[_filterVarName.Peek()] = value;
        }

        public override VisitedExpression Visit(DbVariableReferenceExpression expression)
        {
            return new VariableReferenceExpression(expression.VariableName, _variableSubstitution);
        }

        public override VisitedExpression Visit(DbUnionAllExpression expression)
        {
            // UNION ALL keyword
            return new CombinedProjectionExpression(expression.Left.Accept(this),
                "UNION ALL", expression.Right.Accept(this));
        }

        public override VisitedExpression Visit(DbTreatExpression expression)
        {
            throw new NotImplementedException();
        }

        public override VisitedExpression Visit(DbSkipExpression expression)
        {
            // not quite sure, but may be a paging like function
            // almost the opposite of limit, possibly actually the skip those first.
            InputExpression offset = CheckedConvertFrom(expression.Input.Expression.Accept(this), expression.Input.VariableName);
            offset.Append(" ORDER BY ");
            bool first = true;
            foreach (var order in expression.SortOrder)
            {
                if (!first)
                    offset.Append(",");
                offset.Append(order.Expression.Accept(this));
                if (order.Ascending)
                    offset.Append(" ASC ");
                else
                    offset.Append(" DESC ");
                first = false;
            }
            offset.Append(" OFFSET ");
            offset.Append(expression.Count.Accept(this));
            return offset;
        }

        public override VisitedExpression Visit(DbSortExpression expression)
        {
            // order by
            _filterVarName.Push(expression.Input.VariableName);
            VisitedExpression inputExpression = expression.Input.Expression.Accept(this);
            InputExpression from = inputExpression as InputExpression;
            if (from == null)
            {
                from = new FromExpression(inputExpression, expression.Input.VariableName);
                _variableSubstitution[_projectVarName.Peek()] = expression.Input.VariableName;
                SubstituteFilterVar(expression.Input.VariableName);
            }
            else
            {
                if (from is FromExpression)
                {
                    SubstituteFilterVar(((FromExpression)from).Name);
                }
            }
            _filterVarName.Pop();
            from.OrderBy = new OrderByExpression();
            foreach (var order in expression.SortOrder)
            {
                from.OrderBy.AppendSort(order.Expression.Accept(this), order.Ascending);
            }
            return from;
        }

        public override VisitedExpression Visit(DbScanExpression expression)
        {
            // name of a table identifier?
            // may come from a join or a select
            //return new LiteralExpression(expression.Target.MetadataProperties.TryGetValue(
            // replace with better solution
            if (_projectVarName.Count != 0) // this can happen in dml
                _variableSubstitution[_projectVarName.Peek()] = expression.Target.Name;
            SubstituteFilterVar(expression.Target.Name);
            if (expression.Target.MetadataProperties.Contains("DefiningQuery"))
            {
                MetadataProperty definingQuery = expression.Target.MetadataProperties.GetValue("DefiningQuery", false);
                if (definingQuery.Value != null)
                {
                    return new LiteralExpression("(" + definingQuery.Value + ")");
                }
            }

            MetadataProperty metadata;
            LiteralExpression scan;
            if (expression.Target.MetadataProperties.TryGetValue("Schema", false, out metadata) && metadata.Value != null)
            {
                scan = new LiteralExpression(QuoteIdentifier(metadata.Value.ToString()));
            }
            else
            {
                scan = new LiteralExpression(QuoteIdentifier(expression.Target.EntityContainer.Name));
            }
            scan.Append(".");
            scan.Append(QuoteIdentifier(expression.Target.Name));

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
            throw new NotImplementedException();
        }

        public override VisitedExpression Visit(DbProjectExpression expression)
        {
            ProjectionExpression project = expression.Projection.Accept(this) as ProjectionExpression;
            // TODO: test if this should always be true
            if (project == null)
                throw new NotSupportedException();
            _projectVarName.Push(expression.Input.VariableName);
            project.From = CheckedConvertFrom(expression.Input.Expression.Accept(this), expression.Input.VariableName);
            _projectVarName.Pop();

            return project;
        }

        internal InputExpression CheckedConvertFrom(VisitedExpression fromExpression, string variableName)
        {
            InputExpression result = fromExpression as InputExpression;
            if (result == null)
            {
                result = new FromExpression(fromExpression, variableName);
                if (string.IsNullOrEmpty(variableName)) variableName = ((FromExpression)result).Name;
                _variableSubstitution[_projectVarName.Peek()] = variableName;
                SubstituteFilterVar(variableName);
            }
            return result;
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
            return new LiteralExpression(" NULL ");
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
            RowType rowType = expression.ResultType.EdmType as RowType;

            if (rowType != null)
            {
                // should be the child of a project
                // which means it's a select
                ProjectionExpression visitedExpression = new ProjectionExpression();
                for (int i = 0; i < rowType.Properties.Count && i < expression.Arguments.Count; ++i)
                {
                    visitedExpression.AppendColumn(new ColumnExpression(expression.Arguments[i].Accept(this), rowType.Properties[i].Name));
                }

                return visitedExpression;
            }
            else if (expression.ResultType.EdmType is CollectionType)
            {
                ProjectionExpression visitedExpression = new ProjectionExpression();
                foreach (var arg in expression.Arguments)
                {
                    visitedExpression.AppendColumn(arg.Accept(this));
                }
                return visitedExpression;
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public override VisitedExpression Visit(DbLimitExpression expression)
        {
            // Need more complex operation where ties are needed
            // TODO: implement WithTies
            if (expression.WithTies)
                throw new NotSupportedException();
            // limit expressions should be structured like where clauses
            // see Visit(DbFilterExpression)
            VisitedExpression limit = expression.Argument.Accept(this);
            InputExpression input;
            if (!(limit is ProjectionExpression))
            {
                input = CheckedConvertFrom(limit, null);
                // return this value
                limit = input;
            }
            else
            {
                input = ((ProjectionExpression)limit).From;
            }
            input.Limit = new LimitExpression(expression.Limit.Accept(this));
            return limit;
        }

        public override VisitedExpression Visit(DbLikeExpression expression)
        {
            // LIKE keyword
            // also uses ESCAPE
            // ESCAPE may be way of identifying wild cards
            // TODO: enhance this.  Only supporting simple case for now
            return new BooleanExpression("LIKE", expression.Argument.Accept(this), expression.Pattern.Accept(this));
        }

        public override VisitedExpression Visit(DbJoinExpression expression)
        {
            // table join
            //throw new NotSupportedException();
            // the following code works ok, but the rest of the code doesn't work well in a join
            // need to take _projectVarName and append .left then do the same for right
            // use this to do combo variable substitution
            return new JoinExpression(VisitJoinPart(expression.Left),
                expression.ExpressionKind,
                VisitJoinPart(expression.Right),
                expression.JoinCondition.Accept(this));
        }

        private VisitedExpression VisitJoinPart(DbExpressionBinding joinPart)
        {
            _projectVarName.Push(joinPart.VariableName);
            string variableName = null;
            VisitedExpression joinPartExpression = null;
            if (joinPart.Expression is DbFilterExpression)
            {
                joinPartExpression = VisitFilterExpression((DbFilterExpression)joinPart.Expression, true);
            }
            else
            {
                joinPartExpression = joinPart.Expression.Accept(this);
            }
            if (joinPartExpression is FromExpression)
            {
                variableName = ((FromExpression)joinPartExpression).Name;
            }
            else if (!(joinPartExpression is JoinExpression)) // don't alias join expressions at all
            {
                joinPartExpression = new FromExpression(joinPartExpression, joinPart.VariableName);
                variableName = joinPart.VariableName;
            }
            _projectVarName.Pop();
            if (variableName != null)
            {
                _variableSubstitution[_projectVarName.Peek()] = variableName;
                string[] dottedNames = _projectVarName.ToArray();
                // reverse because the stack has them last in first out
                Array.Reverse(dottedNames);
                SubstituteAllNames(dottedNames, joinPart.VariableName, variableName);
                if (_filterVarName.Count != 0)
                {
                    dottedNames = _filterVarName.ToArray();
                    // reverse because the stack has them last in first out
                    Array.Reverse(dottedNames);
                    SubstituteAllNames(dottedNames, joinPart.VariableName, variableName);
                }
                _variableSubstitution[joinPart.VariableName] = variableName;
            }
            return joinPartExpression;
        }

        private void SubstituteAllNames(string[] dottedNames, string joinPartVariableName, string variableName)
        {
            int nameCount = dottedNames.Length;
            for (int i = 0; i < dottedNames.Length; ++i)
            {
                _variableSubstitution[string.Join(".", dottedNames, i, nameCount - i) + "." + joinPartVariableName] = variableName;
            }
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
            return new CombinedProjectionExpression(expression.Left.Accept(this),
                "INTERSECT", expression.Right.Accept(this));
        }

        public override VisitedExpression Visit(DbGroupByExpression expression)
        {
            // complicated
            // GROUP BY expression
            // first implementation this is a COUNT(column) query ???
            //EnterNewVariableScope();
            ProjectionExpression projectExpression = new ProjectionExpression();
            GroupByExpression groupByExpression = new GroupByExpression();
            RowType rowType = ((CollectionType)(expression.ResultType.EdmType)).TypeUsage.EdmType as RowType;
            int columnIndex = 0;
            foreach (var key in expression.Keys)
            {
                VisitedExpression keyColumnExpression = key.Accept(this);
                projectExpression.AppendColumn(new ColumnExpression(keyColumnExpression, rowType.Properties[columnIndex].Name));
                groupByExpression.AppendGroupingKey(keyColumnExpression);
                ++columnIndex;
            }
            foreach (var ag in expression.Aggregates)
            {
                DbFunctionAggregate function = ag as DbFunctionAggregate;
                if (function == null)
                    throw new NotSupportedException();
                VisitedExpression functionExpression = VisitFunction(function);
                projectExpression.AppendColumn(new ColumnExpression(functionExpression, rowType.Properties[columnIndex].Name));
                ++columnIndex;
            }
            _projectVarName.Push(expression.Input.GroupVariableName);
            _filterVarName.Push(expression.Input.VariableName);
            projectExpression.From = CheckedConvertFrom(expression.Input.Expression.Accept(this), expression.Input.GroupVariableName);
            projectExpression.From.GroupBy = groupByExpression;
            if (_variableSubstitution.ContainsKey(_projectVarName.Peek()))
            {
                _variableSubstitution[expression.Input.VariableName] = _variableSubstitution[_projectVarName.Peek()];
            }
            if (_variableSubstitution.ContainsKey(_filterVarName.Peek()))
            {
                _variableSubstitution[expression.Input.VariableName] = _variableSubstitution[_filterVarName.Peek()];
            }
            _projectVarName.Pop();
            _filterVarName.Pop();
            //LeaveVariableScope();
            //_variableSubstitution[_projectVarName.Peek()] = expression.Input.VariableName;
            //return new FromExpression(projectExpression, expression.Input.VariableName);
            return projectExpression;
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
            return VisitFilterExpression(expression, false);
        }

        private VisitedExpression VisitFilterExpression(DbFilterExpression expression, bool partOfJoin)
        {
            // complicated
            // similar logic used for other expressions (such as group by)
            // TODO: this is too simple.  Replace this
            // need to move the from keyword out so that it can be used in the project
            // when there is no where clause
            _filterVarName.Push(expression.Input.VariableName);
            InputExpression inputExpression = CheckedConvertFrom(expression.Input.Expression.Accept(this), expression.Input.VariableName);
            if (!(inputExpression is JoinExpression))
            {
                //from = new FromExpression(inputExpression, expression.Input.VariableName);
                FromExpression from = (FromExpression)inputExpression;
                if (from.Where != null)
                {
                    _variableSubstitution[expression.Input.VariableName] = from.Name;
                    from.Where.And(expression.Predicate.Accept(this));
                }
                else
                {
                    _variableSubstitution[_projectVarName.Peek()] = expression.Input.VariableName;
                    if (_variableSubstitution.ContainsKey(_filterVarName.Peek()))
                        _variableSubstitution[_filterVarName.Peek()] = expression.Input.VariableName;
                    from.Where = new WhereExpression(expression.Predicate.Accept(this));
                }
            }
            else
            {
                // TODO: this isn't quite right
                // need to make this work for general case of where as part of a join
                JoinExpression join = (JoinExpression)inputExpression;

                if (partOfJoin)
                {
                    System.Diagnostics.Debug.Assert(join.Condition != null);
                    join.Condition = new BooleanExpression("AND", join.Condition, expression.Predicate.Accept(this));
                }
                else
                {
                    VisitedExpression predicate = expression.Predicate.Accept(this);
                    if (join.Where != null)
                        join.Where.And(predicate);
                    else
                        join.Where = new WhereExpression(predicate);
                }
            }
            _filterVarName.Pop();
            return inputExpression;
        }

        public override VisitedExpression Visit(DbExceptExpression expression)
        {
            // Except keyword
            return new CombinedProjectionExpression(expression.Left.Accept(this),
                "EXCEPT", expression.Right.Accept(this));
        }

        public override VisitedExpression Visit(DbElementExpression expression)
        {
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
            // the distinct clause for a select
            VisitedExpression distinctArg = expression.Argument.Accept(this);
            ProjectionExpression projection = distinctArg as ProjectionExpression;
            if (projection == null)
                throw new NotSupportedException();
            projection.Distinct = true;
            return new FromExpression(projection, _projectVarName.Peek());
        }

        public override VisitedExpression Visit(DbDerefExpression expression)
        {
            throw new NotImplementedException();
        }

        public override VisitedExpression Visit(DbCrossJoinExpression expression)
        {
            // join without ON
            return new JoinExpression(VisitJoinPart(expression.Inputs[0]),
                expression.ExpressionKind,
                VisitJoinPart(expression.Inputs[1]),
                null);
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
            string comparisonOperator;
            switch (expression.ExpressionKind)
            {
                case DbExpressionKind.Equals:
                    comparisonOperator = "=";
                    break;
                case DbExpressionKind.GreaterThan:
                    comparisonOperator = ">";
                    break;
                case DbExpressionKind.GreaterThanOrEquals:
                    comparisonOperator = ">=";
                    break;
                case DbExpressionKind.LessThan:
                    comparisonOperator = "<";
                    break;
                case DbExpressionKind.LessThanOrEquals:
                    comparisonOperator = "<=";
                    break;
                case DbExpressionKind.Like:
                    comparisonOperator = " LIKE ";
                    break;
                case DbExpressionKind.NotEquals:
                    comparisonOperator = "!=";
                    break;
                default:
                    throw new NotSupportedException();
            }
            return new BooleanExpression(comparisonOperator, expression.Left.Accept(this), expression.Right.Accept(this));
        }

        public override VisitedExpression Visit(DbCastExpression expression)
        {
            return new CastExpression(expression.Argument.Accept(this), GetDbType(expression.ResultType.EdmType));
        }

        private string GetDbType(EdmType edmType)
        {
            PrimitiveType primitiveType = edmType as PrimitiveType;
            if (primitiveType == null)
                throw new NotSupportedException();
            switch (primitiveType.PrimitiveTypeKind)
            {
                case PrimitiveTypeKind.Boolean:
                    return "bool";
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
            // like a join, but used when one of the arguments is a function.
            // it lets you return the results of a function call given values from the
            // other table.
            // sql standard seems to be lateral join
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
                switch (functionAggregate.Function.Name)
                {
                    case "Avg":
                    case "Count":
                    case "Min":
                    case "Max":
                    case "StdDev":
                    case "Sum":
                        aggregate = new FunctionExpression(functionAggregate.Function.Name);
                        break;
                    case "BigCount":
                        aggregate = new FunctionExpression("count");
                        break;
                    default:
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
                // TODO: support more functions
                switch (function.Name)
                {
                        // string functions
                    case "Left":
                        FunctionExpression left = new FunctionExpression("substring");
                        System.Diagnostics.Debug.Assert(args.Count == 2);
                        arg = args[0].Accept(this);
                        arg.Append(" FROM 1 FOR ");
                        arg.Append(args[1].Accept(this));
                        left.AddArgument(arg);
                        return left;
                    case "Length":
                        FunctionExpression length = new FunctionExpression("char_length");
                        System.Diagnostics.Debug.Assert(args.Count == 1);
                        length.AddArgument(args[0].Accept(this));
                        return new CastExpression(length, GetDbType(resultType.EdmType));
                    case "Concat":
                        System.Diagnostics.Debug.Assert(args.Count == 2);
                        arg = args[0].Accept(this);
                        arg.Append(" || ");
                        arg.Append(args[1].Accept(this));
                        return arg;

                        // date functions
                    case "Day":
                    case "Hour":
                    case "Minute":
                    case "Month":
                    case "Second":
                    case "Year":
                        FunctionExpression extract_date = new FunctionExpression("extract");
                        System.Diagnostics.Debug.Assert(args.Count == 1);
                        arg = new LiteralExpression(function.Name + " FROM ");
                        ((LiteralExpression)arg).Append(args[0].Accept(this));
                        extract_date.AddArgument(arg);
                        return extract_date;

                    default:
                        throw new NotSupportedException();
                }
            }
            throw new NotSupportedException();
        }


        private Stack<Dictionary<string, string>> _varScopeStack = new Stack<Dictionary<string, string>>();
        private Stack<Stack<string>> _projectScopeStack = new Stack<Stack<string>>();
        private Stack<Stack<string>> _filterScopeStack = new Stack<Stack<string>>();

        private void EnterNewVariableScope()
        {
            _varScopeStack.Push(_variableSubstitution);
            _projectScopeStack.Push(_projectVarName);
            _filterScopeStack.Push(_filterVarName);
            _variableSubstitution = new Dictionary<string, string>();
            _projectVarName = new Stack<string>();
            _filterVarName = new Stack<string>();
        }

        private void LeaveVariableScope()
        {
            _variableSubstitution = _varScopeStack.Pop();
            _projectVarName = _projectScopeStack.Pop();
            _filterVarName = _filterScopeStack.Pop();
        }
    }
}
#endif