using JetBrains.Annotations;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Storage;
using Microsoft.Data.Entity.Internal;
using System;

namespace EntityFramework.Npgsql.Extensions
{
    public class NpgsqlDataStoreSource : DataStoreSource<NpgsqlDataStoreServices, NpgsqlOptionsExtension>
    {
        public NpgsqlDataStoreSource([NotNull] DbContextServices services, [NotNull] IDbContextOptions options)
            : base(services, options)
        {
        }

        public override string Name
        {
            get { return typeof(NpgsqlDataStore).Name; }
        }

        public override void AutoConfigure()
        {
            ContextOptions.UseNpgsql();
        }
    }
}
