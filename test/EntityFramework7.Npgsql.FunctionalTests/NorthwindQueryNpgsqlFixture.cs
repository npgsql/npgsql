// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using EntityFramework7.Npgsql.FunctionalTests;
using EntityFramework7.Npgsql.FunctionalTests.TestModels;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.FunctionalTests;
using Microsoft.Data.Entity.FunctionalTests.TestModels.Northwind;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EntityFramework7.Npgsql.FunctionalTests
{
    public class NorthwindQueryNpgsqlFixture : NorthwindQueryRelationalFixture, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly DbContextOptions _options;

        private readonly NpgsqlTestStore _testStore = NpgsqlNorthwindContext.GetSharedStore();
        private readonly TestSqlLoggerFactory _testSqlLoggerFactory = new TestSqlLoggerFactory();

        public NorthwindQueryNpgsqlFixture()
        {
            _serviceProvider
                = new ServiceCollection()
                    .AddEntityFramework()
                    .AddNpgsql()
                    .ServiceCollection()
                    .AddSingleton(TestNpgsqlModelSource.GetFactory(OnModelCreating))
                    .AddSingleton<ILoggerFactory>(_testSqlLoggerFactory)
                    .BuildServiceProvider();

            _options = BuildOptions();
        }

        protected DbContextOptions BuildOptions()
        {
            var optionsBuilder = new DbContextOptionsBuilder();

            var NpgsqlDbContextOptionsBuilder
                = optionsBuilder
                    .EnableSensitiveDataLogging()
                    .UseNpgsql(_testStore.Connection.ConnectionString);

            ConfigureOptions(NpgsqlDbContextOptionsBuilder);

            //NpgsqlDbContextOptionsBuilder.ApplyConfiguration();

            return optionsBuilder.Options;
        }

        protected virtual void ConfigureOptions(NpgsqlDbContextOptionsBuilder npgsqlDbContextOptionsBuilder)
        {
        }

        public override NorthwindContext CreateContext()
        {
            var context = new NpgsqlNorthwindContext(_serviceProvider, _options);

            context.ChangeTracker.AutoDetectChangesEnabled = false;

            return context;
        }

        public void Dispose() => _testStore.Dispose();

        public override CancellationToken CancelQuery() => _testSqlLoggerFactory.CancelQuery();
    }
}
