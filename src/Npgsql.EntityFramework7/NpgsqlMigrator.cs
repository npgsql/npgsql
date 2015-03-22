//using JetBrains.Annotations;
//using Microsoft.Data.Entity.Relational;
//using Microsoft.Data.Entity.Relational.Migrations;
//using Microsoft.Data.Entity.Relational.Migrations.Infrastructure;
//using Microsoft.Framework.Logging;

//namespace EntityFramework.Npgsql.Extensions
//{
//    public class NpgsqlMigrator : Migrator
//    {
//        /// <summary>
//        ///     This constructor is intended only for use when creating test doubles that will override members
//        ///     with mocked or faked behavior. Use of this constructor for other purposes may result in unexpected
//        ///     behavior including but not limited to throwing <see cref="NullReferenceException" />.
//        /// </summary>
//        protected NpgsqlMigrator()
//        {
//        }

//        public NpgsqlMigrator(
//            [NotNull] HistoryRepository historyRepository,
//            [NotNull] MigrationAssembly migrationAssembly,
//            [NotNull] NpgsqlModelDiffer modelDiffer,
//            [NotNull] NpgsqlMigrationOperationSqlGeneratorFactory sqlGeneratorFactory,
//            [NotNull] NpgsqlSqlGenerator sqlGenerator,
//            [NotNull] SqlStatementExecutor sqlStatementExecutor,
//            [NotNull] NpgsqlDataStoreCreator storeCreator,
//            [NotNull] NpgsqlRelationalConnection connection,
//            [NotNull] ILoggerFactory loggerFactory)
//            : base(
//                historyRepository,
//                migrationAssembly,
//                modelDiffer,
//                sqlGeneratorFactory,
//                sqlGenerator,
//                sqlStatementExecutor,
//                storeCreator,
//                connection,
//                loggerFactory)
//        {
//        }
//    }
//}