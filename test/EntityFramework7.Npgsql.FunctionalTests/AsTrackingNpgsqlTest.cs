using Microsoft.Data.Entity.FunctionalTests;

namespace EntityFramework7.Npgsql.FunctionalTests
{
    public class AsTrackingSqlServerTest : AsTrackingTestBase<NorthwindQueryNpgsqlFixture>
    {
        public AsTrackingSqlServerTest(NorthwindQueryNpgsqlFixture fixture)
            : base(fixture)
        {
        }
    }
}
