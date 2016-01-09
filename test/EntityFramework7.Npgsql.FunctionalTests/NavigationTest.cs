﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.FunctionalTests;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Xunit;

namespace EntityFramework7.Npgsql.FunctionalTests
{
    public class NavigationTest : IClassFixture<NavigationTestFixture>
    {
        [Fact]
        public void Duplicate_entries_are_not_created_for_navigations_to_principal()
        {
            using (var context = _fixture.CreateContext())
            {
                context.ConfigAction = modelBuilder =>
                {
                    modelBuilder.Entity<Person>().HasMany(p => p.Siblings).WithOne(p => p.SiblingReverse).IsRequired(false);
                    modelBuilder.Entity<Person>().HasOne(p => p.Lover).WithOne(p => p.LoverReverse).IsRequired(false);
                    return 0;
                };

                var model = context.Model;
                var entityType = model.GetEntityTypes().First();

                Assert.Equal("'Person' {'LoverId'} -> 'Person' {'Id'}", entityType.GetForeignKeys().First().ToString());
                Assert.Equal("'Person' {'SiblingReverseId'} -> 'Person' {'Id'}", entityType.GetForeignKeys().Skip(1).First().ToString());
            }
        }

        [Fact]
        public void Duplicate_entries_are_not_created_for_navigations_to_dependant()
        {
            using (var context = _fixture.CreateContext())
            {
                context.ConfigAction = modelBuilder =>
                {
                    modelBuilder.Entity<Person>().HasOne(p => p.SiblingReverse).WithMany(p => p.Siblings).IsRequired(false);
                    modelBuilder.Entity<Person>().HasOne(p => p.Lover).WithOne(p => p.LoverReverse).IsRequired(false);
                    return 0;
                };

                var model = context.Model;
                var entityType = model.GetEntityTypes().First();

                Assert.Equal("'Person' {'LoverId'} -> 'Person' {'Id'}", entityType.GetForeignKeys().First().ToString());
                Assert.Equal("'Person' {'SiblingReverseId'} -> 'Person' {'Id'}", entityType.GetForeignKeys().Skip(1).First().ToString());
            }
        }

        private readonly NavigationTestFixture _fixture;

        public NavigationTest(NavigationTestFixture fixture)
        {
            _fixture = fixture;
        }
    }

    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<Person> Siblings { get; set; }
        public Person Lover { get; set; }
        public Person LoverReverse { get; set; }
        public Person SiblingReverse { get; set; }
    }

    public class GoTContext : DbContext
    {
        public GoTContext(IServiceProvider serviceProvider, DbContextOptions options)
            : base(serviceProvider, options)
        {
        }

        public DbSet<Person> People { get; set; }
        public Func<ModelBuilder, int> ConfigAction { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigAction.Invoke(modelBuilder);
        }
    }

    public class NavigationTestFixture
    {
        private readonly DbContextOptions _options;
        private readonly IServiceProvider _serviceProvider;

        public NavigationTestFixture()
        {
            _serviceProvider = new ServiceCollection()
                .AddEntityFramework()
                .AddNpgsql()
                .ServiceCollection()
                .BuildServiceProvider();

            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseNpgsql(NpgsqlTestStore.CreateConnectionString("StateManagerBug"));
            _options = optionsBuilder.Options;
        }

        public virtual GoTContext CreateContext() => new GoTContext(_serviceProvider, _options);
    }
}
