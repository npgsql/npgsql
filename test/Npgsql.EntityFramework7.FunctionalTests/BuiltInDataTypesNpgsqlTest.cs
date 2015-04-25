using System;

using Microsoft.Data.Entity;
using Microsoft.Data.Entity.FunctionalTests;

namespace Npgsql.EntityFramework7.FunctionalTests
{
    public class BuiltInDataTypesNpgsqlTest : BuiltInDataTypesTestBase<NpgsqlTestStore, BuiltInDataTypesNpgsqlFixture>
    {
        public BuiltInDataTypesNpgsqlTest(BuiltInDataTypesNpgsqlFixture fixture)
            : base(fixture)
        {
        }
    }
}
