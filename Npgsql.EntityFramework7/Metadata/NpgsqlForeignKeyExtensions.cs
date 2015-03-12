using JetBrains.Annotations;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Utilities;

namespace EntityFramework.Npgsql.Extensions
{
    public class NpgsqlForeignKeyExtensions : ReadOnlyNpgsqlForeignKeyExtensions
    {
        public NpgsqlForeignKeyExtensions([NotNull] ForeignKey foreignKey)
            : base(foreignKey)
        {
        }

        [CanBeNull]
        public new virtual string Name
        {
            get { return base.Name; }
            [param: CanBeNull]
            set
            {
                Check.NullButNotEmpty(value, "value");

                ((ForeignKey)ForeignKey)[NpgsqlNameAnnotation] = value;
            }
        }
    }
}