// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using EntityFramework7.Npgsql.FunctionalTests.TestModels;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Internal;
using Microsoft.Data.Entity.Storage.Internal;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Xunit;

namespace EntityFramework7.Npgsql.FunctionalTests
{
    public class ConnectionSpecificationTest
    {
        [Fact]
        public void Can_specify_connection_string_in_OnConfiguring()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection
                .AddEntityFramework()
                .AddNpgsql()
                .AddDbContext<StringInOnConfiguringContext>();

            var serviceProvider = serviceCollection.BuildServiceProvider();

            using (NpgsqlNorthwindContext.GetSharedStore())
            {
                using (var context = serviceProvider.GetRequiredService<StringInOnConfiguringContext>())
                {
                    Assert.True(context.Customers.Any());
                }
            }
        }

        [Fact]
        public void Can_specify_connection_string_in_OnConfiguring_with_default_service_provider()
        {
            using (NpgsqlNorthwindContext.GetSharedStore())
            {
                using (var context = new StringInOnConfiguringContext())
                {
                    Assert.True(context.Customers.Any());
                }
            }
        }

        private class StringInOnConfiguringContext : NorthwindContextBase
        {
            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseNpgsql(NpgsqlNorthwindContext.ConnectionString);
            }
        }

        [Fact]
        public void Can_specify_connection_in_OnConfiguring()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection
                .AddScoped(p => new NpgsqlConnection(NpgsqlNorthwindContext.ConnectionString))
                .AddEntityFramework()
                .AddNpgsql()
                .AddDbContext<ConnectionInOnConfiguringContext>();

            var serviceProvider = serviceCollection.BuildServiceProvider();

            using (NpgsqlNorthwindContext.GetSharedStore())
            {
                using (var context = serviceProvider.GetRequiredService<ConnectionInOnConfiguringContext>())
                {
                    Assert.True(context.Customers.Any());
                }
            }
        }

        [Fact]
        public void Can_specify_connection_in_OnConfiguring_with_default_service_provider()
        {
            using (NpgsqlNorthwindContext.GetSharedStore())
            {
                using (var context = new ConnectionInOnConfiguringContext(new NpgsqlConnection(NpgsqlNorthwindContext.ConnectionString)))
                {
                    Assert.True(context.Customers.Any());
                }
            }
        }

        private class ConnectionInOnConfiguringContext : NorthwindContextBase
        {
            private readonly NpgsqlConnection _connection;

            public ConnectionInOnConfiguringContext(NpgsqlConnection connection)
            {
                _connection = connection;
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseNpgsql(_connection);
            }

            public override void Dispose()
            {
                _connection.Dispose();
                base.Dispose();
            }
        }

        private class StringInConfigContext : NorthwindContextBase
        {
            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseNpgsql("Database=Crunchie");
            }
        }

        [Fact]
        public void Throws_if_no_connection_found_in_config_without_UseNpgsql()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection
                .AddEntityFramework()
                .AddNpgsql()
                .AddDbContext<NoUseNpgsqlContext>();

            var serviceProvider = serviceCollection.BuildServiceProvider();

            using (var context = serviceProvider.GetRequiredService<NoUseNpgsqlContext>())
            {
                Assert.Equal(
                    CoreStrings.NoProviderConfigured,
                    Assert.Throws<InvalidOperationException>(() => context.Customers.Any()).Message);
            }
        }

        [Fact]
        public void Throws_if_no_config_without_UseNpgsql()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection
                .AddEntityFramework()
                .AddNpgsql()
                .AddDbContext<NoUseNpgsqlContext>();

            var serviceProvider = serviceCollection.BuildServiceProvider();

            using (var context = serviceProvider.GetRequiredService<NoUseNpgsqlContext>())
            {
                Assert.Equal(
                    CoreStrings.NoProviderConfigured,
                    Assert.Throws<InvalidOperationException>(() => context.Customers.Any()).Message);
            }
        }

        private class NoUseNpgsqlContext : NorthwindContextBase
        {
        }

        [Fact]
        public void Can_select_appropriate_provider_when_multiple_registered()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection
                .AddScoped<SomeService>()
                .AddEntityFramework()
                .AddNpgsql()
                .AddInMemoryDatabase()
                .AddDbContext<MultipleProvidersContext>();

            var serviceProvider = serviceCollection.BuildServiceProvider();

            using (NpgsqlNorthwindContext.GetSharedStore())
            {
                MultipleProvidersContext context1;
                MultipleProvidersContext context2;

                using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    using (context1 = serviceScope.ServiceProvider.GetRequiredService<MultipleProvidersContext>())
                    {
                        context1.UseNpgsql = true;

                        Assert.True(context1.Customers.Any());
                    }

                    using (var context1B = serviceScope.ServiceProvider.GetRequiredService<MultipleProvidersContext>())
                    {
                        Assert.Same(context1, context1B);
                    }

                    var someService = serviceScope.ServiceProvider.GetRequiredService<SomeService>();
                    Assert.Same(context1, someService.Context);
                }
                using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    using (context2 = serviceScope.ServiceProvider.GetRequiredService<MultipleProvidersContext>())
                    {
                        context2.UseNpgsql = false;

                        Assert.False(context2.Customers.Any());
                    }

                    using (var context2B = serviceScope.ServiceProvider.GetRequiredService<MultipleProvidersContext>())
                    {
                        Assert.Same(context2, context2B);
                    }

                    var someService = serviceScope.ServiceProvider.GetRequiredService<SomeService>();
                    Assert.Same(context2, someService.Context);
                }

                Assert.NotSame(context1, context2);
            }
        }

        [Fact]
        public void Can_select_appropriate_provider_when_multiple_registered_with_default_service_provider()
        {
            using (NpgsqlNorthwindContext.GetSharedStore())
            {
                using (var context = new MultipleProvidersContext())
                {
                    context.UseNpgsql = true;

                    Assert.True(context.Customers.Any());
                }

                using (var context = new MultipleProvidersContext())
                {
                    context.UseNpgsql = false;

                    Assert.False(context.Customers.Any());
                }
            }
        }

        private class MultipleProvidersContext : NorthwindContextBase
        {
            public bool UseNpgsql { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                if (UseNpgsql)
                {
                    optionsBuilder.UseNpgsql(NpgsqlNorthwindContext.ConnectionString);
                }
                else
                {
                    optionsBuilder.UseInMemoryDatabase();
                }
            }
        }

        private class SomeService
        {
            public SomeService(MultipleProvidersContext context)
            {
                Context = context;
            }

            public MultipleProvidersContext Context { get; }
        }

        [Fact]
        public void Can_depend_on_DbContextOptions()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection
                .AddScoped(p => new NpgsqlConnection(NpgsqlNorthwindContext.ConnectionString))
                .AddEntityFramework()
                .AddNpgsql()
                .AddDbContext<OptionsContext>();

            var serviceProvider = serviceCollection.BuildServiceProvider();

            using (NpgsqlNorthwindContext.GetSharedStore())
            {
                using (var context = serviceProvider.GetRequiredService<OptionsContext>())
                {
                    Assert.True(context.Customers.Any());
                }
            }
        }

        [Fact]
        public void Can_depend_on_DbContextOptions_with_default_service_provider()
        {
            using (NpgsqlNorthwindContext.GetSharedStore())
            {
                using (var context = new OptionsContext(
                    new DbContextOptions<OptionsContext>(),
                    new NpgsqlConnection(NpgsqlNorthwindContext.ConnectionString)))
                {
                    Assert.True(context.Customers.Any());
                }
            }
        }

        private class OptionsContext : NorthwindContextBase
        {
            private readonly NpgsqlConnection _connection;
            private readonly DbContextOptions<OptionsContext> _options;

            public OptionsContext(DbContextOptions<OptionsContext> options, NpgsqlConnection connection)
                : base(options)
            {
                _options = options;
                _connection = connection;
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                Assert.Same(_options, optionsBuilder.Options);

                optionsBuilder.UseNpgsql(_connection);

                Assert.NotSame(_options, optionsBuilder.Options);
            }

            public override void Dispose()
            {
                _connection.Dispose();
                base.Dispose();
            }
        }

        [Fact]
        public void Can_register_multiple_context_types()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection
                .AddEntityFramework()
                .AddNpgsql()
                .AddInMemoryDatabase()
                .AddDbContext<MultipleContext1>()
                .AddDbContext<MultipleContext2>();

            var serviceProvider = serviceCollection.BuildServiceProvider();

            using (NpgsqlNorthwindContext.GetSharedStore())
            {
                using (var context = serviceProvider.GetRequiredService<MultipleContext1>())
                {
                    Assert.True(context.Customers.Any());
                }

                using (var context = serviceProvider.GetRequiredService<MultipleContext2>())
                {
                    Assert.False(context.Customers.Any());
                }
            }
        }

        [Fact]
        public void Can_register_multiple_context_types_with_default_service_provider()
        {
            using (NpgsqlNorthwindContext.GetSharedStore())
            {
                using (var context = new MultipleContext1(new DbContextOptions<MultipleContext1>()))
                {
                    Assert.True(context.Customers.Any());
                }

                using (var context = new MultipleContext2(new DbContextOptions<MultipleContext2>()))
                {
                    Assert.False(context.Customers.Any());
                }
            }
        }

        private class MultipleContext1 : NorthwindContextBase
        {
            private readonly DbContextOptions<MultipleContext1> _options;

            public MultipleContext1(DbContextOptions<MultipleContext1> options)
                : base(options)
            {
                _options = options;
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                Assert.Same(_options, optionsBuilder.Options);

                optionsBuilder.UseNpgsql(NpgsqlNorthwindContext.ConnectionString);

                Assert.NotSame(_options, optionsBuilder.Options);
            }
        }

        private class MultipleContext2 : NorthwindContextBase
        {
            private readonly DbContextOptions<MultipleContext2> _options;

            public MultipleContext2(DbContextOptions<MultipleContext2> options)
                : base(options)
            {
                _options = options;
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                Assert.Same(_options, optionsBuilder.Options);

                optionsBuilder.UseInMemoryDatabase();

                Assert.NotSame(_options, optionsBuilder.Options);
            }
        }

        [Fact]
        public void Can_depend_on_non_generic_options_when_only_one_context()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection
                .AddEntityFramework()
                .AddNpgsql()
                .AddInMemoryDatabase()
                .AddDbContext<NonGenericOptionsContext>();

            var serviceProvider = serviceCollection.BuildServiceProvider();

            using (NpgsqlNorthwindContext.GetSharedStore())
            {
                using (var context = serviceProvider.GetRequiredService<NonGenericOptionsContext>())
                {
                    Assert.True(context.Customers.Any());
                }
            }
        }

        [Fact]
        public void Can_depend_on_non_generic_options_when_only_one_context_with_default_service_provider()
        {
            using (NpgsqlNorthwindContext.GetSharedStore())
            {
                using (var context = new NonGenericOptionsContext(new DbContextOptions<DbContext>()))
                {
                    Assert.True(context.Customers.Any());
                }
            }
        }

        private class NonGenericOptionsContext : NorthwindContextBase
        {
            private readonly DbContextOptions _options;

            public NonGenericOptionsContext(DbContextOptions options)
                : base(options)
            {
                _options = options;
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                Assert.Same(_options, optionsBuilder.Options);

                optionsBuilder.UseNpgsql(NpgsqlNorthwindContext.ConnectionString);

                Assert.NotSame(_options, optionsBuilder.Options);
            }
        }

        private class NorthwindContextBase : DbContext
        {
            protected NorthwindContextBase()
            {
            }

            protected NorthwindContextBase(DbContextOptions options)
                : base(options)
            {
            }

            public DbSet<Customer> Customers { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Customer>(b =>
                {
                    b.HasKey(c => c.CustomerID);
                    b.ForNpgsqlToTable("Customers");
                });
            }
        }

        private class Customer
        {
            public string CustomerID { get; set; }
            public string CompanyName { get; set; }
            public string Fax { get; set; }
        }

        #region Added for Npgsql

        [Fact]
        public void Can_specify_connection_in_OnConfiguring_and_create_master_connection()
        {
            using (var conn = new NpgsqlConnection(NpgsqlNorthwindContext.ConnectionString))
            {
                conn.Open();

                var serviceCollection = new ServiceCollection();
                serviceCollection
                    .AddEntityFramework()
                    .AddNpgsql();

                using (NpgsqlNorthwindContext.GetSharedStore())
                {
                    using (var context = new ConnectionInOnConfiguringContext(conn))
                    {
                        var relationalConn = context.GetService<NpgsqlRelationalConnection>();
                        using (var masterConn = relationalConn.CreateMasterConnection())
                        {
                            masterConn.Open();
                        }
                    }
                }
            }
        }

        #endregion
    }
}
