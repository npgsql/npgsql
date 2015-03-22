using JetBrains.Annotations;
using Microsoft.Data.Entity.Relational.Metadata;

namespace EntityFramework.Npgsql.Metadata
{
    public interface INpgsqlIndexExtensions : IRelationalIndexExtensions
    {
        [CanBeNull]
        bool? IsClustered { get; }
    }
}