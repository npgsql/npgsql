using EntityFramework.Npgsql.Migrations;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Relational;
using Microsoft.Data.Entity.Storage;
using Microsoft.Data.Entity.Utilities;
using Microsoft.Framework.DependencyInjection;

namespace EntityFramework.Npgsql.Extensions
{
	public static class NpgsqlEntityServicesBuilderExtensions
    {
        public static EntityFrameworkServicesBuilder AddNpgsql([NotNull] this EntityFrameworkServicesBuilder builder)
        {
            Check.NotNull(builder, "builder");

			((IAccessor<IServiceCollection>)builder.AddRelational()).Service
                .AddScoped<DataStoreSource<NpgsqlDataStoreServices, NpgsqlOptionsExtension>, NpgsqlDataStoreSource>()
			    .TryAdd(new ServiceCollection()
					.AddSingleton<NpgsqlModelBuilderFactory>()
					.AddSingleton<NpgsqlValueGeneratorCache>()
					.AddSingleton<NpgsqlSequenceValueGeneratorFactory>()
					.AddSingleton<NpgsqlSqlGenerator>()
					.AddSingleton<SqlStatementExecutor>()
					.AddSingleton<NpgsqlTypeMapper>()
					.AddSingleton<NpgsqlModificationCommandBatchFactory>()
					.AddSingleton<NpgsqlCommandBatchPreparer>()
					.AddSingleton<NpgsqlModelSource>()
					.AddScoped<NpgsqlBatchExecutor>()
					.AddScoped<NpgsqlDataStoreServices>()
					.AddScoped<NpgsqlDataStore>()
					.AddScoped<NpgsqlEntityFrameworkConnection>()
					.AddScoped<NpgsqlModelDiffer>()
					.AddScoped<NpgsqlDatabase>()
					.AddScoped<NpgsqlMigrationSqlGenerator>()
					.AddScoped<NpgsqlDataStoreCreator>()
					.AddScoped<NpgsqlHistoryRepository>());
			return builder;
        }
    }
}
