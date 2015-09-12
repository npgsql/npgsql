// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq.Expressions;
using EntityFramework7.Npgsql.Query.Expressions;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Query.Expressions;
using Microsoft.Data.Entity.Query.ExpressionTranslators;

namespace EntityFramework7.Npgsql.Query.ExpressionTranslators
{
    public class DateTimeNowTranslator : IMemberTranslator
    {
        public virtual Expression Translate([NotNull] MemberExpression memberExpression)
        {
            if (memberExpression.Expression == null
                && memberExpression.Member.DeclaringType == typeof(DateTime))
            {
                if (memberExpression.Member.Name == nameof(DateTime.Now))
                {
                    return new SqlFunctionExpression("now", memberExpression.Type);
                }
                else if (memberExpression.Member.Name == nameof(DateTime.UtcNow))
                {
                    return new AtTimeZoneExpression(new SqlFunctionExpression("now", memberExpression.Type), "UTC");
                }
            }

            return null;
        }
    }
}
