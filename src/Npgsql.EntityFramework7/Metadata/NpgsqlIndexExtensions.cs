using JetBrains.Annotations;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Utilities;

namespace EntityFramework.Npgsql.Extensions
{
    public class NpgsqlIndexExtensions : ReadOnlyNpgsqlIndexExtensions
    {
        public NpgsqlIndexExtensions([NotNull] Index index)
            : base(index)
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

                ((Index)Index)[NpgsqlNameAnnotation] = value;
            }
        }

        [CanBeNull]
        public new virtual bool? IsClustered
        {
            get { return base.IsClustered; }
            [param: CanBeNull]
            set
            {
                // TODO: Issue #777: Non-string annotations
                ((Index)Index)[NpgsqlClusteredAnnotation] = value == null ? null : value.ToString();
            }
        }
    }
}