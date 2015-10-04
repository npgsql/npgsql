// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Query.Expressions;
using Microsoft.Data.Entity.Query.Expressions.Internal;

// ReSharper disable once CheckNamespace
namespace Microsoft.Data.Entity.Query.ExpressionTranslators.Internal
{
    public class NpgsqlDateTimeNowTranslator : IMemberTranslator
    {
        public virtual Expression Translate([NotNull] MemberExpression memberExpression)
        {
            if (memberExpression.Expression == null
                && memberExpression.Member.DeclaringType == typeof(DateTime))
            {
                if (memberExpression.Member.Name == nameof(DateTime.Now))
                {
                    return new SqlFunctionExpression("NOW", memberExpression.Type);
                }
                else if (memberExpression.Member.Name == nameof(DateTime.UtcNow))
                {
                    return new AtTimeZoneExpression(new SqlFunctionExpression("NOW", memberExpression.Type), "UTC");
                }
            }

            return null;
        }
    }
}
