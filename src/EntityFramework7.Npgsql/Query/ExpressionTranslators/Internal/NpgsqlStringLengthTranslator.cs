// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Query.Expressions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Data.Entity.Query.ExpressionTranslators.Internal
{
    public class NpgsqlStringLengthTranslator : IMemberTranslator
    {
        public virtual Expression Translate([NotNull] MemberExpression memberExpression)
        {
            if (memberExpression.Expression != null
                && memberExpression.Expression.Type == typeof(string)
                && memberExpression.Member.Name == "Length")
            {
                return new SqlFunctionExpression("LENGTH", memberExpression.Type, new[] { memberExpression.Expression });
            }

            return null;
        }
    }
}
