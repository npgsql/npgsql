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