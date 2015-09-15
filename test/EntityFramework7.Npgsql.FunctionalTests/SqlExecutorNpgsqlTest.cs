using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity.FunctionalTests;
using Xunit;

namespace EntityFramework7.Npgsql.FunctionalTests
{
    public class SqlExecutorNpgsqlTest : SqlExecutorTestBase<NorthwindQueryNpgsqlFixture>
    {
        public override void Executes_stored_procedure()
        {
            base.Executes_stored_procedure();

            Assert.Equal(
                TenMostExpensiveProductsSproc,
                Sql);
        }

        public override void Executes_stored_procedure_with_parameter()
        {
            base.Executes_stored_procedure_with_parameter();

            Assert.Equal(
                @"@p0: ALFKI

SELECT * FROM ""CustOrderHist""(@p0)",
                Sql);
        }

        public SqlExecutorNpgsqlTest(NorthwindQueryNpgsqlFixture fixture)
            : base(fixture)
        {
        }

        protected override string TenMostExpensiveProductsSproc => @"SELECT * FROM ""Ten Most Expensive Products""()";

        protected override string CustomerOrderHistorySproc => @"SELECT * FROM ""CustOrderHist""({0})";

        private static string Sql => TestSqlLoggerFactory.Sql;
    }
}
