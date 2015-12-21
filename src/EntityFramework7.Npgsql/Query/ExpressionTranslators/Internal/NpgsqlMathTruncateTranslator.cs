// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Query.Expressions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Data.Entity.Query.ExpressionTranslators.Internal
{
    public class NpgsqlMathTruncateTranslator : IMethodCallTranslator
    {
        private static readonly IEnumerable<MethodInfo> _methodInfos = typeof(Math).GetTypeInfo().GetDeclaredMethods(nameof(Math.Truncate));

        public virtual Expression Translate([NotNull] MethodCallExpression methodCallExpression)
        {
            if (_methodInfos.Contains(methodCallExpression.Method))
            {
                return new SqlFunctionExpression("TRUNC", methodCallExpression.Type, new[] { methodCallExpression.Arguments[0] });
            }

            return null;
        }
    }
}
