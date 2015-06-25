using System.Data.SqlClient;
using System.Linq;
using Microsoft.Data.Entity.FunctionalTests;
using Microsoft.Data.Entity.FunctionalTests.TestModels.Northwind;
using Microsoft.Data.Entity.Relational.FunctionalTests;
using Microsoft.Data.Entity.Storage;
using Remotion.Linq.Parsing.Structure.IntermediateModel;
using Xunit;

#if DNXCORE50
using System.Threading;
#endif

namespace Npgsql.EntityFramework7.FunctionalTests
{
    public class QueryNpgsqlTest : QueryTestBase<NorthwindQueryNpgsqlFixture>
    {
        public QueryNpgsqlTest(NorthwindQueryNpgsqlFixture fixture)
            : base(fixture) { }

        [Fact(Skip = "TODO")]
        public override void Projection_when_arithmetic_mixed() { }

        [Fact(Skip = "TODO")]
        public override void Projection_when_arithmetic_expressions() { }
    }
}
