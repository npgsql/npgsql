//using JetBrains.Annotations;
//using Microsoft.Data.Entity.Metadata;
//using Microsoft.Data.Entity.Relational.Metadata;

//namespace EntityFramework.Npgsql.Extensions
//{
//    public class NpgsqlMetadataExtensionProvider : IRelationalMetadataExtensionProvider
//    {
//        private RelationalNameBuilder _nameBuilder;

//        public virtual INpgsqlModelExtensions Extensions([NotNull] IModel model)
//        {
//            return model.Npgsql();
//        }

//        public virtual INpgsqlEntityTypeExtensions Extensions([NotNull] IEntityType entityType)
//        {
//            return entityType.Npgsql();
//        }

//        public virtual INpgsqlPropertyExtensions Extensions([NotNull] IProperty property)
//        {
//            return property.Npgsql();
//        }

//        public virtual INpgsqlKeyExtensions Extensions([NotNull] IKey key)
//        {
//            return key.Npgsql();
//        }

//        public virtual INpgsqlForeignKeyExtensions Extensions([NotNull] IForeignKey foreignKey)
//        {
//            return foreignKey.Npgsql();
//        }

//        public virtual INpgsqlIndexExtensions Extensions([NotNull] IIndex index)
//        {
//            return index.Npgsql();
//        }

//        public virtual RelationalNameBuilder NameBuilder
//        {
//            get { return _nameBuilder ?? (_nameBuilder = new RelationalNameBuilder(this)); }

//            [param: NotNull]
//            protected set
//            { _nameBuilder = value; }
//        }

//        IRelationalModelExtensions IRelationalMetadataExtensionProvider.Extensions(IModel model)
//        {
//            return Extensions(model);
//        }

//        IRelationalEntityTypeExtensions IRelationalMetadataExtensionProvider.Extensions(IEntityType entityType)
//        {
//            return Extensions(entityType);
//        }

//        IRelationalPropertyExtensions IRelationalMetadataExtensionProvider.Extensions(IProperty property)
//        {
//            return Extensions(property);
//        }

//        IRelationalKeyExtensions IRelationalMetadataExtensionProvider.Extensions(IKey key)
//        {
//            return Extensions(key);
//        }

//        IRelationalForeignKeyExtensions IRelationalMetadataExtensionProvider.Extensions(IForeignKey foreignKey)
//        {
//            return Extensions(foreignKey);
//        }

//        IRelationalIndexExtensions IRelationalMetadataExtensionProvider.Extensions(IIndex index)
//        {
//            return Extensions(index);
//        }
//    }
//}