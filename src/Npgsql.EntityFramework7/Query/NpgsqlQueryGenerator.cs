// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Relational.Query.Expressions;
using Microsoft.Data.Entity.Relational.Query.Sql;
using Microsoft.Data.Entity.Utilities;

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
            // No TOP in postgres
        }

        public override Expression VisitCountExpression(CountExpression countExpression)
        {
            Check.NotNull(countExpression, nameof(countExpression));

            if (countExpression.Type == typeof(long))
            {
                Sql.Append("COUNT_BIG(*)");
                return countExpression;
            }

            return base.VisitCountExpression(countExpression);
        }
    }
}
