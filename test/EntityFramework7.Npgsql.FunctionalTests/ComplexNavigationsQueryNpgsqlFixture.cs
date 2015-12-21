using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.FunctionalTests;
using Microsoft.Data.Entity.FunctionalTests.TestModels.ComplexNavigationsModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EntityFramework7.Npgsql.FunctionalTests
{
    public class ComplexNavigationsQueryNpgsqlFixture : ComplexNavigationsQueryRelationalFixture<NpgsqlTestStore>
    {
        public static readonly string DatabaseName = "ComplexNavigations";

        private readonly IServiceProvider _serviceProvider;

        private readonly string _connectionString = NpgsqlTestStore.CreateConnectionString(DatabaseName);

        public ComplexNavigationsQueryNpgsqlFixture()
        {
            _serviceProvider = new ServiceCollection()
                .AddEntityFramework()
                .AddNpgsql()
                .ServiceCollection()
                .AddSingleton(TestNpgsqlModelSource.GetFactory(OnModelCreating))
                .AddSingleton<ILoggerFactory>(new TestSqlLoggerFactory())
                .BuildServiceProvider();
        }

        public override NpgsqlTestStore CreateTestStore() =>
            NpgsqlTestStore.GetOrCreateShared(
                DatabaseName,
                () =>
                {
                    var optionsBuilder = new DbContextOptionsBuilder();
                    optionsBuilder.UseNpgsql(_connectionString);

                    using (var context = new ComplexNavigationsContext(_serviceProvider, optionsBuilder.Options))
                    {
                        // TODO: Delete DB if model changed
                        context.Database.EnsureDeleted();
                        if (context.Database.EnsureCreated())
                        {
                            ComplexNavigationsModelInitializer.Seed(context);
                        }

                        TestSqlLoggerFactory.SqlStatements.Clear();
                    }
                });

        public override ComplexNavigationsContext CreateContext(NpgsqlTestStore testStore)
        {
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseNpgsql(testStore.Connection);

            var context = new ComplexNavigationsContext(_serviceProvider, optionsBuilder.Options);
            context.Database.UseTransaction(testStore.Transaction);

            return context;
        }
    }
}
