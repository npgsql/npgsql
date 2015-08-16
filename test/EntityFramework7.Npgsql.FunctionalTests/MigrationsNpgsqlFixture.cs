using Microsoft.Data.Entity;
using Microsoft.Data.Entity.FunctionalTests;
using Microsoft.Framework.DependencyInjection;

namespace EntityFramework7.Npgsql.FunctionalTests
{
    public class MigrationsNpgsqlFixture : MigrationsFixtureBase
    {
        protected override void ConfigureServices(IServiceCollection services)
        {
            services.AddEntityFramework().AddNpgsql();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var testStore = NpgsqlTestStore.GetOrCreateShared(nameof(MigrationsNpgsqlTest), async () =>
            {
                NpgsqlTestStore.CreateDatabaseIfNotExists(nameof(MigrationsNpgsqlTest));
            });
            optionsBuilder.UseNpgsql(testStore.Connection.ConnectionString);
        }
    }
}