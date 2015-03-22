using JetBrains.Annotations;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational.Metadata;

namespace EntityFramework.Npgsql.Metadata
{
    public class ReadOnlyNpgsqlForeignKeyExtensions : ReadOnlyRelationalForeignKeyExtensions, INpgsqlForeignKeyExtensions
    {
        protected const string NpgsqlNameAnnotation = NpgsqlAnnotationNames.Prefix + RelationalAnnotationNames.Name;

        public ReadOnlyNpgsqlForeignKeyExtensions([NotNull] IForeignKey foreignKey)
            : base(foreignKey)
        {
        }

        public override string Name
            => ForeignKey[NpgsqlNameAnnotation] as string
               ?? base.Name;
    }
}