using System;

using Microsoft.Data.Entity;
using Microsoft.Data.Entity.FunctionalTests;

namespace Npgsql.EntityFramework7.FunctionalTests
{
    public class BuiltInDataTypesSqlServerTest : BuiltInDataTypesTestBase<NpgsqlTestStore, BuiltInDataTypesNpgsqlFixture>
    {
        public BuiltInDataTypesSqlServerTest(BuiltInDataTypesNpgsqlFixture fixture)
            : base(fixture)
        {
        }
    }
}
