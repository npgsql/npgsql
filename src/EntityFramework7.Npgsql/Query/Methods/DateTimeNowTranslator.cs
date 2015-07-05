// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Query.Expressions;
using Microsoft.Data.Entity.Query.Methods;

namespace EntityFramework7.Npgsql.Query.Methods
{
    public class DateTimeNowTranslator : IMemberTranslator
    {
        public virtual Expression Translate([NotNull] MemberExpression memberExpression)
        {
            if (memberExpression.Expression == null
                && memberExpression.Member.DeclaringType == typeof(DateTime)
                && memberExpression.Member.Name == "Now")
            {
                // TODO: Is this safe from a timezone perspective?
                // TODO: What about DateTime.UtcNow?
                return new SqlFunctionExpression("now", Enumerable.Empty<Expression>(), memberExpression.Type);
            }

            return null;
        }
    }
}
