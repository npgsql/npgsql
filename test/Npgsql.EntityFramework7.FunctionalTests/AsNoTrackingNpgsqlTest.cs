// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Data.Entity.FunctionalTests;

namespace Npgsql.EntityFramework7.FunctionalTests
{
    public class AsNoTrackingNpgsqlTest : AsNoTrackingTestBase<NorthwindQueryNpgsqlFixture>
    {
        public AsNoTrackingNpgsqlTest(NorthwindQueryNpgsqlFixture fixture)
            : base(fixture)
        {
        }
    }
}
