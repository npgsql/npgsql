using JetBrains.Annotations;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Relational;
using Microsoft.Data.Entity.Utilities;

namespace EntityFramework.Npgsql.Extensions
{
	public class NpgsqlOptionsExtension : RelationalOptionsExtension
    {
    	public NpgsqlOptionsExtension([NotNull] IDbContextOptions options)
            : base(options)
        {
        }
        
		public override void ApplyServices(EntityFrameworkServicesBuilder builder)
		{
			Check.NotNull(builder, "builder");

			builder.AddNpgsql();
		}
	}
}
