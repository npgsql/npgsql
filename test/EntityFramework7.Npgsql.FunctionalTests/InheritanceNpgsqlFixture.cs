using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.FunctionalTests;
using Microsoft.Data.Entity.FunctionalTests.TestModels.Inheritance;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EntityFramework7.Npgsql.FunctionalTests
{
    public class InheritanceNpgsqlFixture : InheritanceRelationalFixture
    {
        private readonly DbContextOptions _options;
        private readonly IServiceProvider _serviceProvider;

        public InheritanceNpgsqlFixture()
        {
            _serviceProvider
                = new ServiceCollection()
                    .AddEntityFramework()
                    .AddNpgsql()
                    .ServiceCollection()
                    .AddSingleton(TestNpgsqlModelSource.GetFactory(OnModelCreating))
                    .AddSingleton<ILoggerFactory>(new TestSqlLoggerFactory())
                    .BuildServiceProvider();

            var testStore = NpgsqlTestStore.CreateScratch();

            var optionsBuilder = new DbContextOptionsBuilder();

            optionsBuilder
                .EnableSensitiveDataLogging()
                .UseNpgsql(testStore.Connection);

            _options = optionsBuilder.Options;

            // TODO: Do this via migrations & update pipeline

            testStore.ExecuteNonQuery(@"
                DROP TABLE IF EXISTS ""Country"";
                DROP TABLE IF EXISTS ""Animal"";

                CREATE TABLE ""Country"" (
                    ""Id"" int NOT NULL PRIMARY KEY,
                    ""Name"" text NOT NULL
                );

                CREATE TABLE ""Animal"" (
                    ""Species"" text NOT NULL PRIMARY KEY,
                    ""Name"" text NOT NULL,
                    ""CountryId"" int NOT NULL ,
                    ""IsFlightless"" boolean NOT NULL,
                    ""EagleId"" text,
                    ""Group"" int,
                    ""FoundOn"" smallint,
                    ""Discriminator"" text,

                    FOREIGN KEY(""CountryId"") REFERENCES ""Country""(""Id""),
                    FOREIGN KEY(""EagleId"") REFERENCES ""Animal""(""Species"")
                );

                CREATE TABLE ""Plant"" (
                    ""Genus"" int NOT NULL,
                    ""Species"" text NOT NULL PRIMARY KEY,
                    ""Name"" text NOT NULL,
                    ""CountryId"" int,
                    ""HasThorns"" boolean,

                    FOREIGN KEY(""CountryId"") REFERENCES ""Country""(""Id"")
                );
            ");

            using (var context = CreateContext())
            {
                SeedData(context);
            }
            TestSqlLoggerFactory.Reset();
        }

        public override InheritanceContext CreateContext()
        {
            return new InheritanceContext(_serviceProvider, _options);
        }
    }
}
