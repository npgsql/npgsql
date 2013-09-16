#if CODE_FIRST
using Npgsql;
using System.Data.Entity;

namespace CodeFirst.DataModel
{
    public class CodeFirstTestContext : DbContext
	{
        public CodeFirstTestContext() : base("CodeFirstTestContext")
        {
            
        }

        public DbSet<Entity01> Entities01 { get; set; }
	}

#if CODE_FIRST6
    public class NpgsqlTestsConfiguration : DbConfiguration
    {
        public NpgsqlTestsConfiguration()
        {
            SetProviderServices(
                NpgsqlServices.ProviderInvariantName,
                NpgsqlServices.Instance);
        }
    }
#endif
}
#endif