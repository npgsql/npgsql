// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Query.Expressions;
using Microsoft.Data.Entity.Query.Methods;

namespace EntityFramework7.Npgsql.Query.Methods
{
    public class MathTruncateTranslator : IMethodCallTranslator
    {
        public virtual Expression Translate([NotNull] MethodCallExpression methodCallExpression)
        {
            var methodInfos = typeof(Math).GetTypeInfo().GetDeclaredMethods("Truncate");
            if (methodInfos.Contains(methodCallExpression.Method))
            {
                return new SqlFunctionExpression("TRUNC", new[] { methodCallExpression.Arguments[0] }, methodCallExpression.Type);
            }

            return null;
        }
    }
}
