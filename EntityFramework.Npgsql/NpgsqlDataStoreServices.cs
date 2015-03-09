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
using Microsoft.Data.Entity.ValueGeneration.Internal;

namespace EntityFramework.Npgsql.Extensions
{
	public class NpgsqlDataStoreServices : RelationalDataStoreServices
    {
		private readonly NpgsqlDataStore _store;
		private readonly NpgsqlDataStoreCreator _creator;
		private readonly NpgsqlEntityFrameworkConnection _connection;
		private readonly NpgsqlValueGeneratorCache _valueGeneratorCache;
		private readonly NpgsqlDatabase _database;
		private readonly NpgsqlModelBuilderFactory _modelBuilderFactory;
		private readonly NpgsqlModelSource _modelSource;

		public NpgsqlDataStoreServices(
			[NotNull] NpgsqlDataStore store,
			[NotNull] NpgsqlDataStoreCreator creator,
			[NotNull] NpgsqlEntityFrameworkConnection connection,
			[NotNull] NpgsqlValueGeneratorCache valueGeneratorCache,
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
			Check.NotNull(valueGeneratorCache, "valueGeneratorCache");
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

		public override DataStore Store => _store;

		public override DataStoreCreator Creator => _creator;

		public override DataStoreConnection Connection => _connection;

		public override ValueGeneratorCache ValueGeneratorCache => _valueGeneratorCache;

		public override Database Database => _database;

		public override ModelBuilderFactory ModelBuilderFactory => _modelBuilderFactory;

		public override ModelDiffer ModelDiffer { get; }
		public override IHistoryRepository HistoryRepository { get; }
		public override MigrationSqlGenerator MigrationSqlGenerator { get; }

		public override ModelSource ModelSource => _modelSource;
	}
}