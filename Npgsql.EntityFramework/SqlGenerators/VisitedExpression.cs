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

        internal virtual IEnumerable<ColumnExpression> GetProjectedColumns()
        {
            return ExpressionList.Aggregate(Enumerable.Empty<ColumnExpression>(), (list, ve) => list.Concat(ve.GetProjectedColumns()));
        }

        internal virtual IEnumerable<PropertyExpression> GetAccessedProperties()
        {
            return ExpressionList.Aggregate(Enumerable.Empty<PropertyExpression>(), (list, ve) => list.Concat(ve.GetAccessedProperties()));
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

    internal class ProjectionExpression : VisitedExpression
    {
        private bool requiresColumnSeperator;
        private InputExpression _from;

        public bool Distinct { get; set; }
        public InputExpression From
        {
            get { return _from; }
            set
            {
                _from = value;
                Append(" FROM ");
                Append(_from);
            }
        }

        internal override void WriteSql(StringBuilder sqlText)
        {
            sqlText.Append("SELECT ");
            if (Distinct)
                sqlText.Append("DISTINCT ");
            base.WriteSql(sqlText);
        }

        internal IEnumerable<ColumnExpression> Columns { get { return _columns; } }

        private List<ColumnExpression> _columns = new List<ColumnExpression>();
        public void AppendColumn(ColumnExpression column)
        {
            _columns.Add(column);
            if (requiresColumnSeperator)
                Append(",");
            Append(column);
            requiresColumnSeperator = true;
        }

        public void ReplaceColumn(ColumnExpression existingColumn, ColumnExpression newColumn)
        {
            int index = _columns.IndexOf(existingColumn);
            if (index != -1)
            {
                _columns[index] = newColumn;
                int baseIndex = ExpressionList.IndexOf(existingColumn);
                ExpressionList[baseIndex] = newColumn;
            }
        }

        internal override IEnumerable<ColumnExpression> GetProjectedColumns()
        {
            return _columns;
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
                Append("\"" + (returingProperty as DbPropertyExpression).Property.Name + "\"");
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

        internal override IEnumerable<ColumnExpression> GetProjectedColumns()
        {
            return Enumerable.Empty<ColumnExpression>();
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

        internal override IEnumerable<ColumnExpression> GetProjectedColumns()
        {
            return Enumerable.Empty<ColumnExpression>();
        }
    }

    internal class AllColumnsExpression : VisitedExpression
    {
        private InputExpression _input;

        public AllColumnsExpression(InputExpression input)
        {
            _input = input;
        }

        internal override void WriteSql(StringBuilder sqlText)
        {
            sqlText.Append("SELECT ");
            bool first = true;
            foreach (var column in _input.GetProjectedColumns())
            {
                if (!first)
                    sqlText.Append(",");
                first = false;
                column.WriteSql(sqlText);
            }
            sqlText.Append(" FROM ");
            _input.WriteSql(sqlText);
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

        internal override IEnumerable<ColumnExpression> GetProjectedColumns()
        {
            return new ColumnExpression[] { this };
        }

        internal override IEnumerable<PropertyExpression> GetAccessedProperties()
        {
            return _column.GetAccessedProperties().Concat(base.GetAccessedProperties());
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

        List<ColumnExpression> _projectedColumns;
        internal override IEnumerable<ColumnExpression> GetProjectedColumns()
        {
            if (_projectedColumns == null)
            {
                _projectedColumns = new List<ColumnExpression>();
                foreach (var property in _target.ElementType.Members.OfType<EdmProperty>())
                {
                    _projectedColumns.Add(new ColumnExpression(new PropertyExpression(property), property.Name, property.TypeUsage));
                }
            }
            return _projectedColumns;
        }
    }

    internal class InputExpression : VisitedExpression
    {
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

        internal override void WriteSql(StringBuilder sqlText)
        {
            base.WriteSql(sqlText);
            if (Where != null) Where.WriteSql(sqlText);
            if (GroupBy != null) GroupBy.WriteSql(sqlText);
            if (OrderBy != null) OrderBy.WriteSql(sqlText);
            if (Skip != null) Skip.WriteSql(sqlText);
            if (Limit != null) Limit.WriteSql(sqlText);
        }

        internal override IEnumerable<PropertyExpression> GetAccessedProperties()
        {
            var accessedProperties = base.GetAccessedProperties();
            if (Where != null) accessedProperties = accessedProperties.Concat(Where.GetAccessedProperties());
            if (GroupBy != null) accessedProperties = accessedProperties.Concat(GroupBy.GetAccessedProperties());
            if (OrderBy != null) accessedProperties = accessedProperties.Concat(OrderBy.GetAccessedProperties());
            if (Skip != null) accessedProperties = accessedProperties.Concat(Skip.GetAccessedProperties());
            if (Limit != null) accessedProperties = accessedProperties.Concat(Limit.GetAccessedProperties());
            return accessedProperties;
        }
    }

    internal class FromExpression : InputExpression
    {
        private VisitedExpression _from;
        private string _name;
        static int _uniqueName = 1;

        public FromExpression(VisitedExpression from, string name)
        {
            _from = from;
            if (name != null)
            {
                _name = name;
            }
            else
            {
                _name = "ALIAS" + _uniqueName++;
            }
        }

        public string Name
        {
            get { return _name; }
        }

        internal override void WriteSql(StringBuilder sqlText)
        {
            bool wrap = !(_from is LiteralExpression || _from is ScanExpression);
            if (wrap)
                sqlText.Append("(");
            _from.WriteSql(sqlText);
            if (wrap)
                sqlText.Append(")");
            sqlText.Append(" AS ");
            sqlText.Append(SqlBaseGenerator.QuoteIdentifier(_name));
            base.WriteSql(sqlText);
        }

        internal override IEnumerable<ColumnExpression> GetProjectedColumns()
        {
            Dictionary<string, string> emptySubstitution = new Dictionary<string, string>();
            if (_from is ScanExpression)
            {
                ScanExpression scan = (ScanExpression)_from;
                foreach (var property in scan.Target.ElementType.Members.OfType<EdmProperty>())
                {
                    yield return new ColumnExpression(new PropertyExpression(new VariableReferenceExpression(Name, emptySubstitution), property), property.Name, property.TypeUsage);
                }
            }
            else
            {
                foreach (var column in _from.GetProjectedColumns())
                {
                    string columnRef = string.Format("{0}.{1}", SqlBaseGenerator.QuoteIdentifier(Name), column.Name);
                    yield return new ColumnExpression(new LiteralExpression(columnRef), column.Name, column.ColumnType);
                }
            }
        }

        internal override IEnumerable<PropertyExpression> GetAccessedProperties()
        {
            return _from.GetAccessedProperties().Concat(base.GetAccessedProperties());
        }
    }

    internal class JoinExpression : InputExpression
    {
        private InputExpression _left;
        private DbExpressionKind _joinType;
        private InputExpression _right;
        private VisitedExpression _condition;

        public JoinExpression(InputExpression left, DbExpressionKind joinType, InputExpression right, VisitedExpression condition)
        {
            _left = left;
            _joinType = joinType;
            _right = right;
            _condition = condition;
        }

        public InputExpression Left { get { return _left; } }
        public DbExpressionKind JoinType { get { return _joinType; } }
        public InputExpression Right { get { return _right; } }

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
                default:
                    throw new NotSupportedException();
            }
            _right.WriteSql(sqlText);
            if (_joinType != DbExpressionKind.CrossJoin)
            {
                sqlText.Append(" ON ");
                _condition.WriteSql(sqlText);
            }
            base.WriteSql(sqlText);
        }

        internal override IEnumerable<ColumnExpression> GetProjectedColumns()
        {
            return GetProjectedColumns(this);
        }

        internal static IEnumerable<ColumnExpression> GetProjectedColumns(JoinExpression join)
        {
            IEnumerable<ColumnExpression> projectedColumns;
            if (join.Left is FromExpression)
                projectedColumns = ((FromExpression)join.Left).GetProjectedColumns();
            else
                projectedColumns = GetProjectedColumns((JoinExpression)join.Left);
            if (join.Right is FromExpression)
                projectedColumns = projectedColumns.Concat(((FromExpression)join.Right).GetProjectedColumns());
            else
                projectedColumns = projectedColumns.Concat(GetProjectedColumns((JoinExpression)join.Right));
            return projectedColumns;
        }

        internal override IEnumerable<PropertyExpression> GetAccessedProperties()
        {
            var accessedProperties = _left.GetAccessedProperties()
                .Concat
                (_right.GetAccessedProperties());
            if (_joinType != DbExpressionKind.CrossJoin)
                accessedProperties = accessedProperties.Concat(_condition.GetAccessedProperties());
            accessedProperties = accessedProperties.Concat(base.GetAccessedProperties());
            return accessedProperties;
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

        internal override IEnumerable<PropertyExpression> GetAccessedProperties()
        {
            return _where.GetAccessedProperties().Concat(base.GetAccessedProperties());
        }

        internal override IEnumerable<ColumnExpression> GetProjectedColumns()
        {
            return Enumerable.Empty<ColumnExpression>();
        }
    }

    internal class VariableReferenceExpression : VisitedExpression
    {
        private string _name;
        private IDictionary<string, string> _variableSubstitution;

        public VariableReferenceExpression(string name, IDictionary<string, string> variableSubstitution)
        {
            _name = name;
            _variableSubstitution = variableSubstitution;
        }

        internal VariableReferenceExpression(VariableReferenceExpression expression)
        {
            _name = expression._name;
            _variableSubstitution = expression._variableSubstitution;
        }

        internal override void WriteSql(StringBuilder sqlText)
        {
            if (_variableSubstitution.ContainsKey(_name))
                sqlText.Append(SqlBaseGenerator.QuoteIdentifier(_variableSubstitution[_name]));
            else
            {
                // TODO: come up with a better solution
                // need some way of removing extra levels of dots
                if (_name.Contains("."))
                {
                    sqlText.Append(SqlBaseGenerator.QuoteIdentifier(_name.Substring(_name.LastIndexOf('.') + 1)));
                }
                else
                {
                    sqlText.Append(SqlBaseGenerator.QuoteIdentifier(_name));
                }
            }
            base.WriteSql(sqlText);
        }

        // override ToString since we don't want variable substitution
        // until writing out the SQL.
        public override string ToString()
        {
            StringBuilder unsubstitutedText = new StringBuilder();
            unsubstitutedText.Append(_name);
            foreach (var expression in this.ExpressionList)
            {
                unsubstitutedText.Append(expression.ToString());
            }
            return unsubstitutedText.ToString();
        }

        internal void AdjustAccess(string projectName)
        {
            int projectIndex = _name.IndexOf(projectName);
            // start substring at the end of the projectName
            int substringAt = projectIndex + projectName.Length + 1;
            // skip over the ending quote if it exists
            if (_name[substringAt] == '"')
                ++substringAt;
            // skip over the connecting .
            if (_name[substringAt] == '.')
                ++substringAt;
            _name = _name.Substring(substringAt);
        }
    }

    internal class PropertyExpression : VisitedExpression
    {
        private VariableReferenceExpression _variable;
        private EdmMember _property;

        public PropertyExpression(VariableReferenceExpression variable, EdmMember property)
        {
            _variable = variable;
            _property = property;
        }

        public PropertyExpression(PropertyExpression expression)
        {
            _variable = new VariableReferenceExpression(expression._variable);
            _property = expression._property;
        }

        // used for inserts or updates where the column is not qualified
        public PropertyExpression(EdmMember property)
        {
            _variable = null;
            _property = property;
        }

        public string Name { get { return _property.Name; } }

        public TypeUsage PropertyType { get { return _property.TypeUsage; } }

        internal override void WriteSql(StringBuilder sqlText)
        {
            if (_variable != null)
            {
                _variable.WriteSql(sqlText);
                sqlText.Append(".");
            }
            sqlText.Append(SqlBaseGenerator.QuoteIdentifier(_property.Name));
            base.WriteSql(sqlText);
        }

        // override ToString since we don't want variable substitution or identifier quoting
        // until writing out the SQL.
        public override string ToString()
        {
            StringBuilder unsubstitutedText = new StringBuilder();
            if (_variable != null)
            {
                unsubstitutedText.Append(_variable.ToString());
                unsubstitutedText.Append(".");
            }
            unsubstitutedText.Append(_property.Name);
            return unsubstitutedText.ToString();
        }

        internal override IEnumerable<PropertyExpression> GetAccessedProperties()
        {
            return new PropertyExpression[] { this }.Concat(base.GetAccessedProperties());
        }

        internal void AdjustVariableAccess(string projectName)
        {
            if (_variable != null)
            {
                _variable.AdjustAccess(projectName);
            }
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

        internal override IEnumerable<PropertyExpression> GetAccessedProperties()
        {
            return _args
                .Aggregate(Enumerable.Empty<PropertyExpression>(),
                    (list, ve) => list.Concat(ve.GetAccessedProperties()))
                .Concat(base.GetAccessedProperties());
        }

        internal override IEnumerable<ColumnExpression> GetProjectedColumns()
        {
            return Enumerable.Empty<ColumnExpression>();
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

        internal override IEnumerable<PropertyExpression> GetAccessedProperties()
        {
            return _value.GetAccessedProperties().Concat(base.GetAccessedProperties());
        }

        internal override IEnumerable<ColumnExpression> GetProjectedColumns()
        {
            return Enumerable.Empty<ColumnExpression>();
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

        internal override IEnumerable<ColumnExpression> GetProjectedColumns()
        {
            return Enumerable.Empty<ColumnExpression>();
        }
    }

    internal class LimitExpression : VisitedExpression
    {
        private VisitedExpression _arg;

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

        internal override IEnumerable<PropertyExpression> GetAccessedProperties()
        {
            return _arg.GetAccessedProperties().Concat(base.GetAccessedProperties());
        }

        internal override IEnumerable<ColumnExpression> GetProjectedColumns()
        {
            return Enumerable.Empty<ColumnExpression>();
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

        internal override IEnumerable<PropertyExpression> GetAccessedProperties()
        {
            return _arg.GetAccessedProperties().Concat(base.GetAccessedProperties());
        }

        internal override IEnumerable<ColumnExpression> GetProjectedColumns()
        {
            return Enumerable.Empty<ColumnExpression>();
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
            bool wrapLeft = !(_left is PropertyExpression || _left is ConstantExpression);
            bool wrapRight = !(_right is PropertyExpression || _right is ConstantExpression);
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

        internal override IEnumerable<PropertyExpression> GetAccessedProperties()
        {
            return _left.GetAccessedProperties()
                .Concat
                (_right.GetAccessedProperties())
                .Concat
                (base.GetAccessedProperties());
        }

        internal override IEnumerable<ColumnExpression> GetProjectedColumns()
        {
            return Enumerable.Empty<ColumnExpression>();
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
            bool wrapLeft = !(_left is PropertyExpression || _left is ConstantExpression);
            bool wrapRight = !(_right is PropertyExpression || _right is ConstantExpression);
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

        internal override IEnumerable<PropertyExpression> GetAccessedProperties()
        {
            return _left.GetAccessedProperties()
                .Concat
                (_right.GetAccessedProperties())
                .Concat
                (base.GetAccessedProperties());
        }

        internal override IEnumerable<ColumnExpression> GetProjectedColumns()
        {
            return Enumerable.Empty<ColumnExpression>();
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

        internal override void WriteSql(StringBuilder sqlText)
        {
            _first.WriteSql(sqlText);
            sqlText.Append(" ");
            sqlText.Append(_setOperator);
            sqlText.Append(" ");
            _second.WriteSql(sqlText);
            base.WriteSql(sqlText);
        }

        internal override IEnumerable<PropertyExpression> GetAccessedProperties()
        {
            return _first.GetAccessedProperties()
                .Concat
                (_second.GetAccessedProperties())
                .Concat
                (base.GetAccessedProperties());
        }

        internal override IEnumerable<ColumnExpression> GetProjectedColumns()
        {
            return _first.GetProjectedColumns().Concat(_second.GetProjectedColumns());
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

        internal override IEnumerable<PropertyExpression> GetAccessedProperties()
        {
            return _argument.GetAccessedProperties().Concat(base.GetAccessedProperties());
        }

        internal override IEnumerable<ColumnExpression> GetProjectedColumns()
        {
            return Enumerable.Empty<ColumnExpression>();
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

        internal override IEnumerable<PropertyExpression> GetAccessedProperties()
        {
            return _argument.GetAccessedProperties().Concat(base.GetAccessedProperties());
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

        internal override IEnumerable<PropertyExpression> GetAccessedProperties()
        {
            return _argument.GetAccessedProperties().Concat(base.GetAccessedProperties());
        }

        internal override IEnumerable<ColumnExpression> GetProjectedColumns()
        {
            return Enumerable.Empty<ColumnExpression>();
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

        internal override IEnumerable<ColumnExpression> GetProjectedColumns()
        {
            return Enumerable.Empty<ColumnExpression>();
        }
    }

    internal class TruncateTimeExpression : VisitedExpression
    {
        readonly VisitedExpression _arg;
        readonly string _truncationType;
        public TruncateTimeExpression(string truncationType, VisitedExpression visitedExpression)
        {
            _arg = visitedExpression;
            _truncationType = truncationType;
        }


        internal override void WriteSql(StringBuilder sqlText)
        {
            sqlText.Append("date_trunc");
            sqlText.Append("(");
            sqlText.Append("'" + _truncationType + "',");
            _arg.WriteSql(sqlText);
            sqlText.Append(")");
            base.WriteSql(sqlText);
        }
    }
}
