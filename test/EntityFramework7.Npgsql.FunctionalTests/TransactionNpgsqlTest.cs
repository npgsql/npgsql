// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.Data.Entity.FunctionalTests;
using Xunit;

namespace EntityFramework7.Npgsql.FunctionalTests
{
    public class TransactionNpgsqlTest : TransactionTestBase<NpgsqlTestStore, TransactionNpgsqlFixture>
    {
        public TransactionNpgsqlTest(TransactionNpgsqlFixture fixture)
            : base(fixture)
        {
        }

        protected override bool SnapshotSupported => true;

        protected override bool DirtyReadsOccur => false;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        [Fact(Skip = "TODO")]
        public override async Task QueryAsync_uses_explicit_transaction() { }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    }
}
