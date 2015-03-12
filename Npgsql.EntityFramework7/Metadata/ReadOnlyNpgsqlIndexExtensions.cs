using JetBrains.Annotations;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational.Metadata;

namespace EntityFramework.Npgsql.Extensions
{
    public class ReadOnlyNpgsqlIndexExtensions : ReadOnlyRelationalIndexExtensions, INpgsqlIndexExtensions
    {
        protected const string NpgsqlNameAnnotation = NpgsqlAnnotationNames.Prefix + RelationalAnnotationNames.Name;
        protected const string NpgsqlClusteredAnnotation = NpgsqlAnnotationNames.Prefix + NpgsqlAnnotationNames.Clustered;

        public ReadOnlyNpgsqlIndexExtensions([NotNull] IIndex index)
            : base(index)
        {
        }

        public override string Name
        {
            get { return Index[NpgsqlNameAnnotation] ?? base.Name; }
        }

        public virtual bool? IsClustered
        {
            get
            {
                // TODO: Issue #777: Non-string annotations
                var value = Index[NpgsqlClusteredAnnotation];
                return value == null ? null : (bool?)bool.Parse(value);
            }
        }
    }
}