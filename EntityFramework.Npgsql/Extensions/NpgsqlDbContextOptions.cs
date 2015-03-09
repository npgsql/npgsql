using JetBrains.Annotations;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Relational;

namespace EntityFramework.Npgsql.Extensions
{
	public class NpgsqlDbContextOptions : RelationalDbContextOptions
    {
        public NpgsqlDbContextOptions([NotNull] DbContextOptions options)
            : base(options)
        { }

        public virtual NpgsqlDbContextOptions MaxBatchSize(int maxBatchSize)
        {
            ((IDbContextOptions)Options).AddOrUpdateExtension<NpgsqlOptionsExtension>(x => x.MaxBatchSize = maxBatchSize);

            return this;
        }

        public virtual NpgsqlDbContextOptions CommandTimeout(int? commandTimeout)
        {
            ((IDbContextOptions)Options).AddOrUpdateExtension<NpgsqlOptionsExtension>(x => x.CommandTimeout = commandTimeout);

            return this;
        }
    }
}