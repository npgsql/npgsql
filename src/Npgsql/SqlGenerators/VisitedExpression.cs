#if ENTITIES
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Metadata.Edm;
using System.Data.Common.CommandTrees;

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
            switch (_primitiveType)
            {
                case PrimitiveTypeKind.Int16:
                case PrimitiveTypeKind.Int32:
                case PrimitiveTypeKind.Int64:
                case PrimitiveTypeKind.Decimal:
                    sqlText.Append(_value);
                    break;
                case PrimitiveTypeKind.String:
                    sqlText.Append("'" + _value + "'");
                    break;
                case PrimitiveTypeKind.Boolean:
                    sqlText.Append(_value.ToString().ToUpperInvariant());
                    break;
                case PrimitiveTypeKind.DateTime:
                    sqlText.AppendFormat("'{0:s}'", _value);
                    break;
            }
            base.WriteSql(sqlText);
        }
    }

    internal class UnionAllExpression : VisitedExpression
    {
        internal override void WriteSql(StringBuilder sqlText)
        {
            sqlText.Append(" UNION ALL ");
            base.WriteSql(sqlText);
        }
    }

    internal class ProjectionExpression : VisitedExpression
    {
        public bool Distinct { get; set; }

        internal override void WriteSql(StringBuilder sqlText)
        {
            sqlText.Append("SELECT ");
            if (Distinct)
                sqlText.Append("DISTINCT ");
            base.WriteSql(sqlText);
        }
    }

    internal class InsertExpression : VisitedExpression
    {
        internal override void WriteSql(StringBuilder sqlText)
        {
            sqlText.Append("INSERT INTO ");
            base.WriteSql(sqlText);
        }
    }

    internal class UpdateExpression : VisitedExpression
    {
        internal override void WriteSql(StringBuilder sqlText)
        {
            sqlText.Append("UPDATE ");
            base.WriteSql(sqlText);
        }
    }

    internal class DeleteExpression : VisitedExpression
    {
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

        public ColumnExpression(VisitedExpression column, string columnName)
        {
            _column = column;
            _columnName = columnName;
        }

        internal override void WriteSql(StringBuilder sqlText)
        {
            _column.WriteSql(sqlText);
            sqlText.Append(" AS " + _columnName);
            base.WriteSql(sqlText);
        }
    }

    internal class FromExpression : VisitedExpression
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
            bool wrap = !(_from is LiteralExpression);
            if (wrap)
                sqlText.Append("(");
            _from.WriteSql(sqlText);
            if (wrap)
                sqlText.Append(")");
            sqlText.Append(" AS ");
            sqlText.Append(_name);
            base.WriteSql(sqlText);
        }
    }

    internal class JoinExpression : VisitedExpression
    {
        private VisitedExpression _left;
        private DbExpressionKind _joinType;
        private VisitedExpression _right;
        private VisitedExpression _condition;

        public JoinExpression(VisitedExpression left, DbExpressionKind joinType, VisitedExpression right, VisitedExpression condition)
        {
            _left = left;
            _joinType = joinType;
            _right = right;
            _condition = condition;
        }

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
                default:
                    throw new NotSupportedException();
            }
            _right.WriteSql(sqlText);
            sqlText.Append(" ON ");
            _condition.WriteSql(sqlText);
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

        internal override void WriteSql(StringBuilder sqlText)
        {
            if (_variableSubstitution.ContainsKey(_name))
                sqlText.Append(_variableSubstitution[_name]);
            else
            {
                // TODO: come up with a better solution
                // need some way of removing extra levels of dots
                if (_name.Contains("."))
                {
                    sqlText.Append(_name.Substring(_name.LastIndexOf('.') + 1));
                }
                else
                {
                    sqlText.Append(_name);
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
        internal override void WriteSql(StringBuilder sqlText)
        {
            if (ExpressionList.Count != 0)
                sqlText.Append(" GROUP BY ");
            base.WriteSql(sqlText);
        }
    }

    internal class AndExpression : VisitedExpression
    {
        private VisitedExpression _left;
        private VisitedExpression _right;

        public AndExpression(VisitedExpression left, VisitedExpression right)
        {
            _left = left;
            _right = right;
        }

        internal override void WriteSql(StringBuilder sqlText)
        {
            sqlText.Append("(");
            _left.WriteSql(sqlText);
            sqlText.Append(") AND (");
            _right.WriteSql(sqlText);
            sqlText.Append(")");
            base.WriteSql(sqlText);
        }
    }
}
#endif