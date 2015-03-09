using System.Data.Common;
using JetBrains.Annotations;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Utilities;

namespace EntityFramework.Npgsql.Extensions
{
    public static class NpgsqlDbContextOptionsExtensions
    {
        public static NpgsqlDbContextOptions UseNpgsql([NotNull] this DbContextOptions options)
        {
            Check.NotNull(options, "options");

            ((IDbContextOptions)options).AddOrUpdateExtension<NpgsqlOptionsExtension>(x => { });

            return new NpgsqlDbContextOptions(options);
        }

        public static NpgsqlDbContextOptions UseNpgsql([NotNull] this DbContextOptions options, [NotNull] string connectionString)
        {
            Check.NotNull(options, "options");
            Check.NotEmpty(connectionString, "connectionString");

            ((IDbContextOptions)options).AddOrUpdateExtension<NpgsqlOptionsExtension>(x => x.ConnectionString = connectionString);

            return new NpgsqlDbContextOptions(options);
        }

        public static NpgsqlDbContextOptions UseNpgsql<T>([NotNull] this DbContextOptions<T> options, [NotNull] string connectionString)
        {
            return UseNpgsql((DbContextOptions)options, connectionString);
        }

        public static NpgsqlDbContextOptions UseNpgsql([NotNull] this DbContextOptions options, [NotNull] DbConnection connection)
        {
            ((IDbContextOptions)options).AddOrUpdateExtension<NpgsqlOptionsExtension>(x => x.Connection = connection);

            return new NpgsqlDbContextOptions(options);
        }

        public static NpgsqlDbContextOptions UseNpgsql<T>([NotNull] this DbContextOptions<T> options, [NotNull] DbConnection connection)
        {
            return UseNpgsql((DbContextOptions)options, connection);
        }
    }
}