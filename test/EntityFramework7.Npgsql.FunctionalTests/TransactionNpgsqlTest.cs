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
    }
}
