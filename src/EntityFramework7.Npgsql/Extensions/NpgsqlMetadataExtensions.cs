using JetBrains.Annotations;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Metadata.Internal;
using Microsoft.Data.Entity.Utilities;

// ReSharper disable once CheckNamespace

namespace Microsoft.Data.Entity
{
    public static class NpgsqlMetadataExtensions
    {
        public static IRelationalEntityTypeAnnotations Npgsql([NotNull] this IEntityType entityType)
            => new RelationalEntityTypeAnnotations(Check.NotNull(entityType, nameof(entityType)), NpgsqlAnnotationNames.Prefix);

        public static RelationalEntityTypeAnnotations Npgsql([NotNull] this EntityType entityType)
            => (RelationalEntityTypeAnnotations)Npgsql((IEntityType)entityType);

        public static IRelationalForeignKeyAnnotations Npgsql([NotNull] this IForeignKey foreignKey)
            => new RelationalForeignKeyAnnotations(Check.NotNull(foreignKey, nameof(foreignKey)), NpgsqlAnnotationNames.Prefix);

        public static RelationalForeignKeyAnnotations Npgsql([NotNull] this ForeignKey foreignKey)
            => (RelationalForeignKeyAnnotations)Npgsql((IForeignKey)foreignKey);

        public static NpgsqlIndexAnnotations Npgsql([NotNull] this IIndex index)
            => new NpgsqlIndexAnnotations(Check.NotNull(index, nameof(index)));

        public static RelationalIndexAnnotations Npgsql([NotNull] this Index index)
            => Npgsql((IIndex)index);

        public static IRelationalKeyAnnotations Npgsql([NotNull] this IKey key)
            => new RelationalKeyAnnotations(Check.NotNull(key, nameof(key)), NpgsqlAnnotationNames.Prefix);

        public static RelationalKeyAnnotations Npgsql([NotNull] this Key key)
            => (RelationalKeyAnnotations)Npgsql((IKey)key);

        public static RelationalModelAnnotations Npgsql([NotNull] this Model model)
            => (RelationalModelAnnotations)Npgsql((IModel)model);

        public static IRelationalModelAnnotations Npgsql([NotNull] this IModel model)
            => new RelationalModelAnnotations(Check.NotNull(model, nameof(model)), NpgsqlAnnotationNames.Prefix);

        public static IRelationalPropertyAnnotations Npgsql([NotNull] this IProperty property)
            => new RelationalPropertyAnnotations(Check.NotNull(property, nameof(property)), NpgsqlAnnotationNames.Prefix);

        public static RelationalPropertyAnnotations Npgsql([NotNull] this Property property)
            => (RelationalPropertyAnnotations)Npgsql((IProperty)property);
    }
}
