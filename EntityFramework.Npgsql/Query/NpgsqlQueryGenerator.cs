using System;
using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Relational.Query.Expressions;
using Microsoft.Data.Entity.Relational.Query.Sql;
using Microsoft.Data.Entity.Utilities;

namespace EntityFramework.Npgsql.Extensions
{
    public class NpgsqlQueryGenerator : DefaultSqlQueryGenerator
    {
        protected override string ConcatOperator
        {
            get
            {
                return "||";
            }
        }

        protected override void GenerateTop([NotNull]SelectExpression selectExpression)
        {
            // No TOP in postgres
        }

        public override Expression VisitSelectExpression(SelectExpression selectExpression)
        {
            var expression = base.VisitSelectExpression(selectExpression);

            // add LIMIT if requested
            if ( selectExpression.Limit != null && selectExpression.Offset == null )
            {
                Sql.Append(" LIMIT ")
                    .Append(selectExpression.Limit)
                    .Append(" ");
            }
			
            return expression;
        }

        protected override void GenerateLimitOffset([NotNull]SelectExpression selectExpression)
        {
            Check.NotNull(selectExpression, "selectExpression");

            if ( selectExpression.Offset != null )
            {
                if ( !selectExpression.OrderBy.Any() )
                {
                    throw new InvalidOperationException(Strings.SkipNeedsOrderBy);
                }

                if ( selectExpression.Limit != null )
                {
                    Sql.Append(" LIMIT ")
                        .Append(selectExpression.Limit);
                }

                Sql.Append(" OFFSET ")
                    .Append(selectExpression.Offset);

            }
        }
    }
}