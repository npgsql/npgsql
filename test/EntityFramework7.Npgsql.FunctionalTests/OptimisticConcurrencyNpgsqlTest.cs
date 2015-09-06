using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity.FunctionalTests;
using Xunit;

namespace EntityFramework7.Npgsql.FunctionalTests
{
    public class OptimisticConcurrencyNpgsqlTest : OptimisticConcurrencyTestBase<NpgsqlTestStore, F1NpgsqlFixture>
    {
        public OptimisticConcurrencyNpgsqlTest(F1NpgsqlFixture fixture)
            : base(fixture)
        {
        }

        #region Skipped tests

        [Fact(Skip= "https://github.com/aspnet/EntityFramework/issues/3039")]
        public override Task Change_in_independent_association_after_change_in_different_concurrency_token_results_in_independent_association_exception() { return null; }

        [Fact(Skip = "https://github.com/aspnet/EntityFramework/issues/3039")]
        public override Task Change_in_independent_association_results_in_independent_association_exception() { return null; }

        [Fact(Skip = "https://github.com/aspnet/EntityFramework/issues/3039")]
        public override Task Simple_concurrency_exception_can_be_resolved_with_client_values() { return null; }

        [Fact(Skip = "https://github.com/aspnet/EntityFramework/issues/3039")]
        public override Task Simple_concurrency_exception_can_be_resolved_with_new_values() { return null; }

        [Fact(Skip = "https://github.com/aspnet/EntityFramework/issues/3039")]
        public override Task Simple_concurrency_exception_can_be_resolved_with_store_values() { return null; }

        [Fact(Skip = "https://github.com/aspnet/EntityFramework/issues/3039")]
        public override Task Simple_concurrency_exception_can_be_resolved_with_store_values_using_Reload() { return null; }

        [Fact(Skip = "https://github.com/aspnet/EntityFramework/issues/3039")]
        public override Task Simple_concurrency_exception_can_be_resolved_with_store_values_using_equivalent_of_accept_changes() { return null; }

        [Fact(Skip = "https://github.com/aspnet/EntityFramework/issues/3039")]
        public override Task Updating_then_deleting_the_same_entity_results_in_DbUpdateConcurrencyException() { return null; }

        [Fact(Skip = "https://github.com/aspnet/EntityFramework/issues/3039")]
        public override Task Updating_then_deleting_the_same_entity_results_in_DbUpdateConcurrencyException_which_can_be_resolved_with_store_values() { return null; }

        #endregion
    }
}
