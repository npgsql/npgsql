using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.FunctionalTests;
using Xunit;

namespace EntityFramework7.Npgsql.FunctionalTests
{
    public class MappingQueryNpgsqlTest : MappingQueryTestBase, IClassFixture<MappingQueryNpgsqlFixture>
    {
        public override void All_customers()
        {
            base.All_customers();

            Assert.Equal(
                @"SELECT ""c"".""CustomerID"", ""c"".""CompanyName""
FROM ""Customers"" AS ""c""",
                Sql);
        }

        public override void All_employees()
        {
            base.All_employees();

            Assert.Equal(
                @"SELECT ""e"".""EmployeeID"", ""e"".""City""
FROM ""Employees"" AS ""e""",
                Sql);
        }

        public override void All_orders()
        {
            base.All_orders();

            Assert.Equal(
                @"SELECT ""o"".""OrderID"", ""o"".""ShipVia""
FROM ""Orders"" AS ""o""",
                Sql);
        }

        public override void Project_nullable_enum()
        {
            base.Project_nullable_enum();

            Assert.Equal(
                @"SELECT ""o"".""ShipVia""
FROM ""Orders"" AS ""o""",
                Sql);
        }

        private readonly MappingQueryNpgsqlFixture _fixture;

        public MappingQueryNpgsqlTest(MappingQueryNpgsqlFixture fixture)
        {
            _fixture = fixture;
        }

        protected override DbContext CreateContext()
        {
            return _fixture.CreateContext();
        }

        private static string Sql => TestSqlLoggerFactory.Sql;
    }
}
