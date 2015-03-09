using JetBrains.Annotations;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Utilities;

namespace EntityFramework.Npgsql.Extensions
{
    public static class NpgsqlMetadataExtensions
    {
        public static INpgsqlPropertyExtensions Npgsql([NotNull] this Property property)
        {
            Check.NotNull(property, "property");

            return new NpgsqlPropertyExtensions(property);
        }

        public static INpgsqlPropertyExtensions Npgsql([NotNull] this IProperty property)
        {
            Check.NotNull(property, "property");

            return new ReadOnlyNpgsqlPropertyExtensions(property);
        }

        public static NpgsqlEntityTypeExtensions Npgsql([NotNull] this EntityType entityType)
        {
            Check.NotNull(entityType, "entityType");

            return new NpgsqlEntityTypeExtensions(entityType);
        }

        public static INpgsqlEntityTypeExtensions Npgsql([NotNull] this IEntityType entityType)
        {
            Check.NotNull(entityType, "entityType");

            return new ReadOnlyNpgsqlEntityTypeExtensions(entityType);
        }

        public static NpgsqlKeyExtensions Npgsql([NotNull] this Key key)
        {
            Check.NotNull(key, "key");

            return new NpgsqlKeyExtensions(key);
        }

        public static INpgsqlKeyExtensions Npgsql([NotNull] this IKey key)
        {
            Check.NotNull(key, "key");

            return new ReadOnlyNpgsqlKeyExtensions(key);
        }

        public static NpgsqlIndexExtensions Npgsql([NotNull] this Index index)
        {
            Check.NotNull(index, "index");

            return new NpgsqlIndexExtensions(index);
        }

        public static INpgsqlIndexExtensions Npgsql([NotNull] this IIndex index)
        {
            Check.NotNull(index, "index");

            return new ReadOnlyNpgsqlIndexExtensions(index);
        }

        public static NpgsqlForeignKeyExtensions Npgsql([NotNull] this ForeignKey foreignKey)
        {
            Check.NotNull(foreignKey, "foreignKey");

            return new NpgsqlForeignKeyExtensions(foreignKey);
        }

        public static INpgsqlForeignKeyExtensions Npgsql([NotNull] this IForeignKey foreignKey)
        {
            Check.NotNull(foreignKey, "foreignKey");

            return new ReadOnlyNpgsqlForeignKeyExtensions(foreignKey);
        }

        public static NpgsqlModelExtensions Npgsql([NotNull] this Model model)
        {
            Check.NotNull(model, "model");

            return new NpgsqlModelExtensions(model);
        }

        public static INpgsqlModelExtensions Npgsql([NotNull] this IModel model)
        {
            Check.NotNull(model, "model");

            return new ReadOnlyNpgsqlModelExtensions(model);
        }
    }
}