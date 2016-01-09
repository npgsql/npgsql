// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Data.Entity.FunctionalTests;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFramework7.Npgsql.FunctionalTests
{
    public class NpgsqlFixture
    {
        public readonly IServiceProvider ServiceProvider;

        public NpgsqlFixture()
        {
            ServiceProvider = new ServiceCollection()
                .AddEntityFramework()
                .AddNpgsql()
                .ServiceCollection()
                .BuildServiceProvider();
        }
    }
}
