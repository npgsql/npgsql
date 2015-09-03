using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity;

namespace EntityFramework7.Npgsql.FunctionalTests
{
    public class SentinelGraphUpdatesNpgsqlTest : GraphUpdatesNpgsqlTestBase<SentinelGraphUpdatesNpgsqlTest.SentinelGraphUpdatesNpgsqlFixture>
    {
        public SentinelGraphUpdatesNpgsqlTest(SentinelGraphUpdatesNpgsqlFixture fixture)
            : base(fixture)
        {
        }

        public class SentinelGraphUpdatesNpgsqlFixture : GraphUpdatesNpgsqlFixtureBase
        {
            protected override string DatabaseName => "SentinelGraphUpdatesTest";

            public override int IntSentinel => -10000000;

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                SetSentinelValues(modelBuilder, IntSentinel);
            }
        }
    }
}
