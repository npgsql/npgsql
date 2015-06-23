using System;

using Microsoft.Data.Entity;
using Microsoft.Data.Entity.FunctionalTests;
using Xunit;

namespace Npgsql.EntityFramework7.FunctionalTests
{
    public class BuiltInDataTypesNpgsqlTest : BuiltInDataTypesTestBase<BuiltInDataTypesNpgsqlFixture>
    {
        public BuiltInDataTypesNpgsqlTest(BuiltInDataTypesNpgsqlFixture fixture)
            : base(fixture)
        {
        }

        [Fact(Skip = "https://github.com/aspnet/EntityFramework/pull/2447")]
        public override void Can_insert_and_read_back_all_nullable_data_types_with_values_set_to_non_null() {}

        [Fact(Skip = "https://github.com/aspnet/EntityFramework/pull/2447")]
        public override void Can_query_using_any_data_type() { }

        [Fact(Skip = "https://github.com/aspnet/EntityFramework/pull/2447")]
        public override void Can_insert_and_read_back_all_non_nullable_data_types() { }

        [Fact(Skip = "https://github.com/aspnet/EntityFramework/pull/2447")]
        public override void Can_query_using_any_nullable_data_type() { }

        // TODO: Other tests like in BuiltInDataTypesSqlServerTest?
    }
}
