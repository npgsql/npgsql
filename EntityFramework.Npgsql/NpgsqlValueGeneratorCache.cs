using JetBrains.Annotations;
using Microsoft.Data.Entity.ValueGeneration.Internal;

namespace EntityFramework.Npgsql.Extensions
{
	public class NpgsqlValueGeneratorCache : ValueGeneratorCache
    {
        public NpgsqlValueGeneratorCache([NotNull] NpgsqlValueGeneratorFactorySelector selector)
            : base(selector)
        { }
    }
}