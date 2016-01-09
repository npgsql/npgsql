// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;

namespace EntityFramework7.Npgsql.FunctionalTests
{
    public class QueryNoClientEvalNpgsqlFixture : NorthwindQueryNpgsqlFixture
    {
        protected override void ConfigureOptions(NpgsqlDbContextOptionsBuilder npgsqlDbContextOptionsBuilder)
            => npgsqlDbContextOptionsBuilder.QueryClientEvaluationBehavior(QueryClientEvaluationBehavior.Throw);
    }
}
