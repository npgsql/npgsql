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

        public override VisitedExpression Visit(DbViewExpression expression)
        {
            throw new NotImplementedException();
        }

        public override VisitedExpression Visit(DbVariableReferenceExpression expression)
        {
            return new VariableReferenceExpression(expression.VariableName, _variableSubstitution);
        }

        public override VisitedExpression Visit(DbUnionAllExpression expression)
        {
            // UNION ALL keyword
            VisitedExpression union = expression.Left.Accept(this);
            union.Append(" UNION ALL ");
            union.Append(expression.Right.Accept(this));
            return union;
        }

        public override VisitedExpression Visit(DbTreatExpression expression)
        {
            throw new NotImplementedException();
        }

        public override VisitedExpression Visit(DbSkipExpression expression)
        {
            // not quite sure, but may be a paging like function
            // almost the opposite of limit, possibly actually the skip those first.
            throw new NotImplementedException();
        }

        public override VisitedExpression Visit(DbSortExpression expression)
        {
            // order by
            _filterVarName.Push(expression.Input.VariableName);
            VisitedExpression inputExpression = expression.Input.Expression.Accept(this);
            VisitedExpression from;
            if (!(inputExpression is JoinExpression))
            {
                from = new FromExpression(inputExpression, expression.Input.VariableName);
                _variableSubstitution[_projectVarName.Peek()] = expression.Input.VariableName;
                SubstituteFilterVar(expression.Input.VariableName);
            }
            else
            {
                from = inputExpression;
            }
            _filterVarName.Pop();
            from.Append(" ORDER BY ");
            bool first = true;
            foreach (var order in expression.SortOrder)
            {
                if (!first)
                    from.Append(",");
                from.Append(order.Expression.Accept(this));
                if (order.Ascending)
                    from.Append(" ASC ");
                else
                    from.Append(" DESC ");
                first = false;
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
            return new LiteralExpression(expression.Target.Name);
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
            // EXISTS or NOT EXISTS depending on expression.ExpressionKind
            // comes with it's built in test (subselect for EXISTS)
            throw new NotImplementedException();
        }

        public override VisitedExpression Visit(DbProjectExpression expression)
        {
            VisitedExpression project = expression.Projection.Accept(this);
            project.Append(" FROM ");
            _projectVarName.Push(expression.Input.VariableName);
            AppendFrom(project, expression.Input.Expression, expression.Input.VariableName);
            _projectVarName.Pop();

            return project;
        }

        private void AppendFrom(VisitedExpression visitedExpression, DbExpression dbFromExpression, string variableName)
        {
            VisitedExpression fromExpression = dbFromExpression.Accept(this);
            if (!(fromExpression is FromExpression) && !(fromExpression is JoinExpression))
            {
                fromExpression = new FromExpression(fromExpression, variableName);
                _variableSubstitution[_projectVarName.Peek()] = variableName;
                SubstituteFilterVar(variableName);
            }
            visitedExpression.Append(fromExpression);
        }

        public override VisitedExpression Visit(DbParameterReferenceExpression expression)
        {
            // use parameter in sql
            return new LiteralExpression("@" + expression.ParameterName);
        }

        public override VisitedExpression Visit(DbOrExpression expression)
        {
            LiteralExpression or = new LiteralExpression("(");
            or.Append(expression.Left.Accept(this));
            or.Append(") OR (");
            or.Append(expression.Right.Accept(this));
            or.Append(")");
            return or;
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
            // TODO: produces ugly SQL, particularly in the case of NOT NOT EXISTS
            LiteralExpression not = new LiteralExpression(" NOT (");
            not.Append(expression.Argument.Accept(this));
            not.Append(")");
            return not;
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
                    if (i != 0)
                        visitedExpression.Append(",");
                    visitedExpression.Append(new ColumnExpression(expression.Arguments[i].Accept(this), rowType.Properties[i].Name));
                }

                return visitedExpression;
            }
            else if (expression.ResultType.EdmType is CollectionType)
            {
                ProjectionExpression visitedExpression = new ProjectionExpression();
                bool first = true;
                foreach (var arg in expression.Arguments)
                {
                    if (!first)
                        visitedExpression.Append(",");
                    visitedExpression.Append(arg.Accept(this));
                    first = false;
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
            // TODO: what is WithTies?
            FromExpression limit = new FromExpression(expression.Argument.Accept(this), null);
            _variableSubstitution[_projectVarName.Peek()] = limit.Name;
            SubstituteFilterVar(limit.Name);
            limit.Append(" LIMIT ");
            limit.Append(expression.Limit.Accept(this));
            return limit;
        }

        public override VisitedExpression Visit(DbLikeExpression expression)
        {
            // LIKE keyword
            // also uses ESCAPE
            // ESCAPE may be way of identifying wild cards
            // TODO: enhance this.  Only supporting simple case for now
            VisitedExpression visited = expression.Argument.Accept(this);
            visited.Append(" LIKE ");
            visited.Append(expression.Pattern.Accept(this));
            return visited;
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
            VisitedExpression joinPartExpression = joinPart.Expression.Accept(this);
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
            VisitedExpression visited = expression.Argument.Accept(this);
            visited.Append(" IS NULL ");
            return visited;
        }

        public override VisitedExpression Visit(DbIsEmptyExpression expression)
        {
            // NOT EXISTS
            LiteralExpression exists = new LiteralExpression(" NOT EXISTS (");
            exists.Append(expression.Argument.Accept(this));
            exists.Append(")");
            return exists;
        }

        public override VisitedExpression Visit(DbIntersectExpression expression)
        {
            // INTERSECT keyword
            throw new NotImplementedException();
        }

        public override VisitedExpression Visit(DbGroupByExpression expression)
        {
            // complicated
            // GROUP BY expression
            // first implementation this is a COUNT(column) query ???
            EnterNewVariableScope();
            ProjectionExpression projectExpression = new ProjectionExpression();
            GroupByExpression groupByExpression = new GroupByExpression();
            RowType rowType = ((CollectionType)(expression.ResultType.EdmType)).TypeUsage.EdmType as RowType;
            int columnIndex = 0;
            foreach (var key in expression.Keys)
            {
                if (columnIndex != 0)
                    projectExpression.Append(",");
                VisitedExpression keyColumnExpression = key.Accept(this);
                projectExpression.Append(new ColumnExpression(keyColumnExpression, rowType.Properties[columnIndex].Name));
                groupByExpression.Append(keyColumnExpression);
                ++columnIndex;
            }
            foreach (var ag in expression.Aggregates)
            {
                DbFunctionAggregate function = ag as DbFunctionAggregate;
                if (function == null)
                    throw new NotSupportedException();
                VisitedExpression functionExpression = VisitFunction(function);
                if (columnIndex != 0)
                    projectExpression.Append(",");
                projectExpression.Append(new ColumnExpression(functionExpression, rowType.Properties[columnIndex].Name));
                ++columnIndex;
            }
            projectExpression.Append(" FROM ");
            _projectVarName.Push(expression.Input.GroupVariableName);
            _filterVarName.Push(expression.Input.VariableName);
            AppendFrom(projectExpression, expression.Input.Expression, expression.Input.GroupVariableName);
            projectExpression.Append(groupByExpression);
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
            LeaveVariableScope();
            //_variableSubstitution[_projectVarName.Peek()] = expression.Input.VariableName;
            //return new FromExpression(projectExpression, expression.Input.VariableName);
            return projectExpression;
        }

        public override VisitedExpression Visit(DbRefKeyExpression expression)
        {
            // not supported by sample
            throw new NotImplementedException();
        }

        public override VisitedExpression Visit(DbEntityRefExpression expression)
        {
            // not supported by sample
            throw new NotImplementedException();
        }

        public override VisitedExpression Visit(DbFunctionExpression expression)
        {
            if (expression.IsLambda)
                throw new NotSupportedException();
            // a function call
            // may be built in, canonical, or user defined
            return VisitFunction(expression.Function, expression.Arguments, expression.ResultType);
        }

        public override VisitedExpression Visit(DbFilterExpression expression)
        {
            // complicated
            // similar logic used for other expressions (such as group by)
            // TODO: this is too simple.  Replace this
            // need to move the from keyword out so that it can be used in the project
            // when there is no where clause
            _filterVarName.Push(expression.Input.VariableName);
            VisitedExpression inputExpression = expression.Input.Expression.Accept(this);
            VisitedExpression from;
            if (!(inputExpression is JoinExpression))
            {
                from = new FromExpression(inputExpression, expression.Input.VariableName);
                _variableSubstitution[_projectVarName.Peek()] = expression.Input.VariableName;
                if (_variableSubstitution.ContainsKey(_filterVarName.Peek()))
                    _variableSubstitution[_filterVarName.Peek()] = expression.Input.VariableName;
            }
            else
            {
                from = inputExpression;
            }
            from.Append(new WhereExpression(expression.Predicate.Accept(this)));
            _filterVarName.Pop();
            return from;
        }

        public override VisitedExpression Visit(DbExceptExpression expression)
        {
            // Except keyword
            throw new NotImplementedException();
        }

        public override VisitedExpression Visit(DbElementExpression expression)
        {
            // a scalar expression (ie ExecuteScalar)
            // so it will likely be translated into a select
            //throw new NotImplementedException();
            VisitedExpression scalar = new LiteralExpression("(");
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
            // don't know.  Sample generator doesn't support it.
            throw new NotImplementedException();
        }

        public override VisitedExpression Visit(DbCrossJoinExpression expression)
        {
            throw new NotImplementedException();
        }

        public override VisitedExpression Visit(DbConstantExpression expression)
        {
            // literals to be inserted into the sql
            // may require some formatting depending on the type
            //throw new NotImplementedException();
            // TODO: this is just for testing
            return new ConstantExpression(expression.Value.ToString(), expression.ResultType);
        }

        public override VisitedExpression Visit(DbComparisonExpression expression)
        {
            VisitedExpression comparison = expression.Left.Accept(this);
            switch (expression.ExpressionKind)
            {
                case DbExpressionKind.Equals:
                    comparison.Append("=");
                    break;
                case DbExpressionKind.GreaterThan:
                    comparison.Append(">");
                    break;
                case DbExpressionKind.GreaterThanOrEquals:
                    comparison.Append(">=");
                    break;
                case DbExpressionKind.LessThan:
                    comparison.Append("<");
                    break;
                case DbExpressionKind.LessThanOrEquals:
                    comparison.Append("<=");
                    break;
                case DbExpressionKind.Like:
                    comparison.Append(" LIKE ");
                    break;
                case DbExpressionKind.NotEquals:
                    comparison.Append("!=");
                    break;
                default:
                    throw new NotSupportedException();
            }
            comparison.Append(expression.Right.Accept(this));
            return comparison;
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
                    return "boolean";
                case PrimitiveTypeKind.Int16:
                    return "smallint";
                case PrimitiveTypeKind.Int32:
                    return "integer";
                case PrimitiveTypeKind.Int64:
                    return "bigint";
                case PrimitiveTypeKind.String:
                    return "character varying";
                case PrimitiveTypeKind.Decimal:
                    return "numeric";
                case PrimitiveTypeKind.DateTime:
                    return "timestamp without time zone";
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
            throw new NotImplementedException();
        }

        public override VisitedExpression Visit(DbApplyExpression expression)
        {
            // like a join, but used when one of the arguments is a function.
            // it lets you return the results of a function call given values from the
            // other table.
            // see: http://www.sqlteam.com/article/using-cross-apply-in-sql-server-2005
            // for ideas on implementing
            // see: http://www.paragoncorporation.com/ITConsumerGuide.aspx?ArticleID=1
            throw new NotImplementedException();
        }

        public override VisitedExpression Visit(DbAndExpression expression)
        {
            LiteralExpression and = new LiteralExpression("(");
            and.Append(expression.Left.Accept(this));
            and.Append(") AND (");
            and.Append(expression.Right.Accept(this));
            and.Append(")");
            return and;
        }

        public override VisitedExpression Visit(DbExpression expression)
        {
            // only concrete types visited
            throw new NotSupportedException();
        }

        public abstract void BuildCommand(DbCommand command);

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
                    aggregateArg.Append(functionAggregate.Arguments[0].Accept(this));
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
                // TODO: support more functions
                switch (function.Name)
                {
                    case "Length":
                        FunctionExpression length = new FunctionExpression("char_length");
                        System.Diagnostics.Debug.Assert(args.Count == 1);
                        length.AddArgument(args[0].Accept(this));
                        return new CastExpression(length, GetDbType(resultType.EdmType));
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