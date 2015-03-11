using EntityFramework.Npgsql.Migrations;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational;
using Microsoft.Data.Entity.Relational.Migrations.History;
using Microsoft.Data.Entity.Relational.Migrations.Infrastructure;
using Microsoft.Data.Entity.Relational.Migrations.Sql;
using Microsoft.Data.Entity.Storage;
using Microsoft.Data.Entity.Utilities;
using Microsoft.Data.Entity.ValueGeneration;

namespace EntityFramework.Npgsql.Extensions
{
	public class NpgsqlDataStoreServices : IRelationalDataStoreServices
    {
		private readonly NpgsqlDataStore _store;
		private readonly NpgsqlDataStoreCreator _creator;
		private readonly NpgsqlEntityFrameworkConnection _connection;
		private readonly NpgsqlValueGeneratorSelector _valueGeneratorSelector;
		private readonly NpgsqlDatabase _database;
		private readonly NpgsqlModelBuilderFactory _modelBuilderFactory;
		private readonly NpgsqlModelSource _modelSource;

		public NpgsqlDataStoreServices(
			[NotNull] NpgsqlDataStore store,
			[NotNull] NpgsqlDataStoreCreator creator,
			[NotNull] NpgsqlEntityFrameworkConnection connection,
			[NotNull] NpgsqlValueGeneratorSelector valueGeneratorSelector,
			[NotNull] NpgsqlDatabase database,
			[NotNull] NpgsqlModelBuilderFactory modelBuilderFactory,
			[NotNull] NpgsqlModelDiffer modelDiffer,
			[NotNull] NpgsqlHistoryRepository historyRepository,
			[NotNull] NpgsqlMigrationSqlGenerator migrationSqlGenerator,
			[NotNull] NpgsqlModelSource modelSource)
		{
			Check.NotNull(store, "store");
			Check.NotNull(creator, "creator");
			Check.NotNull(connection, "connection");
			Check.NotNull(valueGeneratorSelector, "valueGeneratorSelector");
			Check.NotNull(database, "database");
			Check.NotNull(modelBuilderFactory, "modelBuilderFactory");
			Check.NotNull(modelDiffer, nameof(modelDiffer));
			Check.NotNull(historyRepository, nameof(historyRepository));
			Check.NotNull(migrationSqlGenerator, nameof(migrationSqlGenerator));
			Check.NotNull(modelSource, "migrator");

			_store = store;
			_creator = creator;
			_connection = connection;
			_valueGeneratorCache = valueGeneratorCache;
			_database = database;
			_modelBuilderFactory = modelBuilderFactory;
			ModelDiffer = modelDiffer;
			HistoryRepository = historyRepository;
			MigrationSqlGenerator = migrationSqlGenerator;
			_modelSource = modelSource;
		}

		public virtual IDataStore Store => _store;

		public virtual IDataStoreCreator Creator => _creator;

		public virtual IRelationalConnection RelationalConnection => _connection;
		public virtual IDataStoreConnection Connection => _connection;
		
		public virtual IValueGeneratorSelector ValueGeneratorSelector => _valueGeneratorSelector;

		public override Database Database => _database;

		public virtual IModelBuilderFactory ModelBuilderFactory => _modelBuilderFactory;

		public virtual IModelDiffer ModelDiffer { get; }
		public virtual IHistoryRepository HistoryRepository { get; }
		public virtual IMigrationSqlGenerator MigrationSqlGenerator { get; }

		public virtual IModelSource ModelSource => _modelSource;
	}
}
