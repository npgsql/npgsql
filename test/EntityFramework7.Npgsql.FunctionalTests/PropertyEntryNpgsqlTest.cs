using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity.FunctionalTests;
using Xunit;

namespace EntityFramework7.Npgsql.FunctionalTests
{
    public class PropertyEntryNpgsqlTest : PropertyEntryTestBase<NpgsqlTestStore, F1NpgsqlFixture>
    {
        public PropertyEntryNpgsqlTest(F1NpgsqlFixture fixture)
            : base(fixture)
        {
        }

        public override void Property_entry_original_value_is_set()
        {
            base.Property_entry_original_value_is_set();

            Assert.Contains(
                @"SELECT ""e"".""Id"", ""e"".""EngineSupplierId"", ""e"".""Name""
FROM ""Engines"" AS ""e""
LIMIT 1",
                Sql);

            Assert.Contains(
                @"UPDATE ""Engines"" SET ""Name"" = @p0
WHERE ""Id"" = @p1 AND ""EngineSupplierId"" = @p2 AND ""Name"" = @p3",
                Sql);
        }

        private static string Sql => TestSqlLoggerFactory.Sql.ToUnixNewlines();
    }
}
