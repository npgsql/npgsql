using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.FunctionalTests;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFramework7.Npgsql.FunctionalTests
{
    public class NullKeysNpgsqlTest : NullKeysTestBase<NullKeysNpgsqlTest.NullKeysNpgsqlFixture>
    {
        public NullKeysNpgsqlTest(NullKeysNpgsqlFixture fixture)
            : base(fixture)
        {
        }

        public class NullKeysNpgsqlFixture : NullKeysFixtureBase
        {
            private readonly IServiceProvider _serviceProvider;
            private readonly DbContextOptions _options;

            public NullKeysNpgsqlFixture()
            {
                _serviceProvider = new ServiceCollection()
                    .AddEntityFramework()
                    .AddNpgsql()
                    .ServiceCollection()
                    .AddSingleton(TestNpgsqlModelSource.GetFactory(OnModelCreating))
                    .BuildServiceProvider();

                var optionsBuilder = new DbContextOptionsBuilder();
                optionsBuilder.UseNpgsql(NpgsqlTestStore.CreateConnectionString("StringsContext"));
                _options = optionsBuilder.Options;

                EnsureCreated();
            }

            public override DbContext CreateContext()
            {
                return new DbContext(_serviceProvider, _options);
            }
        }
    }
}
