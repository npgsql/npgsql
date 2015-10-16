// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Query.Expressions;
using Microsoft.Data.Entity.Query.Expressions.Internal;
using Microsoft.Data.Entity.Storage;
using Microsoft.Data.Entity.Utilities;

// ReSharper disable once CheckNamespace
namespace Microsoft.Data.Entity.Query.Sql.Internal
{
    public class NpgsqlQuerySqlGenerator : DefaultQuerySqlGenerator
    {
        protected override string ConcatOperator => "||";
        protected override string TrueLiteral => "TRUE";
        protected override string FalseLiteral => "FALSE";
        protected override string TypedTrueLiteral => "TRUE::bool";
        protected override string TypedFalseLiteral => "FALSE::bool";

        public NpgsqlQuerySqlGenerator(
            [NotNull] IRelationalCommandBuilderFactory commandBuilderFactory,
            [NotNull] ISqlGenerator sqlGenerator,
            [NotNull] IParameterNameGeneratorFactory parameterNameGeneratorFactory,
            [NotNull] SelectExpression selectExpression)
            : base(commandBuilderFactory, sqlGenerator, parameterNameGeneratorFactory, selectExpression)
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
                Sql.AppendLine().Append("LIMIT ").Append(selectExpression.Limit);
            }

            if (selectExpression.Offset != null)
            {
                if (selectExpression.Limit == null) {
                    Sql.AppendLine();
                } else {
                    Sql.Append(' ');
                }
                Sql.Append("OFFSET ").Append(selectExpression.Offset);
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
            if (sumExpression.Type == typeof(short))
            {
                Sql.Append("::INT2");
            }
            else if (sumExpression.Type == typeof (int))
            {
                Sql.Append("::INT4");
            }
            else if (sumExpression.Type == typeof(long))
            {
                Sql.Append("::INT8");
            }

            return sumExpression;
        }

        protected override Expression VisitBinary(BinaryExpression binaryExpression)
        {
            // PostgreSQL 9.4 and below has some weird operator precedence fixed in 9.5 and described here:
            // http://git.postgresql.org/gitweb/?p=postgresql.git&a=commitdiff&h=c6b3c939b7e0f1d35f4ed4996e71420a993810d2
            // As a result we must surround string concatenation with parentheses
            if (binaryExpression.NodeType == ExpressionType.Add &&
                binaryExpression.Left.Type == typeof (string) &&
                binaryExpression.Right.Type == typeof (string))
            {
                Sql.Append("(");
                var exp = base.VisitBinary(binaryExpression);
                Sql.Append(")");
                return exp;
            }

            return base.VisitBinary(binaryExpression);
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

        public Expression VisitAtTimeZone([NotNull] AtTimeZoneExpression atTimeZoneExpression)
        {
            Check.NotNull(atTimeZoneExpression, nameof(atTimeZoneExpression));

            Visit(atTimeZoneExpression.TimestampExpression);

            Sql.Append(" AT TIME ZONE '");
            Sql.Append(atTimeZoneExpression.TimeZone);
            Sql.Append('\'');
            return atTimeZoneExpression;
        }
    }
}
