using JetBrains.Annotations;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Storage;
using Microsoft.Data.Entity.Utilities;
using System;
using Microsoft.Framework.DependencyInjection;

namespace EntityFramework.Npgsql.Extensions
{
    public class NpgsqlDataStoreSource : DataStoreSource<NpgsqlDataStore,NpgsqlDataStoreServices, NpgsqlOptionsExtension>
    {
        public override string Name
        {
            get { return typeof(NpgsqlDataStore).Name; }
        }

        public override void AutoConfigure(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql();
        }
    }
}
