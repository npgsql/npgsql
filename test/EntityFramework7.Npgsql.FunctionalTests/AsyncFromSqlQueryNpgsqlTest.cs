using Microsoft.Data.Entity.FunctionalTests;

namespace EntityFramework7.Npgsql.FunctionalTests
{
    public class AsyncFromSqlQueryNpgsqlTest : AsyncFromSqlQueryTestBase<NorthwindQueryNpgsqlFixture>
    {
        public AsyncFromSqlQueryNpgsqlTest(NorthwindQueryNpgsqlFixture fixture)
            : base(fixture)
        {
        }
    }
}
