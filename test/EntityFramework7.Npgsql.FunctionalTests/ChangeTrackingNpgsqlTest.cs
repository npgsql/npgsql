// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Data.Entity.FunctionalTests;
using Xunit;

namespace EntityFramework7.Npgsql.FunctionalTests
{
    public class ChangeTrackingNpgsqlTest : ChangeTrackingTestBase<NorthwindQueryNpgsqlFixture>
    {
        public ChangeTrackingNpgsqlTest(NorthwindQueryNpgsqlFixture fixture)
            : base(fixture)
        {
        }

        [Fact]
        public override void Multiple_entities_can_revert()
        { }
    }
}
