//using JetBrains.Annotations;
//using Microsoft.Data.Entity.Metadata;
//using Microsoft.Data.Entity.Utilities;

//namespace EntityFramework.Npgsql.Extensions
//{
//    public class NpgsqlMigrationOperationSqlGeneratorFactory : IMigrationOperationSqlGeneratorFactory
//    {
//        private readonly NpgsqlMetadataExtensionProvider _extensionProvider;

//        public NpgsqlMigrationOperationSqlGeneratorFactory(
//            [NotNull] NpgsqlMetadataExtensionProvider extensionProvider)
//        {
//            Check.NotNull(extensionProvider, "extensionProvider");

//            _extensionProvider = extensionProvider;
//        }

//        public virtual NpgsqlMetadataExtensionProvider ExtensionProvider
//        {
//            get { return _extensionProvider; }
//        }

//        public virtual NpgsqlMigrationOperationSqlGenerator Create()
//        {
//            return Create(new Model());
//        }

//        public virtual NpgsqlMigrationOperationSqlGenerator Create([NotNull] IModel targetModel)
//        {
//            Check.NotNull(targetModel, "targetModel");

//            return
//                new NpgsqlMigrationOperationSqlGenerator(
//                    ExtensionProvider,
//                    new NpgsqlTypeMapper())
//                {
//                    TargetModel = targetModel,
//                };
//        }

//        MigrationOperationSqlGenerator IMigrationOperationSqlGeneratorFactory.Create()
//        {
//            return Create();
//        }

//        MigrationOperationSqlGenerator IMigrationOperationSqlGeneratorFactory.Create(IModel targetModel)
//        {
//            return Create(targetModel);
//        }
//    }
//}