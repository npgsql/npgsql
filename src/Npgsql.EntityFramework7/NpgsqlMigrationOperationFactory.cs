//using System;
//using JetBrains.Annotations;
//using Microsoft.Data.Entity.Metadata;
//using Microsoft.Data.Entity.Relational.Migrations.Operations;

//namespace EntityFramework.Npgsql.Extensions
//{
//	public class NpgsqlMigrationOperationFactory : MigrationOperationFactory
//    {
//        public NpgsqlMigrationOperationFactory(
//            [NotNull] NpgsqlMetadataExtensionProvider extensionProvider)
//            : base(extensionProvider)
//        {
//        }

//        public virtual new NpgsqlMetadataExtensionProvider ExtensionProvider
//        {
//            get { return (NpgsqlMetadataExtensionProvider)base.ExtensionProvider; }
//        }

//        public override Column Column(IProperty property)
//        {
//            var column = base.Column(property);

//            // TODO: This is essentially duplicated logic from the selector; combine if possible
//            if (property.GenerateValueOnAdd)
//            {
//                var strategy = property.Npgsql().ValueGenerationStrategy
//                               ?? property.EntityType.Model.Npgsql().ValueGenerationStrategy;

//                if (strategy == NpgsqlValueGenerationStrategy.Identity
//                    || (strategy == null
//                        && property.PropertyType.IsInteger()
//                        && property.PropertyType != typeof(byte)
//                        && property.PropertyType != typeof(byte?)))
//                {
//                    column.IsIdentity = true;
//                }
//            }

//            return column;
//        }

//        public override AddPrimaryKeyOperation AddPrimaryKeyOperation(IKey target)
//        {
//            var operation = base.AddPrimaryKeyOperation(target);
//            var isClustered = ExtensionProvider.Extensions(target).IsClustered;

//            if (isClustered.HasValue)
//            {
//                operation.IsClustered = isClustered.Value;
//            }

//            return operation;
//        }

//        public override CreateIndexOperation CreateIndexOperation(IIndex target)
//        {
//            var operation = base.CreateIndexOperation(target);
//            var isClustered = ExtensionProvider.Extensions(target).IsClustered;

//            if (isClustered.HasValue)
//            {
//                operation.IsClustered = isClustered.Value;
//            }

//            return operation;
//        }
//    }
//}