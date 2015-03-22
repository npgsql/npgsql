using System;
using Microsoft.Data.Entity.FunctionalTests;
using Microsoft.Data.Entity.FunctionalTests.TestModels.Northwind;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Relational.FunctionalTests;
//using Microsoft.Data.Entity.Tests;
using Microsoft.Data.Entity;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using Npgsql.EntityFramework7.FunctionalTests.TestModels;
using EntityFramework.Npgsql;

namespace Npgsql.EntityFramework7.FunctionalTests
{
    public class NorthwindQueryNpgsqlFixture : NorthwindQueryRelationalFixture, IDisposable
    {
        readonly IServiceProvider _serviceProvider;
        readonly DbContextOptions _options;
        readonly NpgsqlTestStore _testStore;

        public NorthwindQueryNpgsqlFixture()
        {
            _testStore = NpgsqlNorthwindContext.GetSharedStore();

            _serviceProvider = new ServiceCollection()
                .AddEntityFramework()
                .AddNpgsql()
                .ServiceCollection()
                .AddSingleton(TestNpgsqlModelSource.GetFactory(OnModelCreating))
                .AddInstance<ILoggerFactory>(new TestSqlLoggerFactory())
                .BuildServiceProvider();

            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseNpgsql(_testStore.Connection.ConnectionString);
            _options = optionsBuilder.Options;
        }

        public override NorthwindContext CreateContext()
        {
            return new NpgsqlNorthwindContext(_serviceProvider, _options);
        }

        public void Dispose()
        {
            _testStore.Dispose();
        }
    }
}