using System;

using Microsoft.Data.Entity;
using Microsoft.Data.Entity.FunctionalTests;

namespace Npgsql.EntityFramework7.FunctionalTests
{
    public class BuiltInDataTypesNpgsqlTest : BuiltInDataTypesTestBase<BuiltInDataTypesNpgsqlFixture>
    {
        public BuiltInDataTypesNpgsqlTest(BuiltInDataTypesNpgsqlFixture fixture)
            : base(fixture)
        {
        }

        // TODO: Other tests like in BuiltInDataTypesSqlServerTest?
    }
}
