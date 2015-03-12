using JetBrains.Annotations;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Utilities;

namespace EntityFramework.Npgsql.Extensions
{
    public class NpgsqlKeyExtensions : ReadOnlyNpgsqlKeyExtensions
    {
        public NpgsqlKeyExtensions([NotNull] Key key)
            : base(key)
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

                ((Key)Key)[NpgsqlNameAnnotation] = value;
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
                // TODO: Issue #700: Annotate associated index object instead
                ((Key)Key)[NpgsqlClusteredAnnotation] = value == null ? null : value.ToString();
            }
        }
    }
}