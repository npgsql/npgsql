using JetBrains.Annotations;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Relational;
using Microsoft.Data.Entity.Utilities;
using Microsoft.Framework.DependencyInjection;

namespace EntityFramework.Npgsql
{
	public class NpgsqlOptionsExtension : RelationalOptionsExtension
    {
        public NpgsqlOptionsExtension()
        {
        }

        public NpgsqlOptionsExtension([NotNull] NpgsqlOptionsExtension copyFrom)
            : base(copyFrom)
        {
        }
        
		public override void ApplyServices(EntityFrameworkServicesBuilder builder)
		{
			Check.NotNull(builder, "builder");

			builder.AddNpgsql();
		}
	}
}
