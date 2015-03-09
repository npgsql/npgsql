using JetBrains.Annotations;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational.Metadata;

namespace EntityFramework.Npgsql.Extensions
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
        {
            get { return EntityType[NpgsqlTableAnnotation] ?? base.Table; }
        }

        public override string Schema
        {
            get { return EntityType[NpgsqlSchemaAnnotation] ?? base.Schema; }
        }
    }
}