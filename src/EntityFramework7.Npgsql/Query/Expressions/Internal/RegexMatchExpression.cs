// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Query.Sql;
using Microsoft.Data.Entity.Query.Sql.Internal;
using Microsoft.Data.Entity.Utilities;

// ReSharper disable once CheckNamespace
namespace Microsoft.Data.Entity.Query.Expressions.Internal
{
    public class RegexMatchExpression : Expression
    {
        public RegexMatchExpression([NotNull] Expression match, [NotNull] Expression pattern, RegexOptions options)
        {
            Check.NotNull(match, nameof(match));
            Check.NotNull(pattern, nameof(pattern));

            Match = match;
            Pattern = pattern;
            Options = options;
        }

        public Expression Match { get; }
        public Expression Pattern { get; }
        public RegexOptions Options { get; }

        public override ExpressionType NodeType => ExpressionType.Extension;

        public override Type Type => typeof(bool);

        protected override Expression Accept([NotNull] ExpressionVisitor visitor)
        {
            Check.NotNull(visitor, nameof(visitor));

            var specificVisitor = visitor as NpgsqlQuerySqlGenerator;

            return specificVisitor != null
                ? specificVisitor.VisitRegexMatch(this)
                : base.Accept(visitor);
        }

        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            var newMatchExpression = visitor.Visit(Match);
            var newPatternExpression = visitor.Visit(Pattern);

            return newMatchExpression != Match
                   || newPatternExpression != Pattern
                ? new RegexMatchExpression(newMatchExpression, newPatternExpression, Options)
                : this;
        }

        public override string ToString() => $"{Match} ~ {Pattern}";
    }
}
