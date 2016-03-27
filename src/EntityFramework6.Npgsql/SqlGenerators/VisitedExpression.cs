#region License
// The PostgreSQL License
//
// Copyright (C) 2016 The Npgsql Development Team
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
using System.Globalization;

namespace Npgsql.SqlGenerators
{
    internal abstract class VisitedExpression
    {
        protected VisitedExpression()
        {
            ExpressionList = new List<VisitedExpression>();
        }

        public VisitedExpression Append(VisitedExpression expression)
        {
            ExpressionList.Add(expression);
            return this;
        }

        public VisitedExpression Append(string literal)
        {
            ExpressionList.Add(new LiteralExpression(literal));
            return this;
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

        public new LiteralExpression Append(VisitedExpression expresion)
        {
            base.Append(expresion);
            return this;
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
            var ni = CultureInfo.InvariantCulture.NumberFormat;
            object value = _value;
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
                    if ((decimal)_value < 0)
                    {
                        sqlText.AppendFormat(ni, "({0})::numeric", _value);
                    }
                    else
                    {
                        sqlText.AppendFormat(ni, "{0}::numeric", _value);
                    }
                    break;
                case PrimitiveTypeKind.Double:
                    if (double.IsNaN((double)_value))
                    {
                        sqlText.AppendFormat("'NaN'::float8");
                    }
                    else if (double.IsPositiveInfinity((double)_value))
                    {
                        sqlText.AppendFormat("'Infinity'::float8");
                    }
                    else if (double.IsNegativeInfinity((double)_value))
                    {
                        sqlText.AppendFormat("'-Infinity'::float8");
                    }
                    else if ((double)_value < 0)
                    {
                        sqlText.AppendFormat(ni, "({0:r})::float8", _value);
                    }
                    else
                    {
                        sqlText.AppendFormat(ni, "{0:r}::float8", _value);
                    }
                    break;
                    // PostgreSQL has no support for bytes. int2 is used instead in Npgsql.
                case PrimitiveTypeKind.Byte:
                    value = (short)(byte)_value;
                    goto case PrimitiveTypeKind.Int16;
                case PrimitiveTypeKind.SByte:
                    value = (short)(sbyte)_value;
                    goto case PrimitiveTypeKind.Int16;
                case PrimitiveTypeKind.Int16:
                    if ((short)value < 0)
                    {
                        sqlText.AppendFormat(ni, "({0})::int2", _value);
                    }
                    else
                    {
                        sqlText.AppendFormat(ni, "{0}::int2", _value);
                    }
                    break;
                case PrimitiveTypeKind.Int32:
                    sqlText.AppendFormat(ni, "{0}", _value);
                    break;
                case PrimitiveTypeKind.Int64:
                    if ((long)_value < 0)
                    {
                        sqlText.AppendFormat(ni, "({0})::int8", _value);
                    }
                    else
                    {
                        sqlText.AppendFormat(ni, "{0}::int8", _value);
                    }
                    break;
                case PrimitiveTypeKind.Single:
                    if (float.IsNaN((float)_value))
                    {
                        sqlText.AppendFormat("'NaN'::float4");
                    }
                    else if (float.IsPositiveInfinity((float)_value))
                    {
                        sqlText.AppendFormat("'Infinity'::float4");
                    }
                    else if (float.IsNegativeInfinity((float)_value))
                    {
                        sqlText.AppendFormat("'-Infinity'::float4");
                    }
                    else if ((float)_value < 0)
                    {
                        sqlText.AppendFormat(ni, "({0:r})::float4", _value);
                    }
                    else
                    {
                        sqlText.AppendFormat(ni, "{0:r}::float4", _value);
                    }
                    break;
                case PrimitiveTypeKind.Boolean:
                    sqlText.Append(((bool)_value) ? "TRUE" : "FALSE");
                    break;
                case PrimitiveTypeKind.Guid:
                    sqlText.Append('\'').Append((Guid)_value).Append('\'');
                    sqlText.Append("::uuid");
                    break;
                case PrimitiveTypeKind.String:
                    sqlText.Append("E'").Append(((string)_value).Replace(@"\", @"\\").Replace("'", @"\'")).Append("'");
                    break;
                case PrimitiveTypeKind.Time:
                    sqlText.AppendFormat(ni, "INTERVAL '{0}'", (NpgsqlTimeSpan)(TimeSpan)_value);
                    break;
                default:
                    // TODO: must support more constant value types.
                    throw new NotSupportedException(string.Format("NotSupported: {0} {1}", _primitiveType, _value));
            }
            base.WriteSql(sqlText);
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
            if (!columns.Any())
                return;

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
            if (columns.Any())
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
            else
            {
                Append(" DEFAULT VALUES");
            }
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

            ColumnReferenceExpression column = _column as ColumnReferenceExpression;
            if (column == null || column.Name != _columnName)
            {
                sqlText.Append(" AS ");
                sqlText.Append(SqlBaseGenerator.QuoteIdentifier(_columnName));
            }

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

        public readonly Dictionary<StringPair, string> ColumnsToProject = new Dictionary<StringPair, string>(); // (from, name) -> newName
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
                        if (column.Key.Item2 != column.Value)
                        {
                            sqlText.Append(" AS ");
                            sqlText.Append(SqlBaseGenerator.QuoteIdentifier(column.Value));
                        }
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
            // useNewPrecedence doesn't matter here since there was no change with the AND operator
            _where = OperatorExpression.Build(Operator.And, true, _where, andAlso);
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

        internal FunctionExpression AddArgument(VisitedExpression visitedExpression)
        {
            _args.Add(visitedExpression);
            return this;
        }

        internal FunctionExpression AddArgument(string argument)
        {
            _args.Add(new LiteralExpression(argument));
            return this;
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

    internal class Operator
    {
        private string op;
        private int leftPrecedence;
        private int rightPrecedence;
        private int newPrecedence; // Since PostgreSQL 9.5, the operator precedence was changed
        private UnaryTypes unaryType;
        private bool rightAssoc;

        public string Op { get { return op; } }
        public int LeftPrecedence { get { return leftPrecedence; } }
        public int RightPrecedence { get { return rightPrecedence; } }
        public int NewPrecedence { get { return newPrecedence; } }
        public UnaryTypes UnaryType { get { return unaryType; } }
        public bool RightAssoc { get { return rightAssoc; } }

        internal enum UnaryTypes {
            Binary,
            Prefix,
            Postfix
        }

        private Operator(string op, int precedence, int newPrecedence)
        {
            this.op = ' ' + op + ' ';
            this.leftPrecedence = precedence;
            this.rightPrecedence = precedence;
            this.newPrecedence = newPrecedence;
            this.unaryType = UnaryTypes.Binary;
        }

        private Operator(string op, int leftPrecedence, int rightPrecedence, int newPrecedence)
        {
            this.op = ' ' + op + ' ';
            this.leftPrecedence = leftPrecedence;
            this.rightPrecedence = rightPrecedence;
            this.newPrecedence = newPrecedence;
            this.unaryType = UnaryTypes.Binary;
        }

        private Operator(string op, int precedence, int newPrecedence, UnaryTypes unaryType, bool rightAssoc)
        {
            this.op = unaryType == UnaryTypes.Binary ? ' ' + op + ' ' : unaryType == UnaryTypes.Prefix ? op + ' ' : ' ' + op;
            this.leftPrecedence = precedence;
            this.rightPrecedence = precedence;
            this.newPrecedence = newPrecedence;
            this.unaryType = unaryType;
            this.rightAssoc = rightAssoc;
        }

        /*
         * Operator table
         * Corresponds to the operator precedence table at
         * http://www.postgresql.org/docs/current/interactive/sql-syntax-lexical.html
         *
         * Note that in versions up to 9.4, NOT IN and NOT LIKE have different precedences depending on
         * if the other operator is to the left or to the right.
         * For example, "a = b NOT LIKE c" is parsed as "(a = b) NOT LIKE c"
         * but "a NOT LIKE b = c" is parsed as "(a NOT LIKE b) = c"
         * This is because PostgreSQL's parser uses Bison's automatic
         * operator precedence handling, and NOT and LIKE has different precedences,
         * so this happens when the two keywords are put together like this.
         *
         */
        public static readonly Operator UnaryMinus = new Operator("-", 17, 12, UnaryTypes.Prefix, true);
        public static readonly Operator Mul = new Operator("*", 15, 10);
        public static readonly Operator Div = new Operator("/", 15, 10);
        public static readonly Operator Mod = new Operator("%", 15, 10);
        public static readonly Operator Add = new Operator("+", 14, 9);
        public static readonly Operator Sub = new Operator("-", 14, 9);
        public static readonly Operator IsNull = new Operator("IS NULL", 13, 4, UnaryTypes.Postfix, false);
        public static readonly Operator IsNotNull = new Operator("IS NOT NULL", 13, 4, UnaryTypes.Postfix, false);
        public static readonly Operator LessThanOrEquals = new Operator("<=", 10, 5);
        public static readonly Operator GreaterThanOrEquals = new Operator(">=", 10, 5);
        public static readonly Operator NotEquals = new Operator("!=", 10, 5);
        public static readonly Operator BitwiseAnd = new Operator("&", 10, 8);
        public static readonly Operator BitwiseOr = new Operator("|", 10, 8);
        public static readonly Operator BitwiseXor = new Operator("#", 10, 8);
        public static readonly Operator BitwiseNot = new Operator("~", 10, 8, UnaryTypes.Prefix, false);
        public static readonly Operator Concat = new Operator("||", 10, 8);
        public static readonly Operator In = new Operator("IN", 9, 6);
        public static readonly Operator NotIn = new Operator("NOT IN", 3, 9, 6);
        public static readonly Operator Like = new Operator("LIKE", 6, 6);
        public static readonly Operator NotLike = new Operator("NOT LIKE", 3, 6, 6);
        public static readonly Operator LessThan = new Operator("<", 5, 5);
        public static readonly Operator GreaterThan = new Operator(">", 5, 5);
        public static readonly new Operator Equals = new Operator("=", 4, 5, UnaryTypes.Binary, true);
        public static readonly Operator Not = new Operator("NOT", 3, 3, UnaryTypes.Prefix, true);
        public static readonly Operator And = new Operator("AND", 2, 2);
        public static readonly Operator Or = new Operator("OR", 1, 1);

        public static readonly Operator QueryMatch = new Operator("@@", 10, 8);
        public static readonly Operator QueryAnd = new Operator("&&", 10, 8);
        public static readonly Operator QueryOr = Concat;
        public static readonly Operator QueryNegate = new Operator("!!", 10, 8, UnaryTypes.Prefix, true);
        public static readonly Operator QueryContains = new Operator("@>", 10, 8);
        public static readonly Operator QueryIsContained = new Operator("<@", 10, 8);

        public static readonly Dictionary<Operator, Operator> NegateDict;

        static Operator()
        {
            NegateDict = new Dictionary<Operator, Operator>()
            {
                {IsNull, IsNotNull},
                {IsNotNull, IsNull},
                {LessThanOrEquals, GreaterThan},
                {GreaterThanOrEquals, LessThan},
                {NotEquals, Equals},
                {In, NotIn},
                {NotIn, In},
                {Like, NotLike},
                {NotLike, Like},
                {LessThan, GreaterThanOrEquals},
                {GreaterThan, LessThanOrEquals},
                {Equals, NotEquals}
            };
        }
    }

    internal class OperatorExpression : VisitedExpression
    {
        private Operator op;
        private bool useNewPrecedences;
        private VisitedExpression left;
        private VisitedExpression right;

        private OperatorExpression(Operator op, bool useNewPrecedences, VisitedExpression left, VisitedExpression right)
        {
            this.op = op;
            this.useNewPrecedences = useNewPrecedences;
            this.left = left;
            this.right = right;
        }

        public static OperatorExpression Build(Operator op, bool useNewPrecedences, VisitedExpression left, VisitedExpression right)
        {
            if (op.UnaryType == Operator.UnaryTypes.Binary)
            {
                return new OperatorExpression(op, useNewPrecedences, left, right);
            }
            else
            {
                throw new InvalidOperationException("Unary operator with two operands");
            }
        }

        public static OperatorExpression Build(Operator op, bool useNewPrecedences, VisitedExpression exp)
        {
            if (op.UnaryType == Operator.UnaryTypes.Prefix)
            {
                return new OperatorExpression(op, useNewPrecedences, null, exp);
            }
            else if (op.UnaryType == Operator.UnaryTypes.Postfix)
            {
                return new OperatorExpression(op, useNewPrecedences, exp, null);
            }
            else
            {
                throw new InvalidOperationException("Binary operator with one operand");
            }
        }

        /// <summary>
        /// Negates an expression.
        /// If possible, replaces the operator of exp if exp is a negatable OperatorExpression,
        /// else return a new OperatorExpression of type Not that wraps exp.
        /// </summary>
        public static VisitedExpression Negate(VisitedExpression exp, bool useNewPrecedences)
        {
            OperatorExpression expOp = exp as OperatorExpression;
            if (expOp != null)
            {
                Operator op = expOp.op;
                Operator newOp = null;
                if (Operator.NegateDict.TryGetValue(op, out newOp))
                {
                    expOp.op = newOp;
                    return expOp;
                }
                if (expOp.op == Operator.Not)
                {
                    return expOp.right;
                }
            }

            return OperatorExpression.Build(Operator.Not, useNewPrecedences, exp);
        }

        internal override void WriteSql(StringBuilder sqlText)
        {
            WriteSql(sqlText, null);
        }

        private void WriteSql(StringBuilder sqlText, OperatorExpression rightParent)
        {
            OperatorExpression leftOp = left as OperatorExpression;
            OperatorExpression rightOp = right as OperatorExpression;

            bool wrapLeft, wrapRight;

            if (!useNewPrecedences)
            {
                wrapLeft = leftOp != null && (op.RightAssoc ? leftOp.op.RightPrecedence <= op.LeftPrecedence : leftOp.op.RightPrecedence < op.LeftPrecedence);
                wrapRight = rightOp != null && (!op.RightAssoc ? rightOp.op.LeftPrecedence <= op.RightPrecedence : rightOp.op.LeftPrecedence < op.RightPrecedence);
            }
            else
            {
                wrapLeft = leftOp != null && (op.RightAssoc ? leftOp.op.NewPrecedence <= op.NewPrecedence : leftOp.op.NewPrecedence < op.NewPrecedence);
                wrapRight = rightOp != null && (!op.RightAssoc ? rightOp.op.NewPrecedence <= op.NewPrecedence : rightOp.op.NewPrecedence < op.NewPrecedence);
            }

            // Avoid parentheses for prefix operators if possible,
            // e.g. BitwiseNot: (a & (~ b)) & c is written as a & ~ b & c
            // but (a + (~ b)) + c must be written as a + (~ b) + c
            if (!useNewPrecedences)
            {
                if (wrapRight && rightOp.left == null && (rightParent == null || (!rightParent.op.RightAssoc ? rightOp.op.RightPrecedence >= rightParent.op.LeftPrecedence : rightOp.op.RightPrecedence > rightParent.op.LeftPrecedence)))
                    wrapRight = false;
            }
            else
            {
                if (wrapRight && rightOp.left == null && (rightParent == null || (!rightParent.op.RightAssoc ? rightOp.op.NewPrecedence >= rightParent.op.NewPrecedence : rightOp.op.NewPrecedence > rightParent.op.NewPrecedence)))
                    wrapRight = false;
            }

            if (left != null)
            {
                if (wrapLeft)
                    sqlText.Append("(");
                if (leftOp != null && !wrapLeft)
                    leftOp.WriteSql(sqlText, this);
                else
                    left.WriteSql(sqlText);
                if (wrapLeft)
                    sqlText.Append(")");
            }

            sqlText.Append(op.Op);

            if (right != null)
            {
                if (wrapRight)
                    sqlText.Append("(");
                if (rightOp != null && !wrapRight)
                    rightOp.WriteSql(sqlText, rightParent);
                else
                    right.WriteSql(sqlText);
                if (wrapRight)
                    sqlText.Append(")");
            }

            base.WriteSql(sqlText);
        }
    }

    internal class ConstantListExpression : VisitedExpression
    {
        private IEnumerable<ConstantExpression> _list;

        public ConstantListExpression(IEnumerable<ConstantExpression> list)
        {
            _list = list;
        }

        internal override void WriteSql(StringBuilder sqlText)
        {
            sqlText.Append("(");
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
        private List<VisitedExpression> _list;
        private string _setOperator;

        public CombinedProjectionExpression(DbExpressionKind setOperator, List<VisitedExpression> list)
        {
            _setOperator = setOperator == DbExpressionKind.UnionAll ? "UNION ALL" : setOperator == DbExpressionKind.Except ? "EXCEPT" : "INTERSECT";
            _list = list;
        }

        internal override void WriteSql(StringBuilder sqlText)
        {
            for (var i = 0; i < _list.Count; i++)
            {
                if (i != 0)
                {
                    sqlText.Append(' ').Append(_setOperator).Append(' ');
                }
                sqlText.Append('(');
                _list[i].WriteSql(sqlText);
                sqlText.Append(')');
            }

            base.WriteSql(sqlText);
        }
    }

    internal class ExistsExpression : VisitedExpression
    {
        private VisitedExpression _argument;

        public ExistsExpression(VisitedExpression argument)
        {
            _argument = argument;
        }

        internal override void WriteSql(StringBuilder sqlText)
        {
            sqlText.Append("EXISTS (");
            _argument.WriteSql(sqlText);
            sqlText.Append(")");
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
