// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Relational.Query.Expressions;
using Microsoft.Data.Entity.Relational.Query.Sql;
using Microsoft.Data.Entity.Utilities;
using System.Linq;

namespace Npgsql.EntityFramework7.Query
{
    public class NpgsqlQueryGenerator : DefaultSqlQueryGenerator
    {
        protected override string ConcatOperator => "||";

        protected override string TrueLiteral => "TRUE";

        protected override string FalseLiteral => "FALSE";

        protected override string DelimitIdentifier(string identifier)
        {
            return "\"" + identifier.Replace("\"", "\"\"") + "\"";
        }

        public NpgsqlQueryGenerator([NotNull] SelectExpression selectExpression)
            : base(Check.NotNull(selectExpression, nameof(selectExpression)))
        {
        }

        protected override void GenerateTop([NotNull]SelectExpression selectExpression)
        {
            // No TOP() in PostgreSQL, see GenerateLimitOffset
        }

        protected override void GenerateLimitOffset([NotNull] SelectExpression selectExpression)
        {
            Check.NotNull(selectExpression, nameof(selectExpression));

            if (selectExpression.Limit != null)
            {
                Sql.Append(" LIMIT ").Append(selectExpression.Limit);
            }

            if (selectExpression.Offset != null)
            {
                if (!selectExpression.OrderBy.Any())
                {
                    throw new InvalidOperationException(Microsoft.Data.Entity.Relational.Strings.SkipNeedsOrderBy);
                }

                Sql.Append(" OFFSET ").Append(selectExpression.Offset);
            }
        }

        public override Expression VisitCountExpression(CountExpression countExpression)
        {
            Check.NotNull(countExpression, nameof(countExpression));

            // Note that PostgreSQL COUNT(*) is BIGINT (64-bit). For 32-bit Count() expressions we cast.
            if (countExpression.Type == typeof(long))
            {
                Sql.Append("COUNT(*)");
            }
            else if (countExpression.Type == typeof(int))
            {
                Sql.Append("COUNT(*)::INT4");
            }
            else throw new NotSupportedException(string.Format("Count expression with type {0} not supported", countExpression.Type));

            return countExpression;
        }

        public override Expression VisitSumExpression(SumExpression sumExpression)
        {
            base.VisitSumExpression(sumExpression);

            // In PostgreSQL SUM() doesn't return the same type as its argument for smallint, int and bigint.
            // Cast to get the same type.
            // http://www.postgresql.org/docs/current/static/functions-aggregate.html
            switch (Type.GetTypeCode(sumExpression.Expression.Type))
            {
                case TypeCode.Int16:
                    Sql.Append("::INT2");
                    break;
                case TypeCode.Int32:
                    Sql.Append("::INT4");
                    break;
                case TypeCode.Int64:
                    Sql.Append("::INT8");
                    break;
            }

            return sumExpression;
        }
    }
}
