// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using EntityFramework7.Npgsql.Query.Expressions;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Query.Methods;

namespace EntityFramework7.Npgsql.Query.Methods
{
    /// <summary>
    /// Translates Regex.IsMatch calls into PostgreSQL regex expressions for database-side processing.
    /// </summary>
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/functions-matching.html
    /// </remarks>
    public class RegexIsMatchTranslator : IMethodCallTranslator
    {
        static readonly MethodInfo IsMatch;
        static readonly MethodInfo IsMatchWithRegexOptions;

        const RegexOptions UnsupportedRegexOptions = RegexOptions.RightToLeft | RegexOptions.ECMAScript;

        static RegexIsMatchTranslator()
        {
            IsMatch = typeof (Regex).GetTypeInfo().GetDeclaredMethods("IsMatch").Single(m =>
                m.GetParameters().Count() == 2 &&
                m.GetParameters().All(p => p.ParameterType == typeof(string))
            );
            IsMatchWithRegexOptions = typeof(Regex).GetTypeInfo().GetDeclaredMethods("IsMatch").Single(m =>
               m.GetParameters().Count() == 3 &&
               m.GetParameters().Take(2).All(p => p.ParameterType == typeof(string)) &&
               m.GetParameters()[2].ParameterType == typeof(RegexOptions)
            );
        }

        public Expression Translate([NotNull] MethodCallExpression methodCallExpression)
        {
            // Regex.IsMatch(string, string)
            if (methodCallExpression.Method == IsMatch)
            {
                return new RegexMatchExpression(
                    methodCallExpression.Arguments[0],
                    methodCallExpression.Arguments[1],
                    RegexOptions.None
                );
            }

            // Regex.IsMatch(string, string, RegexOptions)
            if (methodCallExpression.Method == IsMatchWithRegexOptions)
            {
                var constantExpr = methodCallExpression.Arguments[2] as ConstantExpression;

                if (constantExpr == null)
                {
                    return null;
                }

                var options = (RegexOptions) constantExpr.Value;

                if ((options & UnsupportedRegexOptions) != 0)
                {
                    return null;
                }

                return new RegexMatchExpression(
                    methodCallExpression.Arguments[0],
                    methodCallExpression.Arguments[1],
                    options
                );
            }

            return null;
        }
    }
}
