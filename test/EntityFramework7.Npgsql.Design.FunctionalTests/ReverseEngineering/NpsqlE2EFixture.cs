using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFramework7.Npgsql.FunctionalTests;

namespace EntityFramework7.Npgsql.Design.FunctionalTests.ReverseEngineering
{
    public class NpgsqlE2EFixture
    {
        public NpgsqlE2EFixture()
        {
            NpgsqlTestStore.CreateDatabase(
                "NpgsqlReverseEngineerTestE2E", "ReverseEngineering/E2E.sql", true);
        }
    }
}
