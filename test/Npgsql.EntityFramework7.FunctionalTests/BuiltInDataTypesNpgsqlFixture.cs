using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.FunctionalTests;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Framework.DependencyInjection;
using Npgsql.EntityFramework7.Metadata;

namespace Npgsql.EntityFramework7.FunctionalTests
{
    public class BuiltInDataTypesNpgsqlFixture : BuiltInDataTypesFixtureBase<NpgsqlTestStore>
    {
        private readonly IServiceProvider _serviceProvider;

        public BuiltInDataTypesNpgsqlFixture()
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
            return NpgsqlTestStore.CreateScratch();
        }

        public override DbContext CreateContext(NpgsqlTestStore testStore)
        {
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseNpgsql(testStore.Connection);

            var context = new DbContext(_serviceProvider, optionsBuilder.Options);
            context.Database.EnsureCreated();
            context.Database.AsRelational().Connection.UseTransaction(testStore.Transaction);
            return context;
        }

        public override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BuiltInNonNullableDataTypes>(b =>
            {
                b.Ignore(dt => dt.TestUnsignedInt16);
                b.Ignore(dt => dt.TestUnsignedInt32);
                b.Ignore(dt => dt.TestUnsignedInt64);
                b.Ignore(dt => dt.TestCharacter);
                b.Ignore(dt => dt.TestSignedByte);
            });

            modelBuilder.Entity<BuiltInNullableDataTypes>(b =>
            {
                b.Ignore(dt => dt.TestNullableUnsignedInt16);
                b.Ignore(dt => dt.TestNullableUnsignedInt32);
                b.Ignore(dt => dt.TestNullableUnsignedInt64);
                b.Ignore(dt => dt.TestNullableCharacter);
                b.Ignore(dt => dt.TestNullableSignedByte);
            });
        }
    }
}
