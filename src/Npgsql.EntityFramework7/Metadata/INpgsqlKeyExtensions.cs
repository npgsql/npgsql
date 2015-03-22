using JetBrains.Annotations;
using Microsoft.Data.Entity.Relational.Metadata;

namespace EntityFramework.Npgsql.Metadata
{
    public interface INpgsqlKeyExtensions : IRelationalKeyExtensions
    {
        [CanBeNull]
        bool? IsClustered { get; }
    }
}