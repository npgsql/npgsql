using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.FunctionalTests;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace EntityFramework7.Npgsql.FunctionalTests
{
    public class MigrationsNpgsqlFixture : MigrationsFixtureBase
    {
        private readonly DbContextOptions _options;
        private readonly IServiceProvider _serviceProvider;

        public MigrationsNpgsqlFixture()
        {
            _serviceProvider = new ServiceCollection()
                .AddEntityFramework()
                .AddNpgsql()
                .ServiceCollection()
                .BuildServiceProvider();

            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseNpgsql(NpgsqlTestStore.CreateConnectionString(nameof(MigrationsNpgsqlTest)));
            _options = optionsBuilder.Options;
        }

        public override MigrationsContext CreateContext()
            => new MigrationsContext(_serviceProvider, _options);
    }
}
