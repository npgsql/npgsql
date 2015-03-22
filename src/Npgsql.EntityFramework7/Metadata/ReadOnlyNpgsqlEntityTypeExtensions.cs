using JetBrains.Annotations;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational.Metadata;

namespace EntityFramework.Npgsql.Metadata
{
    public class ReadOnlyNpgsqlEntityTypeExtensions : ReadOnlyRelationalEntityTypeExtensions, INpgsqlEntityTypeExtensions
    {
        protected const string NpgsqlTableAnnotation = NpgsqlAnnotationNames.Prefix + RelationalAnnotationNames.TableName;
        protected const string NpgsqlSchemaAnnotation = NpgsqlAnnotationNames.Prefix + RelationalAnnotationNames.Schema;

        public ReadOnlyNpgsqlEntityTypeExtensions([NotNull] IEntityType entityType)
            : base(entityType)
        {
        }

        public override string Table
            => EntityType[NpgsqlTableAnnotation] as string
               ?? base.Table;

        public override string Schema
            => EntityType[NpgsqlSchemaAnnotation] as string
               ?? base.Schema;
    }
}