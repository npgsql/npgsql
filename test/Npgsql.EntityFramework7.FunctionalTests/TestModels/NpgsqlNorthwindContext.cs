using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.FunctionalTests.TestModels.Northwind;

namespace Npgsql.EntityFramework7.FunctionalTests.TestModels
{
    public class NpgsqlNorthwindContext : NorthwindContext
    {
        public static readonly string DatabaseName = StoreName;
        public static readonly string ConnectionString = NpgsqlTestStore.CreateConnectionString(DatabaseName);

        public NpgsqlNorthwindContext(IServiceProvider serviceProvider, DbContextOptions options)
            : base(serviceProvider, options)
        {
        }

        /// <summary>
        ///     A transactional test database, pre-populated with Northwind schema/data
        /// </summary>
        public static Task<NpgsqlTestStore> GetSharedStoreAsync()
        {
            return NpgsqlTestStore.GetOrCreateSharedAsync(
                DatabaseName,
                () => NpgsqlTestStore.CreateDatabaseIfNotExistsAsync(DatabaseName, scriptPath: @"..\..\Northwind.sql")); // relative from bin/<config>
        }

        public static NpgsqlTestStore GetSharedStore()
        {
            return NpgsqlTestStore.GetOrCreateShared(
                DatabaseName,
                () => NpgsqlTestStore.CreateDatabaseIfNotExists(DatabaseName, scriptPath: @"..\..\Northwind.sql")); // relative from bin/<config>
        }
    }
}