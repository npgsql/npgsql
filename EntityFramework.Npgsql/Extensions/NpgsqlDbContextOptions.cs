using System.Data.Common;
using JetBrains.Annotations;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Relational;
using Microsoft.Data.Entity.Utilities;

namespace EntityFramework.Npgsql.Extensions
{
	public class NpgsqlDbContextOptionsBuilder : RelationalDbContextOptionsBuilder
    {
        public NpgsqlDbContextOptionsBuilder([NotNull] DbContextOptionsBuilder optionsBuilder)
            : base(optionsBuilder)
        { }

        public virtual NpgsqlDbContextOptionsBuilder MaxBatchSize(int maxBatchSize)
        {
        	var extension = GetOrCreateExtension(OptionsBuilder);
            extension.MaxBatchSize = maxBatchSize;
            
            ((IOptionsBuilderExtender)OptionsBuilder).AddOrUpdateExtension<NpgsqlOptionsExtension>(extension);

            return this;
        }

        public virtual NpgsqlDbContextOptionsBuilder CommandTimeout(int? commandTimeout)
        {
        	var extension = GetOrCreateExtension(OptionsBuilder);
            extension.CommandTimeout = commandTimeout;
            
            ((IOptionsBuilderExtender)OptionsBuilder).AddOrUpdateExtension<NpgsqlOptionsExtension>(extension);

            return this;
        }
        
        private static NpgsqlOptionsExtension GetOrCreateExtension(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.Options.FindExtension<NpgsqlOptionsExtension>()
               ?? new NpgsqlOptionsExtension(optionsBuilder.Options);
    }
}
