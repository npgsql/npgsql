using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity.FunctionalTests;

namespace EntityFramework7.Npgsql.FunctionalTests
{
    public class IncludeAsyncNpgsqlTest : IncludeAsyncTestBase<NorthwindQueryNpgsqlFixture>
    {
        public IncludeAsyncNpgsqlTest(NorthwindQueryNpgsqlFixture fixture)
            : base(fixture)
        {
        }
    }
}
