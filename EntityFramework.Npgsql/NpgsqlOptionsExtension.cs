using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Relational;
using Microsoft.Data.Entity.Utilities;

namespace EntityFramework.Npgsql.Extensions
{
	public class NpgsqlOptionsExtension : RelationalOptionsExtension
    {
		protected override void ApplyServices(EntityFrameworkServicesBuilder builder)
		{
			Check.NotNull(builder, "builder");

			builder.AddNpgsql();
		}
	}
}