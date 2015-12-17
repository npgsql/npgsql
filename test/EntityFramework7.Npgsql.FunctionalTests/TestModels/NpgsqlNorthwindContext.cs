// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.Data.Entity.FunctionalTests.TestModels.Northwind;
using Microsoft.Data.Entity.Infrastructure;

namespace EntityFramework7.Npgsql.FunctionalTests.TestModels
{
    public class NpgsqlNorthwindContext : NorthwindContext
    {
        public static readonly string DatabaseName = StoreName;
        public static readonly string ConnectionString = NpgsqlTestStore.CreateConnectionString(DatabaseName);

        public NpgsqlNorthwindContext(IServiceProvider serviceProvider, DbContextOptions options)
            : base(serviceProvider, options)
        {
        }
        public static NpgsqlTestStore GetSharedStore()
        {
            return NpgsqlTestStore.GetOrCreateShared(
                DatabaseName,
                () => NpgsqlTestStore.CreateDatabase(DatabaseName, scriptPath: "Northwind.sql")); // relative from bin/<config>
        }
    }
}
