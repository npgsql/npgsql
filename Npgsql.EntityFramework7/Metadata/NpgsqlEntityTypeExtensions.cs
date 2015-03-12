using JetBrains.Annotations;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Utilities;

namespace EntityFramework.Npgsql.Extensions
{
    public class NpgsqlEntityTypeExtensions : ReadOnlyNpgsqlEntityTypeExtensions
    {
        public NpgsqlEntityTypeExtensions([NotNull] EntityType entityType)
            : base(entityType)
        {
        }

        public new virtual string Table
        {
            get { return base.Table; }
            [param: CanBeNull]
            set
            {
                Check.NullButNotEmpty(value, "value");

                ((EntityType)EntityType)[NpgsqlTableAnnotation] = value;
            }
        }

        [CanBeNull]
        public new virtual string Schema
        {
            get { return base.Schema; }
            [param: CanBeNull]
            set
            {
                Check.NullButNotEmpty(value, "value");

                ((EntityType)EntityType)[NpgsqlSchemaAnnotation] = value;
            }
        }
    }
}