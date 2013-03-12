using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace NpgsqlTests
{
    public class TestContext : DbContext
    {
        public DbSet<Tablea> Tablea { get; set; }
    }
}
