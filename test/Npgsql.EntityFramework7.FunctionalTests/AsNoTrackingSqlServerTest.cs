using Microsoft.Data.Entity.FunctionalTests;

namespace Npgsql.EntityFramework7.FunctionalTests
{
    public class AsNoTrackingSqlServerTest : AsNoTrackingTestBase<NorthwindQueryNpgsqlFixture>
    {
        public AsNoTrackingSqlServerTest(NorthwindQueryNpgsqlFixture fixture)
            : base(fixture)
        {
        }
    }
}
