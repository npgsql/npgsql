// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.Data.Entity.Relational.FunctionalTests;
using Xunit;

namespace Npgsql.EntityFramework7.FunctionalTests
{
    public class TransactionNpgsqlTest : TransactionTestBase<NpgsqlTestStore, TransactionNpgsqlFixture>
    {
        public TransactionNpgsqlTest(TransactionNpgsqlFixture fixture)
            : base(fixture)
        {
        }

        protected override bool SnapshotSupported => true;

        [Fact(Skip = "https://github.com/aspnet/EntityFramework/pull/2470")]
        public override void Query_uses_explicit_transaction() { }

        [Fact(Skip = "https://github.com/aspnet/EntityFramework/pull/2470")]
        public override async Task QueryAsync_uses_explicit_transaction() { }
    }
}
