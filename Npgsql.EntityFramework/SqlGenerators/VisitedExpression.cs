using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if ENTITIES6
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Metadata.Edm;
#else
using System.Data.Metadata.Edm;
using System.Data.Common.CommandTrees;
#endif
using NpgsqlTypes;
using System.Data;

namespace Npgsql.SqlGenerators
{
    internal abstract class VisitedExpression
    {
        protected VisitedExpression()
        {
            ExpressionList = new List<VisitedExpression>();
        }

        public void Append(VisitedExpression expression)
        {
            ExpressionList.Add(expression);
        }

        public void Append(string literal)
        {
            ExpressionList.Add(new LiteralExpression(literal));
        }

        public override string ToString()
        {
            StringBuilder sqlText = new StringBuilder();
            WriteSql(sqlText);
            return sqlText.ToString();
        }

        protected List<VisitedExpression> ExpressionList { get; private set; }

        internal virtual void WriteSql(StringBuilder sqlText)
        {
            foreach (VisitedExpression expression in ExpressionList)
            {
                expression.WriteSql(sqlText);
            }
        }
    }

    internal class LiteralExpression : VisitedExpression
    {
        private string _literal;

        public LiteralExpression(string literal)
        {
            _literal = literal;
        }

        public new void Append(VisitedExpression expresion)
        {
            base.Append(expresion);
        }

        public new void Append(string literal)
        {
            base.Append(literal);
        }

        internal override void WriteSql(StringBuilder sqlText)
        {
            sqlText.Append(_literal);
            base.WriteSql(sqlText);
        }
    }

    internal class CommaSeparatedExpression : VisitedExpression
    {
        public readonly List<VisitedExpression> Arguments = new List<VisitedExpression>();

        internal override void WriteSql(StringBuilder sqlText)
        {
            for (int i = 0; i < Arguments.Count; ++i)
            {
                if (i != 0)
                    sqlText.Append(", ");
                Arguments[i].WriteSql(sqlText);
            }
            base.WriteSql(sqlText);
        }
    }

    internal class ConstantExpression : VisitedExpression
    {
        private PrimitiveTypeKind _primitiveType;
        private object _value;

        public ConstantExpression(object value, TypeUsage edmType)
        {
            if (edmType == null)
                throw new ArgumentNullException("edmType");
            if (edmType.EdmType == null || edmType.EdmType.BuiltInTypeKind != BuiltInTypeKind.PrimitiveType)
                throw new ArgumentException("Require primitive EdmType", "edmType");
            _primitiveType = ((PrimitiveType)edmType.EdmType).PrimitiveTypeKind;
            _value = value;
        }

        internal override void WriteSql(StringBuilder sqlText)
        {
            NpgsqlNativeTypeInfo typeInfo;
            System.Globalization.NumberFormatInfo ni = NpgsqlNativeTypeInfo.NumberFormat;
            switch (_primitiveType)
            {
                case PrimitiveTypeKind.Binary:
                    {
                        sqlText.AppendFormat("decode('{0}', 'base64')", Convert.ToBase64String((byte[])_value));
                    }
                    break;
                case PrimitiveTypeKind.DateTime:
                    sqlText.AppendFormat(ni, "TIMESTAMP '{0:o}'", _value);
                    break;
                case PrimitiveTypeKind.DateTimeOffset:
                    sqlText.AppendFormat(ni, "TIMESTAMP WITH TIME ZONE '{0:o}'", _value);
                    break;
                case PrimitiveTypeKind.Decimal:
                    sqlText.AppendFormat(ni, "cast({0} as numeric)", _value);
                    break;
                case PrimitiveTypeKind.Double:
                    sqlText.AppendFormat(ni, "cast({0} as float8)", _value);
                    break;
                case PrimitiveTypeKind.Int16:
                    sqlText.AppendFormat(ni, "cast({0} as int2)", _value);
                    break;
                case PrimitiveTypeKind.Int32:
                    sqlText.AppendFormat(ni, "{0}", _value);
                    break;
                case PrimitiveTypeKind.Int64:
                    sqlText.AppendFormat(ni, "cast({0} as int8)", _value);
                    break;
                case PrimitiveTypeKind.Single:
                    sqlText.AppendFormat(ni, "cast({0} as float4)", _value);
                    break;
                case PrimitiveTypeKind.Boolean:
                    sqlText.AppendFormat(ni, "cast({0} as boolean)", ((bool)_value) ? "TRUE" : "FALSE");
                    break;
                case PrimitiveTypeKind.Guid:
                case PrimitiveTypeKind.String:
                    NpgsqlTypesHelper.TryGetNativeTypeInfo(GetDbType(_primitiveType), out typeInfo);
                    // Escape syntax is needed for strings with escape values.
                    // We don't check if there are escaped strings for performance reasons.
                    // Check https://github.com/franciscojunior/Npgsql2/pull/10 for more info.
                    // NativeToBackendTypeConverterOptions.Default should provide the correct
                    // formatting rules for any backend >= 8.0.
                    sqlText.Append(BackendEncoding.UTF8Encoding.GetString(typeInfo.ConvertToBackend(_value, false)));
                    break;
                case PrimitiveTypeKind.Time:
                    sqlText.AppendFormat(ni, "TIME '{0:T}'", _value);
                    break;
                case PrimitiveTypeKind.Byte:
                case PrimitiveTypeKind.SByte:
                default:
                    // TODO: must support more constant value types.
                    throw new NotSupportedException(string.Format("NotSupported: {0} {1}", _primitiveType, _value));
            }
            base.WriteSql(sqlText);
        }

        private DbType GetDbType(PrimitiveTypeKind _primitiveType)
        {
            switch (_primitiveType)
            {
                case PrimitiveTypeKind.Boolean:
                    return DbType.Boolean;
                case PrimitiveTypeKind.Guid:
                    return DbType.Guid;
                case PrimitiveTypeKind.String:
                    return DbType.String;
                default:
                    return DbType.Object;
            }
        }
    }

    internal class InsertExpression : VisitedExpression
    {
        public void AppendTarget(VisitedExpression target)
        {
            Append(target);
        }

        public void AppendColumns(IEnumerable<VisitedExpression> columns)
        {
            Append("(");
            bool first = true;
            foreach (VisitedExpression expression in columns)
            {
                if (!first)
                    Append(",");
                Append(expression);
                first = false;
            }
            Append(")");
        }

        public void AppendValues(IEnumerable<VisitedExpression> columns)
        {
            Append(" VALUES (");
            bool first = true;
            foreach (VisitedExpression expression in columns)
            {
                if (!first)
                    Append(",");
                Append(expression);
                first = false;
            }
            Append(")");
        }

        internal void AppendReturning(DbNewInstanceExpression expression)
        {
            Append(" RETURNING ");//Don't put () around columns it will probably have unwanted effect
            bool first = true;
            foreach (var returingProperty in expression.Arguments)
            {
                if (!first)
                    Append(",");
                Append(SqlBaseGenerator.QuoteIdentifier((returingProperty as DbPropertyExpression).Property.Name));
                first = false;
            }
        }

        internal override void WriteSql(StringBuilder sqlText)
        {
            sqlText.Append("INSERT INTO ");
            base.WriteSql(sqlText);
        }
    }

    internal class UpdateExpression : VisitedExpression
    {
        private bool _setSeperatorRequired;

        public void AppendTarget(VisitedExpression target)
        {
            Append(target);
        }

        public void AppendSet(VisitedExpression property, VisitedExpression value)
        {
            if (_setSeperatorRequired)
                Append(",");
            else
                Append(" SET ");
            Append(property);
            Append("=");
            Append(value);
            _setSeperatorRequired = true;
        }

        public void AppendWhere(VisitedExpression where)
        {
            Append(" WHERE ");
            Append(where);
        }

        internal override void WriteSql(StringBuilder sqlText)
        {
            sqlText.Append("UPDATE ");
            base.WriteSql(sqlText);
        }
    }

    internal class DeleteExpression : VisitedExpression
    {
        public void AppendFrom(VisitedExpression from)
        {
            Append(from);
        }

        public void AppendWhere(VisitedExpression where)
        {
            Append(" WHERE ");
            Append(where);
        }

        internal override void WriteSql(StringBuilder sqlText)
        {
            sqlText.Append("DELETE FROM ");
            base.WriteSql(sqlText);
        }
    }

    internal class ColumnExpression : VisitedExpression
    {
        private VisitedExpression _column;
        private string _columnName;
        private TypeUsage _columnType;

        public ColumnExpression(VisitedExpression column, string columnName, TypeUsage columnType)
        {
            _column = column;
            _columnName = columnName;
            _columnType = columnType;
        }

        public string Name { get { return _columnName; } }
        internal TypeUsage ColumnType { get { return _columnType; ;} }

        public Type CLRType
        {
            get
            {
                if (_columnType == null)
                    return null;
                PrimitiveType pt = _columnType.EdmType as PrimitiveType;
                if (pt != null)
                    return pt.ClrEquivalentType;
                else
                    return null;
            }
        }

        internal override void WriteSql(StringBuilder sqlText)
        {
            _column.WriteSql(sqlText);
            sqlText.Append(" AS " + SqlBaseGenerator.QuoteIdentifier(_columnName));
            base.WriteSql(sqlText);
        }
    }

    internal class ColumnReferenceExpression : VisitedExpression
    {
        public string Variable { get; set; }
        public string Name { get; set; }

        internal override void WriteSql(StringBuilder sqlText)
        {
            if (Variable != null)
            {
                sqlText.Append(SqlBaseGenerator.QuoteIdentifier(Variable));
                sqlText.Append(".");
            }
            sqlText.Append(SqlBaseGenerator.QuoteIdentifier(Name));
            base.WriteSql(sqlText);
        }
    }

    internal class ScanExpression : VisitedExpression
    {
        private string _scanString;
        private EntitySetBase _target;

        public ScanExpression(string scanString, EntitySetBase target)
        {
            _scanString = scanString;
            _target = target;
        }

        internal EntitySetBase Target { get { return _target; } }

        internal override void WriteSql(StringBuilder sqlText)
        {
            sqlText.Append(_scanString);
            base.WriteSql(sqlText);
        }
    }

    internal class InputExpression : VisitedExpression
    {
        public bool Distinct { get; set; }

        public CommaSeparatedExpression Projection { get; set; }

        public readonly Dictionary<Tuple<string, string>, string> ColumnsToProject = new Dictionary<Tuple<string, string>, string>(); // (from, name) -> newName
        public readonly HashSet<string> ProjectNewNames = new HashSet<string>();

        // Either FromExpression or JoinExpression
        public VisitedExpression From { get; set; }

        private WhereExpression _where;

        public WhereExpression Where
        {
            get { return _where; }
            set
            {
                _where = value;
            }
        }

        private GroupByExpression _groupBy;

        public GroupByExpression GroupBy
        {
            get { return _groupBy; }
            set
            {
                _groupBy = value;
            }
        }

        private OrderByExpression _orderBy;

        public OrderByExpression OrderBy
        {
            get { return _orderBy; }
            set { _orderBy = value; }
        }

        private SkipExpression _skip;

        public SkipExpression Skip
        {
            get { return _skip; }
            set { _skip = value; }
        }

        private LimitExpression _limit;

        public LimitExpression Limit
        {
            get { return _limit; }
            set
            {
                _limit = value;
            }
        }

        public InputExpression() { }

        public InputExpression(VisitedExpression from, string asName)
        {
            From = new FromExpression(from, asName);
        }

        internal override void WriteSql(StringBuilder sqlText)
        {
            sqlText.Append("SELECT ");
            if (Distinct) sqlText.Append("DISTINCT ");
            if (Projection != null) Projection.WriteSql(sqlText);
            else
            {
                if (ColumnsToProject.Count == 0) sqlText.Append("1"); // Could be arbitrary, let's pick 1
                else
                {
                    bool first = true;
                    foreach (var column in ColumnsToProject)
                    {
                        if (!first)
                        {
                            sqlText.Append(", ");
                        }
                        else first = false;
                        sqlText.Append(SqlBaseGenerator.QuoteIdentifier(column.Key.Item1));
                        sqlText.Append(".");
                        sqlText.Append(SqlBaseGenerator.QuoteIdentifier(column.Key.Item2));
                        sqlText.Append(" AS ");
                        sqlText.Append(SqlBaseGenerator.QuoteIdentifier(column.Value));
                    }
                }
            }
            sqlText.Append(" FROM ");
            From.WriteSql(sqlText);
            if (Where != null) Where.WriteSql(sqlText);
            if (GroupBy != null) GroupBy.WriteSql(sqlText);
            if (OrderBy != null) OrderBy.WriteSql(sqlText);
            if (Skip != null) Skip.WriteSql(sqlText);
            if (Limit != null) Limit.WriteSql(sqlText);
            base.WriteSql(sqlText);
        }
    }

    internal class FromExpression : VisitedExpression
    {
        private VisitedExpression _from;
        private string _name;

        public FromExpression(VisitedExpression from, string name)
        {
            _from = from;
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }

        public bool ForceSubquery { get; set; }

        internal override void WriteSql(StringBuilder sqlText)
        {
            if (_from is InputExpression)
            {
                InputExpression input = (InputExpression)_from;
                if (!ForceSubquery && input.Projection == null && input.Where == null && input.Distinct == false && input.OrderBy == null &&
                    input.Skip == null && input.Limit == null)
                {
                    // There is no point of writing
                    // (SELECT ? FROM <from> AS <name>) AS <name>
                    // so just write <from> AS <name>
                    // <name> is always the same for both nodes
                    // However, PostgreSQL needs a subquery in case we are in the right hand side of an Apply expression
                    if (((FromExpression)input.From).Name != Name)
                        throw new ArgumentException();
                    input.From.WriteSql(sqlText);
                }
                else
                {
                    sqlText.Append("(");
                    input.WriteSql(sqlText);
                    sqlText.Append(") AS ");
                    sqlText.Append(SqlBaseGenerator.QuoteIdentifier(_name));
                }
            }
            else
            {
                bool wrap = !(_from is LiteralExpression || _from is ScanExpression);
                if (wrap)
                    sqlText.Append("(");
                _from.WriteSql(sqlText);
                if (wrap)
                    sqlText.Append(")");
                sqlText.Append(" AS ");
                sqlText.Append(SqlBaseGenerator.QuoteIdentifier(_name));
            }
            base.WriteSql(sqlText);
        }
    }

    internal class JoinExpression : VisitedExpression
    {
        private VisitedExpression _left;
        private DbExpressionKind _joinType;
        private VisitedExpression _right;
        private VisitedExpression _condition;

        public JoinExpression() { }

        public JoinExpression(InputExpression left, DbExpressionKind joinType, InputExpression right, VisitedExpression condition)
        {
            _left = left;
            _joinType = joinType;
            _right = right;
            _condition = condition;
        }

        public VisitedExpression Left { get { return _left; } set { _left = value; } }
        public DbExpressionKind JoinType { get { return _joinType; } set { _joinType = value; } }
        public VisitedExpression Right { get { return _right; } set { _right = value; } }

        public VisitedExpression Condition
        {
            get { return _condition; }
            set { _condition = value; }
        }

        internal override void WriteSql(StringBuilder sqlText)
        {
            _left.WriteSql(sqlText);
            switch (_joinType)
            {
                case DbExpressionKind.InnerJoin:
                    sqlText.Append(" INNER JOIN ");
                    break;
                case DbExpressionKind.LeftOuterJoin:
                    sqlText.Append(" LEFT OUTER JOIN ");
                    break;
                case DbExpressionKind.FullOuterJoin:
                    sqlText.Append(" FULL OUTER JOIN ");
                    break;
                case DbExpressionKind.CrossJoin:
                    sqlText.Append(" CROSS JOIN ");
                    break;
                case DbExpressionKind.CrossApply:
                    sqlText.Append(" CROSS JOIN LATERAL ");
                    break;
                case DbExpressionKind.OuterApply:
                    sqlText.Append(" LEFT OUTER JOIN LATERAL ");
                    break;
                default:
                    throw new NotSupportedException();
            }
            _right.WriteSql(sqlText);
            if (_joinType == DbExpressionKind.OuterApply)
                sqlText.Append(" ON TRUE");
            else if (_joinType != DbExpressionKind.CrossJoin && _joinType != DbExpressionKind.CrossApply)
            {
                sqlText.Append(" ON ");
                _condition.WriteSql(sqlText);
            }
            base.WriteSql(sqlText);
        }
    }

    internal class WhereExpression : VisitedExpression
    {
        private VisitedExpression _where;

        public WhereExpression(VisitedExpression where)
        {
            _where = where;
        }

        internal override void WriteSql(StringBuilder sqlText)
        {
            sqlText.Append(" WHERE ");
            _where.WriteSql(sqlText);
            base.WriteSql(sqlText);
        }

        internal void And(VisitedExpression andAlso)
        {
            _where = new BooleanExpression("AND", _where, andAlso);
        }
    }

    internal class PropertyExpression : VisitedExpression
    {
        private EdmMember _property;

        // used for inserts or updates where the column is not qualified
        public PropertyExpression(EdmMember property)
        {
            _property = property;
        }

        public string Name { get { return _property.Name; } }

        public TypeUsage PropertyType { get { return _property.TypeUsage; } }

        internal override void WriteSql(StringBuilder sqlText)
        {
            sqlText.Append(SqlBaseGenerator.QuoteIdentifier(_property.Name));
            base.WriteSql(sqlText);
        }

        // override ToString since we don't want variable substitution or identifier quoting
        // until writing out the SQL.
        public override string ToString()
        {
            return _property.Name;
        }
    }

    internal class FunctionExpression : VisitedExpression
    {
        private string _name;
        private List<VisitedExpression> _args = new List<VisitedExpression>();

        public FunctionExpression(string name)
        {
            _name = name;
        }

        internal void AddArgument(VisitedExpression visitedExpression)
        {
            _args.Add(visitedExpression);
        }

        internal override void WriteSql(StringBuilder sqlText)
        {
            sqlText.Append(_name);
            sqlText.Append("(");
            bool first = true;
            foreach (var arg in _args)
            {
                if (!first)
                    sqlText.Append(",");
                arg.WriteSql(sqlText);
                first = false;
            }
            sqlText.Append(")");
            base.WriteSql(sqlText);
        }
    }

    internal class CastExpression : VisitedExpression
    {
        private VisitedExpression _value;
        private string _type;

        public CastExpression(VisitedExpression value, string type)
        {
            _value = value;
            _type = type;
        }

        internal override void WriteSql(StringBuilder sqlText)
        {
            sqlText.Append("CAST (");
            _value.WriteSql(sqlText);
            sqlText.AppendFormat(" AS {0})", _type);
            base.WriteSql(sqlText);
        }
    }

    internal class GroupByExpression : VisitedExpression
    {
        private bool _requiresGroupSeperator;

        public void AppendGroupingKey(VisitedExpression key)
        {
            if (_requiresGroupSeperator)
                Append(",");
            Append(key);
            _requiresGroupSeperator = true;
        }

        internal override void WriteSql(StringBuilder sqlText)
        {
            if (ExpressionList.Count != 0)
                sqlText.Append(" GROUP BY ");
            base.WriteSql(sqlText);
        }
    }

    internal class LimitExpression : VisitedExpression
    {
        private VisitedExpression _arg;

        public VisitedExpression Arg { get { return _arg; } set { _arg = value; } }

        public LimitExpression(VisitedExpression arg)
        {
            _arg = arg;
        }

        internal override void WriteSql(StringBuilder sqlText)
        {
            sqlText.Append(" LIMIT ");
            _arg.WriteSql(sqlText);
            base.WriteSql(sqlText);
        }
    }

    internal class SkipExpression : VisitedExpression
    {
        private VisitedExpression _arg;

        public SkipExpression(VisitedExpression arg)
        {
            _arg = arg;
        }

        internal override void WriteSql(StringBuilder sqlText)
        {
            sqlText.Append(" OFFSET ");
            _arg.WriteSql(sqlText);
            base.WriteSql(sqlText);
        }
    }

    internal class BooleanExpression : VisitedExpression
    {
        private string _booleanOperator;
        private VisitedExpression _left;
        private VisitedExpression _right;

        public BooleanExpression(string booleanOperator, VisitedExpression left, VisitedExpression right)
        {
            _booleanOperator = booleanOperator;
            _left = left;
            _right = right;
        }

        internal override void WriteSql(StringBuilder sqlText)
        {
            bool wrapLeft = !(_left is PropertyExpression || _left is ColumnReferenceExpression || _left is ConstantExpression);
            bool wrapRight = !(_right is PropertyExpression || _right is ColumnReferenceExpression || _right is ConstantExpression);
            if (wrapLeft)
                sqlText.Append("(");
            _left.WriteSql(sqlText);
            if (wrapLeft)
                sqlText.Append(") ");
            sqlText.Append(_booleanOperator);
            if (wrapRight)
                sqlText.Append(" (");
            _right.WriteSql(sqlText);
            if (wrapRight)
                sqlText.Append(")");
            base.WriteSql(sqlText);
        }
    }

    internal class NegatableBooleanExpression : NegatableExpression
    {
        private DbExpressionKind _booleanOperator;
        private VisitedExpression _left;
        private VisitedExpression _right;

        public NegatableBooleanExpression(DbExpressionKind booleanOperator, VisitedExpression left, VisitedExpression right)
        {
            _booleanOperator = booleanOperator;
            _left = left;
            _right = right;
        }

        internal override void WriteSql(StringBuilder sqlText)
        {
            bool wrapLeft = !(_left is PropertyExpression || _left is ColumnReferenceExpression || _left is ConstantExpression);
            bool wrapRight = !(_right is PropertyExpression || _right is ColumnReferenceExpression || _right is ConstantExpression);
            if (wrapLeft)
                sqlText.Append("(");
            _left.WriteSql(sqlText);
            if (wrapLeft)
                sqlText.Append(") ");
            switch (_booleanOperator)
            {
                case DbExpressionKind.Equals:
                    if (Negated)
                        sqlText.Append("!=");
                    else
                        sqlText.Append("=");
                    break;
                case DbExpressionKind.GreaterThan:
                    if (Negated)
                        sqlText.Append("<=");
                    else
                        sqlText.Append(">");
                    break;
                case DbExpressionKind.GreaterThanOrEquals:
                    if (Negated)
                        sqlText.Append("<");
                    else
                        sqlText.Append(">=");
                    break;
                case DbExpressionKind.LessThan:
                    if (Negated)
                        sqlText.Append(">=");
                    else
                        sqlText.Append("<");
                    break;
                case DbExpressionKind.LessThanOrEquals:
                    if (Negated)
                        sqlText.Append(">");
                    else
                        sqlText.Append("<=");
                    break;
                case DbExpressionKind.Like:
                    if (Negated)
                        sqlText.Append(" NOT");
                    sqlText.Append(" LIKE ");
                    break;
                case DbExpressionKind.NotEquals:
                    if (Negated)
                        sqlText.Append("=");
                    else
                        sqlText.Append("!=");
                    break;
                default:
                    throw new NotSupportedException();
            }
            if (wrapRight)
                sqlText.Append(" (");
            _right.WriteSql(sqlText);
            if (wrapRight)
                sqlText.Append(")");
            base.WriteSql(sqlText);
        }
    }

    internal class InExpression : VisitedExpression
    {
        private VisitedExpression _left;
        private List<ConstantExpression> _list;

        public InExpression(VisitedExpression left, List<ConstantExpression> list)
        {
            _left = left;
            _list = list;
        }

        internal override void WriteSql(StringBuilder sqlText)
        {
            _left.WriteSql(sqlText);
            sqlText.Append(" IN (");
            bool first = true;
            foreach (var constant in _list)
            {
                if (!first)
                    sqlText.Append(",");
                constant.WriteSql(sqlText);
                first = false;
            }
            sqlText.Append(")");
            base.WriteSql(sqlText);
        }
    }

    internal class CombinedProjectionExpression : VisitedExpression
    {
        private VisitedExpression _first;
        private VisitedExpression _second;
        private string _setOperator;

        public CombinedProjectionExpression(VisitedExpression first, string setOperator, VisitedExpression second)
        {
            _first = first;
            _setOperator = setOperator;
            _second = second;
        }

        public CombinedProjectionExpression(VisitedExpression first, DbExpressionKind setOperator, VisitedExpression second)
        {
            _first = first;
            _setOperator = setOperator == DbExpressionKind.UnionAll ? "UNION ALL" : setOperator == DbExpressionKind.Except ? "EXCEPT" : "INTERSECT";
            _second = second;
        }

        internal override void WriteSql(StringBuilder sqlText)
        {
            sqlText.Append("(");
            _first.WriteSql(sqlText);
            sqlText.Append(") ");
            sqlText.Append(_setOperator);
            sqlText.Append(" (");
            _second.WriteSql(sqlText);
            sqlText.Append(")");
            base.WriteSql(sqlText);
        }
    }

    internal class NegatableExpression : VisitedExpression
    {
        private bool _negated;

        protected bool Negated
        {
            get { return _negated; }
            set { _negated = value; }
        }

        public NegatableExpression Negate()
        {
            _negated = !_negated;
            // allows to be used inline
            return this;
        }
    }

    internal class ExistsExpression : NegatableExpression
    {
        private VisitedExpression _argument;

        public ExistsExpression(VisitedExpression argument)
        {
            _argument = argument;
        }

        internal override void WriteSql(StringBuilder sqlText)
        {
            if (Negated)
                sqlText.Append("NOT ");
            sqlText.Append("EXISTS (");
            _argument.WriteSql(sqlText);
            sqlText.Append(")");
            base.WriteSql(sqlText);
        }
    }

    internal class NegateExpression : NegatableExpression
    {
        private VisitedExpression _argument;

        public NegateExpression(VisitedExpression argument)
        {
            _argument = argument;
            Negated = true;
        }

        internal override void WriteSql(StringBuilder sqlText)
        {
            if (Negated)
                sqlText.Append(" NOT ");
            sqlText.Append("(");
            _argument.WriteSql(sqlText);
            sqlText.Append(")");
            base.WriteSql(sqlText);
        }
    }

    internal class IsNullExpression : NegatableExpression
    {
        private VisitedExpression _argument;

        public IsNullExpression(VisitedExpression argument)
        {
            _argument = argument;
        }

        internal override void WriteSql(StringBuilder sqlText)
        {
            _argument.WriteSql(sqlText);
            sqlText.Append(" IS ");
            if (Negated)
                sqlText.Append("NOT ");
            sqlText.Append("NULL ");
            base.WriteSql(sqlText);
        }
    }

    class OrderByExpression : VisitedExpression
    {
        private bool _requiresOrderSeperator;

        public void AppendSort(VisitedExpression sort, bool ascending)
        {
            if (_requiresOrderSeperator)
                Append(",");
            Append(sort);
            if (ascending)
                Append(" ASC ");
            else
                Append(" DESC ");
            _requiresOrderSeperator = true;
        }

        internal override void WriteSql(StringBuilder sqlText)
        {
            sqlText.Append(" ORDER BY ");
            base.WriteSql(sqlText);
        }
    }
}
