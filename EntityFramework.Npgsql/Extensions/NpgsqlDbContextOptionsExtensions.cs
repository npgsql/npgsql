using System.Data.Common;
using JetBrains.Annotations;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Utilities;

namespace EntityFramework.Npgsql.Extensions
{
    public static class NpgsqlDbContextOptionsExtensions
    {
        public static NpgsqlDbContextOptionsBuilder UseNpgsql([NotNull] this DbContextOptionsBuilder optionsBuilder)
        {
            Check.NotNull(optionsBuilder, "optionsBuilder");

            ((IOptionsBuilderExtender)optionsBuilder).AddOrUpdateExtension<NpgsqlOptionsExtension>(GetOrCreateExtension(optionsBuilder));

            return new NpgsqlDbContextOptionsBuilder(optionsBuilder);
        }

        public static NpgsqlDbContextOptionsBuilder UseNpgsql([NotNull] this DbContextOptionsBuilder optionsBuilder, [NotNull] string connectionString)
        {
            Check.NotNull(optionsBuilder, "optionsBuilder");
            Check.NotEmpty(connectionString, "connectionString");
            
            var extension = GetOrCreateExtension(optionsBuilder);
            extension.ConnectionString = connectionString;

            ((IOptionsBuilderExtender)optionsBuilder).AddOrUpdateExtension<NpgsqlOptionsExtension>(extension);

            return new NpgsqlDbContextOptionsBuilder(optionsBuilder);
        }

//        public static NpgsqlDbContextOptions UseNpgsql<T>([NotNull] this DbContextOptions<T> options, [NotNull] string connectionString)
//        {
//            return UseNpgsql((DbContextOptions)options, connectionString);
//        }

        public static NpgsqlDbContextOptionsBuilder UseNpgsql([NotNull] this DbContextOptionsBuilder optionsBuilder, [NotNull] DbConnection connection)
        {
            Check.NotNull(optionsBuilder, "optionsBuilder");
            Check.NotNull(connection, "connection");
            
            var extension = GetOrCreateExtension(optionsBuilder);
            extension.Connection = connection;

            ((IOptionsBuilderExtender)optionsBuilder).AddOrUpdateExtension<NpgsqlOptionsExtension>(extension);

            return new NpgsqlDbContextOptionsBuilder(optionsBuilder);
        }

//        public static NpgsqlDbContextOptions UseNpgsql<T>([NotNull] this DbContextOptions<T> options, [NotNull] DbConnection connection)
//        {
//            return UseNpgsql((DbContextOptions)options, connection);
//        }
        
        private static NpgsqlOptionsExtension GetOrCreateExtension(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.Options.FindExtension<NpgsqlOptionsExtension>()
               ?? new NpgsqlOptionsExtension(optionsBuilder.Options);
    }
}
