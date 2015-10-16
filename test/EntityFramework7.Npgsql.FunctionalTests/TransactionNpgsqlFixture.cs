// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Data.Common;
using Microsoft.Data.Entity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Data.Entity.FunctionalTests;

namespace EntityFramework7.Npgsql.FunctionalTests
{
    public class TransactionNpgsqlFixture : TransactionFixtureBase<NpgsqlTestStore>
    {
        private readonly IServiceProvider _serviceProvider;

        public TransactionNpgsqlFixture()
        {
            _serviceProvider = new ServiceCollection()
                .AddEntityFramework()
                .AddNpgsql()
                .ServiceCollection()
                .AddSingleton(TestNpgsqlModelSource.GetFactory(OnModelCreating))
                .BuildServiceProvider();
        }

        public override NpgsqlTestStore CreateTestStore()
        {
            var db = NpgsqlTestStore.CreateScratch();

            using (var context = CreateContext(db))
            {
                Seed(context);
            }

            return db;
        }

        public override DbContext CreateContext(NpgsqlTestStore testStore)
        {
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseNpgsql(testStore.ConnectionString);

            return new DbContext(_serviceProvider, optionsBuilder.Options);
        }

        public override DbContext CreateContext(DbConnection connection)
        {
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseNpgsql(connection);

            return new DbContext(_serviceProvider, optionsBuilder.Options);
        }
    }
}
