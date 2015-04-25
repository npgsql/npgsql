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
            : base(fixture) {}

        #region Skipped tests (TODO)

        [Fact(Skip = "TODO")]
        public override void Take_Distinct() {}

        [Fact(Skip = "TODO")]
        public override void Where_query_composition() {}

        [Fact(Skip = "TODO")]
        public override void Where_bool_parameter_compared_to_binary_expression() {}

        [Fact(Skip = "TODO")]
        public override void Where_bool_member_and_parameter_compared_to_binary_expression_nested() { }

        [Fact(Skip = "TODO")]
        public override void Where_not_bool_member_compared_to_binary_expression() { }

        [Fact(Skip = "TODO")]
        public override void Where_bool_member_compared_to_binary_expression() { }
        
        [Fact(Skip = "TODO")]
        public override void Projection_when_arithmetic_mixed() { }

        [Fact(Skip = "TODO")]
        public override void Projection_when_arithmetic_mixed_subqueries() {}

        [Fact(Skip = "TODO")]
        public override void Projection_when_arithmetic_expressions() { }

        [Fact(Skip = "TODO")]
        public override void Select_subquery_recursive_trivial() {}

        [Fact(Skip = "TODO")]
        public override void Where_subquery_anon() {}

        [Fact(Skip = "TODO")]
        public override void SelectMany_mixed() {}

        [Fact(Skip = "TODO")]
        public override void SelectMany_correlated_subquery_hard() { }

        [Fact(Skip = "TODO")]
        public override void SelectMany_Joined_DefaultIfEmpty() { }

        [Fact(Skip = "TODO")]
        public override void SelectMany_Joined_DefaultIfEmpty2() { }

        [Fact(Skip = "TODO")]
        public override void SelectMany_primitive_select_subquery() { }

        [Fact(Skip = "TODO")]
        public override void Select_nested_collection_deep() { }

        [Fact(Skip = "TODO")]
        public override void Where_subquery_recursive_trivial() { }

        [Fact(Skip = "TODO")]
        public override void Where_shadow_subquery_first() { }

        [Fact(Skip = "TODO")]
        public override void All_top_level_subquery() { }

        [Fact(Skip = "TODO")]
        public override void OrderBy_SelectMany() { }

        [Fact(Skip = "TODO")]
        public override void Contains_with_subquery() { }

        #endregion
    }
}