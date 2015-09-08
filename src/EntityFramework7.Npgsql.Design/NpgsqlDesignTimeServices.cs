using JetBrains.Annotations;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Scaffolding.Internal;
using Microsoft.Data.Entity.Storage;
using Microsoft.Data.Entity.Storage.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Data.Entity.Scaffolding
{
    public class NpgsqlDesignTimeServices
    {
        public virtual void ConfigureDesignTimeServices([NotNull] IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddSingleton<IScaffoldingModelFactory, NpgsqlScaffoldingModelFactory>()
                .AddSingleton<IRelationalAnnotationProvider, NpgsqlAnnotationProvider>()
                .AddSingleton<IRelationalTypeMapper, NpgsqlTypeMapper>()
                .AddSingleton<IDatabaseModelFactory, NpgsqlDatabaseModelFactory>();
        }
    }
}
