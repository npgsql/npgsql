// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Query.Expressions;
using Microsoft.Data.Entity.Query.Sql;
using Microsoft.Data.Entity.Storage;
using Microsoft.Data.Entity.Utilities;
using EntityFramework7.Npgsql.Query.Expressions;

namespace EntityFramework7.Npgsql.Query
{
    public class NpgsqlQuerySqlGenerator : DefaultQuerySqlGenerator
    {
        protected override string ConcatOperator => "||";
        protected override string TrueLiteral => "TRUE";
        protected override string FalseLiteral => "FALSE";
        protected override string TypedTrueLiteral => "TRUE::bool";
        protected override string TypedFalseLiteral => "FALSE::bool";

        protected override string DelimitIdentifier(string identifier)
        {
            return "\"" + identifier.Replace("\"", "\"\"") + "\"";
        }

        public NpgsqlQuerySqlGenerator(
            [NotNull] SelectExpression selectExpression,
            [NotNull] IRelationalTypeMapper typeMapper)
            : base(selectExpression, typeMapper)
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
                Sql.Append(" OFFSET ").Append(selectExpression.Offset);
            }
        }

        public override Expression VisitCount(CountExpression countExpression)
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
            else throw new NotSupportedException($"Count expression with type {countExpression.Type} not supported");

            return countExpression;
        }

        public override Expression VisitSum(SumExpression sumExpression)
        {
            base.VisitSum(sumExpression);

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

        // See http://www.postgresql.org/docs/current/static/functions-matching.html
        public Expression VisitRegexMatch([NotNull] RegexMatchExpression regexMatchExpression)
        {
            Check.NotNull(regexMatchExpression, nameof(regexMatchExpression));
            var options = regexMatchExpression.Options;

            Visit(regexMatchExpression.Match);
            Sql.Append(" ~ ");

            // PG regexps are singleline by default
            if (options == RegexOptions.Singleline)
            {
                Visit(regexMatchExpression.Pattern);
                return regexMatchExpression;
            }

            Sql.Append("('(?");
            if (options.HasFlag(RegexOptions.IgnoreCase)) {
                Sql.Append('i');
            }

            if (options.HasFlag(RegexOptions.Multiline)) {
                Sql.Append('n');
            }
            else if (!options.HasFlag(RegexOptions.Singleline)) {
                // In .NET's default mode, . doesn't match newlines but PostgreSQL it does.
                Sql.Append('p');
            }

            if (options.HasFlag(RegexOptions.IgnorePatternWhitespace))
            {
                Sql.Append('x');
            }

            Sql.Append(")' || ");
            Visit(regexMatchExpression.Pattern);
            Sql.Append(')');
            return regexMatchExpression;
        }
    }
}
