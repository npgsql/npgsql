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
        protected override string DelimitIdentifier(string identifier)
        {
            return "\"" + identifier.Replace("\"", "\"\"") + "\"";
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
    }
}
