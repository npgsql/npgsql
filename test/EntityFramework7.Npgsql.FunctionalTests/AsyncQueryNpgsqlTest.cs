using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.FunctionalTests;
using Xunit;

namespace EntityFramework7.Npgsql.FunctionalTests
{
    public class AsyncQueryNpgsqlTest : AsyncQueryTestBase<NorthwindQueryNpgsqlFixture>
    {
        private readonly NorthwindQueryNpgsqlFixture _fixture;

        public AsyncQueryNpgsqlTest(NorthwindQueryNpgsqlFixture fixture) : base(fixture)
        {
            _fixture = fixture;
        }

        #region Skipped tests

        [Fact(Skip = "Test commented out in EF7 (SqlServer/Sqlite)")]
        public override Task Projection_when_arithmetic_expressions() { return null; }

        [Fact(Skip = "Test commented out in EF7 (SqlServer/Sqlite)")]
        public override Task Projection_when_arithmetic_mixed() { return null; }

        [Fact(Skip = "Test commented out in EF7 (SqlServer/Sqlite)")]
        public override Task Projection_when_arithmetic_mixed_subqueries() { return null; }

        [Fact(Skip = "https://github.com/aspnet/EntityFramework/issues/3036")]
        public override Task GroupJoin_customers_orders_count() { return null; }

        #endregion

        [Fact]
        public async Task Completes_When_No_Results()
        {
            using (var db = _fixture.CreateContext())
            {
                await db.Customers.Where(r => r.Address == "notindatabase").FirstOrDefaultAsync();
            }
        }
    }
}
